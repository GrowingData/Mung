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



namespace GrowingData.Mung.MetricJs {
	public class PeriodMetric {

		private JavascriptContext _context;

		private TimePeriod _period;
		protected string _name;


		public string ValuesKey {
			get {
				return string.Format("persistent-metric|{0}|{1}", _name, _period.ToString());
			}
		}
		public string ContextKey {
			get {
				return string.Format("persistent-metric|{0}|{1}-context", _name, _period.ToString());
			}
		}



		public PeriodMetric(string name, string jsAggregator, TimePeriod period) {
			_name = name;
			var javascriptErrorMessageContext = string.Format("{0} ({1})", _name, "Metric Aggregator");
			_context = new JavascriptContext(jsAggregator, javascriptErrorMessageContext);
			_period = period;

			Initialize();
		}

		public bool PassesFilter(string evtJson) {
			JsValue v = _context.ExecuteFunction("filter", new string[] { evtJson });

			if (!v.IsBoolean()) {

				var details = string.Format(@"Error, unable to test filter for {0}, filter did not return a boolean value:
Initial script Javascript:
------
{1}
------

Message: {3}
In: {4}", _name,
			_context.JavaScript);

				throw new Exception(details);
			}

			return v.AsBoolean();

		}

		public void Initialize() {
			var saved = LoadResultContext();

			if (saved != null) {
				_context.ExecuteFunction("init", new string[] { saved });
			}
		}




		public double ProcessEvent(string evtJson) {
			JsValue v = _context.ExecuteFunction("accumulate", new string[] { evtJson });
			var value = v.AsNumber();
			StoreResultValue(value);

			return value;
		}


		public void StoreResultValue(double value) {
			var score = (double)_period.StandardizeDate(DateTime.UtcNow).Ticks;
			SortedSetEntry sse = new SortedSetEntry(value, score);
			RedisClient.Current.Database.SortedSetAdd(ValuesKey, new[] { sse });
		}

		public void StoreResultContext() {
			JsValue v = _context.ExecuteFunction("save", new string[] { });
			var json = JsonConvert.SerializeObject(v);
			RedisClient.Current.Database.StringSet(ContextKey, json);

		}

		public string LoadResultContext() {
			string json = RedisClient.Current.Database.StringGet(ContextKey);
			return json;
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
