using System;
using GrowingData.Mung.Core;
using Microsoft.AspNet.SignalR;

namespace GrowingData.Mung.Server {
	public abstract class ConnectedEventProcessor : EventProcessor {


		private IConnection _connection;
		private string _connectionId;


		public string ConnectionId { get { return _connectionId; } }


		public ConnectedEventProcessor(string name, IConnection connection, string connectionId)
			: base(name) {

			_connection = connection;
			_connectionId = connectionId;
		}

		public void Send(object obj) {
			_connection.Send(_connectionId, obj);
		}


	}
}