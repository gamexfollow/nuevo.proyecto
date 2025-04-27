using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class UserInfoEntity
	{
		public List<string> Bio { get; set; }

		public List<string> Favorite { get; set; }

		public List<string> Education { get; set; }

		public List<string> Hometown { get; set; }

		public List<string> CurentCity { get; set; }

		public List<string> Work { get; set; }

		public UserInfoEntity()
		{
			Education = new List<string>();
			Hometown = new List<string>();
			CurentCity = new List<string>();
			Work = new List<string>();
			Bio = new List<string>();
			Favorite = new List<string>();
		}

		internal void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
