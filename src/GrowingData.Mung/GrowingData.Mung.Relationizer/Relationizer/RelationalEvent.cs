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


	public class RelationalField : IComparable {
		public string ColumnName;
		public DbType Type;

		public int CompareTo(object obj) {
			var other = obj as RelationalField;
			return this.ColumnName.CompareTo(other.ColumnName);
		}

		public override int GetHashCode() {
			return ColumnName.GetHashCode();
		}
	}

	public class RelationalEvent {

		public static string GetKey() {
			return RandomString.Get(DbTypes.KeyLength);
		}

		public string Name;

		public string _Id_;
		public string _ParentId_;

		public DateTime _At_;

		public string ParentType;

		private List<RelationalField> _schema;
		private Dictionary<string, object> _values;

		public string this[string key] {
			get {
				object o = null;
				if (_values.TryGetValue(key, out o)) {
					return Serialize(o);
				} else {
					return string.Empty;
				}
			}
		}
		public IEnumerable<RelationalField> Schema { get { return _schema; } }

		public void AddField(string name, DbType type, object value) {
			var field = new RelationalField() {
				ColumnName = name.ToLowerInvariant(),
				Type = type
			};
			_schema.Add(field);
			_values[field.ColumnName] = value;

		}

		public RelationalEvent() {
			_schema = new List<RelationalField>();
			_values = new Dictionary<string, object>();
		}

		public override string ToString() {
			return string.Format("{0}: {1}->{2} | {3}",
				Name,
				_Id_,
				_ParentId_,
				string.Join(",", _values.Select(s => string.Format("\"{0}\"", s)))
			);
		}


		public static string Serialize(object o) {
			if (o == null) {
				return string.Empty;
			}

			if (o is string) {
				// Strings are escaped 
				return "\"" + Escape(o.ToString()) + "\"";

			}

			if (o is DateTime) {
				return string.Format("\"{0}\"", ((DateTime)o).ToString("o"));
			}

			return o.ToString();

		}

		public static string Escape(string unescaped) {

			//https://www.monetdb.org/Documentation/Cookbooks/SQLrecipes/LoadingBulkData
			return unescaped
				.Replace("\\", "\\" + "\\")		// '\' -> '\\'
				.Replace("\"", "\\" + "\"");		// '"' -> '""'
		}




	}


}
