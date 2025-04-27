namespace CCKTiktok.Bussiness
{
	internal class LoginResult
	{
		public bool Success { get; set; }

		public string Message { get; set; }

		public LoginResult()
		{
			Success = false;
			Message = "";
		}
	}
}
