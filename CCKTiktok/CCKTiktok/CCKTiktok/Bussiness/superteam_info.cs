using System.Threading;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	internal class superteam_info
	{
		public class EmailVerificationResult
		{
			public string Gmail { get; set; }

			public string Status { get; set; }

			public string Otp { get; set; }

			public bool Success { get; set; }
		}

		public string API { get; set; }

		public superteam_info(string api = "0WHJL5DGCFA4BMG5")
		{
			API = api;
		}

		public string GetEmail()
		{
			string url = GetUrl($"http://api.superteam.info/api/otp-services/gmail-otp-rental?apiKey={API}&otpServiceCode=tiktok");
			EmailVerificationResult emailVerificationResult = new JavaScriptSerializer
			{
				MaxJsonLength = int.MaxValue
			}.Deserialize<EmailVerificationResult>(url);
			if (emailVerificationResult != null && emailVerificationResult.Gmail != "")
			{
				return emailVerificationResult.Gmail.ToString() + "|cck";
			}
			return "";
		}

		public string GetCode(string mail)
		{
			string text = "";
			int num = 0;
			text = GetUrl($"http://api.superteam.info/api/otp-services/gmail-otp-lookup?apiKey={API}&otpServiceCode=tiktok&gmail={mail}");
			if (text != "")
			{
				EmailVerificationResult emailVerificationResult = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				}.Deserialize<EmailVerificationResult>(text);
				if (emailVerificationResult != null && !string.IsNullOrEmpty(emailVerificationResult.Otp))
				{
					if (emailVerificationResult.Otp != null && Utils.Convert2Int(emailVerificationResult.Otp) > 0)
					{
						return emailVerificationResult.Otp;
					}
					text = "";
				}
				else
				{
					text = "";
				}
				Thread.Sleep(5000);
				num++;
			}
			return text;
		}

		private string GetUrl(string url)
		{
			try
			{
				return Utils.GetResponse(url);
			}
			catch
			{
			}
			return "";
		}
	}
}
