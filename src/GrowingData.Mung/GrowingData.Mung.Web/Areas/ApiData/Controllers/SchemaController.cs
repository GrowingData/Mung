using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrowingData.Utilities;
using GrowingData.Mung.Web;

namespace GrowingData.Mung.Web.Areas.ApiData.Controllers {
	public class SchemaController : Controller {



		[Route("api/schema/mung")]
		public ActionResult Index() {


			var sql = @"
				    SELECT table_schema, table_name, column_name, data_type
					FROM information_schema.columns
					WHERE	table_schema='dyn'
					ORDER BY table_catalog, table_schema, table_name, ordinal_position
			";

			using (var cn = Db.Warehouse()) {


				var tables = new Dictionary<string, MungTable>();


				cn.ExecuteRow(sql, null, (reader) => {
					var tableName = (string)reader["table_name"];
					var tableSchema = (string)reader["table_schema"];
					var columnName = (string)reader["column_name"];
					var columnType = (string)reader["data_type"];

					MungTable tbl = null;

					if (!tables.ContainsKey(tableName)) {
						tbl = new MungTable(tableName, tableSchema);
						tables[tableName] = tbl;
					} else {
						tbl = tables[tableName];
					}

					tbl.Columns.Add(new MungColumn(columnName, columnType));
				});


				return Json(new { Schema = tables.Values.OrderBy(x=>x.TableName) }, JsonRequestBehavior.AllowGet);
			}



		}
	}
}