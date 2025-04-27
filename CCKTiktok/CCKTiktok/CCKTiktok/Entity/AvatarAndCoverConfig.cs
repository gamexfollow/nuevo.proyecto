namespace CCKTiktok.Entity
{
	public class AvatarAndCoverConfig
	{
		public bool IsChangeAvatarIfExist { get; set; }

		public bool IsChangeCoverIfExist { get; set; }

		public bool DeleteCover { get; set; }

		public bool DeleteAvatar { get; set; }

		public AvatarAndCoverConfig()
		{
			IsChangeAvatarIfExist = false;
			IsChangeCoverIfExist = false;
			DeleteAvatar = false;
			DeleteCover = false;
		}
	}
}
