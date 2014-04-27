using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.SignalR;

namespace GrowingData.Mung.Server {
	public class LogReader : PersistentConnection {


		protected override Task OnReceived(IRequest request, string connectionId, string data) {
			dynamic d = JObject.Parse(data);

			if (d.type == "stream") {
				EventSink.Sink.AddProcessor(new EventStream(Connection, connectionId));
			}




			return base.OnReceived(request, connectionId, data);

		}


		protected override Task OnConnected(IRequest request, string connectionId) {
			//Manager.Connect(connectionId, this);
			return base.OnConnected(request, connectionId);

		}


		protected override Task OnDisconnected(IRequest request, string connectionId) {
			EventSink.Sink.Disconnect(connectionId);
			return base.OnDisconnected(request, connectionId);


		}


		protected override Task OnReconnected(IRequest request, string connectionId) {
			//Manager.Connect(connectionId, this);
			return base.OnReconnected(request, connectionId);
		}


	}
}