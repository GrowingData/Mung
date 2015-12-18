using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GrowingData.Mung;
using GrowingData.Utilities;

namespace GrowingData.Mung.Web.Models {
	public class Dashboard {
		public int DashboardId;
		public string Url;
		public string Title;
		public string Css;
		public string Js;
		public int CreatedByUserId;
		public int ModifiedByUserId;


		public static Dashboard Get(string url) {
			using (var cn = Db.Metadata()) {
				var dashboard = cn.ExecuteAnonymousSql<Dashboard>(
						@"SELECT * FROM mung.Dashboard WHERE Url = @Url",
						 new { Url = url }
					)
					.FirstOrDefault();
				return dashboard;
			}
		}
		public static Dashboard Get(int dashboardId) {
			using (var cn = Db.Metadata()) {
				var dashboard = cn.ExecuteAnonymousSql<Dashboard>(
						@"SELECT * FROM mung.Dashboard WHERE Url = @DashboardId",
						 new { DashboardId = dashboardId }
					)
					.FirstOrDefault();
				return dashboard;
			}
		}


		public List<Component> GetComponents() {
			using (var cn = Db.Metadata()) {
				var components = cn.ExecuteAnonymousSql<Component>(
					@"SELECT * FROM mung.Component WHERE DashboardId = @DashboardId",
					new { DashboardId = DashboardId }
				);
				return components;
			}

		}
	}
}