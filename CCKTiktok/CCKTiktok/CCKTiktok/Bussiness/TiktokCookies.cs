using System;

namespace CCKTiktok.Bussiness
{
	public class TiktokCookies
	{
		public string domain { get; set; }

		public double expirationDate { get; set; }

		public bool httpOnly { get; set; }

		public string name { get; set; }

		public string path { get; set; }

		public bool session { get; set; }

		public string storeId { get; set; }

		public string value { get; set; }

		public TiktokCookies()
		{
			domain = ".tiktok.com";
			expirationDate = DateTime.Now.ToFileTime();
			httpOnly = false;
			name = "";
			path = "/";
			session = false;
			storeId = "0";
			value = "";
		}
	}
}
