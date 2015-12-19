using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GrowingData.Utilities;

namespace GrowingData.Mung.Web.Models {
	public class Graph {



		public int GraphId;
		public int DashboardId;
		public string Title;
		public string Html;
		public string Sql;
		public string Js;
		public double X;
		public double Y;
		public double Width;
		public double Height;



		public bool Save(Dashboard dashboard) {

			var graphs = dashboard.GetGraphs();
			// Make sure we are pointing to the right dashboard
			DashboardId = dashboard.DashboardId;

			using (var cn = Db.Metadata()) {

				if (GraphId == -1) {
					var sql = @"
	INSERT INTO mung.Graph(DashboardId, Title, Html, Sql, Js, X, Y, Width, Height)
		SELECT @DashboardId, @Title, @Html, @Sql, @Js, @X, @Y, @Width, @Height";
					cn.ExecuteSql(sql, this);

					return true;

				} else {
					// Make sure that this graph actually belongs to this Dashboard
					if (graphs.Count(x => x.GraphId == this.GraphId) != 1) {
						throw new HttpException(401, "The graph specified does not belong to the dashboard with Url: " + dashboard.Url);
					}

					var sql = @"
	UPDATE mung.Graph
		SET Title=@Title, Html=@Html, Sql=@Sql, Js=@Js, X=@X, Y=@Y, Width=@Width, Height=@Height
		WHERE GraphId = @GraphId";
					cn.ExecuteSql(sql, this);
					return true;
				}
			}
		}



		/// <summary>
		/// Deletes a Graph from a Dashboard, checking that the Graph does infact
		/// live within the dashboard.
		/// </summary>
		/// <param name="dashboard"></param>
		/// <returns></returns>
		public bool Delete(Dashboard dashboard) {
			var graphs = dashboard.GetGraphs();

			using (var cn = Db.Metadata()) {
				if (GraphId == -1) {
					throw new HttpException(401, "No GraphId was specified for deletion: " + dashboard.Url);
				} else {
					// Make sure that this graph actually belongs to this Dashboard
					if (graphs.Count(x => x.GraphId == GraphId) != 1) {
						throw new HttpException(401, "The graph specified does not belong to the dashboard with Url: " + dashboard.Url);
					}

					var sql = @"
	DELETE FROM mung.Graph
		WHERE GraphId = @GraphId
		AND		DashboardId = @DashboardId";

					cn.ExecuteSql(sql, new {
						ComponentId = GraphId,
						DashboardId = dashboard.DashboardId
					});

					return true;
				}
			}
		}

	}
}