using System.Data;
using CCKTiktok.DAL;

namespace CCKTiktok.Bussiness
{
	public class TDSConfigUtils
	{
		private SQLiteUtils sql = new SQLiteUtils();

		public string username { get; set; }

		public string password { get; set; }

		public string token { get; set; }

		public string device_id { get; set; }

		public TDSConfigUtils()
		{
			username = "";
			password = "";
			token = "";
			device_id = "";
			sql = new SQLiteUtils();
		}

		public void Insert()
		{
			sql.ExecuteQuery($"Insert Into TDSConfig (username,password,token,device_id) values ('{username}','{password}','{token}','{device_id}')");
		}

		public void Update()
		{
			sql.ExecuteQuery(string.Format("Update TDSConfig set password='{1}',token='{2}',device_id='{3}' where username='{0}'", username, password, token, device_id));
		}

		public DataTable GetAll()
		{
			return sql.ExecuteQuery("Select * from TDSConfig");
		}

		public void CreateAccount()
		{
		}
	}
}
