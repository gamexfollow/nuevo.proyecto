using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class ReviewItem
	{
		public string PageId { get; set; }

		public List<string> Contents { get; set; }

		public ReviewItem()
		{
			PageId = "";
			Contents = new List<string>();
		}
	}
}
