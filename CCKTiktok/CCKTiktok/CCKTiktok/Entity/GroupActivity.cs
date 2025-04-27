namespace CCKTiktok.Entity
{
	public class GroupActivity
	{
		public int Like { get; set; }

		public int Commemt { get; set; }

		public string GroupId { get; set; }

		public GroupActivity()
		{
			Like = 0;
			Commemt = 0;
			GroupId = "";
		}

		public override string ToString()
		{
			return $"{GroupId},{Like},{Commemt}";
		}
	}
}
