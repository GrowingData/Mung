using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrowingData.Mung.Core;
using GrowingData.Mung.MetricJs;
using Microsoft.AspNet.SignalR;



namespace GrowingData.Mung.Server {
	public class JavascriptMetricWatcher {


		private IConnection _connection;
		private string _connectionId;


		public string ConnectionId { get { return _connectionId; } }

		/// <summary>
		/// Create a new, ephermal metric to watch
		/// </summary>
		/// <param name="pipeline"></param>
		/// <param name="jsAggregator"></param>
		/// <param name="name"></param>
		/// <param name="connection"></param>
		/// <param name="connectionId"></param>
		public JavascriptMetricWatcher(EventPipeline pipeline, string jsAggregator, string name, IConnection connection, string connectionId) {
			_connection = connection;
			_connectionId = connectionId;

			var jsMetric = new JavascriptMetric(name, jsAggregator);

			jsMetric.Updated(connectionId, Updated);

			pipeline.AddProcessor(jsMetric);
		}

		/// <summary>
		/// Bind to an existing metric given by "name"
		/// </summary>
		/// <param name="pipeline"></param>
		/// <param name="name"></param>
		/// <param name="connection"></param>
		/// <param name="connectionId"></param>
		public JavascriptMetricWatcher(EventPipeline pipeline, string name, IConnection connection, string connectionId) {
			_connection = connection;
			_connectionId = connectionId;

			// Try to find the metric, and add a watcher
			var processor = pipeline.GetProcessor(name);

			if (processor == null) {
				throw new Exception(string.Format("Unable to find metric with name {0}", name));
			}

			var jsMetric = processor as JavascriptMetric;
			if (jsMetric == null) {
				throw new Exception(string.Format("Found metric with name {0}, but its not a JavascriptMetric", name));
			}

			jsMetric.Updated(_connectionId, Updated);


		}

		public void Updated(JaascriptMetricUpdate d) {
			_connection.Send(_connectionId, d);

		}

	}
}
