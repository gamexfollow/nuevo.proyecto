using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class TDS
	{
		public enum TaskType
		{
			tiktok_like = 1,
			tiktok_follow,
			tiktok_comment
		}

		public class TaskTypeData
		{
			public class TaskTypeSubData
			{
				public string id { get; set; }

				public string link { get; set; }

				public string type { get; set; }

				public TaskTypeSubData()
				{
					type = "";
					link = "";
					id = "";
				}
			}

			public int cache { get; set; }

			public List<TaskTypeSubData> data { get; set; }

			public TaskTypeData()
			{
				data = new List<TaskTypeSubData>();
			}
		}

		public class TDSResult
		{
			public int Success { get; set; }

			public Data Data { get; set; }

			public string Error { get; set; }

			public int Cache { get; set; }
		}

		public class Data
		{
			public int Xu { get; set; }

			public int JobSuccess { get; set; }

			public int XuThem { get; set; }

			public string Message { get; set; }
		}

		public class TaskTypeCommentData
		{
			public string noidung { get; set; }

			public string id { get; set; }

			public string link { get; set; }

			public string type { get; set; }

			public TaskTypeCommentData()
			{
				type = "";
				link = "";
				id = "";
				noidung = "";
			}
		}

		public const string TASKTYPE_TIKTOK_LIKE = "tiktok_like";

		public const string TASKTYPE_TIKTOK_FOLLOW = "tiktok_follow";

		public const string TASKTYPE_TIKTOK_COMMENT = "tiktok_comment";

		public const string TIKTOK_LIKE_CACHE = "TIKTOK_LIKE_CACHE";

		public const string TIKTOK_FOLLOW_CACHE = "TIKTOK_FOLLOW_CACHE";

		private string Token { get; set; }

		private string Proxy { get; set; }

		public TDS(TDSEntity entity)
		{
			Token = entity.token;
			Proxy = entity.proxy;
		}

		public bool AccAccount(string uid, out string outMessage)
		{
			outMessage = "";
			string response = Utils.GetResponse($"https://traodoisub.com/api/?fields=tiktok_add&id={uid.TrimStart('@')}&access_token={Token}", Proxy);
			if (!(response != ""))
			{
				return false;
			}
			dynamic val = new JavaScriptSerializer().DeserializeObject(response);
			if (val.Count > 0 && val.ContainsKey("error"))
			{
				dynamic val2 = val["error"];
				if (!(val2.ToString().Contains("ID này đã được thêm vào tài khoản này rồi") ? true : false))
				{
					outMessage = val2;
					return false;
				}
				return true;
			}
			return true;
		}

		public bool Run(string uid)
		{
			string response = Utils.GetResponse($"https://cck.vn/Api/tds/api.ashx?fields=tiktok_run&id={uid}&access_token={Token}", Proxy);
			if (response != "")
			{
				dynamic val = new JavaScriptSerializer().DeserializeObject(response);
				if (val.Count > 0 && val.ContainsKey("success"))
				{
					dynamic val2 = val["success"];
					if (val2.ToString().Contains("200"))
					{
						return true;
					}
					return false;
				}
			}
			return true;
		}

		public TaskTypeData GetTask(string tasktype)
		{
			string response = Utils.GetResponse($"https://cck.vn/Api/tds/api.ashx?fields={tasktype}&access_token={Token}", Proxy);
			if (!Directory.Exists("TDS"))
			{
				Directory.CreateDirectory("TDS");
			}
			File.AppendAllLines("TDS\\" + tasktype + "_" + Token.Substring(Token.Length - 10) + "_" + DateTime.Now.ToString("ddMMyyyy") + ".txt", new List<string> { response });
			return new JavaScriptSerializer().Deserialize<TaskTypeData>(response);
		}

		public int GetXu()
		{
			try
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				string url = "https://traodoisub.com/api/?fields=profile&access_token=" + Token;
				string response = Utils.GetResponse(url, Proxy);
				dynamic val = javaScriptSerializer.DeserializeObject(response);
				return Utils.Convert2Int(val["data"]["xu"].ToString());
			}
			catch (Exception)
			{
			}
			return 0;
		}

		public int UpdateTask(string id_job, string tasktype)
		{
			string response = Utils.GetResponse($"https://cck.vn/Api/tds/api_coin.ashx?type={tasktype}&id={id_job}&access_token={Token}", Proxy);
			dynamic val = new JavaScriptSerializer().DeserializeObject(response.ToString().ToLower());
			if (!((val != null && val.ContainsKey("cache")) ? true : false))
			{
				return 0;
			}
			return Utils.Convert2Int(val["cache"]);
		}

		public TDSResult GetXu(string id_job, string tasktype)
		{
			string response = Utils.GetResponse($"https://cck.vn/Api/tds/api_coin.ashx?type={tasktype}&id={id_job}&access_token={Token}", Proxy);
			return (response != "") ? new JavaScriptSerializer().Deserialize<TDSResult>(response.ToString()) : new TDSResult();
		}

		internal List<TaskTypeCommentData> GetTaskComment()
		{
			string text = "";
			using WebClient webClient = new WebClient();
			webClient.Encoding = Encoding.UTF8;
			text = Utils.GetResponse(string.Format("https://cck.vn/Api/tds/api.ashx?fields={0}&access_token={1}", "tiktok_comment", Token), Proxy);
			return new JavaScriptSerializer().Deserialize<List<TaskTypeCommentData>>(text);
		}
	}
}
