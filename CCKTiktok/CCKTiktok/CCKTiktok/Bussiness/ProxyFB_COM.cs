using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class ProxyFB_COM
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
						if (list.Count == 0)
						{
							return "";
						}
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
					catch
					{
					}
				}
			}
			return "";
		}

		public string GetNewProxy()
		{
			string firstItemFromFile = GetFirstItemFromFile(CaChuaConstant.ProxyFB_COM, remove: false);
			if (firstItemFromFile != "")
			{
				string[] array = firstItemFromFile.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string response = Utils.GetResponse($"http://api.proxyfb.com/api/changeProxy.php?key={array[0]}");
				if (response != null && response != "")
				{
					if (response.Contains("\"success\":false"))
					{
						response = Utils.GetResponse($"http://api.proxyfb.com/api/getProxy.php?key={array[0]}");
					}
					dynamic val = new JavaScriptSerializer().DeserializeObject(response);
					if (val.ContainsKey("proxy"))
					{
						dynamic val2 = val["proxy"];
						return val2;
					}
				}
			}
			return "127.0.0.1:8080";
		}
	}
}
