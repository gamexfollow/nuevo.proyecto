namespace CCKTiktok.Entity
{
	public class SystemConfigItem
	{
		public string Name { get; set; }

		public int Num { get; set; }

		public int Delay { get; set; }

		public bool Status { get; set; }

		public SystemConfigItem()
		{
			Name = "";
			Num = 2;
			Delay = 5;
			Status = true;
		}
	}
}
