using System;
using System.Collections.Generic;

namespace CCKTiktok.Bussiness
{
	public class ShareItem
	{
		public bool RandomCamXuc { get; set; }

		public ShareGroupType shareGroupType { get; set; }

		public bool GetRealtimeGroup { get; set; }

		public bool DeleteCommentAfterUsing { get; set; }

		public bool InsertIcon { get; set; }

		public bool Like { get; set; }

		public bool Share2Story { get; set; }

		public string Link { get; set; }

		public List<string> Comments { get; set; }

		public List<string> Contents { get; set; }

		public LinkTye Type { get; set; }

		public decimal Post_From_Count { get; set; }

		public decimal Post_To_Count { get; set; }

		public decimal Delay_From { get; set; }

		public decimal Delay_To { get; set; }

		public decimal Delay_ViewBeforeShare { get; set; }

		public decimal Delay_ViewAfterShare { get; set; }

		public bool Share2Wall { get; set; }

		public bool Share2Group { get; set; }

		public bool Active { get; set; }

		public decimal RandomCamXuc_Count { get; set; }

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public ShareItem()
		{
			Link = "";
			Post_From_Count = 0m;
			Post_To_Count = 0m;
			Delay_From = 0m;
			Delay_To = 0m;
			Type = LinkTye.LiveStream;
			Active = false;
			InsertIcon = false;
			Delay_ViewBeforeShare = 0m;
			Delay_ViewAfterShare = 0m;
			Contents = new List<string>();
			Comments = new List<string>();
			InsertIcon = false;
			Share2Story = false;
			Share2Group = false;
			Share2Wall = false;
			RandomCamXuc = false;
			RandomCamXuc_Count = 0m;
			shareGroupType = ShareGroupType.Random;
			GetRealtimeGroup = false;
			DeleteCommentAfterUsing = true;
		}
	}
}
