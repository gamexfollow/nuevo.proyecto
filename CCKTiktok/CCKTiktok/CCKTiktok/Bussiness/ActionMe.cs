namespace CCKTiktok.Bussiness
{
	public class ActionMe
	{
		public bool Active { get; set; }

		public int From { get; set; }

		public int To { get; set; }

		public ActionMe()
		{
			Active = false;
			From = 1;
			To = 2;
		}
	}
}
