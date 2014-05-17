using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace GrowingData.Mung.Client {
	public class MungEvent {


		public DateTime LogTime;
		public object Data;
		public string Type;
		public string Source;

		public MungEvent() {
			LogTime = DateTime.UtcNow;
		}
	}
}