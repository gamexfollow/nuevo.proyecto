namespace CCKTiktok.Bussiness
{
	public class ViewByKeyword
	{
		public bool Repost { get; set; }

		public ViewByKeyword_Type Type { get; set; }

		public string Keyword { get; set; }

		public int ViewVideoFrom { get; set; }

		public int ViewVideoTo { get; set; }

		public bool FollowShop { get; set; }

		public bool IsTym { get; set; }

		public bool IsFavorite { get; set; }

		public int FollowFrom { get; set; }

		public int FollowTo { get; set; }

		public int CommentFrom { get; set; }

		public int CommentTo { get; set; }

		public int FollowDelay { get; set; }

		public int CommentDelay { get; set; }

		public int SuggestionDelay { get; set; }

		public bool RemoveComment { get; set; }

		public ViewByKeyword()
		{
			Keyword = "";
			ViewVideoFrom = 0;
			ViewVideoTo = 0;
			IsTym = false;
			IsFavorite = false;
			FollowFrom = 0;
			FollowShop = false;
			FollowTo = 0;
			CommentFrom = 0;
			CommentTo = 0;
			CommentDelay = 1;
			FollowDelay = 1;
			RemoveComment = false;
			Type = ViewByKeyword_Type.Keyword;
			Repost = false;
			SuggestionDelay = 5;
		}
	}
}
