
(function ($) {
	// «
	$.fn.mungEditor = function (dashboard, component, mode) {
		var _ = this;


		_.dashboard = dashboard;
		_.component = component;
		_.textarea = _.find("textarea");

		this.keyMap = {
			name: "mung-execute-component",
			"Ctrl-Enter": function (cm) {
				_.component.execute();
			}
		}

		this.bindCodeMirror = function () {
			_.codeMirror = CodeMirror.fromTextArea(_.textarea[0], {
				mode: 'text/x-sql',
				matchBrackets: true,
				theme: 'light-table'
			});

			//this.sqlAutoComplete = new SqlAutoComplete(this);
			//this.codeMirror.on("change", _.textChanged);
			//this.codeMirror.on("cursorActivity", _.sqlAutoComplete.cursorActivity);

			_.codeMirrorDiv = _.find(".CodeMirror");


			// Bind our shortcuts
			this.codeMirror.addKeyMap(_.keyMap);

		}

		this.code = function(code) {
			if (!code){
				return _.codeMirror.getValue();
			}

			_.codeMirror.getDoc().setValue(code);
			_.codeMirror.refresh();
		}

		function init() {
			_.bindCodeMirror();
		}

		init();

		return this;
	}

}(jQuery));
