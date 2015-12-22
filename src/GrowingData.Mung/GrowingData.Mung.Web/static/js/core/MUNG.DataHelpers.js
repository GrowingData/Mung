"use strict";

var MUNG = MUNG || {};

MUNG.DataHelpers = {

	formatCurrency: function (v) {
		return "$" + d3.format(',.2s')(v).replace("G", "B");
	},

	formatInt: function (v) {
		return d3.format('.0f')(v)
	},
	formatFloat: function (v) {
		return d3.format(',.2r')(v)
	},
	formatDays: function (v) {
		return parseInt(v) + " days";
	},
	formatPercent: function (v) {
		return d3.format('%')(v);
	},

	format: function (unit) {
		if (unit == "$") return MUNG.DataHelpers.formatCurrency;
		if (unit == "days") return MUNG.DataHelpers.formatDays;
		if (unit == "%") return MUNG.DataHelpers.formatPercent;
		if (unit == "i") return MUNG.DataHelpers.formatInt;
		if (unit == "f") return MUNG.DataHelpers.formatFloat;

		return MUNG.DataHelpers.formatInt;
	},

	formatDate_YYYMMDD: function(d){
		return d3.time.format("%Y-%m-%d")(new Date(d));
	},

	parseDate: function (input) {
		if (input.indexOf('T') > 0) {
			input = input.split('T')[0];
		}
		var parts = input.split('-');
		// new Date(year, month [, day [, hours[, minutes[, seconds[, ms]]]]])
		return new Date(parts[0], parts[1] - 1, parts[2]); // Note: months are 0-based
	},

	getOrdinal: function (n) {
		var s = ["th", "st", "nd", "rd"],
			v = n % 100;
		return n + (s[(v - 20) % 10] || s[v] || s[0]);
	}

};
