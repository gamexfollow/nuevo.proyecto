using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class GroupByKeyword
	{
		public int Delay { get; set; }

		public int PageCount { get; set; }

		public int GroupCount { get; set; }

		public List<string> GroupId { get; set; }

		public List<string> PageId { get; set; }

		public List<string> Keywords { get; set; }

		public bool IsActiveGroupType { get; set; }

		public List<string> Pages { get; set; }

		public GroupByKeyword()
		{
			GroupId = new List<string>();
			PageId = new List<string>();
			Keywords = new List<string>();
			IsActiveGroupType = false;
			PageCount = 3;
			GroupCount = 3;
			Delay = 5;
			Pages = new List<string>();
		}

		internal void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
