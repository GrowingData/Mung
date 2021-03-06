﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {
	[Serializable]
	public class AggregateAverage : Aggregate {

		public int Count = 0;
		public double Sum = 0;


		public AggregateAverage(JsonMetric metric, string name)
			: base(metric, name) {
		}

		public override double Accumulate(MungServerEvent evt) {
			Count++;
			Sum += EvaluateNumeric(evt);
			return Terminate();
		}

		public override double Terminate() {
			return Sum / Count;
		}

		public override void Reset() {
			Sum = 0;
			Count = 0;
		}

		public override void Merge(Aggregate other) {
			var avg = other as AggregateAverage;
			Sum += avg.Sum;
			Count += avg.Count;
		}
	}
}
