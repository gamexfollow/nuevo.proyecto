using System.Web.Script.Serialization;
using CCKTiktok.Entity;

namespace CCKTiktok.Bussiness
{
	internal class PhoneUtils
	{
		public static CodeResult GetPhone(ServiceType services)
		{
			VerifyPhoneType verifyPhoneType = new JavaScriptSerializer().Deserialize<VerifyPhoneType>(Utils.ReadTextFile(CaChuaConstant.VERI_PHONE_TYPE));
			if (verifyPhoneType != VerifyPhoneType.SmartOtp)
			{
				if (verifyPhoneType == VerifyPhoneType.Viotp)
				{
					viotpcom viotpcom2 = new viotpcom(Utils.ReadTextFile(CaChuaConstant.VIOTP));
					switch (services)
					{
					case ServiceType.Microsoft:
						return viotpcom2.GetPhoneNumber(5);
					case ServiceType.Tiktok:
						return viotpcom2.GetPhoneNumber(29);
					}
				}
				return new CodeResult
				{
					Success = false
				};
			}
			SmartOtp smartOtp = new SmartOtp(Utils.ReadTextFile(CaChuaConstant.OTP));
			if (services == ServiceType.Tiktok)
			{
				return smartOtp.GetPhoneNumber("tiktok");
			}
			return smartOtp.GetPhoneNumber("microsoft");
		}

		public static CodeResult GetCode(string sessionId)
		{
			VerifyPhoneType verifyPhoneType = new JavaScriptSerializer().Deserialize<VerifyPhoneType>(Utils.ReadTextFile(CaChuaConstant.VERI_PHONE_TYPE));
			SmartOtp smartOtp = new SmartOtp(Utils.ReadTextFile(CaChuaConstant.OTP));
			switch (verifyPhoneType)
			{
			case VerifyPhoneType.SmartOtp:
				return smartOtp.GetCode(sessionId);
			case VerifyPhoneType.Viotp:
			{
				viotpcom viotpcom2 = new viotpcom(Utils.ReadTextFile(CaChuaConstant.VIOTP));
				return viotpcom2.GetCode(sessionId);
			}
			default:
				return new CodeResult
				{
					Success = false
				};
			}
		}
	}
}
