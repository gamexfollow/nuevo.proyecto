namespace CCKTiktok.Entity
{
	public class LikePostItem
	{
		public bool ViewOnly { get; set; }

		public string ListPost { get; set; }

		public int LikeCount { get; set; }

		public int Delay { get; set; }

		public int ViewCount { get; set; }

		public int ViewDelay { get; set; }

		public int ViewProductDelay { get; set; }

		public bool ViewProduct { get; set; }

		public LikePostItem()
		{
			ListPost = "";
			LikeCount = 0;
			Delay = 1;
			ViewDelay = 1;
			ViewCount = 1;
			ViewOnly = false;
			ViewProduct = false;
			ViewProductDelay = 0;
		}
	}
}
