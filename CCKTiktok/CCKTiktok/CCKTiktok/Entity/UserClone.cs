using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class UserClone
	{
		public class Base
		{
			public string name { get; set; }

			public string id { get; set; }

			public Base()
			{
				name = "";
				id = "";
			}
		}

		public class Cover
		{
			public string name { get; set; }

			public string id { get; set; }

			public string source { get; set; }

			public Cover()
			{
				name = "";
				id = "";
				source = "";
			}
		}

		public class Edication : Base
		{
			public Base school { get; set; }

			public string type { get; set; }

			public Edication()
			{
				school = new Base();
				type = "";
			}
		}

		public class Picture
		{
			public class Data
			{
				public string url { get; set; }

				public Data()
				{
					url = "";
				}
			}

			public Data data { get; set; }

			public Picture()
			{
				data = new Data();
			}
		}

		public class Posts
		{
			public class Data
			{
				public class Attachments
				{
					public class Data
					{
						public class Media
						{
							public class Image
							{
								public int height { get; set; }

								public int width { get; set; }

								public string src { get; set; }
							}

							public Image image { get; set; }

							public Media()
							{
								image = new Image();
							}
						}

						public string description { get; set; }

						public string url { get; set; }

						public string type { get; set; }

						public Media media { get; set; }

						public Attachments subattachments { get; set; }

						public Data()
						{
							subattachments = new Attachments();
							media = new Media();
							type = "";
							url = "";
						}
					}

					public List<Data> data { get; set; }

					public Attachments()
					{
						data = new List<Data>();
					}
				}

				public string message { get; set; }

				public string id { get; set; }

				public string full_picture { get; set; }

				public string created_time { get; set; }

				public Attachments attachments { get; set; }

				public Data()
				{
					attachments = new Attachments();
					message = "";
					id = "";
					full_picture = "";
					created_time = "";
				}
			}

			public List<Data> data { get; set; }

			public Posts()
			{
				data = new List<Data>();
			}
		}

		public string name { get; set; }

		public string id { get; set; }

		public Cover cover { get; set; }

		public string birthday { get; set; }

		public Base hometow { get; set; }

		public List<Edication> education { get; set; }

		public string gender { get; set; }

		public Base location { get; set; }

		public Picture picture { get; set; }

		public Posts posts { get; set; }

		public UserClone()
		{
			cover = new Cover();
			hometow = new Base();
			location = new Base();
			education = new List<Edication>();
			posts = new Posts();
			birthday = "";
		}
	}
}
