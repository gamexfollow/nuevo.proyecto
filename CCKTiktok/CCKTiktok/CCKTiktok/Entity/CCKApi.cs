using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class CCKApi
	{
		public string Url { get; set; }

		public string FileName { get; set; }

		public string Cookies { get; set; }

		public List<string> Data { get; set; }

		public string Method { get; set; }

		public CCKApi()
		{
			Url = "";
			FileName = "";
			Cookies = "";
			Data = new List<string>();
			Method = "GET";
		}

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
