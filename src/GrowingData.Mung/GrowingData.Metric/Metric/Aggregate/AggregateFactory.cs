using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {
	public class AggregateFactory {
		public static AggregateFactory Default = new AggregateFactory();

		private Dictionary<string, Func<JsonMetric, Aggregate>> _aggregates =
			new Dictionary<string, Func<JsonMetric, Aggregate>>(){

			{"Average", (m) =>  new AggregateAverage(m, "Average") },
			{"Count", (m) =>  new AggregateCount(m, "Count") },
			{"Sum", (m) =>  new AggregateSum(m, "Sum") }
		};

		public void AddAggregate(string name, Func<JsonMetric, Aggregate> constructor) {
			_aggregates.Add(name, constructor);
		}

		public Aggregate Get(JsonMetric metric) {
			foreach (var k in _aggregates.Keys) {
				if (metric.Aggregate.StartsWith(k)) {
					return _aggregates[k](metric);
				}
			}

			throw new Exception(string.Format("Unable to fidn aggregate for expression: {0}", metric.Aggregate));

		}



	}
}
