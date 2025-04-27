namespace CCKTiktok.Bussiness
{
	public class TMProxyInfo
	{
		public string sign { get; set; }

		public string api_key { get; set; }

		public int id_location { get; set; }

		public TMProxyInfo()
		{
			api_key = "";
			id_location = 0;
			sign = "";
		}
	}
}
