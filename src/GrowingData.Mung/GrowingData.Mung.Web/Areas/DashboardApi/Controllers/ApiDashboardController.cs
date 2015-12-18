using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrowingData.Mung;
using GrowingData.Mung.Web.Models;
using GrowingData.Utilities;

using Newtonsoft.Json;

namespace GrowingData.Mung.Web.Areas.DashboardApi.Controllers {
	public class ApiDashboardController : Controller {


		[HttpPost, ValidateInput(false)]
		[Route("api/dashboard/component")]
		public ActionResult SaveComponent(string url, string componentJson) {
			var dashboard = Dashboard.Get(url);
			if (dashboard == null) {
				throw new HttpException(404, "Unable to find the dashboard at the url: " + url);
			}
			var components = dashboard.GetComponents();
			var toSave = JsonConvert.DeserializeObject<Component>(componentJson);
			// Make sure we are pointing to the right dashboard
			toSave.DashboardId = dashboard.DashboardId;

			using (var cn = Db.Metadata()) {

				if (toSave.ComponentId == -1) {
					var sql = @"
	INSERT INTO mung.Component(DashboardId, Html, Sql, Js, PositionX, PositionY, Width, Height)
		SELECT @DashboardId, @Html, @Sql, @Js, @PositionX, @PositionY, @Width, @Height";
					cn.ExecuteSql(sql, toSave);
				} else {
					// Make sure that this component actually belongs to this Dashboard
					if (components.Count(x => x.ComponentId == toSave.ComponentId) != 1) {
						throw new HttpException(401, "The component specified does not belong to the dashboard with Url: " + url);
					}

					var sql = @"
	UPDATE mung.Component
		SET Html=@Html, Sql=@Sql, PositionX=@PositionX, PositionY=@PositionY, Width=@Width, Height=@Height
		WHERE ComponentId = @ComponentId";
					cn.ExecuteSql(sql, toSave);

				}
			}
			return Json(new { Success = true, Message = "Success" });
		}

		[HttpDelete, ValidateInput(false)]
		[Route("api/dashboard/component")]
		public ActionResult DeleteComponent(string url, string componentJson) {
			url = "/" + url;
			var dashboard = Dashboard.Get(url);
			if (dashboard == null) {
				throw new HttpException(404, "Unable to find the dashboard at the url: " + url);
			}

			var components = dashboard.GetComponents();
			var toDelete = JsonConvert.DeserializeObject<Component>(componentJson);

			// Make sure we are pointing to the right dashboard
			toDelete.DashboardId = dashboard.DashboardId;

			using (var cn = Db.Metadata()) {
				if (toDelete.ComponentId == -1) {
					throw new HttpException(401, "No ComponentId was specified for deletion: " + url);
				} else {
					// Make sure that this component actually belongs to this Dashboard
					if (components.Count(x => x.ComponentId == toDelete.ComponentId) != 1) {
						throw new HttpException(401, "The component specified does not belong to the dashboard with Url: " + url);
					}

					var sql = @"
	DELETE FROM mung.Component
		WHERE ComponentId = @ComponentId
		AND		DashboardId = @DashboardId";

					cn.ExecuteSql(sql, new {
						ComponentId = toDelete.ComponentId,
						DashboardId = dashboard.DashboardId
					});
				}
			}


			return Json(new { Success = true, Message = "Success" });

		}
	}
}