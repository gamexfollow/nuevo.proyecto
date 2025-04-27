using System;
using System.Collections.Generic;
using System.Data;
using CCKTiktok.DAL;
using CCKTiktok.Entity;

namespace CCKTiktok.Bussiness
{
	public class BOUserComment
	{
		private SQLiteUtils sql = new SQLiteUtils();

		public string TABLE = "CREATE TABLE 'NewsfeedComment' ('id' INTEGER NOT NULL UNIQUE,'uid' TEXT,'comment' TEXT,'updatedtime' TEXT, 'status' INTEGER, PRIMARY KEY ('id' AUTOINCREMENT));";

		public UserComments Mapping(DataRow row)
		{
			if (row == null)
			{
				return new UserComments();
			}
			return new UserComments
			{
				status = Utils.Convert2Int(row["id"].ToString()),
				comment = row["comment"].ToString(),
				commetid = Utils.Convert2Int(row["commetid"].ToString()),
				uid = row["uid"].ToString(),
				updated_time = ((row["updated_time"] != "") ? Convert.ToDateTime(row["updated_time"].ToString()) : DateTime.MinValue)
			};
		}

		public List<UserComments> GetCommentByUid(string uid)
		{
			List<UserComments> list = new List<UserComments>();
			DataTable dataTable = sql.ExecuteQuery($"select * from NewsfeedComment where uid='{uid}' order by id desc");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					new UserComments();
					list.Add(Mapping(dataTable.Rows[i]));
				}
			}
			return list;
		}

		public void InsertComment(List<string> comments, string uid)
		{
			foreach (string comment in comments)
			{
				sql.ExecuteQuery(string.Format("insert into NewsfeedComment (uid, comment, updatedtime,status) values ('{0}','{1}','{2}',0)", uid, comment, ""));
			}
		}

		public void DeleteComment(string id)
		{
			sql.ExecuteQuery($"delete from NewsfeedComment where id= '{id}'");
		}

		public void DeleteCommentByUid(string uid)
		{
			sql.ExecuteQuery($"delete from NewsfeedComment where uid= '{uid}'");
		}
	}
}
