using System;

namespace CCKTiktok.Bussiness
{
	public class Permission
	{
		public bool IsActive { get; set; }

		public int PermissionId { get; set; }

		public DateTime From { get; set; }

		public DateTime To { get; set; }

		public int NumberOfPhone { get; set; }

		public Permission()
		{
			PermissionId = 0;
			From = DateTime.Now.AddDays(-2.0);
			To = DateTime.Now.AddDays(-1.0);
			IsActive = false;
			NumberOfPhone = 10;
		}
	}
}
