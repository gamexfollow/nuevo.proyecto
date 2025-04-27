using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	internal class achicaptcha
	{
		private string api = "";

		public achicaptcha(string apikey = "")
		{
			api = apikey;
		}

		private static string ImageToBase64(string imagePath)
		{
			try
			{
				byte[] inArray = File.ReadAllBytes(imagePath);
				return Convert.ToBase64String(inArray);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return string.Empty;
			}
		}

		public List<int> ObjectCaptcha(string path)
		{
			string url = "https://api.achicaptcha.com/createTask";
			Root root = new Root();
			root.clientKey = api;
			root.task = new CaptchaTask
			{
				image = ImageToBase64(path),
				subType = 2,
				type = "TiktokCaptchaTask"
			};
			string text = new Utils().PostData(url, new JavaScriptSerializer().Serialize(root));
			Thread.Sleep(5000);
			string value = "";
			if (text.Contains("taskId"))
			{
				dynamic val = new JavaScriptSerializer().DeserializeObject(text);
				value = val["taskId"].ToString();
			}
			url = "http://api.achicaptcha.com/getTaskResult";
			Dictionary<string, string> obj = new Dictionary<string, string>
			{
				{ "clientKey", api },
				{ "taskId", value }
			};
			int num = 10;
			string text2 = "";
			dynamic val2;
			do
			{
				if (num-- > 0)
				{
					text = new Utils().PostData(url, new JavaScriptSerializer().Serialize(obj));
					val2 = new JavaScriptSerializer().DeserializeObject(text);
					bool flag;
					if ((!(flag = text.Contains("status"))) ? ((object)flag) : (flag & (val2["status"].ToString() != "ready")))
					{
						Thread.Sleep(5000);
					}
					continue;
				}
				return new List<int>();
			}
			while (!text.Contains("solution"));
			text2 = val2["solution"].ToString();
			return text2.Split(',').Select(int.Parse).ToList();
		}

		public int RotateCaptcha(string path)
		{
			string url = "https://api.achicaptcha.com/createTask";
			Root root = new Root();
			root.clientKey = api;
			root.task = new CaptchaTask
			{
				image = ImageToBase64(path),
				subType = 3,
				type = "TiktokCaptchaTask"
			};
			string text = new Utils().PostData(url, new JavaScriptSerializer().Serialize(root));
			Thread.Sleep(5000);
			string value = "";
			if (text.Contains("taskId"))
			{
				dynamic val = new JavaScriptSerializer().DeserializeObject(text);
				value = val["taskId"].ToString();
			}
			url = "http://api.achicaptcha.com/getTaskResult";
			Dictionary<string, string> obj = new Dictionary<string, string>
			{
				{ "clientKey", api },
				{ "taskId", value }
			};
			int num = 10;
			int num2 = 0;
			while (num-- > 0)
			{
				text = new Utils().PostData(url, new JavaScriptSerializer().Serialize(obj));
				dynamic val2 = new JavaScriptSerializer().DeserializeObject(text);
				bool flag;
				if ((!(flag = text.Contains("status"))) ? ((object)flag) : (flag & (val2["status"].ToString() != "ready")))
				{
					Thread.Sleep(5000);
				}
				if (text.Contains("solution"))
				{
					return Utils.Convert2Int(val2["solution"].ToString());
				}
			}
			return 0;
		}

		public string GetBalance()
		{
			string url = "https://api.achicaptcha.com/checkClientKey";
			Dictionary<string, string> obj = new Dictionary<string, string> { { "clientKey", api } };
			string text = new Utils().PostData(url, new JavaScriptSerializer().Serialize(obj));
			dynamic val = new JavaScriptSerializer().DeserializeObject(text);
			bool flag;
			if (!(((!(flag = text.Contains("data"))) ? ((object)flag) : (flag & (val["data"] != null))) ? true : false))
			{
				return "0";
			}
			return val["data"]["balance"].ToString();
		}
	}
}
