using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GrowingData.Mung.Client;
using GrowingData.Mung.MetricJs;

namespace GrowingData.Mung.Tester {
	class Program {
		public class TestType {
			public string country;
			public string state;
			public string city;
			public int count;
		}


		static void Main(string[] args) {
			TestFilterGeneration();

			List<TestType> testEvents = new List<TestType>(){
				new TestType() {country = "au", state = "vic", count = 100 },
				new TestType() {country = "au", state = "wa", count = 20  },
				new TestType() { country = "au", state = "vic", count = 100 },
				new TestType() {state = "vic", count = 100 }
			};



			MUNG.Client.Write("console", "test-two", new { });
			MUNG.Client.Write("console", "test-two", new { country = "au", state = "nsw", count = 10 });
			MUNG.Client.Write("console", "test-two", new { country = "au", state = "wa", count = 20 });
			MUNG.Client.Write("console", "test-two", new { country = "au", state = "vic", count = 100 });
			MUNG.Client.Write("console", "test-two", new { state = "vic", count = 100 });
			var r = new Random();

			int count = 0;
			while (true) {

				var index = r.Next(0, testEvents.Count);

				MUNG.Client.WriteDirect("console", "test-two", testEvents[index]);
				Console.WriteLine("Wrote event: {0}", count);
				Console.ReadKey();

				count++;
			}

			MUNG.Client.WaitUntilQueueEmpty();
			Console.WriteLine("Sent.");
			Console.ReadKey();
		}


		public static void TestFilterGeneration() {
			var evt = new { country = "au", state = "vic", city = "melboure", count = 100 };

			var json = JsonConvert.SerializeObject(evt);



			var token = JToken.Parse(json);

			foreach (var f in MetricFilter.Filters(string.Empty, token, new string[] { "country", "state", "city" })) {
				Console.WriteLine(f);
			}


		}
	}
}
