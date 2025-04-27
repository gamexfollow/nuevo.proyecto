namespace CCKTiktok.Entity
{
	public class LiveConfigEntity
	{
		public bool Follow { get; set; }

		public bool CopyLink { get; set; }

		public ViewType Type { get; set; }

		public string Keyword { get; set; }

		public string ChannelName { get; set; }

		public string Link { get; set; }

		public int TimeFrom { get; set; }

		public int TimeTo { get; set; }

		public int NumOfClick_From { get; set; }

		public int NumOfClick_To { get; set; }

		public int CommentRepeatCount { get; set; }

		public int TymRepeatCount { get; set; }

		public int CommentDelayFrom { get; set; }

		public int CommentDelayTo { get; set; }

		public bool RemoveComment { get; set; }

		public int ShareCount { get; set; }

		public bool ViewShoppingCard { get; set; }

		public bool ViewProduct { get; set; }

		public int DelayLiveView { get; set; }

		public bool ClickGimProduct { get; set; }

		public int DelayGimFrom { get; set; }

		public int DelayGimto { get; set; }

		public LiveConfigEntity()
		{
			Link = "";
			TimeFrom = 10;
			TimeTo = 20;
			NumOfClick_From = 1;
			NumOfClick_To = 2;
			CommentRepeatCount = 1;
			CommentDelayFrom = 1;
			CommentDelayTo = 2;
			ShareCount = 0;
			ViewShoppingCard = false;
			ViewProduct = false;
			TymRepeatCount = 1;
			Type = ViewType.Link;
			ChannelName = "";
			RemoveComment = true;
			Keyword = "";
			CopyLink = false;
			Follow = false;
			DelayLiveView = 3;
			ClickGimProduct = false;
			DelayGimto = 10;
			DelayGimFrom = 1;
		}
	}
}
