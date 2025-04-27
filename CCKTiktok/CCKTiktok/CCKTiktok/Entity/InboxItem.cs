using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class InboxItem
	{
		public class MyData
		{
			public class Messages
			{
				public class Data
				{
					public class From
					{
						public string name { get; set; }

						public string id { get; set; }
					}

					public string message { get; set; }

					public DateTime created_time { get; set; }

					public From from { get; set; }
				}

				public List<Data> data { get; set; }
			}

			public string updated_time { get; set; }

			public Messages messages { get; set; }
		}

		public List<MyData> data { get; set; }
	}
}
