using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using GrowingData.Mung.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;



namespace GrowingData.Mung.Core {

	/// <summary>
	/// Builds 'filters' out of Json objects, eg:
	///		new { country = "au", state = "vic", count = 100 }
	///	
	/// Would be represented as:
	///		country=au&state=vic
	///	
	/// 
	/// </summary>
	public class FilterBuilder {
	
		public static IEnumerable<string> Filters(JToken token, string[] dimensions) {
		return Filters(string.Empty, token, dimensions);
		}
		public static IEnumerable<string> Filters(string prefix, JToken token, string[] dimensions) {
			if (dimensions.Count() == 0) {
				yield return prefix;
			} else {

				var keys = new List<KeyValuePair<string, string>>();
				var d = dimensions.First();

				//foreach (var d in dimensions) {
				var withValues = (string.IsNullOrEmpty(prefix) ? "" : prefix + "&") + FilterPortion(d, token[d].ToString());
				var withoutValues = prefix;

				foreach (var f in Filters(withValues, token, dimensions.Skip(1).ToArray())) {
					yield return f;
				}

				foreach (var f in Filters(withoutValues, token, dimensions.Skip(1).ToArray())) {
					yield return f;
				}

				//}
			}
		}
		public string Filter(string prefix, JToken token, string[] dimensions) {
			var keys = new List<KeyValuePair<string, string>>();
			foreach (var d in dimensions) {
				var v = token[d];
				if (token[d] != null) {
					keys.Add(new KeyValuePair<string, string>(d, v.ToString()));
				}
			}
			if (string.IsNullOrEmpty(prefix)) {
				return string.Join("&", keys.Select(x => FilterPortion(x.Key, x.Value)));

			} else {
				return prefix + "&" + string.Join("&", keys.Select(x => FilterPortion(x.Key, x.Value)));
			}
		}

		public static string FilterPortion(string key, string value) {
			return string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value));
		}

	}
}
