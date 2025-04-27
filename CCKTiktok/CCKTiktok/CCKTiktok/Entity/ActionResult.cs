namespace CCKTiktok.Entity
{
	public class ActionResult
	{
		public bool Success { get; set; }

		public string Source { get; set; }

		public string Message { get; set; }

		public ActionResult()
		{
			Success = false;
			Message = "";
			Source = "";
		}

		public ActionResult(bool Success, string Message, string Source)
		{
			this.Success = false;
			this.Source = Source;
			this.Message = Message;
		}

		public ActionResult(bool Success, string Message)
		{
			this.Success = Success;
			Source = "";
			this.Message = Message;
		}
	}
}
