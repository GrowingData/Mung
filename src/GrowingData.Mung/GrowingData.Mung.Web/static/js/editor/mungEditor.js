
(function ($) {
	// «
	$.fn.mungEditor = function (dashboard, component, mode) {
		var self = this;


		self.dashboard = dashboard;
		self.component = component;
		self.textarea = self.find("textarea");

		this.keyMap = {
			name: "mung-execute-component",
			"Ctrl-Enter": function (cm) {
				self.component.execute();
			}
		}

		this.bindCodeMirror = function () {
			self.codeMirror = CodeMirror.fromTextArea(self.textarea[0], {
				mode: 'text/x-sql',
				matchBrackets: true,
				theme: 'light-table'
			});

			self.codeMirrorDiv = self.find(".CodeMirror");


			// Bind our shortcuts
			this.codeMirror.addKeyMap(self.keyMap);

		}

		this.code = function(code) {
			if (!code){
				return self.codeMirror.getValue();
			}

			self.codeMirror.getDoc().setValue(code);
			self.codeMirror.refresh();
		}

		function init() {
			self.bindCodeMirror();
		}

		init();

		return this;
	}

}(jQuery));
