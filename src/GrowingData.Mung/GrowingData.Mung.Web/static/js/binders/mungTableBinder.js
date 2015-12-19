
(function ($) {
	// «
	$.fn.mungTableBinder = function (data, formatMap) {
		if (data == null) throw "mungTableBinder: data is null";
		if (data.ColumnNames == null) throw "mungTableBinder: data.ColumnNames is missing";
		if (data.Rows == null) throw "mungTableBinder: data.Rows is missing";

		var self = this;


		this.bindTable = function () {
			self.empty();

			self.addClass("mung-table-binder");
			var table = $("<table>").addClass("mung-table table").appendTo(self);

			var tr = $("<tr>").appendTo(table);
			_.each(data.ColumnNames, function (column) {
				$("<th>").text(column).appendTo(tr);
			});
			_.each(data.Rows, function (row) {
				var tr = $("<tr>").appendTo(table);
				_.each(data.ColumnNames, function (column) {
					var val = row[column];
					if (formatMap && formatMap[column]) {
						val = formatMap[column](val);
					}
					$("<td>").text(val).appendTo(tr);
				});
			});




		}

		this.bindTable();

		return this;
	}

}(jQuery));
