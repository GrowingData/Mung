"use strict";

var MUNG = MUNG || {};

MUNG.EventManager = function () {
	var self = this;

	this.functionMap = {};


	// Bind an event to fire
	this.bind = function (name, callback) {
		if (!Array.isArray(this.functionMap[name])) {
			this.functionMap[name] = [];
		}
		this.functionMap[name].push(callback);
	}

	// Fire all the events registered with the name
	this.fire = function (name) {
		var list = this.functionMap[name];
		if (!Array.isArray(list)) {
			// Nothing to see here thanks
			return;
		}

		var args = Array.prototype.splice.call(arguments, 1);
		for (var i = 0; i < list.length; i++) {
			var f = list[i];
			f.apply(null, args);
		}

	}


}