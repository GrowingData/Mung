
(function ($) {
	$.fn.mungViewEditor = function (dashboard) {
		var self = this;


		self.viewModel = null;

		self.editorHtml = self.find(".editor-html");
		self.editorSql = self.find(".editor-sql");
		self.editorJs = self.find(".editor-js");

		self.saveButton = self.find(".btn-save").click(function () {
			self.viewModel.update(self.readUi());
			self.viewModel.save(function () {
				alert("Saved");
			});
		});

		// Actually bind codeMirror after the modal has appeared to 
		// so that CodeMirror displays code without you needing to click on it
		self.on('shown.bs.modal', function () {

			self.editorHtml.editor.code(self.viewModel.Html);
			self.editorSql.editor.code(self.viewModel.Sql);
			self.editorJs.editor.code(self.viewModel.Js);
		});


		self.previewButton = self.find(".btn-preview").click(function () {
			var preview = self.find(".viewModel.preview").mungView(self.readUi());
			preview.refresh();
		});



		this.create = function () {
			self.viewModel = new mungViewModel({
				ComponentId: -1,
				Html: $(".default-html").html(),
				Sql: $(".default-sql").text(),
				Js: $(".default-js").text(),
				PositionX: -1,
				PositionY: -1,
				Width: 100,
				Height: 50,
			});

			self.find(".modal-title .edit").hide();
			self.find(".modal-title .add").show();

			self.modal('show');

		}

		this.edit = function (viewModel) {
			self.viewModel = viewModel;

			self.find(".modal-title .edit").show();
			self.find(".modal-title .add").hide();


			self.modal('show');

		}

		this.readUi = function () {
			return {
				ComponentId: self.viewModel.ComponentId,

				Html: self.editorHtml.editor.code(),
				Sql: self.editorSql.editor.code(),
				Js: self.editorJs.editor.code(),

				PositionX: self.viewModel.PositionX,
				PositionY: self.viewModel.PositionY,
				Width: self.viewModel.Width,
				Height: self.viewModel.Height,
			}
		}



		function initializeEditors() {
			self.editorSql.mungEditorSql(dashboard, self);
			self.editorHtml.mungEditorSql(dashboard, self);
			self.editorJs.mungEditorSql(dashboard, self);
		}

		initializeEditors();
		return this;
	}

}(jQuery));
