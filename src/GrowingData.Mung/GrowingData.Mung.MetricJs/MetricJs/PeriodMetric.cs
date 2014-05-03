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
using Jint;
using Jint.Native;
using Jint.Native.Object;
using GrowingData.Mung.Core;



namespace GrowingData.Mung.MetricJs {
	public class PeriodMetric {
		protected object _sync = new object();

		private Jint.Engine _engine;

		private JsValue _filter;
		private JsValue _accumulate;
		private JsValue _calculate;
		private JsValue _initialize;

		private string _jsAggregator;
		private TimePeriod _period;
		protected string _name;

		public string Key {
			get {
				return string.Format("persistent-metric|{0}|{1}", _name, _period.ToString());
			}
		}

		private ConcurrentDictionary<string, Action<object>> _updateCallbacks;

		public PeriodMetric(string name, string jsAggregator, TimePeriod period) {
			_name = name;
			_jsAggregator = jsAggregator;
			_period = period;

			Initialize();
		}

		public bool PassesFilter(MungServerEvent evt) {
			bool answer = false;
			lock (_sync) {
				JsValue v = JsValue.FromObject(_engine, evt);
				answer = _filter.Invoke(v).AsBoolean();
			}
			return answer;
		}

		public void Initialize() {
			lock (_sync) {
				_engine = new Engine();

				var script = "var __aggregator__ = " + _jsAggregator + "()";

				// Define the aggregator
				_engine.Execute(script);

				// Get the initialize and accumulator
				_accumulate = _engine.GetValue("accumulate");
				_calculate = _engine.GetValue("calculate");
				_initialize = _engine.GetValue("init");

				InitializeValues();
			}
		}




		public double ProcessEvent(MungServerEvent evt) {
			JsValue result;
			lock (_sync) {
				JsValue v = JsValue.FromObject(_engine, evt);
				_accumulate.Invoke(v);
				result = _calculate.Invoke(v);
			}
			var val = (double)UncastJsValue(result);

			StoreResult(val);

			return val;


		}


		public void InitializeValues() {
			var score = (double)_period.StandardizeDate(DateTime.UtcNow).Ticks;
			var values = RedisClient.Current.Database.SortedSetRangeByScore(Key, start: score, stop: score);

			if (values.Length == 1) {

				_initialize.Invoke(new JsValue(values[0]));
			}
		}

		public void StoreResult(double value) {
			var score = (double)_period.StandardizeDate(DateTime.UtcNow).Ticks;
			SortedSetEntry sse = new SortedSetEntry(value, score);
			RedisClient.Current.Database.SortedSetAdd(Key, new[] { sse });
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
}
