using System.Web.Mvc;

namespace GrowingData.Mung.Web.Areas.Auth {
	public class AuthAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "Auth";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
		}
	}
}