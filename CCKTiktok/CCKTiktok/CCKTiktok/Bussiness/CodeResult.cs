namespace CCKTiktok.Bussiness
{
	public class CodeResult
	{
		public string Message { get; set; }

		public string Code { get; set; }

		public bool Success { get; set; }

		public string PhoneOrEmail { get; set; }

		public string SessionId { get; set; }

		public CodeResult()
		{
			Message = "";
			Code = "";
			PhoneOrEmail = "";
			Success = false;
			SessionId = "";
		}
	}
}
