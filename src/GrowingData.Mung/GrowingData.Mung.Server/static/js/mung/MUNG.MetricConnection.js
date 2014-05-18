
var MUNG = MUNG || {};

MUNG.MetricConnection = function (settings) {
	var _ = this;

	this.state = -1;

	this.metrics = {};

	function initializeDefaultSetttings() {
		_.settings = settings || {};

		// Settings looks like:
		_.settings = {
			host: settings.host || "",
		};
	}

	this.listen = function (metric) {
		var id = Math.random().toString().split('.')[1];
		metric.id = id;

		_.metrics[id] = metric;

		if (_.state.newState == connectionState.connected) {
			_.connection.send({
				type: "js-metric",
				name: metric.name,
				id: metric.id,
				keyFilter: metric.keyFilter
			});
			console.log("Sent subscription: " + metric.name);

		}
	}


	this.waitUntil = function (fn, cb) {
		if (!fn()) {
			setTimeout(function () { _.waitUntil() }, 100);
			return;
		}
		cb();
	}

	this.connected = function () {
		for (var k in _.metrics) {
			// Make references to the metrics we are watching
			var metric = _.metrics[k];

			_.connection.send({
				type: "js-metric",
				name: metric.name,
				id: metric.id,
				keyFilter: metric.keyFilter
			});
		}
		console.log("Sent subscription");
	}

	function init() {
		initializeDefaultSetttings();
		_.connection = $.connection(_.settings.host + '/log/read');

		_.connection.received(function (data) {
			if (_.metrics[data.id]) {
				_.metrics[data.id].updated(data);
			}

			console.log("Updated.");
		});


		_.connection.start();

		_.connection.reconnecting(function () {


		});

		_.connection.reconnected(function () {

		});

		_.connection.disconnected(function () {

		});

		_.connection.error(function (errorData, sendData) {
			console.error(errorData);
		});

		_.connection.stateChanged(function (state) {

			console.log("Connection state: " + JSON.stringify(state));

			_.state = state.newState;
			if (state.newState == connectionState.connected) {
				_.connected();
			}
		});
	}


	var connectionState = {
		connecting: 0,
		connected: 1,
		reconnecting: 2,
		disconnected: 4
	};

	init();


}