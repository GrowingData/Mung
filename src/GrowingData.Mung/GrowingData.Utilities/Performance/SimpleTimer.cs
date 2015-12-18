using System;
using System.Diagnostics;

namespace GrowingData.Utilities {
	public class SimpleTimer : IDisposable {

		private Stopwatch _watch;
		private Action<double> _callback;

		public SimpleTimer(Action<double> callback) {
			_watch = new Stopwatch();
			_callback = callback;
			_watch.Start();
		}

		public void Dispose() {
			_watch.Stop();
			_callback(_watch.ElapsedMilliseconds);
		}


		public static void Time(Action run, Action<double> callback) {
			using (var t = new SimpleTimer(callback)) {
				run();
			}
		}
		public static void DebugTime(string name, Action run) {
			using (var t = new SimpleTimer((ms) => Debug.WriteLine("{0}\t {1}ms", name, ms))) {
				run();
			}
		}
	}
}