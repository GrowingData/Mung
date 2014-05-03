using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingData.Mung.Server {

	public class DbColumn {

		public string Name { get; set; }
		public DbType Type { get; set; }
		public DbColumn(string name, DbType type) {
			Name = name;
			Type = type;
		}
		public DbColumn(string name, string type) {
			Name = name;
			Type = DbTypes.FromSqlType(type);
		}
		public DbColumn(string name, JTokenType type) {
			Name = name;
			Type = DbTypes.FromJsonType(type);
		}

	}
}
