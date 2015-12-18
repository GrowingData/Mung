
(function ($) {
	$.fn.mungView = function (viewModel) {
		var self = this;

		self.viewModel = viewModel;
		self.data("model", viewModel);

		self.content = self.find(".view-content");
		self.error = self.find(".view-error");
		self.content = self.find(".view-content");

		self.status = self.find(".status");

		setStatus("Initializing...");

		function parseFunctionBody(functionString) {
			var firstBrace = functionString.indexOf("{") + 1;
			var lastBrace = functionString.lastIndexOf("}");


			return functionString.substring(firstBrace, lastBrace);

		}

		this.refresh = function () {
			setStatus("Refreshing...");
			self.content.html(self.viewModel.data.Html);


			fn = Function("data", "$component", parseFunctionBody(self.viewModel.data.Js));

			// Firstly try to get the binding function
			var fn = null;
			try {
				fn = Function("data", "$component", parseFunctionBody(self.viewModel.data.Js));
			} catch (x) {
				self.error.html(JSON.stringify(x)).css("color", "red");
				setStatus("Javascript function parse error");
				console.log(x);
				return;
			}

			$.ajax({
				url: "/api/sql/mung",
				data: { sql: self.viewModel.data.Sql },
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
		}

		init();

		return this;
	}

}(jQuery));
