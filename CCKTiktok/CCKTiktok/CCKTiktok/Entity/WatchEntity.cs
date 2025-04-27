using System;

namespace CCKTiktok.Entity
{
	public class WatchEntity
	{
		public bool Comments_Multiline { get; set; }

		public string Keyword { get; set; }

		public bool ViewRandom { get; set; }

		public bool VewByKeyword { get; set; }

		public bool RandomIcon { get; set; }

		public int ViewTimeFrom { get; set; }

		public int ViewTimeTo { get; set; }

		public bool Share2Wall { get; set; }

		public int ShareCount { get; set; }

		public string Comments { get; set; }

		public string Picture { get; set; }

		public int NumOfComment { get; set; }

		public bool FollowPage { get; set; }

		public WatchEntity()
		{
			Keyword = "";
			Share2Wall = false;
			ViewRandom = false;
			ShareCount = 0;
			VewByKeyword = false;
			ViewTimeFrom = 30;
			ViewTimeTo = 60;
			Comments = "";
			Comments_Multiline = false;
			RandomIcon = false;
			Picture = "";
			FollowPage = false;
			NumOfComment = 2;
		}

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
