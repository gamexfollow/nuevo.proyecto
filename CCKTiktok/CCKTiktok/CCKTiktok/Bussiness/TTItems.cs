using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;

namespace CCKTiktok.Bussiness
{
	public class TTItems
	{
		public CookieContainer container = new CookieContainer();

		public TDSEntity TdsItem { get; set; }

		public bool LoggedIn { get; set; }

		public List<string> MyGroup { get; set; }

		public ProxyInfo MyProxy { get; set; }

		public string LastIp { get; set; }

		public string UserAgent { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string PassEmail { get; set; }

		public string Phone { get; set; }

		public string TwoFA { get; set; }

		public string Pass { get; set; }

		public string Uid { get; set; }

		public string Cookie { get; set; }

		public string TrangThai { get; set; }

		public string DateOfBirth { get; set; }

		public string CreateDate { get; set; }

		public string MyTempId { get; set; }

		public int FolderId { get; set; }

		public string Token { get; set; }

		public bool Avatar { get; set; }

		public string UidLayBai { get; set; }

		public string Brand { get; set; }

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public TTItems(bool initName = false)
		{
			TTItems tTItems = CreateName();
			FirstName = "";
			LastName = "";
			Pass = tTItems.Pass;
			TwoFA = "";
			Phone = "";
			Cookie = "";
			Uid = "";
			UserAgent = "";
			MyProxy = new ProxyInfo();
			MyTempId = "Temp_" + Guid.NewGuid().ToString("N");
			FolderId = 1;
			DateOfBirth = "";
			Email = ((tTItems.Email != null) ? tTItems.Email : "");
			LastIp = "";
			CreateDate = "";
			TrangThai = "Live";
			Token = "";
			MyGroup = new List<string>();
			PassEmail = "";
			UidLayBai = DateTime.Now.ToFileTime().ToString();
			Avatar = false;
			Brand = "";
			LoggedIn = false;
			TdsItem = new TDSEntity
			{
				token = ""
			};
		}

		private TTItems CreateName()
		{
			if (File.Exists("Config\\name.txt"))
			{
				string[] array = File.ReadAllLines("Config\\name.txt");
				if (array.Length >= 3)
				{
					dynamic val = array[0].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					dynamic val2 = array[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					dynamic val3 = array[2].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					LastName = val[new Random().Next(0, val.Length - 1)] + " " + val2[new Random().Next(0, val2.Length - 1)];
					FirstName = val3[new Random().Next(0, val3.Length - 1)];
					Email = Utils.UnicodeToKoDauAndGach(LastName + FirstName) + new Random().Next(1000, 9999) + "@battrangonline.vn";
					Pass = ((char)Convert.ToInt16(new Random().Next(65, 90))).ToString().ToUpper() + Guid.NewGuid().ToString("N").Substring(0, 7);
					Pass = "Bato@2019";
					PassEmail = "";
				}
			}
			return this;
		}

		public string NinjaString()
		{
			return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", Uid, Pass, "", Cookie, TwoFA, MyProxy.ToString());
		}

		public TTItems MappingItem(DataRow row, string domain = "")
		{
			return new TTItems
			{
				Uid = row["id"].ToString(),
				Pass = row["password"].ToString(),
				TwoFA = row["privatekey"].ToString(),
				FirstName = row["name"].ToString(),
				DateOfBirth = row["birthday"].ToString(),
				Cookie = row["cookies"].ToString(),
				Token = row["token"].ToString(),
				MyTempId = row["id"].ToString(),
				FolderId = 1,
				Email = ((row["email"] == null || !(row["email"].ToString() != "")) ? "" : row["email"].ToString()),
				PassEmail = ((row["passemail"] != null) ? row["passemail"].ToString() : ""),
				UserAgent = ((row["useragent"] != null) ? row["useragent"].ToString() : ""),
				UidLayBai = ((row["uidlaybai"] != null) ? row["uidlaybai"].ToString() : ""),
				Brand = ((row["brand"] != null) ? row["brand"].ToString() : ""),
				MyProxy = new ProxyInfo(row["proxy"].ToString()),
				TdsItem = new TDSEntity
				{
					token = ""
				}
			};
		}
	}
}
