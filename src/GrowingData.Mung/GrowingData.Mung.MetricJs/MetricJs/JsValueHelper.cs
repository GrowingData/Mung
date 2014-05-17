using System;
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



namespace GrowingData.Mung.MetricJs {
	public static class JsValueHelpers {


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
		public static string ToJson(this JsValue value) {
			var o = Uncast(value);
			return Newtonsoft.Json.JsonConvert.SerializeObject(o);
		}
	}
}
