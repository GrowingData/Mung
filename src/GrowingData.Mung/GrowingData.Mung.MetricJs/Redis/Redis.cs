﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Configuration;
using System.Threading;

namespace GrowingData.Mung.MetricJs {
	public class RedisClient {

		private static RedisClient _current = new RedisClient();
		public static RedisClient Current { get { return _current; } }

		private IDatabase _db;

		private string _redisConnectionString = ConfigurationManager.ConnectionStrings["Redis"].ConnectionString;
		private ConnectionMultiplexer _redisConnection;

		public ConnectionMultiplexer RedisConnection {
			get {
				if (_redisConnection == null || !_redisConnection.IsConnected) {
					_redisConnection = ConnectionMultiplexer.Connect(_redisConnectionString);
					return _redisConnection;
				} else {
					return _redisConnection;
				}
			}
		}
		public IDatabase Database { get { return _db; } }

		public RedisClient() {
			_db = RedisConnection.GetDatabase();

		}

		static byte[] Serialize<T>(T o) {
			if (o == null) {
				return null;
			}

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream()) {
				binaryFormatter.Serialize(memoryStream, o);
				byte[] objectDataAsStream = memoryStream.ToArray();
				return objectDataAsStream;
			}
		}

		static T Deserialize<T>(byte[] stream) {
			if (stream == null) {
				return default(T);
			}

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream(stream)) {
				T result = (T)binaryFormatter.Deserialize(memoryStream);
				return result;
			}
		}
	}

}