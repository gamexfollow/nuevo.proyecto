using System;

namespace CCKTiktok.Entity
{
	public class GroupPostConfig
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Content { get; set; }

		public string GroupId { get; set; }

		public string AccountId { get; set; }

		public DateTime PostDate { get; set; }

		public bool Active { get; set; }

		public string FolderPicture { get; set; }

		public GroupPostConfig()
		{
			Content = "";
			GroupId = "";
			AccountId = "";
			Active = false;
			PostDate = DateTime.Now;
			FolderPicture = "";
			Name = "";
			Id = DateTime.Now.ToFileTime();
		}
	}
}
