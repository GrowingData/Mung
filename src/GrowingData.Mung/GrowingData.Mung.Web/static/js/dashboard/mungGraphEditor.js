
(function ($) {
	$.fn.mungGraphEditor = function (dashboard) {
		if (dashboard == null) throw "mungGraphEditor: dashboard is null";

		var self = this;

		
		self.graphModel = null;

		self.editorHtml = self.find(".editor-html");
		self.editorSql = self.find(".editor-sql");
		self.editorJs = self.find(".editor-js");

		self.saveButton = self.find(".btn-save").click(function () {
			self.graphModel.update(self.readUi());
			self.graphModel.save(function () {
				alert("Saved");
			});
		});

		self.closeButton = self.find(".btn-close").click(function () {
			self.modal('hide');
		});

		// Actually bind codeMirror after the modal has appeared to 
		// so that CodeMirror displays code without you needing to click on it
		self.on('shown.bs.modal', function () {
			self.editorHtml.editor.code(self.graphModel.data.Html);
			self.editorSql.editor.code(self.graphModel.data.Sql);
			self.editorJs.editor.code(self.graphModel.data.Js);
		});


		self.previewButton = self.find(".btn-preview").click(function () {
			self.find(".graph.preview").empty();

			var previewModel = new mungGraphModel(self.readUi(), dashboard);

			var graph = $(".graph.template")
				.clone()
				.removeClass("template")
				.appendTo(self.find(".graph.preview"))
				.mungGraph(previewModel);

		});



		this.create = function () {
			self.graphModel = new mungGraphModel({
				GraphId: -1,
				Html: $(".default-html").html(),
				Sql: $(".default-sql").text(),
				Js: $(".default-js").text(),
				X: -1,
				Y: -1,
				Width: 1,
				Height: 1,
			}, dashboard);

			self.graphModel.saved(function () {
				document.location.reload();
				self.modal('hide');
			});

			self.find(".modal-title .edit").hide();
			self.find(".modal-title .add").show();

			self.modal('show');

		}

		this.edit = function (graphModel) {
			self.graphModel = graphModel;

			self.graphModel.saved(function () {
				document.location.reload();
				self.modal('hide');
			});

			self.find(".modal-title .edit").show();
			self.find(".modal-title .add").hide();

			self.find(".graph-title").val(graphModel.data.Title);

			self.modal('show');

		}

		this.readUi = function () {
			return {
				GraphId: self.graphModel.data.GraphId,

				Title: self.find(".graph-title").val(),

				Html: self.editorHtml.editor.code(),
				Sql: self.editorSql.editor.code(),
				Js: self.editorJs.editor.code(),

				X: self.graphModel.data.X,
				Y: self.graphModel.data.Y,
				Width: self.graphModel.data.Width,
				Height: self.graphModel.data.Height,
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
