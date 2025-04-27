using System;
using System.Security.Cryptography;
using System.Text;

namespace CCKTiktok.Bussiness
{
	public class DataEncrypterDecrypter
	{
		public static string GetMD5(string input)
		{
			using MD5 md5Hash = MD5.Create();
			string md5Hash2 = GetMd5Hash(md5Hash, input);
			if (VerifyMd5Hash(md5Hash, input, md5Hash2))
			{
				return md5Hash2;
			}
			return "";
		}

		private static string GetMd5Hash(MD5 md5Hash, string input)
		{
			byte[] array = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
		{
			string md5Hash2 = GetMd5Hash(md5Hash, input);
			StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
			if (ordinalIgnoreCase.Compare(md5Hash2, hash) == 0)
			{
				return true;
			}
			return false;
		}

		public static string Encrypt(string input, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			tripleDESCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(key);
			tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
			tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			tripleDESCryptoServiceProvider.Clear();
			return Convert.ToBase64String(array, 0, array.Length);
		}

		public static string Decrypt(string input, string key)
		{
			byte[] array = Convert.FromBase64String(input);
			TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			tripleDESCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(key);
			tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
			tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor();
			byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			tripleDESCryptoServiceProvider.Clear();
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
