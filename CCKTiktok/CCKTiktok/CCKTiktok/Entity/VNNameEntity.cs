using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class VNNameEntity
	{
		public List<string> FirstName { get; set; }

		public List<string> MidleName { get; set; }

		public List<string> LastName { get; set; }

		public VNNameEntity()
		{
			FirstName = new List<string>();
			MidleName = new List<string>();
			LastName = new List<string>();
		}

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
