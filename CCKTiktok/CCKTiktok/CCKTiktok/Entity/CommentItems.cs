using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class CommentItems
	{
		public bool IsRandom { get; set; }

		public List<string> ListID { get; set; }

		public string FileComment { get; set; }

		public string Tags { get; set; }

		public int CommentCount { get; set; }

		public int Delay { get; set; }

		public int DelayViewLink { get; set; }

		public bool Deleted { get; set; }

		public bool MultiLine { get; set; }

		public CommentItems()
		{
			IsRandom = true;
			ListID = new List<string>();
			FileComment = "";
			CommentCount = 0;
			Delay = 1;
			Tags = "";
			Deleted = true;
			MultiLine = false;
			DelayViewLink = 5;
		}
	}
}
