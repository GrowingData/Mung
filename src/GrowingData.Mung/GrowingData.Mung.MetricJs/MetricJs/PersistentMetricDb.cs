using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrowingData.Mung.Core;
using Jint;
using Jint.Native;
using Jint.Native.Object;



namespace GrowingData.Mung.MetricJs {
	public class PersistentMetricDb {

		protected EventPipeline _pipeline;
		protected string _basePath;

		public PersistentMetricDb(EventPipeline pipeline, string path) {
			_pipeline = pipeline;
			_basePath = path;
		}

		public void Reload() {
			//var path = AppDomain.CurrentDomain.BaseDirectory.Replace("\bin", "");
			//var persistenMetricPath = Path.Combine(path, "user", "metrics");

			foreach (var file in Directory.EnumerateFiles(_basePath)) {
				var js = File.ReadAllText(file);
				var name = new FileInfo(file).Name.Replace(".js", "");

				AddPersistentMetric(name, js);
			}


		}

		public void AddPersistentMetric(string name, string js) {
			_pipeline.RemoveProcessor(name);

			var newProcessor = new PersistentMetric(_pipeline, name, js);

		}
	}
}
