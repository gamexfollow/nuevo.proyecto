namespace CCKTiktok.Entity
{
	public class ArticleItem
	{
		public enum PostDataType
		{
			Post = 1,
			Story
		}

		public int Index { get; set; }

		public string Name { get; set; }

		public string ProductName { get; set; }

		public string Contents { get; set; }

		public bool Active { get; set; }

		public string ProductShortName { get; set; }

		public int Delay { get; set; }

		public bool FavoriteSong { get; set; }

		public string Songname { get; set; }

		public string Location { get; set; }

		public bool KickView { get; set; }

		public string PictureFolder { get; set; }

		public bool ProductOnShop { get; set; }

		public bool TopSong { get; set; }

		public bool NonMusic { get; set; }

		public string Uid { get; set; }

		public bool DeletePhotoAfterUse { get; set; }

		public long Id { get; set; }

		public int MusicVolumn { get; set; }

		public bool TurnOffVideoVolumn { get; set; }

		public bool ContentInVideoFile { get; set; }

		public bool IsContentVideo { get; set; }

		public int NumOfPic { get; set; }

		public ArticleItem()
		{
			Id = 0L;
			Name = "";
			Contents = "";
			PictureFolder = "";
			Active = true;
			Uid = "";
			DeletePhotoAfterUse = false;
			ProductName = "";
			Location = "";
			Songname = "";
			Delay = 20;
			ProductShortName = "";
			TopSong = false;
			FavoriteSong = false;
			Index = 0;
			MusicVolumn = 100;
			TurnOffVideoVolumn = false;
			NonMusic = true;
			ContentInVideoFile = false;
			NumOfPic = 1;
			IsContentVideo = true;
			KickView = false;
			ProductOnShop = true;
		}
	}
}
