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



namespace GrowingData.Mung.Web {
	public class JavascriptMetricConnection {


		private IConnection _connection;
		private string _connectionId;
		private TimePeriod _timePeriod;
		private string _javascriptId;
		private string _keyFilter;


		public string ConnectionId { get { return _connectionId; } }

		///// <summary>
		///// Create a new, ephermal metric to watch
		///// </summary>
		///// <param name="pipeline"></param>
		///// <param name="jsAggregator"></param>
		///// <param name="name"></param>
		///// <param name="connection"></param>
		///// <param name="connectionId"></param>
		//public JavascriptMetricConnection(EventPipeline pipeline, string jsAggregator, string name, IConnection connection, string connectionId, string javascriptId) {
		//	_connection = connection;
		//	_javascriptId = javascriptId;
		//	_connectionId = connectionId;

		//	var jsMetric = new JavascriptMetric(name, jsAggregator);

		//	jsMetric.Updated(connectionId, Updated);

		//	pipeline.AddProcessor(jsMetric);

		//	SendUpdates();
		//}

		/// <summary>
		/// Bind to an existing metric given by "name"
		/// </summary>
		/// <param name="pipeline"></param>
		/// <param name="name"></param>
		/// <param name="connection"></param>
		/// <param name="connectionId"></param>
		public JavascriptMetricConnection(EventPipeline pipeline, string name, IConnection connection, string connectionId, string javascriptId, string keyFilter, TimePeriod period) {
			_connection = connection;
			_connectionId = connectionId;
			_javascriptId = javascriptId;
			_timePeriod = period;
			_keyFilter = PeriodMetric.BaseKey(name, _timePeriod, keyFilter);

			if (string.IsNullOrEmpty(_keyFilter)) {
				_keyFilter = "*";
			}

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

			SendUpdates();
		}

		public void Updated(List<JavascriptMetricUpdate> updates) {

			var filterMatching = updates
				.Where(x => x.Filter.Like(_keyFilter));

			if (filterMatching.Count() > 0) {
				_connection.Send(_connectionId, new {
					id = _javascriptId,
					keyFilter = _keyFilter,
					metrics = filterMatching
				});
				SendUpdates();
			}
		}

		public void SendUpdates() {
			// When an update has been triggered, we need to get the data from redis
			// and send the whole lot to the client.
			var values = RedisClient.Current.MatchingValues(_keyFilter);

		}

	}
}
