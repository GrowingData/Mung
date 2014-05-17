using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace GrowingData.Mung.Core {
	public class MungServerEvent {


		public DateTime LogTime;
		public JToken Token;
		public JToken Data;
		public string Type;
		public string Source;

		public MungServerEvent() {
			LogTime = DateTime.UtcNow;
		}
		public MungServerEvent(string json) {

			Token = JToken.Parse(json);

			LogTime = (DateTime)Token["LogTime"];
			Source = (string)Token["Source"];
			Type = (string)Token["Type"];
			Data = Token["Data"];
		}
	}
}