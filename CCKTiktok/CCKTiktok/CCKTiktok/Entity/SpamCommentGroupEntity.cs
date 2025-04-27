using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class SpamCommentGroupEntity
	{
		public int TimeWork { get; set; }

		public int Percent { get; set; }

		public bool BackupGroup { get; set; }

		public int DelayFrom { get; set; }

		public int DelayTo { get; set; }

		public bool DeletePicture { get; set; }

		public string PictureFolder { get; set; }

		public string Comment { get; set; }

		public bool RandomIcon { get; set; }

		public bool CommentOnJoinedGroup { get; set; }

		public bool CommentOnListID { get; set; }

		public int NumberOfGroupFrom { get; set; }

		public int NumberOfGroupTo { get; set; }

		public List<string> GroupId { get; set; }

		public int CommentCountFrom { get; set; }

		public int CommentCountTo { get; set; }

		public int LikeCountFrom { get; set; }

		public int LikeCountTo { get; set; }

		public SpamCommentGroupEntity()
		{
			DeletePicture = false;
			PictureFolder = "";
			Comment = "";
			RandomIcon = false;
			CommentOnJoinedGroup = true;
			CommentOnListID = false;
			CommentCountFrom = 0;
			CommentCountTo = 0;
			LikeCountFrom = 0;
			LikeCountTo = 0;
			GroupId = new List<string>();
			NumberOfGroupFrom = 0;
			NumberOfGroupTo = 0;
			DelayFrom = 0;
			DelayTo = 0;
			BackupGroup = true;
			Percent = 10;
			TimeWork = 60;
		}
	}
}
