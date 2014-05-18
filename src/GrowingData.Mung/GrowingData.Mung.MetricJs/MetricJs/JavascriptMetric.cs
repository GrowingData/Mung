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



namespace GrowingData.Mung.MetricJs {
	public class JavascriptMetric : EventProcessor {

		private JavascriptContext _context;
		private string _javascript;
		Dictionary<string, PeriodMetric> _aggregators = new Dictionary<string, PeriodMetric>();
		string[] _dimensions;

		public static TimePeriod[] ReportingPeriods = new TimePeriod[] {
				  TimePeriod.Minutes,
				  TimePeriod.Hours,
				  TimePeriod.Days,
				  TimePeriod.Months
		};


		private ConcurrentDictionary<string, Action<List<JavascriptMetricUpdate>>> _updateCallbacks;

		public JavascriptMetric(string name, string jsAggregator)
			: base(name) {
			_javascript = jsAggregator;
			_aggregators = new Dictionary<string, PeriodMetric>();

			var javascriptErrorMessageContext = string.Format("{0} ({1})", name, "Metric Filter");
			_context = new JavascriptContext(jsAggregator, javascriptErrorMessageContext);

			try {
				var dimensions = _context.ExecuteFunction("dimensions", new string[] { });
				if (dimensions.IsArray()) {
					var ad = dimensions.AsArray();
					var length = (int)ad.Properties["length"].Value.Value.AsNumber();

					_dimensions = new string[length];

					foreach (var prop in ad.Properties) {
						int index = 0;
						if (int.TryParse(prop.Key, out index)) {
							_dimensions[index] = prop.Value.Value.Value.AsString();
						}
					}

				}
			} catch (Exception ex) {
				Debug.WriteLine("Unable to load dimensions for metric: '{0}'.  {1}", Name, ex.Message);
			}

			_updateCallbacks = new ConcurrentDictionary<string, Action<List<JavascriptMetricUpdate>>>();

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
In: {4}", Name,
			_context.JavaScript);

				throw new Exception(details);
			}

			return v.AsBoolean();

		}

		public void Updated(string connectionId, Action<List<JavascriptMetricUpdate>> callback) {
			_updateCallbacks[connectionId] = callback;
		}

		public void RemoveUpdated(string connectionId) {
			Action<List<JavascriptMetricUpdate>> output = null;
			while (!_updateCallbacks.TryRemove(connectionId, out output)) {
				Thread.Sleep(10);
			}
		}

		protected override void ProcessEvent(MungServerEvent evt) {
			var json = evt.Token.ToString();
			if (PassesFilter(json)) {
				var updates = new List<JavascriptMetricUpdate>();
				foreach (var filter in MetricFilter.Filters(evt.Data, _dimensions)) {
					Console.WriteLine("Filter: {0}", filter);
					Dictionary<string, double> values = new Dictionary<string, double>();
					foreach (var period in ReportingPeriods) {

						var key = PeriodMetric.BaseKey(Name, period, filter);
						if (!_aggregators.ContainsKey(key)) {
							var agg = new PeriodMetric(Name, _javascript, filter, period);
							_aggregators[key] = agg;
						}

						_aggregators[key].ProcessEvent(json);

					}
					updates.Add(new JavascriptMetricUpdate() {
						Periods = values,
						Filter = filter
					});

				}

				// Do all the callbacks in the background
				Task.Run(() => {
					foreach (var callback in _updateCallbacks.Values) {
						callback(updates);
					}
				});

			}
		}


		public void StoreResult(TimePeriod period, double value) {
			var key = string.Format("persistent-metric|{0}|{1}", Name, period.ToString());
			var score = (double)period.StandardizeDate(DateTime.UtcNow).Ticks;
			SortedSetEntry sse = new SortedSetEntry(value, score);
			RedisClient.Current.Database.SortedSetAdd(key, new[] { sse });
		}


	}

	public class JavascriptMetricUpdate {
		public Dictionary<string, double> Periods;
		public string Filter;
	}
}
