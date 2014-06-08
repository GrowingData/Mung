using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;
using GrowingData.Mung.Core;
using Jint;
using Jint.Parser;
using Jint.Runtime;
using Jint.Native;
using Jint.Native.Json;
using Jint.Native.Object;
using GrowingData.Mung.Core;



namespace GrowingData.Mung.MetricJs {
	public class PeriodMetric {
		public static string BaseKey(string name, TimePeriod period, string filter) {
			return string.Format("persistent-metric|{0}|{1}|=>{2}", name, period.ToString(), filter);
		}

		private JavascriptContext _context;
		private DateTime _currentPeriod = DateTime.MinValue;

		private TimePeriod _period;
		protected string _name;
		protected string _filter;


		public string ValuesKey {
			get {
				return BaseKey(_name, _period, _filter);
			}
		}
		public string ContextKey {
			get {
				return BaseKey(_name, _period, _filter) + "|context";
			}
		}
		public string ContextDateKey {
			get {
				return BaseKey(_name, _period, _filter) + "|date";
			}
		}



		public PeriodMetric(string name, string jsAggregator, string filter, TimePeriod period) {
			_name = name;
			_filter = filter;
			_period = period;

			var javascriptErrorMessageContext = string.Format("{0} ({1})", _name, "Metric Aggregator");

			_context = new JavascriptContext(jsAggregator, javascriptErrorMessageContext);

			Initialize();
		}


		public void Initialize() {
			var saved = LoadResultContext();

			if (saved != null) {

				_context.ExecuteFunction("init", new[] { saved });
			}
		}




		public double ProcessEvent(string evtJson) {
			// Should we accumulate this metric for the current period, or should
			// we reset the accumulator for the new period.

			var currentPeriod = _period.StandardizeDate(DateTime.UtcNow);

			if (currentPeriod != _currentPeriod) {
				if (_currentPeriod != DateTime.MinValue) {
					// Get the terminated value for this period
					JsValue terminationJsValue = _context.ExecuteFunction("terminate");
					var terminationValue = terminationJsValue.AsNumber();
					// Store it in the database for this period / TimePeriod
					StoreResultValue(_currentPeriod, terminationValue);

					// Reset the current value
					_context.ExecuteFunction("reset");
				}
				_currentPeriod = currentPeriod;
			}

			JsValue v = _context.ExecuteFunction("accumulate", new string[] { evtJson });
			var value = v.AsNumber();


			// Update the "current" context we have for this value, so if the process
			// restarts, it can restore its state.
			StoreResultContext();

			return value;
		}


		public void StoreResultValue(DateTime period, double value) {
			var score = (double)period.Ticks;
			SortedSetEntry sse = new SortedSetEntry(value, score);
			RedisClient.Current.Database.SortedSetAdd(ValuesKey, new[] { sse });
		}

		public void StoreResultContext() {
			JsValue v = _context.ExecuteFunction("save", new string[] { });
			var json = _context.ToJson(v);
			RedisClient.Current.Database.StringSet(ContextKey, json);
			RedisClient.Current.Database.StringSet(ContextDateKey, _currentPeriod.ToString("o"));

		}

		public string LoadResultContext() {
			// Only load the actual context if the period is still active.
			var strDate = RedisClient.Current.Database.StringGet(ContextDateKey);
			DateTime contextDate = DateTime.MinValue;
			if (DateTime.TryParse(strDate, out contextDate)) {
				var currentPeriod = _period.StandardizeDate(DateTime.UtcNow);
				if (contextDate == currentPeriod) {
					string json = RedisClient.Current.Database.StringGet(ContextKey);
					return json;
				}
			}
			return null;
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
