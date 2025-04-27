using System;

namespace CCKTiktok.Bussiness
{
	public class CCKLanguage
	{
		public string Prefix { get; set; }

		public string key { get; set; }

		public string en { get; set; }

		public string vn { get; set; }

		public CCKLanguage()
		{
			key = Guid.NewGuid().ToString("N").ToLower();
			vn = "";
			en = "";
			Prefix = "";
		}
	}
}
