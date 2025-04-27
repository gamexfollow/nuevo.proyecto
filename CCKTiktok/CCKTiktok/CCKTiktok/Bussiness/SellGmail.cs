using System;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	internal class SellGmail
	{
		public class ServiceResponse
		{
			public bool State { get; set; }

			public string Msg { get; set; }

			public string Path { get; set; }

			public ServiceData Data { get; set; }
		}

		public class ServiceData
		{
			public string ServiceName { get; set; }

			public DateTime StartTime { get; set; }

			public int Duration { get; set; }

			public DateTime EndTime { get; set; }

			public string Otp { get; set; }

			public string Gmail { get; set; }

			public int Price { get; set; }

			public string Status { get; set; }
		}

		public string API { get; set; }

		public SellGmail(string api = "")
		{
			API = api;
		}

		public string GetEmail()
		{
			string url = GetUrl($"http://sellgmail.com/api/mailselling/order-rent-mail?apiKey={API}&serviceId=5");
			dynamic val = new JavaScriptSerializer
			{
				MaxJsonLength = int.MaxValue
			}.DeserializeObject(url);
			if (!((val.ContainsKey("data") && val.ContainsKey("state") && Convert.ToBoolean(val["state"])) ? true : false))
			{
				return "";
			}
			return val["data"].ToString() + "|cck";
		}

		public string GetCode(string mail)
		{
			string text = "";
			int num = 0;
			DateTime.Now.AddDays(1.0);
			text = GetUrl($"http://sellgmail.com/api/mailselling/get-mail-otp?apiKey={API}&mail={mail}");
			if (text != "")
			{
				ServiceResponse serviceResponse = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				}.Deserialize<ServiceResponse>(text);
				if (serviceResponse != null && serviceResponse.Data.Otp != null)
				{
					dynamic otp = serviceResponse.Data.Otp;
					if (otp != null && Utils.Convert2Int(otp) > 0)
					{
						return otp;
					}
					text = "";
				}
				else
				{
					text = "";
				}
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
