
var MUNG = MUNG || {};

MUNG.MetricSubscription = function (settings) {
	var _ = this;

	function initializeDefaultSetttings() {
		_.settings = settings || {};

		// Settings looks like:
		_.settings = {
			name: settings.name || "Untitled",
			host: settings.host || "",
			aggregator: settings.aggregator || "",
			updated: settings.updated || function (evt) { }
		};
	}

	this.connected = function () {
		_.connection.send({
			type: "js-metric",
			aggregator: _.settings.aggregator.toString(),
			name: _.settings.name 
		});
		console.log("Sent subscription");
	}

	var cnCount = 0;
	function init() {
		initializeDefaultSetttings();
		_.connection = $.connection(_.settings.host + '/log/read');

		_.connection.received(function (data) {
			console.log("Updated.");
			_.settings.updated(data);
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
			var connectionState = {
				connecting: 0,
				connected: 1,
				reconnecting: 2,
				disconnected: 4
			};
			console.log("Connection state: " + JSON.stringify(state));

			if (state.newState == connectionState.connected) {
				if (cnCount==0){
					_.connected();
					cnCount++
				}
			}
		});
	}




	init();


}