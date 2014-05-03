using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;

namespace GrowingData.Mung.Server {
	public class MungedDataReader : CsvReader {

		private DbType[] _types;
		private string[] _columnNames;

		public DbType[] ColumnTypes {
			get {
				return _types;
			}
		}
		public string[] ColumnNames { get { return _columnNames; } }

		public MungedDataReader(TextReader reader)
			: base(reader, true, '\t') {

			_columnNames = this.GetFieldHeaders();

			// Read the types
			this.ReadNextRecord();

			_types = new DbType[_columnNames.Length];
			for (var i = 0; i < this.FieldCount; i++) {
				_types[i] = DbTypes.FromString(this[i]);
			}


		}
		private string InvalidFormatErrorMessage(DbType expected, int fieldNumber, string value) {
			return string.Format("Error reading line: {0}, field: {1}. Expected a DbType.{2}, got: '{3}'",
				 LineNumber,
				 fieldNumber,
				 expected.ToString(),
				value
				);

		}

		public override object GetValue(int fieldIndex) {
			ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

			if (((IDataRecord)this).IsDBNull(fieldIndex)) {
				return DBNull.Value;
			}

			var val = this[fieldIndex];
			if (_types[fieldIndex] == DbType.Bit) {
				var intValue = -1;
				if (int.TryParse(val, out intValue)) {
					if (intValue == 0) return false;
					if (intValue == 1) return true;
				}
				throw new FormatException(InvalidFormatErrorMessage(DbType.Bit, fieldIndex, val));
			}

			if (_types[fieldIndex] == DbType.DateTime) {
				var dateTime = DateTime.MinValue;
				if (DateTime.TryParse(val, out dateTime)) {
					return dateTime;
				}
				throw new FormatException(InvalidFormatErrorMessage(DbType.DateTime, fieldIndex, val));
			}

			if (_types[fieldIndex] == DbType.Float) {
				var d = double.NaN;
				if (double.TryParse(val, out d)) {
					return d;
				}
				throw new FormatException(InvalidFormatErrorMessage(DbType.Float, fieldIndex, val));
			}

			if (_types[fieldIndex] == DbType.Integer) {
				var intValue = -1;
				if (int.TryParse(val, out intValue)) {
					return intValue;
				}
				throw new FormatException(InvalidFormatErrorMessage(DbType.Float, fieldIndex, val));
			}


			return val;

		}



	}
}
