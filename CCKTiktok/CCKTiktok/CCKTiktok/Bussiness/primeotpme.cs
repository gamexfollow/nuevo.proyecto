using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace CCKTiktok.Bussiness
{
	public class primeotpme
	{
		public string API { get; set; }

		public primeotpme(string api = "cc992d47aba7ada69df9db79e4f6ae41")
		{
			API = api;
		}

		public string GetPhoneNumber(out string requestid)
		{
			try
			{
				string url = GetUrl($"http://private.primeotp.me/apiv3.php?apikey={API}&action=create-request&serviceId=1&count=1");
				Regex regex = new Regex("\"sdt\":\"([0-9]+)\"(.*?)\"requestId\":([0-9]+)");
				Match match = regex.Match(url);
				requestid = match.Groups[3].Value;
				return match.Groups[1].Value;
			}
			catch
			{
				requestid = "0";
				return "";
			}
		}

		public string GetCode(string requestid)
		{
			try
			{
				string text = "";
				int num = 0;
				while (text == "" && num < 20)
				{
					text = GetUrl($"http://private.primeotp.me/apiv3.php?apikey={API}&action=data-request&requestId={requestid}");
					Regex regex = new Regex("\"otp\":\"([0-9]+)\"");
					Match match = regex.Match(text);
					string value = match.Groups[1].Value;
					if (!(value != ""))
					{
						Thread.Sleep(10000);
						num++;
						continue;
					}
					return value;
				}
				return "";
			}
			catch
			{
			}
			return "";
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
