using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class XProxy
	{
		private string xproxyServer = "";

		private Xproxy_API api = Xproxy_API.Old;

		public XProxy(string ip, int port)
		{
			if (File.Exists(CaChuaConstant.XPROXY_LAN_API))
			{
				api = new JavaScriptSerializer().Deserialize<Xproxy_API>(Utils.ReadTextFile(CaChuaConstant.XPROXY_LAN_API));
			}
			xproxyServer = $"http://{ip}:{port}";
		}

		public void ResetAll()
		{
			new WebClient().DownloadString(xproxyServer + "/reset_all");
		}

		public void ResetOne(ProxyInfo p)
		{
			try
			{
				if (api != 0)
				{
					new WebClient().DownloadString(xproxyServer + $"/api/v1/rotate_ip/proxy/{p.Ip}:{p.Port}");
				}
				else
				{
					new WebClient().DownloadString(xproxyServer + $"/reset?proxy={p.Ip}:{p.Port}");
				}
			}
			catch
			{
			}
		}

		public ProxyInfo GetOne(string ip, int port)
		{
			List<ProxyInfo> all = GetAll();
			foreach (ProxyInfo item in all)
			{
				if (item.Ip == ip && item.Port == port)
				{
					return item;
				}
			}
			return null;
		}

		public List<ProxyInfo> GetAll()
		{
			List<ProxyInfo> list = new List<ProxyInfo>();
			string text = new WebClient().DownloadString(xproxyServer + "/proxy_list");
			if (text != null)
			{
				dynamic val = new JavaScriptSerializer().DeserializeObject(text);
				if (val != null && val.Length > 0)
				{
					for (int i = 0; i < val.Length; i++)
					{
						if (!(val[i].ContainsKey("public_ip") ? true : false))
						{
							continue;
						}
						dynamic val2 = val[i]["public_ip"];
						if (val2 != null)
						{
							ProxyInfo proxyInfo = new ProxyInfo();
							proxyInfo.Ip = val[i]["system"];
							proxyInfo.Port = val[i]["proxy_port"];
							proxyInfo.SockPort = val[i]["sock_port"];
							proxyInfo.PublicIp = val[i]["public_ip"];
							ProxyInfo proxyInfo2 = proxyInfo;
							Regex regex = new Regex("([0-9]+).([0-9]+).([0-9]+).([0-9]+)");
							if (regex.Match(proxyInfo2.PublicIp).Success)
							{
								list.Add(proxyInfo2);
							}
						}
					}
				}
			}
			return list;
		}
	}
}
