using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.SqlBatch {
	public class SqlServerBulkInserter : BulkInserter {

		protected Func<SqlConnection> _getConnection;

		public SqlServerBulkInserter(string schema, string filename, Func<SqlConnection> cn)
			: base(schema, filename) {

			_getConnection = cn;

			// Make sure that the schema exists
			CreateSchemaIfRequired(schema);
		}

		public override DbTable GetDbSchema() {
			string sql = @"
					SELECT 
						TABLE_NAME,
						COLUMN_NAME,
						DATA_TYPE
					FROM INFORMATION_SCHEMA.COLUMNS C
					WHERE	TABLE_SCHEMA = @Schema
					AND		TABLE_NAME = @Table
					ORDER BY TABLE_NAME, ORDINAL_POSITION
				";
			using (var cn = _getConnection()) {
				using (var cmd = new SqlCommand(sql, cn)) {
					cmd.Parameters.AddWithValue("@Schema", _schema);
					cmd.Parameters.AddWithValue("@Table", _table);
					using (var reader = cmd.ExecuteReader()) {

						return DbTable.LoadDb(reader);
					}
				}
			}
		}

		public override bool BulkInsert(DbTable schema, MungedDataReader reader) {

			using (var cn = _getConnection()) {
				using (SqlBulkCopy copy = new SqlBulkCopy(cn)) {
					//copy.ColumnMappings = new SqlBulkCopyColumnMappingCollection();

					for (var i = 0; i < reader.ColumnNames.Length; i++) {
						var column = reader.ColumnNames[i];
						var sourceOrdinal = i;
						var destinationOrdinal = schema.Columns.FindIndex(x => x.Name == column);

						if (destinationOrdinal == -1) {
							var msg = string.Format("Unable to resolve column mapping, column: {0} was not found in destination table {1}",
								column,
								_table
							);
							throw new Exception(msg);
						}
						copy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(i, destinationOrdinal));
					}

					copy.DestinationTableName = string.Format("[{0}].[{1}]", _schema, _table);

					copy.BatchSize = 1000;
					copy.BulkCopyTimeout = 9999999;

					copy.WriteToServer(reader);
				}

			}

			return true;
		}

		public override bool CreateTable(DbTable tbl) {

			StringBuilder ddl = new System.Text.StringBuilder();
			ddl.Append(string.Format("CREATE TABLE [{0}].[{1}] (\r\n", Schema, tbl.Name));


			foreach (var c in tbl.Columns) {
				if (c.Name == "_Id_" || c.Name == "_At_") {
					ddl.Append(string.Format("\t[{0}] {1} NOT NULL,\r\n", c.Name, c.Type.SqlType()));
				} else {
					ddl.Append(string.Format("\t[{0}] {1} NULL,\r\n", c.Name, c.Type.SqlType()));
				}
			}
			ddl.Append(string.Format("\tCONSTRAINT [PK_{0}] PRIMARY KEY ([_Id_])\r\n", tbl.Name));
			ddl.Append(")");

			return ExecuteCommand(ddl.ToString());
		}

		public bool ExecuteCommand(string sql) {
			using (var cn = _getConnection()) {
				using (var cmd = new SqlCommand(sql, cn)) {
					cmd.ExecuteNonQuery();
				}
			}
			return true;

		}

		public bool SameColumn(DbColumn a, DbColumn b) {
			return a.Name.ToLowerInvariant() == b.Name.ToLowerInvariant();

		}

		public bool CreateSchemaIfRequired(string schemaName) {
			var cmd = string.Format(@"
				IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name='{0}')
				BEGIN
					EXEC('CREATE SCHEMA {0}')
				END
				", schemaName);
			ExecuteCommand(cmd);
			return true;
		}


		public override bool ModifySchema(DbTable fromTbl, DbTable toTbl) {

			foreach (var c in toTbl.Columns) {
				var existing = fromTbl
					.Columns
					.FirstOrDefault(x => SameColumn(x, c));

				// Add the new column...
				if (existing == null) {
					fromTbl.Columns.Add(c);
					string ddl = string.Format("ALTER TABLE [{0}].[{1}] ADD [{2}] {3} NULL",
						Schema,
						fromTbl.Name,
						c.Name,
						c.Type.SqlType());

					ExecuteCommand(ddl);
				} else {
					if (c.Type != existing.Type) {
						if (existing.Type == DbType.Varchar) {
							// Do nothing, your already as generic as you can be.
						} else if (existing.Type == DbType.Float && c.Type == DbType.Integer) {
							// Was a float, and we are adding an int, so do nothing

						} else if (existing.Type == DbType.Integer && c.Type == DbType.Float) {
							// Change from INT to FLOAT

							existing.Type = DbType.Float;

							string ddl = string.Format("ALTER TABLE [{0}].[{1}] ALTER COLUMN [{2}] {3} NULL",
								Schema,
								fromTbl.Name,
								c.Name,
								DbType.Float.SqlType());

							ExecuteCommand(ddl);

						} else {
							// Change from ANYTHING else to VARCHAR
							existing.Type = DbType.Varchar;

							// Push it out to be VARCHAR
							string ddl = string.Format("ALTER TABLE [{0}].[{1}] ALTER COLUMN [{2}] {3} NULL",
								Schema,
								fromTbl.Name,
								c.Name,
								DbType.Varchar.SqlType());

							ExecuteCommand(ddl);
						}

					}
				}
			}

			return true;
		}

	}
}
