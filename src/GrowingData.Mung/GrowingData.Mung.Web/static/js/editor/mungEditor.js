
(function ($) {
	// «
	$.fn.mungEditor = function (dashboard, graph, mode) {
		if (dashboard == null) throw "mungEditor: dashboard is null";
		if (graph == null) throw "mungEditor: graph is null";
		if (mode == null) throw "mungEditor: mode is null";

		var self = this;


		self.dashboard = dashboard;
		self.graph = graph;
		self.textarea = self.find("textarea");

		this.keyMap = {
			name: "mung-execute-graph",
			"Ctrl-Enter": function (cm) {
				self.graph.execute();
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
