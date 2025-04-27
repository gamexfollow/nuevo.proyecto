namespace CCKTiktok.Entity
{
	public class FBlitf
	{
		public class Profile
		{
			public string uid { get; set; }

			public string name { get; set; }

			public Profile()
			{
				uid = "";
				name = "";
			}
		}

		public string access_token { get; set; }

		public Profile profile { get; set; }

		public FBlitf()
		{
			access_token = "";
			profile = new Profile();
		}
	}
}
