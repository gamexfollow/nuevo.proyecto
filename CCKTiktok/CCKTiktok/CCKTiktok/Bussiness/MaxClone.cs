using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using CCKTiktok.Entity;

namespace CCKTiktok.Bussiness
{
	public class MaxClone
	{
		private string key = "";

		public MaxClone(string key)
		{
			this.key = key;
		}

		public void CheckAvail()
		{
			new WebClient().DownloadString("ttp://api.maxclone.vn/api/global/stocknow");
		}

		public List<MaxCloneEntity.Datas.EmailInfo> GetEmail(int num)
		{
			try
			{
				string arg = "HOTMAIL";
				if (File.Exists(CaChuaConstant.HOTMAILBOX_TYPE))
				{
					arg = Utils.ReadTextFile(CaChuaConstant.HOTMAILBOX_TYPE);
				}
				string input = new WebClient().DownloadString(string.Format("https://api.hotmailbox.me/mail/buy?apikey={0}&mailcode={2}&quantity={1}", key, num, arg));
				MaxCloneEntity maxCloneEntity = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				}.Deserialize<MaxCloneEntity>(input);
				if (maxCloneEntity.Data != null && maxCloneEntity.Data.Emails != null)
				{
					return maxCloneEntity.Data.Emails;
				}
			}
			catch
			{
			}
			return null;
		}

		public MaxCloneEntity.Datas.EmailInfo GetEmail()
		{
			List<MaxCloneEntity.Datas.EmailInfo> email = GetEmail(1);
			if (email != null && email.Count > 0)
			{
				return email[0];
			}
			return null;
		}
	}
}
