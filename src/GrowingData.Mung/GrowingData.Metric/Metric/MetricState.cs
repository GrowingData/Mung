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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace GrowingData.Mung.Metric {

	[Serializable]
	public class MetricState {

		public Dictionary<string, Aggregate> Aggregates;
		public JsonMetric Metric;
		public string[] Dimensions;
		public Aggregate Total;
		public DateTime AsOf;

		public object Sync = new object();

		public MetricState() {

		}

		public static DateTime PeriodTime(TimePeriod period, DateTime time) {
			return period.StandardizeDate(time);
		}

		public static DateTime PeriodTime(TimePeriod period) {
			return PeriodTime(period, DateTime.UtcNow);
		}

		public MetricValueSet Values {
			get {
				var current = new MetricValueSet() {
					Metric = Metric,
					At = AsOf,
					Values = new List<MetricValue>()
				};

				foreach (var agg in Aggregates) {
					current.Values.Add(new MetricValue() {
						Value = agg.Value.Terminate(),
						Group = agg.Key
					});
				}

				return current;
			}
		}



		public MetricState(string name, JsonMetric jsonMetric) {

			Metric = jsonMetric;
			if (string.IsNullOrEmpty(Metric.Group)) {
				Dimensions = new string[] { };
			} else {
				Dimensions = Metric.Group.Split(',');
			}
			Aggregates = new Dictionary<string, Aggregate>();

			Total = AggregateFactory.Default.Get(Metric);
			AsOf = MetricState.PeriodTime(Metric.Period);

		}

		public void CheckIsCurrent() {
			lock (Sync) {
				var at = Metric.Period.StandardizeDate(DateTime.UtcNow);
				if (AsOf != at) {
					foreach (var aggregate in Aggregates) {
						aggregate.Value.Reset();
					}
					Total.Reset();

					AsOf = at;
				}
			}
		}

		/// <summary>
		/// Processes the event, returning true if it passes the metrics filter
		/// and thus updates a value, or false if it doesn't pass the filter.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public bool ProcessEvent(MungServerEvent evt) {
			lock (Sync) {
				if (!Total.Filter(evt)) {
					return false;
				}
				var at = Metric.Period.StandardizeDate(DateTime.UtcNow);

				var updates = new List<MetricValue>();

				foreach (var filter in FilterBuilder.Filters(evt.Data, Dimensions)) {
					Console.WriteLine("Filter: {0}", filter);
					Dictionary<string, double> values = new Dictionary<string, double>();

					if (!Aggregates.ContainsKey(filter)) {
						var agg = AggregateFactory.Default.Get(Metric); ;
						Aggregates[filter] = agg;
					}

					var value = Aggregates[filter].Accumulate(evt);

				}
			}
			return true;
		}

	}

}
