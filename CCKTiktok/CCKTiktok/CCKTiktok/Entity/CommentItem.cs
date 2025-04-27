namespace CCKTiktok.Entity
{
	public class CommentItem
	{
		public InputContentType CommentType { get; set; }

		public long Id { get; set; }

		public string PostId { get; set; }

		public string Name { get; set; }

		public string Contents { get; set; }

		public string PictureFolder { get; set; }

		public bool Active { get; set; }

		public bool RemoveAfterUse { get; set; }

		public bool DeletePictureAfterUse { get; set; }

		public int TagFriendCount { get; set; }

		public CommentItem()
		{
			Id = 0L;
			PostId = "";
			Name = "";
			Contents = "";
			PictureFolder = "";
			Active = true;
			RemoveAfterUse = false;
			DeletePictureAfterUse = false;
			CommentType = InputContentType.MultiLine;
			TagFriendCount = 0;
		}
	}
}
