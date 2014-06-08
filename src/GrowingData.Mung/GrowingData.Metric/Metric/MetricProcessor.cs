using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using GrowingData.Mung.Core;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace GrowingData.Mung.Metric {

	public class MetricProcessor : EventProcessor {

		public MetricState CurrentState;
		public string Name;
		public JsonMetric Metric;
		private bool _stateIsDirty = false;
		private bool _clientNeedData = false;

		public ConcurrentDictionary<string, Action<MetricProcessor>> _updateCallbacks;
		public ConcurrentDictionary<string, Action<string>> _errorCallbacks;

		public MetricProcessor(string name, JsonMetric jsonMetric)
			: base(name) {

			Name = name;
			Metric = jsonMetric;

			CurrentState = LoadState();

			_updateCallbacks = new ConcurrentDictionary<string, Action<MetricProcessor>>();
			_errorCallbacks = new ConcurrentDictionary<string, Action<string>>();

			Task.Run(() => { EventLoop(); });
		}


		public void Updated(string connectionId, Action<MetricProcessor> callback) {
			_updateCallbacks[connectionId] = callback;
		}
		public void Error(string connectionId, Action<string> callback) {
			_errorCallbacks[connectionId] = callback;
		}

		public void RemoveUpdated(string connectionId) {
			Action<MetricProcessor> output = null;
			while (!_updateCallbacks.TryRemove(connectionId, out output)) {
				Thread.Sleep(10);
			}
		}

		protected override void ProcessEvent(MungServerEvent evt) {
			try {
				CurrentState.CheckIsCurrent();

				if (CurrentState.ProcessEvent(evt)) {
					// Let our loop know that we need to send data to the client.
					_clientNeedData = true;

					// Let our event loop know that we need to update redis.
					_stateIsDirty = true;
				}
			} catch (Exception ex) {
				foreach (var callback in _errorCallbacks.Values) {
					callback(ex.Message + "\r\n" + ex.StackTrace);
				}
			}
		}

		public void EventLoop() {
			while (true) {
				if (_stateIsDirty) {
					StoreState();
				}
				if (_clientNeedData) {
					ExecuteCallbacks();
				}

				Thread.Sleep(1000);
			}
		}

		public void ExecuteCallbacks() {
			foreach (var callback in _updateCallbacks.Values) {
				try {
					callback(this);
				} catch (Exception ex) {

				}
			}
			_clientNeedData = false;
		}

		public string MetricSortedListKey { get { return string.Format("metric-state|{0}|", Name); } }

		public string StateKey(DateTime time) {
			return string.Format("{0}|{1}", MetricSortedListKey, MetricState.PeriodTime(Metric.Period, time));
		}
		public string StateKey() {
			return string.Format("{0}|{1}", MetricSortedListKey, MetricState.PeriodTime(Metric.Period));
		}

		public void StoreState() {
			try {
				var key = StateKey();
				// Save the actual state...
				RedisClient.Current.Database.StringSet(key, Serialize(CurrentState));


				// Now save our sorted list of keys, so we can easily iterate over
				// them by date to generate lists of values for charts and such
				var score = (double)CurrentState.AsOf.Ticks;
				SortedSetEntry sse = new SortedSetEntry(key, score);

				RedisClient.Current.Database.SortedSetAdd(MetricSortedListKey, new[] { sse });

				_stateIsDirty = false;
			} catch (Exception ex) {

			}
		}


		public MetricState LoadState() {
			while (RedisClient.Current.Database == null) {
				Thread.Sleep(10);
			}

			var state = GetState(MetricState.PeriodTime(Metric.Period));
			if (state != null) {
				return state;
			} else {
				return new MetricState(Name, Metric);
			}

		}

		public MetricState GetState(DateTime periodTime) {
			while (RedisClient.Current.Database == null) {
				Thread.Sleep(10);
			}

			var key = StateKey(periodTime);

			byte[] data = RedisClient.Current.Database.StringGet(key);
			var state = Deserialize(data);
			return state;

		}

		static byte[] Serialize(MetricState o) {
			if (o == null) {
				return null;
			}

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream()) {
				binaryFormatter.Serialize(memoryStream, o);
				byte[] objectDataAsStream = memoryStream.ToArray();
				return objectDataAsStream;
			}
		}

		static MetricState Deserialize(byte[] stream) {
			if (stream == null) {
				return null;
			}

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream(stream)) {
				MetricState result = (MetricState)binaryFormatter.Deserialize(memoryStream);
				return result;
			}
		}

	}

}
