using System;
using System.Linq;

namespace CCKTiktok.Bussiness
{
	public class Account
	{
		public string User => accounts.Contains('|') ? accounts.Split('|')[0].ToLower() : "";

		public string Pass => accounts.Contains('|') ? accounts.Split('|')[1] : "";

		public bool success { get; set; }

		public string message { get; set; }

		public string accounts { get; set; }

		public int balance { get; set; }

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public Account()
		{
			accounts = "|";
			success = true;
		}
	}
}
