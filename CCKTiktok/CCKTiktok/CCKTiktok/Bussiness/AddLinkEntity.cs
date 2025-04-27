namespace CCKTiktok.Bussiness
{
	public class AddLinkEntity
	{
		public bool LinkOnly { get; set; }

		public bool LinkAndName { get; set; }

		public string FileUrl { get; set; }

		public int NumOfLink { get; set; }

		public AddLinkEntity()
		{
			LinkAndName = false;
			LinkOnly = true;
			FileUrl = "";
			NumOfLink = 1;
		}
	}
}
