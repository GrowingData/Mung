using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrowingData.Mung.Core;
using GrowingData.Mung.Metric;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using System.IO;



namespace GrowingData.Mung.Web {
	public class MetricConnection {


		private IConnection _connection;
		private string _connectionId;

		private JsonMetric _jsonMetric;
		private string _clientId;

		public string ConnectionId { get { return _connectionId; } }


		/// <summary>
		/// Bind to an existing metric given by "name"
		/// </summary>
		/// <param name="pipeline"></param>
		/// <param name="name"></param>
		/// <param name="connection"></param>
		/// <param name="connectionId"></param>
		public MetricConnection(EventPipeline pipeline, IConnection connection, string connectionId, string jsonMetric, string clientId) {
			_connection = connection;
			_connectionId = connectionId;
			_clientId = clientId;
			_jsonMetric = JsonConvert.DeserializeObject<JsonMetric>(jsonMetric);



			// Try to find the metric, and add a watcher
			var metric = pipeline.GetProcessor(_jsonMetric.Name) as MetricProcessor;

			if (metric == null) {
				// Its new so we need to create a new one...
				var filePath = Path.Combine(PathManager.BasePath, "user", "metric", _jsonMetric.Name + ".json");
				//if (!File.Exists(filePath)) {
				var formattedJson = JsonConvert.SerializeObject(_jsonMetric, Formatting.Indented);
				File.WriteAllText(filePath, formattedJson);

				//}

				metric = new MetricProcessor(_jsonMetric.Name, _jsonMetric);
				pipeline.AddProcessor(metric);
			}

			metric.Updated(_connectionId, WriteValueUpdate);
			metric.Error(_connectionId, WriteErrorUpdate);


			SendUpdates(metric);
		}

		public void WriteErrorUpdate(string message) {
			_connection.Send(_connectionId, new {
				ClientId = _clientId,
				Error = message,
				Success = false
			});
		}

		public void WriteValueUpdate(MetricProcessor metric) {
			SendUpdates(metric);
		}

		public void SendUpdates(MetricProcessor metric) {
			_connection.Send(_connectionId, new {
				ClientId = _clientId,
				Metric = metric.CurrentState.Values,
				Success = true
			});
		}

	}
}
