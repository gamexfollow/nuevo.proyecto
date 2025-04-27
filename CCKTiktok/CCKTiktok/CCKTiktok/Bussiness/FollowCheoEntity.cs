namespace CCKTiktok.Bussiness
{
	public class FollowCheoEntity
	{
		public FollowCheoType FollowType { get; set; }

		public string File { get; set; }

		public int Number { get; set; }

		public int Delay { get; set; }

		public FollowCheoEntity()
		{
			FollowType = FollowCheoType.Full;
			File = "";
			Number = 10;
			Delay = 5;
		}
	}
}
