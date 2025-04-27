namespace CCKTiktok.Bussiness
{
	public class UpdateInfoField
	{
		public bool LogOut { get; set; }

		public bool XoaLichSuDangNhap { get; set; }

		public bool PublicInfo { get; set; }

		public bool GetLike { get; set; }

		public bool GetFollow { get; set; }

		public bool CheckAvatar { get; set; }

		public bool GetYear { get; set; }

		public UpdateInfoField()
		{
			XoaLichSuDangNhap = false;
			CheckAvatar = false;
			PublicInfo = false;
			GetFollow = false;
			GetYear = false;
			GetLike = false;
			LogOut = false;
		}
	}
}
