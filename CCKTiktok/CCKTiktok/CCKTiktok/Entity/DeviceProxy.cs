using System;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Entity
{
	public class DeviceProxy
	{
		public DateTime LastChange { get; set; }

		public bool IsRotate { get; set; }

		public int Stt { get; set; }

		public string DeviceId { get; set; }

		public bool MultipleDevice { get; set; }

		public ProxyInfo Proxy { get; set; }

		public string ProxyServer { get; set; }

		public string Api { get; set; }

		public DeviceProxy()
		{
			DeviceId = "";
			Proxy = new ProxyInfo();
			Stt = 0;
			ProxyServer = "";
			Api = "";
			IsRotate = false;
			LastChange = DateTime.Now.AddMinutes(10.0);
			MultipleDevice = false;
		}

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(Proxy.UserName) && !string.IsNullOrWhiteSpace(Proxy.Password))
			{
				return $"{Proxy.Ip}:{Proxy.Port}:{Proxy.UserName}:{Proxy.Password}";
			}
			return $"{Proxy.Ip}:{Proxy.Port}";
		}
	}
}
