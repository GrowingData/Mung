
(function ($) {
	$.fn.mungComponentEditor = function (dashboard) {
		var _ = this;


		_.component = null;

		_.editorHtml = _.find(".editor-html");
		_.editorSql = _.find(".editor-sql");
		_.editorJs = _.find(".editor-js");

		_.saveButton = _.find(".btn-save").click(function () {
			_.save(function () {
				alert("Saved")
			});
		});


		_.previewButton = _.find(".btn-preview").click(function () {
			var preview = _.find(".component.preview").mungComponent(_.readUi());
			preview.refresh();
		});


		// Actually bind codeMirror after the modal has appeared to 
		// so that CodeMirror displays code without you needing to click on it
		_.on('shown.bs.modal', function () {

			_.editorHtml.editor.code(_.component.Html);
			_.editorSql.editor.code(_.component.Sql);
			_.editorJs.editor.code(_.component.Js);
		});

		_.create = function () {
			_.component = {
				ComponentId: -1,
				Html: $(".default-html").html(),
				Sql: $(".default-sql").text(),
				Js: $(".default-js").text(),
				PositionX: -1,
				PositionY: -1,
				Width: 100,
				Height: 50,
			};

			_.find(".modal-title .edit").hide();
			_.find(".modal-title .add").show();

			_.modal('show');

		}

		_.edit = function (component) {
			_.component = component;

			_.find(".modal-title .edit").show();
			_.find(".modal-title .add").hide();


			_.modal('show');

		}

		_.readUi = function () {
			return {
				ComponentId: _.component.ComponentId,

				Html: _.editorHtml.editor.code(),
				Sql: _.editorSql.editor.code(),
				Js: _.editorJs.editor.code(),

				PositionX: _.component.PositionX,
				PositionY: _.component.PositionY,
				Width: _.component.Width,
				Height: _.component.Height,
			}
		}



		_.save = function (callback) {
			$.ajax({
				url: "/api/dashboard/component",
				data: {
					"url": dashboard.Url,
					"componentJson": JSON.stringify(_.readUi())
				},
				method: "POST",
				success: function (response) {
					callback(response);
				},
				error: function (a, b, c, d) {
					console.error("Error: mungComponentEditor.save", { a: a, b: b, c: c, d: d });
				}
			});
		}


		function initializeEditors() {
			_.editorSql.mungEditorSql(dashboard, _);
			_.editorHtml.mungEditorSql(dashboard, _);
			_.editorJs.mungEditorSql(dashboard, _);
		}

		initializeEditors();
		return this;
	}

}(jQuery));
