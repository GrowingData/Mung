using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GrowingData.Mung.SqlBatch;
using GrowingData.Mung.Client;
using GrowingData.Mung.MetricJs;

namespace GrowingData.Mung.Tester {
	public class TestFilterGeneration {


		public static void Go() {
			var evt = new { country = "au", state = "vic", city = "melboure", count = 100 };

			var json = JsonConvert.SerializeObject(evt);



			var token = JToken.Parse(json);

			foreach (var f in FilterBuilder.Filters(string.Empty, token, new string[] { "country", "state", "city" })) {
				Console.WriteLine(f);
			}

		}

	}
}
