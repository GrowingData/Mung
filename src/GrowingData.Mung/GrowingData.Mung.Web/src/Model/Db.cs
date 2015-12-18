using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;

namespace GrowingData.Mung {
	public static class Db {
		public static string SqlWarehouseConnectionString = ConfigurationManager.ConnectionStrings["SqlWarehouse"].ConnectionString;
		public static string SqMetaDataConnectionString = ConfigurationManager.ConnectionStrings["SqlMetadata"].ConnectionString;


		public static Func<SqlConnection> Warehouse {
			get {
				return () => {
					var cn = new SqlConnection(SqlWarehouseConnectionString);
					cn.Open();
					return cn;
				};
			}
		}
		public static Func<SqlConnection> Metadata {
			get {
				return () => {
					var cn = new SqlConnection(SqMetaDataConnectionString);
					cn.Open();
					return cn;
				};
			}
		}
	}
}