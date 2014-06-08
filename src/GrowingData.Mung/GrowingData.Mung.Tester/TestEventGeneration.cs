using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.SqlBatch;
using GrowingData.Mung.Client;
using GrowingData.Mung.MetricJs;
using System.Configuration;

namespace GrowingData.Mung.Tester {
	public class TestEventGeneration {

		public class TestType {
			public string country;
			public string state;
			public string city;
			public int count;
		}
		public static void Go() {

			List<TestType> testEvents = new List<TestType>(){
				new TestType() {country = "au", state = "vic", count = 100 },
				new TestType() {country = "au", state = "wa", count = 20  },
				new TestType() { country = "au", state = "vic", count = 100 },
				new TestType() {state = "vic", count = 100 }
			};
			var mungClient = new MungClient(ConfigurationManager.ConnectionStrings["MungServer"].ConnectionString);



			mungClient.Write("console", "test-two", new { });
			mungClient.Write("console", "test-two", new { country = "au", state = "nsw", count = 10 });
			mungClient.Write("console", "test-two", new { country = "au", state = "wa", count = 20 });
			mungClient.Write("console", "test-two", new { country = "au", state = "vic", count = 100 });
			mungClient.Write("console", "test-two", new { state = "vic", count = 100 });
			var r = new Random();

			int count = 0;
			while (true) {

				var index = r.Next(0, testEvents.Count);

				mungClient.Write("console", "test-two", testEvents[index]);
				Console.WriteLine("Wrote event: {0}", count);
				Console.ReadKey();

				count++;
			}

			mungClient.WaitUntilQueueEmpty();
			Console.WriteLine("Sent.");
		}

	}
}
