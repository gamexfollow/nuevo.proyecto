namespace CCKTiktok.Entity
{
	internal class NewsFeedItem
	{
		public bool RemoveComment { get; set; }

		public bool ForYou { get; set; }

		public bool Following { get; set; }

		public string ShopName { get; set; }

		public int TimeFrom { get; set; }

		public int ViewVideoTimeFrom { get; set; }

		public int ViewVideoTimeTo { get; set; }

		public int FavoriteViewVideoTimeFrom { get; set; }

		public int FavoriteViewVideoTimeTo { get; set; }

		public int TimeTo { get; set; }

		public int LikeCount { get; set; }

		public int Percent { get; set; }

		public bool Repost { get; set; }

		public int PercentRepost { get; set; }

		public bool IsSpin { get; set; }

		public bool Follow { get; set; }

		public int FollowFrom { get; set; }

		public int FollowTo { get; set; }

		public NewsFeedItem()
		{
			TimeFrom = 10;
			TimeTo = 20;
			LikeCount = 1;
			Percent = 100;
			IsSpin = true;
			RemoveComment = false;
			ShopName = "";
			ViewVideoTimeFrom = 3;
			ViewVideoTimeTo = 5;
			FavoriteViewVideoTimeFrom = 10;
			FavoriteViewVideoTimeTo = 15;
			ForYou = true;
			Following = false;
			Repost = false;
			PercentRepost = 0;
			Follow = false;
			FollowFrom = 0;
			FollowTo = 0;
		}
	}
}
