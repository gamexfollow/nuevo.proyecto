using System.Collections.Generic;
using System.Web;

namespace CCKTiktok.Bussiness
{
	public class UrlUtility
	{
		private SecurityController control;

		public UrlUtility()
		{
			control = new SecurityController();
		}

		public string EncodeParam(Dictionary<string, string> url)
		{
			string text = "";
			foreach (KeyValuePair<string, string> item in url)
			{
				text = text + item.Key + "=" + HttpUtility.UrlEncode(item.Value) + "&";
			}
			return HttpUtility.UrlEncode(control.Encrypt(text.TrimEnd('&')));
		}

		public Dictionary<string, string> GetParam()
		{
			string text = HttpContext.Current.Request.QueryString["data"];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (text != null && text.Length > 0)
			{
				string text2 = control.Decrypt(text);
				if (text2 != "")
				{
					string[] array = text2.Split('&');
					string[] array2 = array;
					foreach (string text3 in array2)
					{
						string[] array3 = text3.Split('=');
						if (array3.Length == 2 && !dictionary.ContainsKey(array3[0].Trim()))
						{
							dictionary.Add(array3[0].Trim(), array3[1].Trim());
						}
					}
				}
			}
			return dictionary;
		}
	}
}
