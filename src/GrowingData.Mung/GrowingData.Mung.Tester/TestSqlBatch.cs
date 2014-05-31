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
			SqlBatchChecker.ResetLock(logPath);


			Func<SqlConnection> fnCn = () => {
				var cn = new SqlConnection(connectionString);
				cn.Open();
				return cn;
			};


			SqlBatchChecker.Check("failed-", logPath, fnCn);
			SqlBatchChecker.Check("complete-", logPath, fnCn);
			SqlBatchChecker.Check("active-", logPath, fnCn);
		}

	}
}
