using System;
using System.Linq;

namespace CCKTiktok.Bussiness
{
	public static class StringHelper
	{
		private static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

		public static string ToBase32String(this byte[] secret)
		{
			string bits = secret.Select((byte b) => Convert.ToString(b, 2).PadLeft(8, '0')).Aggregate((string a, string b) => a + b);
			return (from i in Enumerable.Range(0, bits.Length / 5)
				select alphabet.Substring(Convert.ToInt32(bits.Substring(i * 5, 5), 2), 1)).Aggregate((string a, string b) => a + b);
		}

		public static byte[] ToByteArray(this string secret)
		{
			string bits = (from c in secret.ToUpper().ToCharArray()
				select Convert.ToString(alphabet.IndexOf(c), 2).PadLeft(5, '0')).Aggregate((string a, string b) => a + b);
			return (from i in Enumerable.Range(0, bits.Length / 8)
				select Convert.ToByte(bits.Substring(i * 8, 8), 2)).ToArray();
		}
	}
}
