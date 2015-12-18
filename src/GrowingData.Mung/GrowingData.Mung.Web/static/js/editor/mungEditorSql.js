
(function ($) {
	// «
	$.fn.mungEditorSql = function (dashboard, component) {
		var _ = this;

		_.component = component;
		_.dashboard = dashboard;

		_.editor = _.mungEditor(dashboard, component, "text/x-sql");
		_.codeMirrorDiv = _.editor.codeMirrorDiv;

		function getSchemata() {
			$.ajax({
				url: "/api/schema/mung",
				method: "GET",
				success: function (r) {
					_.autoComplete = new AutoCompleteSql(_, r.Schema);
					_.editor.codeMirror.on("cursorActivity", _.autoComplete.cursorActivity);
				}
			});

		}

		getSchemata();


		return this;
	}

}(jQuery));
