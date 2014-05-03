using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

using GrowingData.Mung.Core;

namespace GrowingData.Mung.Server {
	public class DbTable {

		public string Name { get; set; }
		public List<DbColumn> Columns { get; set; }

		private DbTable(string tableName) {
			Name = tableName;
			Columns = new List<DbColumn>();

		}

		public DbTable(string tableName, MungedDataReader reader) {



			Name = tableName;
			Columns = new List<DbColumn>();


			for (var i = 0; i < reader.ColumnNames.Length; i++) {
				var type = reader.ColumnTypes[i];
				var name = reader.ColumnNames[i];
				var c = new DbColumn(name, type);
				Columns.Add(c);
			}

		}


		public static DbTable LoadDb(DbDataReader reader) {

			if (!reader.Read()) {
				return null;
			}

			var tbl = new DbTable((string)reader["TABLE_NAME"]);


			var firstColumn = new DbColumn(
				(string)reader["COLUMN_NAME"],
				(string)reader["DATA_TYPE"]
			);
			tbl.Columns = new List<DbColumn>() { firstColumn };


			while (reader.Read()) {
				if (tbl.Name != (string)reader["TABLE_NAME"]) {
					break;
				}
				tbl.Columns.Add(new DbColumn(
					(string)reader["COLUMN_NAME"],
					(string)reader["DATA_TYPE"]
				));

			}
			return tbl;

		}

	}
}
