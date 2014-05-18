using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.SignalR;

namespace GrowingData.Mung.Server {
	public class LogReader : PersistentConnection {


		protected override Task OnReceived(IRequest request, string connectionId, string data) {
			dynamic d = JObject.Parse(data);

			if (d.type == "stream") {
				MungState.App.Pipeline.AddProcessor(new EventStream(Connection, connectionId));
			}


			if (d.type == "js-metric") {
				var jsId = (string)d.id;
				var keyFilter = (string)d.keyFilter;

				//if (string.IsNullOrEmpty((string)d.aggregator)) {
				// Watching a persistent metric
				var watcher = new JavascriptMetricConnection(MungState.App.Pipeline,
					(string)d.name,
					Connection,
					connectionId,
					jsId,
					keyFilter);
				//} else {

				//	// Watching a live / demo metric
				//	var watcher = new JavascriptMetricWatcher(MungState.App.Pipeline,
				//		(string)d.aggregator,
				//		(string)d.name,
				//		Connection,
				//		connectionId);
				//}
			}


			return base.OnReceived(request, connectionId, data);

		}


		protected override Task OnConnected(IRequest request, string connectionId) {
			//Manager.Connect(connectionId, this);
			return base.OnConnected(request, connectionId);

		}


		protected override Task OnDisconnected(IRequest request, string connectionId) {
			MungState.App.Pipeline.Disconnect(connectionId);
			return base.OnDisconnected(request, connectionId);


		}


		protected override Task OnReconnected(IRequest request, string connectionId) {
			//Manager.Connect(connectionId, this);
			return base.OnReconnected(request, connectionId);
		}


	}
}