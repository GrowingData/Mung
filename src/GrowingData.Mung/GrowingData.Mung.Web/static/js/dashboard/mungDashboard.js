
(function ($) {
	// «
	$.fn.mungDashboard = function (dashboard, views) {
		var self = this;
		self.dashboard = dashboard;


		self.componentPopup = $("#edit-view").mungViewEditor(dashboard);

		self.addComponentButton = $("#add-view").click(function () {
			self.componentPopup.create();
		});

		function bindGridStack() {
			self.viewHolder = self.find(".view-holder")
				.gridstack({
					width: 12,
					always_show_resize_handle: /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),
					resizable: {
						handles: 'e, se, s, sw, w'
					}
				});

			self.grid = self.viewHolder.data("gridstack");

			self.viewHolder.on('change', function (e, items) {
				for (var i = 0; i < items.length; i++) {
					var item = items[i];
					if (item._dirty) {
						var model = item.el.data("model");
						model.setDimensions(item.x, item.y, item.width, item.height);
						console.log(item);
						item._dirty = false;
					}
				}
			});

			//self.viewHolder.on('resizestop', function (event, ui) {
			//	var grid = this;
			//	var element = event.target;
			//	// Update the component
			//	$(element).data("model").setDimensions(ui.position, ui.size);
			//});

			//self.viewHolder.on('dragstop', function (event, ui) {
			//	var grid = this;
			//	var element = event.target;
			//	// Update the component
			//	$(element).data("model").setPosition(ui.position);
			//});
		}

		function addViews() {
			_.each(views, function (v, k, l) {
				var model = new mungViewModel(v, dashboard);
				var view = $(".view.template")
					.clone()
					.removeClass("template")
					//.appendTo(self.viewHolder)
					.mungView(model);

				self.grid.add_widget(view, v.PositionX, v.PositionY, v.Width, v.Height);
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
