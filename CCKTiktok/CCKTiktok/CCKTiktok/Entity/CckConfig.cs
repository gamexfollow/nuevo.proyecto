namespace CCKTiktok.Entity
{
	public class CckConfig
	{
		public SystemConfigItem AddFriendByKeyword { get; set; }

		public SystemConfigItem DongYKetBan { get; set; }

		public SystemConfigItem KetBanGoiY { get; set; }

		public SystemConfigItem KetBanUID_TuongTac { get; set; }

		public SystemConfigItem HuyLoiMoiKetBan { get; set; }

		public SystemConfigItem LikeBaiViet { get; set; }

		public SystemConfigItem JoinGroup { get; set; }

		public SystemConfigItem LikePage { get; set; }

		public SystemConfigItem LikePost { get; set; }

		public SystemConfigItem LuotTuong { get; set; }

		public SystemConfigItem CMSN { get; set; }

		public SystemConfigItem KetBanTrongNhom { get; set; }

		public SystemConfigItem CommentDao { get; set; }

		public SystemConfigItem LeaveGroup { get; set; }

		public SystemConfigItem XemVideo { get; set; }

		public SystemConfigItem TuongTacGroup { get; set; }

		public SystemConfigItem TuongTacPage { get; set; }

		public SystemConfigItem SeedingGroup { get; set; }

		public SystemConfigItem FollowPageOnWatch { get; set; }

		public SystemConfigItem InviteFriendJoinGroup { get; set; }

		public SystemConfigItem InviteFriendLikePage { get; set; }

		public SystemConfigItem ViewPage { get; set; }

		public CckConfig()
		{
			DongYKetBan = new SystemConfigItem();
			KetBanGoiY = new SystemConfigItem();
			HuyLoiMoiKetBan = new SystemConfigItem();
			LikeBaiViet = new SystemConfigItem();
			JoinGroup = new SystemConfigItem();
			LikePage = new SystemConfigItem();
			LikePost = new SystemConfigItem();
			LuotTuong = new SystemConfigItem();
			CMSN = new SystemConfigItem();
			KetBanTrongNhom = new SystemConfigItem();
			CommentDao = new SystemConfigItem();
			LeaveGroup = new SystemConfigItem();
			XemVideo = new SystemConfigItem();
			TuongTacGroup = new SystemConfigItem();
			TuongTacPage = new SystemConfigItem();
			SeedingGroup = new SystemConfigItem();
			FollowPageOnWatch = new SystemConfigItem();
			AddFriendByKeyword = new SystemConfigItem();
			InviteFriendLikePage = new SystemConfigItem();
			InviteFriendJoinGroup = new SystemConfigItem();
			ViewPage = new SystemConfigItem();
		}
	}
}
