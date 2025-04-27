namespace CCKTiktok.Bussiness
{
	public class SpamCommentByUIDEntity
	{
		public bool CommentSingleLine { get; set; }

		public string Comments { get; set; }

		public string Picture { get; set; }

		public string Uid { get; set; }

		public bool DeleteImage { get; set; }

		public bool LikePost { get; set; }

		public bool DeleteUid { get; set; }

		public SpamType Type { get; set; }

		public string Keyword { get; set; }

		public int NumberOfComment { get; set; }

		public int Delay { get; set; }

		public SpamCommentByUIDEntity()
		{
			Comments = "";
			Picture = "";
			Uid = "";
			DeleteImage = false;
			DeleteUid = false;
			NumberOfComment = 1;
			Delay = 5;
			CommentSingleLine = true;
			Keyword = "";
			LikePost = false;
		}
	}
}
