
(function ($) {
	// «
	$.fn.mungHtmlEditor = function (dashboard, component) {
		var self = this;

		self.component = component;
		self.dashboard = dashboard;

		self.editor = self.mungEditor(dashboard, component, "text/html");
		self.codeMirrorDiv = self.editor.codeMirrorDiv;


		return this;
	}

}(jQuery));