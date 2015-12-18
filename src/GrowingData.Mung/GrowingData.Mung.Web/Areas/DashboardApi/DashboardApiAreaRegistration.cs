using System.Web.Mvc;

namespace GrowingData.Mung.Web.Areas.DashboardApi {
	public class DashboardApiAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "DashboardApi";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
		}
	}
}