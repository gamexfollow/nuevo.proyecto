using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Entity;
using CCKTiktok.Helper;

namespace CCKTiktok.Bussiness
{
	public class FaceBookHelper
	{
		public static List<string> dicGroup = new List<string>();

		public static bool IsApprovedGroup(string deviceId, long uid, string cookies)
		{
			CCKApi cckApi = new CCKApi
			{
				Cookies = cookies,
				Method = "GET",
				FileName = $"{uid}.txt",
				Url = "https://m.facebook.com/groups/" + uid + "/madminpanel"
			};
			return GetDataByApiPhone(deviceId, uid.ToString(), cckApi).Contains("madminpanel/pending/");
		}

		public static List<string> GetAllGroupBasicInfo(string deviceId, string uid, string cookies)
		{
			CCKApi cckApi = new CCKApi
			{
				Cookies = cookies,
				Method = "GET",
				FileName = $"g_{uid}.txt",
				Url = "https://mbasic.facebook.com/groups/?seemore&refid=27"
			};
			string dataByApiPhone = GetDataByApiPhone(deviceId, uid, cckApi);
			Regex regex = new Regex("/groups/([^/]+)/\\?refid=27\">([^/]+)</a>");
			MatchCollection matchCollection = regex.Matches(dataByApiPhone);
			List<string> list = new List<string>();
			foreach (Match item in matchCollection)
			{
				if (item.Success)
				{
					list.Add(item.Groups[1].Value + "|" + item.Groups[2].Value);
				}
			}
			return list;
		}

		public static List<string> GetAllGroupByKeywords(string deviceId, string kewword, string cookies)
		{
			string text = $"https://mbasic.facebook.com/search/groups/?q={HttpUtility.UrlEncode(kewword)}&source=filter&isTrending=0";
			List<string> list = new List<string>();
			while (text != "")
			{
				CCKApi cckApi = new CCKApi
				{
					Cookies = cookies,
					Method = "GET",
					FileName = $"g_{deviceId}.txt",
					Url = text
				};
				string dataByApiPhone = GetDataByApiPhone(deviceId, deviceId, cckApi);
				Regex regex = new Regex("https://mbasic.facebook.com/groups/([^/]+)/\\?refid=46&amp;([^/]+)\"><([^/]+)><([^/]+)>([^/]+)</([^/]+)></([^/]+)>");
				MatchCollection matchCollection = regex.Matches(dataByApiPhone);
				bool flag = false;
				foreach (Match item2 in matchCollection)
				{
					if (item2.Success && item2.Groups.Count > 5)
					{
						string item = item2.Groups[1].Value.ToString();
						if (!list.Contains(item))
						{
							list.Add(item);
							flag = true;
						}
					}
				}
				if (!flag)
				{
					break;
				}
				if (dataByApiPhone != null && dataByApiPhone.Contains("see_more_pager"))
				{
					Regex regex2 = new Regex("https://mbasic.facebook.com/search/groups/\\?q=(.*?)refid=46");
					Match match2 = regex2.Match(dataByApiPhone);
					text = (match2.Success ? match2.Groups[0].Value : "");
				}
				else
				{
					text = "";
				}
			}
			return list;
		}

		public static string MbasicGetGroupInbox(string deviceId, TTItems item)
		{
			CCKApi cckApi = new CCKApi
			{
				Cookies = item.Cookie,
				Method = "GET",
				FileName = $"grpup_{item.Uid}.txt",
				Url = $"https://mbasic.facebook.com/messages/?ref_component=mbasic_home_header&ref_page=MMessagingThreadlistController&refid=11"
			};
			return GetDataByApiPhone(deviceId, item.Uid, cckApi);
		}

		public static GroupActivity MbasicGetGroupWall(string deviceId, string cookies, string groupId)
		{
			new GroupActivity();
			CCKApi cckApi = new CCKApi
			{
				Cookies = cookies,
				Method = "GET",
				FileName = $"grpup_{groupId}.txt",
				Url = string.Format("https://mbasic.facebook.com/groups/" + groupId)
			};
			string dataByApiPhone = GetDataByApiPhone(deviceId, groupId, cckApi);
			int num = 0;
			int num2 = 0;
			try
			{
				Regex regex = new Regex(">([0-9]+) (Bình luận|bình luận|Comments|comments|Comment)<");
				dataByApiPhone = dataByApiPhone.Replace(".", "").Replace(",", "");
				MatchCollection matchCollection = regex.Matches(dataByApiPhone);
				foreach (Match item in matchCollection)
				{
					if (item.Success)
					{
						num += Utils.Convert2Int(item.Groups[1].Value);
					}
				}
				regex = new Regex("</span>([0-9]+)</a><span aria-hidden=\"true\"> · </span>");
				matchCollection = regex.Matches(dataByApiPhone);
				foreach (Match item2 in matchCollection)
				{
					if (item2.Success)
					{
						num2 += Utils.Convert2Int(item2.Groups[1].Value);
					}
				}
			}
			catch
			{
			}
			return new GroupActivity
			{
				GroupId = groupId,
				Commemt = num,
				Like = num2
			};
		}

		public static string GetDataByApiPhone(string deviceId, string uid, CCKApi cckApi)
		{
			try
			{
				ADBHelperCCK.ExecuteCMD(deviceId, "shell rm -r /sdcard/cck_api.txt");
				ADBHelperCCK.ExecuteCMD(deviceId, $"shell rm -r /sdcard/{cckApi.FileName}");
				string text = Application.StartupPath + $"\\Uid\\api_{uid}.txt";
				File.WriteAllText(text, new JavaScriptSerializer().Serialize(cckApi));
				Thread.Sleep(1000);
				if (ADBHelperCCK.PushInfoFile(deviceId, $"\"{text}\" \"/sdcard/cck_api.txt\""))
				{
					Thread.Sleep(1000);
					ADBHelperCCK.ExecuteCMD(deviceId, "shell su -c am startservice com.cck.support/.CckService");
					Thread.Sleep(2000);
					for (int i = 0; i < 10; i++)
					{
						if (i == 5)
						{
							ADBHelperCCK.ExecuteCMD(deviceId, "shell su -c am startservice com.cck.support/.CckService");
							Thread.Sleep(2000);
						}
						string text2 = ADBHelperCCK.ExecuteCMD(deviceId, string.Format(" pull \"/sdcard/{1}\" \"" + Application.StartupPath + "\\Uid\\q{0}.txt\"", uid, cckApi.FileName));
						if (!text2.Contains("file pulled"))
						{
							Thread.Sleep(1000);
							continue;
						}
						return Utils.ReadTextFile(text.Replace("api_", "q"));
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("GetDataByApiPhone: " + uid, ex.Message);
			}
			return "";
		}

		public static string GetIPFromDevice(string deviceId, string proxy)
		{
			try
			{
				if (proxy == "")
				{
					return ADBHelperCCK.ExecuteCMD(deviceId, "shell curl -s https://domains.google.com/checkip");
				}
				string[] array = proxy.Split(":".ToCharArray());
				if (array.Length == 2)
				{
					return ADBHelperCCK.ExecuteCMD(deviceId, "shell curl -x " + proxy + " -s https://domains.google.com/checkip");
				}
				ADBHelperCCK.ExecuteCMD(deviceId, "shell curl -x " + array[0] + ":" + array[1] + " -U " + array[2] + ":" + array[3] + " -s https://domains.google.com/checkip");
			}
			catch
			{
			}
			return "";
		}

		public static string GetClientIP(string deviceId, string uid)
		{
			CCKApi cckApi = new CCKApi
			{
				Cookies = "",
				Method = "GET",
				FileName = $"{uid}.txt",
				Url = "https://api.myip.com/"
			};
			return GetDataByApiPhone(deviceId, uid.ToString(), cckApi);
		}
	}
}
