namespace CCKTiktok.Entity
{
	public class RegNickEntity
	{
		public string PasswordDefault { get; set; }

		public string HotmailAPI { get; set; }

		public string TextNowAPI { get; set; }

		public int AgeFrom { get; set; }

		public int AgeTo { get; set; }

		public string EmailDomain { get; set; }

		public bool ErrorStop { get; set; }

		public bool ErrorContinue { get; set; }

		public int ErrorDelay { get; set; }

		public bool Register8AccountOnly { get; set; }

		public int GmailDelayFrom { get; set; }

		public int GmailDelayTo { get; set; }

		public RegAccountType RegType { get; set; }

		public RegNickEntity()
		{
			HotmailAPI = "";
			TextNowAPI = "";
			EmailDomain = "";
			PasswordDefault = "";
			AgeFrom = 18;
			AgeTo = 20;
			ErrorDelay = 30;
			ErrorStop = false;
			ErrorContinue = true;
			Register8AccountOnly = true;
			RegType = RegAccountType.Email;
			GmailDelayFrom = 5;
			GmailDelayTo = 6;
		}
	}
}
