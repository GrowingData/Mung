using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace GrowingData.Utilities {
	public static class StringLineReader {
		public static IEnumerable<string> Lines(this string text) {
			using (StringReader sr = new StringReader(text)) {
				string line;
				while ((line = sr.ReadLine()) != null) {
					yield return line;
				}
			}
		}

	}
}
