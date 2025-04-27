using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class MaxCloneEntity
	{
		public class Datas
		{
			public class EmailInfo
			{
				public string Email { get; set; }

				public string Password { get; set; }

				public EmailInfo()
				{
					Email = "";
					Password = "";
				}
			}

			public string TransId { get; set; }

			public List<EmailInfo> Emails { get; set; }

			public Datas()
			{
				TransId = "";
				Emails = new List<EmailInfo>();
			}
		}

		public Datas Data { get; set; }

		public MaxCloneEntity()
		{
			Data = new Datas();
		}
	}
}
