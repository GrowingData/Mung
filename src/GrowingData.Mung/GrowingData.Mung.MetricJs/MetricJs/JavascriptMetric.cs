using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using GrowingData.Mung.Core;
using Jint;
using Jint.Native;
using Jint.Native.Object;



namespace GrowingData.Mung.MetricJs {
	public class JavascriptMetric : EventProcessor {

		Dictionary<TimePeriod, PeriodMetric> _periods = new Dictionary<TimePeriod, PeriodMetric>();

		public static TimePeriod[] ReportingPeriods = new TimePeriod[] {
				  TimePeriod.Minutes,
				  TimePeriod.Hours,
				  TimePeriod.Days,
				  TimePeriod.Months
		};

		public PeriodMetric Default { get { return _periods[TimePeriod.Days]; } }

		private ConcurrentDictionary<string, Action<JaascriptMetricUpdate>> _updateCallbacks;

		public JavascriptMetric(string name, string jsAggregator)
			: base(name) {

			_periods = new Dictionary<TimePeriod, PeriodMetric>();

			foreach (var p in ReportingPeriods) {

				_periods[p] = new PeriodMetric(name, jsAggregator, p);

			}

			_updateCallbacks = new ConcurrentDictionary<string, Action<JaascriptMetricUpdate>>();

		}


		public void Updated(string connectionId, Action<JaascriptMetricUpdate> callback) {
			_updateCallbacks[connectionId] = callback;
		}

		public void RemoveUpdated(string connectionId) {
			Action<JaascriptMetricUpdate> output = null;
			while (!_updateCallbacks.TryRemove(connectionId, out output)) {
				Thread.Sleep(10);
			}
		}





		protected override void ProcessEvent(MungServerEvent evt) {
			var json = evt.Token.ToString();
			if (Default.PassesFilter(json)) {
				Dictionary<string, double> values = new Dictionary<string, double>();



				foreach (var m in _periods) {
					values[m.Key.ToString()] = m.Value.ProcessEvent(json);
				}



				foreach (var callback in _updateCallbacks.Values) {
					callback(new JaascriptMetricUpdate() {
						Periods = values
					});
				}
			}
		}


		public void StoreResult(TimePeriod period, double value) {
			var key = string.Format("persistent-metric|{0}|{1}", Name, period.ToString());
			var score = (double)period.StandardizeDate(DateTime.UtcNow).Ticks;
			SortedSetEntry sse = new SortedSetEntry(value, score);
			RedisClient.Current.Database.SortedSetAdd(key, new[] { sse });
		}


		public object UncastJsValue(JsValue value) {
			if (value.IsBoolean()) {
				return value.AsBoolean();
			}
			if (value.IsNumber()) {
				return value.AsNumber();
			}
			if (value.IsObject()) {
				return value.AsObject();
			}
			if (value.IsString()) {
				return value.AsString();
			}

			return null;
		}

	}

	public class JaascriptMetricUpdate {
		public Dictionary<string, double> Periods;
	}
}
