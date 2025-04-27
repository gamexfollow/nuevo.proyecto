namespace CCKTiktok.Entity
{
	public class FollowEntity
	{
		public int FollowFrom { get; set; }

		public int FollowTo { get; set; }

		public int DelayTo { get; set; }

		public int DelayFrom { get; set; }

		public int WorkingFrom { get; set; }

		public int WorkingTo { get; set; }

		public FollowEntity()
		{
			FollowFrom = 1;
			FollowTo = 5;
			WorkingFrom = 30;
			WorkingTo = 60;
			DelayFrom = 1;
			DelayTo = 3;
		}
	}
}
