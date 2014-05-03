using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Relationizer {
	/// <summary>
	/// Manages schema changes over time, ensuring that everything is
	/// always written in a consistent manner, even if the objects schema
	/// is somewhat different to the actual schema (e.g. more columns)
	/// </summary>
	public class RelationalSchema : IDisposable {
		private object _syncRoot = new object();
		private string _basePath;


		private Dictionary<string, RelationalEventWriter> _writers;


		public RelationalSchema(string basePath) {
			_basePath = basePath;
			_writers = new Dictionary<string, RelationalEventWriter>();
		}


		public void Write(JToken token, string type) {

			lock (_syncRoot) {
				foreach (var item in JsonReader.ProcessJsonObject(token, type, null)) {

					if (!_writers.ContainsKey(item.Name)) {
						_writers[item.Name] = new RelationalEventWriter(_basePath, item.Name);
					}

					_writers[item.Name].Write(item);
				}
			}
		}



		public void Dispose() {
			lock (_syncRoot) {
				foreach (var writer in _writers) {
					writer.Value.Dispose();
				}
			}
		}
	}

}
