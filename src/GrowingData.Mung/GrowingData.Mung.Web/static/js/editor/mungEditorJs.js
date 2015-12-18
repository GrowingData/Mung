
(function ($) {
	// «
	$.fn.mungEditorJs = function (dashboard, component) {
		var self = this;

		self.component = component;
		self.dashboard = dashboard;

		self.editor = self.mungEditor(dashboard, component, "text/javascript");
		self.codeMirrorDiv = self.editor.codeMirrorDiv;


		return this;
	}

}(jQuery));