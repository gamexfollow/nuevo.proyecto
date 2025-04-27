using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using OpenQA.Selenium.Remote;

namespace CCKTiktok.Bussiness
{
	public class OmoCaptcha
	{
		private string api = "";

		public OmoCaptcha(string api)
		{
			this.api = api;
		}

		public decimal GetBalance()
		{
			string url = "https://omocaptcha.com/api/getBalance";
			string text = new Utils().PostData(url, $"{{\"api_token\":\"{api}\"}}");
			if (text.Contains("balance"))
			{
				dynamic val = new JavaScriptSerializer().DeserializeObject(text);
				return Convert.ToDecimal(val["balance"]);
			}
			return 0m;
		}

		public void GetService()
		{
			string url = "https://omocaptcha.com/api/getService";
			string text = new Utils().PostData(url, $"{{\"api_token\":\"{api}\"}}");
			if (text.Contains("message"))
			{
				new JavaScriptSerializer().DeserializeObject(text);
			}
		}

		public static string ImageToBase64(string _imagePath, out int with, out int height)
		{
			string text = null;
			with = 0;
			height = 0;
			using Image image = Image.FromFile(_imagePath);
			with = image.Width;
			height = image.Height;
			using MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, image.RawFormat);
			byte[] inArray = memoryStream.ToArray();
			return Convert.ToBase64String(inArray);
		}

		public List<Point> MakeRequestChose2Point(RemoteWebDriver driver, int with, int height)
		{
			try
			{
				string url = "https://omocaptcha.com/api/createJob";
				string text = driver.GetScreenshot().AsBase64EncodedString.Replace(Environment.NewLine, "");
				Thread.Sleep(2000);
				string text2 = new Utils().PostData(url, $"{{\"api_token\": \"{api}\",\"data\": {{\t\"type_job_id\": \"{25}\",\t\"image_base64\": \"{text}\",\"width_view\": {with},\"height_view\": {height}}}}}");
				Thread.Sleep(2000);
				if (text2 == "")
				{
					File.AppendAllLines("Log\\omocaptcha.txt", new List<string> { "Tao Job Omoocaptcha khong thanh cong" });
					return new List<Point>();
				}
				dynamic val = new JavaScriptSerializer().DeserializeObject(text2);
				text = "";
				Thread.Sleep(2000);
				url = "https://omocaptcha.com/api/getJobResult";
				int num = 0;
				while (num++ < 5)
				{
					text2 = new Utils().PostData(url, string.Format("{{\"api_token\":\"{0}\", \"job_id\" : {1} }}", api, val["job_id"]));
					if (!text2.Contains("running"))
					{
						File.AppendAllLines("Log\\omocaptcha.txt", new List<string> { text2 });
					}
					else
					{
						Thread.Sleep(5000);
					}
				}
				Thread.Sleep(2000);
				if (text2.Contains("result"))
				{
					val = new JavaScriptSerializer().DeserializeObject(text2);
					dynamic val2 = val["result"];
					if (val2 != null)
					{
						dynamic val3 = val2.ToString().Split("|".ToCharArray());
						if (val3 != null && val3.Length == 4)
						{
							List<Point> list = new List<Point>();
							list.Add(new Point
							{
								X = Convert.ToInt32(val3[0]),
								Y = Convert.ToInt32(val3[1])
							});
							list.Add(new Point
							{
								X = Convert.ToInt32(val3[2]),
								Y = Convert.ToInt32(val3[3])
							});
							return list;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Capcha 2 chu - bi loi ", ex.Message);
			}
			return new List<Point>();
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

		public int CaptchaXoay(string bigPic, string smallPic)
		{
			try
			{
				Thread.Sleep(2000);
				string url = "https://omocaptcha.com/api/createJob";
				string text = new Utils().PostData(url, $"{{\"api_token\": \"{api}\",\"data\": {{\t\"type_job_id\": \"23\",\t\"image_base64\": \"{ImageToBase64(bigPic)}|{ImageToBase64(smallPic)}\"}}}}");
				Thread.Sleep(2000);
				dynamic val = new JavaScriptSerializer().DeserializeObject(text);
				Thread.Sleep(2000);
				url = "https://omocaptcha.com/api/getJobResult";
				int num = 0;
				while (num++ < 5)
				{
					text = new Utils().PostData(url, string.Format("{{\"api_token\":\"{0}\", \"job_id\" : {1} }}", api, val["job_id"]));
					if (text.Contains("running"))
					{
						Thread.Sleep(5000);
					}
					else if (text.Contains("success"))
					{
						break;
					}
				}
				Thread.Sleep(2000);
				File.AppendAllLines("omocaptcha.txt", new List<string> { text });
				if (text.Contains("result"))
				{
					val = new JavaScriptSerializer().DeserializeObject(text);
					dynamic val2 = val["result"];
					if (val2 != null && Utils.Convert2Int(val2) > 0)
					{
						return Utils.Convert2Int(val2);
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Capcha xoay - bi loi ", ex.Message);
			}
			return 0;
		}
	}
}
