using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Server {
	public class MungApp : IMungApp {
		private EventPipeline _pipeline;
		public EventPipeline Pipeline { get { return _pipeline; } }


		public MungApp() {
			_pipeline = new EventPipeline();
		}


	}
}