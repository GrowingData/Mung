using System;
using Owin;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;


using GrowingData.Mung.Core;
using GrowingData.Mung.SqlBatch;
using GrowingData.Mung.Relationizer;
using GrowingData.Mung.Metric;

[assembly: OwinStartupAttribute(typeof(GrowingData.Mung.Server.Startup))]
namespace GrowingData.Mung.Server {
	public partial class Startup {



		public void Configuration(IAppBuilder app) {


			app.MapSignalR<LogWriter>("/log/write");
			app.MapSignalR<LogReader>("/log/read");

			app.MapSignalR();


			MungState.App.Pipeline.AddProcessor(new RelationalEventProcessor(PathManager.DataPath));

			var metricPath = Path.Combine(PathManager.BasePath, "user", "metric");
			var metrics = new MetricFactory(metricPath, MungState.App.Pipeline);

			metrics.Reload();

			Task.Run(() => {
				// If the app shut down without properly disposing of the file
				// objects, we may have a whole lot of files still marked as "active"
				// which are actually dead.
				SqlBatchChecker.CleanUpOldFiles(PathManager.DataPath, Db.Warehouse);
			});

			Task.Run(() => {
				while (true) {
					Thread.Sleep(1000 * 60 );
					SqlBatchChecker.Check(PathManager.DataPath, Db.Warehouse);
				}
			});
		}

	}

}
