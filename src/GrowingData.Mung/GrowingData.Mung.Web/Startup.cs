using System;
using Owin;
using System.IO;
using Microsoft.Owin;
using System.Threading;
using System.Threading.Tasks;

using GrowingData.Mung.Core;
using GrowingData.Mung.SqlBatch;
using GrowingData.Mung.Relationizer;

[assembly: OwinStartupAttribute(typeof(GrowingData.Mung.Web.Startup))]
namespace GrowingData.Mung.Web {
	public partial class Startup {
		public void Configuration(IAppBuilder app) {
			app.MapSignalR<LogWriter>("/log/write");
			app.MapSignalR<LogReader>("/log/read");

			app.MapSignalR();


			MungState.App.Pipeline.AddProcessor(new RelationalEventProcessor(PathManager.DataPath));


			Task.Run(() => {
				// If the app shut down without properly disposing of the file
				// objects, we may have a whole lot of files still marked as "active"
				// which are actually dead.
				SqlBatchChecker.CleanUpOldFiles(PathManager.DataPath, Db.Warehouse);
			});

			Task.Run(() => {
				while (true) {
					Thread.Sleep(1000 * 60);
					SqlBatchChecker.Check(PathManager.DataPath, Db.Warehouse);
				}
			});
		}
	}
}
