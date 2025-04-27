using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace CCKTiktok.Bussiness
{
	public static class TimeSensitivePassCode
	{
		public static string GeneratePresharedKey()
		{
			byte[] array = new byte[10];
			using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rNGCryptoServiceProvider.GetBytes(array);
			}
			return array.ToBase32String();
		}

		public static string GetCurrentOtp(string base32EncodedSecret)
		{
			base32EncodedSecret = base32EncodedSecret.Replace(" ", string.Empty);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			long counter = (long)Math.Floor((DateTime.UtcNow - dateTime).TotalSeconds / 30.0);
			return GetHotp(base32EncodedSecret, counter);
		}

		public static IList<string> GetListOfOTPs(string base32EncodedSecret)
		{
			base32EncodedSecret = base32EncodedSecret.Replace(" ", string.Empty);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			long num = (long)Math.Floor((DateTime.UtcNow - dateTime).TotalSeconds / 30.0);
			return new List<string>
			{
				GetHotp(base32EncodedSecret, num - 1L),
				GetHotp(base32EncodedSecret, num),
				GetHotp(base32EncodedSecret, num + 1L)
			};
		}

		private static string GetHotp(string base32EncodedSecret, long counter)
		{
			byte[] buffer = BitConverter.GetBytes(counter).Reverse().ToArray();
			byte[] key = base32EncodedSecret.ToByteArray();
			HMACSHA1 hMACSHA = new HMACSHA1(key, useManagedSha1: true);
			byte[] array = hMACSHA.ComputeHash(buffer);
			int num = array[array.Length - 1] & 0xF;
			int num2 = ((array[num] & 0x7F) << 24) | ((array[num + 1] & 0xFF) << 16) | ((array[num + 2] & 0xFF) << 8) | (array[num + 3] & 0xFF);
			return (num2 % 1000000).ToString().PadLeft(6, '0');
		}
	}
}
