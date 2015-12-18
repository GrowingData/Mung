using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GrowingData.Mung.Web {
	public class MvcApplication : System.Web.HttpApplication {
		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}


		public override void Init() {
			this.BeginRequest += BeginRequestHandler;
			this.EndRequest += EndRequestHandler;

		}
		void BeginRequestHandler(object sender, EventArgs e) {
			SessionManager.InitializeMunger();

			//if (!Request.IsSecureConnection) {
			//	// Make sure that all requests are secure.
			//	Response.RedirectPermanent("https://" + Request.Url.Host + Request.Url.PathAndQuery);
			//}

		}

		void EndRequestHandler(object sender, EventArgs e) {
		}


	}
}
