
(function ($) {
	// «
	$.fn.mungDashboard = function (dashboardModel, graphs) {
		if (dashboardModel == null) throw "mungDashboard: dashboardModel is null";
		if (graphs == null) throw "mungDashboard: graphs is null";


		var self = this;
		self.dashboardModel = dashboardModel;


		self.componentPopup = $("#edit-graph").mungGraphEditor(dashboardModel);

		self.addComponentButton = $("#add-graph").click(function () {
			self.componentPopup.create();
		});

		this.editGraph = function (graph) {
			self.componentPopup.edit(graph.graphModel);
		}

		function bindGridStack() {
			self.graphHolder = self.find(".graph-holder")
				.gridstack({
					width: 12,
					always_show_resize_handle: /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),
					resizable: {
						handles: 'e, se, s, sw, w'
					}
				});

			self.grid = self.graphHolder.data("gridstack");

			self.graphHolder.on('change', function (e, items) {
				for (var i = 0; i < items.length; i++) {
					var item = items[i];
					if (item._dirty) {
						var model = item.el.data("model");
						var ref = item.el.data("ref");
						model.setDimensions(item.x, item.y, item.width, item.height);
						console.log(item);
						item._dirty = false;

						// Changing the size of them element might necessitate
						// a re-render (especially for d3 stuff)
						if (ref.render) {
							ref.render();
						}
					}
				}
			});

		}

		function addViews() {
			_.each(graphs, function (graphData, key, l) {
				var model = new mungGraphModel(graphData, dashboardModel);
				var graph = $(".graph.template")
					.clone()
					.removeClass("template")
					.mungGraph(model, self);

				self.grid.add_widget(graph, graphData.X, graphData.Y, graphData.Width, graphData.Height);
			});

		}


		function init() {
			bindGridStack();

			addViews();
		}


		init();

		return this;
	}

}(jQuery));
