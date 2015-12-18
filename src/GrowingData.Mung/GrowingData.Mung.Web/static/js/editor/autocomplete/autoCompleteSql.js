function AutoCompleteSql(editor, schema) {
	var self = this;


	this.editor = editor;
	this.schema = schema;
	this.codeMirror = editor.codeMirror;

	this.tableTriggers = { "from": true, "join": true };

	this.referencedTables = [];
	this.isAutoCompleting = false;
	this.selectedIndex = -1;
	this.autoCompleteOptions = []

	this.tableAliases = {};


	this.searchTables = function (match) {
		var lower = match.toLowerCase();

		if (match == null || match.length == 0) {
			return [];
		}

		var matches = [];
		for (var i = 0; i < self.schema.length; i++) {
			if (self.schema[i].TableName.toLowerCase().indexOf(lower) >= 0) {
				matches.push(self.schema[i]);
			}
		}
		return matches;
	}


	// Gets the current token, allowing for multi name tokens that might look
	// like "schema.table"
	this.getMultiNameToken = function (cur) {
		var token = self.codeMirror.getTokenAt(cur);
		var prev = self.previousToken(token, cur);
		if (token != null && token.string == ".") {
			prev = token;
		}
		//console.log("start|> token:" + token.string + ", prev: " + prev.string);

		// Check to make sure its not like "schemaselfname.tableselfname"
		if (token != null && token.string.length > 0 && token.string[0] == ".") {
			if (token.string == ".") {
				prev = self.previousToken(token, cur);
			}
			if (prev != null) {
				token.string = prev.string + token.string;
				token.start = prev.start;
			}
		}

		return token;
	}

	this.cursorActivity = function () {

		var cur = self.codeMirror.getCursor();
		var token = self.getMultiNameToken(cur);

		if (token.string == " ") {
			return;
		}
		console.log("token |> token: " + token.string);


		// Do we need to popup a list of tables?
		var space = self.previousToken(token, cur);
		if (space != null) {
			if (space.string == " ") {
				var trigger = self.previousToken(space, cur);
				if (trigger != null) {
					if (self.tokenIsTableTrigger(trigger)) {
						self.showAutocomplete(token);
						var matches = self.searchTables(token.string);
						self.autoCompleteOptions = [];

						for (var i = 0; i < matches.length; i++) {
							// Create the option
							var opt = self.createAutoCompleteTable(matches[i]);
							opt.appendTo(self.matches)
							// Keep a reference to it so we dont need to keep hitting up the dom
							self.autoCompleteOptions.push(opt);
						}
						self.selectedIndex = -1;
						return;
					}
				}
			}
		}
		// 


		if (self.isAutoCompleting) {
			self.hideAutocomplete();
		}
	};

	this.createAutoCompleteTable = function (match) {
		var li = $("<li>").addClass("table opt").data("match", match);

		var name = $("<div>").addClass("table-name").text(match.TableName)
			.appendTo(li);

		//var deets = $("<ul>").addClass("which-one")
		//	.append($("<li>").text(match.connection).addClass("connection"))
		//	.appendTo(li);
		return li;
	}

	this.autoCompletePick = function () {
		var cur = self.codeMirror.getCursor();
		var token = self.getMultiNameToken(cur);

		var match = self.autoCompleteOptions[self.selectedIndex].data("match");

		//var table = "\"" + [match.connection, match.TableName].join(".") + "\"";
		var table = match.TableName;
		var alias = self.buildAlias(match.TableName) + "\r\n";
		self.tableAliases[alias] = match;

		self.codeMirror.replaceRange(table + " " + alias,
			{ line: cur.line, ch: token.start },
			{ line: cur.line, ch: token.end });


		var lineText = self.codeMirror.getLine(self.codeMirror.firstLine());

		//if (lineText.indexOf("using") == 0) {
		//	self.codeMirror.replaceRange("using \"" + match.connection + "\"", { line: 0, ch: 0 }, { line: 0, ch: lineText.length });
		//} else {
		//	self.codeMirror.replaceRange("using \"" + match.connection + "\"\n\n", { line: 0, ch: 0 }, { line: 0, ch: 0 })
		//}


		self.hideAutocomplete();

	};

	this.buildAlias = function (tableName) {
		// Try the first letter...
		var firstLetter = tableName[0].toUpperCase();
		if (!(firstLetter in self.tableAliases)) {
			return firstLetter;
		}
		var uscore = null;
		var camel = null;

		var uppers = [];
		var underscores = [];
		var underScore = false;
		for (var i = 0; i < tableName.length; i++) {
			currentCode = tableName.charCodeAt(i);
			if (currentCode >= 65 && currentCode <= 90) {
				uppers.push(tableName[i])
			}
			if (currentCode >= 97 && currentCode <= 122) {
				hasLower = true;
			}
			if (underScore) {
				underscores.push(tableName[i].toUpperCase());
				underScore = false;
			}
			if (tableName[i] == 'self') {
				underScore = true;
			}
		}
		// "goorooselfskill" => "GS"
		if (underscores.length > 0) {
			uscore = tableName[0].toUpperCase() + underscores.join("")
			if (!(uscore in self.tableAliases)) {
				return uscore;
			}
		}

		// "GoorooSkill" => "GS"
		if (uppers.length > 0 && hasLower) {
			camel = uppers.join("").toUpperCase();
			if (!(camel in self.tableAliases)) {
				return camel;
			}
		}

		// Just keep on adding numbers until we end up with something
		// assuming we have less than 1000 tables.
		for (var i = 2; i < 1000; i++) {
			if (!((firstLetter + i) in self.tableAliases)) {
				return firstLetter + i;
			}
			if (camel != null && !((camel + i) in self.tableAliases)) {
				return camel + i;
			}
			if (uscore != null && !((uscore + i) in self.tableAliases)) {
				return uscore + i;
			}
		}


	};

	this.isMixedCase = function (str) {
		hasUpper = false;
		hasLower = false;
		hasUnderscore = false;

	}

	this.autoCompleteSelect = function (direction) {
		if (direction == -1) {
			console.log("Autocomplete up");
		} else {
			console.log("Autocomplete down");
		}

		this.selectedIndex += direction;
		this.autocomplete.find(".selected").removeClass("selected");

		if (this.selectedIndex < 0) {
			this.selectedIndex = this.autoCompleteOptions.length + this.selectedIndex;
		}

		if (this.selectedIndex > this.autoCompleteOptions.length) {
			this.selectedIndex -= this.autoCompleteOptions.length;
		}

		this.autoCompleteOptions[this.selectedIndex].addClass("selected");

	}


	this.showAutocomplete = function (token) {
		var cur = { ch: token.start, line: self.codeMirror.getCursor().line };
		var pos = self.codeMirror.charCoords(cur);
		this.autocomplete.css("top", pos.top + "px").css("left", pos.left + "px");
		this.matches.empty();
		this.autocomplete.fadeIn(100);
		this.isAutoCompleting = true;

		this.codeMirror.addKeyMap(self.keyMap);

	}

	this.hideAutocomplete = function () {
		this.autocomplete.fadeOut(100);
		this.isAutoCompleting = false;

		this.codeMirror.removeKeyMap("mung-autocomplete");
	}

	this.previousToken = function (token, cur) {

		if (token == null || token.start == 0) {
			return null;
		}
		return self.codeMirror.getTokenAt({ ch: token.start, line: cur.line })
	}


	this.tokenIsTableTrigger = function (token) {
		return token.string.toLowerCase() in this.tableTriggers;

	}

	this.init = function () {
		this.autocomplete = $("<div>")
			.addClass("autocomplete schemata")
			.appendTo($("body"));

		this.matches = $("<ul>").appendTo(this.autocomplete);



	}

	this.keyMap = {
		name: "mung-autocomplete",
		Up: function (cm) {
			self.autoCompleteSelect(-1);
		},
		Down: function (cm) {
			self.autoCompleteSelect(1)
		},
		//Enter: function (cm) {
		//	self.autoCompletePick();
		//},
		Tab: function (cm) {
			self.autoCompletePick();
		}
	}

	this.init();
}

