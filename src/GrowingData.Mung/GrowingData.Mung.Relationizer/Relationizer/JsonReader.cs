using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Relationizer {
	public class JsonReader {


		private static JsonConverter[] Converters = new JsonConverter[] { 
				new IsoDateTimeConverter() 
		};


		public static object GetValue(JProperty token, DbType type) {
			if (type == DbType.Bit) {
				return (bool)token ? "1" : "0";
			}


			return token.Value.ToObject<object>();
		}

		public static IEnumerable<RelationalEvent> Process(string json, string eventName) {

			var firstToken = JToken.Parse(json);
			return Process(firstToken, eventName, null);


		}
		public static IEnumerable<RelationalEvent> Process(JToken t, string eventName, RelationalEvent parent) {
			if (t.Type == JTokenType.Object) {
				foreach (var r in ProcessJsonObject(t, eventName, null)) {
					yield return r;

				}
			}
			if (t.Type == JTokenType.Array) {
				foreach (var r in ProcessJsonArray(t, eventName, null)) {
					yield return r;

				}
			}
		}

		public static IEnumerable<RelationalEvent> ProcessJsonArray(JToken t, string eventName, RelationalEvent parent) {

			if (t.Type == JTokenType.Array) {
				foreach (var child in t.Children()) {
					if (child.Type == JTokenType.Object) {
						foreach (var r in ProcessJsonObject(child, eventName, parent)) {
							yield return r;
						}

					}

				}
			}
		}

		public static IEnumerable<RelationalEvent> ProcessJsonObject(JToken t, string eventName, RelationalEvent parent) {
			var relation = new RelationalEvent();

			relation._Id_ = RelationalEvent.GetKey();
			relation._At_ = DateTime.UtcNow;
			relation.Name = eventName.ToLowerInvariant();


			if (parent != null) {
				relation._ParentId_ = parent._Id_;
				relation.ParentType = parent.Name;

				relation.Name = relation.ParentType + "_" + relation.Name;
			} else {
				relation._ParentId_ = null;
				relation.ParentType = null;
			}


			if (t.Type == JTokenType.Object) {
				// If we have an object, we want to create fields for all the 
				// "simple" types, and child items for all the complex ones

				foreach (var child in t.Children()) {
					if (child.Type == JTokenType.Property) {
						var property = child as JProperty;
						var dbType = DbTypes.FromJsonType(property.Value.Type);
						if (dbType != DbType.Unknown) {
							relation.AddField(property.Name, dbType, GetValue(property, dbType));
						} else {
							if (property.Value.Type == JTokenType.Array) {
								foreach (var r in ProcessJsonArray(property.Value, property.Name, relation)) {
									yield return r;
								}
							}

							if (property.Value.Type == JTokenType.Object) {
								foreach (var r in ProcessJsonObject(property.Value, property.Name, relation)) {
									yield return r;
								}

							}

						}
					}
				}
			}
			yield return relation;
		}




	}

}
