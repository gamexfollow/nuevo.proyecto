using System;
using System.Net.NetworkInformation;

namespace CCKTiktok.Bussiness
{
	public class ProxyInfo
	{
		public int SockPort { get; set; }

		public string PublicIp { get; set; }

		public string Ip { get; set; }

		public int Port { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public void Disposse()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public ProxyInfo(string fullInfo)
		{
			if (fullInfo.Contains("@"))
			{
				string[] array = fullInfo.Split('@');
				if (array.Length == 2)
				{
					fullInfo = array[1] + ":" + array[0];
				}
			}
			string[] array2 = fullInfo.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length >= 2)
			{
				Ip = array2[0];
				Port = Convert.ToInt32(array2[1]);
			}
			PublicIp = "";
			if (array2.Length >= 4)
			{
				UserName = array2[2];
				Password = array2[3];
			}
		}

		public override string ToString()
		{
			return (Ip != "") ? $"{Ip}:{Port}:{UserName}:{Password}" : "";
		}

		private static bool CanPing(string address)
		{
			Ping ping = new Ping();
			try
			{
				PingReply pingReply = ping.Send(address, 2000);
				if (pingReply != null)
				{
					return pingReply.Status == IPStatus.Success;
				}
				return false;
			}
			catch (PingException)
			{
				return false;
			}
		}

		public ProxyInfo()
		{
			Ip = "";
			Port = 0;
			UserName = "";
			Password = "";
			SockPort = 0;
			PublicIp = "";
		}
	}
}
