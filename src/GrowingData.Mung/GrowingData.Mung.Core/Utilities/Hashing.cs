using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingData.Mung.Core {
	public static class Hashing {

		public static string HashStringSHA1(string valueUTF8) {
			byte[] valueBytes = Encoding.UTF8.GetBytes(valueUTF8);
			SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
			byte[] hashBytes = sha1.ComputeHash(valueBytes);
			return BitConverter.ToString(hashBytes).Replace("-", "");

		}
		public static string HashStringMD5(string valueUTF8) {
			byte[] valueBytes = Encoding.UTF8.GetBytes(valueUTF8);
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(valueBytes);
			return BitConverter.ToString(hashBytes).Replace("-", "");

		}

		public static string HashStringSHA256(string valueUTF8) {
			byte[] valueBytes = Encoding.UTF8.GetBytes(valueUTF8);
			SHA256CryptoServiceProvider shar2 = new SHA256CryptoServiceProvider();
			byte[] hashBytes = shar2.ComputeHash(valueBytes);
			return BitConverter.ToString(hashBytes).Replace("-", "");

		}
	}
}
