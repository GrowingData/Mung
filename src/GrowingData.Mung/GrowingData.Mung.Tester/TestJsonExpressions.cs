using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GrowingData.Mung.SqlBatch;
using GrowingData.Mung.Metric;
using NCalc;

namespace GrowingData.Mung.Tester {
	public class TestJsonExpressions {

		public static void Go() {

			var json = JsonConvert.SerializeObject(new {
				type = "blah",
				country = new {
					name = "Australia",
					iso = "AU",
					pop = 22000000
				}
			});


			Console.WriteLine(string.Format("Value {0}", JsonExpression.Evalulate(json, "[country.pop] > 2100000")));

		}


	}
}
