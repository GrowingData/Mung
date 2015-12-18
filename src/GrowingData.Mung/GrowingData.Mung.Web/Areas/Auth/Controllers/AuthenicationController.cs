using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrowingData.Utilities;
using GrowingData.Mung.Web;
using GrowingData.Mung.Web.Models;

namespace GrowingData.Mung.Web.Areas.Auth.Controllers {
	[RouteArea("Auth", AreaPrefix = "")]
	public class AuthenicationController : Controller {
		// GET: Auth/Authenication
		[Route("login")]
		[HttpGet]
		public ActionResult Login() {
			return View();
		}

		[Route("login")]
		[HttpPost]
		public ActionResult Login(string email, string password) {


			var munger = Munger.Get(email);

			var inputHash = StringHashing.HashStrings(password, munger.PasswordSalt);

			if (inputHash == munger.PasswordHash) {
				SessionManager.Login(munger);
				return Redirect("/");

			}
			return View();
		}

		[Route("logout")]
		[HttpPost]
		public ActionResult Logout() {
			SessionManager.Adandon();
			return Redirect("/");
		}
	}
}