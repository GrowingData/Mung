
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

	this.listen = function ($elem) {
		var clientId = $elem.data("clientId");
		if (!clientId) {
			clientId = Math.random().toString().split('.')[1];
			$elem.data("clientId", clientId);
			$elem.addClass("mung-client-id-" + clientId);
		}


		var serverMetric = {
			Name: $elem.data("name"),
			EventType: $elem.data("type"),
			Aggregate: $elem.data("aggregate"),
			Filter: $elem.data("filter"),
			Group: $elem.data("group"),
			Period: $elem.data("period")
		};

		var container = {
			elem: $elem,
			serverMetric: serverMetric,
			ClientId: $elem.data("clientId")
		}


		this.metrics[clientId] = container;

		if (_.state.newState == connectionState.connected) {
			_.connection.send({
				type: "metric",
				clientId: container.ClientId,
				metric: container.serverMetric
			});

			console.log("Sent subscription: " + container.serverMetric.Name);

		}
	}

	this.updated = function (container, data) {
		console.log("Updatint: " + data.ClientId);

		var elem = container.elem;
		var ul = elem.find("ul.mung-values");
		if (ul.length == 0) {
			ul = $("<ul>").addClass("mung-values").appendTo(elem);
		}

		elem.find("li.value").addClass("unseen");

		for (var i = 0; i < data.Metric.Values.length; i++) {
			var v = data.Metric.Values[i];
			var className = "g-" + hex_md5(v.Group);

			var li = elem.find("li.value." + className);
			if (li.length == 0) {
				li = $("<li>")
					.addClass("value")
					.addClass("className")
					.appendTo(ul);
			}else{
				li.removeClass("unseen");
			}

			$("<label>").addClass("added").text(decodeURIComponent(v.Group)).appendTo(li);
			$("<div>").addClass("added").text(v.Value).appendTo(li);
		}

		// If ".unseen" hasn't been removed then we didnt
		// have any values in the update, so we want to remove 
		// the item.
		elem.find("li.value.unseen").remove();
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
			var container = _.metrics[k];

			_.connection.send({
				type: "metric",
				clientId: container.ClientId,
				metric: container.serverMetric
			});
			console.log("Sent subscription: " + container.serverMetric.Name);
		}
	}

	function init() {
		initializeDefaultSetttings();
		_.connection = $.connection(_.settings.host + '/log/read');

		_.connection.received(function (data) {
			if (data.Success) {
				var container = _.metrics[data.ClientId];

				if (container != null) {
					_.updated(container, data);
				}

			} else {
				console.log("An error occurred. clientId:" + data.ClientId + ", message: '" + data.Error + "'");
			}
			console.log("Updated.");
		});



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


		_.connection.start();
	}


	var connectionState = {
		connecting: 0,
		connected: 1,
		reconnecting: 2,
		disconnected: 4
	};

	init();


}