using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class ShopLikeProxy
	{
		private static object removeFile = new object();

		public static string GetFirstItemFromFile(string file, bool remove = true)
		{
			if (File.Exists(file))
			{
				lock (removeFile)
				{
					try
					{
						List<string> list = File.ReadAllLines(file).ToList();
						if (list.Count != 0)
						{
							string text = list[0];
							list.RemoveAt(0);
							if (!remove)
							{
								list.Add(text);
							}
							File.WriteAllLines(file, list);
							list.Clear();
							list = null;
							return text;
						}
						return "";
					}
					catch
					{
					}
				}
			}
			return "";
		}

		public string GetNewProxy()
		{
			string firstItemFromFile = GetFirstItemFromFile(CaChuaConstant.Shoplive_Proxy, remove: false);
			if (firstItemFromFile != "")
			{
				using WebClient webClient = new WebClient();
				string[] array = firstItemFromFile.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string text = webClient.DownloadString(string.Format("http://proxy.shoplike.vn/Api/getNewProxy?access_token={0}&location={1}&provider={2}", array[0], (array.Length > 1) ? array[1] : "", (array.Length > 2) ? array[2] : ""));
				if (text != null && text != "")
				{
					if (text.Contains("\"error\""))
					{
						text = webClient.DownloadString($"http://proxy.shoplike.vn/Api/getCurrentProxy?access_token={array[0]}");
					}
					dynamic val = new JavaScriptSerializer().DeserializeObject(text);
					if (val.ContainsKey("data"))
					{
						dynamic val2 = val["data"]["proxy"];
						return val2;
					}
				}
			}
			return "";
		}
	}
}
