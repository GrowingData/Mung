
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GrowingData.Mung.Core {
	/// <summary>
	/// Summary description for PathManager
	/// </summary>
	public static class PathManager {
		private static string _basePath = GetBasePath();
		public static string BasePath { get { return _basePath; } }


		private static string GetBasePath() {
			// Walk the current path backwards until we find a "config" directory
			DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			while (d != null) {
				if (d.Name == "bin" || d.Name == "Debug" || d.Name == "Release") {
					d = d.Parent;
				} else {
					return d.FullName;
				}
			}
			return null;
		}

		private static string _dataPath = null;
		public static string DataPath {
			get {
				if (_dataPath != null) {
					return _dataPath;
				}

				var checkPath = Path.Combine(BasePath, "data");

				if (!Directory.Exists(checkPath)) {
					Directory.CreateDirectory(checkPath);
				}


				return checkPath;
			}
		}



	}
}
