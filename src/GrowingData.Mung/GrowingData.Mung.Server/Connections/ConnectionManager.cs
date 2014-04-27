using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNet.SignalR;
using GrowingData.Mung.Core; 

namespace GrowingData.Mung.Server {
	public class ConnectionManager<T> where T : PersistentConnection {
		private object _sync = new object();
		private ConcurrentDictionary<string, T> _active = new ConcurrentDictionary<string, T>();

		public void Connect(string connectionId, T connection) {

			_active[connectionId] = connection;

		}

		public void Disconnect(string connectionId) {
			T obj;
			while (!_active.TryRemove(connectionId, out obj)) {
				Thread.Sleep(5);
			}
		}

		public void Broadcast(MungServerEvent message) {
			foreach (var cn in _active) {
				var connectionId = cn.Key;
				var connection = cn.Value.Connection;
				connection.Send(connectionId, message);
			}

		}

	}
}