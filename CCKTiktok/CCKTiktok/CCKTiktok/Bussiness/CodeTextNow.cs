using System.Net;
using System.Threading;

namespace CCKTiktok.Bussiness
{
	public class CodeTextNow
	{
		public string API { get; set; }

		public CodeTextNow(string api = "8bc99d4d371c81244e9e660a2dd09957")
		{
			API = api;
		}

		public string GetPhoneNumber()
		{
			return GetUrl($"http://codetextnow.com/api.php?apikey={API}&action=create-request&serviceId=1&count=1");
		}

		public string GetCode(string requestid)
		{
			string text = "";
			int num = 0;
			while (text == "" && num < 10)
			{
				text = GetUrl(string.Format("http://codetextnow.com/api.php?apikey={0}&action=data-request&requestId={2}", API, requestid));
				Thread.Sleep(5000);
				num++;
			}
			return text;
		}

		private string GetUrl(string url)
		{
			try
			{
				return new WebClient().DownloadString(url);
			}
			catch
			{
			}
			return "";
		}
	}
}
