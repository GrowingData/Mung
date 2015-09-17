using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Relationizer {
	/// <summary>
	/// Manages schema changes over time, ensuring that everything is
	/// always written in a consistent manner, even if the objects schema
	/// is somewhat different to the actual schema (e.g. more columns)
	/// </summary>
	public class RelationalEventWriter : IDisposable {
		private object _syncRoot = new object();
		private string _eventName;
		private string _basePath;
		private string _lastTimeString;

		private SortedList<string, RelationalField> _schema;

		// For the same event type, we might actually have multiple
		// different schemas (e.g. the event is being logged with different params),
		// so we need to track them independently
		private Dictionary<string, RelationalEventFile> _schemaFiles;

		private int _rowsWritten = 0;

		public RelationalEventWriter(string basePath, string eventName) {
			_eventName = eventName;
			_schemaFiles = new Dictionary<string, RelationalEventFile>();
			_basePath = basePath;
		}


		/// <summary>
		/// Get the file name for the current moment in time, plus the schema version
		/// </summary>
		/// <returns></returns>
		private string GetCurrentTimeString() {
			var minutes = Math.Floor(DateTime.UtcNow.Minute / 10d) * 10;

			return string.Format("{0}.{1}",
				DateTime.UtcNow.ToString("yyyy-MM-dd.HH"),
				minutes
			);
		}

		public void Write(RelationalEvent evt) {
			lock (_syncRoot) {
				if (_schema == null) {
					_schema = new SortedList<string, RelationalField>();
					foreach (var s in evt.Schema) {
						_schema.Add(s.ColumnName, s);
					}


				} else {
					// Check to see if anything has changed...
					foreach (var s in evt.Schema) {
						if (_schema.ContainsKey(s.ColumnName)) {
							if (_schema[s.ColumnName].Type != s.Type) {
								// There is a change in type, always expand to
								_schema[s.ColumnName].Type = DbType.Varchar;
							}
						} else {
							// There is a new field
							_schema.Add(s.ColumnName, s);
						}
					}
				}

				// Its a new time block, so dispose of all the active files
				if (_lastTimeString == null || _lastTimeString != GetCurrentTimeString()) {
					var keys = _schemaFiles.Keys.ToList();

					foreach (var k in keys) {
						_schemaFiles[k].Dispose();
						_schemaFiles.Remove(k);
					}

					_lastTimeString = GetCurrentTimeString();
				}

				var schemaHash = RelationalEventFile.GetSchemaHash(_schema);

				if (!_schemaFiles.ContainsKey(schemaHash)) {
					_schemaFiles[schemaHash] = new RelationalEventFile(_basePath, GetCurrentTimeString(), _eventName, _schema);
				}

				_schemaFiles[schemaHash].WriteRow(evt);
			}
		}


		public void Dispose() {
			foreach (var k in _schemaFiles.Keys) {
				_schemaFiles[k].Dispose();
				_schemaFiles.Remove(k);
			}
		}
	}

}
