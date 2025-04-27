using System;
using System.Net;
using System.Threading;

namespace CCKTiktok.Bussiness
{
	public class Proxyv6net
	{
		private string LIST_COUNTRY = "https://api.proxyv6.net/api/list-country?api_key={0}";

		private string API = "36e6b2bd-8e34-4869-98b8-2724794b53a4";

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public Proxyv6net(string apiKey)
		{
			API = apiKey;
		}

		public bool ChangeProxy(string host, int port)
		{
			DateTime now = DateTime.Now;
			string address = $"https://api.proxyv6.net/api/reset-ip-manual?api_key={API}&host={host}&port={port}";
			string text = new WebClient().DownloadString(address);
			if (text != "")
			{
				int num = 0;
				while (num < 60)
				{
					text = new WebClient().DownloadString($"https://api.proxyv6.net/api/get-change-ip-status?api_key={API}&host={host}&port={port}");
					if (!text.Contains("\"done\""))
					{
						num++;
						Thread.Sleep(5000);
						continue;
					}
					_ = DateTime.Now.Subtract(now).TotalMilliseconds;
					return true;
				}
			}
			return false;
		}
	}
}
