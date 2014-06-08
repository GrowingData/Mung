using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NCalc;

namespace GrowingData.Mung.Metric {

	public class JsonExpressionException : Exception {
		public JsonExpressionException(JToken token, string expression, string message) :
			base(string.Format(@"Unable to execute expression: {0} on {1}. {2}.", expression, token.ToString(), message)) {

		}

	}

	public static class JsonExpression {

		public static object Evalulate(JToken token, string expression) {
			var e = new Expression(expression);

			e.EvaluateParameter += (name, args) => {
				args.Result = GetValue(token, name);
				args.HasResult = true;
			};

			return e.Evaluate();
		}
		public static double EvalulateNumber(JToken token, string expression) {
			var r = Evalulate(token, expression);
			if (r == null) {
				throw new JsonExpressionException(token, expression, "The expression path could not be found in the json");
			}

			if (r.GetType() == typeof(int)) {
				return (double)(int)r;
			}
			if (r.GetType() == typeof(double)) {
				return (double)r;
			}
			if (r.GetType() == typeof(float)) {
				return (double)(float)r;
			}
			if (r.GetType() == typeof(decimal)) {
				return (double)(decimal)r;
			}


			throw new JsonExpressionException(token, expression, string.Format("Unable to convert result to number. Type: {0}", r.GetType()));

		}
		public static bool IsTrue(JToken token, string expression) {

			var r = Evalulate(token, expression);
			if (r.GetType() == typeof(bool)) {
				return (bool)r;
			}

			throw new JsonExpressionException(token, expression, string.Format("Unable to convert result to boolean. Type: {0}", r.GetType()));

		}

		public static object GetValue(JToken token, string path) {

			var selected = token.SelectToken(path);
			if (selected == null) {
				return null;
			}

			if (selected.Type == JTokenType.Boolean) {
				return selected.Value<bool>();
			}
			if (selected.Type == JTokenType.Date) {
				return selected.Value<DateTime>();
			}
			if (selected.Type == JTokenType.Float) {
				return selected.Value<double>();
			}
			if (selected.Type == JTokenType.Integer) {
				return selected.Value<int>();
			}
			if (selected.Type == JTokenType.Null) {
				return null;
			}
			if (selected.Type == JTokenType.String) {
				return selected.Value<string>();
			}
			throw new JsonExpressionException(token, path, "Unable to get base types value from path.");

		}
	}
}
