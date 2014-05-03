using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrowingData.Mung.Core {
	public class MungServerEvent {


		public DateTime LogTime;
		public object Data;
		public JToken Token;
		public string Type;
		public string Source;

		public MungServerEvent() {
			LogTime = DateTime.UtcNow;
		}
		public MungServerEvent(string json) {

			var firstToken = JToken.Parse(json);

			LogTime = (DateTime) firstToken["LogTime"];
			Source = (string)firstToken["Source"];
			Type = (string)firstToken["Type"];
			Token = firstToken["Data"];
		}
	}
}