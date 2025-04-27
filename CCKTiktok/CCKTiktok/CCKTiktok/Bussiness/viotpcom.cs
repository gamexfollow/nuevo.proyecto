using System.Net;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class viotpcom
	{
		private string Api = "";

		public viotpcom(string api = "")
		{
			Api = api;
		}

		private string GetData(string url)
		{
			try
			{
				return new WebClient().DownloadString(url);
			}
			catch
			{
				try
				{
					return new WebClient().DownloadString(url.Replace("https://", "http://"));
				}
				catch
				{
				}
			}
			return "";
		}

		public int Getbalance()
		{
			string data = GetData("https://api.viotp.com/users/balance?token=" + Api);
			if (data != null)
			{
				dynamic val = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				}.DeserializeObject(data);
				if (val != null && Utils.Convert2Int(val["status_code"]) == 200)
				{
					return Utils.Convert2Int(val["data"]["balance"]);
				}
			}
			return 0;
		}

		public CodeResult GetPhoneNumber(int serviceId = 7)
		{
			CodeResult codeResult = new CodeResult();
			string data = GetData($"https://api.viotp.com/request/getv2?token={Api}&serviceId={serviceId}");
			if (data != null)
			{
				dynamic val = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				}.DeserializeObject(data);
				if (val != null && Utils.Convert2Int(val["status_code"]) == 200)
				{
					codeResult.SessionId = val["data"]["request_id"].ToString();
					codeResult.PhoneOrEmail = val["data"]["phone_number"].ToString();
					codeResult.Success = true;
					return codeResult;
				}
			}
			return new CodeResult
			{
				Success = false
			};
		}

		public CodeResult GetCode(string requestId)
		{
			CodeResult codeResult = new CodeResult();
			string text = "";
			try
			{
				string data = GetData($"https://api.viotp.com/session/getv2?requestId={requestId}&token={Api}");
				if (data != null && !string.IsNullOrWhiteSpace(data))
				{
					dynamic val = new JavaScriptSerializer
					{
						MaxJsonLength = int.MaxValue
					}.DeserializeObject(data);
					if (val != null && Utils.Convert2Int(val["status_code"]) == 200 && val["data"] != null && val["data"].ContainsKey("Code") && val["data"]["Code"] != null)
					{
						text = (codeResult.Code = val["data"]["Code"].ToString());
						codeResult.Success = true;
					}
				}
			}
			catch
			{
			}
			return codeResult;
		}
	}
}
