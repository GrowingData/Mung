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


		static void Main(string[] args) {
			Console.WriteLine("Running: {0}...", args[0]);
			if (args[0] == "events") {
				TestEventGeneration.Go();
			}
			if (args[0] == "filters") {
				TestFilterGeneration.Go();
			}
			if (args[0] == "sqlbatch") {
				SqlBatchRunner.Go(args[1], args[2]);
			}
			Console.WriteLine("Done");
			Console.ReadKey();
		}


	}
}
