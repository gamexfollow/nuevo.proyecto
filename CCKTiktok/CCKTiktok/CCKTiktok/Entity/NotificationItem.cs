using CCKTiktok.DAL;

namespace CCKTiktok.Entity
{
	public class NotificationItem
	{
		public NotificationItemType Type { get; set; }

		public string story_fbid { get; set; }

		public string uid { get; set; }

		public string text { get; set; }

		public string id { get; set; }

		public string comment_id { get; set; }

		public string reply_comment_id { get; set; }

		public string seennotification { get; set; }

		public string Link { get; set; }

		public string pageID { get; set; }

		public string cloneId { get; set; }

		public NotificationItem(string uid, NotificationItemType type)
		{
			story_fbid = "";
			id = "";
			comment_id = "";
			reply_comment_id = "";
			seennotification = "";
			this.uid = uid;
			Type = type;
			Link = "";
			pageID = "";
			text = "";
		}

		public void Insert()
		{
			SQLiteUtils sQLiteUtils = new SQLiteUtils();
			try
			{
				sQLiteUtils.ExecuteQuery(string.Format("Insert Into cck_InboxAndComment(story_fbid,uid,text,comment_id,reply_comment_id,seennotification,Link,pageID,Type,cloneId) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", story_fbid, uid, text.Replace("'", ""), comment_id, reply_comment_id, seennotification, Link, pageID, Type, cloneId));
			}
			catch
			{
				try
				{
					sQLiteUtils.ExecuteQuery("CREATE TABLE \"cck_InboxAndComment\" ( \"story_fbid\" TEXT, \"Type\" TEXT, \"uid\" TEXT, \"text\" TEXT, \"id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, \"comment_id\" TEXT, \"reply_comment_id\" NUMERIC, \"seennotification\" NUMERIC, \"Link\" TEXT, \"pageID\" TEXT, \"cloneId\" TEXT, \"created\" TEXT, \"status\" INTEGER);");
				}
				catch
				{
				}
			}
		}
	}
}
