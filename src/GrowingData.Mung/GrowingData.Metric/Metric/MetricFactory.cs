using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Metric {
	public class MetricFactory {

		private string _path;
		private EventPipeline _pipeline;


		public MetricFactory(string path, EventPipeline pipeline) {
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


		public void AddPersistentMetric(string name, string json) {
			_pipeline.RemoveProcessor(name);

			
			var metric = JsonConvert.DeserializeObject<JsonMetric>(json);

			var newProcessor = new MetricProcessor(name, metric);

			_pipeline.AddProcessor(newProcessor);

		}

	}
}
