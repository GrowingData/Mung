using System;
using Owin;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using GrowingData.Mung.Core;

[assembly: OwinStartupAttribute(typeof(GrowingData.Mung.Server.Startup))]
namespace GrowingData.Mung.Server {
	public partial class Startup {



		public void Configuration(IAppBuilder app) {


			app.MapSignalR<LogWriter>("/log/write");
			app.MapSignalR<LogReader>("/log/read");

			app.MapSignalR();


		}

	}

}
