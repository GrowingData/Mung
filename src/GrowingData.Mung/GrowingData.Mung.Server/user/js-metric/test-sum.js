(function () {
	this.sum = 0;
	this.init = function (other) { this.sum = other.sum; };
	this.filter = function (evt) { return (evt != null && evt.Data != null && evt.Data.count != null); };
	this.accumulate = function (evt) { this.sum += parseInt(evt.Data.count); return this.sum; };
	this.terminate = function () { return this.sum; };
	this.reset = function () { this.sum = 0; };
	this.save = function () { return this; };
	return this;
})();