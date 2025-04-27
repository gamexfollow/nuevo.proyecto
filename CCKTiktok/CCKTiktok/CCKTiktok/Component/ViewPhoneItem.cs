namespace CCKTiktok.Component
{
	internal class ViewPhoneItem
	{
		public string Name { get; set; }

		public int Top { get; set; }

		public int Left { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public ViewPhoneItem()
		{
			Top = 0;
			Left = 0;
			Width = 0;
			Height = 0;
			Name = string.Empty;
		}
	}
}
