namespace CCKTiktok.Bussiness
{
	public class ScrcpyEntity
	{
		public string DeviceId { get; set; }

		public string DeviceName { get; set; }

		public int Top { get; set; }

		public int Left { get; set; }

		public int Height { get; set; }

		public int Width { get; set; }

		public ScrcpyEntity()
		{
			DeviceId = "";
			DeviceName = "";
			Top = 0;
			Left = 0;
			Width = 0;
			Height = 0;
		}
	}
}
