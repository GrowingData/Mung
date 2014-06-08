using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {

	[Serializable]
	public abstract class Aggregate {

		public string Name;

		public string Expression;
		public JsonMetric Metric;

		public Aggregate() { }

		public Aggregate(JsonMetric metric, string name) {
			Metric = metric;
			Name = name;
			if (!metric.Aggregate.StartsWith(Name)) {
				throw new Exception(string.Format("Aggregate does not match name, name: {0}, Aggregate: {1}", Name, metric.Aggregate));
			}
			Expression = metric.Aggregate.Substring(Name.Length + 1, metric.Aggregate.Length - (Name.Length + 2));
		}

		public bool Filter(MungServerEvent evt) {
			if (!string.IsNullOrEmpty(Metric.EventType) && Metric.EventType != evt.Type) {
				return false;
			}
			if (string.IsNullOrEmpty(Metric.Filter)) {
				return true;
			}

			return JsonExpression.IsTrue(evt.Token, Metric.Filter);
		}

		public double EvaluateNumeric(MungServerEvent evt) {
			return EvaluateNumeric(evt, Expression);
		}
		public double EvaluateNumeric(MungServerEvent evt, string expression) {
			return JsonExpression.EvalulateNumber(evt.Token, expression);

		}


		public abstract double Accumulate(MungServerEvent evt);
		public abstract double Terminate();
		public abstract void Reset();
		public abstract void Merge(Aggregate other);

	}
}
