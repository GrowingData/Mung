using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrowingData.Mung;
using GrowingData.Utilities;


namespace GrowingData.Mung.Web.Areas.Dashboards.Controllers {
	[RouteArea("Dashboard", AreaPrefix = "")]
	public class DashboardCreateController : Controller {
		// GET: Dashboards/CreateDashboard



		[Route("create/dashboard/{*newUrl}")]
		public ActionResult Create(string newUrl, string title) {

			newUrl = "/" + newUrl;
			if (string.IsNullOrEmpty(title)) {
				title = newUrl
					.Replace("-", " ")
					.Replace("_", " ")
					.Replace("/", "");
			}

			using (var cn = Db.Metadata()) {

				// Does the URL already exist?

				var existing = cn.DumpList<int>(@"SELECT DashboardId FROM mung.Dashboard WHERE Url = @Url", new { Url = newUrl });
				if (existing.Count > 0) {
					return Redirect("/dashboard/" + newUrl);
				} else {
					cn.ExecuteSql(@"INSERT INTO mung.Dashboard (Url, Title, CreatedAt, UpdatedAt, CreatedByUserId, ModifiedByUserId)
						SELECT @Url, @Title, GETUTCDATE(), GETUTCDATE(), @UserId, @UserId
					", new { Url = newUrl, UserId = 1, Title = title });

					return Redirect("/dashboard/" + newUrl);
				}
			}
		}
	}
}