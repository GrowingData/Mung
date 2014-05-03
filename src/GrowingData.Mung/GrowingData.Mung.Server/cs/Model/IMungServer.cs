using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Server {
	public interface IMungApp {
		EventPipeline Pipeline { get; }
	}

}