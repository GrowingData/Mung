using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrowingData.Mung.Web {
	public class MungColumn {
		public string ColumnName;
		public string ColumnType;
		public MungColumn(string name, string type) {
			ColumnName = name;
			ColumnType = type;
		}
	}
}