using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.MetricJs {
	public class JavascriptMetricFactory {

		private string _path;
		private EventPipeline _pipeline;


		public JavascriptMetricFactory(string path, EventPipeline pipeline) {
			_path = path;
			_pipeline = pipeline;
		}

		public void Reload() {
			foreach (var file in Directory.EnumerateFiles(_path)) {
				var js = File.ReadAllText(file);
				var name = new FileInfo(file).Name.Replace(".js", "");

				AddPersistentMetric(name, js);
			}
		}


		public void AddPersistentMetric(string name, string js) {
			_pipeline.RemoveProcessor(name);

			var newProcessor = new JavascriptMetric(name, js);

			_pipeline.AddProcessor(newProcessor);

		}

	}
}
