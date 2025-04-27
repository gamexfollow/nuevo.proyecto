using System;

namespace CCKTiktok.Entity
{
	public class FollowFriendEntity
	{
		public decimal Number { get; set; }

		public decimal Delay { get; set; }

		public bool RemoveAfterFollow { get; set; }

		public FollowFriendEntity()
		{
			Number = 5m;
			Delay = 5m;
			RemoveAfterFollow = false;
		}

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
