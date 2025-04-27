using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class IAEntity
	{
		public int DelayAdsClick { get; set; }

		public List<string> PageUrl { get; set; }

		public List<string> Comments { get; set; }

		public bool IsComment { get; set; }

		public bool IsLike { get; set; }

		public int PercentClickAds { get; set; }

		public int Speed { get; set; }

		public bool Share2Group { get; set; }

		public int Share2Group_Count { get; set; }

		public int Share2Group_Delay { get; set; }

		public bool Share2Wall { get; set; }

		public void Dispose()
		{
			GC.SuppressFinalize(true);
		}

		public IAEntity()
		{
			PageUrl = new List<string>();
			Comments = new List<string>();
			IsComment = false;
			IsLike = false;
			PercentClickAds = 0;
			Speed = 0;
			Share2Group = false;
			Share2Group_Count = 0;
			Share2Group_Delay = 0;
			Share2Wall = false;
			DelayAdsClick = 5;
		}
	}
}
