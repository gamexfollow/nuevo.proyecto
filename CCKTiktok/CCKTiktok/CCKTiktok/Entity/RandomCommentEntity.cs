namespace CCKTiktok.Entity
{
	public class RandomCommentEntity
	{
		public bool IsActive { get; set; }

		public string Comment { get; set; }

		public decimal Rate { get; set; }

		public bool DeletePicture { get; set; }

		public string PictureFolder { get; set; }

		public RandomCommentEntity()
		{
			Comment = "";
			Rate = 5m;
			DeletePicture = false;
			PictureFolder = "";
			IsActive = true;
		}
	}
}
