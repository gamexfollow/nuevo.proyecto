using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CCKTiktok.Bussiness
{
	public class TwoFactorAuthenticator
	{
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private TimeSpan DefaultClockDriftTolerance { get; set; }

		public TwoFactorAuthenticator()
		{
			DefaultClockDriftTolerance = TimeSpan.FromMinutes(5.0);
		}

		private static string RemoveWhitespace(string str)
		{
			return new string(str.Where((char c) => !char.IsWhiteSpace(c)).ToArray());
		}

		private string UrlEncode(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
			foreach (char c in value)
			{
				if (text.IndexOf(c) == -1)
				{
					stringBuilder.Append("%" + $"{(int)c:X2}");
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Replace(" ", "%20");
		}

		public string GeneratePINAtInterval(string accountSecretKey, long counter, int digits = 6)
		{
			return GenerateHashedCode(accountSecretKey, counter, digits);
		}

		internal string GenerateHashedCode(string secret, long iterationNumber, int digits = 6)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(secret);
			return GenerateHashedCode(bytes, iterationNumber, digits);
		}

		internal string GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
		{
			byte[] bytes = BitConverter.GetBytes(iterationNumber);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			HMACSHA1 hMACSHA = new HMACSHA1(key);
			byte[] array = hMACSHA.ComputeHash(bytes);
			int num = array[array.Length - 1] & 0xF;
			int num2 = ((array[num] & 0x7F) << 24) | (array[num + 1] << 16) | (array[num + 2] << 8) | array[num + 3];
			return (num2 % (int)Math.Pow(10.0, digits)).ToString(new string('0', digits));
		}

		private long GetCurrentCounter()
		{
			return GetCurrentCounter(DateTime.UtcNow, _epoch, 30);
		}

		private long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
		{
			return (long)(now - epoch).TotalSeconds / timeStep;
		}

		public bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient)
		{
			return ValidateTwoFactorPIN(accountSecretKey, twoFactorCodeFromClient, DefaultClockDriftTolerance);
		}

		public bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient, TimeSpan timeTolerance)
		{
			string[] currentPINs = GetCurrentPINs(accountSecretKey, timeTolerance);
			return currentPINs.Any((string c) => c == twoFactorCodeFromClient);
		}

		public string GetCurrentPIN(string accountSecretKey)
		{
			return GeneratePINAtInterval(accountSecretKey, GetCurrentCounter());
		}

		public string GetCurrentPIN(string accountSecretKey, DateTime now)
		{
			return GeneratePINAtInterval(accountSecretKey, GetCurrentCounter(now, _epoch, 30));
		}

		public string[] GetCurrentPINs(string accountSecretKey)
		{
			return GetCurrentPINs(accountSecretKey, DefaultClockDriftTolerance);
		}

		public string[] GetCurrentPINs(string accountSecretKey, TimeSpan timeTolerance)
		{
			List<string> list = new List<string>();
			long currentCounter = GetCurrentCounter();
			int num = 0;
			if (timeTolerance.TotalSeconds > 30.0)
			{
				num = Convert.ToInt32(timeTolerance.TotalSeconds / 30.0);
			}
			long num2 = currentCounter - num;
			long num3 = currentCounter + num;
			for (long num4 = num2; num4 <= num3; num4++)
			{
				list.Add(GeneratePINAtInterval(accountSecretKey, num4));
			}
			return list.ToArray();
		}
	}
}
