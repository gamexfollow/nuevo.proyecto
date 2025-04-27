using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CCKTiktok.Bussiness
{
	public class SecurityController
	{
		public string Encrypt(string data, string key = "371986ddde5a9a085c94a2e074a7206a")
		{
			string result = null;
			byte[][] hashKeys = GetHashKeys(key);
			try
			{
				result = EncryptStringToBytes_Aes(data, hashKeys[0], hashKeys[1]);
			}
			catch (CryptographicException)
			{
			}
			catch (ArgumentNullException)
			{
			}
			return result;
		}

		public string Decrypt(string data, string key = "371986ddde5a9a085c94a2e074a7206a")
		{
			string result = null;
			byte[][] hashKeys = GetHashKeys(key);
			try
			{
				result = DecryptStringFromBytes_Aes(data, hashKeys[0], hashKeys[1]);
			}
			catch (CryptographicException)
			{
			}
			catch (ArgumentNullException)
			{
			}
			return result;
		}

		private byte[][] GetHashKeys(string key)
		{
			byte[][] array = new byte[2][];
			Encoding uTF = Encoding.UTF8;
			SHA256 sHA = new SHA256CryptoServiceProvider();
			byte[] bytes = uTF.GetBytes(key);
			byte[] bytes2 = uTF.GetBytes(key);
			byte[] array2 = sHA.ComputeHash(bytes);
			byte[] array3 = sHA.ComputeHash(bytes2);
			Array.Resize(ref array3, 16);
			array[0] = array2;
			array[1] = array3;
			return array;
		}

		private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
		{
			if (plainText != null && plainText.Length > 0)
			{
				if (Key != null && Key.Length != 0)
				{
					if (IV != null && IV.Length != 0)
					{
						byte[] inArray;
						using (AesManaged aesManaged = new AesManaged())
						{
							aesManaged.Key = Key;
							aesManaged.IV = IV;
							ICryptoTransform transform = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV);
							using MemoryStream memoryStream = new MemoryStream();
							using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
							using (StreamWriter streamWriter = new StreamWriter(stream))
							{
								streamWriter.Write(plainText);
							}
							inArray = memoryStream.ToArray();
						}
						return Convert.ToBase64String(inArray);
					}
					throw new ArgumentNullException("IV");
				}
				throw new ArgumentNullException("Key");
			}
			throw new ArgumentNullException("plainText");
		}

		private static string DecryptStringFromBytes_Aes(string cipherTextString, byte[] Key, byte[] IV)
		{
			byte[] array = Convert.FromBase64String(cipherTextString);
			if (array != null && array.Length != 0)
			{
				if (Key != null && Key.Length != 0)
				{
					if (IV != null && IV.Length != 0)
					{
						string result = null;
						using (Aes aes = Aes.Create())
						{
							aes.Key = Key;
							aes.IV = IV;
							ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
							using MemoryStream stream = new MemoryStream(array);
							using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
							using StreamReader streamReader = new StreamReader(stream2);
							result = streamReader.ReadToEnd();
						}
						return result;
					}
					throw new ArgumentNullException("IV");
				}
				throw new ArgumentNullException("Key");
			}
			throw new ArgumentNullException("cipherText");
		}
	}
}
