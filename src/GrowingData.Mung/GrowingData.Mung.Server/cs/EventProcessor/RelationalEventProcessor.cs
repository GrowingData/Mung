using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using GrowingData.Mung.Core;



namespace GrowingData.Mung.Server {
	public class RelationalEventProcessor : EventProcessor {
		private object _sync = new object();

		// Holds all our state for 
		private Relationizer _relationizer;



		public RelationalEventProcessor(string basePath)
			: base("RelationalEventWriter") {
			_relationizer = new Relationizer(basePath);

		}



		protected override void ProcessEvent(MungServerEvent evt) {
			_relationizer.Write(evt);


		}


	}
}
