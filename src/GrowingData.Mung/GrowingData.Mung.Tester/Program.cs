using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrowingData.Mung.Client;

namespace GrowingData.Mung.Tester {
	class Program {
		static void Main(string[] args) {

			int count = 0;
			while (true) {


				count++;
				MUNG.Client.WriteDirect("console", "test", new {
					name = "tez",
					count = count
				});
				Console.WriteLine("Wrote event: {0}", count);

				Console.ReadKey();
			}
		}
	}
}
