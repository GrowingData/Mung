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
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Newtonsoft.Json;


namespace GrowingData.Mung.MetricJs {
	public static class JsValueHelpers {


		public static HashSet<string> GlobalProperties = new HashSet<string>(new string[] {
			"Object", 
			"Function", 
			"Array", 
			"String", 
			"RegExp", 
			"Number", 
			"Boolean", 
			"Date", 
			"Math", 
			"JSON", 
			"Error", 
			"EvalError", 
			"RangeError", 
			"ReferenceError", 
			"SyntaxError", 
			"TypeError", 
			"URIError", 
			"NaN", 
			"Infinity", 
			"undefined", 
			"parseInt", 
			"parseFloat", 
			"isNaN", 
			"isFinite", 
			"decodeURI", 
			"decodeURIComponent", 
			"encodeURI", 
			"encodeURIComponent", 
			"eval"
		});

		public static object Uncast(this JsValue value) {
			if (value.IsBoolean()) {
				return value.AsBoolean();
			}
			if (value.IsNumber()) {
				return value.AsNumber();
			}
			if (value.IsObject()) {
				return value.AsObject();
			}
			if (value.IsString()) {
				return value.AsString();
			}

			return null;
		}


		//public static string ToJson(this JsValue value, Engine context) {
		//	return context.Json.Stringify(value, new[] { value }).AsString();
		//}
	}
}
