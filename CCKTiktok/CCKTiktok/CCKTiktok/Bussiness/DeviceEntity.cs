namespace CCKTiktok.Bussiness
{
	public class DeviceEntity
	{
		public int pId { get; set; }

		public int Id { get; set; }

		public string DeviceId { get; set; }

		public int Port { get; set; }

		public int SystemPort { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public bool Rooted { get; set; }

		public string User { get; set; }

		public string Pass { get; set; }

		public string FBUser { get; set; }

		public string Message { get; set; }

		public DeviceEntity()
		{
			Port = 4723;
			SystemPort = 8200;
			Id = 0;
			pId = 0;
			Message = "";
			Rooted = false;
		}
	}
}
