using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using CCKTiktok.DAL;

namespace CCKTiktok.Bussiness
{
	public class BOGroupInfo
	{
		private SQLiteUtils sql = new SQLiteUtils();

		public BOGroupInfo()
		{
			sql = new SQLiteUtils();
			AddColumn();
		}

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		public static void LogGroup(GroupInfo g, string uid)
		{
			try
			{
				new WebClient().UploadString("https://cck.vn/api/group.aspx", "POST", new JavaScriptSerializer().Serialize(g));
			}
			catch
			{
			}
		}

		public void UserGroupInsert(string uid, string groupid)
		{
			try
			{
				using SQLiteUtils sQLiteUtils = new SQLiteUtils();
				sQLiteUtils.ExecuteQuery(string.Format("Delete from UserGroups where id_group = '{1}' and uid = '{0}' ;Insert Into UserGroups(id_group,uid) values ('{0}','{1}')", groupid, uid));
			}
			catch (Exception)
			{
			}
		}

		public void DeleteGroupOfUser(string uid)
		{
			try
			{
				using SQLiteUtils sQLiteUtils = new SQLiteUtils();
				sQLiteUtils.ExecuteQuery($"Delete from UserGroups where uid = '{uid}'");
			}
			catch
			{
			}
		}

		public void Insert(GroupInfo g)
		{
			using SQLiteUtils sQLiteUtils = new SQLiteUtils();
			string commandText = string.Format("Delete from Groups where id='{0}' ; Insert Into Groups(id,name,type,memcount,status, note) values ('{0}','{1}','{2}','{3}',0,'')", g.Uid, g.Name.Replace("'", ""), g.IsApproved, g.Member);
			sQLiteUtils.ExecuteQuery(commandText);
		}

		private void AddColumn()
		{
			try
			{
				DataTable dataTable = sql.ExecuteQuery("Select LastUpdate from UserGroups limit 1");
				if (dataTable == null)
				{
					try
					{
						sql.ExecuteQuery("ALTER TABLE UserGroups ADD COLUMN LastUpdate int default 0");
					}
					catch
					{
					}
				}
			}
			catch
			{
				sql.ExecuteQuery("ALTER TABLE UserGroups ADD COLUMN LastUpdate int default 0");
			}
		}

		public List<string> GetAllGroupOfUser(string uid)
		{
			try
			{
				DataTable dataTable = sql.ExecuteQuery($"select b.* from UserGroups a inner join Groups b on b.id = a.id_Group and a.uid='{uid}' order by a.LastUpdate asc ");
				List<string> result = (from r in dataTable.AsEnumerable()
					select r.Field<string>("id")).ToList();
				dataTable.Dispose();
				return result;
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("SQL logic error or missing database"))
				{
					AddColumn();
				}
			}
			return new List<string>();
		}

		public List<string> GetAllGroup()
		{
			DataTable dataTable = sql.ExecuteQuery("select id from Groups");
			List<string> result = (from r in dataTable.AsEnumerable()
				select r.Field<string>("id")).ToList();
			dataTable.Dispose();
			return result;
		}

		public List<string> GetUnApprovalGroupOfUser(string uid, int memcount = 0)
		{
			new List<string>();
			DataTable dataTable = sql.ExecuteQuery($"select b.* from UserGroups a inner join Groups b on b.id = a.id_Group and a.uid='{uid}' and (b.Type = 'True' or b.memcount < {memcount})");
			List<string> result = (from r in dataTable.AsEnumerable()
				select r.Field<string>("id")).ToList();
			dataTable.Dispose();
			return result;
		}

		public void DeleteUserGroup(string uid, string groupid)
		{
			try
			{
				using SQLiteUtils sQLiteUtils = new SQLiteUtils();
				sQLiteUtils.ExecuteQuery($"Delete from UserGroups where id_group='{groupid}' and uid = '{uid}'");
			}
			catch
			{
			}
		}

		internal void UpdateLastComment(string uid, string groupId, long func)
		{
			try
			{
				using SQLiteUtils sQLiteUtils = new SQLiteUtils();
				sQLiteUtils.ExecuteQuery(string.Format("Update UserGroups set Lastupdate= '{2}' where id_group='{0}' and uid = '{1}'", groupId, uid, func));
			}
			catch
			{
			}
		}
	}
}
