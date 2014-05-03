using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingData.Mung.Server {

	public enum DbType {
		Key,
		Integer,
		Varchar,
		DateTime,
		Float,
		Unknown,
		Bit
	}

	public static class DbTypes {
		   

		public static DbType FromSqlType(string type) {
			if (type == "varchar" || type == "nvarchar") {
				return DbType.Varchar;
			}
			if (type == "tinyint" || type == "smallint" || type == "int" || type == "bigint") {
				return DbType.Integer;
			}
			if (type == "smalldatetime" || type == "datetime") {
				return DbType.DateTime;
			}
			if (type == "float" || type == "decimal" || type == "money") {
				return DbType.Float;
			}

			return DbType.Varchar;
		}


		public static DbType FromString(string type) {
			if (type == "Key") return DbType.Key;
			if (type == "Integer") return DbType.Integer;
			if (type == "Varchar") return DbType.Varchar;
			if (type == "DateTime") return DbType.DateTime;
			if (type == "Float") return DbType.Float;
			if (type == "Bit") return DbType.Bit;

			throw new Exception(string.Format("Unknown DbType: {0}", type));
		}


		public static DbType FromJsonType(JTokenType type) {
			if (type == JTokenType.String) {
				return DbType.Varchar;
			}

			if (type == JTokenType.Float) {
				return DbType.Float;
			}

			if (type == JTokenType.Integer) {
				return DbType.Integer;
			}
			if (type == JTokenType.Date) {
				return DbType.DateTime;
			}
			if (type == JTokenType.Boolean) {
				return DbType.Bit;
			}
			return DbType.Unknown;
		}

		public static DbType FromType(Type type) {
			if (type == typeof(string)) {
				return DbType.Varchar;
			}

			if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)
			|| type == typeof(float?) || type == typeof(double?) || type == typeof(decimal?)) {
				return DbType.Float;
			}

			if (type == typeof(int) || type == typeof(short) || type == typeof(long)
				|| type == typeof(uint) || type == typeof(ushort) || type == typeof(ulong)

				|| type == typeof(int?) || type == typeof(short?) || type == typeof(long?)
				|| type == typeof(uint?) || type == typeof(ushort?) || type == typeof(ulong?)
				) {
				return DbType.Integer;
			}

			if (type == typeof(bool) || type == typeof(bool?)) {
				return DbType.Bit;

			}



			if (type == typeof(DateTime)
				|| type == typeof(DateTime?)) {
				return DbType.DateTime;
			}
			return DbType.Unknown;
		}

		public static string SqlType(this DbType type) {
			if (type == DbType.DateTime) return "DATETIME";
			if (type == DbType.Float) return "FLOAT";
			if (type == DbType.Integer) return "BIGINT";
			if (type == DbType.Bit) return "BIT";
			if (type == DbType.Varchar) return "NVARCHAR(MAX)";
			if (type == DbType.Key) return string.Format("CHAR({0})", RelationalEvent.KeyLength);

			throw new Exception(string.Format("Unknown DbType: {0}", type));
		}
	}



}
