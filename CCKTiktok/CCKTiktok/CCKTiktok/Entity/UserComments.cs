using System;

namespace CCKTiktok.Entity
{
	public class UserComments
	{
		public string uid { get; set; }

		public string comment { get; set; }

		public int commetid { get; set; }

		public int status { get; set; }

		public DateTime updated_time { get; set; }

		public UserComments()
		{
			uid = "";
			commetid = 0;
			comment = "";
			status = 0;
			updated_time = DateTime.MinValue;
		}
	}
}
