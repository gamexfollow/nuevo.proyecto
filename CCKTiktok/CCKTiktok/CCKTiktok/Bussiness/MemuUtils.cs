using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CCKTiktok.Bussiness
{
	public class MemuUtils
	{
		private const string PackageName = "com.zhiliaoapp.musically.go";

		private static Random rand = new Random();

		private static string PathMu => "D:\\Program Files\\Microvirt\\MEmu\\memuc.exe";

		public static void ExecuteCommandMemu(string cmd)
		{
			Process process = new Process();
			process.StartInfo.FileName = PathMu;
			process.StartInfo.Arguments = cmd;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.EnableRaisingEvents = true;
			process.Start();
			process.WaitForExit();
			process.Close();
		}

		public static void Close(string param, string NameOrId)
		{
			ExecuteCommandMemu($"stop -{param} {NameOrId}");
		}

		public static void Open(string param, string NameOrId)
		{
			ExecuteCommandMemu($"start -{param} {NameOrId}");
		}

		public static void ChangeInfoMemu(string param, string NameOrId)
		{
			Close(param, NameOrId);
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} macaddress auto");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} country_code \"vn\"");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} operator_countrycode \"84\"");
			ExecuteCommandMemu($"setgps -{param} {NameOrId} {rand.Next(1, 200)}.{rand.Next(100000, 999999)} {rand.Next(1, 200)}.{rand.Next(100000, 999999)}");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} linenum \"+8435{rand.Next(1000000, 9999999)}\"");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} bssid \"{GetRandomMacAddress()}\"");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} hmac \"{GetRandomMacAddress()}\"");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} macaddress \"{GetRandomMacAddress()}\"");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} operator_iso \"vn\"");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} imsi {RandomImei()}");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} imei {RandomImei()}");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} simserial {RandomSimserial()}");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} ssid auto");
			ExecuteCommandMemu($"setconfigex -{param} {NameOrId} custom_resolution 360 600 160");
			ExecuteCommandMemu(string.Format("setconfigex -{0} {1} microvirt_vm_brand  \"{2}\"", param, NameOrId, ""));
			ExecuteCommandMemu(string.Format("setconfigex -{0} {1} microvirt_vm_model  \"{2}\"", param, NameOrId, ""));
			ExecuteCommandMemu(string.Format("setconfigex -{0} {1} microvirt_vm_manufacturer \"{2}\"", param, NameOrId, ""));
			Thread.Sleep(5000);
			ExecuteCommandMemu($"start -{param} {NameOrId}");
		}

		public static string GetRandomMacAddress()
		{
			byte[] array = new byte[6];
			rand.NextBytes(array);
			string text = string.Concat(array.Select(delegate(byte x)
			{
				byte b = x;
				return string.Format("{0}:", b.ToString("X2"));
			}).ToArray());
			return text.TrimEnd(':');
		}

		public static string RandomImei()
		{
			return new string((from s in Enumerable.Repeat("0123456789", 15)
				select s[rand.Next(s.Length)]).ToArray());
		}

		public static string RandomSimserial()
		{
			return new string((from s in Enumerable.Repeat("0123456789", 20)
				select s[rand.Next(s.Length)]).ToArray());
		}
	}
}
