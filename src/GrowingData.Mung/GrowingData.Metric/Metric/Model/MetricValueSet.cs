using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {
	[Serializable]
	public class MetricValueSet{
		public List<MetricValue> Values;
		public JsonMetric Metric;
		public DateTime At;

	}


}
