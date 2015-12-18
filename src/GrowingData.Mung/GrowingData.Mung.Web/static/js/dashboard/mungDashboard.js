
(function ($) {
	// «
	$.fn.mungDashboard = function (d) {
		var _ = this;
		_.d = d;


		_.componentPopup = $("#edit-component").mungComponentEditor(d);

		_.addComponentButton = $("#add-component").click(function () {
			_.componentPopup.create();

		});


		return this;
	}

}(jQuery));
