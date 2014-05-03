using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.SqlBatch {
	public abstract class BulkInserter {

		public abstract bool CreateTable(DbTable tbl);
		public abstract bool BulkInsert(DbTable schema, MungedDataReader reader);
		public abstract bool ModifySchema(DbTable oldSchema, DbTable newSchema);
		public abstract DbTable GetDbSchema();

		protected string _schema;
		protected string _table;
		protected string _filename;

		public string Schema { get { return _schema; } }
		public string TableName { get { return _table; } }

		protected BulkInserter(string schema, string filename) {
			var info = new FileInfo(filename);

			_schema = schema;
			_table = info.Name.Split('.').First().Replace("complete-", ""); 
			_filename = filename;
		}

		public bool Execute() {

			using (var stream = File.OpenText(_filename)) {
				var reader = new MungedDataReader(stream);

				var currentSchema = new DbTable(_table, reader);
				var oldSchema = GetDbSchema();

				// Check / Update the schema in the DB
				if (oldSchema == null) {
					CreateTable(currentSchema);
				} else {
					ModifySchema(oldSchema, currentSchema);
				}

				var newSchema = GetDbSchema();

				BulkInsert(newSchema, reader);

			}
			return true;
		}




	}
}
