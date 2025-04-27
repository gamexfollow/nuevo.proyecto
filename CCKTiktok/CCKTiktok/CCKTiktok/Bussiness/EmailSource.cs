using System;
using System.Data;
using CCKTiktok.DAL;

namespace CCKTiktok.Bussiness
{
	public class EmailSource
	{
		public string gmail { get; set; }

		public string pass { get; set; }

		public string domain { get; set; }

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public EmailSource()
		{
			gmail = "";
			domain = "";
			pass = "";
		}

		public void CreateDefaultDB()
		{
			DataTable dataTable = new SQLiteUtils().ExecuteQuery("SELECT name FROM sqlite_master WHERE name='EmailSystem' and type='table' ORDER BY name");
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				new SQLiteUtils().ExecuteQuery("CREATE TABLE 'EmailSystem' ('email' TEXT,'pass'\tTEXT,'domain' TEXT);");
			}
		}

		public void Insert()
		{
			new SQLiteUtils().ExecuteQuery($"Insert Into EmailSystem (email, pass, domain) values ('{gmail}','{pass}','{domain}')");
		}

		public void Delete(string domain)
		{
			new SQLiteUtils().ExecuteQuery($"Delete from EmailSystem where domain='{domain}'");
		}

		public DataTable GetAll()
		{
			try
			{
				return new SQLiteUtils().ExecuteQuery($"SELECT * from EmailSystem");
			}
			catch
			{
				CreateDefaultDB();
				return new DataTable();
			}
		}

		public EmailSource GetByEmail(string email)
		{
			DataTable dataTable = new SQLiteUtils().ExecuteQuery(string.Format("SELECT * from EmailSystem where domain = '{0}'", email.Replace("@", "")));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new EmailSource
				{
					gmail = dataTable.Rows[0]["email"].ToString(),
					domain = dataTable.Rows[0]["domain"].ToString(),
					pass = dataTable.Rows[0]["pass"].ToString()
				};
			}
			return new EmailSource();
		}
	}
}
