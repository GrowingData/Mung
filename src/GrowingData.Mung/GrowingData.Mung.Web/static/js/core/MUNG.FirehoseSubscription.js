
var MUNG = MUNG || {};

MUNG.FirehoseSubscription = function (settings) {
	var self = this;

	function initializeDefaultSetttings() {
		self.settings = settings || {};

		// Settings looks like:
		self.settings = {
			filter: settings.filter || function () { return true; },
			received: settings.received || function (evt) { },
			host: settings.host || ""
		};
	}

	this.connected = function () {
		self.connection.send({
			type: "stream",
			filter: self.settings.filter.toString()
		});
		console.log("Sent subscription");
	}

	function init() {
		initializeDefaultSetttings();
		self.connection = $.connection(self.settings.host + '/log/read');
		self.connection.received(function (data) {
			console.log("Received.");
			self.settings.received(data);
		});


		self.connection.start();

		self.connection.reconnecting(function () {


		});

		self.connection.reconnected(function () {

		});

		self.connection.disconnected(function () {

		});

		self.connection.error(function (errorData, sendData) {
			console.error(errorData);
		});

		self.connection.stateChanged(function (state) {
			var connectionState = {
				connecting: 0,
				connected: 1,
				reconnecting: 2,
				disconnected: 4
			};
			console.log("Connection state: " + JSON.stringify(state));

			if (state.newState == connectionState.connected) {
				self.connected();
			}
		});
	}




	init();


}