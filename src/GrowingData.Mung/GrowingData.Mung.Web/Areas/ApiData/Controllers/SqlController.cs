using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrowingData.Utilities;
using GrowingData.Mung.Web;

namespace GrowingData.Mung.Web.Areas.ApiData.Controllers {
	public class MungSqlController : Controller {



		[Route("api/sql/mung")]
		[HttpPost]
		public ActionResult Index(string sql) {


			using (var cn = Db.Warehouse()) {
				Response.ContentType = "application/json";
				Response.Write(cn.DumpJsonRows(sql, null));
				Response.End();
				return null;
			}



		}
	}
}