using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {
	[Serializable]
	public class AggregateCount : Aggregate {

		public int Count = 0;


		public AggregateCount(JsonMetric metric, string name)
			: base(metric, name) {
		}

		public override double Accumulate(MungServerEvent evt) {
			Count++;
			return Terminate();
		}

		public override double Terminate() {
			return Count;
		}

		public override void Reset() {
			Count = 0;
		}

		public override void Merge(Aggregate other) {
			var avg = other as AggregateCount;
			Count += avg.Count;
		}
	}
}
