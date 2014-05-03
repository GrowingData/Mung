using System;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.SqlBatch {
	public static class SqlBatchChecker {

		public static void CleanUpOldFiles(string dataPath, Func<SqlConnection> fnConnection) {
			Check("active-", dataPath, fnConnection);

		}



		public static void Check(string dataPath, Func<SqlConnection> fnConnection) {
			Check("complete-", dataPath, fnConnection);
		}

		/// <summary>
		/// Checks for files marked with the "prefix" given, and attempts to load them
		/// into the database given by the connection "fnConnection".  If an error occurrs
		/// duirng loading, the file will either be left alone of
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="dataPath"></param>
		/// <param name="fnConnection"></param>
		/// <param name="moveWithError"></param>
		private static void Check(string prefix, string dataPath, Func<SqlConnection> fnConnection) {
			var lockFilePath = Path.Combine(dataPath, "sql-batch.lock");

			// Make sure we aren't already running...
			if (File.Exists(lockFilePath)) {
				return;
			}

			try {
				File.WriteAllText(lockFilePath, DateTime.Now.ToString());

				foreach (var file in Directory.EnumerateFiles(dataPath, prefix + "*")) {
					try {

						var sqlInsert = new SqlServerBulkInserter("dyn", file, fnConnection);
						sqlInsert.Execute();

						File.Move(file, file.Replace("\\" + prefix, "\\loaded-"));

					} catch (Exception ex) {
						var exceptionLogPath = Path.Combine(dataPath, "sql-batch-exceptions.log");

						var errorDetails = string.Format("{0}	{1}: {2}\r\n{3}",
							DateTime.Now.ToString(),
							ex.Message,
							file,
							ex.StackTrace
						);

						File.AppendAllText(exceptionLogPath, errorDetails);

						File.Move(file, file.Replace("\\" + prefix, "\\failed-"));


					}
				}

			} catch (Exception ex) {
				var exceptionLogPath = Path.Combine(dataPath, "sql-batch-exceptions.log");
				var errorDetails = string.Format("{0}	{1}: {2}\r\n{3}",
					DateTime.Now.ToString(),
					ex.Message,
					"Big error",
					ex.StackTrace
				);

				File.AppendAllText(exceptionLogPath, errorDetails);



			} finally {
				if (File.Exists(lockFilePath)) {
					File.Delete(lockFilePath);
				}
			}
		}

	}
}
