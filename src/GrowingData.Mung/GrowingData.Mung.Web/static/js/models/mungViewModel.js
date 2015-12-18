
function mungViewModel(data, dashboard) {
	var self = this;
	self.data = data;

	this.setDimensions = function (x, y, width, height) {

		self.data.PositionX = x;
		self.data.PositionY = y;
		self.data.Width = width;
		self.data.Height = height;

		self.save(function () {
			//alert("saved");
		})
	}

	this.setPosition = function (position) {
		self.data.PositionX = position.left;
		self.data.PositionY = position.top;

		self.save(function () {
			//alert("saved");
		})
	}

	this.update = function (newViewData) {
		self.data = newViewData;
	}

	this.save = function (callback) {
		$.ajax({
			url: "/api/dashboard/component",
			data: {
				"url": dashboard.Url,
				"componentJson": JSON.stringify(self.data)
			},
			method: "POST",
			success: function (response) {
				callback(response);
			},
			error: function (a, b, c, d) {
				console.error("Error: mungViewEditor.save", { a: a, b: b, c: c, d: d });
			}
		});
	}


	return this;
}
