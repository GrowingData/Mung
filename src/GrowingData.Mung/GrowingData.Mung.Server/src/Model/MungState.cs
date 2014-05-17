using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Server {

	public static class MungState {
		public static IMungApp App = new MungApp();
	}

}