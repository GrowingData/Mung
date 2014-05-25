using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.SqlBatch;

namespace GrowingData.Mung.Tester {
	public class SqlBatchRunner {

		public static void Go(string connectionString, string logPath) {
			Func<SqlConnection> fnCn = () => {
				return new SqlConnection(connectionString);
			};


			SqlBatchChecker.Check(logPath, fnCn);
		}

	}
}
