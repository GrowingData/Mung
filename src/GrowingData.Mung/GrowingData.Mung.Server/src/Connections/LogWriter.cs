using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Server {
	public class LogWriter : PersistentConnection {

		// All we have is a raw connection here, so track the Source
		// property from the first event that gets fired.
		public static Dictionary<string, string> _connectionToInstance = new Dictionary<string, string>();


		protected override Task OnReceived(IRequest request, string connectionId, string data) {

			var evt = new MungServerEvent(data);

			MungState.App.Pipeline.Process(evt);


			return base.OnReceived(request, connectionId, data);
		}


		protected override Task OnConnected(IRequest request, string connectionId) {
			return base.OnConnected(request, connectionId);
		}

		protected override Task OnDisconnected(IRequest request, string connectionId) {
			string instance = null;
			if (_connectionToInstance.TryGetValue(connectionId, out instance)) {
			}


			return base.OnDisconnected(request, connectionId);
		}

	}

}