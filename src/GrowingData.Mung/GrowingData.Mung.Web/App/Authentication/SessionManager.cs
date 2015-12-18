using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.Security;
using Newtonsoft.Json;
using GrowingData.Utilities;
using GrowingData.Mung.Web.Models;


namespace GrowingData.Mung.Web {
	public class SessionManager {
		private static Dictionary<string, string> _applicationState = new Dictionary<string, string>();
		public static bool RequireSSL = false;

		public const string CookieNameSessionId = "sid";
		public const string CookieNameSessionData = "s";
		public const string CookieNameEternalId = "eid";

		public static string Get(string key) {
			var sesh = SessionVariables();
			if (sesh != null) {
				string val = null;
				sesh.TryGetValue(key, out val);

				return val;
			} else {
				return null;
			}
		}

		public static void Set(string key, string value) {
			var sesh = SessionVariables();
			if (sesh == null) {
				sesh = new Dictionary<string, string>();
			}
			sesh[key] = value;
			SetSession(sesh);
		}


		private static FormsAuthenticationTicket GetTicketFromCookie(string cookieKey) {
			var c = HttpContext.Current.Request.Cookies[cookieKey];
			if (c != null) {
				try {
					return FormsAuthentication.Decrypt(c.Value);
				} catch {
					return null;
				}
			}
			return null;

		}

		private static Dictionary<string, string> SessionVariables() {
			// Encryption / decryption may be slow so lets cache it in the 
			// request object
			if (HttpContext.Current.Items["session"] != null) {
				return HttpContext.Current.Items["session"] as Dictionary<string, string>;
			}

			// Check the cookie
			var ticket = GetTicketFromCookie(CookieNameSessionData);
			if (ticket != null) {
				if (ticket.Expired) {
					HttpContext.Current.Request.Cookies.Remove(CookieNameSessionData);
					HttpContext.Current.Response.Cookies.Remove(CookieNameSessionData);

				} else {
					var json = ticket.UserData;
					var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

					HttpContext.Current.Items["session"] = data;

					return data;

				}
			}

			return null;
		}

		private static void SetSession(Dictionary<string, string> session) {
			// Make sure that the session is recorded in the request context, 
			// otherwise the next time we try to set something we will overwrite
			// whatever this is (since we only look at the request cookies when
			// getting the session object).
			HttpContext.Current.Items["session"] = session;

			string json = JsonConvert.SerializeObject(session);
			FormsAuthenticationTicket t = new FormsAuthenticationTicket(1, "sesh", DateTime.UtcNow, DateTime.UtcNow.AddMinutes(20), false, json);

			SetCookieSession(CookieNameSessionData, FormsAuthentication.Encrypt(t));
		}


		public static void Login(Munger munger) {

			munger.LastSeenAt = DateTime.UtcNow;
			munger.Update();

			HttpContext.Current.Items["munger"] = munger;

			// And set their session cookie
			SetLoggedInSessionCookie(munger);

		}

		public static string GetEncryptedTicket(Munger munger) {
			var ticket = new FormsAuthenticationTicket(1, "uid", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), false, munger.MungerId.ToString());
			return FormsAuthentication.Encrypt(ticket);
		}


		public static void SetCookieSession(string name, string value) {

			HttpCookie c = new HttpCookie(name);
			c.Path = "/";
			c.Secure = RequireSSL;
			c.Value = value;

			HttpContext.Current.Response.Cookies.Add(c);

		}


		public static void SetCookieMonth(string name, string value) {

			HttpCookie responseCookie = new HttpCookie(name);
			responseCookie.Path = "/";
			responseCookie.Secure = RequireSSL;
			responseCookie.Value = value;
			responseCookie.Expires = DateTime.Now.AddMonths(1);

			HttpContext.Current.Response.Cookies.Add(responseCookie);

		}

		public static void SetCookieAbandoned(string name) {
			if (HttpContext.Current.Request.Cookies[name] != null) {
				var responseCookie = HttpContext.Current.Response.Cookies[name];

				responseCookie.Expires = DateTime.UtcNow.Subtract(new TimeSpan(365, 0, 0, 0));
				responseCookie.Path = "/";
				responseCookie.Secure = RequireSSL;

				HttpContext.Current.Request.Cookies.Remove(name);
			}
		}


		public static void SetTrackingCookies() {
			SetSessionTrackingCookie(CookieNameEternalId, new TimeSpan(30, 0, 0, 0));
			SetSessionTrackingCookie(CookieNameSessionId, new TimeSpan(0, 0, 0, 0));
		}

		public static string GetTrackingCookieEternal() {
			return HttpContext.Current.Items[CookieNameEternalId] as string;

		}
		public static string GetTrackingCookieSession() {
			return HttpContext.Current.Items[CookieNameSessionId] as string;
		}

		public static void SetSessionTrackingCookie(string name, TimeSpan expiresIn) {
			var requestCookies = HttpContext.Current.Request.Cookies;
			var responseCookies = HttpContext.Current.Response.Cookies;
			var cookie = requestCookies[name];

			if (cookie != null) {
				HttpContext.Current.Items[name] = cookie.Value;
				if (cookie.Expires != DateTime.MinValue) {
					if (DateTime.UtcNow.Subtract(cookie.Expires).TotalDays < expiresIn.TotalDays - 1) {
						cookie.Expires = DateTime.UtcNow.Add(expiresIn);
						responseCookies.Add(cookie);
					}
				}

			} else {
				cookie = new HttpCookie(name);
				if (expiresIn.TotalMilliseconds != 0) {
					cookie.Expires = DateTime.UtcNow.Add(expiresIn);
				}
				cookie.Path = "/";
				cookie.Secure = RequireSSL;
				cookie.Value = RandomString.Get(12);
				HttpContext.Current.Items[name] = cookie.Value;

				responseCookies.Add(cookie);

			}
		}

		public static void SetLoggedInSessionCookie(Munger munger) {
			SetCookieSession("u", GetEncryptedTicket(munger));
		}

		public static void Adandon() {
			SetCookieAbandoned("u");
			SetCookieAbandoned("remember-me");
		}


		public static void InitializeMunger() {
			// Still set it to NULL so that we know that this has been processed
			HttpContext.Current.Items["munger"] = null;

			FormsAuthenticationTicket ticket = GetTicketFromCookie("u");
			if (ticket == null) {
				return;
			}

			if (ticket.Expiration < DateTime.UtcNow) {
				HttpContext.Current.Request.Cookies.Remove("u");
				HttpContext.Current.Response.Cookies.Remove("u");
				return;
			}

			var userId = -1;
			if (!int.TryParse(ticket.UserData, out userId)) {
				return;
			}

			// Get the munger, using the token
			Munger mungerById = Munger.Get(userId);
			if (mungerById != null) {
				// Store it for later
				HttpContext.Current.Items["munger"] = mungerById;
				// Reset the cookie to keep them alive
				SetLoggedInSessionCookie(mungerById);
			}
		}

		public static Munger CurrentMunger {
			get {
				Munger munger = null;
				if (HttpContext.Current != null && HttpContext.Current.Items != null) {
					if (!HttpContext.Current.Items.Contains("munger")) {
						throw new InvalidOperationException("Please use a session specific view pipeline");
					}
				}

				if (HttpContext.Current != null && HttpContext.Current.Items != null) {
					munger = HttpContext.Current.Items["munger"] as Munger;
				}

				return munger;
			}
		}
	}
}