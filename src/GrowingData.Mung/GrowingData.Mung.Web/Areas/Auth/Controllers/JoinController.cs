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
	public class JoinController : Controller {
		// GET: Auth/Authenication
		[Route("join")]
		[HttpGet]
		public ActionResult Join() {
			return View();
		}

		[Route("join")]
		[HttpPost]
		public ActionResult Join(string email, string password, string name) {

			var salt = StringHashing.CreateSalt();
			var passwordHash = StringHashing.HashStrings(password, salt);

			var existing = Munger.Get(email);
			if (existing != null) {
				ViewBag.ErrorMessage = "A user with the same email has already created an account";
				return View();
			}

			var munger = new Munger() {
				Name = name,
				Email = email,
				PasswordSalt = salt,
				PasswordHash = passwordHash
			};

			munger.MungerId = munger.Insert();

			SessionManager.Login(munger);

			return Redirect("/");

		}
	}
}