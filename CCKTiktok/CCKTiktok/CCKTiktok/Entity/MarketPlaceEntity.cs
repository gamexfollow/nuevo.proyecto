using System;

namespace CCKTiktok.Entity
{
	public class MarketPlaceEntity
	{
		public float Id { get; set; }

		public string Title { get; set; }

		public bool Share2Group { get; set; }

		public int GroupCount { get; set; }

		public string Reply { get; set; }

		public decimal Price { get; set; }

		public string Category { get; set; }

		public string Condition { get; set; }

		public string SKU { get; set; }

		public string Tags { get; set; }

		public string Picturefolder { get; set; }

		public decimal PictureCount { get; set; }

		public bool RemovePicture { get; set; }

		public string Desciption { get; set; }

		public string Location { get; set; }

		public bool Active { get; set; }

		public ProductType Type { get; set; }

		public MarketPlaceEntity()
		{
			Title = "";
			Price = 0m;
			Category = "";
			Condition = "";
			Tags = "";
			Picturefolder = "";
			PictureCount = 0m;
			RemovePicture = false;
			Desciption = "";
			Location = "";
			Id = DateTime.Now.ToFileTime();
			Active = false;
			Type = ProductType.Items;
			SKU = Guid.NewGuid().ToString("N").Substring(0, 10);
			Reply = "";
			Share2Group = false;
			GroupCount = 0;
		}
	}
}
