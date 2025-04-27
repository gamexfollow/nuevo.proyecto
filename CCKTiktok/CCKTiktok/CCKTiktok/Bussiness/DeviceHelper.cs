using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using CCKTiktok.DAL;
using CCKTiktok.Helper;

namespace CCKTiktok.Bussiness
{
	internal class DeviceHelper
	{
		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		public static int GetRandomNumberInRange(double minNumber, double maxNumber)
		{
			double value = new Random().NextDouble() * (maxNumber - minNumber) + minNumber;
			return Convert.ToInt32(value);
		}

		public static Dictionary<string, string> GenernateDeviceInfo()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				if (!File.Exists(Application.StartupPath + "/Config/deviceData.txt"))
				{
					new WebClient().DownloadFile("https://cck.vn//Download/Utils/deviceData.txt", Application.StartupPath + "/Config/deviceData.txt");
				}
				string[] array = File.ReadAllLines(Application.StartupPath + "/Config/deviceData.txt");
				dictionary.Add("Manufacture", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[0]);
				dictionary.Add("Brand", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[1]);
				dictionary.Add("Model", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[2]);
				dictionary.Add("Board", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[3]);
				dictionary.Add("Device", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[4]);
				if (File.Exists(Application.StartupPath + "/Config/longlat.txt"))
				{
					try
					{
						List<string> list = Utils.ReadTextFile(Application.StartupPath + "/Config/longlat.txt").Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
						if (list.Count > 0)
						{
							string text = list[new Random().Next(list.Count)];
							string value = text.ToString().Split(",|;".ToCharArray())[0];
							string value2 = text.ToString().Split(",|;".ToCharArray())[1];
							dictionary.Add("long", value);
							dictionary.Add("lat", value2);
						}
						else
						{
							dictionary.Add("long", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[5]);
							dictionary.Add("lat", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[6]);
						}
					}
					catch
					{
						dictionary.Add("long", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[5]);
						dictionary.Add("lat", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[6]);
					}
				}
				else
				{
					dictionary.Add("long", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[5]);
					dictionary.Add("lat", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[6]);
				}
				dictionary.Add("alt", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[7]);
				dictionary.Add("speed", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[8]);
				List<string> list2 = new List<string> { "Vinaphone", "Mobifone", "Viettel" };
				dictionary.Add("Carrier", list2[new Random().Next(0, list2.Count)]);
				dictionary.Add("CarrierCode", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[10]);
				dictionary.Add("ContryCode", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[11]);
				dictionary.Add("OSName", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[12]);
				dictionary.Add("OSArch", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[13]);
				dictionary.Add("OSVersion", array[GetRandomNumberInRange(0.0, array.Length - 1)].Split('|')[14]);
			}
			catch
			{
			}
			return dictionary;
		}

		public static Dictionary<string, string> ProduceTxtAddNew(string deviceId, string uid = "")
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string empty = string.Empty;
			try
			{
				Dictionary<string, string> dictionary2 = GenernateDeviceInfo();
				string text = GenerateAndroidSerial();
				string text2 = GenerateAndroidID();
				string randomMacAddress = GetRandomMacAddress();
				string text3 = randomIMEI();
				string text4 = randomIMSI();
				string text5 = randomPhoneNumber();
				string text6 = randomSimSerial();
				empty = empty + dictionary2["Manufacture"] + "\n" + dictionary2["Brand"] + "\n" + dictionary2["Model"] + "\n" + dictionary2["Board"] + "\n" + dictionary2["Device"] + "\n" + text2 + "\n" + text + "\n" + dictionary2["long"] + "\n" + dictionary2["lat"] + "\n" + dictionary2["alt"] + "\n" + dictionary2["speed"] + "\n" + randomMacAddress + "\n" + Guid.NewGuid().ToString("N").Substring(0, new Random().Next(5, 20)) + "\n" + randomMacAddress + "\n" + text3 + "\n" + text4 + "\n" + text5 + "\n" + text6 + "\n" + dictionary2["Carrier"] + "\n" + dictionary2["CarrierCode"] + "\n" + dictionary2["ContryCode"] + "\n" + dictionary2["OSName"] + "\n" + dictionary2["OSArch"] + "\n" + dictionary2["OSVersion"];
				dictionary.Add("Manufacture", dictionary2["Manufacture"]);
				dictionary.Add("Brand", dictionary2["Brand"]);
				dictionary.Add("Model", dictionary2["Model"]);
				dictionary.Add("Board", dictionary2["Board"]);
				dictionary.Add("Device", dictionary2["Device"]);
				dictionary.Add("AndroidID", text2);
				dictionary.Add("AndroidSerial", text);
				dictionary.Add("long", dictionary2["long"]);
				dictionary.Add("lat", dictionary2["lat"]);
				dictionary.Add("alt", dictionary2["alt"]);
				dictionary.Add("speed", dictionary2["speed"]);
				dictionary.Add("WifiMac", randomMacAddress);
				dictionary.Add("WifiName", "MyFiod");
				dictionary.Add("BSSID", randomMacAddress);
				dictionary.Add("IMEI", text3);
				dictionary.Add("IMSI", text4);
				dictionary.Add("PhoneNumber", text5);
				dictionary.Add("SimSerial", text6);
				dictionary.Add("Carrier", dictionary2["Carrier"]);
				dictionary.Add("CarrierCode", dictionary2["CarrierCode"]);
				dictionary.Add("ContryCode", dictionary2["ContryCode"]);
				dictionary.Add("OSName", dictionary2["OSName"]);
				dictionary.Add("OSArch", dictionary2["OSArch"]);
				dictionary.Add("OSVersion", dictionary2["OSVersion"]);
				SQLiteUtils sQLiteUtils = new SQLiteUtils();
				DataRow accountById = sQLiteUtils.GetAccountById(uid);
				if (accountById != null && uid != "" && accountById["brand"].ToString().Length > 50)
				{
					empty = accountById["brand"].ToString();
					if (accountById["Device"] != null && !(accountById["Device"].ToString() == ""))
					{
					}
				}
				else
				{
					sQLiteUtils.ExecuteQuery($"Update Account Set Brand='{empty}' where id='{uid}'");
				}
				using FileStream stream = new FileStream(Application.StartupPath + "\\Devices\\CCKInfo_" + ADBHelperCCK.NormalizeDeviceName(deviceId) + ".txt", FileMode.Create, FileAccess.Write);
				using StreamWriter streamWriter = new StreamWriter(stream);
				streamWriter.WriteLine(empty);
			}
			catch
			{
			}
			return dictionary;
		}

		public static string GetRandomMacAddress()
		{
			Random random = new Random();
			byte[] array = new byte[6];
			random.NextBytes(array);
			string text = string.Concat(array.Select((byte x) => string.Format("{0}:", x.ToString("X2"))).ToArray());
			return text.TrimEnd(':');
		}

		public static string randomMACAddress()
		{
			Random random = new Random();
			byte[] array = new byte[6];
			random.NextBytes(array);
			array[0] = (byte)(array[0] & 0xFEu);
			StringBuilder stringBuilder = new StringBuilder(18);
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(":");
				}
				stringBuilder.Append(string.Format("02", b));
			}
			return stringBuilder.ToString();
		}

		public static string GenerateAndroidID()
		{
			string source = "0123456789abcdef";
			string text = "";
			Random random = new Random();
			while (text.Length < 16)
			{
				text += source.ElementAt(random.Next(16));
			}
			return text;
		}

		public static string GenerateAndroidSerial()
		{
			string source = "0123456789abcdef";
			string text = "";
			Random random = new Random();
			while (text.Length < 12)
			{
				text += source.ElementAt(random.Next(16));
			}
			return text;
		}

		public static string randomSimSerial(string originSimSerial = "8984048830072898197")
		{
			string text = "";
			if (originSimSerial.Length > 8)
			{
				text = originSimSerial.Substring(0, 8);
			}
			else if (originSimSerial.Length == 8)
			{
				text = originSimSerial;
			}
			string text2 = text;
			Random random = new Random();
			while (text2.Length < 14)
			{
				text2 += random.Next(10);
			}
			return text2 + LuhnCheck(text2);
		}

		public static string randomIMEI(string originIMEI = "351713080968624")
		{
			string text = "";
			if (originIMEI.Length > 8)
			{
				text = originIMEI.Substring(0, 8);
			}
			else if (originIMEI.Length == 8)
			{
				text = originIMEI;
			}
			string text2 = text;
			Random random = new Random();
			while (text2.Length < 14)
			{
				text2 += random.Next(10);
			}
			return text2 + LuhnCheck(text2);
		}

		public static string randomIMSI(string MCCMNC = "351713080968624")
		{
			string text = MCCMNC;
			Random random = new Random();
			while (text.Length < 15)
			{
				text += Convert.ToString(random.Next(10));
			}
			return text + LuhnCheck(text);
		}

		private static string LuhnCheck(string digitString)
		{
			int i = 0;
			int num = 0;
			int num2 = digitString.Length - 1;
			for (; i < digitString.Length; i++)
			{
				int num3 = Convert.ToInt32(digitString.ElementAt(num2 - i));
				if (i % 2 == 0)
				{
					num3 *= 2;
					if (num3 > 9)
					{
						num3 -= 9;
					}
				}
				num += num3;
			}
			return Convert.ToString(num * 9 % 10);
		}

		public static string randomPhoneNumber()
		{
			string source = "0123456789";
			string text = "09";
			Random random = new Random();
			while (text.Length < 9)
			{
				text += source.ElementAt(random.Next(10));
			}
			return text;
		}
	}
}
