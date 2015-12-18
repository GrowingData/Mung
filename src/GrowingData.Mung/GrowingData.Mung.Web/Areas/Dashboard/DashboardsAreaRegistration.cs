using System.Web.Mvc;

namespace GrowingData.Mung.Web.Areas.Dashboards {
	public class DashboardAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "Dashboard";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
		}
	}
}