namespace CCKTiktok.Bussiness
{
	public class TDSEntity
	{
		public bool IsSuccess { get; set; }

		public string Message { get; set; }

		public string user_name { get; set; }

		public string password { get; set; }

		public string token { get; set; }

		public string proxy { get; set; }

		public TDSEntity()
		{
			user_name = "";
			password = "";
			token = "";
			IsSuccess = false;
			Message = "";
			proxy = "";
		}
	}
}
