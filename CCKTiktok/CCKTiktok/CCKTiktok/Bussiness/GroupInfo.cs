using System;

namespace CCKTiktok.Bussiness
{
	public class GroupInfo
	{
		public string MemberId { get; set; }

		public string Category { get; set; }

		public int Member { get; set; }

		public string Uid { get; set; }

		public string Name { get; set; }

		public bool IsApproved { get; set; }

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public GroupInfo()
		{
			Member = 0;
			Uid = "";
			Name = "";
			IsApproved = true;
			Category = "";
			MemberId = "";
		}
	}
}
