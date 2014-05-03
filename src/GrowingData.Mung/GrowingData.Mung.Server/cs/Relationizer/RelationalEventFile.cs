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

namespace GrowingData.Mung.Server {

	/// <summary>
	/// Holds the actual reference to a file which a RelationalEventWriter
	/// will write to.
	/// </summary>
	public class RelationalEventFile : IDisposable {
		public const string FilePrefixActive = "active";
		public const string FilePrefixComplete = "complete";

		private StreamWriter _currentStream;

		private string _basePath;
		private string _time;
		private string _eventName;
		private string _schemaHash;

		private SortedList<string, RelationalField> _schema;

		public string SchemaHash { get { return _schemaHash; } }
		public string TimeString { get { return _time; } }


		public string GetFilepath(string type) {
			return Path.Combine(_basePath, GetFilename(type));
		}


		public string GetFilename(string type) {

			var fileName = string.Format("{0}-{1}.{2}-{3}.tsv",
				type,
				_eventName,
				_time,
				_schemaHash);

			return fileName;
		}

		public RelationalEventFile(string basePath, string time, string eventName, SortedList<string, RelationalField> schema) {
			_basePath = basePath;
			_time = time;
			_eventName = eventName;
			_schema = new SortedList<string, RelationalField>(schema);

			_schemaHash = GetSchemaHash(schema);

			Open();
		}

		private void Open() {
			var fileStream = File.Open(GetFilepath(FilePrefixActive), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
			_currentStream = new StreamWriter(fileStream);

			WriteHeader(_currentStream, _schema);
		}

		public void WriteRow(RelationalEvent evt) {
			WriteRow(_currentStream, _schema, evt);
		}




		public void Dispose() {
			if (_currentStream != null) {
				_currentStream.Close();
				_currentStream.Dispose();

				File.Move(GetFilepath(FilePrefixActive), GetFilepath(FilePrefixComplete));
			}
		}


		public static void WriteHeader(StreamWriter writer, SortedList<string, RelationalField> schema) {
			var columns = new string[] { "_Id_", "_ParentId_", "_At_" }
				.Concat(schema.Keys);

			var types = new DbType[] { DbType.Key, DbType.Key, DbType.DateTime }
				.Concat(schema.Values.Select(x => x.Type));


			writer.WriteLine(string.Join("\t", columns));
			writer.WriteLine(string.Join("\t", types.Select(x => x.ToString())));


		}

		public static void WriteRow(StreamWriter writer, SortedList<string, RelationalField> schema, RelationalEvent evt) {
			var values = new string[] {
				RelationalEvent.Serialize(evt._Id_),
				RelationalEvent.Serialize(evt._ParentId_),
				RelationalEvent.Serialize(evt._At_)
			}.Concat(
				schema.Select(x => evt[x.Key])
			);

			writer.WriteLine(string.Join("\t", values.Select(x => x)));
			writer.Flush();
		}

		public static string GetSchemaHash(SortedList<string, RelationalField> schema) {
			var schemaString = string.Join("|", schema.Values.Select(x => string.Format("{0}:{1}", x.ColumnName, x.Type)));
			return Hashing.HashStringMD5(schemaString);
		}

	}

}
