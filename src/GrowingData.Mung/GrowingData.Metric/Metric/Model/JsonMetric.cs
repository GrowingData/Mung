using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {
	[Serializable]
	public class JsonMetric {
		public string Name;

		public string EventType;
		public string Aggregate;
		public string Filter;
		public string Group;
		public TimePeriod Period;
	}
}
