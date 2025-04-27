using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class GroupDataEntity
	{
		public class Data
		{
			public string updated_time { get; set; }

			public string message { get; set; }

			public string id { get; set; }

			public Data()
			{
				updated_time = "";
				message = "";
				id = "";
			}
		}

		public class Paging
		{
			public string next { get; set; }

			public Paging()
			{
				next = "";
			}
		}

		public Paging paging { get; set; }

		public List<Data> data { get; set; }

		public GroupDataEntity()
		{
			paging = new Paging();
			data = new List<Data>();
		}

		internal void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
