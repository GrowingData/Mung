using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GrowingData.Mung.Core;

namespace GrowingData.Mung.Server {
	public class EventSink {

		private static EventSink _sink = new EventSink();
		public static EventSink Sink { get { return _sink; } }

		private ConcurrentDictionary<string, EventProcessor> _active;

		private EventSink() {
			_active = new ConcurrentDictionary<string, EventProcessor>();
		}

		public void Disconnect(string name) {
			var toRemove = new Queue<string>(
				_active
					.Where(s => s.Value.Name == name)
					.Select(s => s.Key)
			);

			while (toRemove.Count > 0) {
				var key = toRemove.Peek();
				EventProcessor removed = null;
				if (_active.TryRemove(key, out removed)) {
					toRemove.Dequeue();
				}
			}
		}

		public bool RemoveProcessor(string name) {
			if (_active.ContainsKey(name)) {
				EventProcessor p;
				return _active.TryRemove(name, out p);
			}

			return false;
		}
		public EventProcessor GetProcessor(string name) {
			if (_active.ContainsKey(name)) {
				return _active[name];
			}
			return null;
		}

		public void AddProcessor(EventProcessor processor) {
			_active[processor.Name] = processor;
		}

		public void Process(MungServerEvent evt) {
			foreach (var subs in _active.Values) {
				try {
					subs.EnqueueEvent(evt);
				} catch (Exception ex) {
					Debug.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
				}
			}
		}



	}
}
