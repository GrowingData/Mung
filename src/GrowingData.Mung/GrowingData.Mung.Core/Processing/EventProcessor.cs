﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace GrowingData.Mung.Core {
	public abstract class EventProcessor {
		protected object _sync = new object();

		protected ConcurrentQueue<MungServerEvent> _events;

		private Guid _id;
		public Guid Id { get { return _id; } }

		private string _name;
		public string Name { get { return _name; } }

		protected EventPipeline _pipeline;

		public EventProcessor(EventPipeline pipeline, string name) {
			_id = new Guid();
			_name = name;
			_events = new ConcurrentQueue<MungServerEvent>();
			_pipeline = pipeline;
			
			Task.Factory.StartNew(() => PumpQueue(), TaskCreationOptions.LongRunning);
		}

		public void EnqueueEvent(MungServerEvent evt) {
			if (_events.Count < 100) {
				_events.Enqueue(evt);
			}
		}

		public void PumpQueue() {

			while (true) {
				while (_events.Count > 0) {
					MungServerEvent evt;
					if (_events.TryDequeue(out evt)) {
						try {
							ProcessEvent(evt);
						} catch (Exception ex) {



						}
					}
				}
				Thread.Sleep(100);

			}

		}



		protected abstract void ProcessEvent(MungServerEvent evt);


	}
}