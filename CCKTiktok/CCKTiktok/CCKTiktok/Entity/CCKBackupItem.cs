namespace CCKTiktok.Entity
{
	public class CCKBackupItem
	{
		public string Email { get; set; }

		public string PassEmail { get; set; }

		public string Uid { get; set; }

		public string Password { get; set; }

		public string PrivateKey { get; set; }

		public string Brand { get; set; }

		public CCKBackupItem()
		{
			Uid = "";
			Password = "";
			PrivateKey = "";
			Brand = "";
			Email = "";
			PassEmail = "";
		}
	}
}
