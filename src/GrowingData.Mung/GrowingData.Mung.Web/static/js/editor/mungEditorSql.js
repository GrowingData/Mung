
(function ($) {
	// «
	$.fn.mungEditorSql = function (dashboard, component) {
		var self = this;

		self.component = component;
		self.dashboard = dashboard;

		self.editor = self.mungEditor(dashboard, component, "text/x-sql");
		self.codeMirrorDiv = self.editor.codeMirrorDiv;

		function getSchemata() {
			$.ajax({
				url: "/api/schema/mung",
				method: "GET",
				success: function (r) {
					self.autoComplete = new AutoCompleteSql(self, r.Schema);
					self.editor.codeMirror.on("cursorActivity", self.autoComplete.cursorActivity);
				}
			});

		}

		getSchemata();


		return this;
	}

}(jQuery));
