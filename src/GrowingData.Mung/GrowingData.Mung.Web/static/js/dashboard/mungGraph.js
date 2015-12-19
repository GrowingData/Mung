
(function ($) {
	$.fn.mungGraph = function (graphModel, dashboard) {
		if (graphModel == null) throw "mungGraph: graphModel is null";

		var self = this;

		self.dashboard = dashboard;
		self.graphModel = graphModel;
		self.data("model", graphModel);

		self.content = self.find(".graph-content");
		self.error = self.find(".graph-error");
		self.content = self.find(".graph-content");
		self.title = self.find(".graph-title");

		self.edit = self.find(".edit-graph");

		self.status = self.find(".status");

		setStatus("Initializing...");

		function parseFunctionBody(functionString) {
			var firstBrace = functionString.indexOf("{") + 1;
			var lastBrace = functionString.lastIndexOf("}");


			return functionString.substring(firstBrace, lastBrace);

		}

		this.refresh = function () {
			setStatus("Refreshing...");
			self.content.html(self.graphModel.data.Html);
			self.title.text(self.graphModel.data.Title);

			fn = Function("data", "$component", parseFunctionBody(self.graphModel.data.Js));

			// Firstly try to get the binding function
			var fn = null;
			try {
				fn = Function("data", "$component", parseFunctionBody(self.graphModel.data.Js));
			} catch (x) {
				self.error.html(JSON.stringify(x)).css("color", "red");
				setStatus("Javascript function parse error");
				console.log(x);
				return;
			}

			$.ajax({
				url: "/api/sql/mung",
				data: { sql: self.graphModel.data.Sql },
				method: "POST",
				success: function (r) {
					setStatus("Binding...");
					try {
						fn(r, self);
						setStatus("Done");
					} catch (x) {
						self.error.html(JSON.stringify(x)).css("color", "red");
						setStatus("Binding error");
						console.log(x);
						return
					}
				},
				error: function (r) {
					self.error.html(JSON.stringify(r)).css("color", "red");
					setStatus("Sql / Server error");

				}
			});
		}
		
		function setStatus(text) {
			self.status.text(text);

		}

		function init() {
			self.refresh();
			self.edit.click(function () {
				self.dashboard.editGraph(self);

			});
		}

		init();

		return this;
	}

}(jQuery));
