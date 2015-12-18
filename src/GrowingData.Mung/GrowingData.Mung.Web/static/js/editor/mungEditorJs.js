
(function ($) {
	// «
	$.fn.mungEditorJs = function (dashboard, component) {
		var _ = this;

		_.component = component;
		_.dashboard = dashboard;

		_.editor = _.mungEditor(dashboard, component, "text/javascript");
		_.codeMirrorDiv = _.editor.codeMirrorDiv;


		return this;
	}

}(jQuery));