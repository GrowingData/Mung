using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;

namespace GrowingData.Mung.Client {
	public class MungClient {
		private Connection _connection;
		private ConcurrentQueue<MungEvent> _q;
		private string _serverUrl;
		private string _connectionUrl;

		public MungClient(string serverHost) {
			_serverUrl = serverHost;
			_connectionUrl = _serverUrl + "/log/write";
			_connection = new Connection(_connectionUrl);

			// Ignore broken certs
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => {
				return true;
			};


			_q = new ConcurrentQueue<MungEvent>();
			Task.Run(() => ProcessQueue());
		}

		private void ProcessQueue() {
			while (true) {
				MungEvent msg;
				if (_connection.State == ConnectionState.Connected) {
					while (_q.TryDequeue(out msg)) {
						try {
							SimpleTimer.DebugTime("event-send", () => {
								_connection.Send(msg);
							});
							Debug.WriteLine("MUNG: sent: " + JsonConvert.SerializeObject(msg));
						} catch (Exception ex) {
							System.Diagnostics.Debug.WriteLine(string.Format("MUNG: Unable to send event: {0}", ex.Message));
						}
					}
				} else {
					if (_connection.State == ConnectionState.Disconnected) {
						System.Diagnostics.Debug.WriteLine(string.Format("MUNG: Connecting to {0}", _connectionUrl));
						_connection.Start();
						//Console.WriteLine("Mung not connected");
						Thread.Sleep(1000);
					}
				}

				Thread.Sleep(100);
			}

		}
		//public void WriteDirect(string source, string type, dynamic data) {
		//	while (true) {
		//		if (_connection.State == ConnectionState.Connected) {
		//			var msg = new MungEvent() {
		//				Data = data,
		//				Type = type,
		//				LogTime = DateTime.UtcNow,
		//			};
		//			_connection.Send(msg);
		//			return;
		//		} else {
		//			_connection.Start();
		//			System.Diagnostics.Debug.WriteLine(string.Format("MUNG: Not connected, unablet to WriteDirect"));
		//			Thread.Sleep(1000);
		//		}
		//	}
		//}
		public void WaitUntilQueueEmpty() {
			while (_q.Count > 0) {
				Thread.Sleep(10);
			}

		}

		public void Write(string source, string type, dynamic data) {
			_q.Enqueue(new MungEvent() {
				Data = data,
				Type = type,
				LogTime = DateTime.UtcNow,

			});

			// Make sure the queue doesn't get too long and break everything
			// by crashing the app, so remove old entries
			MungEvent msg;
			while (_q.Count > 1000) {
				_q.TryDequeue(out msg);

			}
		}


	}
}
