namespace CCKTiktok.Bussiness
{
	public class TDSConfig
	{
		public int NhanTien { get; set; }

		public int Count { get; set; }

		public int DelayFrom { get; set; }

		public int DelayTo { get; set; }

		public int Stop { get; set; }

		public ActionMe MyWall { get; set; }

		public ActionMe FriendWall { get; set; }

		public bool IsBefore { get; set; }

		public TDSConfig()
		{
			Count = 1;
			DelayFrom = 1;
			DelayTo = 2;
			NhanTien = 8;
			IsBefore = false;
			MyWall = new ActionMe();
			FriendWall = new ActionMe();
			Stop = 2;
		}
	}
}
