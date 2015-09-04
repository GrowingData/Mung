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

		public static void ResetLock(string dataPath) {
			var lockFilePath = Path.Combine(dataPath, "sql-batch.lock");

			if (File.Exists(lockFilePath)) {
				File.Delete(lockFilePath);
			}
		}

		public static void CleanUpOldFiles(string dataPath, Func<SqlConnection> fnConnection) {
			try {
				File.AppendAllText(Path.Combine(dataPath, "cleanup-running.log"), "Start: " + DateTime.UtcNow.ToString());
				ResetLock(dataPath);

				foreach (var file in Directory.EnumerateFiles(dataPath, "loaded-*")) {
					File.Delete(file);
				}

				Check("active-", dataPath, fnConnection);
				Check("failed-", dataPath, fnConnection);
				File.AppendAllText(Path.Combine(dataPath, "cleanup-running.log"), "Done: " + DateTime.UtcNow.ToString());

			} catch (Exception ex) {
				var exceptionLogPath = Path.Combine(dataPath, "cleanup.log");
				var errorDetails = string.Format("{0}	{1}: {2}\r\n{3}",
					DateTime.Now.ToString(),
					ex.Message,
					"Cleanup",
					ex.StackTrace
				);

				File.AppendAllText(exceptionLogPath, errorDetails);

			}
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
		public static void Check(string prefix, string dataPath, Func<SqlConnection> fnConnection) {

			File.AppendAllText(Path.Combine(dataPath, "sql-batch-check.log"), "Start: " + DateTime.UtcNow.ToString());

			var lockFilePath = Path.Combine(dataPath, "sql-batch.lock");

			// Make sure we aren't already running...
			if (File.Exists(lockFilePath)) {
				File.AppendAllText(Path.Combine(dataPath, "sql-batch-check.log"), "End (skipped due to lock): " + DateTime.UtcNow.ToString());
				return;
			}

			try {
				File.WriteAllText(lockFilePath, DateTime.Now.ToString());

				foreach (var file in Directory.EnumerateFiles(dataPath, prefix + "*")) {
					try {
						var loadedFileName = file.Replace("\\" + prefix, "\\loaded-");
						if (File.Exists(loadedFileName)) {
							// If we have already loaded it, just delete the old file
							File.Delete(file);
							System.Diagnostics.Debug.WriteLine("Skipped loading {0}, as it already has a loaded file", file);
							continue;
						}

						System.Diagnostics.Debug.Write("Loading {0}...", file);
						var sqlInsert = new SqlServerBulkInserter("dyn", file, fnConnection);
						sqlInsert.Execute();

						File.Move(file, file.Replace("\\" + prefix, "\\loaded-"));


						System.Diagnostics.Debug.WriteLine(" Done.");
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

				File.AppendAllText(Path.Combine(dataPath, "sql-batch-check.log"), "Finished: " + DateTime.UtcNow.ToString());
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
				ResetLock(dataPath);
			}
		}

	}
}
