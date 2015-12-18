
(function ($) {
	// «
	$.fn.mungHtmlEditor = function (dashboard, component) {
		var _ = this;

		_.component = component;
		_.dashboard = dashboard;

		_.editor = _.mungEditor(dashboard, component, "text/html");
		_.codeMirrorDiv = _.editor.codeMirrorDiv;


		return this;
	}

}(jQuery));