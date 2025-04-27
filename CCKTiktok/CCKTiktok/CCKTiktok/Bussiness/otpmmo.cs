using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class otpmmo
	{
		public class Results
		{
			public class Data
			{
				public string name { get; set; }

				public string sdt { get; set; }

				public string otp { get; set; }

				public string created_time { get; set; }

				public int requestId { get; set; }

				public Data()
				{
					name = "";
					sdt = "";
					otp = "";
					created_time = "";
					requestId = 0;
				}
			}

			public List<Data> data { get; set; }
		}

		public int status { get; set; }

		public Results results { get; set; }

		public Results.Data data { get; set; }

		public string API { get; set; }

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public otpmmo(string api = "ZWV1CM8Y2INB03951627523831")
		{
			API = api;
		}

		public string GetPhoneNumber(out string sessionId)
		{
			sessionId = "";
			return GetUrl($"https://otpmmo.xyz/textnow/api.php?apikey={API}&type=getphone&qty=1");
		}

		public string GetCode(string phone)
		{
			try
			{
				string text = "";
				int num = 0;
				while (text == "" && num < 20)
				{
					text = GetUrl($"https://otpmmo.xyz/textnow/api.php?apikey={API}&type=getotp&sdt={phone}");
					dynamic val = new JavaScriptSerializer().DeserializeObject(text);
					if (!((val != null && val.Length > 0) ? true : false))
					{
						Thread.Sleep(5000);
						num++;
						continue;
					}
					Regex regex = new Regex(" ([0-9]+) ");
					Match match = regex.Match(val[0]["otp"]);
					return match.Groups[1].Value;
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
