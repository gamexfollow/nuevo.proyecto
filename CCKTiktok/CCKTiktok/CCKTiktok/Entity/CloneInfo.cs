using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Entity
{
	public class CloneInfo
	{
		private string Uid = "";

		private string UidLayBai = "";

		public bool IsChangecover { get; set; }

		public bool IsChangeAvatar { get; set; }

		public bool IsCopyName { get; set; }

		public bool IsCopyBirthday { get; set; }

		public bool IsCopyGender { get; set; }

		public int NumOfPost { get; set; }

		public List<string> ListPostId { get; set; }

		public bool PreventOwner { get; set; }

		public CloneInfo()
		{
			PreventOwner = false;
			IsChangeAvatar = false;
			IsChangecover = false;
			IsCopyBirthday = false;
			IsCopyGender = false;
			IsCopyName = false;
			ListPostId = new List<string>();
			NumOfPost = 0;
			Uid = "";
		}

		public CloneInfo(string uid)
		{
			PreventOwner = false;
			IsChangeAvatar = false;
			IsChangecover = false;
			IsCopyBirthday = false;
			IsCopyGender = false;
			IsCopyName = false;
			ListPostId = new List<string>();
			NumOfPost = 0;
			Uid = uid;
		}

		public CloneInfo(TTItems item)
		{
			PreventOwner = false;
			IsChangeAvatar = false;
			IsChangecover = false;
			IsCopyBirthday = false;
			IsCopyGender = false;
			IsCopyName = false;
			ListPostId = new List<string>();
			NumOfPost = 0;
			UidLayBai = item.UidLayBai;
			Uid = item.Uid;
		}

		public CloneInfo GetOne()
		{
			CloneInfo result = new CloneInfo(Uid);
			if (File.Exists("CloneInfo\\" + Uid + ".json"))
			{
				result = new JavaScriptSerializer().Deserialize<CloneInfo>(Utils.ReadTextFile("CloneInfo\\" + Uid + ".json"));
			}
			return result;
		}

		public void WriteToFile()
		{
			if (!Directory.Exists("CloneInfo"))
			{
				Directory.CreateDirectory("CloneInfo");
			}
			File.WriteAllText("CloneInfo\\" + Uid + ".json", new JavaScriptSerializer().Serialize(this));
		}
	}
}
