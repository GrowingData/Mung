using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using GrowingData.Mung.Core;
using Newtonsoft.Json;




namespace GrowingData.Mung.Server {
	public class EventStream : ConnectedEventProcessor {
		public EventStream(EventPipeline pipeline, IConnection connection, string connectionId)
			: base(pipeline, "EventStream", connection, connectionId) {

		}


		protected override void ProcessEvent(MungServerEvent evt) {
			Send(evt);
		}

	}
}
