using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrowingData.Mung;
using GrowingData.Utilities;
using GrowingData.Mung.Web.Models;


namespace GrowingData.Mung.Web.Areas.Dashboards.Controllers {
	[RouteArea("Dashboard", AreaPrefix = "")]
	public class DashboardController : Controller {
		// GET: Dashboards/CreateDashboard
		
		[Route("")]
		public ActionResult Default(string url) {
			return View("Default");
		}


		[Route("dashboard/{*url}")]
		public ActionResult ViewDashboard(string url) {


			url = "/" + url;
			using (var cn = Db.Metadata()) {

				// Does the URL already exist?

				var dashboard = Dashboard.Get(url);

				if (dashboard == null) {
					throw new HttpException(404, "Unable to find the dashboard at the url: " + Request.RawUrl);
				}

				ViewBag.Dashboard = dashboard;
				ViewBag.Components = dashboard.GetComponents();


			}
			return View("Dashboard");
		}
	}
}