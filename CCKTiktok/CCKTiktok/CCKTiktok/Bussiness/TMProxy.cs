using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class TMProxy
	{
		private string ApiKey = "";

		private string DeviceId = "";

		private static Dictionary<string, List<string>> DicDevice = new Dictionary<string, List<string>>();

		private static Dictionary<string, TMProxyResult> DicProxy = new Dictionary<string, TMProxyResult>();

		private static object lockDic = new object();

		public TMProxy(string api, string deviceId)
		{
			ApiKey = api;
			DeviceId = deviceId;
		}

		public bool CheckLive()
		{
			return true;
		}

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public void RemoveProxy()
		{
			if (DicDevice.ContainsKey(ApiKey) && DicDevice[ApiKey].Count > 1)
			{
				DicDevice[ApiKey].Remove(DeviceId);
				while (DicDevice.ContainsKey(ApiKey) && DicDevice[ApiKey].Count >= 1)
				{
					Thread.Sleep(1000);
				}
			}
			else
			{
				if (!DicDevice.ContainsKey(ApiKey))
				{
					return;
				}
				TMProxyResult tMProxyResult = DicProxy[ApiKey];
				try
				{
					string[] array = tMProxyResult.data.expired_at.Split(' ');
					DateTime dateTime = Convert.ToDateTime(array[1] + " " + array[0], new CultureInfo(1066)).AddMinutes(-30.0);
					while (DateTime.Now < dateTime.AddSeconds(tMProxyResult.data.next_request))
					{
						Thread.Sleep(5000);
					}
					Thread.Sleep(1500);
					DicDevice[ApiKey].Remove(DeviceId);
					DicDevice.Remove(ApiKey);
					DicProxy.Remove(ApiKey);
				}
				catch
				{
				}
			}
		}

		public TMProxyResult GetNewProxy(int locationId = 0)
		{
			TMProxyInfo tMProxyInfo = new TMProxyInfo();
			tMProxyInfo.api_key = ApiKey;
			tMProxyInfo.sign = "";
			tMProxyInfo.id_location = locationId;
			lock (lockDic)
			{
				if (!DicDevice.ContainsKey(ApiKey))
				{
					List<string> list = new List<string>();
					list.Add(DeviceId);
					DicDevice.Add(ApiKey, list);
				}
				else
				{
					DicDevice[ApiKey].Add(DeviceId);
				}
			}
			if (DicProxy.ContainsKey(ApiKey))
			{
				return DicProxy[ApiKey];
			}
			while (DicDevice.ContainsKey(ApiKey) && DicDevice[ApiKey].Count > 1)
			{
				Thread.Sleep(5000);
				if (!DicProxy.ContainsKey(ApiKey))
				{
					if (DicProxy.Count == 0)
					{
						break;
					}
					continue;
				}
				return DicProxy[ApiKey];
			}
			string text = new Utils().PostData("https://tmproxy.com/api/proxy/get-new-proxy", new JavaScriptSerializer().Serialize(tMProxyInfo));
			if (text == null)
			{
				DicProxy.Add(ApiKey, new TMProxyResult());
			}
			else
			{
				TMProxyResult tMProxyResult = new JavaScriptSerializer().Deserialize<TMProxyResult>(text);
				if (tMProxyResult.code != 6)
				{
					if (tMProxyResult.code == 5)
					{
						text = new Utils().PostData("https://tmproxy.com/api/proxy/get-current-proxy", new JavaScriptSerializer().Serialize(tMProxyInfo));
						if (text != null)
						{
							return new JavaScriptSerializer().Deserialize<TMProxyResult>(text);
						}
					}
					else if (tMProxyResult.code == 0)
					{
						if (!DicProxy.ContainsKey(ApiKey))
						{
							DicProxy.Add(ApiKey, tMProxyResult);
						}
						return tMProxyResult;
					}
				}
			}
			return DicProxy.ContainsKey(ApiKey) ? DicProxy[ApiKey] : new TMProxyResult();
		}
	}
}
