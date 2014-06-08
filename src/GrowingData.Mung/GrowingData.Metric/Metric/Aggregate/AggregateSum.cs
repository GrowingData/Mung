using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {
	[Serializable]
	public class AggregateSum : Aggregate {

		public double Sum = 0;


		public AggregateSum() { }
		public AggregateSum(JsonMetric metric, string name)
			: base(metric, name) {
		}

		public override double Accumulate(MungServerEvent evt) {
			Sum += EvaluateNumeric(evt);
			return Terminate();
		}

		public override double Terminate() {
			return Sum ;
		}

		public override void Reset() {
			Sum = 0;
		}

		public override void Merge(Aggregate other) {
			var avg = other as AggregateSum;
			Sum += avg.Sum;
		}
	}
}
