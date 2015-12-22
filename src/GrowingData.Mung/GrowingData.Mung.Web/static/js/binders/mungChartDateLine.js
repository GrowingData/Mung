
(function ($) {
	// «
	$.fn.mungChartDateLine = function (data, settings) {
		if (data == null) throw "mungChartDateLine: data is null";
		if (data.ColumnNames == null) throw "mungChartDateLine: data.ColumnNames is missing";
		if (data.Rows == null) throw "mungChartDateLine: data.Rows is missing";

		if (settings.dateColumn == null) throw "mungChartDateLine: Please specify settings.dateColumn";
		if (settings.seriesColumns == null) throw "mungChartDateLine: Please specify settings.seriesColumns";
		if (settings.yFormatter == null) throw "mungChartDateLine: Please specify settings.yFormatter(d)";

		var self = this;

		var containerHeight = self.parents(".graph").height() - 55;
		if (containerHeight < 100) {
			containerHeight = 100;

		}

		self.id = "r" + Math.random().toString().split(".")[1];
		self
			.attr("id", self.id)
			.css("height", containerHeight + "px");

		this.getSeriesFromColumns = function () {
			// Use series columns to build out the charts series' data
			var seriesMap = {}

			_.each(settings.seriesColumns, function (column) {
				if (column.name == null) throw "column objects must specify a .name";
				if (column.color == null) throw "column objects must specify a .color";

				seriesMap[column.name] = {
					key: column.name,
					values: [],
					color: column.color
				}

			});

			// Bind the actual series data
			_.each(data.Rows, function (row) {
				_.each(settings.seriesColumns, function (column) {

					seriesMap[column.name].values.push({
						x: MUNG.DataHelpers.parseDate(row[settings.dateColumn]),
						y: parseFloat(row[column.name]),
					});
				});

			});

			var list = []
			for (var k in seriesMap) {
				list.push(seriesMap[k]);
			}
			return list;
		}

		this.render = function () {
			this.bindChart();
		}

		this.bindChart = function () {
			self.empty();

			var series = null;
			if (settings.seriesColumns != null) {
				series = this.getSeriesFromColumns();

			}

			var svg = self.append("<svg>");

			nv.addGraph(function () {
				//var chart = nv.models.stackedAreaChart()
				var chart = nv.models.lineChart()
					.useInteractiveGuideline(true)  //We want nice looking tooltips and a guideline!
					.showLegend(true)       //Show the legend, allowing users to turn on/off line series.
					.showYAxis(true)        //Show the y-axis
					.showXAxis(true)        //Show the x-axis

				chart.xAxis.tickFormat(function (d) {
					return MUNG.DataHelpers.formatDate_YYYMMDD(d);
				});

				chart.yAxis.tickFormat(function (d) {
					return settings.yFormatter(d);
				});
				//chart.showLegend(0);
				chart.margin({ top: 8, right: 24, bottom: 24, left: 48 })

				//chart.range1.interpolate("linear");

				d3.select("#" + self.id + " svg")
					.datum(series)
					.transition()
					.duration(500)
					.call(chart);

			});
		}

		this.bindChart();

		return this;
	}

}(jQuery));
