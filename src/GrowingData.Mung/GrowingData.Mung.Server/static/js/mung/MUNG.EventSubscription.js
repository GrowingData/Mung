
var MUNG = MUNG || {};

MUNG.EventSubscription = function (settings) {
	var _ = this;

	function initializeDefaultSetttings() {
		_.settings = settings || {};

		// Settings looks like:
		_.settings = {
			filter: settings.filter || function () { return true; },
			received: settings.received || function (evt) { },
			host: settings.host || ""
		};
	}

	this.connected = function () {
		_.connection.send({
			type: "stream",
			filter: _.settings.filter.toString()
		});
		console.log("Sent subscription");
	}

	function init() {
		initializeDefaultSetttings();
		_.connection = $.connection(_.settings.host + '/log/read');
		_.connection.received(function (data) {
			console.log("Received.");
			_.settings.received(data);
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
				_.connected();
			}
		});
	}




	init();


}