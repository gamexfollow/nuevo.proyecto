using OpenQA.Selenium.Remote;

namespace CCKTiktok.Bussiness
{
	public class PhoneInfo
	{
		private string paramsValue;

		private string orderValue;

		private string composerValue;

		private bool expressionValue;

		private string _ManagerValue;

		private string _ResolverValue;

		public int width = 0;

		public int heigth = 0;

		public RemoteWebDriver driver { get; set; }

		public string platformName { get; set; }

		public string deviceName { get; set; }

		public string udid { get; set; }

		public bool is_running { get; set; }

		public string message { get; set; }

		public string port { get; set; }
	}
}
