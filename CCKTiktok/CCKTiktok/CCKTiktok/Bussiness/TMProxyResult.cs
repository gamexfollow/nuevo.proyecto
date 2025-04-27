namespace CCKTiktok.Bussiness
{
	public class TMProxyResult
	{
		public class Data
		{
			public string location_name { get; set; }

			public string ip_allow { get; set; }

			public string https { get; set; }

			public string socks5 { get; set; }

			public int timeout { get; set; }

			public int next_request { get; set; }

			public string expired_at { get; set; }

			public Data()
			{
				ip_allow = "";
				https = "";
				socks5 = "";
				timeout = 0;
				location_name = "";
				next_request = 0;
				expired_at = "";
			}
		}

		public int code { get; set; }

		public string message { get; set; }

		public Data data { get; set; }

		public TMProxyResult()
		{
			code = -1;
			message = "";
			data = new Data();
		}
	}
}
