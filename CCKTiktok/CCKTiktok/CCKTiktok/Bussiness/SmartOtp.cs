using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class SmartOtp
	{
		private class JsonData
		{
			public string id { get; set; }

			public string phoneNumber { get; set; }

			public string provider { get; set; }

			public string serviceName { get; set; }

			public string content { get; set; }

			public string status { get; set; }

			public string otp { get; set; }

			public DateTime createdAt { get; set; }
		}

		private string Api;

		public SmartOtp(string api)
		{
			Api = api;
		}

		public CodeResult GetPhoneNumber(string service)
		{
			CodeResult codeResult = new CodeResult();
			try
			{
				string address = "http://api.smartotp.net:3001/api/" + service + "/" + Api;
				string text = new WebClient().DownloadString(address);
				if (text != null)
				{
					dynamic val = new JavaScriptSerializer
					{
						MaxJsonLength = int.MaxValue
					}.DeserializeObject(text);
					if (val != null && val.ContainsKey("phoneNumber") && val["phoneNumber"].ToString() != "")
					{
						string text2 = val["phoneNumber"].ToString();
						if (text2.StartsWith("+84"))
						{
							text2 = "0" + text2.Substring(3);
						}
						dynamic val2 = val["id"].ToString();
						codeResult.SessionId = val2;
						codeResult.PhoneOrEmail = text2;
						codeResult.Message = "";
						codeResult.Success = true;
						return codeResult;
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("GetPhoneNumber", ex.Message);
			}
			return new CodeResult
			{
				Success = false
			};
		}

		public CodeResult GetCode(string id_order)
		{
			try
			{
				CodeResult codeResult = new CodeResult();
				string address = "http://api.smartotp.net:3001/api/order/" + id_order + "/" + Api;
				string text = new WebClient().DownloadString(address);
				if (text != null)
				{
					JsonData jsonData = new JavaScriptSerializer
					{
						MaxJsonLength = int.MaxValue
					}.Deserialize<JsonData>(text);
					if (jsonData != null && jsonData.status.Equals("Successed"))
					{
						Regex regex = new Regex("([0-9]+) ");
						Match match = regex.Match(jsonData.content);
						if (match.Success)
						{
							string text3 = (codeResult.Code = match.Groups[1].Value.ToLower());
							codeResult.Success = true;
							return codeResult;
						}
					}
					if (jsonData.status.Equals("Failed"))
					{
						string text4 = (codeResult.Message = jsonData.status);
						codeResult.Success = false;
						return codeResult;
					}
				}
			}
			catch
			{
			}
			return new CodeResult
			{
				Success = false
			};
		}
	}
}
