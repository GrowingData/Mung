
(function ($) {
	$.fn.mungGraph = function (graphModel, dashboard) {
		if (graphModel == null) throw "mungGraph: graphModel is null";

		var self = this;

		self.dashboard = dashboard;
		self.graphModel = graphModel;
		self.data("model", graphModel);
		self.data("ref", this);

		self.content = self.find(".graph-content");
		self.error = self.find(".graph-error");
		self.content = self.find(".graph-content");
		self.title = self.find(".graph-title");

		self.edit = self.find(".edit-graph");
		self.status = self.find(".status");

		self.currentData = null;
		self.currentDataBinder = null;

		setStatus("Initializing...");

		function parseFunctionBody(functionString) {
			var firstBrace = functionString.indexOf("{") + 1;
			var lastBrace = functionString.lastIndexOf("}");


			return functionString.substring(firstBrace, lastBrace);

		}

		// Render method for drawing the current graph without doing a data
		// request (for resize events, etc).
		this.render = function () {
			setStatus("Rendering...");
			try {
				self.currentDataBinder(self.currentData, self);
				setStatus("Done");
			} catch (x) {
				self.error.text("Javascript data binding: " + x.toString()).show();
				setStatus("Binding error");
				console.log(x);
				return
			}

		}

		this.refresh = function () {
			setStatus("Refreshing...");
			self.content.html(self.graphModel.data.Html);
			self.title.text(self.graphModel.data.Title);

			//fn = Function("data", "$component", parseFunctionBody(self.graphModel.data.Js));

			// Firstly try to get the binding function
			var fn = null;
			try {
				fn = Function("data", "$component", parseFunctionBody(self.graphModel.data.Js));
			
			} catch (x) {
				self.error.text("Javascript parse error: " + x.toString()).show();
				setStatus("Javascript function parse error");
				console.log(x);
				return;
			}
			self.currentDataBinder = fn;
			$.ajax({
				url: "/api/sql/mung",
				data: { sql: self.graphModel.data.Sql },
				method: "POST",
				success: function (r) {
					self.currentData = r;
					self.render();
				},
				error: function (r) {
					self.error.text("SQL / Command Execution Error: " + r.toString()).show();
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
