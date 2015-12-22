using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrowingData.Mung;
using GrowingData.Utilities;
using GrowingData.Mung.Web.Models;

namespace GrowingData.Mung.Web.Areas.Dashboards.Controllers {
	[RouteArea("Dashboard", AreaPrefix = "")]
	public class HomeController : Controller {
		// GET: Dashboards/CreateDashboard

		[Route("")]
		public ActionResult Default() {
			var munger = SessionManager.CurrentMunger;
			if (munger == null) {
				return Redirect("/login");
			}


			ViewBag.Dashboards = Dashboard.List(SessionManager.CurrentMunger.MungerId);



			return View("Default");
		}

	}
}