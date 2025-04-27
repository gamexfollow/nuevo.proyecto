using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class SpamInboxEntity
	{
		public int SpamCount { get; set; }

		public List<string> Uids { get; set; }

		public bool RemoveUid { get; set; }

		public List<string> Messages { get; set; }

		public bool RemoveMessages { get; set; }

		public string PictureFolder { get; set; }

		public SpamInboxEntity()
		{
			Uids = new List<string>();
			RemoveMessages = false;
			RemoveUid = false;
			Messages = new List<string>();
			PictureFolder = "";
			SpamCount = 3;
		}
	}
}
