using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrowingData.Mung.Web {
	public class MungTable {
		public string TableName;
		public string SchemaName;
		public List<MungColumn> Columns;

		public MungTable(string tableName, string schemaName) {
			TableName = tableName;
			SchemaName = schemaName;
			Columns = new List<MungColumn>();
		}
	}
}