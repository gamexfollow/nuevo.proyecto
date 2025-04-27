using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class WatchVideoEntity
	{
		public string Link { get; set; }

		public decimal From { get; set; }

		public decimal To { get; set; }

		public bool Share2Wall { get; set; }

		public bool Share2Group { get; set; }

		public List<string> Comments { get; set; }

		public WatchVideoEntity()
		{
			Link = "";
			From = 60m;
			To = 120m;
			Comments = new List<string>();
			Share2Group = false;
			Share2Wall = false;
		}
	}
}
