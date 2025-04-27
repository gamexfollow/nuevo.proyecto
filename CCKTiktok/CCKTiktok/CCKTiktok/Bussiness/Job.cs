using System;

namespace CCKTiktok.Bussiness
{
	public class Job
	{
		public string fb_id { get; set; }

		public string type { get; set; }

		public int amount { get; set; }

		public long job_id { get; set; }

		public Job()
		{
			job_id = DateTime.Now.ToFileTime();
			fb_id = "";
			type = "";
			amount = 0;
		}
	}
}
