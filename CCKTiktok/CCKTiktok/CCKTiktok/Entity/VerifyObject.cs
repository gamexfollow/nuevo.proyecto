namespace CCKTiktok.Entity
{
	internal class VerifyObject
	{
		public VerifyType Type { get; set; }

		public string Name { get; set; }

		public string API { get; set; }

		public VerifyObject()
		{
			Type = VerifyType.MAIL;
			Name = "";
			API = "";
		}
	}
}
