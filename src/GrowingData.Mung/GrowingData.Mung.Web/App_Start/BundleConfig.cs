using System.Web;
using System.Web.Optimization;

namespace GrowingData.Mung.Web {
	public class BundleConfig {
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles) {


			bundles.Add(new ScriptBundle("~/bundles/js").Include(
				"~/Scripts/modernizr-2.6.2.js",
				"~/Scripts/jquery-2.1.4.js",
				"~/Scripts/bootstrap.js",
				"~/Scripts/respond.js",

				"~/lib/gridstack.js-0.2.3/src/gridstack.js",

				"~/lib/codemirror-4.4/lib/codemirror.js",
				"~/lib/codemirror-4.4/mode/sql/sql.js",
				"~/lib/codemirror-4.4/addon/hint/show-hint.js",
				"~/lib/codemirror-4.4/addon/hint/sql-hint.js",
				"~/lib/codemirror-4.4/addon/hint/css-hint.js",
				"~/lib/codemirror-4.4/addon/hint/html-hint.js",
				"~/lib/codemirror-4.4/addon/hint/javascript-hint.js",


				"~/static/js/editor/mungEditor.js",
				"~/static/js/editor/mungEditorHtml.js",
				"~/static/js/editor/mungEditorSql.js",
				"~/static/js/editor/mungEditorJs.js",
				"~/static/js/editor/autocomplete/autoCompleteSql.js",

				"~/static/js/dashboard/mungDashboard.js",
				"~/static/js/dashboard/mungComponentEditor.js",
				"~/static/js/dashboard/mungComponent.js",
				"~/static/js/dashboard/mungDashboard.js"
			));



			bundles.Add(new StyleBundle("~/bundles/css").Include(
				"~/Content/bootstrap.css",

				"~/lib/gridstack.js-0.2.3/src/gridstack.css",

				"~/lib/codemirror-4.4/lib/codemirror.css",
				"~/lib/codemirror-4.4/addon/hint/show-hint.css",
				"~/static/css/theme/light-table.css",
				"~/static/css/site.css",
				"~/static/css/dashboard.css",
				"~/static/css/component.css",
				"~/static/css/editor.css"
			));
		}
	}
}
