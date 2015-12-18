using System.Web.Mvc;

namespace GrowingData.Mung.Web.Areas.ApiData {
	public class ApiDataAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "ApiData";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
		}
	}
}