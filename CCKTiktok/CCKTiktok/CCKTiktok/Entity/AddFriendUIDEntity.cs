namespace CCKTiktok.Entity
{
	public class AddFriendUIDEntity
	{
		public bool RemoveAfterUsed { get; set; }

		public int NumberOfAcc { get; set; }

		public int Delay { get; set; }

		public AddFriendUIDEntity()
		{
			RemoveAfterUsed = true;
			NumberOfAcc = 5;
			Delay = 10;
		}
	}
}
