using System;
using System.Collections.Generic;

namespace CCKTiktok.Bussiness
{
	public class FBBackUpIitem
	{
		public class Photo
		{
			public string source { get; set; }

			public Photo()
			{
				source = "";
			}
		}

		public string uid { get; set; }

		public string name { get; set; }

		public List<Photo> photos { get; set; }

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public FBBackUpIitem()
		{
			uid = "";
			name = "";
			photos = new List<Photo>();
		}
	}
}
