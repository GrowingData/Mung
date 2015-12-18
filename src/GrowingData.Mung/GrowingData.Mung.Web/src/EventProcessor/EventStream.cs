using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using GrowingData.Mung.Core;
using Newtonsoft.Json;




namespace GrowingData.Mung.Web {
	public class EventStream : ConnectedEventProcessor {
		public EventStream(IConnection connection, string connectionId)
			: base("EventStream", connection, connectionId) {

		}


		protected override void ProcessEvent(MungServerEvent evt) {
			Send(evt);
		}

	}
}
