using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using GrowingData.Mung.Core;
using Newtonsoft.Json;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Native.Object;



namespace GrowingData.Mung.MetricJs {
	public class JavascriptException : Exception {
		public JavascriptException(string message, Exception inner)
			: base(message, inner) {

		}
	}

	public class JavascriptContext {
		protected object _sync = new object();
		private string _errorMessageContext;
		private string _initialJavascript;

		private Jint.Engine _engine;

		private ObjectInstance mng;
		public string JavaScript { get { return _initialJavascript; } }


		public JavascriptContext(string script, string errorMessageContext) {
			_engine = new Engine();
			_errorMessageContext = errorMessageContext;
			_initialJavascript = script;



			Initialize(_initialJavascript);

		}
		public string ToJson(JsValue value) {
			var json = _engine.Json.Stringify(value, new[] { value });
			if (json.IsString()) {
				return json.AsString();
			}
			return null;
		}
		public JsValue FromJson(string json) {
			var jj = new JsValue(json);
			return _engine.Json.Parse(jj, new[] { jj });

		}

		public void Initialize(string javascript) {
			lock (_sync) {
				try {
					// Define the aggregator
					mng = _engine.Execute(javascript).GetCompletionValue().AsObject();

				} catch (Exception ex) {
					var details = string.Format(@"Error, unable to execute initial script:
Javascript
------
{0}
------
Message: {1}
In: {2}

The script needs to follow the pattern: 

E.g. `(function () { return this; })();`
",
								javascript,
								ex.Message,
								_errorMessageContext + ":" + "Initialize"
						);
					throw new JavascriptException(details, ex);
				}
			}
		}
		public JsValue ExecuteFunction(string name) {
			return ExecuteFunction(name, new string[] { });
		}

		public JsValue ExecuteFunction(string name, string[] arguments) {
			var parameters = ParseParameters(name, arguments);
			var fn = GetFunction(name);

			lock (_sync) {
				try {
					JsValue result = fn.Invoke(parameters);

					System.Diagnostics.Debug.WriteLine("ExecuteFn: {0} => {1} ToString", name, ToJson(result));
					return result;

				} catch (Exception ex) {
					var details = string.Format(@"Error, unable to execute function '{0}':
Parameters:
------
{1}
------

Initial script Javascript:
------
{2}
------

Message: {3}
In: {4}
",
						name,
						string.Join("\r\n", arguments.Select((x, i) => string.Format("\t [{0}]: {1}", i, x))),
						_initialJavascript,
						ex.Message,
						_errorMessageContext + ":" + "Execute function");

					throw new JavascriptException(details, ex);
				}
			}
		}

		public JsValue GetFunction(string name) {
			JsValue fn;
			try {
				fn = mng.Get(name);

				if (fn.IsUndefined()) {
					throw new Exception(string.Format("Function '{0}' is undefined.", name));
				}
				if (fn.IsNull()) {
					throw new Exception(string.Format("Function '{0}' is null.", name));
				}
				if (!fn.IsObject()) {
					throw new Exception(string.Format("Function '{0}' is not a function.", name));
				}
				return fn;
			} catch (Exception ex) {
				var details = string.Format(@"Error, unable to find function '{0}':
Initial script Javascript:
------
{1}
------
Message: {2}
In: {3}
",
				name,
				_initialJavascript,
				ex.Message,
				_errorMessageContext + ":" + "Execute function");

				throw new JavascriptException(details, ex);
			}
		}

		public JsValue[] ParseParameters(string name, string[] arguments) {
			List<JsValue> parameters = new List<JsValue>();

			int parameterNumber = 0;
			foreach (var json in arguments) {
				try {
					var input = new JsonParser(_engine).Parse(json);
					parameters.Add(input);
					parameterNumber++;
				} catch (Exception ex) {
					var details = string.Format(@"Error, unable to parse parameter '{0}':
Parameter JSON {0}:
------
{1}
------
Message: {2}
Function target: {3}
In: {4}
",
							parameterNumber,
							json,
							ex.Message,
							name,
							_errorMessageContext + ":" + "Parse parameters"
					);
					throw new JavascriptException(details, ex);
				}
			}

			return parameters.ToArray();
		}

	}
}
