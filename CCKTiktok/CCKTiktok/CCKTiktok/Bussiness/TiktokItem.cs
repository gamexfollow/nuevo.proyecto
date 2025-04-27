using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using CCKTiktok.BO;
using CCKTiktok.Component;
using CCKTiktok.DAL;
using CCKTiktok.Entity;
using CCKTiktok.Helper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace CCKTiktok.Bussiness
{
	public class TiktokItem
	{
		private RemoteWebDriver m_driver;

		private DeviceEntity deviceEntity;

		private CCKDriver cckDriver = null;

		public TTItems Info = new TTItems();

		public string SessionString = "";

		private Point screen = default(Point);

		private DataGridView dataGridViewPhone;

		private DataGridViewRow CurrentRow;

		public List<string> JobResult = new List<string>();

		private double minTime = -2147483648.0;

		private static Dictionary<string, string> dicFunctionKey = new Dictionary<string, string>();

		private static SQLiteUtils sql = new SQLiteUtils();

		private EmailUtils emailUti = new EmailUtils();

		private bool unlockbyhand = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.UNLOCKCAPTCHA_BYHAND));

		private Random rnd = new Random();

		private static JavaScriptSerializer js = new JavaScriptSerializer();

		private Point likePoint = default(Point);

		public string FaceBookID { get; set; }

		public RangeValue RandomTime { get; set; }

		public RangeValue RandomQuantity { get; set; }

		private string p_DeviceId { get; set; }

		private string UID { get; set; }

		public bool CloseCaptcha { get; set; }

		public bool RunNextAccount { get; set; }

		public TiktokItem(RemoteWebDriver driverApp, DeviceEntity entity, string uid, DataGridView dataGridViewPhone, string session, DataGridViewRow row)
		{
			RunNextAccount = true;
			CloseCaptcha = true;
			CurrentRow = row;
			screen = ADBHelperCCK.GetScreenResolution(entity.DeviceId);
			UID = uid;
			deviceEntity = entity;
			Info = new TTItems
			{
				Uid = uid
			};
			m_driver = driverApp;
			this.dataGridViewPhone = dataGridViewPhone;
			p_DeviceId = m_driver.Capabilities.GetCapability("deviceName").ToString();
			RandomTime = new RangeValue();
			RandomQuantity = new RangeValue
			{
				From = 1,
				To = 1
			};
			cckDriver = new CCKDriver(p_DeviceId);
			DataRow accountById = sql.GetAccountById(uid);
			SessionString = session;
			if (accountById != null)
			{
				Info.Uid = accountById["id"].ToString();
				Info.Pass = accountById["password"].ToString();
				if (Info.Pass == "")
				{
					Info.Pass = Info.Uid;
				}
				Info.Email = accountById["email"].ToString();
				Info.PassEmail = accountById["passemail"].ToString();
				Info.TwoFA = accountById["privatekey"].ToString();
				Info.TdsItem = GetTokenByDevice(p_DeviceId);
			}
		}

		private void LoadingWait(int maxDelay = 10, string text = "tab_animation_icon")
		{
			Thread.Sleep(1000);
			string pageSource = GetPageSource();
			DateTime dateTime = DateTime.Now.AddSeconds(maxDelay);
			while (pageSource.Contains(text) && DateTime.Now < dateTime)
			{
				Thread.Sleep(2000);
				pageSource = GetPageSource();
			}
		}

		public bool SetProxy(string deviceId, string proxy)
		{
			if (!(Utils.ReadTextFile(CaChuaConstant.PROXY_APP) == "clientnoroot"))
			{
				bool result = false;
				try
				{
					List<string> list = proxy.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
					if (list.Count == 1)
					{
						proxy = list[0];
					}
					else if (list.Count == 2)
					{
						proxy = list[0];
						Utils.GetResponse(list[1]);
						Thread.Sleep(10000);
					}
					string empty = string.Empty;
					string[] array = proxy.Split(':');
					string empty2 = string.Empty;
					string empty3 = string.Empty;
					empty = " shell settings put global http_proxy :0";
					ADBHelperCCK.ExecuteCMD(deviceId, empty);
					if (array.Length == 4 && Utils.ReadTextFile(CaChuaConstant.PROXY_APP) == "ip_port")
					{
						int portForward = 10000 + Utils.Convert2Int(array[1]);
						ADBHelperCCK.ForwardProxy(proxy, portForward);
						string localIp = ADBHelperCCK.GetLocalIp();
						array = new string[2]
						{
							localIp,
							portForward.ToString()
						};
					}
					if (array.Length == 2)
					{
						empty2 = array[0].Trim();
						empty3 = array[1].Trim();
						empty = string.Concat(new string[4] { "shell settings put global http_proxy ", empty2, ":", empty3 });
						ADBHelperCCK.ExecuteCMD(deviceId, empty);
						return true;
					}
					if (array.Length > 2)
					{
						string text = array[0].Trim();
						string text2 = array[1].Trim();
						string text3 = ((array.Length == 4) ? array[2].Trim() : "");
						string text4 = ((array.Length == 4) ? array[3].Trim() : "");
						if (!ADBHelperCCK.IsInstallApp(deviceId, "com.cell47.College_Proxy"))
						{
							string text5 = "College_Proxy.apk";
							if (!File.Exists(Application.StartupPath + "\\Devices\\" + text5))
							{
								try
								{
									new WebClient().DownloadFile($"http://cachuake.com/Download/Utils/{text5}.rar", Application.StartupPath + "\\Devices\\" + text5);
								}
								catch (Exception ex)
								{
									Utils.CCKLog(CaChuaConstant.LOG_ACTION, ex.Message, "SetupPhone");
								}
							}
							ADBHelperCCK.InstallApp(deviceId, Application.StartupPath + "\\Devices\\" + text5);
							Thread.Sleep(2000);
						}
						List<IWebElement> list2;
						while (true)
						{
							ADBHelperCCK.ExecuteCMD(deviceId, $"shell pm clear com.cell47.College_Proxy");
							ADBHelperCCK.BackToHome(deviceId, aOpenApp: false);
							ADBHelperCCK.OpenApp(deviceId, "com.cell47.College_Proxy");
							Thread.Sleep(2000);
							ADBHelperCCK.WaitMe("//android.widget.EditText", m_driver);
							string pageSource = GetPageSource();
							if (pageSource != "")
							{
								ADBHelperCCK.ShowTextMessageOnPhone(deviceId, "Proxy - " + text);
								Thread.Sleep(500);
								pageSource = GetPageSource();
								list2 = ADBHelperCCK.WaitMes(By.XPath("//android.widget.EditText"), m_driver);
								if (list2 != null && list2.Count >= 4)
								{
									Regex regex = new Regex("([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+)");
									if (!regex.Match(text).Success)
									{
										string input = ADBHelperCCK.RunCommand("ping " + text);
										Match match = regex.Match(input);
										if (match.Success)
										{
											text = match.Groups[1].Value + "." + match.Groups[2].Value + "." + match.Groups[3].Value + "." + match.Groups[4].Value;
										}
									}
									list2[0].SendKeys(text);
									Thread.Sleep(500);
									if (list2 != null && list2.Count >= 4)
									{
										break;
									}
								}
								else
								{
									Utils.CCKLog("SetProxy", pageSource);
								}
								continue;
							}
							return SetProxy(deviceId, proxy);
						}
						list2[1].SendKeys(text2);
						Thread.Sleep(500);
						if (text3 != "")
						{
							if (list2 != null && list2.Count >= 4)
							{
								list2[2].SendKeys(text3);
								Thread.Sleep(500);
							}
							if (list2 != null && list2.Count >= 4)
							{
								list2[3].SendKeys(text4);
								Thread.Sleep(500);
							}
						}
						string pageSource2 = GetPageSource();
						ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.cell47.College_Proxy:id/proxy_start_button\"]", m_driver)?.Click();
						int num = 0;
						pageSource2 = GetPageSource();
						while (!pageSource2.Contains("text=\"Connected\"") && num++ < 5)
						{
							if (pageSource2.Contains("text=\"OK\""))
							{
								IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", m_driver);
								if (webElement != null)
								{
									webElement.Click();
									Thread.Sleep(1000);
								}
							}
							else
							{
								if (pageSource2.Contains("\"STOP PROXY SERVICE\""))
								{
									ADBHelperCCK.BackToHome(p_DeviceId);
									return true;
								}
								Thread.Sleep(3000);
							}
							pageSource2 = GetPageSource();
						}
						Thread.Sleep(1000);
						ADBHelperCCK.BackToHome(p_DeviceId);
						return true;
					}
				}
				catch (Exception ex2)
				{
					ADBHelperCCK.ShowTextMessageOnPhone(deviceId, "Proxy - Error ");
					Utils.CCKLog("SetProxy - Error", ex2.Message);
				}
				return result;
			}
			return ADBHelperCCK.SetProxyNoRootClient(deviceId, proxy, m_driver);
		}

		private bool OnOffModule(DeviceEntity device, RemoteWebDriver driver, bool state)
		{
			if (device.DeviceId.Contains("emulator"))
			{
				return true;
			}
			string deviceId = device.DeviceId;
			if (Utils.Convert2Int(device.Version) <= 8)
			{
				return true;
			}
			ADBHelperCCK.CloseApp(deviceId, "org.meowcat.edxposed.manager");
			Thread.Sleep(500);
			ADBHelperCCK.EnableModules(deviceId);
			Thread.Sleep(500);
			ADBHelperCCK.OpenApp(deviceId, "org.meowcat.edxposed.manager");
			Thread.Sleep(1000);
			try
			{
				string text = "";
				int num = 0;
				bool flag = false;
				CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
				while (num < 5)
				{
					text = GetPageSource();
					if (text.Contains("android:id/checkbox"))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@resource-id=\"android:id/checkbox\"]", driver);
						if (webElement != null)
						{
							Thread.Sleep(500);
							webElement.Click();
						}
						text = GetPageSource();
					}
					num++;
					Thread.Sleep(500);
					if (text.Contains("org.meowcat.edxposed.manager:id/md_buttonDefaultPositive"))
					{
						List<IWebElement> list = ADBHelperCCK.FindElements(driver, By.Id("org.meowcat.edxposed.manager:id/md_buttonDefaultPositive"));
						if (list != null)
						{
							list[0].Click();
							Thread.Sleep(1000);
							text = GetPageSource();
						}
					}
					if (!text.Contains("EdXposed Framework is not (properly) installed"))
					{
						if (text.Contains("Open navigation drawer"))
						{
							CCKNode cCKNode = cCKDriver.FindElement("//*[@content-desc=\"Open navigation drawer\" or @text=\"Open navigation drawer\"]", text);
							if (cCKNode != null)
							{
								cCKNode.Click();
								flag = true;
								break;
							}
						}
						continue;
					}
					return false;
				}
				if (flag)
				{
					if (text.Contains("is not currently installed or active"))
					{
						return false;
					}
					text = GetPageSource();
					List<CCKNode> list2 = cCKDriver.FindElements("//*[@text=\"Modules\" or @content-desc=\"Modules\"]", text);
					if (list2 != null && list2.Count > 0)
					{
						list2[0].Click();
						Thread.Sleep(1000);
						text = GetPageSource();
						List<CCKNode> list3 = cCKDriver.FindElements("//android.widget.Switch[@resource-id=\"org.meowcat.edxposed.manager:id/checkbox\"]", text);
						foreach (CCKNode item in list3)
						{
							if (state && item.Text.ToString().ToUpper().Equals("ON"))
							{
								item.Click();
								Thread.Sleep(1000);
								if (item.Text.ToString().ToUpper().Equals("OFF"))
								{
									item.Click();
									Thread.Sleep(1000);
								}
							}
							if (item.Text.ToString().ToUpper().Equals(state ? "OFF" : "ON"))
							{
								item.Click();
								Thread.Sleep(1000);
							}
						}
					}
				}
				if (!state)
				{
					ADBHelperCCK.CloseApp(deviceId, "org.meowcat.edxposed.manager");
				}
				Thread.Sleep(500);
			}
			catch (Exception ex)
			{
				File.WriteAllText(CaChuaConstant.LOG_ACTION, "org.meowcat.edxposed.manager: " + deviceId + ": " + ex.Message);
			}
			return true;
		}

		public bool ProcessRootPhone(Networking network, bool appLite, DataGridView gridPhone, Form main)
		{
			string deviceId = deviceEntity.DeviceId;
			ADBHelperCCK.SetPortrait(deviceId);
			string text = deviceId;
			ShowMessageOnGrid(text, "Remove System Account", gridPhone, main);
			DeviceEntity device = deviceEntity;
			RemoteWebDriver driver = m_driver;
			string uid = Info.Uid;
			Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA);
			device.Rooted = deviceEntity.Rooted;
			_ = DateTime.Now;
			if (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.REMOVE_GMAIL)))
			{
				RemoveSystemAccount(m_driver);
			}
			try
			{
				if (device.Rooted)
				{
					ADBHelperCCK.ClearAppData(deviceId, appLite ? CaChuaConstant.PACKAGE_NAME_LITE : CaChuaConstant.PACKAGE_NAME);
				}
				else
				{
					ADBHelperCCK.CloseApp(deviceId, appLite ? CaChuaConstant.PACKAGE_NAME_LITE : CaChuaConstant.PACKAGE_NAME);
				}
				ADBHelperCCK.CloseApp(deviceId, new List<string> { "com.cck.support" });
				Thread.Sleep(200);
				ADBHelperCCK.OpenApp(deviceId, new List<string> { "com.cck.support" });
				ADBHelperCCK.ExecuteCMD(text, "shell ime set com.android.adbkeyboard/.AdbIME");
				ADBHelperCCK.SetPortrait(text);
				try
				{
					ShowMessageOnGrid(text, "Change Network", gridPhone, main);
					ADBHelperCCK.ClearAppData(deviceId, "com.cell47.College_Proxy");
					ADBHelperCCK.ClearProxy(deviceId);
					switch (network)
					{
					case Networking._3G:
						ShowMessageOnGrid(text, "Turn On 3G", gridPhone, main);
						ADBHelperCCK.TurnOnAirplane(device.DeviceId, driver);
						break;
					case Networking.Wifi:
						ShowMessageOnGrid(text, "Wifi", gridPhone, main);
						ADBHelperCCK.EnalbeWifi(deviceId);
						ADBHelperCCK.ClearProxy(deviceId);
						break;
					case Networking.Proxy:
					{
						ShowMessageOnGrid(text, "Set Proxy", gridPhone, main);
						SQLiteUtils sQLiteUtils = new SQLiteUtils();
						DataRow accountById = sQLiteUtils.GetAccountById(uid);
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						Thread.Sleep(2000);
						string text5 = "";
						int num5 = 0;
						while (true)
						{
							IL_01e2:
							try
							{
								text5 = ((accountById == null) ? "" : ((accountById["Proxy"] != null) ? accountById["Proxy"].ToString() : ""));
								if (text5.Length > 10)
								{
									ADBHelperCCK.ShowTextMessageOnPhone(text, "Proxy IP: " + text5);
								}
							}
							catch (Exception)
							{
								ADBHelperCCK.ShowTextMessageOnPhone(text, "Proxy Error");
							}
							sQLiteUtils = null;
							if (text5.Length < 10 && File.Exists(CaChuaConstant.STATIC_PROXY))
							{
								string[] array2 = File.ReadAllLines(CaChuaConstant.STATIC_PROXY);
								if (array2 != null && array2.Length != 0)
								{
									text5 = Utils.GetFirstItemFromFile(CaChuaConstant.STATIC_PROXY, remove: false);
								}
							}
							if (!(text5 != ""))
							{
								break;
							}
							if (text5.Contains("|"))
							{
								string[] array3 = text5.Split('|');
								text5 = array3[0].Trim();
								try
								{
									if (array3.Length >= 2)
									{
										new WebClient().DownloadString(array3[1].Trim());
									}
									int num6 = 30;
									if (array3.Length >= 3)
									{
										num6 = Utils.Convert2Int(array3[2].Trim());
										if (num6 == 0)
										{
											num6 = 10;
										}
									}
									Thread.Sleep(num6 * 1000);
								}
								catch
								{
								}
							}
							bool flag2 = false;
							while (!flag2)
							{
								ADBHelperCCK.StopApp(text, "com.cell47.College_Proxy");
								if (!ADBHelperCCK.CheckLiveProxy(text5))
								{
									if (num5++ < 5)
									{
										goto IL_01e2;
									}
									text5 = "127.0.0.1:8080";
								}
								if (!(flag2 = SetProxy(deviceId, text5)))
								{
									File.WriteAllText(Application.StartupPath + "\\Log\\" + text + ".xml", GetPageSource());
									Thread.Sleep(5000);
								}
								ADBHelperCCK.ShowTextMessageOnPhone(text, flag2 ? FaceBookHelper.GetIPFromDevice(deviceId, text5) : "Unable to change proxy");
							}
							break;
						}
						break;
					}
					case Networking.VPN:
					{
						ShowMessageOnGrid(text, "VPN", gridPhone, main);
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						string text2 = "hotspotshield.android.vpn";
						if (ADBHelperCCK.IsInstallApp(text, text2))
						{
							ADBHelperCCK.StopApp(text, text2);
							ADBHelperCCK.OpenApp(text, text2);
							string pageSource3 = GetPageSource();
							ADBHelperCCK.WaitMe("//*[@resource-id=\"hotspotshield.android.vpn:id/server_location_label\"]", driver);
							while (!pageSource3.Contains("hotspotshield.android.vpn:id/server_location_label"))
							{
								pageSource3 = GetPageSource();
								if (!pageSource3.Contains("By using the app"))
								{
									continue;
								}
								IWebElement webElement3 = ADBHelperCCK.WaitMe("//*[@text=\"Connect\"]", driver);
								if (webElement3 == null)
								{
									continue;
								}
								webElement3.Click();
								Thread.Sleep(2000);
								webElement3 = ADBHelperCCK.WaitMe("//*[@text=\"OK\"]", driver);
								if (webElement3 == null)
								{
									continue;
								}
								webElement3.Click();
								Thread.Sleep(2000);
								int num = 0;
								while (!pageSource3.Contains("\"STOP\"") && num < 30)
								{
									Thread.Sleep(1000);
									num++;
									pageSource3 = GetPageSource();
									if (pageSource3.Contains("\"STOP\""))
									{
										webElement3 = ADBHelperCCK.WaitMe("//*[@text=\"STOP\"]", driver);
										if (webElement3 != null)
										{
											webElement3.Click();
											Thread.Sleep(2000);
											break;
										}
									}
								}
							}
							string text3 = Utils.GetFirstItemFromFile(CaChuaConstant.VPN, remove: false);
							if (text3 == "")
							{
								text3 = "Vietnam";
							}
							IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[@resource-id=\"hotspotshield.android.vpn:id/server_location_label\"]", driver);
							if (webElement4 == null)
							{
								break;
							}
							string text4 = webElement4.Text;
							if (!(text3.ToLower() == text4.ToLower()))
							{
								webElement4.Click();
								ADBHelperCCK.WaitMe("//*[@text=\"Optimal\"]", driver);
								Point screenResolution = ADBHelperCCK.GetScreenResolution(text);
								pageSource3 = GetPageSource();
								new List<string>();
								while (!pageSource3.Contains(text3))
								{
									pageSource3 = GetPageSource();
									if (pageSource3.Contains(text3))
									{
										IWebElement webElement5 = ADBHelperCCK.AppGetObject("//*[@resource-id=\"hotspotshield.android.vpn:id/title\" and @text=\"" + text3 + "\"]", driver);
										if (webElement5 != null)
										{
											webElement5.Click();
											Thread.Sleep(2000);
											pageSource3 = GetPageSource();
											ADBHelperCCK.WaitMe("//*[@text=\"Connect\"]", driver)?.Click();
											pageSource3 = GetPageSource();
											int num2 = 0;
											while (!pageSource3.Contains("\"STOP\"") && num2 < 30)
											{
												Thread.Sleep(1000);
												num2++;
												pageSource3 = GetPageSource();
											}
											break;
										}
									}
									ADBHelperCCK.Swipe(text, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4, 500);
								}
								IWebElement webElement6 = ADBHelperCCK.AppGetObject("//*[@resource-id=\"hotspotshield.android.vpn:id/title\" and @text=\"" + text3 + "\"]", driver);
								if (webElement6 != null)
								{
									webElement6.Click();
									Thread.Sleep(2000);
									pageSource3 = GetPageSource();
									ADBHelperCCK.WaitMe("//*[@text=\"Connect\"]", driver)?.Click();
									pageSource3 = GetPageSource();
									int num3 = 0;
									while (!pageSource3.Contains("\"STOP\"") && num3 < 30)
									{
										Thread.Sleep(1000);
										num3++;
										pageSource3 = GetPageSource();
									}
								}
								ADBHelperCCK.BackToHome(text);
							}
							else
							{
								ADBHelperCCK.WaitMe("//*[@text=\"Connect\"]", driver)?.Click();
								pageSource3 = GetPageSource();
								int num4 = 0;
								while (!pageSource3.Contains("\"STOP\"") && num4 < 30)
								{
									Thread.Sleep(1000);
									num4++;
									pageSource3 = GetPageSource();
								}
								ADBHelperCCK.BackToHome(text);
							}
						}
						else
						{
							ADBHelperCCK.ShowTextMessageOnPhone(text, "Not install hotspotshield.android.vpn");
						}
						break;
					}
					case Networking.OBCProxy:
					{
						DataRow accountById2 = sql.GetAccountById(uid);
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						string text6 = "";
						try
						{
							text6 = ((accountById2 == null) ? "" : ((accountById2["Proxy"] != null) ? accountById2["Proxy"].ToString() : ""));
						}
						catch
						{
						}
						if (text6 != "")
						{
							SetProxy(deviceId, text6);
							Utils.ResetOBCProxy(text6);
						}
						accountById2 = null;
						break;
					}
					case Networking.ProxySeller:
					{
						DataRow accountById3 = sql.GetAccountById(uid);
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						string text9 = "";
						try
						{
							text9 = ((accountById3 == null) ? "" : ((accountById3["Proxy"] != null) ? accountById3["Proxy"].ToString() : ""));
						}
						catch
						{
						}
						if (text9.Length < 10 && File.Exists(CaChuaConstant.Proxy_Seller))
						{
							string[] array6 = File.ReadAllLines(CaChuaConstant.Proxy_Seller);
							if (array6 != null && array6.Length != 0)
							{
								text9 = Utils.GetFirstItemFromFile(CaChuaConstant.Proxy_Seller, remove: false);
							}
						}
						if (text9 != "")
						{
							DateTime now = DateTime.Now;
							ADBHelperCCK.SetProxyDrony(deviceId, text9, driver);
							_ = DateTime.Now.Subtract(now).TotalMilliseconds;
							ADBHelperCCK.BackToHome(device.DeviceId);
						}
						accountById3 = null;
						break;
					}
					case Networking.Proxy_V6NET:
					{
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						ADBHelperCCK.ClearProxy(deviceId);
						string text8 = Utils.ResetProxyV6(deviceId);
						if (text8.Length > 10)
						{
							SetProxy(deviceId, text8);
						}
						break;
					}
					case Networking.XProxy_LAN:
					{
						ShowMessageOnGrid(text, "XProxy", gridPhone, main);
						ADBHelperCCK.ClearAppData(deviceId, "com.cell47.College_Proxy");
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						if (!File.Exists(CaChuaConstant.XPROXY_LAN))
						{
							break;
						}
						List<DeviceProxy> list2 = new JavaScriptSerializer().Deserialize<List<DeviceProxy>>(Utils.ReadTextFile(CaChuaConstant.XPROXY_LAN));
						DeviceProxy deviceProxy = list2.Find((DeviceProxy a) => a.DeviceId == device.DeviceId);
						if (deviceProxy == null || !(deviceProxy.DeviceId == device.DeviceId))
						{
							break;
						}
						ADBHelperCCK.ClearProxy(deviceId);
						SetProxy(deviceId, $"{deviceProxy.Proxy.Ip}:{deviceProxy.Proxy.Port}");
						if (deviceProxy.MultipleDevice)
						{
							string key = $"{deviceProxy.Proxy.Ip}:{deviceProxy.Proxy.Port}";
							deviceProxy.LastChange = DateTime.Now.AddMinutes(10.0);
							if (!frmMain.dicXproxyLan.ContainsKey(key))
							{
								frmMain.dicXproxyLan.Add(key, new List<DeviceProxy> { deviceProxy });
							}
							else
							{
								frmMain.dicXproxyLan[key].Add(deviceProxy);
							}
						}
						else
						{
							Utils.ResetXProxyLAN(deviceProxy);
						}
						break;
					}
					case Networking.ShopLive:
						ShowMessageOnGrid(text, "Shop Like", gridPhone, main);
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						Thread.Sleep(500);
						try
						{
							ShopLikeProxy shopLikeProxy = new ShopLikeProxy();
							string newProxy2 = shopLikeProxy.GetNewProxy();
							SetProxy(text, newProxy2);
						}
						catch
						{
						}
						break;
					case Networking.ProxyFB:
						ShowMessageOnGrid(text, "Proxy FB", gridPhone, main);
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						Thread.Sleep(500);
						try
						{
							ProxyFB_COM proxyFB_COM = new ProxyFB_COM();
							string newProxy = proxyFB_COM.GetNewProxy();
							SetProxy(text, newProxy);
						}
						catch
						{
						}
						break;
					case Networking.TMProxy:
					{
						ShowMessageOnGrid(text, "TMProxy", gridPhone, main);
						ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
						ADBHelperCCK.ClearProxy(deviceId);
						string[] array4 = File.ReadAllLines(CaChuaConstant.TM_Proxy);
						string text7 = "";
						int locationId = 0;
						if (array4 != null && array4.Length != 0)
						{
							text7 = Utils.GetFirstItemFromFile(CaChuaConstant.TM_Proxy, remove: false);
							if (text7 == "")
							{
								return false;
							}
							string[] array5 = text7.Split("|;:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
							text7 = array5[0];
							if (array5.Length == 2)
							{
								locationId = Utils.Convert2Int(array5[1]);
							}
							frmMain.DicTmProxy.Add(deviceId, text7);
						}
						TMProxy tMProxy = new TMProxy(text7, deviceId);
						try
						{
							TMProxyResult newProxy3 = tMProxy.GetNewProxy(locationId);
							if (newProxy3.code == 0 && newProxy3.data.https != "")
							{
								ADBHelperCCK.SetProxy(deviceId, $"{newProxy3.data.https}", driver, device);
							}
						}
						catch
						{
						}
						tMProxy = null;
						break;
					}
					case Networking.HMA:
					{
						ShowMessageOnGrid(text, "HMA", gridPhone, main);
						bool flag;
						if (flag = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.HMA_TYPE)))
						{
							ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
							ADBHelperCCK.OpenApp(deviceId, "com.hidemyass.hidemyassprovpn");
							string pageSource = GetPageSource();
							if (pageSource.Contains("com.hidemyass.hidemyassprovpn:id/reload_button"))
							{
								IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.hidemyass.hidemyassprovpn:id/reload_button\"]", driver);
								if (webElement != null)
								{
									webElement.Click();
									Thread.Sleep(1000);
									ADBHelperCCK.WaitMe("//android.widget.Switch[@text=\"ON\"]", driver);
								}
							}
						}
						if (!flag)
						{
							ADBHelperCCK.ClearAppData(deviceId, "com.hidemyass.hidemyassprovpn");
							ADBHelperCCK.ExecuteCMD(device.DeviceId, "shell \"svc wifi enable\"");
							ADBHelperCCK.OpenApp(deviceId, "com.hidemyass.hidemyassprovpn");
							Thread.Sleep(5000);
							string pageSource2 = GetPageSource();
							if (pageSource2.Contains("\"CANCEL\""))
							{
								IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"CANCEL\"]", driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(1000);
									webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Already purchased?\"]", driver);
									if (webElement2 != null)
									{
										webElement2.Click();
										Thread.Sleep(1000);
										webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Sign in\"]", driver);
										if (webElement2 != null)
										{
											webElement2.Click();
											Thread.Sleep(1000);
											pageSource2 = GetPageSource();
											List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), driver);
											if (list != null && list.Count == 2)
											{
												string firstItemFromFile = Utils.GetFirstItemFromFile(CaChuaConstant.HMA, remove: false);
												if (firstItemFromFile != "")
												{
													string[] array = firstItemFromFile.Split('|');
													if (array != null && array.Length >= 2)
													{
														list[0].Clear();
														list[0].SendKeys(array[0]);
														Thread.Sleep(500);
														list[1].Clear();
														list[1].SendKeys(array[1]);
														Thread.Sleep(500);
														webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"SIGN IN\"]", driver);
														if (webElement2 != null)
														{
															webElement2.Click();
															Thread.Sleep(3000);
															webElement2 = ADBHelperCCK.WaitMe("//*[@resource-id=\"com.hidemyass.hidemyassprovpn:id/text_off\"]", driver);
															if (webElement2 != null)
															{
																webElement2.Click();
																Thread.Sleep(1000);
																webElement2 = ADBHelperCCK.WaitMe("//android.widget.Switch[@text=\"ON\"]", driver);
																Thread.Sleep(1000);
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						if (File.Exists(CaChuaConstant.HMA_DELAY))
						{
							Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.HMA_DELAY)));
						}
						break;
					}
					}
					ShowMessageOnGrid(text, "End Change Network", gridPhone, main);
				}
				catch (Exception ex2)
				{
					ShowMessageOnGrid(text, ex2.Message, gridPhone, main);
					Utils.CCKLog("Process Root", ex2.Message);
				}
				if (!Directory.Exists(Application.StartupPath + "\\Devices"))
				{
					Directory.CreateDirectory(Application.StartupPath + "\\Devices");
				}
				if (Utils.Convert2Int(device.Version) > 8)
				{
					ADBHelperCCK.EnableModules(deviceId);
				}
				bool result = false;
				if (device.Rooted)
				{
					string path = Application.StartupPath + "\\Devices\\CCKInfo_" + ADBHelperCCK.NormalizeDeviceName(deviceId) + ".txt";
					if (File.Exists(path))
					{
						File.Delete(path);
					}
					DeviceHelper.ProduceTxtAddNew(deviceId, uid);
					ADBHelperCCK.PushInfoFile(deviceId, "\"" + Application.StartupPath + "\\Devices\\CCKInfo_" + ADBHelperCCK.NormalizeDeviceName(deviceId) + ".txt\" /sdcard/CCKInfo_" + ADBHelperCCK.NormalizeDeviceName(deviceId) + ".txt");
					Thread.Sleep(500);
					result = ADBHelperCCK.StartServiceFake(deviceId);
				}
				if (network == Networking._3G)
				{
					ADBHelperCCK.ClearProxy(deviceId);
					if (File.Exists(CaChuaConstant.DELAY4G))
					{
						Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY4G)));
					}
					ADBHelperCCK.TurnOffAirplane(device.DeviceId);
				}
				Thread.Sleep(10000);
				if (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.SHOW_IP)))
				{
					if (!ADBHelperCCK.IsInstallApp(text, CaChuaConstant.SHOW_IP_APP))
					{
						string text10 = "ShowMyIPAddress.apk";
						if (!File.Exists(Application.StartupPath + "\\Config\\" + text10))
						{
							frmDownload frmDownload = new frmDownload();
							frmDownload.Download("https://cck.vn/Download/Utils/" + text10 + ".rar", Application.StartupPath + "\\Config\\" + text10);
							frmDownload.ShowDialog();
							if (frmDownload.DownloadCompleted)
							{
								Thread.Sleep(1000);
							}
						}
						ADBHelperCCK.InstallApp(text, Application.StartupPath + "\\Config\\" + text10);
						Thread.Sleep(3000);
					}
					ADBHelperCCK.OpenApp(text, CaChuaConstant.SHOW_IP_APP);
					Thread.Sleep(10000);
					ADBHelperCCK.CloseApp(text, CaChuaConstant.SHOW_IP_APP);
				}
				ADBHelperCCK.OpenApp(deviceId, appLite ? CaChuaConstant.PACKAGE_NAME_LITE : CaChuaConstant.PACKAGE_NAME);
				string pageSource4 = GetPageSource();
				if (!pageSource4.Contains("Something went wrong"))
				{
				}
				return result;
			}
			catch (Exception ex3)
			{
				Utils.CCKLog(ex3.Message + " -" + uid, "ProcessRootPhone");
			}
			finally
			{
				ShowMessageOnGrid(text, "Process Root Phone finally", gridPhone, main);
			}
			ShowMessageOnGrid(text, "Process Root Phone false", gridPhone, main);
			return false;
		}

		private void GridViewMessageLog(string searchValue, string msg, Color c, string findColmn = "Phone", string columnName = "Status")
		{
		}

		private void ShowMessage(DataGridViewRow CurrentRow, string msg, string field = "Log")
		{
			try
			{
				CurrentRow.Cells[field].Value = msg;
			}
			catch
			{
			}
		}

		public void CleanPhone(DeviceEntity entity)
		{
			DateTime now = DateTime.Now;
			string deviceId = entity.DeviceId;
			Point screenResolution = ADBHelperCCK.GetScreenResolution(deviceId);
			ADBHelperCCK.ExecuteCMD(deviceId, "shell am start -a android.settings.SETTINGS");
			Thread.Sleep(2000);
			ADBHelperCCK.Swipe(deviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4);
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"System\"]", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(2000);
				GridViewMessageLog(deviceId, "Reset options", Color.Green);
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Reset options\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(2000);
					GridViewMessageLog(deviceId, "Erase all data (factory reset)", Color.Green);
					webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Erase all data (factory reset)\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(2000);
						ADBHelperCCK.Swipe(deviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4);
						webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Erase all data\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(2000);
							GridViewMessageLog(deviceId, "Erase all data", Color.Green);
							webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Erase all data\"]", m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(10000);
								Thread.Sleep(60000);
							}
						}
					}
				}
				GridViewMessageLog(deviceId, "Started", Color.Green);
			}
			_ = DateTime.Now.Subtract(now).TotalMilliseconds;
			SetupEnviroment(deviceId);
			ADBHelperCCK.Reconnect2Device(deviceId);
			m_driver = ADBHelperCCK.StartPhone(entity);
		}

		public void SetupEnviroment(string p_DeviceId)
		{
			ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell settings put secure autofill_service null");
			GridViewMessageLog(p_DeviceId, "Setting Shopee APK", Color.Green);
			ADBHelperCCK.InstallApp(p_DeviceId, "App\\Shopee_cck.apk");
			while (!ADBHelperCCK.IsInstallApp(p_DeviceId, "com.shopee.vn"))
			{
				Thread.Sleep(5000);
				ADBHelperCCK.InstallApp(p_DeviceId, "App\\Shopee_cck.apk");
			}
			GridViewMessageLog(p_DeviceId, "Setup APK Successfully", Color.Green);
		}

		private string GetLocalSource()
		{
			string text = "";
			try
			{
				_ = DateTime.Now;
				if (m_driver != null)
				{
					text = m_driver.PageSource;
					if (text.Contains("text=\"Bắt đầu\""))
					{
						List<IWebElement> list = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.ui.image.UIImage | //com.lynx.tasm.behavior.ui.LynxFlattenUI", m_driver);
						if (list != null && list.Count > 0)
						{
							list[list.Count - 1].Click();
						}
						list = null;
						text = m_driver.PageSource;
					}
					if (text.Contains("Refresh to try again"))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Refresh\" or @text=\"Refresh\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(2000);
						}
						webElement = null;
						text = m_driver.PageSource;
					}
					if (text.Contains("Allow personalized ads"))
					{
						List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[contains(@content-desc,\"Accept\") or contains(@text,\"Accept\")]", m_driver);
						if (list2 != null && list2.Count > 0)
						{
							list2[list2.Count - 1].Click();
							Thread.Sleep(2000);
						}
						list2 = null;
						text = m_driver.PageSource;
					}
					if (text.Contains("\"NONE OF THE ABOVE\""))
					{
						ADBHelperCCK.AppGetObject("//*[@text=\"NONE OF THE ABOVE\"]", m_driver)?.Click();
						IWebElement webElement2 = null;
						text = m_driver.PageSource;
					}
					if (text.Contains("text=\"Bắt đầu\""))
					{
						List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.behavior.ui.LynxFlattenUI", m_driver);
						if (list3 != null && list3.Count > 0)
						{
							list3[list3.Count - 1].Click();
						}
						list3 = null;
						text = m_driver.PageSource;
					}
					if (text.Contains("text=\"Got it\"") || text.Contains("text=\"Got It\""))
					{
						List<IWebElement> list4 = ADBHelperCCK.AppGetObjects("//*[lower-case(@text)=\"got it\"]", m_driver);
						if (list4 != null && list4.Count > 0)
						{
							list4[list4.Count - 1].Click();
						}
						list4 = null;
						text = m_driver.PageSource;
					}
					if (text.Contains("Drag the slider to fit the puzzle") && unlockbyhand)
					{
						int num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_CAPTCHA));
						Thread.Sleep(1000 * num * 2);
						text = m_driver.PageSource;
						ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Chờ giải Captcha bằng tay");
					}
					if (text.Contains("Enter phone number") && text.Contains("Skip"))
					{
						List<IWebElement> list5 = ADBHelperCCK.AppGetObjects("//*[@text=\"Skip\"] or @content-desc=\"Skip\"]", m_driver);
						if (list5 != null && list5.Count > 0)
						{
							list5[list5.Count - 1].Click();
						}
						list5 = null;
						text = m_driver.PageSource;
					}
					if (text.Contains("\"Don't allow\"") || text.Contains("\"Don’t allow\""))
					{
						IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Don't allow\" or @text=\"Don’t allow\"]", m_driver);
						if (webElement3 != null)
						{
							webElement3.Click();
							webElement3 = null;
							text = m_driver.PageSource;
						}
					}
				}
			}
			catch (Exception)
			{
				ADBHelperCCK.Reconnect2Device(p_DeviceId);
				int i = 0;
				try
				{
					if (m_driver != null)
					{
						m_driver.Quit();
					}
				}
				catch
				{
				}
				for (; i < 5 && text == ""; i++, Thread.Sleep(1000))
				{
					m_driver = ADBHelperCCK.StartPhone(deviceEntity);
					if (m_driver == null)
					{
						continue;
					}
					try
					{
						text = m_driver.PageSource;
					}
					catch (Exception)
					{
						if (m_driver != null)
						{
							m_driver.Close();
							m_driver.Quit();
							m_driver.Dispose();
						}
						text = "";
						continue;
					}
					break;
				}
			}
			return text;
		}

		public string GetPageSource(bool ret = true)
		{
			string localSource = GetLocalSource();
			if (localSource.Contains("\"Got it\""))
			{
				List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Got it\")]", m_driver);
				if (list != null && list.Count > 0)
				{
					ADBHelperCCK.Tap(p_DeviceId, list[list.Count - 1].Location.X, list[list.Count - 1].Location.Y);
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Save\"") && localSource.Contains("Profile view history turned on"))
			{
				IWebElement webElement = ADBHelperCCK.AppGetObject("//*[contains(@text,\"Save\")]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("content-desc=\"Personalized ads") && localSource.Contains("personalized based on your activity"))
			{
				List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[@text=\"Select\"]", m_driver);
				if (list2 != null && list2.Count > 0)
				{
					list2[list2.Count - 1].Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("Edit unfinished video") && localSource.Contains("Discard"))
			{
				List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//*[@text=\"Discard\"]", m_driver);
				if (list3 != null && list3.Count > 0)
				{
					list3[list3.Count - 1].Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"View details\"") && localSource.Contains("\"Not now\""))
			{
				List<IWebElement> list4 = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Not now\") or contains(@content-desc,\"Not now\")]", m_driver);
				if (list4 != null && list4.Count > 0)
				{
					ADBHelperCCK.Tap(p_DeviceId, list4[list4.Count - 1].Location.X, list4[list4.Count - 1].Location.Y);
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Reconfirm email\""))
			{
				List<IWebElement> list5 = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Confirm\") or contains(@content-desc,\"Confirm\")]", m_driver);
				if (list5 != null && list5.Count > 0)
				{
					ADBHelperCCK.Tap(p_DeviceId, list5[list5.Count - 1].Location.X, list5[list5.Count - 1].Location.Y);
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("permission_message"))
			{
				List<IWebElement> list6 = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Allow\")]", m_driver);
				if (list6 != null && list6.Count > 0)
				{
					ADBHelperCCK.Tap(p_DeviceId, list6[list6.Count - 1].Location.X, list6[list6.Count - 1].Location.Y);
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Post anyway\""))
			{
				List<IWebElement> list7 = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Post anyway\") or contains(@content-desc,\"Post anyway\")]", m_driver);
				if (list7 != null && list7.Count > 0)
				{
					ADBHelperCCK.Tap(p_DeviceId, list7[list7.Count - 1].Location.X, list7[list7.Count - 1].Location.Y);
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if ((localSource.Contains("resource-id=\"verify-bar-close\"") || localSource.Contains("verify to continue:")) && CloseCaptcha)
			{
				ADBHelperCCK.AppGetObject("//*[@resource-id=\"verify-bar-close\"]", m_driver)?.Click();
				IWebElement webElement2 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"Notifications keep you up to date!\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Later\"]", m_driver)?.Click();
				IWebElement webElement3 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Profile view history turned"))
			{
				List<IWebElement> list8 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
				if (list8 != null && list8.Count == 5)
				{
					list8[1].Click();
				}
				list8 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Get rewards and earning opportunities") && localSource.Contains("text=\"Cancel\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Cancel\"]", m_driver)?.Click();
				IWebElement webElement4 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"I agree\"") && (localSource.Contains("text=\"Remind me later\"") || localSource.Contains("review and agree to our updated Terms of Service")))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"I agree\"]", m_driver)?.Click();
				IWebElement webElement5 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"Privacy Policy Update\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", m_driver)?.Click();
				IWebElement webElement6 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Bắt đầu nhiệm vụ hôm nay\""))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(localSource);
				XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//com.lynx.component.svg.UISvg");
				if (xmlNodeList != null && xmlNodeList.Count > 0)
				{
					XmlNode nextSibling = xmlNodeList[0].NextSibling;
					string input = nextSibling.Attributes["bounds"].Value.ToString();
					Regex regex = new Regex("([0-9]+)");
					MatchCollection matchCollection = regex.Matches(input);
					int num = 0;
					int num2 = 0;
					if (matchCollection.Count == 4)
					{
						num = (Utils.Convert2Int(matchCollection[0].Value) + Utils.Convert2Int(matchCollection[2].Value)) / 2;
						num2 = (Utils.Convert2Int(matchCollection[1].Value) + Utils.Convert2Int(matchCollection[3].Value)) / 2;
					}
					ADBHelperCCK.Tap(p_DeviceId, num, num2);
					xmlDocument = null;
				}
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Bắt đầu\"") && localSource.Contains("\"Vuốt lên để bỏ qua\""))
			{
				XmlDocument xmlDocument2 = new XmlDocument();
				xmlDocument2.LoadXml(localSource);
				XmlNodeList xmlNodeList2 = xmlDocument2.SelectNodes("//*[@text=\"Vuốt lên để bỏ qua\" or @content-desc=\"Vuốt lên để bỏ qua\"]");
				if (xmlNodeList2 != null && xmlNodeList2.Count > 0)
				{
					XmlNode previousSibling = xmlNodeList2[0].PreviousSibling;
					string input2 = previousSibling.Attributes["bounds"].Value.ToString();
					Regex regex2 = new Regex("([0-9]+)");
					MatchCollection matchCollection2 = regex2.Matches(input2);
					int num3 = 0;
					int num4 = 0;
					if (matchCollection2.Count == 4)
					{
						num3 = (Utils.Convert2Int(matchCollection2[0].Value) + Utils.Convert2Int(matchCollection2[2].Value)) / 2;
						num4 = (Utils.Convert2Int(matchCollection2[1].Value) + Utils.Convert2Int(matchCollection2[3].Value)) / 2;
					}
					ADBHelperCCK.Swipe(p_DeviceId, num3, num4, num3, num4 / 3);
					xmlDocument2 = null;
				}
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Authorize\"") && localSource.Contains("\"Cancel\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Cancel\" or @content-desc=\"Cancel\"]", m_driver)?.Click();
				IWebElement webElement7 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Tiktok is better with friend") && localSource.Contains("\"Skip\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Skip\" or @content-desc=\"Skip\"]", m_driver)?.Click();
				IWebElement webElement8 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Add or make edits to your video before posting."))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Cancel\"]", m_driver)?.Click();
				IWebElement webElement9 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Changes to our Terms of Service") && localSource.Contains("\"Continue\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Continue\"]", m_driver)?.Click();
				IWebElement webElement10 = null;
				localSource = GetLocalSource();
			}
			if (localSource.ToLower().Contains("\"TikTok Stories\"".ToLower()) && localSource.ToLower().Contains("\"Create a Story\"".ToLower()) && localSource.ToLower().Contains("\"Create Stories fast and easy".ToLower()))
			{
				List<IWebElement> list9 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
				if (list9 != null && list9.Count > 0)
				{
					list9[list9.Count - 1].Click();
				}
				list9 = null;
				localSource = GetLocalSource();
			}
			if (localSource.ToLower().Contains("\"share your story\"".ToLower()) && localSource.ToLower().Contains("\"Create a Story\"".ToLower()))
			{
				List<IWebElement> list10 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
				if (list10 != null && list10.Count > 0)
				{
					list10[list10.Count - 1].Click();
				}
				list10 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"Got it\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Got it\"]", m_driver)?.Click();
				IWebElement webElement11 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("content-desc=\"Nhận phần thưởng\"") && localSource.Contains("Nhận phần thưởng tiền mặt của bạn"))
			{
				List<IWebElement> list11 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.behavior.ui.LynxFlattenUI", m_driver);
				if (list11 != null && list11.Count > 0)
				{
					list11[list11.Count - 1].Click();
				}
				list11 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Keep editing your unposted video"))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Cancel\"]", m_driver)?.Click();
				IWebElement webElement12 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Product under review\""))
			{
				ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"ok\"]", m_driver)?.Click();
				IWebElement webElement13 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Add to Home screen"))
			{
				List<IWebElement> list12 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),'cancel') or contains(lower-case(@content-desc),'cancel')]", m_driver);
				if (list12 != null)
				{
					list12[list12.Count - 1].Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("Post view history turned on") && localSource.Contains("\"Save\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Save\" or @content-desc=\"Save\"]", m_driver)?.Click();
				IWebElement webElement14 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Refresh to try again"))
			{
				IWebElement webElement15 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Refresh\"]", m_driver);
				if (webElement15 != null)
				{
					webElement15.Click();
					Thread.Sleep(2000);
				}
				webElement15 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"Bắt đầu\""))
			{
				List<IWebElement> list13 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.behavior.ui.LynxFlattenUI", m_driver);
				if (list13 != null && list13.Count > 0)
				{
					list13[list13.Count - 1].Click();
				}
				list13 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"Mở ngay\"") && localSource.Contains("content-desc=\"Mở ngay\""))
			{
				List<IWebElement> list14 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.ui.image.FlattenUIImage", m_driver);
				if (list14 != null && list14.Count > 0)
				{
					list14[list14.Count - 1].Click();
				}
				list14 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("permission_allow_foreground_only_button"))
			{
				IWebElement webElement16 = ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.android.permissioncontroller:id/permission_allow_foreground_only_button\"]", m_driver);
				if (webElement16 != null)
				{
					webElement16.Click();
					Thread.Sleep(500);
				}
				webElement16 = null;
				localSource = GetLocalSource();
				if (localSource.Contains("permission_allow_one_time_button"))
				{
					webElement16 = ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.android.permissioncontroller:id/permission_allow_one_time_button\"]", m_driver);
					if (webElement16 != null)
					{
						webElement16.Click();
						Thread.Sleep(500);
					}
					webElement16 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("text=\"Wait\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Wait\"]", m_driver)?.Click();
				IWebElement webElement17 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("View your friends") && localSource.Contains("text=\"OK\""))
			{
				ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"OK\"]", m_driver)?.Click();
				IWebElement webElement18 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Mời một người bạn tham gia"))
			{
				List<IWebElement> list15 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.ui.image.UIImage", m_driver);
				if (list15 != null && list15.Count > 0)
				{
					list15[list15.Count - 1].Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("content-desc=\"Ưu đãi đặc biệt dịp cuối tuần\""))
			{
				List<IWebElement> list16 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.ui.image.UIImage", m_driver);
				if (list16 != null && list16.Count > 0)
				{
					list16[list16.Count - 1].Click();
					Thread.Sleep(1000);
				}
			}
			if (localSource.Contains("text=\"Agree and continue\""))
			{
				CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
				CCKNode cCKNode = cCKDriver.FindElement("//*[@text=\"agree and continue\"]", localSource.ToLower());
				if (cCKNode != null)
				{
					cCKNode.Click();
					Thread.Sleep(2000);
					localSource = GetLocalSource();
					if (localSource.ToLower().Contains("\"skip\""))
					{
						cCKNode = cCKDriver.FindElement("//*[@text=\"skip\"]", localSource.ToLower());
						if (cCKNode != null)
						{
							cCKNode.Click();
							Thread.Sleep(1000);
							for (int i = 0; i < 4; i++)
							{
								ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
								Thread.Sleep(500);
							}
						}
						else
						{
							Utils.CCKLog("Skip", "Click Error");
						}
						localSource = GetLocalSource();
					}
					cCKNode = null;
				}
				else
				{
					Utils.CCKLog("Agree and continue", "Click Error");
				}
				cCKDriver = null;
			}
			if (localSource.Contains("Swipe up for more"))
			{
				CCKDriver cCKDriver2 = new CCKDriver(p_DeviceId);
				CCKNode cCKNode2 = cCKDriver2.FindElement("//*[@text=\"Swipe up for more\"]", localSource);
				if (cCKNode2 != null)
				{
					int num5 = cCKNode2.Location.X + cCKNode2.Size.Width / 2;
					int y = cCKNode2.Location.Y + cCKNode2.Size.Height / 2;
					for (int j = 0; j < 4; j++)
					{
						ADBHelperCCK.Swipe(p_DeviceId, num5, y, num5, screen.Y / 4);
						Thread.Sleep(500);
					}
					localSource = GetLocalSource();
					if (localSource.Contains("Swipe up for more"))
					{
						Utils.CCKLog("Swipe up for more", "Not Scroll " + DateTime.Now.ToString());
					}
				}
				cCKDriver2 = null;
				cCKNode2 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Sync your contacts") || localSource.Contains("friends list to connect with them on"))
			{
				IWebElement webElement19 = ADBHelperCCK.AppGetObject("//*[@text=\"Don't allow\"]", m_driver);
				if (webElement19 != null)
				{
					webElement19.Click();
					Thread.Sleep(1000);
					for (int k = 0; k < 4; k++)
					{
						ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
						Thread.Sleep(500);
					}
					webElement19 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("text=\"NONE OF THE ABOVE\""))
			{
				ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"NONE OF THE ABOVE\"]", m_driver)?.Click();
				IWebElement webElement20 = null;
				localSource = GetLocalSource();
			}
			if ((localSource.Contains("text=\"OK\"") || localSource.Contains("content-desc=\"OK\"")) && !localSource.Contains("facebook"))
			{
				if (localSource.Contains("\"Account status\"") && Info.Uid != "" && localSource.Contains("Your account was logged out"))
				{
					Utils.ClearFolder(Application.StartupPath + "\\Authentication\\" + Info.Uid);
				}
				IWebElement webElement21 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"OK\" or upper-case(@content-desc)=\"OK\"]", m_driver);
				if (webElement21 != null)
				{
					webElement21.Click();
					for (int l = 0; l < 4; l++)
					{
						ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
						Thread.Sleep(500);
					}
				}
				webElement21 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Follow your friends\""))
			{
				IWebElement webElement22 = ADBHelperCCK.AppGetObject("//android.widget.RelativeLayout/android.widget.ImageView[2]", m_driver);
				if (webElement22 == null)
				{
					ADBHelperCCK.AppGetObject("//android.widget.RelativeLayout/android.widget.ImageView[1]", m_driver)?.Click();
				}
				else
				{
					webElement22.Click();
				}
				for (int m = 0; m < 4; m++)
				{
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
					Thread.Sleep(500);
				}
				webElement22 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("you like to usse cellular"))
			{
				IWebElement webElement23 = ADBHelperCCK.AppGetObject("//*[@text='Confirm' or @content-desc='Confirm']", m_driver);
				if (webElement23 != null)
				{
					webElement23.Click();
					Thread.Sleep(1000);
				}
				webElement23 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("content-desc=\"Bạn có thể rút tiền mặt hoặc đổi lấy phiếu giảm giá!\""))
			{
				List<IWebElement> list17 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.ui.image.UIImage", m_driver);
				if (list17 != null && list17.Count > 0)
				{
					list17[list17.Count - 1].Click();
					Thread.Sleep(1000);
				}
				list17 = null;
				localSource = GetLocalSource();
			}
			int num6 = 0;
			while ((localSource.Contains("\"Allow\"") || localSource.Contains("\"allow\"") || localSource.Contains("\"ALLOW\"")) && num6++ < 6)
			{
				IWebElement webElement24 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"allow\" or lower-case(@content-desc)=\"allow\"]", m_driver);
				if (webElement24 != null)
				{
					webElement24.Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
				if (num6 > 0 && num6 % 2 == 0)
				{
					ADBHelperCCK.BackToHome(p_DeviceId);
					Thread.Sleep(2000);
					ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
					Thread.Sleep(2000);
				}
				webElement24 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("the author of the mod does not bear any responsibility"))
			{
				CCKDriver cCKDriver3 = new CCKDriver(p_DeviceId);
				List<CCKNode> list18 = cCKDriver3.FindElements("//android.widget.Button[@text='\u200e\u200f\u200e\u200e\u200e\u200e\u200e\u200f\u200e\u200f\u200f\u200f\u200e\u200e\u200e\u200e\u200e\u200e\u200f\u200e\u200e\u200f\u200e\u200e\u200e\u200e\u200f\u200f\u200f\u200f\u200f\u200f\u200f\u200f\u200f\u200f\u200f\u200f\u200e\u200f\u200f\u200f\u200e\u200f\u200f\u200f\u200f\u200e\u200f\u200e\u200e\u200e\u200e\u200e\u200e\u200f\u200e\u200f\u200f\u200e\u200f\u200e\u200f\u200f\u200f\u200f\u200e\u200e\u200f\u200f\u200e\u200f\u200f\u200f\u200e\u200e\u200f\u200f\u200e\u200f\u200e\u200f\u200f\u200e\u200f\u200e\u200e\u200e\u200f\u200e\u200f\u200e\u200f\u200e\u200e\u200f\u200eOK']", localSource);
				CCKNode cCKNode3 = list18[list18.Count - 1];
				if (cCKNode3 != null)
				{
					cCKNode3.Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("How do you feel about the video you just watched"))
			{
				IWebElement webElement25 = ADBHelperCCK.AppGetObject("//*[@text=\"Cancel\" or @content-desc=\"Cancel\"]", m_driver);
				if (webElement25 != null)
				{
					webElement25.Click();
					Thread.Sleep(1000);
				}
				webElement25 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("in to follow accounts and like"))
			{
				IWebElement webElement26 = ADBHelperCCK.AppGetObject("//*[@text=\"Log in or sign up\" or @content-desc=\"Log in or sign up\"]", m_driver);
				if (webElement26 != null)
				{
					webElement26.Click();
					Thread.Sleep(1000);
				}
				webElement26 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("account was banned"))
			{
				IWebElement webElement27 = ADBHelperCCK.AppGetObject("//*[@text=\"Log out\" or @content-desc=\"Log out\"]", m_driver);
				if (webElement27 != null)
				{
					webElement27.Click();
					Thread.Sleep(1000);
				}
				webElement27 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Allow trending content and promotional"))
			{
				IWebElement webElement28 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"allow\" or lower-case(@content-desc)=\"allow\"]", m_driver);
				if (webElement28 != null)
				{
					webElement28.Click();
					Thread.Sleep(1000);
				}
				webElement28 = null;
				localSource = GetLocalSource();
			}
			if ((localSource.Contains("text=\"Add phone number\"") && localSource.Contains("\"Add\"")) || localSource.Contains("\"Not now\""))
			{
				if (localSource.Contains("\"Not now\""))
				{
					IWebElement webElement29 = ADBHelperCCK.AppGetObject("//*[@text=\"Not now\"]", m_driver);
					if (webElement29 != null)
					{
						webElement29.Click();
						webElement29 = null;
					}
				}
				localSource = GetLocalSource();
			}
			if (localSource.Contains("android.widget.EditText") && localSource.Contains("\"Add phone number\"") && localSource.Contains("\"Phone number\"") && localSource.Contains("\"Send code\""))
			{
				ADBHelperCCK.AppGetObject("//android.widget.LinearLayout/android.widget.ImageView", m_driver)?.Click();
				localSource = GetLocalSource();
				IWebElement webElement30 = null;
			}
			if (localSource.Contains("\"Sign up for an account\"") && localSource.Contains("\"Profile\"") && localSource.Contains("\"Sign up\"") && localSource.Contains("\"Home\"") && localSource.Contains("\"Inbox\""))
			{
				IWebElement webElement31 = ADBHelperCCK.AppGetObject("//*[@text=\"Sign up\"]", m_driver);
				if (webElement31 != null)
				{
					webElement31.Click();
					Thread.Sleep(1000);
					webElement31 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("No internet connection") && localSource.Contains("\"Refresh\"") && localSource.Contains("\"Verify to continue:\""))
			{
				IWebElement webElement32 = ADBHelperCCK.AppGetObject("//*[@text=\"Refresh\"]", m_driver);
				if (webElement32 != null)
				{
					webElement32.Click();
					Thread.Sleep(1000);
					webElement32 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("No internet connection") && localSource.Contains("\"Retry\""))
			{
				if (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.CLEARPROXY)) && File.Exists(CaChuaConstant.NETWORK))
				{
					Networking networking = new JavaScriptSerializer().Deserialize<Networking>(Utils.ReadTextFile(CaChuaConstant.NETWORK));
					if (networking != Networking._3G)
					{
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell \"svc wifi disable\"");
						Thread.Sleep(5000);
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell \"svc wifi enable\"");
						Thread.Sleep(5000);
						ADBHelperCCK.SetProxy(p_DeviceId, ":0", m_driver);
						Thread.Sleep(5000);
					}
					else
					{
						ADBHelperCCK.TurnOnAirplane(p_DeviceId, m_driver);
						Thread.Sleep(10000);
						ADBHelperCCK.TurnOffAirplane(p_DeviceId);
						Thread.Sleep(5000);
					}
				}
				IWebElement webElement33 = ADBHelperCCK.AppGetObject("//*[@text=\"Retry\"]", m_driver);
				if (webElement33 != null)
				{
					webElement33.Click();
					Thread.Sleep(1000);
					webElement33 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("More languages") && localSource.Contains("text=\"Confirm\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Confirm\"]", m_driver)?.Click();
				Thread.Sleep(1000);
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"Terms and conditions\"") && localSource.Contains("text=\"I agree to the TikTok") && localSource.Contains("android.widget.CheckBox"))
			{
				IWebElement webElement34 = ADBHelperCCK.AppGetObject("//android.widget.CheckBox", m_driver);
				if (webElement34 != null)
				{
					webElement34.Click();
					Thread.Sleep(1000);
					webElement34 = ADBHelperCCK.AppGetObject("//*[@text=\"Next\" or @text=\"Continue\"]", m_driver);
					if (webElement34 != null)
					{
						webElement34.Click();
						Thread.Sleep(1000);
					}
				}
				webElement34 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("your favorite videos to show your appreciation"))
			{
				IWebElement webElement35 = ADBHelperCCK.AppGetObject("//android.view.View", m_driver);
				if (webElement35 != null)
				{
					if (screen.X != 720)
					{
						ADBHelperCCK.Tap(p_DeviceId, webElement35.Location.X + webElement35.Size.Width / 2, webElement35.Location.Y + webElement35.Size.Height * 40);
					}
					else
					{
						ADBHelperCCK.Tap(p_DeviceId, webElement35.Location.X + (int)((double)(webElement35.Size.Width / 2) * 1.1), webElement35.Location.Y + webElement35.Size.Height * 40);
					}
					Thread.Sleep(1000);
					webElement35 = null;
					localSource = GetLocalSource();
				}
				webElement35 = null;
			}
			if (localSource.Contains("TikTok to access this device"))
			{
				IWebElement webElement36 = ADBHelperCCK.AppGetObject("//*[@text=\"Deny\" or @text=\"Deny\"]", m_driver);
				if (webElement36 != null)
				{
					ADBHelperCCK.Tap(p_DeviceId, webElement36.Location.X + webElement36.Size.Width / 2, webElement36.Location.Y + webElement36.Size.Height / 2);
					webElement36.Click();
					Thread.Sleep(1000);
					webElement36 = null;
					localSource = GetLocalSource();
				}
				webElement36 = null;
			}
			if (localSource.Contains("text=\"Not interested\"") && localSource.Contains("text=\"Follow\""))
			{
				IWebElement webElement37 = ADBHelperCCK.AppGetObject("//X.0Go/android.widget.FrameLayout/android.view.ViewGroup/android.view.ViewGroup/android.widget.ImageView[1]", m_driver);
				if (webElement37 != null)
				{
					webElement37.Click();
					Thread.Sleep(1000);
					webElement37 = null;
					localSource = GetLocalSource();
					webElement37 = null;
				}
			}
			if (localSource.Contains("\"Not interested\"") && localSource.Contains("\"Learn more\""))
			{
				IWebElement webElement38 = ADBHelperCCK.AppGetObject("//*[@text=\"Not interested\" or @content-desc=\"Not interested\"]", m_driver);
				if (webElement38 != null)
				{
					webElement38.Click();
					Thread.Sleep(1000);
					webElement38 = null;
					localSource = GetLocalSource();
					webElement38 = null;
				}
			}
			if (localSource.Contains("\"Gift your favorite videos to show your appreciation\""))
			{
				IWebElement webElement39 = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", m_driver);
				if (webElement39 != null)
				{
					webElement39.Click();
					Thread.Sleep(1000);
					webElement39 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Watch and follow\"") && localSource.Contains("\"Watch only\""))
			{
				IWebElement webElement40 = ADBHelperCCK.AppGetObject("//*[@text=\"Watch only\" or @content-desc=\"Watch only\"]", m_driver);
				if (webElement40 != null)
				{
					webElement40.Click();
					Thread.Sleep(1000);
					webElement40 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Send a gift, show a little extra praise\""))
			{
				IWebElement webElement41 = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", m_driver);
				if (webElement41 != null)
				{
					webElement41.Click();
					Thread.Sleep(1000);
					webElement41 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Add phone number\"") && localSource.Contains("\"Not now\""))
			{
				IWebElement webElement42 = ADBHelperCCK.AppGetObject("//*[@text=\"Not now\"]", m_driver);
				if (webElement42 != null)
				{
					webElement42.Click();
					Thread.Sleep(1000);
					webElement42 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Add phone number\"") && localSource.Contains("\"Send code\"") && localSource.Contains("you with people you may know"))
			{
				ADBHelperCCK.Back(p_DeviceId);
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Mời một người bạn tham gia TikTok để nhận lên đến"))
			{
				IWebElement webElement43 = ADBHelperCCK.AppGetObject("/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/com.lynx.tasm.ui.image.UIImage[2]", m_driver);
				if (webElement43 != null)
				{
					webElement43.Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("text=\"OK\""))
			{
				if (!localSource.Contains("Sync your Facebook friends"))
				{
					IWebElement webElement44 = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", m_driver);
					if (webElement44 != null)
					{
						webElement44.Click();
						Thread.Sleep(1000);
						webElement44 = null;
						localSource = GetLocalSource();
					}
				}
				else
				{
					List<IWebElement> list19 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"allow\") or contains(lower-case(@content-desc),\"allow\")]", m_driver);
					if (list19 != null)
					{
						list19[list19.Count - 1].Click();
						Thread.Sleep(1000);
						list19 = null;
						localSource = GetLocalSource();
					}
				}
			}
			if (localSource.Contains("text=\"Follow the host and get notified when they go LIVE\""))
			{
				ADBHelperCCK.Tap(p_DeviceId, screen.X / 3, screen.Y / 2);
			}
			if (localSource.Contains("Terms and Conditions"))
			{
				if (!localSource.Contains("Select to agree to all"))
				{
					if (localSource.Contains("Select both"))
					{
						IWebElement webElement45 = ADBHelperCCK.AppGetObject("//*[@text=\"Select both\" or @content-desc=\"Select both\"]", m_driver);
						if (webElement45 != null)
						{
							webElement45.Click();
							Thread.Sleep(1000);
							webElement45 = ADBHelperCCK.AppGetObject("//*[@text=\"Continue\" or @content-desc=\"Continue\"]", m_driver);
							if (webElement45 != null)
							{
								webElement45.Click();
								Thread.Sleep(1000);
							}
							localSource = GetLocalSource();
						}
					}
				}
				else
				{
					IWebElement webElement46 = ADBHelperCCK.AppGetObject("//android.widget.CheckBox", m_driver);
					if (webElement46 != null)
					{
						webElement46.Click();
						Thread.Sleep(1000);
						webElement46 = ADBHelperCCK.AppGetObject("//*[@text=\"Continue\" or @content-desc=\"Continue\"]", m_driver);
						if (webElement46 != null)
						{
							webElement46.Click();
							Thread.Sleep(1000);
						}
						localSource = GetLocalSource();
					}
				}
			}
			if (localSource.Contains("Swipe up for more"))
			{
				CCKDriver cCKDriver4 = new CCKDriver(p_DeviceId);
				CCKNode cCKNode4 = cCKDriver4.FindElement("//*[@text=\"Swipe up for more\"]", localSource);
				if (cCKNode4 != null)
				{
					int num7 = cCKNode4.Location.X + cCKNode4.Size.Width / 2;
					int y2 = cCKNode4.Location.Y + cCKNode4.Size.Height / 2;
					for (int n = 0; n < 4; n++)
					{
						ADBHelperCCK.Swipe(p_DeviceId, num7, y2, num7, screen.Y / 4);
						Thread.Sleep(500);
					}
					localSource = GetLocalSource();
					if (localSource.Contains("Swipe up for more"))
					{
						Utils.CCKLog("Swipe up for more", "Not Scroll " + DateTime.Now.ToString());
					}
				}
				cCKDriver4 = null;
				cCKNode4 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"Agree and continue\""))
			{
				IWebElement webElement47 = ADBHelperCCK.AppGetObject("//*[@text=\"Agree and continue\"]", m_driver);
				if (webElement47 != null)
				{
					webElement47.Click();
					Thread.Sleep(1000);
					webElement47 = ADBHelperCCK.AppGetObject("//*[@text=\"Skip\"]", m_driver);
					if (webElement47 != null)
					{
						webElement47.Click();
						Thread.Sleep(1000);
						for (int num8 = 0; num8 < 4; num8++)
						{
							ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
							Thread.Sleep(500);
						}
					}
					webElement47 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("your contacts to easily find people"))
			{
				IWebElement webElement48 = ADBHelperCCK.AppGetObject("//*[upper-case(@content-desc)=\"OK\" or upper-case(@text)=\"OK\" or upper-case(@content-desc)=\"NOT NOW\" or upper-case(@text)=\"NOT NOW\"]", m_driver);
				if (webElement48 != null)
				{
					webElement48.Click();
					Thread.Sleep(1000);
					webElement48 = null;
					sql.UpdateTrangThai(Info.Uid, "Login Success", "Live");
					Info.LoggedIn = true;
					localSource = GetLocalSource();
					webElement48 = ADBHelperCCK.AppGetObject("//*[upper-case(@content-desc)=\"DENY\" or upper-case(@text)=\"DENY\"]", m_driver);
					if (webElement48 != null)
					{
						webElement48.Click();
						Thread.Sleep(1000);
						webElement48 = null;
						localSource = GetLocalSource();
					}
				}
			}
			if (localSource.Contains("android:id/alwaysUse"))
			{
				IWebElement webElement49 = ADBHelperCCK.AppGetObject("//*[@resource-id=\"android:id/alwaysUse\"]", m_driver);
				if (webElement49 != null)
				{
					webElement49.Click();
					Thread.Sleep(1000);
					webElement49 = null;
					Thread.Sleep(1000);
					webElement49 = ADBHelperCCK.AppGetObject("//*[@text=\"Allow\"]", m_driver);
					if (webElement49 != null)
					{
						webElement49.Click();
						Thread.Sleep(1000);
						webElement49 = null;
						localSource = GetLocalSource();
					}
				}
			}
			if (localSource.Contains("TikTok isn't responding"))
			{
				IWebElement webElement50 = ADBHelperCCK.AppGetObject("//*[@text=\"Close app\"]", m_driver);
				if (webElement50 != null)
				{
					webElement50.Click();
					Thread.Sleep(1000);
					webElement50 = null;
					ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
					Thread.Sleep(10000);
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("\"Get better video recommendations\""))
			{
				IWebElement webElement51 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Skip\" or @text=\"Skip\"]", m_driver);
				if (webElement51 != null)
				{
					webElement51.Click();
					Thread.Sleep(1000);
					webElement51 = null;
					Thread.Sleep(1000);
				}
				webElement51 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Start watching\" or @text=\"Start watching\"]", m_driver);
				if (webElement51 != null)
				{
					webElement51.Click();
					Thread.Sleep(1000);
					webElement51 = null;
					localSource = GetLocalSource();
				}
			}
			if (localSource.Contains("Agree and continue"))
			{
				IWebElement webElement52 = ADBHelperCCK.AppGetObject("//*[lower-case(@content-desc)=\"agree and continue\" or lower-case(@text)=\"agree and continue\"]", m_driver);
				if (webElement52 != null)
				{
					webElement52.Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
					List<IWebElement> list20 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout/android.widget.TextView", m_driver);
					if (list20 != null && list20.Count > 0)
					{
						list20[rnd.Next(list20.Count)].Click();
						Thread.Sleep(1000);
						webElement52 = ADBHelperCCK.AppGetObject("//*[lower-case(@content-desc)=\"next\" or lower-case(@text)=\"next\"]", m_driver);
						if (webElement52 != null)
						{
							webElement52.Click();
							Thread.Sleep(1000);
							webElement52 = ADBHelperCCK.AppGetObject("//*[lower-case(@content-desc)=\"agree and continue\" or lower-case(@text)=\"agree and continue\"]", m_driver);
							if (webElement52 != null)
							{
								webElement52.Click();
								Thread.Sleep(1000);
							}
						}
					}
					webElement52 = null;
				}
				for (int num9 = 0; num9 < 3; num9++)
				{
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
				}
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Skip\"") && localSource.Contains("Choose your interests"))
			{
				IWebElement webElement53 = ADBHelperCCK.AppGetObject("//*[@text=\"Skip\"]", m_driver);
				if (webElement53 != null)
				{
					webElement53.Click();
					Thread.Sleep(1000);
					for (int num10 = 0; num10 < 5; num10++)
					{
						localSource = GetLocalSource();
						ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
					}
				}
				webElement53 = null;
				localSource = GetLocalSource();
			}
			if ((localSource.Contains("\"Allow\"") || localSource.Contains("\"ALLOW\"")) && localSource.Contains("TikTok to access your contacts"))
			{
				IWebElement webElement54 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"deny\"]", m_driver);
				if (webElement54 != null)
				{
					webElement54.Click();
					Thread.Sleep(1000);
					for (int num11 = 0; num11 < 3; num11++)
					{
						ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
					}
				}
				webElement54 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("account was logged out"))
			{
				IWebElement webElement55 = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", m_driver);
				if (webElement55 != null)
				{
					webElement55.Click();
					Thread.Sleep(1000);
					webElement55 = null;
				}
				if (Info.Uid != "")
				{
					Utils.ClearFolder(Application.StartupPath + "\\Authentication\\" + Info.Uid);
				}
				localSource = GetLocalSource();
			}
			if (localSource.Contains("\"Start watching\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Start watching\"]", m_driver)?.Click();
				Thread.Sleep(2000);
				localSource = GetLocalSource();
				for (int num12 = 0; num12 < 3; num12++)
				{
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
				}
				IWebElement webElement56 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("content-desc=\"Not now\""))
			{
				ADBHelperCCK.AppGetObject("//*[@content-desc=\"Not now\"]", m_driver)?.Click();
				Thread.Sleep(1000);
				IWebElement webElement57 = null;
				localSource = GetLocalSource();
			}
			if (localSource.ToLower().Contains("\"don't allow\""))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"Don't allow\"]", m_driver)?.Click();
				Thread.Sleep(1000);
				IWebElement webElement58 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains("text=\"IGNORE\"") && localSource.Contains("Warranty"))
			{
				ADBHelperCCK.AppGetObject("//*[@text=\"IGNORE\"]", m_driver)?.Click();
				Thread.Sleep(1000);
				IWebElement webElement59 = null;
				localSource = GetLocalSource();
			}
			if (localSource.Contains(CaChuaConstant.PACKAGE_NAME + ":id/dus") && localSource.Contains("favorite videos to show your appreciation"))
			{
				ADBHelperCCK.AppGetObject("//*[@resource-id=\"" + CaChuaConstant.PACKAGE_NAME + ":id/dus\"]", m_driver)?.Click();
				Thread.Sleep(1000);
				IWebElement webElement60 = null;
				for (int num13 = 0; num13 < 3; num13++)
				{
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
				}
				localSource = GetLocalSource();
			}
			if (localSource.Contains("Turn on your location"))
			{
				IWebElement webElement61 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"continue\"]", m_driver);
				if (webElement61 != null)
				{
					webElement61.Click();
					Thread.Sleep(1000);
					Thread.Sleep(2000);
					webElement61 = ADBHelperCCK.AppGetObject("//android.widget.Button[contains(lower-case(@text),\"allow\")]", m_driver);
					if (webElement61 != null)
					{
						webElement61.Click();
						Thread.Sleep(1000);
						Thread.Sleep(2000);
						ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
					}
				}
			}
			if (localSource.Contains("account was permanently"))
			{
				IWebElement webElement62 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"log out\" or lower-case(@content-desc)=\"log out\" or lower-case(@text)=\"dismiss\" or lower-case(@content-desc)=\"dismiss\"]", m_driver);
				if (webElement62 != null)
				{
					webElement62.Click();
					Thread.Sleep(1000);
					if (Info.Uid != "")
					{
						Info.LoggedIn = false;
						sql.UpdateTrangThai(Info.Uid, "Permanently banned", "Die");
						Utils.ClearFolder(Application.StartupPath + "\\Authentication\\" + Info.Uid);
						localSource = GetLocalSource();
					}
				}
			}
			if (localSource.Contains("text=\"Agree and continue\""))
			{
				IWebElement webElement63 = ADBHelperCCK.AppGetObject("//*[@text=\"Agree and continue\"]", m_driver);
				if (webElement63 != null)
				{
					webElement63.Click();
					Thread.Sleep(1000);
					localSource = GetLocalSource();
					List<IWebElement> list21 = ADBHelperCCK.AppGetObjects("//*[@resource-id=\"" + CaChuaConstant.PACKAGE_NAME + ":id/text\"]", m_driver);
					if (list21 != null)
					{
						List<string> list22 = new List<string>();
						foreach (IWebElement item in list21)
						{
							if (item.Text != "" && list22.Contains(item.Text))
							{
								item.Click();
								Thread.Sleep(1000);
							}
						}
						webElement63 = ADBHelperCCK.AppGetObject("//*[@text=\"Next\"  or @content-desc=\"Next\"]", m_driver);
						if (webElement63 != null && webElement63.Enabled)
						{
							webElement63.Click();
							Thread.Sleep(1000);
						}
						else
						{
							webElement63 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"skip\" or lower-case(@content-desc)=\"skip\"]", m_driver);
							if (webElement63 != null)
							{
								webElement63.Click();
								Thread.Sleep(1000);
							}
						}
					}
					else
					{
						webElement63 = ADBHelperCCK.AppGetObject("//*[@text=\"Skip\" or @content-desc=\"Skip\"]", m_driver);
						if (webElement63 != null)
						{
							webElement63.Click();
							Thread.Sleep(1000);
						}
					}
				}
				ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
				Thread.Sleep(1000);
				ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
				localSource = GetLocalSource();
			}
			if (localSource.Contains("resource-id=\"captcha-verify-image\"") || localSource.Contains("text=\"Drag the slider to fit the puzzle\"") || localSource.Contains("\"Drag the puzzle piece into place\""))
			{
				_ = DateTime.Now;
				Thread.Sleep(3000);
				ResolverCaptcha();
				localSource = GetLocalSource();
			}
			if (localSource.Contains("You can no longer appeal this decision"))
			{
				IWebElement webElement64 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"log out\" or lower-case(@content-desc)=\"log out\" or lower-case(@text)=\"dismiss\" or lower-case(@content-desc)=\"dismiss\"]", m_driver);
				if (webElement64 != null)
				{
					webElement64.Click();
					Thread.Sleep(1000);
					if (Info.Uid != "")
					{
						Info.LoggedIn = false;
						sql.UpdateTrangThai(Info.Uid, "Permanently banned", "Die");
						Utils.ClearFolder(Application.StartupPath + "\\Authentication\\" + Info.Uid);
						localSource = GetLocalSource();
					}
				}
			}
			if (!localSource.Contains(CaChuaConstant.PACKAGE_NAME) && localSource.Contains("com.vinsmart.launcher:id"))
			{
				ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				Thread.Sleep(2000);
				localSource = GetLocalSource();
			}
			return localSource;
		}

		internal int FollowBySuggested()
		{
			Utils.LogFunction("FollowBySuggested", "");
			GotoTab(Tabs.Profile);
			int num = 0;
			if (!File.Exists(CaChuaConstant.FOLLOWERS_SUGGESTED))
			{
				return 0;
			}
			try
			{
				FollowEntity followEntity = js.Deserialize<FollowEntity>(Utils.ReadTextFile(CaChuaConstant.FOLLOWERS_SUGGESTED));
				int num2 = rnd.Next(followEntity.FollowFrom, followEntity.FollowTo);
				int num3 = 0;
				GetPageSource();
				IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Profile\" or @content-desc=\"Profile\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(2000);
					webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Follower\" or @content-desc=\"Follower\" or @text=\"Followers\" or @content-desc=\"Followers\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(2000);
						webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Suggested\" or @content-desc=\"Suggested\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(5000);
							GetPageSource();
							DateTime dateTime = DateTime.Now.AddSeconds(rnd.Next(followEntity.WorkingFrom, followEntity.WorkingTo));
							while (dateTime > DateTime.Now)
							{
								List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[@text=\"Follow\" or @content-desc=\"Follow\"]", m_driver);
								if (list == null)
								{
									break;
								}
								for (int i = 0; i < list.Count; i++)
								{
									try
									{
										list[i].Click();
										num++;
									}
									catch
									{
									}
									Thread.Sleep(1000 * rnd.Next(followEntity.DelayFrom, followEntity.DelayTo));
									if (num2 >= num3)
									{
										num3++;
										continue;
									}
									return num;
								}
								ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4, 300);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("FollowBySuggested", ex.Message);
			}
			return num;
		}

		public void Delay()
		{
			if (File.Exists(CaChuaConstant.DELAY))
			{
				RangeValue rangeValue = new JavaScriptSerializer().Deserialize<RangeValue>(Utils.ReadTextFile(CaChuaConstant.DELAY));
				int num = new Random().Next(rangeValue.From, rangeValue.To);
				for (int num2 = 1000 * num * 60; num2 > 0; num2 -= 5000)
				{
					GridViewMessageLog(p_DeviceId, "Chờ " + num2 / 1000 + " giây", Color.Green);
					Thread.Sleep(5000);
				}
			}
		}

		private string ConnectWifi(string deviceid)
		{
			GridViewMessageLog(deviceid, "Mở cài đặt Wifi", Color.Green);
			ADBHelperCCK.ExecuteCMD(deviceid, " shell am start -a android.settings.WIRELESS_SETTINGS");
			Thread.Sleep(1000);
			string pageSource = GetPageSource();
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Wi‑Fi\"]", m_driver);
			if (webElement != null)
			{
				if (webElement.Text == "OFF")
				{
					webElement.Click();
				}
				Thread.Sleep(1000);
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Wi‑Fi\"]", m_driver);
				ADBHelperCCK.TapLong(deviceid, webElement.Location.X + webElement.Size.Width / 2, webElement.Location.Y + webElement.Size.Height / 2);
				Thread.Sleep(2000);
				pageSource = GetPageSource();
				List<string> list = Utils.ReadTextFile(CaChuaConstant.WIFI).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
				if (list != null && list.Count == 0)
				{
					return "";
				}
				string[] array = list[new Random().Next(list.Count)].Split("|;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string text = ((array.Length != 0) ? array[0] : "");
				string text2 = ((array.Length > 1) ? array[1] : "");
				GridViewMessageLog(deviceid, "Đăng nhập WIFI " + text, Color.Green);
				IWebElement webElement2 = ADBHelperCCK.WaitMeCount("//*[lower-case(@text)=\"" + text.ToLower() + "\"]", m_driver);
				if (webElement2 == null)
				{
					GridViewMessageLog(deviceid, "Không tìm thấy Wifi: " + text, Color.Red);
				}
				else
				{
					webElement2.Click();
					Thread.Sleep(1000);
					pageSource = GetPageSource();
					if (pageSource.Contains("Enter password"))
					{
						IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Enter password\"]", m_driver);
						if (webElement3 != null)
						{
							webElement3.SendKeys(text2);
							Thread.Sleep(1000);
							ADBHelperCCK.AppGetObject("//*[@text=\"Connect\" or @content-desc=\"Connect\"]", m_driver)?.Click();
						}
					}
					pageSource = GetPageSource();
					while (!pageSource.Contains("text=\"Connected\""))
					{
						Thread.Sleep(2000);
						pageSource = GetPageSource();
						ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.android.settings:id/wifi_menu_loadinglist\"]", m_driver)?.Click();
						if (pageSource.Contains("text=\"Saved\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(@content-desc,',Saved,')]", m_driver)?.Click();
						}
					}
					GridViewMessageLog(deviceid, "Kết nối WIFI thành công" + text, Color.Green);
					Thread.Sleep(1000);
				}
			}
			return pageSource;
		}

		private void ShowMessageOnGrid(string searchValue, string msg, DataGridView dataGridViewPhone, Form main)
		{
			try
			{
				if (CurrentRow != null)
				{
					CurrentRow.Cells["Log"].Value = msg;
				}
			}
			catch
			{
			}
			main.Invoke((Action)delegate
			{
				int num = -1;
				bool allowUserToAddRows = dataGridViewPhone.AllowUserToAddRows;
				dataGridViewPhone.AllowUserToAddRows = false;
				DataGridViewRow dataGridViewRow = (from DataGridViewRow r in dataGridViewPhone.Rows
					where r.Cells["Phone"].Value.ToString().Equals(searchValue)
					select r).First();
				num = dataGridViewRow.Index;
				dataGridViewPhone.AllowUserToAddRows = allowUserToAddRows;
				if (num > -1 && num < dataGridViewPhone.Rows.Count)
				{
					dataGridViewPhone.Rows[num].Cells["Status"].Value = msg;
					dataGridViewPhone.Rows[num].DefaultCellStyle.ForeColor = Color.Green;
				}
			});
		}

		private void ChangeGridViewDevice(Form mainForm, string searchValue, string msg, Color c, string findColmn = "Phone", string columnName = "Status")
		{
			mainForm.Invoke((Action)delegate
			{
				int num = -1;
				bool allowUserToAddRows = dataGridViewPhone.AllowUserToAddRows;
				dataGridViewPhone.AllowUserToAddRows = false;
				DataGridViewRow dataGridViewRow = (from DataGridViewRow r in dataGridViewPhone.Rows
					where r.Cells[findColmn].Value.ToString().Equals(searchValue)
					select r).First();
				num = dataGridViewRow.Index;
				dataGridViewPhone.AllowUserToAddRows = allowUserToAddRows;
				if (num > -1 && num < dataGridViewPhone.Rows.Count)
				{
					dataGridViewPhone.Rows[num].Cells[columnName].Value = msg;
					dataGridViewPhone.Rows[num].DefaultCellStyle.ForeColor = c;
				}
			});
		}

		public bool RegisterMemoTiktokByEmail(DataGridView gridPhone, Form main, int CategoryId, RegNickEntity entity)
		{
			Utils.LogFunction("RegisterMemoTiktokByEmail", "");
			CloseCaptcha = false;
			if (entity.Register8AccountOnly)
			{
				SwitchAccount();
				ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			}
			else
			{
				ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				Thread.Sleep(2000);
				ADBHelperCCK.SetStoragePermission(p_DeviceId);
				ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				Thread.Sleep(5000);
				IWebElement webElement = ADBHelperCCK.WaitMe("//*[@text=\"Agree and continue\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
				}
			}
			Info = new TTItems(initName: true);
			ShowMessageOnGrid(p_DeviceId, "Start Reg", gridPhone, main);
			CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
			VNNameEntity vNNameEntity = new VNNameEntity();
			if (File.Exists(CaChuaConstant.VN_Name))
			{
				vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
				Info.FirstName = vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)];
				Info.LastName = vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)];
				Info.Uid = Utils.UnicodeToKoDau(Info.FirstName + Info.LastName);
				Info.Brand = Utils.ReadTextFile(Application.StartupPath + "\\Devices\\CCKInfo_" + p_DeviceId + ".txt").Replace(Environment.NewLine, "|");
			}
			Thread.Sleep(1000);
			string pageSource = GetPageSource();
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
			DateTime dateTime = DateTime.Now.AddMinutes(30.0);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			int num = 15;
			bool flag = false;
			try
			{
				ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4);
				pageSource = GetPageSource().ToLower();
				while (dateTime > DateTime.Now && num-- > 0)
				{
					if (pageSource.Contains("text=\"log in or sign up\""))
					{
						IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"log in or sign up\"]", m_driver);
						if (webElement2 != null)
						{
							webElement2.Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Log in or sign up", gridPhone, main);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("\"profile\""))
					{
						List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"profile\")]", m_driver);
						if (list != null && list.Count > 0)
						{
							list[list.Count - 1].Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Profile click", gridPhone, main);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("text=\"@"))
							{
								list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"@\")]", m_driver);
								if (list != null)
								{
									pageSource = GetPageSource().ToLower();
									Info.Uid = list[0].Text.TrimStart('@');
									Info.UserAgent = p_DeviceId;
									sql.Insert(Info, CategoryId);
									Rename(Info.LastName + " " + Info.FirstName);
									ShowMessageOnGrid(p_DeviceId, "Rename", gridPhone, main);
									BackupAccountTiktok();
									ChangeEmail(change: false);
								}
								return true;
							}
						}
					}
					if (pageSource.ToLower().Contains("sign up for tiktok") && pageSource.ToLower().Contains("use phone or email"))
					{
						List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"use phone or email\")]", m_driver);
						if (list2 != null && list2.Count > 0)
						{
							list2[list2.Count - 1].Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Use phone or email", gridPhone, main);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.ToLower().Contains("text=\"email\""))
					{
						List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"email\")]", m_driver);
						if (list3 != null && list3.Count > 0)
						{
							list3[list3.Count - 1].Click();
							Thread.Sleep(5000);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("Maximum number of attempts reached. Try again later".ToLower()))
							{
								return false;
							}
						}
					}
					if (pageSource.ToLower().Contains("create username"))
					{
						List<IWebElement> list4 = WaitMes(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"));
						if (list4 != null)
						{
							ShowMessageOnGrid(p_DeviceId, "Create username", gridPhone, main);
							Info.UidLayBai = Utils.UnicodeToKoDau(Info.LastName + Info.FirstName + rnd.Next(1000, 9999));
							ADBHelperCCK.Tap(p_DeviceId, list4[0].Location.X + list4[0].Size.Width * 98 / 100, list4[0].Location.Y + list4[0].Size.Height / 2);
							Thread.Sleep(1000);
							ADBHelperCCK.InputTextNormal(p_DeviceId, Info.UidLayBai);
							Thread.Sleep(5000);
							pageSource = GetPageSource();
							if (pageSource.Contains("Try a suggested username"))
							{
								IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"skip\")]", m_driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(2000);
								}
							}
							List<IWebElement> list5 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"sign up\")]", m_driver);
							if (list5 != null && list5.Count > 0)
							{
								list5[list5.Count - 1].Click();
								ShowMessageOnGrid(p_DeviceId, "Sign up", gridPhone, main);
								Thread.Sleep(1000);
								pageSource = GetPageSource().ToLower();
								sql.Insert(Info, CategoryId);
								Rename(Info.LastName + " " + Info.FirstName);
								ChangeEmail(change: false);
								PublicInfo();
								BackupAccountTiktok();
								return true;
							}
						}
						pageSource = GetPageSource().ToLower();
					}
					if (pageSource.Contains("have an account? sign up\""))
					{
						IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"don’t have an account? sign up\"]", m_driver);
						if (webElement4 != null)
						{
							ADBHelperCCK.Tap(p_DeviceId, webElement4.Location.X + webElement4.Size.Width * 9 / 10, webElement4.Location.Y + webElement4.Size.Height / 2);
							Thread.Sleep(1000);
						}
						pageSource = GetPageSource().ToLower();
					}
					if (pageSource.Contains("\"sign up\""))
					{
						pageSource = GetPageSource();
						if ((pageSource.Contains("Send code") || pageSource.Contains("Next")) && pageSource.Contains("\"Email\""))
						{
							ShowMessageOnGrid(p_DeviceId, "Get Email", gridPhone, main);
							List<CCKNode> list6 = cCKDriver.FindElements("//*[@text=\"Email\"]", GetPageSource());
							list6[list6.Count - 1].Click();
							Thread.Sleep(1000);
							Account email = emailUti.GetEmail(Info);
							if (email.accounts == null || email.accounts.ToString().Length <= 2)
							{
								ShowMessageOnGrid(p_DeviceId, "Hết Email", gridPhone, main);
								return false;
							}
							ShowMessageOnGrid(p_DeviceId, "Email" + email.accounts, gridPhone, main);
							File.AppendAllLines("Config\\backupEmail.txt", new List<string> { email.accounts });
							Info.Email = email.User;
							Info.PassEmail = email.Pass;
							pageSource = GetPageSource();
							List<IWebElement> list7 = ADBHelperCCK.AppGetObjects("//*[@text=\"Email address\"]", m_driver);
							if (list7 != null && list7.Count > 0)
							{
								list7[list6.Count - 1].Click();
								list7[list6.Count - 1].SendKeys(Info.Email);
								Thread.Sleep(1000);
								List<IWebElement> list8 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
								if (list8 != null)
								{
									ADBHelperCCK.Tap(p_DeviceId, list8[list8.Count - 1].Location.X + list8[list8.Count - 1].Size.Width / 4, list8[list8.Count - 1].Location.Y);
									Thread.Sleep(1000);
									ADBHelperCCK.WaitMe("//*[@text=\"Enter password\"]", m_driver);
									pageSource = GetPageSource();
									WaitCaptchaXoay(m_driver);
									while (pageSource.Contains("\"Next\"") && pageSource.Contains("\"Email\""))
									{
										list8 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
										if (list8 != null)
										{
											ADBHelperCCK.Tap(p_DeviceId, list8[list8.Count - 1].Location.X + list8[list8.Count - 1].Size.Width / 4, list8[list8.Count - 1].Location.Y);
											Thread.Sleep(5000);
										}
										pageSource = GetPageSource().ToLower();
										if (!pageSource.Contains("maximum number of attempts reached. try again later."))
										{
											Thread.Sleep(1000);
											continue;
										}
										return false;
									}
								}
							}
						}
					}
					if (pageSource.ToLower().Contains("\"Enter password\"".ToLower()))
					{
						List<CCKNode> list9 = cCKDriver.FindElements("//*[@text=\"Enter password\"]".ToLower(), pageSource.ToLower());
						if (list9 != null)
						{
							ShowMessageOnGrid(p_DeviceId, "Enter password", gridPhone, main);
							ADBHelperCCK.Tap(p_DeviceId, list9[0].Location.X - list9[0].Size.Width / 6, list9[0].Location.Y);
							string text = ((entity.PasswordDefault != "") ? entity.PasswordDefault : CreatePassword(10, uppercase: true, specialCharactor: true));
							ADBHelperCCK.InputTextNormal(p_DeviceId, text);
							Info.Pass = text;
							Thread.Sleep(1000);
							File.AppendAllLines("tiktok.txt", new List<string> { Info.Email + "|" + text + "|" + Info.Email + "|" + Info.PassEmail + "|" + Info.UidLayBai });
							List<IWebElement> list10 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
							if (list10 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, list10[list10.Count - 1].Location.X + list10[list10.Count - 1].Size.Width / 4, list10[list10.Count - 1].Location.Y);
								Thread.Sleep(2000);
							}
						}
						WaitCaptchaXoay(m_driver);
					}
					if (pageSource.Contains("\"Create nickname\""))
					{
						bool flag2 = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.EMAIL_NAME));
						IWebElement webElement5 = ADBHelperCCK.AppGetObject("//android.widget.ImageView[@content-desc=\"Clear text\"]", m_driver);
						if (webElement5 != null && flag2)
						{
							webElement5.Click();
							Thread.Sleep(1000);
						}
						Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_RENAME), 30));
						webElement5 = ADBHelperCCK.AppGetObject("//android.widget.EditText", m_driver);
						if (webElement5 != null)
						{
							webElement5.Click();
							webElement5.SendKeys((Info.FirstName + " " + Info.LastName).Trim());
							Thread.Sleep(1000);
						}
						IWebElement webElement6 = ADBHelperCCK.AppGetObject("//*[@text=\"Confirm\" or @content-desc=\"Confirm\"]", m_driver);
						if (webElement6 != null)
						{
							webElement6.Click();
							Thread.Sleep(5000);
							pageSource = GetPageSource();
						}
						if (pageSource.Contains("\"Skip\""))
						{
							webElement6 = ADBHelperCCK.AppGetObject("//*[@text=\"Skip\" or @content-desc=\"Skip\"]", m_driver);
							if (webElement6 != null)
							{
								webElement6.Click();
								Thread.Sleep(5000);
								pageSource = GetPageSource();
							}
						}
					}
					pageSource = GetPageSource().ToLower();
					if (!pageSource.Contains("maximum number of attempts reached"))
					{
						if (!pageSource.Contains("looks like you're not eligible for TikTok".ToLower()))
						{
							if (pageSource.Contains("your birthday") && pageSource.Contains("sign up") && !flag)
							{
								flag = true;
								ShowMessageOnGrid(p_DeviceId, "Your birthday", gridPhone, main);
								List<CCKNode> list11 = cCKDriver.FindElements("//android.widget.SeekBar".ToLower(), pageSource);
								if (list11 != null && list11.Count >= 3)
								{
									list11 = list11.Skip(list11.Count - 3).Take(3).ToList();
									_ = list11.Count - 1;
									int x = list11[0].Location.X;
									int y = list11[0].Location.Y;
									_ = list11[0].Size.Width;
									int height = list11[0].Size.Height;
									ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
									Thread.Sleep(1000);
									x = list11[1].Location.X;
									y = list11[1].Location.Y;
									_ = list11[1].Size.Width;
									height = list11[1].Size.Height;
									ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
									Thread.Sleep(1000);
									x = list11[2].Location.X;
									y = list11[2].Location.Y;
									_ = list11[2].Size.Width;
									height = list11[2].Size.Height;
									int num2 = new Random().Next(entity.AgeFrom / 2, entity.AgeTo / 2);
									for (int i = 0; i < num2; i++)
									{
										ADBHelperCCK.Swipe(p_DeviceId, x, y - height / 3, x, y + height / 3, 500);
									}
									Thread.Sleep(2000);
									pageSource = GetPageSource();
									int num3 = 0;
									int num4 = 5;
									while (pageSource.Contains("\"Next\"") && num3++ < num4)
									{
										List<IWebElement> list12 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
										if (list12 != null)
										{
											ShowMessageOnGrid(p_DeviceId, "Your birthday Next", gridPhone, main);
											ADBHelperCCK.Tap(p_DeviceId, list12[list12.Count - 1].Location.X + list12[list12.Count - 1].Size.Width / 4, list12[list12.Count - 1].Location.Y);
											Thread.Sleep(5000);
										}
										pageSource = GetLocalSource();
										if (pageSource.Contains("Too many attempts") || pageSource.Contains("Sorry, looks like you're not eligible for TikTok"))
										{
											File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
											Thread.Sleep(1800000);
											return false;
										}
									}
									if (num3 == num4)
									{
										return false;
									}
								}
							}
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("code was emailed to"))
							{
								ShowMessageOnGrid(p_DeviceId, "Get Code Email", gridPhone, main);
								string code = emailUti.GetCode(Info.Email, Info.PassEmail);
								if (code == "")
								{
									pageSource = GetPageSource();
									ADBHelperCCK.AppGetObject("//*[@text=\"Resend code\"]", m_driver)?.Click();
									code = emailUti.GetCode(Info.Email, Info.PassEmail);
								}
								pageSource = GetPageSource();
								List<IWebElement> list13 = ADBHelperCCK.AppGetObjects("//android.widget.EditText", m_driver);
								if (list13 != null && list13.Count == 1)
								{
									list13[0].SendKeys(code);
									Thread.Sleep(5000);
									pageSource = GetPageSource().ToLower();
									if (pageSource.Contains("text=\"too many attempts. try again later.\""))
									{
										return false;
									}
									for (int j = 0; j < 5; j++)
									{
										ADBHelperCCK.Back(p_DeviceId);
									}
									pageSource = GetPageSource().ToLower();
									GotoTab(Tabs.Profile);
									pageSource = GetPageSource().ToLower();
									if (pageSource.Contains("text=\"@"))
									{
										List<IWebElement> list14 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"@\")]", m_driver);
										if (list14 != null)
										{
											Info.Uid = list14[0].Text.TrimStart('@');
										}
									}
									ShowMessageOnGrid(p_DeviceId, "Created Account", gridPhone, main);
									sql.Insert(Info, CategoryId);
									Rename(Info.LastName + " " + Info.FirstName);
									PublicInfo();
									return true;
								}
							}
							if (pageSource.Contains("manage account privacy"))
							{
								IWebElement webElement7 = ADBHelperCCK.AppGetObject("//*[@text=\"Public account\" or @content-desc=\"Public account\"]", m_driver);
								if (webElement7 != null)
								{
									webElement7.Click();
									Thread.Sleep(2000);
									pageSource = GetPageSource().ToLower();
									webElement7 = ADBHelperCCK.AppGetObject("//android.widget.Button[contains(@text,\"Public account\") or contains(@content-desc,\"Public account\")]", m_driver);
									if (webElement7 != null)
									{
										webElement7.Click();
										Thread.Sleep(1000);
										webElement7 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"OK\" or upper-case(@content-desc)=\"OK\"]", m_driver);
										if (webElement7 != null)
										{
											webElement7.Click();
											Thread.Sleep(1000);
										}
										webElement7 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"NEXT\" or upper-case(@content-desc)=\"NEXT\"]", m_driver);
										if (webElement7 != null)
										{
											webElement7.Click();
											Thread.Sleep(1000);
										}
									}
								}
							}
							if (pageSource.Contains("Too many attempts. Try again later"))
							{
								ShowMessageOnGrid(p_DeviceId, "Too many attempts", gridPhone, main);
								if (!string.IsNullOrWhiteSpace(Info.Email))
								{
									File.AppendAllLines("Config\\too_many_attempts_email.txt", new List<string> { Info.Email + "|" + Info.PassEmail });
								}
								ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								return false;
							}
							continue;
						}
						File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
						return false;
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Register ", ex.Message);
			}
			return false;
		}

		public bool RegisterTiktokLiteByGmailApp(DataGridView gridPhone, Form main, int CategoryId, RegNickEntity entity)
		{
			Utils.LogFunction("RegisterTiktokLiteByGmailApp", "");
			RemoveSystemAccount(m_driver);
			ADBHelperCCK.CloseApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME_LITE);
			Thread.Sleep(2000);
			ADBHelperCCK.SetPortrait(p_DeviceId);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME_LITE);
			Thread.Sleep(5000);
			Info = new TTItems(initName: true);
			ShowMessageOnGrid(p_DeviceId, "Start Reg", gridPhone, main);
			CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
			VNNameEntity vNNameEntity = new VNNameEntity();
			if (File.Exists(CaChuaConstant.VN_Name))
			{
				vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
				Info.FirstName = vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)];
				Info.LastName = vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)];
				Info.Uid = Utils.UnicodeToKoDau(Info.FirstName + Info.LastName);
				Info.Brand = Utils.ReadTextFile(Application.StartupPath + "\\Devices\\CCKInfo_" + p_DeviceId + ".txt").Replace(Environment.NewLine, "|");
			}
			Thread.Sleep(1000);
			string pageSource = GetPageSource();
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME_LITE);
			Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
			DateTime dateTime = DateTime.Now.AddMinutes(30.0);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			int num = 15;
			bool flag = false;
			try
			{
				ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4);
				pageSource = GetPageSource().ToLower();
				while (dateTime > DateTime.Now && num-- > 0)
				{
					if (pageSource.Contains("text=\"log in or sign up\""))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"log in or sign up\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Log in or sign up", gridPhone, main);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("\"Me\"".ToLower()))
					{
						List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"me\")]", m_driver);
						if (list != null && list.Count > 0)
						{
							list[list.Count - 1].Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Profile click", gridPhone, main);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("\"continue as"))
							{
								IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"continue as\")]", m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(5000);
								}
							}
							if (pageSource.Contains("text=\"@"))
							{
								list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"@\")]", m_driver);
								if (list != null)
								{
									pageSource = GetPageSource().ToLower();
									Info.Uid = list[0].Text.TrimStart('@');
									Info.Pass = "";
									Info.UserAgent = p_DeviceId;
									RenameLite(Info.LastName + " " + Info.FirstName);
									ShowMessageOnGrid(p_DeviceId, "Rename", gridPhone, main);
									GetCookies(CaChuaConstant.PACKAGE_NAME_LITE);
									sql.Insert(Info, CategoryId);
									RegChangePassLite();
									ChangeAvatarLite();
									UpvideoLite();
								}
								return true;
							}
						}
					}
					if (pageSource.Contains("already have an account? log in"))
					{
						IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"already have an account? log in\"]", m_driver);
						if (webElement3 != null)
						{
							ADBHelperCCK.Tap(p_DeviceId, webElement3.Location.X + webElement3.Size.Width * 9 / 10, webElement3.Location.Y + webElement3.Size.Height / 2);
							Thread.Sleep(1000);
						}
						pageSource = GetPageSource().ToLower();
						if (pageSource.Contains("Continue with Google".ToLower()))
						{
							List<CCKNode> list2 = cCKDriver.FindElements(GetFunctionByKey("a32bce59c5ab46c6b5796098dd2c6ff6"), pageSource);
							if (list2 != null && list2.Count > 0)
							{
								list2[0].Click();
								Thread.Sleep(2000);
								pageSource = GetPageSource();
								if (pageSource.Contains("\"Choose an account\""))
								{
									RemoveSystemAccount(m_driver);
									return false;
								}
							}
							IWebElement webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("3e2facd7b419470a91d7cbe6a10087d6"), m_driver, 120);
							if (webElement4 == null)
							{
								pageSource = GetPageSource();
								if (!pageSource.Contains("\"Something went wrong\"") && pageSource.Contains("\"Choose an account\""))
								{
									return false;
								}
								return false;
							}
							Thread.Sleep(3000);
							webElement4.Click();
							EmailUtils emailUtils = new EmailUtils();
							while (true)
							{
								Account email = emailUtils.GetEmail(Info, EmailRegisterType.GoogleAppDomain);
								if (email.User != "")
								{
									Info.Email = email.User;
									Info.PassEmail = email.Pass;
									webElement4.SendKeys(email.User);
									Thread.Sleep(1000);
									ADBHelperCCK.WaitMe("//*[@text=\"Forgot password?\"]", m_driver);
									webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("12ff64449a134d1f9afdd50bb6433d8c"), m_driver, 30);
									if (webElement4 == null)
									{
										break;
									}
									Thread.Sleep(1000);
									webElement4.Click();
									Thread.Sleep(5000);
									pageSource = GetPageSource();
									ADBHelperCCK.WaitMeCount(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver, 10);
									Thread.Sleep(2000);
									pageSource = GetPageSource();
									if (pageSource.Contains("\"Account deleted\""))
									{
										IWebElement webElement5 = ADBHelperCCK.AppGetObject(By.XPath("//*[@text=\"Use another account\"]"), m_driver);
										if (webElement5 != null)
										{
											webElement5.Click();
											Thread.Sleep(1000);
											continue;
										}
									}
									if (!pageSource.Contains("text=\"Show password\""))
									{
									}
									if (pageSource.Contains("\"Got it\""))
									{
										IWebElement webElement6 = ADBHelperCCK.AppGetObject(GetFunctionByKey("70b58c5a4fb04a29b66077117e8ae18e"), m_driver);
										if (webElement6 != null)
										{
											webElement6.Click();
											Thread.Sleep(5000);
										}
									}
									pageSource = GetPageSource();
									List<IWebElement> list3 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
									pageSource = GetPageSource();
									if (list3 != null && list3.Count > 0 && !pageSource.Contains("text=\"Create account\""))
									{
										list3[0].Click();
										Thread.Sleep(1000);
										list3[0].SendKeys(email.Pass);
										Thread.Sleep(2000);
										pageSource = GetPageSource();
										webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("6b82b5de615c41db8a62d30f0ce73e69"), m_driver, 30);
										if (webElement4 != null)
										{
											webElement4.Click();
											Thread.Sleep(2000);
										}
										ADBHelperCCK.WaitMe("//*[@text=\"Welcome\"]", m_driver);
										Point screenResolution2 = ADBHelperCCK.GetScreenResolution(p_DeviceId);
										for (int i = 0; i < 5; i++)
										{
											ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 2, screenResolution2.Y / 2, screenResolution2.X / 2, screenResolution2.Y / 4);
										}
										pageSource = GetPageSource();
										if (!pageSource.Contains("\"Continue\""))
										{
											webElement4 = ADBHelperCCK.AppGetObject(GetFunctionByKey("b612da80bbc14372b9c76d12cee41582"), m_driver);
											if (webElement4 == null)
											{
												webElement4 = ADBHelperCCK.AppGetObject("//android.widget.Button", m_driver);
											}
											DateTime dateTime2 = DateTime.Now.AddMinutes(5.0);
											Thread.Sleep(5000);
											while (DateTime.Now < dateTime2)
											{
												webElement4 = ADBHelperCCK.WaitMe(GetFunctionByKey("a97e0eb041cb484bacfbeb43b8b5434c"), m_driver);
												if (webElement4 == null)
												{
													List<IWebElement> list4 = ADBHelperCCK.AppGetObjects("//android.widget.Button", m_driver);
													if (list4 != null && list4.Count > 0)
													{
														webElement4 = list4[list4.Count - 1];
														Thread.Sleep(2000);
													}
												}
												if (webElement4 != null && webElement4.Enabled)
												{
													webElement4.Click();
													Thread.Sleep(5000);
												}
												pageSource = GetPageSource();
												if (!pageSource.Contains("text=\"Enter a password\""))
												{
													if (!pageSource.Contains("your birthday"))
													{
														if (pageSource.Contains("text=\"Me\""))
														{
															break;
														}
														ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 2, screenResolution2.Y / 2, screenResolution2.X / 2, screenResolution2.Y / 4);
														Thread.Sleep(2000);
														continue;
													}
													goto IL_091b;
												}
												return false;
											}
										}
										else
										{
											webElement4 = ADBHelperCCK.AppGetObject(GetFunctionByKey("db05451af8644111879336a695bd3d92"), m_driver);
											if (webElement4 != null)
											{
												webElement4.Click();
												Thread.Sleep(10000);
											}
										}
										pageSource = GetPageSource();
										if (!pageSource.Contains("your birthday"))
										{
											pageSource = GetPageSource().ToLower();
										}
										break;
									}
									if (!pageSource.Contains("text=\"Create account\""))
									{
										break;
									}
									emailUtils.ReturnStock(email);
									return false;
								}
								return false;
							}
						}
					}
					goto IL_091b;
					IL_091b:
					pageSource = GetPageSource().ToLower();
					if (pageSource.Contains("your birthday?\""))
					{
						pageSource = GetPageSource();
						if ((pageSource.Contains("Send code") || pageSource.Contains("Next")) && pageSource.Contains("\"Email\""))
						{
							ShowMessageOnGrid(p_DeviceId, "Get Email", gridPhone, main);
							List<CCKNode> list5 = cCKDriver.FindElements("//*[@text=\"Email\"]", GetPageSource());
							list5[list5.Count - 1].Click();
							Thread.Sleep(1000);
							Account email2 = emailUti.GetEmail(Info);
							if (email2.accounts == null || email2.accounts.ToString().Length <= 2)
							{
								ShowMessageOnGrid(p_DeviceId, "Hết Email", gridPhone, main);
								RunNextAccount = false;
								return false;
							}
							ShowMessageOnGrid(p_DeviceId, "Email" + email2.accounts, gridPhone, main);
							File.AppendAllLines("Config\\backupEmail.txt", new List<string> { email2.accounts });
							Info.Email = email2.User;
							Info.PassEmail = email2.Pass;
							pageSource = GetPageSource();
							List<IWebElement> list6 = ADBHelperCCK.AppGetObjects("//*[@text=\"Email address\"]", m_driver);
							if (list6 != null && list6.Count > 0)
							{
								list6[list5.Count - 1].Click();
								list6[list5.Count - 1].SendKeys(Info.Email);
								Thread.Sleep(1000);
								List<IWebElement> list7 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
								if (list7 != null)
								{
									ADBHelperCCK.Tap(p_DeviceId, list7[list7.Count - 1].Location.X + list7[list7.Count - 1].Size.Width / 4, list7[list7.Count - 1].Location.Y);
									Thread.Sleep(1000);
									ADBHelperCCK.WaitMe("//*[@text=\"Enter password\"]", m_driver);
									pageSource = GetPageSource();
									WaitCaptchaXoay(m_driver);
									while (pageSource.Contains("\"Next\"") && pageSource.Contains("\"Email\""))
									{
										list7 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
										if (list7 != null)
										{
											ADBHelperCCK.Tap(p_DeviceId, list7[list7.Count - 1].Location.X + list7[list7.Count - 1].Size.Width / 4, list7[list7.Count - 1].Location.Y);
											Thread.Sleep(5000);
										}
										pageSource = GetPageSource().ToLower();
										if (!pageSource.Contains("maximum number of attempts reached. try again later."))
										{
											Thread.Sleep(1000);
											continue;
										}
										return false;
									}
								}
							}
						}
					}
					pageSource = GetPageSource().ToLower();
					if (!pageSource.Contains("maximum number of attempts reached"))
					{
						if (!pageSource.Contains("looks like you're not eligible for TikTok".ToLower()))
						{
							if (pageSource.Contains("your birthday") && !flag)
							{
								flag = true;
								ShowMessageOnGrid(p_DeviceId, "Your birthday", gridPhone, main);
								pageSource = GetPageSource();
								List<CCKNode> list8 = cCKDriver.FindElements("//android.view.View", pageSource);
								if (list8 != null && list8.Count >= 3)
								{
									list8 = list8.Skip(list8.Count - 3).Take(3).ToList();
									_ = list8.Count - 1;
									int x = list8[0].Location.X;
									int y = list8[0].Location.Y;
									_ = list8[0].Size.Width;
									int height = list8[0].Size.Height;
									ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
									Thread.Sleep(1000);
									x = list8[1].Location.X;
									y = list8[1].Location.Y;
									_ = list8[1].Size.Width;
									height = list8[1].Size.Height;
									ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
									Thread.Sleep(1000);
									x = list8[2].Location.X;
									y = list8[2].Location.Y;
									_ = list8[2].Size.Width;
									height = list8[2].Size.Height;
									int num2 = new Random().Next(entity.AgeFrom / 2, entity.AgeTo / 2);
									for (int j = 0; j < num2; j++)
									{
										ADBHelperCCK.Swipe(p_DeviceId, x, y - height / 3, x, y + height / 3, 500);
									}
									Thread.Sleep(2000);
									pageSource = GetPageSource();
									int num3 = 0;
									int num4 = 5;
									while (pageSource.Contains("\"Next\"") && num3++ < num4)
									{
										List<IWebElement> list9 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
										if (list9 != null)
										{
											ShowMessageOnGrid(p_DeviceId, "Your birthday Next", gridPhone, main);
											ADBHelperCCK.Tap(p_DeviceId, list9[list9.Count - 1].Location.X + list9[list9.Count - 1].Size.Width / 4, list9[list9.Count - 1].Location.Y);
											Thread.Sleep(5000);
											WaitMe("//*[@text=\"Create profile name\"]", 15);
										}
										pageSource = GetLocalSource();
										if (!pageSource.Contains("Too many attempts") && !pageSource.Contains("Sorry, looks like you're not eligible for TikTok"))
										{
											if (!pageSource.Contains("\"Create profile name\""))
											{
												continue;
											}
											IWebElement webElement7 = ADBHelperCCK.AppGetObject("//android.widget.EditText", m_driver);
											if (webElement7 != null && webElement7.Text == "")
											{
												webElement7.Clear();
												Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_RENAME), 30));
												webElement7 = ADBHelperCCK.AppGetObject("//android.widget.EditText", m_driver);
												if (webElement7 != null)
												{
													webElement7.Click();
													webElement7.SendKeys((Info.FirstName + " " + Info.LastName).Trim());
													Thread.Sleep(1000);
												}
											}
											IWebElement webElement8 = ADBHelperCCK.AppGetObject("//*[@text=\"Create\" or @content-desc=\"Create\"]", m_driver);
											if (webElement8 != null)
											{
												webElement8.Click();
												Thread.Sleep(5000);
												pageSource = GetPageSource();
											}
											if (pageSource.Contains("\"Skip\""))
											{
												webElement8 = ADBHelperCCK.AppGetObject("//*[@text=\"Skip\" or @content-desc=\"Skip\"]", m_driver);
												if (webElement8 != null)
												{
													webElement8.Click();
													Thread.Sleep(5000);
													pageSource = GetPageSource();
												}
											}
											continue;
										}
										File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
										Thread.Sleep(1800000);
										return false;
									}
									if (num3 == num4)
									{
										return false;
									}
								}
								else
								{
									Utils.CCKLog(p_DeviceId, "Birthday Error");
								}
							}
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("Too many attempts. Try again later"))
							{
								ShowMessageOnGrid(p_DeviceId, "Too many attempts", gridPhone, main);
								if (!string.IsNullOrWhiteSpace(Info.Email))
								{
									File.AppendAllLines("Config\\too_many_attempts_email.txt", new List<string> { Info.Email + "|" + Info.PassEmail });
								}
								ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								return false;
							}
							continue;
						}
						File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
						return false;
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Register ", ex.Message);
			}
			return false;
		}

		private void UpvideoLite()
		{
		}

		internal void ChangeAvatarLite()
		{
			Utils.LogFunction("ChangeAvatarLite", "");
			try
			{
				string text = Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER);
				if (string.IsNullOrEmpty(text))
				{
					text = Application.StartupPath + "\\Data\\Avatar\\";
				}
				if (!Directory.Exists(text) || Directory.GetFiles(text).Length == 0)
				{
					string fileName = text + Guid.NewGuid().ToString("N") + ".jpg";
					int num = 0;
					while (num++ < 10)
					{
						try
						{
							string input = new WebClient().DownloadString("https://boredhumans.com/api_faces2.php");
							Regex regex = new Regex("src=\"(.*?)\"");
							Match match = regex.Match(input);
							if (match.Success)
							{
								string text2 = match.Groups[1].Value;
								if (text2.StartsWith("/"))
								{
									text2 = "https://boredhumans.com" + text2;
								}
								new WebClient().DownloadFile(text2, fileName);
								goto IL_0107;
							}
						}
						catch
						{
						}
						Thread.Sleep(1000);
					}
				}
				goto IL_0107;
				IL_0107:
				GotoTab(Tabs.Me, AppFull: false);
				GetPageSource();
				GotoTab(Tabs.Me, AppFull: false);
				ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 4, screen.X / 3, screen.Y * 3 / 5);
				IWebElement webElement = ADBHelperCCK.WaitMeCount("//*[@text=\"Edit profile\" or @text=\"Set up profile\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					webElement = null;
				}
				IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Change profile picture\"]", m_driver);
				if (webElement2 != null)
				{
					CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
					CCKNode cCKNode = cCKDriver.FindElement("//*[@text=\"Change profile picture\"]", GetPageSource());
					if (cCKNode != null)
					{
						string attribute = cCKNode.GetAttribute("index");
						webElement2 = ADBHelperCCK.AppGetObject($"//*[@index=\"{Utils.Convert2Int(attribute) + 2}\"]", m_driver);
					}
				}
				if (webElement2 == null)
				{
					webElement2 = ADBHelperCCK.AppGetObject("//android.widget.ScrollView/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.FrameLayout/android.widget.ImageView[2]", m_driver);
				}
				if (webElement2 != null)
				{
					webElement2.Click();
					Thread.Sleep(1000);
					string pageSource = GetPageSource();
					if (pageSource.Contains("\"Change photo\""))
					{
						List<IWebElement> list = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
						if (list != null && list.Count == 3)
						{
							list[1].Click();
							Thread.Sleep(1000);
						}
						pageSource = GetPageSource();
					}
					if (pageSource.ToLower().Contains("\"select from gallery\""))
					{
						if (Directory.Exists(text) && Directory.GetFiles(text).Length != 0)
						{
							string OutfileName = "";
							if (!PushRandomFile(text, out OutfileName, 1, Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER_DELETE))))
							{
								AddLogByAction("No Picture", isAppend: false);
								return;
							}
						}
						webElement2 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"select from gallery\"]", m_driver);
						if (webElement2 != null)
						{
							webElement2.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							if (pageSource.Contains("\"All\""))
							{
								IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"All media\" or @content-desc=\"All media\" or @text=\"All\" or @content-desc=\"All media\"]", m_driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(1000);
									pageSource = GetPageSource();
									if (!pageSource.Contains("Pictures"))
									{
										if (pageSource.Contains("All"))
										{
											webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"All\" or @content-desc=\"All\"]", m_driver);
											if (webElement3 != null)
											{
												webElement3.Click();
												Thread.Sleep(1000);
											}
										}
									}
									else
									{
										webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Pictures\" or @content-desc=\"Pictures\"]", m_driver);
										if (webElement3 != null)
										{
											webElement3.Click();
											Thread.Sleep(1000);
										}
									}
								}
							}
							pageSource = GetPageSource();
							List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.ImageView", m_driver);
							if (list2 != null && list2.Count > 0)
							{
								IWebElement webElement4 = list2[list2.Count - 1];
								webElement4.Click();
								Thread.Sleep(1000);
								webElement2 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"save\"]", m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(10000);
									AddLogByAction("Change Avatar Successfully", isAppend: false);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Upvideo", ex.Message);
			}
			GotoTab(Tabs.Me, AppFull: false);
			GetPageSource();
			GotoTab(Tabs.Me, AppFull: false);
		}

		public bool RegisterMemoTiktokLiteByGmailS7(DataGridView gridPhone, Form main, int CategoryId, RegNickEntity entity)
		{
			bool flag = true;
			RemoveSystemAccount(m_driver);
			ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Thread.Sleep(2000);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Thread.Sleep(5000);
			Info = new TTItems(initName: true);
			ShowMessageOnGrid(p_DeviceId, "Start Reg", gridPhone, main);
			CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
			VNNameEntity vNNameEntity = new VNNameEntity();
			if (File.Exists(CaChuaConstant.VN_Name))
			{
				vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
				Info.FirstName = vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)];
				Info.LastName = vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)];
				Info.Uid = Utils.UnicodeToKoDau(Info.FirstName + Info.LastName);
				Info.Brand = Utils.ReadTextFile(Application.StartupPath + "\\Devices\\CCKInfo_" + p_DeviceId + ".txt").Replace(Environment.NewLine, "|");
			}
			Thread.Sleep(1000);
			string pageSource = GetPageSource();
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
			DateTime dateTime = DateTime.Now.AddMinutes(30.0);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			int num = 15;
			bool flag2 = false;
			try
			{
				ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 5 / 6, screenResolution.X / 3, screenResolution.Y / 4);
				pageSource = GetPageSource().ToLower();
				while (dateTime > DateTime.Now && num-- > 0)
				{
					if (pageSource.Contains("text=\"log in or sign up\""))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("ef3eb6f0c8ec40ea89adbee91b028df2"), m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Log in or sign up", gridPhone, main);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("\"profile\""))
					{
						ADBHelperCCK.Tap(p_DeviceId, screenResolution.X / 2, screenResolution.Y / 2);
						List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("3af73cb94e07410695ff53be7a0f86d9"), m_driver);
						if (list != null && list.Count > 0)
						{
							list[list.Count - 1].Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Profile click", gridPhone, main);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("\"continue as"))
							{
								IWebElement webElement2 = ADBHelperCCK.AppGetObject(GetFunctionByKey("44f50bdfca3943849f9381cca0101877"), m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(5000);
								}
							}
							if (pageSource.Contains("text=\"@"))
							{
								list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("e1249390871749b8a6d30774fd272083"), m_driver);
								if (list != null)
								{
									pageSource = GetPageSource().ToLower();
									Info.Uid = list[0].Text.TrimStart('@');
									Info.Pass = "";
									Info.UserAgent = p_DeviceId;
									if (!flag)
									{
										Rename(Info.LastName + " " + Info.FirstName);
									}
									ShowMessageOnGrid(p_DeviceId, "Change Password", gridPhone, main);
									GetCookies();
									sql.Insert(Info, CategoryId);
									RegChangePass();
								}
								return true;
							}
						}
					}
					if (!pageSource.Contains("\"Screen lock type\""))
					{
						if (pageSource.Contains("have an account? sign up\"") || pageSource.Contains("sign up for tiktok"))
						{
							IWebElement webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("012197cb91874128837bda82e19e649e"), m_driver);
							if (webElement3 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, webElement3.Location.X + webElement3.Size.Width * 9 / 10, webElement3.Location.Y + webElement3.Size.Height / 2);
								Thread.Sleep(1000);
							}
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("Continue with Google".ToLower()) || false)
							{
								List<CCKNode> list2 = cCKDriver.FindElements(GetFunctionByKey("a32bce59c5ab46c6b5796098dd2c6ff6"), pageSource);
								if (list2 != null && list2.Count > 0)
								{
									list2[0].Click();
									Thread.Sleep(2000);
									pageSource = GetPageSource();
									if (pageSource.Contains("\"Choose an account\""))
									{
										RemoveSystemAccount(m_driver);
										return false;
									}
								}
								IWebElement webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("3e2facd7b419470a91d7cbe6a10087d6"), m_driver, 120);
								if (webElement4 == null)
								{
									pageSource = GetPageSource();
									if (!pageSource.Contains("\"Something went wrong\"") && pageSource.Contains("\"Choose an account\""))
									{
										return false;
									}
									return false;
								}
								Thread.Sleep(3000);
								webElement4.Click();
								EmailUtils emailUtils = new EmailUtils();
								Account email = emailUtils.GetEmail(Info);
								if (!(email.User != ""))
								{
									RunNextAccount = false;
									return false;
								}
								Info.Email = email.User;
								Info.PassEmail = email.Pass;
								webElement4.SendKeys(email.User);
								Thread.Sleep(1000);
								webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("12ff64449a134d1f9afdd50bb6433d8c"), m_driver, 30);
								if (webElement4 != null)
								{
									webElement4.Click();
									Thread.Sleep(5000);
									pageSource = GetPageSource();
									ADBHelperCCK.WaitMeCount(GetFunctionByKey("a35ea872391f4c488664a0cd4a40410e"), m_driver, 60);
									Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
									pageSource = GetPageSource();
									if (!pageSource.Contains("\"Account deleted\""))
									{
									}
									if (!pageSource.Contains("text=\"Show password\""))
									{
									}
									if (pageSource.Contains("\"Got it\""))
									{
										IWebElement webElement5 = ADBHelperCCK.AppGetObject(GetFunctionByKey("70b58c5a4fb04a29b66077117e8ae18e"), m_driver);
										if (webElement5 != null)
										{
											webElement5.Click();
											Thread.Sleep(5000);
										}
									}
									if (pageSource.Contains("text=\"Create account\""))
									{
										emailUtils.ReturnStock(email);
										return false;
									}
									IWebElement webElement6 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver, 30);
									if (webElement6 != null)
									{
										webElement6.Click();
										Thread.Sleep(3000);
										webElement6.SendKeys(email.Pass);
										Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										pageSource = GetPageSource();
										webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("6b82b5de615c41db8a62d30f0ce73e69"), m_driver, 30);
										if (webElement4 != null)
										{
											webElement4.Click();
											Thread.Sleep(2000);
										}
										Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										Point screenResolution2 = ADBHelperCCK.GetScreenResolution(p_DeviceId);
										for (int i = 0; i < 5; i++)
										{
											ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 2, screenResolution2.Y / 2, screenResolution2.X / 2, screenResolution2.Y / 4);
										}
										pageSource = GetPageSource();
										if (pageSource.Contains("\"Continue\""))
										{
											webElement4 = ADBHelperCCK.AppGetObject(GetFunctionByKey("db05451af8644111879336a695bd3d92"), m_driver);
											if (webElement4 != null)
											{
												webElement4.Click();
												Thread.Sleep(10000);
											}
										}
										else
										{
											webElement4 = ADBHelperCCK.WaitMe(GetFunctionByKey("c0a561b5b37d437c97b828e86d98616a"), m_driver);
											DateTime dateTime2 = DateTime.Now.AddMinutes(2.0);
											while (dateTime2 > DateTime.Now)
											{
												List<IWebElement> list3 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("c0a561b5b37d437c97b828e86d98616a"), m_driver);
												if (list3 != null && list3.Count > 0 && list3[list3.Count - 1].Enabled)
												{
													webElement4 = list3[list3.Count - 1];
													webElement4.Click();
													Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
													webElement4 = ADBHelperCCK.WaitMe(GetFunctionByKey("c0a561b5b37d437c97b828e86d98616a"), m_driver);
													pageSource = GetPageSource();
												}
												ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 2, screenResolution2.Y / 2, screenResolution2.X / 2, screenResolution2.Y / 4);
												Thread.Sleep(5000);
												pageSource = GetPageSource();
												if (!pageSource.Contains("text=\"Birthday\""))
												{
													if (pageSource.Contains("text=\"Profile\"") && pageSource.Contains("text=\"Inbox\""))
													{
														break;
													}
													continue;
												}
												goto IL_08d7;
											}
										}
										pageSource = GetPageSource();
										if (!pageSource.Contains("Your birthday"))
										{
											pageSource = GetPageSource().ToLower();
										}
									}
								}
							}
						}
						goto IL_08d7;
					}
					return false;
					IL_08d7:
					pageSource = GetPageSource().ToLower();
					if (pageSource.Contains("\"sign up\""))
					{
						pageSource = GetPageSource();
						if ((pageSource.Contains("Send code") || pageSource.Contains("Next")) && pageSource.Contains("\"Email\""))
						{
							ShowMessageOnGrid(p_DeviceId, "Get Email", gridPhone, main);
							List<CCKNode> list4 = cCKDriver.FindElements("//*[@text=\"Email\"]", GetPageSource());
							list4[list4.Count - 1].Click();
							Thread.Sleep(1000);
							Account email2 = emailUti.GetEmail(Info);
							if (email2.accounts == null || email2.accounts.ToString().Length <= 2)
							{
								ShowMessageOnGrid(p_DeviceId, "Hết Email", gridPhone, main);
								RunNextAccount = false;
								return false;
							}
							ShowMessageOnGrid(p_DeviceId, "Email" + email2.accounts, gridPhone, main);
							File.AppendAllLines("Config\\backupEmail.txt", new List<string> { email2.accounts });
							Info.Email = email2.User;
							Info.PassEmail = email2.Pass;
							pageSource = GetPageSource();
							List<IWebElement> list5 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("a52f36f38f3147b48de6e180ea85c542"), m_driver);
							if (list5 != null && list5.Count > 0)
							{
								list5[list4.Count - 1].Click();
								list5[list4.Count - 1].SendKeys(Info.Email);
								Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
								List<IWebElement> list6 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), m_driver);
								if (list6 != null)
								{
									ADBHelperCCK.Tap(p_DeviceId, list6[list6.Count - 1].Location.X + list6[list6.Count - 1].Size.Width / 4, list6[list6.Count - 1].Location.Y);
									Thread.Sleep(1000);
									ADBHelperCCK.WaitMe(GetFunctionByKey("4c24b3de3bb34abea952b094307f0880"), m_driver);
									pageSource = GetPageSource();
									WaitCaptchaXoay(m_driver);
									while (pageSource.Contains("\"Next\"") && pageSource.Contains("\"Email\""))
									{
										list6 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), m_driver);
										if (list6 != null)
										{
											ADBHelperCCK.Tap(p_DeviceId, list6[list6.Count - 1].Location.X + list6[list6.Count - 1].Size.Width / 4, list6[list6.Count - 1].Location.Y);
											Thread.Sleep(5000);
										}
										pageSource = GetPageSource().ToLower();
										if (!pageSource.Contains("maximum number of attempts reached. try again later."))
										{
											Thread.Sleep(1000);
											continue;
										}
										return false;
									}
								}
							}
						}
					}
					pageSource = GetPageSource().ToLower();
					if (pageSource.Contains("text=\"share to\""))
					{
						ADBHelperCCK.Back(p_DeviceId);
						pageSource = GetPageSource().ToLower();
					}
					if (!pageSource.Contains("\"choose an account\""))
					{
						if (pageSource.Contains("Terms of Service".ToLower()))
						{
							ADBHelperCCK.Back(p_DeviceId);
							Thread.Sleep(1000);
							pageSource = GetPageSource().ToLower();
						}
						if (pageSource.Contains("\"set up 2-step verification\""))
						{
							GotoTab(Tabs.Profile);
						}
						if (!pageSource.Contains("maximum number of attempts reached"))
						{
							if (!pageSource.Contains("looks like you're not eligible for TikTok".ToLower()))
							{
								if (pageSource.Contains("your birthday") && !flag2)
								{
									flag2 = true;
									ShowMessageOnGrid(p_DeviceId, "Your birthday", gridPhone, main);
									pageSource = GetPageSource();
									List<CCKNode> list7 = cCKDriver.FindElements(GetFunctionByKey("b32d0ce5c6e84f56bfa9c923ed042985"), pageSource);
									if (list7 != null && list7.Count >= 3)
									{
										list7 = list7.Skip(list7.Count - 3).Take(3).ToList();
										_ = list7.Count - 1;
										int x = list7[0].Location.X;
										int y = list7[0].Location.Y;
										_ = list7[0].Size.Width;
										int height = list7[0].Size.Height;
										ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
										Thread.Sleep(1000);
										x = list7[1].Location.X;
										y = list7[1].Location.Y;
										_ = list7[1].Size.Width;
										height = list7[1].Size.Height;
										ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
										Thread.Sleep(1000);
										x = list7[2].Location.X;
										y = list7[2].Location.Y;
										_ = list7[2].Size.Width;
										height = list7[2].Size.Height;
										int num2 = new Random().Next(entity.AgeFrom / 2, entity.AgeTo / 2);
										for (int j = 0; j < num2; j++)
										{
											ADBHelperCCK.Swipe(p_DeviceId, x, y - height / 3, x, y + height / 3, 500);
										}
										Thread.Sleep(2000);
										pageSource = GetPageSource();
										int num3 = 0;
										int num4 = 5;
										while (pageSource.Contains("\"Next\"") && num3++ < num4)
										{
											List<IWebElement> list8 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), m_driver);
											if (list8 != null)
											{
												ShowMessageOnGrid(p_DeviceId, "Your birthday Next", gridPhone, main);
												ADBHelperCCK.Tap(p_DeviceId, list8[list8.Count - 1].Location.X + list8[list8.Count - 1].Size.Width / 4, list8[list8.Count - 1].Location.Y);
												Thread.Sleep(5000);
											}
											pageSource = GetLocalSource();
											if (!pageSource.Contains("Too many attempts") && !pageSource.Contains("Sorry, looks like you're not eligible for TikTok"))
											{
												ADBHelperCCK.WaitMe(GetFunctionByKey("28fbfeedf6b54e868ac80e977ab6f584"), m_driver);
												pageSource = GetLocalSource();
												if (!pageSource.Contains("\"Create nickname\""))
												{
													continue;
												}
												IWebElement webElement7 = ADBHelperCCK.AppGetObject(GetFunctionByKey("28fbfeedf6b54e868ac80e977ab6f584"), m_driver);
												IWebElement webElement8 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
												if (webElement7 != null && webElement8 != null)
												{
													string value = (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.EMAIL_NAME)) ? "" : webElement8.Text);
													if (string.IsNullOrEmpty(value))
													{
														webElement7.Click();
														Thread.Sleep(5000);
														webElement7 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
														if (webElement7 != null)
														{
															webElement7.Click();
															webElement7.SendKeys((Info.FirstName + " " + Info.LastName).Trim());
															Thread.Sleep(1000);
														}
													}
													else
													{
														Thread.Sleep(1000 * rnd.Next(5, 10));
													}
												}
												IWebElement webElement9 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("5f210010a02045989d77b03bbee90369"), m_driver);
												if (webElement9 != null)
												{
													webElement9.Click();
													Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
													pageSource = GetPageSource();
												}
												if (pageSource.Contains("\"Skip\""))
												{
													webElement9 = ADBHelperCCK.AppGetObject(GetFunctionByKey("a06b41a30eb94037a3cc40130307a838"), m_driver);
													if (webElement9 != null)
													{
														webElement9.Click();
														Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
														pageSource = GetPageSource();
													}
												}
												continue;
											}
											File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
											Thread.Sleep(1800000);
											return false;
										}
										if (num3 == num4)
										{
											return false;
										}
									}
									else
									{
										Utils.CCKLog(p_DeviceId, "Birthday Error");
									}
								}
								pageSource = GetPageSource();
								if (pageSource.Contains("\"Create nickname\""))
								{
									IWebElement webElement10 = ADBHelperCCK.AppGetObject(GetFunctionByKey("28fbfeedf6b54e868ac80e977ab6f584"), m_driver);
									if (webElement10 != null)
									{
										webElement10.Click();
										Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										webElement10 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
										if (webElement10 != null)
										{
											webElement10.Click();
											webElement10.SendKeys((Info.FirstName + " " + Info.LastName).Trim());
											Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										}
									}
									IWebElement webElement11 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("5f210010a02045989d77b03bbee90369"), m_driver);
									if (webElement11 != null)
									{
										webElement11.Click();
										Thread.Sleep(5000);
										pageSource = GetPageSource();
									}
									if (pageSource.Contains("\"Skip\""))
									{
										webElement11 = ADBHelperCCK.AppGetObject(GetFunctionByKey("a06b41a30eb94037a3cc40130307a838"), m_driver);
										if (webElement11 != null)
										{
											webElement11.Click();
											Thread.Sleep(5000);
											pageSource = GetPageSource();
										}
									}
								}
								pageSource = GetPageSource().ToLower();
								if (!pageSource.Contains("Too many attempts. Try again later"))
								{
									if (!pageSource.Contains("account deleted"))
									{
										if (!pageSource.Contains("manage account privacy"))
										{
											continue;
										}
										IWebElement webElement12 = ADBHelperCCK.AppGetObject("//*[@text=\"Public account\" or @content-desc=\"Public account\"]", m_driver);
										if (webElement12 == null)
										{
											continue;
										}
										webElement12.Click();
										Thread.Sleep(2000);
										pageSource = GetPageSource().ToLower();
										webElement12 = ADBHelperCCK.AppGetObject("//android.widget.Button[contains(@text,\"Public account\") or contains(@content-desc,\"Public account\")]", m_driver);
										if (webElement12 != null)
										{
											webElement12.Click();
											Thread.Sleep(1000);
											webElement12 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"OK\" or upper-case(@content-desc)=\"OK\"]", m_driver);
											if (webElement12 != null)
											{
												webElement12.Click();
												Thread.Sleep(1000);
											}
											webElement12 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"NEXT\" or upper-case(@content-desc)=\"NEXT\"]", m_driver);
											if (webElement12 != null)
											{
												webElement12.Click();
												Thread.Sleep(1000);
											}
										}
										continue;
									}
									ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
									return false;
								}
								ShowMessageOnGrid(p_DeviceId, "Too many attempts", gridPhone, main);
								if (!string.IsNullOrWhiteSpace(Info.Email))
								{
									File.AppendAllLines("Config\\too_many_attempts_email.txt", new List<string> { Info.Email + "|" + Info.PassEmail });
								}
								ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								return false;
							}
							File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
							return false;
						}
						return false;
					}
					RemoveSystemAccount(m_driver);
					return false;
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Register ", ex.Message);
			}
			return false;
		}

		public bool RegisterMemoTiktokLiteByGmailS7CHplay(DataGridView gridPhone, Form main, int CategoryId, RegNickEntity entity)
		{
			Utils.LogFunction("RegisterMemoTiktokLiteByGmailS7CHplay", "");
			bool flag = true;
			RemoveSystemAccount(m_driver);
			if (entity.Register8AccountOnly)
			{
				ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				SwitchAccount(remove: false);
			}
			else
			{
				ADBHelperCCK.ClearAppStorage(p_DeviceId, m_driver, CaChuaConstant.PACKAGE_NAME);
			}
			Thread.Sleep(2000);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Thread.Sleep(5000);
			Info = new TTItems(initName: true);
			ShowMessageOnGrid(p_DeviceId, "Start Reg", gridPhone, main);
			CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
			VNNameEntity vNNameEntity = new VNNameEntity();
			if (File.Exists(CaChuaConstant.VN_Name))
			{
				vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
				Info.FirstName = vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)];
				Info.LastName = vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)];
				Info.Uid = Utils.UnicodeToKoDau(Info.FirstName + Info.LastName);
				Info.Brand = Utils.ReadTextFile(Application.StartupPath + "\\Devices\\CCKInfo_" + p_DeviceId + ".txt").Replace(Environment.NewLine, "|");
			}
			Thread.Sleep(1000);
			string pageSource = GetPageSource();
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
			DateTime dateTime = DateTime.Now.AddMinutes(30.0);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			int num = 15;
			bool flag2 = false;
			try
			{
				ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 5 / 6, screenResolution.X / 3, screenResolution.Y / 4);
				pageSource = GetPageSource().ToLower();
				while (dateTime > DateTime.Now && num-- > 0)
				{
					pageSource = GetPageSource().ToLower();
					if (pageSource.Contains("text=\"log in or sign up\""))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("ef3eb6f0c8ec40ea89adbee91b028df2"), m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Log in or sign up", gridPhone, main);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("\"profile\""))
					{
						ADBHelperCCK.Tap(p_DeviceId, screenResolution.X / 2, screenResolution.Y / 2);
						List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("3af73cb94e07410695ff53be7a0f86d9"), m_driver);
						if (list != null && list.Count > 0)
						{
							list[list.Count - 1].Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Profile click", gridPhone, main);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("\"continue as"))
							{
								IWebElement webElement2 = ADBHelperCCK.AppGetObject(GetFunctionByKey("44f50bdfca3943849f9381cca0101877"), m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(5000);
								}
							}
							if (pageSource.Contains("text=\"@"))
							{
								list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("e1249390871749b8a6d30774fd272083"), m_driver);
								if (list != null)
								{
									pageSource = GetPageSource().ToLower();
									Info.Uid = list[0].Text.TrimStart('@');
									Info.Pass = "";
									Info.UserAgent = p_DeviceId;
									if (!flag)
									{
										Rename(Info.LastName + " " + Info.FirstName);
									}
									ShowMessageOnGrid(p_DeviceId, "Change Password", gridPhone, main);
									GetCookies();
									sql.Insert(Info, CategoryId);
									RegChangePass();
									ChangeEmail();
								}
								return true;
							}
						}
					}
					if (!pageSource.Contains("\"Screen lock type\""))
					{
						if (pageSource.Contains("have an account? sign up\"") || pageSource.Contains("sign up for tiktok"))
						{
							IWebElement webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("012197cb91874128837bda82e19e649e"), m_driver);
							if (webElement3 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, webElement3.Location.X + webElement3.Size.Width * 9 / 10, webElement3.Location.Y + webElement3.Size.Height / 2);
								Thread.Sleep(1000);
							}
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("Continue with Google".ToLower()) || false)
							{
								List<CCKNode> list2 = cCKDriver.FindElements(GetFunctionByKey("a32bce59c5ab46c6b5796098dd2c6ff6"), pageSource);
								if (list2 != null && list2.Count > 0)
								{
									list2[0].Click();
									Thread.Sleep(5000);
									pageSource = GetPageSource();
									if (pageSource.Contains("\"Choose an account\""))
									{
										RemoveSystemAccount(m_driver);
										return false;
									}
								}
								IWebElement webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("3e2facd7b419470a91d7cbe6a10087d6"), m_driver, 120);
								if (webElement4 == null)
								{
									pageSource = GetPageSource();
									if (!pageSource.Contains("\"Something went wrong\"") && pageSource.Contains("\"Choose an account\""))
									{
										return false;
									}
									return false;
								}
								Thread.Sleep(3000);
								webElement4.Click();
								EmailUtils emailUtils = new EmailUtils();
								Account email = emailUtils.GetEmail(Info);
								if (!(email.User != ""))
								{
									RunNextAccount = false;
									return false;
								}
								Info.Email = email.User;
								Info.PassEmail = email.Pass;
								webElement4.SendKeys(email.User);
								Thread.Sleep(1000);
								webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("12ff64449a134d1f9afdd50bb6433d8c"), m_driver, 30);
								if (webElement4 != null)
								{
									webElement4.Click();
									Thread.Sleep(5000);
									pageSource = GetPageSource();
									ADBHelperCCK.WaitMe("//*[@text=\"Forgot password?\"]", m_driver);
									ADBHelperCCK.WaitMeCount(GetFunctionByKey("a35ea872391f4c488664a0cd4a40410e"), m_driver, 60);
									Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
									pageSource = GetPageSource();
									if (!pageSource.Contains("\"Account deleted\""))
									{
									}
									if (pageSource.Contains("text=\"Show password\""))
									{
									}
									if (pageSource.Contains("\"Got it\""))
									{
										IWebElement webElement5 = ADBHelperCCK.AppGetObject(GetFunctionByKey("70b58c5a4fb04a29b66077117e8ae18e"), m_driver);
										if (webElement5 != null)
										{
											webElement5.Click();
											Thread.Sleep(5000);
										}
									}
									IWebElement webElement6 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver, 30);
									pageSource = GetPageSource();
									if (webElement6 != null && !pageSource.Contains("text=\"Create account\""))
									{
										webElement6.Click();
										Thread.Sleep(3000);
										webElement6.SendKeys(email.Pass);
										Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										pageSource = GetPageSource();
										webElement4 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("6b82b5de615c41db8a62d30f0ce73e69"), m_driver, 30);
										if (webElement4 != null)
										{
											webElement4.Click();
											Thread.Sleep(2000);
										}
										Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										Point screenResolution2 = ADBHelperCCK.GetScreenResolution(p_DeviceId);
										for (int i = 0; i < 5; i++)
										{
											ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 2, screenResolution2.Y / 2, screenResolution2.X / 2, screenResolution2.Y / 4);
										}
										pageSource = GetPageSource();
										if (!pageSource.Contains("\"Continue\""))
										{
											webElement4 = ADBHelperCCK.WaitMe(GetFunctionByKey("c0a561b5b37d437c97b828e86d98616a"), m_driver);
											DateTime dateTime2 = DateTime.Now.AddMinutes(2.0);
											while (dateTime2 > DateTime.Now)
											{
												List<IWebElement> list3 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("c0a561b5b37d437c97b828e86d98616a"), m_driver);
												if (list3 != null && list3.Count > 0 && list3[list3.Count - 1].Enabled)
												{
													webElement4 = list3[list3.Count - 1];
													webElement4.Click();
													Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
													webElement4 = ADBHelperCCK.WaitMe(GetFunctionByKey("c0a561b5b37d437c97b828e86d98616a"), m_driver);
													pageSource = GetPageSource();
												}
												ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 2, screenResolution2.Y / 2, screenResolution2.X / 2, screenResolution2.Y / 4);
												Thread.Sleep(5000);
												pageSource = GetPageSource();
												if (!pageSource.Contains("text=\"Birthday\""))
												{
													if (pageSource.Contains("text=\"Profile\"") && pageSource.Contains("text=\"Inbox\""))
													{
														break;
													}
													continue;
												}
												goto IL_0951;
											}
										}
										else
										{
											webElement4 = ADBHelperCCK.AppGetObject(GetFunctionByKey("db05451af8644111879336a695bd3d92"), m_driver);
											if (webElement4 != null)
											{
												webElement4.Click();
												Thread.Sleep(10000);
											}
										}
										pageSource = GetPageSource();
										if (!pageSource.Contains("Your birthday"))
										{
											pageSource = GetPageSource().ToLower();
										}
									}
									else if (pageSource.Contains("text=\"Create account\""))
									{
										emailUtils.ReturnStock(email);
										return false;
									}
								}
							}
						}
						goto IL_0951;
					}
					return false;
					IL_0951:
					pageSource = GetPageSource().ToLower();
					if (pageSource.Contains("\"sign up\""))
					{
						pageSource = GetPageSource();
						if ((pageSource.Contains("Send code") || pageSource.Contains("Next")) && pageSource.Contains("\"Email\""))
						{
							ShowMessageOnGrid(p_DeviceId, "Get Email", gridPhone, main);
							List<CCKNode> list4 = cCKDriver.FindElements("//*[@text=\"Email\"]", GetPageSource());
							list4[list4.Count - 1].Click();
							Thread.Sleep(1000);
							Account email2 = emailUti.GetEmail(Info);
							if (email2.accounts == null || email2.accounts.ToString().Length <= 2)
							{
								ShowMessageOnGrid(p_DeviceId, "Hết Email", gridPhone, main);
								RunNextAccount = false;
								return false;
							}
							ShowMessageOnGrid(p_DeviceId, "Email" + email2.accounts, gridPhone, main);
							File.AppendAllLines("Config\\backupEmail.txt", new List<string> { email2.accounts });
							Info.Email = email2.User;
							Info.PassEmail = email2.Pass;
							pageSource = GetPageSource();
							List<IWebElement> list5 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("a52f36f38f3147b48de6e180ea85c542"), m_driver);
							if (list5 != null && list5.Count > 0)
							{
								list5[list4.Count - 1].Click();
								list5[list4.Count - 1].SendKeys(Info.Email);
								Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
								List<IWebElement> list6 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), m_driver);
								if (list6 != null)
								{
									ADBHelperCCK.Tap(p_DeviceId, list6[list6.Count - 1].Location.X + list6[list6.Count - 1].Size.Width / 4, list6[list6.Count - 1].Location.Y);
									Thread.Sleep(1000);
									ADBHelperCCK.WaitMe(GetFunctionByKey("4c24b3de3bb34abea952b094307f0880"), m_driver);
									pageSource = GetPageSource();
									WaitCaptchaXoay(m_driver);
									while (pageSource.Contains("\"Next\"") && pageSource.Contains("\"Email\""))
									{
										list6 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), m_driver);
										if (list6 != null)
										{
											ADBHelperCCK.Tap(p_DeviceId, list6[list6.Count - 1].Location.X + list6[list6.Count - 1].Size.Width / 4, list6[list6.Count - 1].Location.Y);
											Thread.Sleep(5000);
										}
										pageSource = GetPageSource().ToLower();
										if (!pageSource.Contains("maximum number of attempts reached. try again later."))
										{
											Thread.Sleep(1000);
											continue;
										}
										return false;
									}
								}
							}
						}
					}
					pageSource = GetPageSource().ToLower();
					if (pageSource.Contains("text=\"share to\""))
					{
						ADBHelperCCK.Back(p_DeviceId);
						pageSource = GetPageSource().ToLower();
					}
					if (!pageSource.Contains("\"choose an account\""))
					{
						if (pageSource.Contains("Terms of Service".ToLower()))
						{
							ADBHelperCCK.Back(p_DeviceId);
							Thread.Sleep(1000);
							pageSource = GetPageSource().ToLower();
						}
						if (pageSource.Contains("\"set up 2-step verification\""))
						{
							GotoTab(Tabs.Profile);
						}
						if (!pageSource.Contains("maximum number of attempts reached"))
						{
							if (!pageSource.Contains("looks like you're not eligible for TikTok".ToLower()))
							{
								if (pageSource.Contains("your birthday") && !flag2)
								{
									flag2 = true;
									ShowMessageOnGrid(p_DeviceId, "Your birthday", gridPhone, main);
									pageSource = GetPageSource();
									List<CCKNode> list7 = cCKDriver.FindElements(GetFunctionByKey("b32d0ce5c6e84f56bfa9c923ed042985"), pageSource);
									if (list7 != null && list7.Count >= 3)
									{
										list7 = list7.Skip(list7.Count - 3).Take(3).ToList();
										_ = list7.Count - 1;
										int x = list7[0].Location.X;
										int y = list7[0].Location.Y;
										_ = list7[0].Size.Width;
										int height = list7[0].Size.Height;
										ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
										Thread.Sleep(1000);
										x = list7[1].Location.X;
										y = list7[1].Location.Y;
										_ = list7[1].Size.Width;
										height = list7[1].Size.Height;
										ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
										Thread.Sleep(1000);
										x = list7[2].Location.X;
										y = list7[2].Location.Y;
										_ = list7[2].Size.Width;
										height = list7[2].Size.Height;
										int num2 = new Random().Next(entity.AgeFrom / 2, entity.AgeTo / 2);
										for (int j = 0; j < num2; j++)
										{
											ADBHelperCCK.Swipe(p_DeviceId, x, y - height / 3, x, y + height / 3, 500);
										}
										Thread.Sleep(2000);
										pageSource = GetPageSource();
										int num3 = 0;
										int num4 = 5;
										while (pageSource.Contains("\"Next\"") && num3++ < num4)
										{
											List<IWebElement> list8 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), m_driver);
											if (list8 != null)
											{
												ShowMessageOnGrid(p_DeviceId, "Your birthday Next", gridPhone, main);
												ADBHelperCCK.Tap(p_DeviceId, list8[list8.Count - 1].Location.X + list8[list8.Count - 1].Size.Width / 4, list8[list8.Count - 1].Location.Y);
												Thread.Sleep(5000);
											}
											pageSource = GetLocalSource();
											if (!pageSource.Contains("Too many attempts") && !pageSource.Contains("Sorry, looks like you're not eligible for TikTok"))
											{
												ADBHelperCCK.WaitMe(GetFunctionByKey("28fbfeedf6b54e868ac80e977ab6f584"), m_driver);
												pageSource = GetLocalSource();
												if (!pageSource.Contains("\"Create nickname\""))
												{
													continue;
												}
												IWebElement webElement7 = ADBHelperCCK.AppGetObject(GetFunctionByKey("28fbfeedf6b54e868ac80e977ab6f584"), m_driver);
												IWebElement webElement8 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
												if (webElement7 != null && webElement8 != null)
												{
													string value = (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.EMAIL_NAME)) ? "" : webElement8.Text);
													if (!string.IsNullOrEmpty(value))
													{
														Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_RENAME), 30));
													}
													else
													{
														webElement7.Click();
														Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_RENAME), 30));
														webElement7 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
														if (webElement7 != null)
														{
															webElement7.Click();
															webElement7.SendKeys((Info.FirstName + " " + Info.LastName).Trim());
															Thread.Sleep(5000);
														}
													}
												}
												IWebElement webElement9 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("5f210010a02045989d77b03bbee90369"), m_driver);
												if (webElement9 != null)
												{
													webElement9.Click();
													Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
													pageSource = GetPageSource();
												}
												if (pageSource.Contains("\"Skip\""))
												{
													webElement9 = ADBHelperCCK.AppGetObject(GetFunctionByKey("a06b41a30eb94037a3cc40130307a838"), m_driver);
													if (webElement9 != null)
													{
														webElement9.Click();
														Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
														pageSource = GetPageSource();
													}
												}
												continue;
											}
											File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
											Thread.Sleep(1800000);
											return false;
										}
										if (num3 == num4)
										{
											return false;
										}
									}
									else
									{
										Utils.CCKLog(p_DeviceId, "Birthday Error");
									}
								}
								pageSource = GetPageSource();
								if (pageSource.Contains("\"Create nickname\""))
								{
									IWebElement webElement10 = ADBHelperCCK.AppGetObject(GetFunctionByKey("28fbfeedf6b54e868ac80e977ab6f584"), m_driver);
									if (webElement10 != null)
									{
										webElement10.Click();
										Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										webElement10 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
										if (webElement10 != null)
										{
											webElement10.Click();
											webElement10.SendKeys((Info.FirstName + " " + Info.LastName).Trim());
											Thread.Sleep(1000 * Utils.GetRandomInRange(entity.GmailDelayFrom, entity.GmailDelayTo));
										}
									}
									IWebElement webElement11 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("5f210010a02045989d77b03bbee90369"), m_driver);
									if (webElement11 != null)
									{
										webElement11.Click();
										Thread.Sleep(5000);
										pageSource = GetPageSource();
									}
									if (pageSource.Contains("\"Skip\""))
									{
										webElement11 = ADBHelperCCK.AppGetObject(GetFunctionByKey("a06b41a30eb94037a3cc40130307a838"), m_driver);
										if (webElement11 != null)
										{
											webElement11.Click();
											Thread.Sleep(5000);
											pageSource = GetPageSource();
											flag = false;
										}
									}
								}
								pageSource = GetPageSource().ToLower();
								if (!pageSource.Contains("Too many attempts. Try again later"))
								{
									if (!pageSource.Contains("account deleted"))
									{
										if (!pageSource.Contains("manage account privacy"))
										{
											continue;
										}
										IWebElement webElement12 = ADBHelperCCK.AppGetObject("//*[@text=\"Public account\" or @content-desc=\"Public account\"]", m_driver);
										if (webElement12 == null)
										{
											continue;
										}
										webElement12.Click();
										Thread.Sleep(2000);
										pageSource = GetPageSource().ToLower();
										webElement12 = ADBHelperCCK.AppGetObject("//android.widget.Button[contains(@text,\"Public account\") or contains(@content-desc,\"Public account\")]", m_driver);
										if (webElement12 != null)
										{
											webElement12.Click();
											Thread.Sleep(1000);
											webElement12 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"OK\" or upper-case(@content-desc)=\"OK\"]", m_driver);
											if (webElement12 != null)
											{
												webElement12.Click();
												Thread.Sleep(1000);
											}
											webElement12 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"NEXT\" or upper-case(@content-desc)=\"NEXT\"]", m_driver);
											if (webElement12 != null)
											{
												webElement12.Click();
												Thread.Sleep(1000);
											}
										}
										continue;
									}
									ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
									return false;
								}
								ShowMessageOnGrid(p_DeviceId, "Too many attempts", gridPhone, main);
								if (!string.IsNullOrWhiteSpace(Info.Email))
								{
									File.AppendAllLines("Config\\too_many_attempts_email.txt", new List<string> { Info.Email + "|" + Info.PassEmail });
								}
								ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								return false;
							}
							File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
							return false;
						}
						return false;
					}
					RemoveSystemAccount(m_driver);
					return false;
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Register ", ex.Message);
			}
			return false;
		}

		public void GetCookies(string appName = "")
		{
			try
			{
				if (appName == "")
				{
					appName = CaChuaConstant.PACKAGE_NAME;
				}
				string text = Application.StartupPath + "\\Authentication\\" + Info.Uid;
				if (Info.Uid != "")
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					List<string> list = new List<string>();
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					string text2 = ADBHelperCCK.ExecuteCMD(p_DeviceId, "pull \"/data/data/" + appName + "/app_webview/Cookies\" \"" + text + "\\Cookies");
					if (text2.Contains("failed"))
					{
						text2 = ADBHelperCCK.ExecuteCMD(p_DeviceId, "pull \"/data/data/" + appName + "/app_webview/Default/Cookies\" \"" + text + "\\Cookies");
					}
					List<TiktokCookies> list2 = new List<TiktokCookies>();
					DataTable dataTable = new SQLiteUtils().ExecuteQuery("select * from cookies where host_key = '.tiktok.com' or host_key = 'www.tiktok.com'", text + "\\Cookies");
					if (dataTable != null)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							TiktokCookies tiktokCookies = new TiktokCookies
							{
								domain = row["host_key"].ToString(),
								expirationDate = Utils.Convert2Int64(row["expires_utc"].ToString()),
								httpOnly = (Utils.Convert2Int(row["is_httponly"].ToString()) == 1),
								name = row["name"].ToString(),
								value = row["value"].ToString(),
								path = row["path"].ToString(),
								session = true,
								storeId = "0"
							};
							list2.Add(tiktokCookies);
							dictionary.Add(tiktokCookies.name, tiktokCookies.value);
							list.Add(tiktokCookies.name + "=" + tiktokCookies.value);
							if (tiktokCookies.name.ToLower().Contains("sessionid_ss"))
							{
								Info.Cookie = js.Serialize(list2);
							}
						}
					}
				}
				if (Info.Cookie == "")
				{
					string input = ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell cat '/data/data/" + appName + "/app_webview/Cookies'");
					Regex regex = new Regex("sessionid_ss(.{32})");
					Match match = regex.Match(input);
					if (match.Success && match.Groups.Count > 0)
					{
						input = "sessionid_ss=" + match.Groups[1].Value;
						Info.Cookie = input;
					}
				}
			}
			catch
			{
			}
		}

		public void FactoryReset()
		{
			string cmd = "shell am broadcast -a android.intent.action.MASTER_CLEAR";
			ADBHelperCCK.ExecuteCMD(p_DeviceId, cmd);
			IWebElement webElement = ADBHelperCCK.WaitMe("//*[@text=\"Reset\" or @content-desc=\"Reset\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(3000);
			webElement = ADBHelperCCK.WaitMe("//*[lower-case(@text)=\"delete all\" or lower-case(@content-desc)=\"delete all\"]", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(60000);
				string text = "";
				while (text.Trim() != "1")
				{
					text = ADBHelperCCK.ExecuteCMDCommand(p_DeviceId, "shell getprop sys.boot_completed");
					Thread.Sleep(10000);
				}
				m_driver = ADBHelperCCK.StartPhone(deviceEntity);
				ADBHelperCCK.InstallADBkeyBoard(p_DeviceId);
				ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\TikTok_32.9.apk");
				ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\TikTok_cck_Lite.apk");
				ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\ADBCLanguage.apk");
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell pm grant net.sanapeli.adbchangelanguage android.permission.CHANGE_CONFIGURATION");
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -n net.sanapeli.adbchangelanguage/.AdbChangeLanguage -e language en");
			}
		}

		public void RemoveSystemAccount(RemoteWebDriver m_driver)
		{
			ADBHelperCCK.SetPortrait(p_DeviceId);
			ADBHelperCCK.CloseApp(p_DeviceId, "com.android.settings");
			ADBHelperCCK.ClearAppData(p_DeviceId, "com.google.android.gm");
			ADBHelperCCK.ClearAppData(p_DeviceId, "com.google.android.gm.lite");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -a android.settings.SYNC_SETTINGS");
			Thread.Sleep(1000);
			ADBHelperCCK.WaitMe("//*[upper-case(@text)=\"ACCOUNTS\" or upper-case(@content-desc)=\"ACCOUNTS\"]", m_driver);
			string pageSource = GetPageSource();
			bool flag;
			do
			{
				Utils.CCKLog("RemoveSystemAccount", DateTime.Now.ToString());
				List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(@text,'Google')]", m_driver);
				flag = false;
				if (list == null)
				{
					continue;
				}
				foreach (IWebElement item in list)
				{
					if (!item.Text.Contains("Google"))
					{
						continue;
					}
					item.Click();
					Thread.Sleep(2000);
					pageSource = GetPageSource();
					if (pageSource.Contains("text=\"More\""))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"More\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
						}
					}
					IWebElement webElement2 = ADBHelperCCK.WaitMe("//*[contains(lower-case(@text),'remove account') or contains(lower-case(@content-desc),'remove account')]", m_driver);
					if (webElement2 == null)
					{
						Utils.CCKLog("RemoveSystemAccount", "Khong tim thay nut Remove Account");
					}
					else
					{
						webElement2.Click();
						Thread.Sleep(2000);
						List<IWebElement> list2 = ADBHelperCCK.WaitMes(By.XPath("//*[contains(lower-case(@text),'remove account') or contains(lower-case(@content-desc),'remove account')]"), m_driver);
						if (list2 != null)
						{
							list2[list2.Count - 1].Click();
							Thread.Sleep(1000);
						}
					}
					flag = true;
					break;
				}
			}
			while (flag);
		}

		public bool RegisterMemoTiktokByPhone(DataGridView gridPhone, Form main, int CategoryId, RegNickEntity entity)
		{
			Utils.LogFunction("RegisterMemoTiktokByPhone", "");
			RemoveSystemAccount(m_driver);
			if (entity.Register8AccountOnly)
			{
				SwitchAccount();
				ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			}
			else
			{
				ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				Thread.Sleep(2000);
				ADBHelperCCK.SetStoragePermission(p_DeviceId);
				ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				Thread.Sleep(5000);
			}
			Info = new TTItems(initName: true);
			ShowMessageOnGrid(p_DeviceId, "Start Reg", gridPhone, main);
			CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
			VNNameEntity vNNameEntity = new VNNameEntity();
			if (File.Exists(CaChuaConstant.VN_Name))
			{
				vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
				Info.FirstName = vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)];
				Info.LastName = vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)];
				Info.Uid = Utils.UnicodeToKoDau(Info.FirstName + Info.LastName);
				Info.Brand = Utils.ReadTextFile(Application.StartupPath + "\\Devices\\CCKInfo_" + p_DeviceId + ".txt").Replace(Environment.NewLine, "|");
			}
			Thread.Sleep(1000);
			string pageSource = GetPageSource();
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
			DateTime dateTime = DateTime.Now.AddMinutes(30.0);
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			int num = 15;
			bool flag = false;
			try
			{
				ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4, 1000);
				pageSource = GetPageSource().ToLower();
				while (dateTime > DateTime.Now && num-- > 0)
				{
					if (pageSource.Contains("text=\"log in or sign up\""))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"log in or sign up\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Log in or sign up", gridPhone, main);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("\"profile\""))
					{
						List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"profile\")]", m_driver);
						if (list != null && list.Count > 0)
						{
							list[list.Count - 1].Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Profile click", gridPhone, main);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("text=\"@"))
							{
								list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"@\")]", m_driver);
								if (list != null)
								{
									pageSource = GetPageSource().ToLower();
									Info.Uid = list[0].Text.TrimStart('@');
									Info.UserAgent = p_DeviceId;
									sql.Insert(Info, CategoryId);
									Rename(Info.LastName + " " + Info.FirstName);
									ShowMessageOnGrid(p_DeviceId, "Rename", gridPhone, main);
									BackupAccountTiktok();
									ChangeEmail(change: false);
								}
								return true;
							}
							if (pageSource.Contains("Login by password"))
							{
							}
						}
					}
					if (pageSource.Contains("text=\"add email address\""))
					{
						IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"email address\")]", m_driver);
						if (webElement2 != null)
						{
							webElement2.Click();
							Thread.Sleep(1000);
							webElement2 = ADBHelperCCK.AppGetObject("//*[lower-case(@text) =\"email address\"]", m_driver);
							if (webElement2 != null)
							{
								EmailUtils emailUtils = new EmailUtils();
								Account email = emailUtils.GetEmail(Info);
								if (email != null && email.User != "")
								{
									webElement2.SendKeys(email.User);
									Info.Email = email.User;
									Info.PassEmail = email.Pass;
								}
								Thread.Sleep(1000);
								webElement2 = ADBHelperCCK.AppGetObject("//*[lower-case(@text) =\"send code\"]", m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(10000);
									string text = "";
									while (text == "")
									{
										text = emailUtils.GetCode(Info.Email, Info.Pass);
										if (text != "")
										{
											break;
										}
										Thread.Sleep(10000);
									}
									if (text != "")
									{
										List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//android.widget.EditText", m_driver);
										if (list2 != null && list2.Count == 1)
										{
											list2[0].SendKeys(text);
											Thread.Sleep(5000);
											pageSource = GetPageSource().ToLower();
										}
									}
								}
							}
						}
					}
					if (pageSource.ToLower().Contains("sign up for tiktok") && pageSource.ToLower().Contains("use phone or email"))
					{
						List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"use phone or email\")]", m_driver);
						if (list3 != null && list3.Count > 0)
						{
							list3[list3.Count - 1].Click();
							Thread.Sleep(1000);
							ShowMessageOnGrid(p_DeviceId, "Use phone or email", gridPhone, main);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.ToLower().Contains("text=\"phone number\""))
					{
						List<IWebElement> list4 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"phone number\")]", m_driver);
						if (list4 != null && list4.Count > 0)
						{
							list4[list4.Count - 1].Click();
							Thread.Sleep(1000);
							CodeResult phone = PhoneUtils.GetPhone(ServiceType.Tiktok);
							if (phone.Success)
							{
								Info.Phone = phone.PhoneOrEmail;
							}
							if (Info.Phone == "")
							{
								RunNextAccount = false;
								return false;
							}
							pageSource = GetPageSource().ToLower();
							list4 = ADBHelperCCK.AppGetObjects("//*[lower-case(@text)=\"phone number\"]", m_driver);
							if (list4 != null && list4.Count > 0)
							{
								list4[list4.Count - 1].SendKeys(Info.Phone);
							}
							Thread.Sleep(1000);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("\"send code\""))
							{
								IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"send code\"]", m_driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(5000);
									LoadingWait();
									pageSource = GetPageSource().ToLower();
								}
								string text2 = "";
								DateTime dateTime2 = DateTime.Now.AddMinutes(2.0);
								while (text2 == "" && dateTime2 > DateTime.Now)
								{
									CodeResult code = PhoneUtils.GetCode(phone.SessionId);
									if (code != null && code.Success)
									{
										text2 = code.Code;
									}
									if (text2 != "")
									{
										break;
									}
									Thread.Sleep(10000);
								}
								if (text2 != "")
								{
									List<IWebElement> list5 = ADBHelperCCK.AppGetObjects("//android.widget.EditText", m_driver);
									if (list5 != null && list5.Count == 1)
									{
										list5[0].SendKeys(text2);
										Thread.Sleep(5000);
										pageSource = GetPageSource().ToLower();
									}
								}
							}
						}
					}
					if (pageSource.ToLower().Contains("create username"))
					{
						List<IWebElement> list6 = WaitMes(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"));
						if (list6 != null)
						{
							ShowMessageOnGrid(p_DeviceId, "Create username", gridPhone, main);
							Info.UidLayBai = Utils.UnicodeToKoDau(Info.LastName + Info.FirstName);
							ADBHelperCCK.Tap(p_DeviceId, list6[0].Location.X + list6[0].Size.Width * 98 / 100, list6[0].Location.Y + list6[0].Size.Height / 2);
							Thread.Sleep(1000);
							ADBHelperCCK.InputTextNormal(p_DeviceId, Info.UidLayBai);
							Thread.Sleep(5000);
							pageSource = GetPageSource();
							if (pageSource.Contains("Try a suggested username"))
							{
								IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"skip\")]", m_driver);
								if (webElement4 != null)
								{
									webElement4.Click();
									Thread.Sleep(2000);
								}
							}
							List<IWebElement> list7 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"sign up\")]", m_driver);
							if (list7 != null && list7.Count > 0)
							{
								list7[list7.Count - 1].Click();
								ShowMessageOnGrid(p_DeviceId, "Sign up", gridPhone, main);
								Thread.Sleep(1000);
								pageSource = GetPageSource().ToLower();
								sql.Insert(Info, CategoryId);
								Rename(Info.LastName + " " + Info.FirstName);
								ChangeEmail(change: false);
								PublicInfo();
								BackupAccountTiktok();
							}
						}
						pageSource = GetPageSource().ToLower();
					}
					if (pageSource.Contains("have an account? sign up\""))
					{
						IWebElement webElement5 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"don’t have an account? sign up\"]", m_driver);
						if (webElement5 != null)
						{
							ADBHelperCCK.Tap(p_DeviceId, webElement5.Location.X + webElement5.Size.Width * 9 / 10, webElement5.Location.Y + webElement5.Size.Height / 2);
							Thread.Sleep(1000);
						}
						pageSource = GetPageSource().ToLower();
					}
					if (pageSource.Contains("\"sign up\""))
					{
						pageSource = GetPageSource();
						if ((pageSource.Contains("Send code") || pageSource.Contains("Next")) && pageSource.Contains("\"Email\""))
						{
							ShowMessageOnGrid(p_DeviceId, "Get Email", gridPhone, main);
							List<CCKNode> list8 = cCKDriver.FindElements("//*[@text=\"Email\"]", GetPageSource());
							list8[list8.Count - 1].Click();
							Thread.Sleep(1000);
							Account email2 = emailUti.GetEmail(Info);
							if (email2.accounts == null || email2.accounts.ToString().Length <= 2)
							{
								ShowMessageOnGrid(p_DeviceId, "Hết Email", gridPhone, main);
								return false;
							}
							ShowMessageOnGrid(p_DeviceId, "Email" + email2.accounts, gridPhone, main);
							File.AppendAllLines("Config\\backupEmail.txt", new List<string> { email2.accounts });
							Info.Email = email2.User;
							Info.PassEmail = email2.Pass;
							pageSource = GetPageSource();
							List<IWebElement> list9 = ADBHelperCCK.AppGetObjects("//*[@text=\"Email address\"]", m_driver);
							if (list9 != null && list9.Count > 0)
							{
								list9[list8.Count - 1].Click();
								list9[list8.Count - 1].SendKeys(Info.Email);
								Thread.Sleep(1000);
								List<IWebElement> list10 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
								if (list10 != null)
								{
									ADBHelperCCK.Tap(p_DeviceId, list10[list10.Count - 1].Location.X + list10[list10.Count - 1].Size.Width / 4, list10[list10.Count - 1].Location.Y);
									Thread.Sleep(1000);
									ADBHelperCCK.WaitMe("//*[@text=\"Enter password\"]", m_driver);
									pageSource = GetPageSource();
									WaitCaptchaXoay(m_driver);
									while (pageSource.Contains("\"Next\"") && pageSource.Contains("\"Email\""))
									{
										list10 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
										if (list10 != null)
										{
											ADBHelperCCK.Tap(p_DeviceId, list10[list10.Count - 1].Location.X + list10[list10.Count - 1].Size.Width / 4, list10[list10.Count - 1].Location.Y);
											Thread.Sleep(5000);
										}
										pageSource = GetPageSource().ToLower();
										Thread.Sleep(1000);
									}
								}
							}
						}
					}
					if (pageSource.ToLower().Contains("\"Enter password\"".ToLower()))
					{
						List<CCKNode> list11 = cCKDriver.FindElements("//*[@text=\"Enter password\"]".ToLower(), pageSource.ToLower());
						if (list11 != null)
						{
							ShowMessageOnGrid(p_DeviceId, "Enter password", gridPhone, main);
							ADBHelperCCK.Tap(p_DeviceId, list11[0].Location.X - list11[0].Size.Width / 6, list11[0].Location.Y);
							string text3 = ((entity.PasswordDefault != "") ? entity.PasswordDefault : CreatePassword(10, uppercase: true, specialCharactor: true));
							ADBHelperCCK.InputTextNormal(p_DeviceId, text3);
							Info.Pass = text3;
							Thread.Sleep(1000);
							File.AppendAllLines("tiktok.txt", new List<string> { Info.Email + "|" + text3 + "|" + Info.Email + "|" + Info.PassEmail + "|" + Info.UidLayBai });
							List<IWebElement> list12 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
							if (list12 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, list12[list12.Count - 1].Location.X + list12[list12.Count - 1].Size.Width / 4, list12[list12.Count - 1].Location.Y);
								Thread.Sleep(2000);
							}
						}
						WaitCaptchaXoay(m_driver);
					}
					pageSource = GetPageSource().ToLower();
					if (!pageSource.Contains("maximum number of attempts reached"))
					{
						if (!pageSource.Contains("looks like you're not eligible for TikTok".ToLower()))
						{
							if (pageSource.Contains("your birthday") && pageSource.Contains("sign up") && !flag)
							{
								flag = true;
								ShowMessageOnGrid(p_DeviceId, "Your birthday", gridPhone, main);
								List<CCKNode> list13 = cCKDriver.FindElements("//android.view.View".ToLower(), pageSource);
								if (list13 != null && list13.Count >= 3)
								{
									list13 = list13.Skip(list13.Count - 3).Take(3).ToList();
									_ = list13.Count - 1;
									int x = list13[0].Location.X;
									int y = list13[0].Location.Y;
									_ = list13[0].Size.Width;
									int height = list13[0].Size.Height;
									ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
									Thread.Sleep(1000);
									x = list13[1].Location.X;
									y = list13[1].Location.Y;
									_ = list13[1].Size.Width;
									height = list13[1].Size.Height;
									ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
									Thread.Sleep(1000);
									x = list13[2].Location.X;
									y = list13[2].Location.Y;
									_ = list13[2].Size.Width;
									height = list13[2].Size.Height;
									int num2 = new Random().Next(entity.AgeFrom / 2, entity.AgeTo / 2);
									for (int i = 0; i < num2; i++)
									{
										ADBHelperCCK.Swipe(p_DeviceId, x, y - height / 3, x, y + height / 3, 500);
									}
									Thread.Sleep(2000);
									pageSource = GetPageSource();
									int num3 = 0;
									int num4 = 5;
									while (pageSource.Contains("\"Next\"") && num3++ < num4)
									{
										List<IWebElement> list14 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
										if (list14 != null)
										{
											ShowMessageOnGrid(p_DeviceId, "Your birthday Next", gridPhone, main);
											ADBHelperCCK.Tap(p_DeviceId, list14[list14.Count - 1].Location.X + list14[list14.Count - 1].Size.Width / 4, list14[list14.Count - 1].Location.Y);
											Thread.Sleep(5000);
										}
										pageSource = GetLocalSource();
										if (pageSource.Contains("Too many attempts") || pageSource.Contains("Sorry, looks like you're not eligible for TikTok"))
										{
											File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
											Thread.Sleep(1800000);
											return false;
										}
									}
									if (num3 == num4)
									{
										return false;
									}
								}
							}
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("code was emailed to"))
							{
								ShowMessageOnGrid(p_DeviceId, "Get Code Email", gridPhone, main);
								string code2 = emailUti.GetCode(Info.Email, Info.PassEmail);
								if (code2 == "")
								{
									pageSource = GetPageSource();
									ADBHelperCCK.AppGetObject("//*[@text=\"Resend code\"]", m_driver)?.Click();
									code2 = emailUti.GetCode(Info.Email, Info.PassEmail);
								}
								pageSource = GetPageSource();
								List<IWebElement> list15 = ADBHelperCCK.AppGetObjects("//android.widget.EditText", m_driver);
								if (list15 != null && list15.Count == 1)
								{
									list15[0].SendKeys(code2);
									Thread.Sleep(5000);
									pageSource = GetPageSource().ToLower();
									if (pageSource.Contains("text=\"too many attempts. try again later.\""))
									{
										return false;
									}
									for (int j = 0; j < 5; j++)
									{
										ADBHelperCCK.Back(p_DeviceId);
									}
									pageSource = GetPageSource().ToLower();
									GotoTab(Tabs.Profile);
									pageSource = GetPageSource().ToLower();
									if (pageSource.Contains("text=\"@"))
									{
										List<IWebElement> list16 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"@\")]", m_driver);
										if (list16 != null)
										{
											Info.Uid = list16[0].Text.TrimStart('@');
										}
									}
									ShowMessageOnGrid(p_DeviceId, "Created Account", gridPhone, main);
									sql.Insert(Info, CategoryId);
									Rename(Info.LastName + " " + Info.FirstName);
									PublicInfo();
									return true;
								}
							}
							if (pageSource.Contains("Too many attempts. Try again later"))
							{
								ShowMessageOnGrid(p_DeviceId, "Too many attempts", gridPhone, main);
								if (!string.IsNullOrWhiteSpace(Info.Email))
								{
									File.AppendAllLines("Config\\too_many_attempts_email.txt", new List<string> { Info.Email + "|" + Info.PassEmail });
								}
								ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								return false;
							}
							continue;
						}
						File.AppendAllLines(CaChuaConstant.EMAIL_FROMFILE, new List<string> { Info.Email + "|" + Info.PassEmail });
						return false;
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Register ", ex.Message);
			}
			return false;
		}

		public bool RestoreAccountTiktok(bool restoreOnly = false)
		{
			try
			{
				string text = Application.StartupPath + $"\\Authentication\\{Info.Uid}\\{CaChuaConstant.PACKAGE_NAME}\\shared_prefs\\aweme_user.xml";
				if (File.Exists(text))
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(Utils.ReadTextFile(text));
					XmlNode xmlNode = xmlDocument.SelectSingleNode("//*[@name=\"current_foreground_uid\"]");
					if (xmlNode != null)
					{
						string innerText = xmlNode.InnerText;
						XmlNode xmlNode2 = xmlDocument.SelectSingleNode($"//*[@name=\"{innerText}_account_user_info\"]");
						_ = xmlNode2.InnerText;
						xmlNode2 = xmlDocument.SelectSingleNode($"//*[@name=\"{innerText}_significant_user_info\"]");
						string innerText2 = xmlNode2.InnerText;
						dynamic val = new JavaScriptSerializer().DeserializeObject(innerText2);
						if (val != null && val.ContainsKey("unique_id"))
						{
							dynamic val2 = val["unique_id"].ToString();
							if (val2 != Info.Uid)
							{
								Utils.ClearFolder(Application.StartupPath + $"\\Authentication\\{Info.Uid}");
								Utils.CCKLog("Log\\wrongaccountn.txt", val2, Info.Uid);
								return false;
							}
						}
					}
				}
			}
			catch
			{
			}
			ADBHelperCCK.StopApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			_ = Info.Uid;
			string text2 = $"{Application.StartupPath}\\Authentication\\{Info.Email}";
			if (Info.Uid.StartsWith("@"))
			{
				Info.Uid = Info.Uid.Substring(1);
			}
			string text3 = $"{Application.StartupPath}\\Authentication\\{Info.Uid}";
			if (!Directory.Exists(text3) && Directory.Exists(text2))
			{
				Directory.Move(text2, text3);
				Thread.Sleep(2000);
			}
			if (Directory.Exists(text3))
			{
				ADBHelperCCK.SetStoragePermission(p_DeviceId);
				ADBHelperCCK.CloseApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				for (int i = 0; i < 2; i++)
				{
					ADBHelperCCK.ExecuteCMD(p_DeviceId, " " + string.Format(GetFunctionByKey("2dbf9139515a4d6ebc115360f9a0f0ff"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME));
				}
				if (restoreOnly)
				{
					return true;
				}
				Thread.Sleep(2000);
				ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				Thread.Sleep(2000);
				string pageSource = GetPageSource();
				GotoTab(Tabs.Profile);
				pageSource = GetPageSource();
				if (pageSource.Contains("@" + Info.Uid))
				{
					return true;
				}
			}
			new Task(delegate
			{
				ADBHelperCCK.AddAndroidPermission(p_DeviceId, new List<string> { "android.permission.READ_CONTACTS", "android.permission.READ_EXTERNAL_STORAGE", "com.android.providers.contacts", "com.google.android.syncadapters.contacts", "com.qualcomm.simcontacts" });
				ADBHelperCCK.SetPortrait(p_DeviceId);
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell ime set com.android.adbkeyboard/.AdbIME");
			}).Start();
			return false;
		}

		public void BackupAccountTiktok(bool zip = false)
		{
			if (!deviceEntity.Rooted)
			{
				return;
			}
			if (Info.Uid == "" && Info.Email.Trim().Length > 0)
			{
				Info.Uid = Info.Email.Trim();
			}
			bool flag = false;
			string path = $"{Application.StartupPath}\\Authentication\\{Info.Uid}\\";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			path = $"{Application.StartupPath}\\Authentication\\{Info.Uid}\\{CaChuaConstant.PACKAGE_NAME}";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (flag)
			{
				AccountUtils.Backup(p_DeviceId, Info.Uid);
				return;
			}
			string text = ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("ba61c88ca4914efc8c7dd3532b7dc595"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			text = text + ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("12275980ac9a4e62b25052eab116a5b2"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			text = text + ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("735216fada724fa488fce4b648eb42b4"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			text = text + ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("2ab48aa78ccd4d7b8fbc5584671022a8"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			text = text + ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("8a145361fcf14514813dff0965134f80"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			text = text + ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("9a217dd694de4cfc8c952eca896779c5"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			text = text + ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("c74a4be5f3974985bbfda512597b514d"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			text = text + ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("599e0840cd8c412dae3075a6fba5e69a"), Application.StartupPath, Info.Uid, CaChuaConstant.PACKAGE_NAME)) + Environment.NewLine;
			try
			{
				string text2 = Application.StartupPath + $"\\Authentication\\{Info.Uid}\\{CaChuaConstant.PACKAGE_NAME}\\shared_prefs\\aweme_user.xml";
				if (!File.Exists(text2))
				{
					return;
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(Utils.ReadTextFile(text2));
				XmlNode xmlNode = xmlDocument.SelectSingleNode("//*[@name=\"current_foreground_uid\"]");
				if (xmlNode == null)
				{
					return;
				}
				string innerText = xmlNode.InnerText;
				XmlNode xmlNode2 = xmlDocument.SelectSingleNode($"//*[@name=\"{innerText}_account_user_info\"]");
				_ = xmlNode2.InnerText;
				xmlNode2 = xmlDocument.SelectSingleNode($"//*[@name=\"{innerText}_significant_user_info\"]");
				string innerText2 = xmlNode2.InnerText;
				dynamic val = new JavaScriptSerializer().DeserializeObject(innerText2);
				if (val != null && val.ContainsKey("unique_id"))
				{
					dynamic val2 = val["unique_id"].ToString();
					if (val2 != Info.Uid)
					{
						Utils.ClearFolder(Application.StartupPath + $"\\Authentication\\{Info.Uid}");
					}
					dynamic val3 = ((val.ContainsKey("nickname")) ? val["nickname"].ToString() : "");
					if (val3 != "" && val2 != "")
					{
						sql.ExecuteQuery($"Update Account set name='{(object)val3}' where id='{(object)val2}'");
					}
				}
			}
			catch
			{
			}
		}

		public string CreatePassword(int length, bool uppercase = false, bool specialCharactor = false)
		{
			char[] array = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
			char[] array2 = "@#*".ToCharArray();
			char[] array3 = "1234567890".ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			Random random = new Random();
			while (0 < length--)
			{
				stringBuilder.Append("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[random.Next("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length)]);
			}
			string text = stringBuilder.ToString();
			if (uppercase)
			{
				text = array[random.Next(0, array.Length)].ToString() + array3[random.Next(0, array3.Length)] + text;
			}
			if (specialCharactor)
			{
				text += array2[random.Next(array2.Length)];
			}
			return text;
		}

		private IWebElement WaitMe(string xpath, int timeout = 5)
		{
			try
			{
				Thread.Sleep(500);
				int num = 0;
				while (num < timeout)
				{
					IWebElement webElement = ADBHelperCCK.AppGetObject(xpath, m_driver);
					if (webElement == null)
					{
						num++;
						Thread.Sleep(1000);
						continue;
					}
					return webElement;
				}
			}
			catch
			{
			}
			return null;
		}

		private List<IWebElement> WaitMes(string xpath)
		{
			try
			{
				Thread.Sleep(1000);
				int num = 10;
				int num2 = 0;
				while (num2 < num)
				{
					List<IWebElement> list = ADBHelperCCK.AppGetObjects(xpath, m_driver);
					if (list == null)
					{
						num2++;
						Thread.Sleep(1000);
						continue;
					}
					return list;
				}
			}
			catch
			{
			}
			return null;
		}

		public IReadOnlyCollection<IWebElement> WaitElements(By selector)
		{
			WebDriverWait webDriverWait = new WebDriverWait(m_driver, new TimeSpan(0, 0, 10));
			IReadOnlyCollection<IWebElement> results = null;
			try
			{
				webDriverWait.Until(delegate(IWebDriver driver)
				{
					try
					{
						ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(selector);
						if (readOnlyCollection.Any())
						{
							results = readOnlyCollection;
							return true;
						}
					}
					catch (StaleElementReferenceException)
					{
					}
					return false;
				});
			}
			catch (WebDriverTimeoutException)
			{
				return null;
			}
			return results;
		}

		public void RegisterTiktokLiteByEmail(DataGridView gridPhone, Form main, int CategoryId, RegNickEntity entity)
		{
			CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
			ADBHelperCCK.ClearAppData(p_DeviceId, "com.zhiliaoapp.musically.go");
			while (true)
			{
				ADBHelperCCK.OpenApp(p_DeviceId, "com.zhiliaoapp.musically.go");
				Thread.Sleep(1000);
				string pageSource = GetPageSource();
				Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
				while (true)
				{
					pageSource = GetPageSource().ToLower();
					if (pageSource.Contains("continue with facebook"))
					{
						if (pageSource.Contains("sign up"))
						{
							List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"sign up\")]", m_driver);
							if (list != null && list.Count > 0)
							{
								GridViewMessageLog(p_DeviceId, "Sign up", Color.Green);
								list[list.Count - 1].Click();
								Thread.Sleep(1000);
								pageSource = GetPageSource().ToLower();
								List<CCKNode> list2 = cCKDriver.FindElements("//com.lynx.tasm.behavior.ui.text.FlattenUIText[@text=\"Use phone or email\"]", GetPageSource());
								if (list2 != null)
								{
									list2[list2.Count - 1].Click();
									continue;
								}
							}
						}
						List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//*[lower-case(@text)=\"continue with facebook\"]", m_driver);
						if (list3 != null && list3.Count > 0)
						{
							list3[list3.Count - 1].Click();
							ADBHelperCCK.WaitMe("//*[contains(lower-case(@text),\"continue as\")]", m_driver);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("continue as"))
					{
						List<IWebElement> list4 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"continue as\")]", m_driver);
						if (list4 != null && list4.Count > 0)
						{
							list4[list4.Count - 1].Click();
							WaitMe("//*[@text=\"Next\"]");
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("your birthday won"))
					{
						GridViewMessageLog(p_DeviceId, "Birthday", Color.Green);
						List<CCKNode> list5 = cCKDriver.FindElements("//com.bytedance.ies.xelement.pickview.LynxPickerViewColumn", GetPageSource());
						if (list5 != null && list5.Count == 3)
						{
							int x = list5[0].Location.X;
							int y = list5[0].Location.Y;
							_ = list5[0].Size.Width;
							int height = list5[0].Size.Height;
							ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
							Thread.Sleep(1000);
							x = list5[1].Location.X;
							y = list5[1].Location.Y;
							_ = list5[1].Size.Width;
							height = list5[1].Size.Height;
							ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
							Thread.Sleep(1000);
							x = list5[2].Location.X;
							y = list5[2].Location.Y;
							_ = list5[2].Size.Width;
							height = list5[2].Size.Height;
							int num = new Random().Next(8, 12);
							for (int i = 0; i < num; i++)
							{
								ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
							}
							Thread.Sleep(2000);
							pageSource = GetPageSource();
							List<CCKNode> list6 = cCKDriver.FindElements("//*[@text=\"Next\"]", GetPageSource());
							if (list6 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, list6[1].Location.X + list6[1].Size.Width / 4, list6[1].Location.Y + list6[1].Size.Height / 2);
								Thread.Sleep(5000);
								pageSource = GetPageSource().ToLower();
								if (!pageSource.Contains("sign up"))
								{
									list6 = cCKDriver.FindElements("//*[@text=\"Next\"]", GetPageSource());
									if (list6 != null)
									{
										break;
									}
								}
							}
						}
					}
					if (pageSource.Contains("Swipe up for more"))
					{
						CCKDriver cCKDriver2 = new CCKDriver(p_DeviceId);
						CCKNode cCKNode = cCKDriver2.FindElement("//*[@text=\"Swipe up for more\"]", pageSource);
						if (cCKNode != null)
						{
							int num2 = cCKNode.Location.X + cCKNode.Size.Width / 2;
							int y2 = cCKNode.Location.Y + cCKNode.Size.Height / 2;
							for (int j = 0; j < 4; j++)
							{
								ADBHelperCCK.Swipe(p_DeviceId, num2, y2, num2, screenResolution.Y / 4);
								Thread.Sleep(500);
							}
							pageSource = GetLocalSource();
							if (pageSource.Contains("Swipe up for more"))
							{
								Utils.CCKLog("Swipe up for more", "Not Scroll " + DateTime.Now.ToString());
							}
						}
						cCKDriver2 = null;
						cCKNode = null;
						pageSource = GetLocalSource();
					}
					if (pageSource.Contains("\"me\""))
					{
						GridViewMessageLog(p_DeviceId, "Click Me", Color.Green);
						List<CCKNode> list7 = cCKDriver.FindElements("//*[@text=\"Me\"]", GetPageSource());
						if (list7 != null)
						{
							list7[0].Click();
							Thread.Sleep(5000);
							pageSource = GetPageSource().ToLower();
							if (pageSource.Contains("@" + Info.Uid.ToLower()) || pageSource.Contains("text=\"set up profile\"") || pageSource.Contains("text=\"edit profile\""))
							{
								List<CCKNode> list8 = cCKDriver.FindElements("//*[contains(@text,\"@\")]", GetPageSource());
								sql.Insert(new TTItems
								{
									Uid = list8[list8.Count - 1].Text.Replace("@", ""),
									Email = Info.Email,
									Pass = ((Info.Pass != null) ? Info.Pass : ""),
									PassEmail = Info.PassEmail
								}, CategoryId);
								return;
							}
						}
					}
					if (pageSource.Contains("\"log in\""))
					{
						List<CCKNode> list9 = cCKDriver.FindElements("//*[@text=\"Log in\"]", GetPageSource());
						if (list9 != null)
						{
							int index = list9.Count - 1;
							ADBHelperCCK.Tap(p_DeviceId, list9[index].Location.X + list9[index].Size.Width / 4, list9[index].Location.Y + list9[index].Size.Height / 2);
							pageSource = GetPageSource().ToLower();
						}
					}
					if (pageSource.Contains("create username"))
					{
						GridViewMessageLog(p_DeviceId, "Create Username", Color.Green);
						List<IWebElement> list10 = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.behavior.ui.view.UIView[string-length(@text) > 0 and string-length(@content-desc) > 0 ]", m_driver);
						if (list10 != null && list10.Count > 3)
						{
							list10[list10.Count - 1].Click();
							Thread.Sleep(2000);
							List<IWebElement> list11 = WaitMes("//*[lower-case(@text)=\"skip\" or lower-case(@content-desc)=\"skip\"]");
							if (list11 != null)
							{
								list11[list11.Count - 1].Click();
								Thread.Sleep(2000);
							}
						}
						else
						{
							pageSource = GetPageSource();
							List<IWebElement> list12 = WaitMes(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"));
							if (list12 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, list12[0].Location.X + list12[0].Size.Width * 98 / 100, list12[0].Location.Y + list12[0].Size.Height / 2);
								Thread.Sleep(2000);
								Info.UidLayBai = CreatePassword(5).ToLower() + "_" + Info.Uid;
								ADBHelperCCK.InputTextNormal(p_DeviceId, Info.UidLayBai);
								Thread.Sleep(5000);
								list12 = WaitMes("//com.lynx.tasm.behavior.ui.view.UIView[lower-case(@text)=\"skip\" or lower-case(@content-desc)=\"skip\"]");
								if (list12 != null && list12.Count > 0)
								{
									ADBHelperCCK.Tap(p_DeviceId, list12[0].Location.X * 11 / 10, list12[0].Location.Y + list12[0].Size.Height / 2);
									Thread.Sleep(2000);
									int num3 = new Random().Next(1, 5);
									for (int k = 0; k < num3; k++)
									{
										ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4, 1000);
									}
									pageSource = GetPageSource();
								}
							}
						}
					}
					if (pageSource.Contains("\"sign up\""))
					{
						pageSource = GetPageSource();
						if (pageSource.Contains("Send code") && pageSource.Contains("\"Email\""))
						{
							GridViewMessageLog(p_DeviceId, "Send code", Color.Green);
							List<CCKNode> list13 = cCKDriver.FindElements("//*[@text=\"Email\"]", pageSource);
							list13[list13.Count - 1].Click();
							Thread.Sleep(1000);
							EmailUtils emailUtils = new EmailUtils();
							Account email = emailUtils.GetEmail(Info);
							Info.Email = email.User;
							Info.PassEmail = email.Pass;
							Thread.Sleep(2000);
							Thread.Sleep(500);
							ADBHelperCCK.InputTextNormal(p_DeviceId, Info.Email);
							Thread.Sleep(1000);
							List<IWebElement> list14 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
							if (list14 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, list14[list14.Count - 1].Location.X + list14[list14.Count - 1].Size.Width / 4, list14[list14.Count - 1].Location.Y);
								Thread.Sleep(5000);
								pageSource = GetPageSource();
								List<CCKNode> list15 = cCKDriver.FindElements("//com.bytedance.ies.xelement.input.LynxInputView", GetPageSource());
								if (list15 != null)
								{
									ADBHelperCCK.Tap(p_DeviceId, list15[0].Location.X - list15[0].Size.Width / 6, list15[0].Location.Y);
									string text = CreatePassword(10, uppercase: true, specialCharactor: true);
									Info.Pass = text;
									ADBHelperCCK.InputTextNormal(p_DeviceId, text);
									Thread.Sleep(1000);
									list14 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
									if (list14 != null)
									{
										ADBHelperCCK.Tap(p_DeviceId, list14[list14.Count - 1].Location.X + list14[list14.Count - 1].Size.Width / 4, list14[list14.Count - 1].Location.Y);
										Thread.Sleep(5000);
										ResolverCaptcha();
									}
								}
							}
							pageSource = GetPageSource();
							if (pageSource.Contains("Enter 6-digit code"))
							{
								GridViewMessageLog(p_DeviceId, "Waiting for code", Color.Green);
								string code = emailUtils.GetCode(Info.Email, Info.PassEmail);
								if (code == "")
								{
									cCKDriver.FindElement("//*[contains(@text,\"Resend code\") or contains(@content-desc,\"Resend code\")]", GetPageSource())?.Click();
									code = emailUtils.GetCode(Info.Email, Info.PassEmail);
								}
								if (code != "")
								{
									GridViewMessageLog(p_DeviceId, "Code: " + code, Color.Green);
									List<CCKNode> list16 = cCKDriver.FindElements("//com.bytedance.ies.xelement.input.LynxInputView", GetPageSource());
									list16[0].CCKSendKeys(code);
									Thread.Sleep(3000);
									pageSource = GetPageSource();
									if (pageSource.Contains("Create username"))
									{
										List<CCKNode> list17 = cCKDriver.FindElements("//com.bytedance.ies.xelement.input.LynxInputView", pageSource);
										if (list17 != null && list17.Count > 0)
										{
											list17[0].Clear();
											list17[0].Click();
											list17[0].CCKSendKeys(Utils.UnicodeToKoDau(Info.FirstName + Info.LastName));
											Thread.Sleep(5000);
											pageSource = GetPageSource();
											if (pageSource.Contains("Try a suggested username, or enter a new one"))
											{
												list17 = cCKDriver.FindElements("//com.lynx.tasm.behavior.ui.text.FlattenUIText", pageSource);
												if (list17 != null && list17.Count > 0)
												{
													list17[list17.Count - 1].Click();
													Thread.Sleep(1000);
												}
											}
										}
									}
								}
							}
						}
						List<CCKNode> list18 = cCKDriver.FindElements("//com.lynx.tasm.behavior.ui.text.FlattenUIText[@text=\"Sign up\" and @content-desc=\"Sign up\"]", GetPageSource());
						if (list18 != null)
						{
							GridViewMessageLog(p_DeviceId, "Sign up Click", Color.Green);
							ADBHelperCCK.Tap(p_DeviceId, list18[1].Location.X + list18[1].Size.Width / 4, list18[1].Location.Y + list18[1].Size.Height / 2);
							Thread.Sleep(2000);
							for (int l = 0; l < 5; l++)
							{
								ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4, 1000);
							}
						}
					}
					pageSource = GetPageSource();
					if (!pageSource.Contains("com.lynx.tasm.ui.image.UIImage".ToLower()))
					{
						Thread.Sleep(1000);
						continue;
					}
					List<CCKNode> list19 = cCKDriver.FindElements("//com.lynx.tasm.behavior.ui.text.UIText[contains(@content-desc,\"\u200e@\")]", GetPageSource());
					if (list19 != null && list19.Count > 0)
					{
						Info.UidLayBai = list19[0].GetAttribute("content-desc");
					}
					List<CCKNode> list20 = cCKDriver.FindElements("//*[@text=\"Me\" or @content-desc=\"Me\"]", GetPageSource());
					if (list20 == null || list20.Count <= 0)
					{
						return;
					}
					list20[0].Click();
					Thread.Sleep(2000);
					pageSource = GetPageSource();
					list20 = cCKDriver.FindElements("//com.lynx.tasm.ui.image.UIImage", GetPageSource());
					if (list20 == null || list20.Count <= 0)
					{
						return;
					}
					list20[0].Click();
					Thread.Sleep(1000);
					EmailUtils emailUtils2 = new EmailUtils();
					string text2 = "";
					list20 = cCKDriver.FindElements("//*[@text=\"Manage account\"]", GetPageSource());
					if (list20 == null || list20.Count <= 0)
					{
						return;
					}
					list20[0].Click();
					Thread.Sleep(1000);
					pageSource = GetPageSource();
					list20 = cCKDriver.FindElements("//*[@text=\"Email\"]", GetPageSource());
					if (list20 != null && list20.Count > 0)
					{
						list20[0].Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
						if (webElement != null)
						{
							ADBHelperCCK.Tap(p_DeviceId, webElement.Location.X + webElement.Size.Width / 4, webElement.Location.Y + webElement.Size.Height / 2);
							Thread.Sleep(1000);
							Account email2 = emailUtils2.GetEmail(Info);
							Info.Email = email2.User;
							Info.PassEmail = email2.Pass;
							ADBHelperCCK.InputTextNormal(p_DeviceId, Info.Email);
							Thread.Sleep(1000);
							webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"send code\"]", m_driver);
							if (webElement != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, webElement.Location.X + webElement.Size.Width / 4, webElement.Location.Y + webElement.Size.Height / 2);
								Thread.Sleep(5000);
								pageSource = GetPageSource();
								Thread.Sleep(2000);
								text2 = new EmailUtils().GetCode(Info.Email, Info.PassEmail);
								if (text2 == "" || text2 != "LOGIN failed")
								{
									return;
								}
							}
						}
					}
					Thread.Sleep(2000);
					pageSource = GetPageSource();
					List<CCKNode> list21 = cCKDriver.FindElements("//com.lynx.tasm.behavior.ui.LynxFlattenUI", GetPageSource());
					char[] array = text2.ToCharArray();
					if (list21 != null && array.Length < list21.Count)
					{
						int num4 = 0;
						for (int m = list21.Count - 7; m < list21.Count - 1; m++)
						{
							ADBHelperCCK.Tap(p_DeviceId, list21[m].Location.X + list21[m].Size.Width / 4, list21[m].Location.Y + list21[m].Size.Height / 2);
							ADBHelperCCK.InputTextNormal(p_DeviceId, array[num4++].ToString());
						}
						Thread.Sleep(5000);
						Info.Uid = Info.Email;
						BackupAccountTiktok();
					}
					list20 = cCKDriver.FindElements("//*[@text=\"Password\" or @content-desc=\"Password\"]", GetPageSource());
					if (list20 == null)
					{
						return;
					}
					list20[0].Click();
					Thread.Sleep(2000);
					string code2;
					while (true)
					{
						code2 = new EmailUtils().GetCode(Info.Email, Info.PassEmail);
						if (code2 != text2 && code2 != "")
						{
							break;
						}
						Thread.Sleep(5000);
					}
					text2 = code2;
					list21 = cCKDriver.FindElements("//com.lynx.tasm.behavior.ui.LynxFlattenUI", GetPageSource());
					array = text2.ToCharArray();
					if (list21 != null && array.Length < list21.Count)
					{
						int num5 = 0;
						for (int n = list21.Count - 7; n < list21.Count - 1; n++)
						{
							ADBHelperCCK.Tap(p_DeviceId, list21[n].Location.X + list21[n].Size.Width / 4, list21[n].Location.Y + list21[n].Size.Height / 2);
							ADBHelperCCK.InputTextNormal(p_DeviceId, array[num5++].ToString());
						}
						Thread.Sleep(5000);
					}
					List<CCKNode> list22 = cCKDriver.FindElements("//com.bytedance.ies.xelement.input.LynxInputView", GetPageSource());
					if (list22 != null)
					{
						ADBHelperCCK.Tap(p_DeviceId, list22[0].Location.X - list22[0].Size.Width / 6, list22[0].Location.Y);
						string text3 = ((entity.PasswordDefault != "") ? entity.PasswordDefault : CreatePassword(10, uppercase: true, specialCharactor: true));
						ADBHelperCCK.InputTextNormal(p_DeviceId, text3);
						File.AppendAllLines("tiktok.txt", new List<string> { Info.Email + "|" + text3 + "|" + Info.Email + "|" + Info.PassEmail + "|" + Info.UidLayBai });
					}
					pageSource = GetPageSource();
					List<CCKNode> list23 = cCKDriver.FindElements("//*[@text=\"Next\"]", GetPageSource());
					if (list23 != null)
					{
						ADBHelperCCK.Tap(p_DeviceId, list23[1].Location.X + list23[1].Size.Width / 4, list23[1].Location.Y + list23[1].Size.Height / 2);
						Thread.Sleep(5000);
						BackupAccountTiktok();
					}
					return;
				}
				ADBHelperCCK.CloseApp(p_DeviceId, "com.zhiliaoapp.musically.go");
			}
		}

		private void ResolverCaptcha()
		{
			Utils.CCKLog("ResolverCaptcha", "Đã vào trong ham");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell ime disable com.android.adbkeyboard/.AdbIME");
			try
			{
				string text = GetLocalSource().ToLower();
				List<string> list = new List<string> { "choose your interests" };
				foreach (string item in list)
				{
					if (text.Contains(item))
					{
						return;
					}
				}
				if (screen.X == 720 && screen.Y == 1600)
				{
					for (int i = 0; i < 2; i++)
					{
						ADBHelperCCK.BackToHome(p_DeviceId);
						Thread.Sleep(500);
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent KEYCODE_APP_SWITCH");
						Thread.Sleep(500);
					}
					ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
					Thread.Sleep(2000);
					text = GetLocalSource();
					for (int j = 0; j < 5; j++)
					{
						ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
						Thread.Sleep(2000);
						text = GetLocalSource();
						if (text.Contains("captcha-verify-image"))
						{
							break;
						}
					}
					text = GetLocalSource();
				}
				text = GetLocalSource();
				int num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_CAPTCHA));
				Thread.Sleep(1000 * num);
				text = GetLocalSource();
				if (text.Contains("Drag the slider to fit the puzzle") && unlockbyhand)
				{
					Thread.Sleep(1000 * num * 3);
					text = GetLocalSource();
					ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Chờ giải Captcha bằng tay");
				}
				bool flag = true;
				int num2 = 0;
				OmoCaptcha omoCaptcha = new OmoCaptcha(Utils.ReadTextFile(CaChuaConstant.MMOCAPTCHA));
				while (text.Contains("captcha-verify-image") || text.Contains("Verify to continue") || text.Contains("Drag the slider to fit the puzzle"))
				{
					text = GetLocalSource();
					while (text.Contains("Drag the slider to fit the puzzle"))
					{
						Thread.Sleep(5000);
						text = GetLocalSource();
						do
						{
							List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//android.app.Dialog/android.view.View[2]/android.widget.Image", m_driver);
							if (list2 == null || list2.Count != 2)
							{
								break;
							}
							Screenshot screenshot = ((ITakesScreenshot)m_driver).GetScreenshot();
							string text2 = string.Format("{0}_{1}", p_DeviceId.Replace(":", ""), DateTime.Now.ToFileTime());
							screenshot.SaveAsFile("Temp\\" + text2 + ".png", ScreenshotImageFormat.Png);
							string text3 = Application.StartupPath + "\\Temp\\xoay_" + text2 + ".png";
							using (Bitmap image = new Bitmap("Temp\\" + text2 + ".png"))
							{
								int width = list2[0].Size.Width;
								int height = list2[0].Size.Height;
								using Bitmap bitmap = new Bitmap(width, height);
								using (Graphics graphics = Graphics.FromImage(bitmap))
								{
									graphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(list2[0].Location.X, list2[0].Location.Y, width, height), GraphicsUnit.Pixel);
								}
								bitmap.Save(text3, ImageFormat.Jpeg);
							}
							achicaptcha achicaptcha2 = new achicaptcha(Utils.ReadTextFile(CaChuaConstant.AHICAPTCHA));
							int num3 = achicaptcha2.RotateCaptcha(text3);
							IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Drag the slider to fit the puzzle\"]", m_driver);
							IWebElement webElement2 = ADBHelperCCK.AppGetObject("//android.webkit.WebView/android.webkit.WebView/android.view.View/android.app.Dialog/android.view.View[3]/android.view.View/android.view.View", m_driver);
							IWebElement webElement3 = ADBHelperCCK.AppGetObject("//android.app.Dialog/android.view.View[3]/android.view.View/android.view.View/android.view.View/android.widget.Image", m_driver);
							if (webElement3 == null || webElement == null || webElement2 == null)
							{
								break;
							}
							float num4 = webElement.Size.Width - 2 * (webElement2.Location.X - webElement.Location.X) - webElement2.Size.Width;
							double num5 = Math.Round(num4 * (float)num3 / 180f) + (double)(webElement2.Location.X - webElement.Location.X);
							ADBHelperCCK.TapLong(p_DeviceId, webElement2.Location.X + webElement2.Size.Width / 2, webElement3.Location.Y + webElement3.Size.Height / 2, (double)(webElement2.Location.X + webElement2.Size.Width / 2) + num5, webElement3.Location.Y + webElement3.Size.Height * 3 / 4);
							screenshot = ((ITakesScreenshot)m_driver).GetScreenshot();
							screenshot.SaveAsFile("xong.png");
							Thread.Sleep(5000);
							text = GetLocalSource();
						}
						while (text.Contains("\"Refresh\""));
					}
					try
					{
						while (text.Contains("Select 2 objects that are the same shape") && num2++ < 5)
						{
							if (Utils.ReadTextFile(CaChuaConstant.MMOCAPTCHA) != "")
							{
								Utils.CCKLog("Captcha chu bat dau", "");
								List<Point> list3 = omoCaptcha.MakeRequestChose2Point(m_driver, screen.X, screen.Y);
								if (list3 == null)
								{
									Utils.CCKLog("Captcha 2 objects ", "Không tìm thấy dữ liệu");
									text = GetLocalSource();
									continue;
								}
								Utils.CCKLog("Select 2 objects", js.Serialize(list3));
								if (list3.Count == 0)
								{
									break;
								}
								for (int k = 0; k < list3.Count; k++)
								{
									ADBHelperCCK.Tap(p_DeviceId, list3[k].X, list3[k].Y);
									Thread.Sleep(1000);
								}
								IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[@text=\"Confirm\"]", m_driver);
								if (webElement4 != null)
								{
									webElement4.Click();
									Thread.Sleep(3000);
								}
								Thread.Sleep(5000);
								text = GetLocalSource();
							}
							else
							{
								ADBHelperCCK.CloseApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								Thread.Sleep(2000);
								ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								Thread.Sleep(5000);
							}
						}
					}
					catch (Exception ex)
					{
						Utils.CCKLog("Captcha 2 objects ", ex.Message);
					}
					while (true)
					{
						try
						{
							while (text.Contains("No internet connection"))
							{
								ADBHelperCCK.AppGetObject("//*[@text=\"Refresh\"]", m_driver)?.Click();
								Thread.Sleep(10000);
								text = GetLocalSource();
							}
							if (flag)
							{
								flag = false;
								WaitMe("//*[@resource-id='captcha-verify-image']", 1);
							}
							List<IWebElement> list4 = ADBHelperCCK.AppGetObjects("//*[@resource-id='captcha-verify-image'] | //android.app.Dialog/android.view.View[2]/android.widget.Image", m_driver);
							if (list4 == null || list4.Count <= 0)
							{
								break;
							}
							IWebElement webElement5 = list4[0];
							string arg = Guid.NewGuid().ToString("N");
							string text4 = $"max{arg}.jpg";
							string source = Application.StartupPath + "\\Temp\\small-x.png";
							string text5 = Application.StartupPath + $"\\Temp\\small-x_{screen.X}_{screen.Y}.png";
							if (File.Exists(text5))
							{
								string text6 = Application.StartupPath + $"\\Temp\\screen_{screen.X}_{screen.Y}.png";
								if (!File.Exists(text6))
								{
									Screenshot screenshot2 = m_driver.GetScreenshot();
									screenshot2.SaveAsFile(text6);
								}
								source = text5;
							}
							string text7 = Application.StartupPath + "\\Temp\\" + text4;
							if (!Directory.Exists(Application.StartupPath + "\\Temp\\"))
							{
								Directory.CreateDirectory(Application.StartupPath + "\\Temp\\");
							}
							try
							{
								ADBHelperCCK.CropImage(p_DeviceId, text4, text7, Convert.ToInt32(webElement5.Location.X) + 200, Convert.ToInt32(webElement5.Location.Y), Convert.ToInt32(webElement5.Size.Width) - 200, Convert.ToInt32(webElement5.Size.Height));
								Point point = ADBHelperCCK.FindImages(text7, source, 1);
								if (point.X == 0 && point.Y == 0)
								{
									text = GetLocalSource();
									IWebElement webElement6 = ADBHelperCCK.AppGetObject("//*[@text=\"Refresh\" or @content-desc=\"Refresh\" or @text=\"Reload\" or @resource-id=\"refresh-button\"]", m_driver);
									if (webElement6 != null)
									{
										webElement6.Click();
									}
									else
									{
										Utils.CCKLog("Log\\captcha_0.txt", "Khong Click Refresh", text);
									}
									Thread.Sleep(5000);
									Utils.DeleteFile(text4);
									Utils.DeleteFile(text7);
									continue;
								}
								Utils.CCKLog("Log\\captcha.txt", point.X + ":" + point.Y, "FindImages - Found");
								IWebElement webElement7 = ADBHelperCCK.AppGetObject("//android.app.Dialog/android.view.View[3]/android.view.View/android.view.View/android.view.View | //*[@resource-id=\"secsdk-captcha-drag-wrapper\"]/android.view.View", m_driver);
								if (webElement7 != null)
								{
									ADBHelperCCK.TapLong(p_DeviceId, webElement7.Location.X + webElement7.Size.Width / 2, webElement7.Location.Y + webElement7.Size.Height / 2, webElement7.Location.X + Convert.ToInt32(point.X) * 97 / 100 + 200 + webElement7.Size.Width / 2, webElement7.Location.Y + webElement7.Size.Height / 2);
								}
								Thread.Sleep(1000);
								break;
							}
							catch (Exception)
							{
								break;
							}
							finally
							{
								try
								{
									Utils.DeleteFile(text4);
									Utils.DeleteFile(text7);
								}
								catch
								{
								}
							}
						}
						catch (Exception)
						{
						}
						break;
					}
					text = GetLocalSource();
				}
			}
			catch (Exception ex4)
			{
				Utils.CCKLog("ResolverCaptcha -", ex4.Message);
			}
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell ime enable com.android.adbkeyboard/.AdbIME");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell ime set com.android.adbkeyboard/.AdbIME");
		}

		private void WaitLoading()
		{
			Thread.Sleep(5000);
		}

		internal void LikePost()
		{
			Utils.LogFunction("LikePost", "");
			string text = Utils.ReadTextFile(CaChuaConstant.LIKE_POST);
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			LikePostItem likePostItem = new LikePostItem();
			try
			{
				likePostItem = new JavaScriptSerializer().Deserialize<LikePostItem>(text);
			}
			catch
			{
			}
			List<string> list = likePostItem.ListPost.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			int num = Math.Min(list.Count, likePostItem.LikeCount);
			for (int i = 0; i < num; i++)
			{
				string text2 = list[rnd.Next(list.Count)];
				list.Remove(text2);
				ADBHelperCCK.OpenLink(p_DeviceId, text2);
				Thread.Sleep(3000);
				if (likePostItem.ViewProduct)
				{
					string arg = Guid.NewGuid().ToString("N");
					string text3 = string.Format(Application.StartupPath + "\\Temp\\shopcard_{0}.jpg", arg);
					m_driver.GetScreenshot().SaveAsFile(text3);
					Utils.CCKLog(p_DeviceId, "Savefile:" + text3);
					string text4 = Application.StartupPath + $"\\Config\\shopcard_{screen.X}_{screen.Y}.png";
					if (!File.Exists(text4))
					{
						Utils.CCKLog(p_DeviceId, $"Khong tim thay file Config\\shopcard_{screen.X}_{screen.Y}.png");
					}
					else
					{
						if (!Directory.Exists(Application.StartupPath + "\\Temp\\"))
						{
							Directory.CreateDirectory(Application.StartupPath + "\\Temp\\");
						}
						try
						{
							Point point = ADBHelperCCK.FindImages(text3, text4, 1);
							if (point.X != 0 && point.Y != 0)
							{
								Image image = Image.FromFile(text4);
								ADBHelperCCK.Tap(p_DeviceId, point.X + image.Width / 2, point.Y + image.Height / 2);
								Utils.CCKLog(p_DeviceId, $"Click vi tri {point.X + 3 * image.Width}x{point.Y + image.Height / 2}");
								DateTime dateTime = DateTime.Now.AddSeconds(likePostItem.ViewProductDelay);
								while (dateTime > DateTime.Now)
								{
									Thread.Sleep(2000);
									GetPageSource();
								}
								ADBHelperCCK.Back(p_DeviceId);
							}
							else
							{
								Utils.CCKLog(p_DeviceId, $" aaaa Click vi tri {point.X}x{point.Y}");
							}
						}
						catch (Exception)
						{
						}
						finally
						{
							try
							{
							}
							catch
							{
							}
						}
					}
				}
				GetPageSource();
				if (!likePostItem.ViewOnly)
				{
					IWebElement webElement = ADBHelperCCK.WaitMe("//*[@content-desc=\"Like\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000 * likePostItem.Delay);
					}
				}
				while (likePostItem.ViewCount > 0)
				{
					likePostItem.ViewCount--;
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 4, screen.X / 3, screen.Y * 3 / 5, 500);
					Thread.Sleep(likePostItem.ViewDelay * 1000);
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4, 500);
					Thread.Sleep(1000 * likePostItem.Delay);
				}
			}
		}

		private void GotoTab(Tabs tab, bool AppFull = true)
		{
			string pageSource = GetPageSource();
			string text = "";
			switch (tab)
			{
			case Tabs.Home:
				text = "Home";
				break;
			case Tabs.Discovery:
				text = "Discovery";
				break;
			case Tabs.Post:
				text = "Post";
				break;
			case Tabs.Inbox:
				text = "Inbox";
				break;
			case Tabs.Profile:
				text = "Profile";
				break;
			case Tabs.Me:
				text = "Me";
				break;
			}
			if (!pageSource.Contains("\"" + text + "\""))
			{
				if (!pageSource.Contains("\"" + text + "\""))
				{
					ADBHelperCCK.CloseApp(p_DeviceId, AppFull ? CaChuaConstant.PACKAGE_NAME : CaChuaConstant.PACKAGE_NAME_LITE);
					Thread.Sleep(1000);
					ADBHelperCCK.OpenApp(p_DeviceId, AppFull ? CaChuaConstant.PACKAGE_NAME : CaChuaConstant.PACKAGE_NAME_LITE);
					Thread.Sleep(3000);
					GotoTab(tab, AppFull);
				}
				return;
			}
			CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
			CCKNode cCKNode = cCKDriver.FindElement($"//*[@text=\"{text}\"]", pageSource);
			if (cCKNode != null)
			{
				for (int i = 0; i < 2; i++)
				{
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4, 500);
				}
				cCKNode.Click();
				Thread.Sleep(1000);
				for (int j = 0; j < 2; j++)
				{
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 4, screen.X / 3, screen.Y * 3 / 5);
				}
			}
		}

		public bool IsLogin()
		{
			if (Info.LoggedIn)
			{
				return true;
			}
			string text = GetPageSource().ToLower();
			if (text.Contains("log in or sign up"))
			{
				Info.LoggedIn = false;
				return false;
			}
			if (text.Contains("appeal deadline expired"))
			{
				IWebElement webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"log out\" or lower-case(@content-desc)=\"log out\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					text = GetPageSource().ToLower();
					Info.LoggedIn = false;
					sql.UpdateTrangThai(Info.Uid, "Appeal deadline expired", "Error");
					return false;
				}
			}
			else
			{
				if (text.Contains("your account was logged out"))
				{
					IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", m_driver);
					if (webElement2 != null)
					{
						webElement2.Click();
						Info.LoggedIn = false;
						Thread.Sleep(1000);
					}
					return false;
				}
				if (text.Contains("incorrect account or password"))
				{
					Info.LoggedIn = false;
					sql.UpdateTrangThai(Info.Uid, "Incorrect account or password", "Die");
					return false;
				}
				if (text.Contains("already have an account? log in"))
				{
					Info.LoggedIn = false;
					return false;
				}
				if (!text.Contains("\"profile\""))
				{
					if (text.Contains("text=\"log in\""))
					{
						Info.LoggedIn = false;
						return false;
					}
					if (text.Contains("text=\"log in to tiktok\""))
					{
						Info.LoggedIn = false;
						return false;
					}
				}
				else
				{
					IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Profile\"]", m_driver);
					if (webElement3 != null)
					{
						webElement3.Click();
						Thread.Sleep(1000);
						text = GetPageSource().ToLower();
						if (text.Contains("text=\"@\"") && text.Contains("something went wrong"))
						{
							sql.UpdateTrangThai(Info.Uid, "Account Error", "Error");
							return false;
						}
						if (!text.Contains("text=\"@" + Info.Uid.TrimStart('@')))
						{
							return false;
						}
						IWebElement webElement4 = ADBHelperCCK.AppGetObject("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.TextView", m_driver);
						if (webElement4 == null)
						{
							webElement4 = ADBHelperCCK.AppGetObject("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.Button", m_driver);
						}
						if (webElement4 != null && webElement4.Text != "" && string.IsNullOrWhiteSpace(Info.FirstName))
						{
							Info.FirstName = webElement4.Text;
							if (Info.FirstName != "")
							{
								sql.ExecuteQuery(string.Format("Update Account set Name='{0}' where id='@{1}' or id='{1}'", webElement4.Text, Info.Uid));
							}
						}
						Info.LoggedIn = true;
						return true;
					}
				}
			}
			return true;
		}

		internal List<string> CommentOnPost()
		{
			Utils.LogFunction("CommentOnPost", "");
			List<string> list = new List<string>();
			if (!IsLogin())
			{
				return new List<string> { "Chưa đăng nhập" };
			}
			string text = Utils.ReadTextFile(CaChuaConstant.RANDOM_COMMENT);
			if (!string.IsNullOrWhiteSpace(text))
			{
				CommentItems commentItems = new CommentItems();
				try
				{
					commentItems = new JavaScriptSerializer().Deserialize<CommentItems>(text);
				}
				catch
				{
				}
				if (commentItems.ListID.Count == 0)
				{
					return new List<string> { "Không có ID để comment" };
				}
				int num = Math.Min(commentItems.ListID.Count, commentItems.CommentCount);
				for (int i = 0; i < num; i++)
				{
					string text2 = commentItems.ListID[rnd.Next(commentItems.ListID.Count)];
					commentItems.ListID.Remove(text2);
					ADBHelperCCK.OpenLink(p_DeviceId, text2);
					Thread.Sleep(1000 * commentItems.DelayViewLink);
					string text3 = GetLocalSource().ToLower();
					if (text3.Contains("Open with".ToLower()) && text3.Contains("android:id/button_always"))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"tiktok\")]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(500);
						}
						ADBHelperCCK.AppGetObject("//android.widget.Button[lower-case(@text)=\"always\"]", m_driver)?.Click();
						Thread.Sleep(500);
						ADBHelperCCK.OpenLink(p_DeviceId, text2);
					}
					text3 = GetPageSource();
					CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
					CCKNode cCKNode = cCKDriver.FindElement("//*[contains(@text,\"Add comment\") or contains(@content-desc,\"Read or add comments\")]", text3);
					if (cCKNode == null)
					{
						CCKNode cCKNode2 = cCKDriver.FindElement("//*[@content-desc=\"Like\"]", text3);
						if (cCKNode2 != null)
						{
							ADBHelperCCK.Tap(p_DeviceId, cCKNode2.Location.X, cCKNode2.Location.Y + cCKNode2.Size.Height);
							Thread.Sleep(10000);
							text3 = GetPageSource();
							cCKNode = cCKDriver.FindElement("//*[@text=\"Add comment...\"]", text3);
						}
					}
					if (cCKNode == null || !File.Exists(commentItems.FileComment))
					{
						continue;
					}
					cCKNode.Click();
					string text4 = "";
					GetPageSource();
					IWebElement webElement2 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement2 != null)
					{
						text4 = (commentItems.MultiLine ? Utils.ReadTextFile(commentItems.FileComment) : Utils.GetFirstItemFromFile(commentItems.FileComment, commentItems.Deleted));
						GetPageSource();
						webElement2.SendKeys(Utils.Spin(text4));
					}
					if (commentItems.Tags != "")
					{
						string[] array = commentItems.Tags.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						ADBHelperCCK.SendToPhoneClipboard(p_DeviceId, " @");
						GetPageSource();
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent 279");
						ADBHelperCCK.SendToPhoneClipboard(p_DeviceId, array[rnd.Next(array.Length)]);
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent 279");
						Thread.Sleep(2000);
						IWebElement webElement3 = ADBHelperCCK.AppGetObject("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.TextView", m_driver);
						if (webElement3 != null)
						{
							webElement3.Click();
							Thread.Sleep(1000);
						}
					}
					Thread.Sleep(1000);
					text3 = GetPageSource();
					CCKNode cCKNode3 = cckDriver.FindElement("//android.widget.EditText", text3);
					if (cCKNode3 != null)
					{
						CCKNode cCKNode4 = cckDriver.FindElement("//*[@content-desc=\"Post comment\" or @text=\"Post comment\"]", text3);
						if (cCKNode4 != null)
						{
							cCKNode4.Click();
							list.Add(text4);
						}
						DateTime now = DateTime.Now;
						text3 = GetPageSource();
						double totalSeconds = DateTime.Now.Subtract(now).TotalSeconds;
						if (totalSeconds < (double)commentItems.Delay)
						{
							Thread.Sleep(1000 * Convert.ToInt32((double)commentItems.Delay - totalSeconds));
						}
						ADBHelperCCK.Back(p_DeviceId);
					}
				}
				return list;
			}
			return new List<string> { "Chưa có nội dung comment" };
		}

		private List<CCKNode> GetActionMenu()
		{
			string text = string.Format(Application.StartupPath + "\\Config\\tiktok_menu_{0}.txt", p_DeviceId);
			if (File.Exists(text))
			{
				new JavaScriptSerializer().DeserializeObject(Utils.ReadTextFile(text));
			}
			List<CCKNode> list = ADBHelperCCK.AppGetObjects("//X.0Go/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout[1]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.ImageView", GetPageSource(), p_DeviceId);
			if (list != null && list.Count > 0)
			{
				File.WriteAllText(text, new JavaScriptSerializer().Serialize(list));
			}
			return (list != null) ? list : new List<CCKNode>();
		}

		private string GetCurrentAccount()
		{
			string text = Utils.ReadTextFile(Application.StartupPath + "\\Config\\acc.txt");
			return text.Split("|".ToCharArray())[0];
		}

		internal void ViewNewsFeed()
		{
			Utils.LogFunction("ViewNewsFeed", "");
			if (!IsLogin())
			{
				return;
			}
			string text = Utils.ReadTextFile(CaChuaConstant.NEWSFEED);
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			NewsFeedItem newsFeedItem = new NewsFeedItem();
			try
			{
				newsFeedItem = new JavaScriptSerializer().Deserialize<NewsFeedItem>(text);
			}
			catch
			{
			}
			GotoTab(Tabs.Home);
			DateTime dateTime = DateTime.Now.AddSeconds(Utils.GetRandomInRange(newsFeedItem.TimeFrom, newsFeedItem.TimeTo + 1));
			Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
			int num = 0;
			while (dateTime > DateTime.Now)
			{
				try
				{
					string pageSource = GetPageSource();
					if (pageSource.Contains("\"Like\""))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Like\"]", m_driver);
						if (webElement != null && num < newsFeedItem.LikeCount)
						{
							num++;
							ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format("shell \"input tap {0} {1} & sleep 0.2; input tap {0} {1} & sleep 0.2; input tap {0} {1} & sleep 0.2; input tap {0} {1}\"", screenResolution.X / 3, screenResolution.Y / 3));
						}
						pageSource = GetPageSource();
					}
					if (pageSource.Contains("text=\"Repost\"") && DateTime.Now.ToFileTime() % 100L < newsFeedItem.PercentRepost)
					{
						IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Repost\" or @text=\"Repost\"]", m_driver);
						if (webElement2 != null)
						{
							webElement2.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
						}
					}
					string text2 = "";
					if (!(newsFeedItem.ShopName != ""))
					{
						goto IL_048f;
					}
					pageSource = GetPageSource().ToLower();
					if (!pageSource.Contains(newsFeedItem.ShopName.ToLower()))
					{
						goto IL_048f;
					}
					if (!pageSource.Contains("text=\"tap to watch live\"") && !pageSource.Contains("text=\"live now\""))
					{
						text2 = (newsFeedItem.IsSpin ? Utils.ReadTextFile(CaChuaConstant.NEWSFEED_COMMENT) : Utils.GetFirstItemFromFile(CaChuaConstant.NEWSFEED_COMMENT, newsFeedItem.RemoveComment));
						if (text2 != "")
						{
							IWebElement webElement3 = ADBHelperCCK.AppGetObject("//X.0Go/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView", m_driver);
							if (webElement3 != null)
							{
								webElement3.Click();
								Thread.Sleep(rnd.Next(1000, 2000));
								if (webElement3 != null && text2 != "" && newsFeedItem.Percent > DateTime.Now.ToFileTime() % 100L)
								{
									webElement3.Click();
									Thread.Sleep(2000);
									pageSource = GetPageSource();
									IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"add comment...\"]", m_driver);
									if (webElement4 != null && text2 != "")
									{
										webElement4.SendKeys(Utils.Spin(text2));
										Thread.Sleep(1000);
										CCKNode cCKNode = cckDriver.FindElement("//android.widget.EditText", GetPageSource());
										List<CCKNode> list = cCKNode.FindElements("./..");
										if (list != null && list.Count > 0)
										{
											cCKNode = list[0];
											List<CCKNode> list2 = cCKNode.FindElements("android.widget.ImageView");
											if (list2 != null && list2.Count > 0)
											{
												list2[0].Click();
											}
											Thread.Sleep(1000);
											ADBHelperCCK.Back(p_DeviceId);
											if (dateTime < DateTime.Now)
											{
												return;
											}
										}
									}
								}
							}
						}
						pageSource = GetPageSource().ToLower();
					}
					else
					{
						ADBHelperCCK.Tap(p_DeviceId, screenResolution.X / 3, screenResolution.Y / 2);
						Thread.Sleep(1000);
						ADBHelperCCK.Tap(p_DeviceId, screenResolution.X / 3, screenResolution.Y / 2);
						Thread.Sleep(1000);
						pageSource = GetPageSource().ToLower();
						SeedingVideo(dontUseLink: true);
						ADBHelperCCK.Back(p_DeviceId);
					}
					Thread.Sleep(1000 * rnd.Next(newsFeedItem.FavoriteViewVideoTimeFrom, newsFeedItem.FavoriteViewVideoTimeTo));
					pageSource = GetPageSource().ToLower();
					if (!pageSource.Contains(CaChuaConstant.PACKAGE_NAME))
					{
						ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
					}
					ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4);
					if (!(dateTime < DateTime.Now))
					{
						continue;
					}
					return;
					IL_048f:
					text2 = (newsFeedItem.IsSpin ? Utils.ReadTextFile(CaChuaConstant.NEWSFEED_COMMENT) : Utils.GetFirstItemFromFile(CaChuaConstant.NEWSFEED_COMMENT, newsFeedItem.RemoveComment));
					if (!string.IsNullOrWhiteSpace(text2))
					{
						IWebElement webElement5 = ADBHelperCCK.AppGetObject("//X.0Go/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView", m_driver);
						if (webElement5 != null)
						{
							webElement5.Click();
							Thread.Sleep(2000);
							pageSource = GetPageSource();
							cckDriver.FindElement("//*[@text=\"Add comment...\"]", pageSource);
							IWebElement webElement6 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"add comment...\"]", m_driver);
							if (webElement6 != null && text2 != "")
							{
								if (text2.Contains("@"))
								{
									try
									{
										string[] array = text2.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
										int num2 = -1;
										for (int i = 0; i < array.Length; i++)
										{
											if (array[i].Contains("@"))
											{
												num2 = i;
												break;
											}
										}
										string text3 = ((num2 > 0) ? string.Join(" ", array, 0, num2).ToString() : "");
										string text4 = array[num2];
										string text5 = ((num2 < 0 || num2 >= array.Length - 1) ? "" : string.Join(" ", array, num2 + 1, array.Length - num2 - 1).ToString());
										webElement6.Click();
										if (text3.Trim() != "")
										{
											ADBHelperCCK.InputUnicode(p_DeviceId, text3.Trim() + " ");
										}
										if (text4.Trim() != "")
										{
											ADBHelperCCK.InputUnicode(p_DeviceId, text4.Trim());
											Thread.Sleep(5000);
											IWebElement webElement7 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='" + text4.ToLower().Trim().Replace("@", "") + "']", m_driver);
											if (webElement7 != null)
											{
												webElement7.Click();
												Thread.Sleep(1000);
											}
										}
										if (text5.Trim() != "")
										{
											ADBHelperCCK.InputUnicode(p_DeviceId, text5.Trim());
										}
									}
									catch
									{
										webElement6.SendKeys(Utils.Spin(text2));
									}
								}
								else
								{
									webElement6.SendKeys(Utils.Spin(text2));
								}
								Thread.Sleep(1000);
								CCKNode cCKNode2 = cckDriver.FindElement("//android.widget.EditText", GetPageSource());
								List<CCKNode> list3 = cCKNode2.FindElements("./..");
								if (list3 != null && list3.Count > 0)
								{
									cCKNode2 = list3[0];
									try
									{
										List<CCKNode> list4 = cCKNode2.FindElements("android.widget.FrameLayout/android.widget.ImageView");
										if (list4 != null && list4.Count > 0)
										{
											list4[0].Click();
										}
										else
										{
											Utils.CCKLog("Add comment", "Loi khoong tim thay nut: " + Info.Uid);
										}
									}
									catch
									{
									}
									Thread.Sleep(1000);
									ADBHelperCCK.Back(p_DeviceId);
									if (dateTime < DateTime.Now)
									{
										return;
									}
								}
							}
						}
						pageSource = GetPageSource();
						List<CCKNode> list5 = cckDriver.FindElements("//androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout/android.view.ViewGroup/android.widget.ImageView", pageSource);
						if (list5 != null && list5.Count > 0)
						{
							for (int num3 = list5.Count - 1; num3 >= 0; num3--)
							{
								if (list5[num3].Size.Width != list5[num3].Size.Height)
								{
									list5.RemoveAt(num3);
								}
							}
							list5[new Random().Next(list5.Count)]?.Click();
						}
						if (dateTime < DateTime.Now)
						{
							return;
						}
						ADBHelperCCK.Back(p_DeviceId);
					}
					Thread.Sleep(1000 * rnd.Next(newsFeedItem.ViewVideoTimeFrom, newsFeedItem.ViewVideoTimeTo));
					pageSource = GetPageSource();
					ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y / 2, screenResolution.X / 3, screenResolution.Y / 4);
				}
				catch (Exception ex)
				{
					Utils.CCKLog("ViewNewsFeed", ex.Message);
				}
			}
		}

		public void AddLogByAction(string logMessage, bool isAppend = true)
		{
			try
			{
				if (CurrentRow != null)
				{
					CurrentRow.Cells["Log"].Value = logMessage;
				}
			}
			catch
			{
			}
			ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, logMessage);
			if (isAppend)
			{
				sql.ExecuteQuery(string.Format("Update Account set Noti  =  '{0}, ' || Noti where id='{1}' ", logMessage.Replace("'", ""), Info.Uid));
			}
			else
			{
				sql.ExecuteQuery(string.Format("Update Account set Noti  = '{0}' where id='{1}' ", logMessage.Replace("'", ""), Info.Uid));
			}
		}

		internal void FollowUID()
		{
			Utils.LogFunction("FollowUID", "");
			if (!File.Exists(CaChuaConstant.FOLLOW_UID))
			{
				return;
			}
			FollowFriendEntity followFriendEntity = new JavaScriptSerializer().Deserialize<FollowFriendEntity>(Utils.ReadTextFile(CaChuaConstant.FOLLOW_UID));
			if (followFriendEntity == null)
			{
				return;
			}
			decimal number = followFriendEntity.Number;
			for (int i = 0; (decimal)i < number; i++)
			{
				try
				{
					string firstItemFromFile = Utils.GetFirstItemFromFile(CaChuaConstant.FOLLOW_UID_DATA, followFriendEntity.RemoveAfterFollow);
					if (firstItemFromFile == "")
					{
						return;
					}
					string link = (firstItemFromFile.StartsWith("http") ? firstItemFromFile : ("https://www.tiktok.com/@" + firstItemFromFile.Replace("@", "")));
					ADBHelperCCK.OpenLink(p_DeviceId, link);
					GetPageSource();
					IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Follow\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(Convert.ToInt32(followFriendEntity.Delay) * 1000);
					}
				}
				catch (Exception ex)
				{
					Utils.CCKLog("FollowUID ", ex.Message);
				}
			}
		}

		internal void FollowByFollower()
		{
			Utils.LogFunction("FollowByFollower", "");
			GotoTab(Tabs.Profile);
			if (!File.Exists(CaChuaConstant.FOLLOWERS))
			{
				return;
			}
			FollowEntity followEntity = js.Deserialize<FollowEntity>(Utils.ReadTextFile(CaChuaConstant.FOLLOWERS));
			int num = rnd.Next(followEntity.FollowFrom, followEntity.FollowTo);
			int num2 = 0;
			DateTime dateTime = DateTime.Now.AddSeconds(rnd.Next(followEntity.WorkingFrom, followEntity.WorkingTo));
			GetPageSource();
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Profile\" or @content-desc=\"Profile\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(2000);
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Follower\" or @content-desc=\"Follower\" or @text=\"Followers\" or @content-desc=\"Followers\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(2000);
			Thread.Sleep(5000);
			GetPageSource();
			while (dateTime > DateTime.Now)
			{
				List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[@text=\"Follow back\" or @content-desc=\"Follow back\"]", m_driver);
				if (list == null)
				{
					break;
				}
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Click();
					Thread.Sleep(1000 * rnd.Next(followEntity.DelayFrom, followEntity.DelayTo));
					if (num >= num2)
					{
						num2++;
						continue;
					}
					return;
				}
				ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4, 300);
			}
		}

		public void RestoreAccount()
		{
			ADBHelperCCK.ClearAppData(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			_ = Info.Uid;
			string text = $"{Application.StartupPath}\\Authentication\\{Info.Email}";
			if (Info.Uid.StartsWith("@"))
			{
				Info.Uid = Info.Uid.Substring(1);
			}
			string text2 = $"{Application.StartupPath}\\Authentication\\{Info.Uid}";
			if (!Directory.Exists(text2) && Directory.Exists(text))
			{
				Directory.Move(text, text2);
				Thread.Sleep(2000);
			}
			ADBHelperCCK.CloseApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			for (int i = 0; i < 2; i++)
			{
				ADBHelperCCK.ExecuteCMD(p_DeviceId, $" push \"{Application.StartupPath}\\Authentication\\{Info.Uid}\\{CaChuaConstant.PACKAGE_NAME}\" /data/data/");
			}
			Thread.Sleep(1000);
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			Thread.Sleep(5000);
			GetPageSource();
		}

		private static string GetFullPath(XmlNode node)
		{
			string text = node.Name;
			while (node.ParentNode != null && node.ParentNode.NodeType != XmlNodeType.Document)
			{
				node = node.ParentNode;
				text = node.Name + "/" + text;
			}
			return text;
		}

		private static string GetXPath(XmlNode node)
		{
			if (node != null)
			{
				if (node.NodeType != XmlNodeType.Attribute)
				{
					if (node.ParentNode != null)
					{
						int num = 1;
						for (XmlNode previousSibling = node.PreviousSibling; previousSibling != null; previousSibling = previousSibling.PreviousSibling)
						{
							if (previousSibling.Name == node.Name)
							{
								num++;
							}
						}
						string text = "/" + node.Name + ((num > 1) ? ("[" + num + "]") : "");
						return GetXPath(node.ParentNode) + text;
					}
					return "/" + node.Name;
				}
				return GetXPath(((XmlAttribute)node).OwnerElement) + "/@" + node.Name;
			}
			return null;
		}

		public SwitchResult SwitchAccount()
		{
			if (Info.Uid == "")
			{
				return SwitchResult.Unknown;
			}
			while (true)
			{
				GotoTab(Tabs.Profile);
				string pageSource = GetPageSource();
				if (!pageSource.Contains("\"@") || !pageSource.Contains("profile\""))
				{
					break;
				}
				IWebElement webElement = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"@\")]", m_driver);
				if (webElement != null && webElement.Text != "")
				{
					string text = webElement.Text.Replace("@", "").Trim();
					if (!(text != Info.Uid))
					{
						sql.SetDieAccount(Info.Uid, "Live", "Live");
						return SwitchResult.Success;
					}
					List<IWebElement> list = ADBHelperCCK.AppGetObjects("//android.widget.Button[@content-desc=\"Profile menu\"]", m_driver);
					if (list != null && list.Count > 0)
					{
						string text2 = "";
						try
						{
							new CCKDriver(p_DeviceId);
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.LoadXml(GetPageSource());
							XmlNode xmlNode = xmlDocument.SelectSingleNode("//android.widget.Button[@content-desc=\"Profile menu\"]");
							if (xmlNode != null)
							{
								xmlNode = xmlNode.ParentNode;
								if (xmlNode != null)
								{
									xmlNode = xmlNode.PreviousSibling;
								}
								if (xmlNode != null && xmlNode.ChildNodes.Count > 0)
								{
									xmlNode = xmlNode.ChildNodes[0];
									text2 = GetXPath(xmlNode).Replace("/#document", "");
								}
							}
						}
						catch
						{
							text2 = "";
						}
						IWebElement webElement2 = ADBHelperCCK.AppGetObject(By.XPath((text2 != "") ? text2 : "//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout/android.widget.TextView | //android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.Button"), m_driver);
						if (webElement2 == null)
						{
							ADBHelperCCK.Tap(p_DeviceId, screen.X / 2, list[0].Location.Y + list[0].Size.Height / 2);
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							if (pageSource.Contains("Your nickname can only be changed once every 7 days"))
							{
								IWebElement webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
								if (webElement3 != null)
								{
									string text3 = Info.FirstName + " " + Info.LastName;
									if (text3.Trim() == "")
									{
										VNNameEntity vNNameEntity = new VNNameEntity();
										if (!File.Exists(CaChuaConstant.VN_Name))
										{
											text3 = "User TT";
										}
										else
										{
											vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
											text3 = (vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)] + " " + vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)]).Trim();
										}
										vNNameEntity = null;
										if (text3 == "")
										{
											text3 = "User TT";
										}
									}
									webElement3.SendKeys(text3);
									Thread.Sleep(1000);
									webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("e1550ec748364cd6ae4bfe53fe4ab048"), m_driver);
									if (webElement3 != null)
									{
										webElement3.Click();
										Thread.Sleep(5000);
										pageSource = GetPageSource();
										if (pageSource.Contains("text=\"Set nickname?\""))
										{
											ADBHelperCCK.AppGetObject(GetFunctionByKey("4d92d33813d7417aa30d2cec42236d13"), m_driver)?.Click();
										}
										Thread.Sleep(5000);
										continue;
									}
								}
							}
						}
						else
						{
							webElement2.Click();
							Thread.Sleep(3000);
						}
						bool flag = false;
						while (true)
						{
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							if (!pageSource.Contains("\"Add account\"") || pageSource.Contains("\"" + Info.Uid + "\""))
							{
								List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.Button", m_driver);
								int num = -1;
								if (list2 == null)
								{
									break;
								}
								for (int i = 0; i < list2.Count; i++)
								{
									string text4 = list2[i].Text;
									if (text4 == "")
									{
										text4 = list2[i].GetAttribute("content-desc");
									}
									if (text4 == Info.Uid && i > 0)
									{
										num = i;
									}
									if (text4 != "" && text4 != "Add account")
									{
										sql.ExecuteQuery($"Update Account set Device='{p_DeviceId}' where id='{text4}'");
									}
								}
								if (num == -1)
								{
									if (list2.Count < 8 && !flag)
									{
										ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 2, screen.X / 3, screen.Y / 4);
									}
									if (list2.Count != 8)
									{
										ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 2, screen.X / 3, screen.Y / 4);
										flag = true;
										continue;
									}
									return SwitchResult.Full;
								}
								list2[num].Click();
								Thread.Sleep(5000);
								GotoTab(Tabs.Profile);
								pageSource = GetPageSource();
								if (!pageSource.Contains("@" + Info.Uid))
								{
									return SwitchResult.Unknown;
								}
								return SwitchResult.Success;
							}
							IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[@text=\"Add account\"]", m_driver);
							if (webElement4 == null)
							{
								break;
							}
							webElement4.Click();
							Thread.Sleep(1000);
							return SwitchResult.LogIn;
						}
					}
					else
					{
						Utils.CCKLog("Profile menu", "Khong tim thay");
					}
				}
				return SwitchResult.Unknown;
			}
			return SwitchResult.Error;
		}

		public void SwitchAccount(bool remove)
		{
			while (true)
			{
				GotoTab(Tabs.Profile);
				string pageSource = GetPageSource();
				if (!pageSource.Contains("\"@") || !pageSource.Contains("profile\""))
				{
					break;
				}
				IWebElement webElement = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"@\")]", m_driver);
				if (webElement == null || !(webElement.Text != ""))
				{
					break;
				}
				List<IWebElement> list = ADBHelperCCK.AppGetObjects("//android.widget.Button[@content-desc=\"Profile menu\"]", m_driver);
				if (list != null && list.Count > 0)
				{
					IWebElement webElement2 = ADBHelperCCK.AppGetObject(By.XPath("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout/android.widget.TextView | //android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.Button"), m_driver);
					if (webElement2 == null)
					{
						ADBHelperCCK.Tap(p_DeviceId, screen.X / 2, list[0].Location.Y + list[0].Size.Height / 2);
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						if (pageSource.Contains("Your nickname can only be changed once every 7 days"))
						{
							IWebElement webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
							if (webElement3 != null)
							{
								string text = Info.FirstName + " " + Info.LastName;
								if (text.Trim() == "")
								{
									VNNameEntity vNNameEntity = new VNNameEntity();
									if (File.Exists(CaChuaConstant.VN_Name))
									{
										vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
										text = (vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)] + " " + vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)]).Trim();
									}
									else
									{
										text = "User TT";
									}
									vNNameEntity = null;
									if (text == "")
									{
										text = "User TT";
									}
								}
								webElement3.SendKeys(text);
								Thread.Sleep(1000);
								webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("e1550ec748364cd6ae4bfe53fe4ab048"), m_driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(5000);
									pageSource = GetPageSource();
									if (pageSource.Contains("text=\"Set nickname?\""))
									{
										ADBHelperCCK.AppGetObject(GetFunctionByKey("4d92d33813d7417aa30d2cec42236d13"), m_driver)?.Click();
									}
									Thread.Sleep(5000);
									continue;
								}
							}
						}
					}
					else
					{
						ADBHelperCCK.Tap(p_DeviceId, webElement2.Location.X + webElement2.Size.Width * 9 / 10, webElement2.Location.Y + webElement2.Size.Height / 2);
						Thread.Sleep(1000);
					}
					bool flag = false;
					while (true)
					{
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						if (!pageSource.Contains("\"Add account\"") || pageSource.Contains("\"" + Info.Uid + "\""))
						{
							List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.Button", m_driver);
							int num = -1;
							if (list2 == null)
							{
								break;
							}
							for (int i = 0; i < list2.Count; i++)
							{
								string text2 = list2[i].Text;
								if (text2 == "")
								{
									text2 = list2[i].GetAttribute("content-desc");
								}
								if (text2 == Info.Uid && i > 0)
								{
									num = i;
								}
								if (text2 != "" && text2 != "Add account")
								{
									sql.ExecuteQuery($"Update Account set Device='{p_DeviceId}' where id='{text2}'");
								}
							}
							if (num == -1)
							{
								if (list2.Count < 8 && !flag)
								{
									ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 2, screen.X / 3, screen.Y / 4);
								}
								if (list2.Count != 8)
								{
									ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 2, screen.X / 3, screen.Y / 4);
									flag = true;
									continue;
								}
								break;
							}
							list2[num].Click();
							Thread.Sleep(5000);
							GotoTab(Tabs.Profile);
							pageSource = GetPageSource();
							break;
						}
						IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[@text=\"Add account\"]", m_driver);
						if (webElement4 != null)
						{
							webElement4.Click();
							Thread.Sleep(1000);
						}
						break;
					}
				}
				else
				{
					Utils.CCKLog("Profile menu", "Khong tim thay");
				}
				break;
			}
		}

		internal bool Login(out string outMessage, bool recoverpass = false)
		{
			LogByVideo(p_DeviceId, Info.Uid);
			ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
			CloseCaptcha = false;
			outMessage = "";
			try
			{
				new Task(delegate
				{
					sql.LastActive(Info.Uid);
					ADBHelperCCK.AddAndroidPermission(p_DeviceId, new List<string> { "android.permission.READ_CONTACTS", "android.permission.READ_EXTERNAL_STORAGE", "com.android.providers.contacts", "com.google.android.syncadapters.contacts", "com.qualcomm.simcontacts" });
					ADBHelperCCK.SetPortrait(p_DeviceId);
					ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell ime set com.android.adbkeyboard/.AdbIME");
				}).Start();
				if (Info.Uid.StartsWith("@"))
				{
					string text = $"{Application.StartupPath}\\Authentication\\{Info.Uid}";
					if (Directory.Exists(text))
					{
						Info.Uid = Info.Uid.Substring(1);
						Directory.Move(text, text.Replace("Authentication\\@", "Authentication\\"));
					}
				}
				string path = $"{Application.StartupPath}\\Authentication\\{Info.Uid}";
				if (!Directory.Exists(path) && Info.Email != "")
				{
					path = $"{Application.StartupPath}\\Authentication\\{Info.Email}";
				}
				CCKNode cCKNode = new CCKNode();
				Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
				if (Directory.Exists(path) && deviceEntity.Rooted)
				{
					if (RestoreAccountTiktok())
					{
						outMessage = "Login Successful";
						return true;
					}
					GetPageSource();
					for (int i = 0; i < 2; i++)
					{
						ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4);
					}
					ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
				}
				string text2 = "";
				int num = 0;
				DateTime dateTime = DateTime.Now.AddMinutes(3.0);
				while (dateTime > DateTime.Now)
				{
					Thread.Sleep(1000);
					text2 = GetPageSource();
					if (text2.Contains("\"Verify identity\""))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject(By.XPath("//*[contains(@text, 'Email:')]"), m_driver);
						if (webElement != null)
						{
							webElement.Click();
							string text3 = webElement.Text.Replace("Email:", "").Trim();
							if (text2.Contains("Verify it’s really you"))
							{
								AddLogByAction("Verify it’s really you:" + text3, isAppend: false);
								outMessage = "Login Failed";
								return false;
							}
							Thread.Sleep(1000);
						}
					}
					if (text2.Contains("text=\"Restore\"") && text2.Contains("text=\"Cancel\""))
					{
						CCKNode cCKNode2 = cckDriver.FindElement(GetFunctionByKey("4a312117c3b140529aeefad57062e5b0"), text2);
						if (cCKNode2 != null)
						{
							cCKNode2.Click();
							Thread.Sleep(1000);
							ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
							text2 = GetPageSource();
						}
					}
					if (text2.Contains("text=\"Select account\"") && text2.Contains("text=\"Add\u00a0existing\u00a0account\""))
					{
						CCKNode cCKNode3 = cckDriver.FindElement(GetFunctionByKey("36d631a41d824b4c9b340c1e348e3ce2"), text2);
						if (cCKNode3 != null)
						{
							cCKNode3.Click();
							Thread.Sleep(1000);
							text2 = GetPageSource();
						}
					}
					if (text2.Contains("text=\"Profile\"") && !text2.Contains("text=\"Login\""))
					{
						CCKNode cCKNode4 = cckDriver.FindElement(GetFunctionByKey("9d54f9db7dfd4fe1868513bc9b538d1f"), text2);
						if (cCKNode4 != null)
						{
							cCKNode4.Click();
							Thread.Sleep(1000);
							ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y / 4, screenResolution.X / 3, screenResolution.Y * 3 / 5);
							Thread.Sleep(500);
							cCKNode4 = null;
							text2 = GetPageSource();
						}
					}
					if (text2.Contains("text=\"Switch account\"") && text2.Contains("text=\"Add account\""))
					{
						cCKNode = cckDriver.FindElement(GetFunctionByKey("abb4dff4dfc64cef8f16e8497a93d6aa"), text2);
						if (cCKNode != null)
						{
							cCKNode.Click();
							Thread.Sleep(500);
							text2 = GetPageSource();
						}
					}
					if (!text2.Contains("text=\"@\"") || !text2.Contains("Something went wrong"))
					{
						if (text2.Contains("Log in\""))
						{
							cCKNode = cckDriver.FindElement(GetFunctionByKey("d05c102bdaff4262b3beda6e7d57549d"), text2);
							if (cCKNode != null)
							{
								cCKNode.Click();
								Thread.Sleep(500);
								text2 = GetPageSource();
							}
						}
						if (text2.Contains("\"Login\""))
						{
							cCKNode = cckDriver.FindElement(GetFunctionByKey("56cb7b4d932845f381f0044fde071bc5"), text2);
							if (cCKNode != null)
							{
								cCKNode.Click();
								Thread.Sleep(500);
								text2 = GetPageSource();
							}
						}
						if (text2.Contains("phone or email") && text2.Contains("Already have an account") && text2.Contains("you agree to our"))
						{
							IWebElement webElement2 = ADBHelperCCK.AppGetObject(GetFunctionByKey("7399177960dc42b9b2c95834a99efe36"), m_driver);
							if (webElement2 != null)
							{
								webElement2.Click();
								webElement2 = null;
								text2 = GetPageSource();
							}
						}
						if (text2.ToLower().Contains("text=\"Log in or sign up\"".ToLower()))
						{
							CCKNode cCKNode5 = cckDriver.FindElement(GetFunctionByKey("5f1a569b866c4e6b999dfa3b6c2996d5"), GetPageSource());
							if (cCKNode5 != null)
							{
								cCKNode5.Click();
								Thread.Sleep(1000);
								cCKNode5 = null;
								text2 = GetPageSource();
							}
						}
						if (text2.Contains("\"Reconfirm email\"".ToLower()) && text2.Contains("\"update\"") && text2.Contains("\"confirm\""))
						{
							CCKNode cCKNode6 = cckDriver.FindElement("//*[@text=\"Confirm\"]", GetPageSource());
							if (cCKNode6 != null)
							{
								cCKNode6.Click();
								Thread.Sleep(1000);
								cCKNode6 = null;
								text2 = GetPageSource();
							}
						}
						if (text2.Contains("When’s your birthday"))
						{
							RegNickEntity regNickEntity = new JavaScriptSerializer().Deserialize<RegNickEntity>(Utils.ReadTextFile(CaChuaConstant.REG_CONFIG));
							List<CCKNode> list = cckDriver.FindElements("//android.view.View".ToLower(), text2.ToLower());
							if (list != null && list.Count >= 3)
							{
								list = list.Skip(list.Count - 3).Take(3).ToList();
								_ = list.Count - 1;
								int x = list[0].Location.X;
								int y = list[0].Location.Y;
								_ = list[0].Size.Width;
								int height = list[0].Size.Height;
								ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
								Thread.Sleep(1000);
								x = list[1].Location.X;
								y = list[1].Location.Y;
								_ = list[1].Size.Width;
								height = list[1].Size.Height;
								ADBHelperCCK.Swipe(p_DeviceId, x, y - 2 * height / 5, x, y + 2 * height / 5, rnd.Next(1, 1000));
								Thread.Sleep(1000);
								x = list[2].Location.X;
								y = list[2].Location.Y;
								_ = list[2].Size.Width;
								height = list[2].Size.Height;
								int num2 = new Random().Next(regNickEntity.AgeFrom / 2, regNickEntity.AgeTo / 2);
								for (int j = 0; j < num2; j++)
								{
									ADBHelperCCK.Swipe(p_DeviceId, x, y - height / 3, x, y + height / 3, 500);
								}
								Thread.Sleep(2000);
								text2 = GetPageSource();
								int num3 = 0;
								int num4 = 5;
								while (text2.Contains("\"Next\"") && num3++ < num4)
								{
									List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[@text=\"Next\"]", m_driver);
									if (list2 != null)
									{
										ADBHelperCCK.Tap(p_DeviceId, list2[list2.Count - 1].Location.X + list2[list2.Count - 1].Size.Width / 4, list2[list2.Count - 1].Location.Y);
										Thread.Sleep(5000);
									}
									text2 = GetPageSource();
								}
								if (text2.Contains("Oops, seems like you were not eligible for TikTok"))
								{
									List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//*[@text=\"Do not download\"]", m_driver);
									if (list3 != null)
									{
										ADBHelperCCK.Tap(p_DeviceId, list3[list3.Count - 1].Location.X + list3[list3.Count - 1].Size.Width / 4, list3[list3.Count - 1].Location.Y);
										Thread.Sleep(5000);
										text2 = GetPageSource();
										if (text2.Contains("Confirmation"))
										{
											list3 = ADBHelperCCK.AppGetObjects("//*[@text=\"Confirm\"]", m_driver);
											if (list3 != null)
											{
												ADBHelperCCK.Tap(p_DeviceId, list3[list3.Count - 1].Location.X + list3[list3.Count - 1].Size.Width / 4, list3[list3.Count - 1].Location.Y);
												Thread.Sleep(3000);
												text2 = GetPageSource();
												if (text2.Contains("Done"))
												{
													list3 = ADBHelperCCK.AppGetObjects("//*[@text=\"Done\"]", m_driver);
													if (list3 != null)
													{
														ADBHelperCCK.Tap(p_DeviceId, list3[list3.Count - 1].Location.X + list3[list3.Count - 1].Size.Width / 4, list3[list3.Count - 1].Location.Y);
														Thread.Sleep(2000);
														text2 = GetPageSource();
													}
												}
											}
										}
									}
								}
							}
						}
						if (text2.Contains("Enter 6-digit code") && Info.Email != "")
						{
							string code = emailUti.GetCode(Info.Email, Info.PassEmail);
							if (!(code != ""))
							{
							}
						}
						if ((!text2.Contains("@") && !IsValidEmail(Info.Uid)) || (!text2.Contains("text=\"Set up profile\"") && !text2.Contains("text=\"Edit profile\"")))
						{
							if (text2.ToLower().Contains("text=\"sign up\"".ToLower()))
							{
								CCKNode cCKNode7 = cckDriver.FindElement("//*[@text=\"sign up\" or @content-desc=\"sign up\"]", GetPageSource());
								if (cCKNode7 != null)
								{
									cCKNode7.Click();
									Thread.Sleep(1000);
									cCKNode7 = null;
									text2 = GetPageSource();
								}
							}
							if (text2.Contains("Log in to follow accounts and like or comment on videos"))
							{
								IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Log in or sign up\"]", m_driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(1000);
									text2 = GetLocalSource();
									if (text2.Contains(Info.Uid))
									{
										webElement3 = ADBHelperCCK.AppGetObject($"//*[@text=\"{Info.Uid.TrimStart('@')}\"]", m_driver);
										if (webElement3 != null)
										{
											webElement3.Click();
											Thread.Sleep(1000);
											List<IWebElement> list4 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
											if (list4 != null && list4.Count == 2)
											{
												list4[0].Clear();
												list4[0].SendKeys((Info.Uid == "") ? Info.Email : Info.Uid);
												Thread.Sleep(1000);
												if (!(Info.Email != "") || !(Info.PassEmail != "") || !(Info.Pass == "") || Info.Uid != "")
												{
												}
												list4[1].Clear();
												list4[1].SendKeys(Info.Pass);
												Thread.Sleep(1000);
												List<IWebElement> list5 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"log in\")]", m_driver);
												if (list5 != null)
												{
													list5[list5.Count - 1].Click();
													int num5 = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_CAPTCHA));
													if (num5 == 0)
													{
														num5 = 10;
													}
													Thread.Sleep(num5 * 1000);
													ResolverCaptcha();
												}
											}
										}
									}
								}
								text2 = GetPageSource();
							}
							if (text2.Contains("\"Authenticator\"") && text2.Contains("2-step verification") && Info.TwoFA != "")
							{
								cCKNode = cckDriver.FindElement(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), text2);
								if (cCKNode != null)
								{
									cCKNode.Click();
									Thread.Sleep(500);
									string currentOtp = TimeSensitivePassCode.GetCurrentOtp(Info.TwoFA);
									cCKNode.CCKSendKeys(currentOtp);
									Thread.Sleep(1000);
									text2 = GetLocalSource();
									cCKNode = cckDriver.FindElement(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), text2);
									if (cCKNode != null)
									{
										cCKNode.Click();
										Thread.Sleep(5000);
										text2 = GetLocalSource();
									}
								}
							}
							if (text2.Contains("\"Use phone / email / username\""))
							{
								cCKNode = cckDriver.FindElement(GetFunctionByKey("00088e3826124c2f98bdadfcb21b03ed"), text2);
								if (cCKNode != null)
								{
									cCKNode.Click();
									Thread.Sleep(500);
									text2 = GetLocalSource();
								}
							}
							if (text2.Contains("\"Email / Username\""))
							{
								cCKNode = cckDriver.FindElement(GetFunctionByKey("ee9f566a53ea457da120f39bd0ab82be"), text2);
								if (cCKNode != null)
								{
									cCKNode.Click();
									Thread.Sleep(500);
								}
								List<IWebElement> list6 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
								if (list6 != null && list6.Count >= 1)
								{
									list6[0].Clear();
									if (list6.Count > 1)
									{
										list6[1].Clear();
									}
									Thread.Sleep(1000);
								}
								IWebElement webElement4 = ADBHelperCCK.AppGetObject(GetFunctionByKey("964af2beb6e440aea174a1f2467c849b"), m_driver);
								if (webElement4 != null)
								{
									if (Info.Uid == "")
									{
										return false;
									}
									webElement4.SendKeys(Info.Uid);
									Thread.Sleep(1000);
								}
								IWebElement webElement5 = ADBHelperCCK.AppGetObject("//*[@text=\"Continue\"]", m_driver);
								if (webElement5 != null)
								{
									webElement5.Click();
									Thread.Sleep(1000);
								}
								webElement5 = ADBHelperCCK.AppGetObject(GetFunctionByKey("e5030d3afa8e4733acccd39cfd551efc"), m_driver);
								if (webElement5 == null || !(Info.Pass != ""))
								{
									if (recoverpass)
									{
										RecoverPass(needRecover: true);
									}
									text2 = GetLocalSource().ToLower();
									CloseCaptcha = true;
									outMessage = "Login Failed";
									return false;
								}
								webElement5.SendKeys(Info.Pass);
								Thread.Sleep(1000);
								num++;
								if (num > 2)
								{
									ADBHelperCCK.StopApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
									CloseCaptcha = true;
									outMessage = "Login Failed";
									return false;
								}
								text2 = GetPageSource();
								int num6 = 5;
								while ((text2.Contains("\"Log in\"") || text2.Contains("\"log in\"")) && num6 >= 0)
								{
									List<CCKNode> list7 = cckDriver.FindElements(GetFunctionByKey("ce750a24a6e64f70b1c40ddabf5954e8"), text2);
									if (list7 != null && list7.Count > 0)
									{
										list7[list7.Count - 1].Click();
										Thread.Sleep(5000);
										while (num6-- > 0)
										{
											Thread.Sleep(2000);
											if (Info.TwoFA != "")
											{
												Thread.Sleep(5000);
												text2 = GetLocalSource();
												if (text2.Contains("\"Authenticator\"") && text2.Contains("2-step verification"))
												{
													cCKNode = cckDriver.FindElement(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), text2);
													if (cCKNode != null)
													{
														cCKNode.Click();
														Thread.Sleep(500);
														string currentOtp2 = TimeSensitivePassCode.GetCurrentOtp(Info.TwoFA);
														cCKNode.CCKSendKeys(currentOtp2);
														Thread.Sleep(1000);
														text2 = GetLocalSource();
														cCKNode = cckDriver.FindElement(GetFunctionByKey("ac4f154e4582431bae2b43e5af88d297"), text2);
														if (cCKNode != null)
														{
															cCKNode.Click();
															Thread.Sleep(5000);
															text2 = GetLocalSource();
														}
													}
												}
											}
											text2 = GetPageSource().ToLower();
											if (text2.Contains("text=\"verify identity\"") && text2.Contains("text=\"email:"))
											{
												list7 = cckDriver.FindElements("//*[contains(@text,\"email:\")]", text2);
												if (list7 != null && list7.Count > 0)
												{
													list7[0].Click();
													Thread.Sleep(2000);
													text2 = GetPageSource().ToLower();
													list7 = cckDriver.FindElements("//*[@text=\"next\"]", text2);
													if (list7 != null && list7.Count > 0)
													{
														list7[0].Click();
														Thread.Sleep(2000);
														text2 = GetPageSource().ToLower();
														if (text2.Contains("enter 6-digit code"))
														{
															EmailUtils emailUtils = new EmailUtils();
															string code2 = emailUtils.GetCode(Info.Email, Info.PassEmail);
															if (code2 == "LOGIN failed" || code2 == "")
															{
																outMessage = "Login Failed";
																return false;
															}
															List<IWebElement> list8 = ADBHelperCCK.AppGetObjects("//android.view.View[@resource-id=\"root\"]/android.view.View/android.view.View/android.view.View | //android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View", m_driver);
															if (list8 != null && Utils.IsNumber(code2))
															{
																char[] array = code2.ToCharArray();
																if (list8.Count <= array.Length)
																{
																	if (list8.Count == array.Length)
																	{
																		for (int k = 0; k < array.Length; k++)
																		{
																			IWebElement webElement6 = list8[k];
																			ADBHelperCCK.Tap(p_DeviceId, webElement6.Location.X + webElement6.Size.Width / 2, webElement6.Location.Y + webElement6.Size.Height / 2);
																			ADBHelperCCK.InputText(p_DeviceId, array[k].ToString());
																		}
																	}
																}
																else
																{
																	for (int l = 0; l < array.Length; l++)
																	{
																		IWebElement webElement7 = list8[l + 1];
																		ADBHelperCCK.Tap(p_DeviceId, webElement7.Location.X + webElement7.Size.Width / 2, webElement7.Location.Y + webElement7.Size.Height / 2);
																		ADBHelperCCK.InputText(p_DeviceId, array[l].ToString());
																	}
																}
																Thread.Sleep(10000);
																text2 = GetPageSource().ToLower();
															}
														}
													}
												}
											}
											if (text2.ToLower().Contains("captcha-verify-image") || text2.ToLower().Contains("drag the puzzle piece into place") || (screenResolution.X == 720 && screenResolution.Y == 1600))
											{
												Utils.CCKLog("ResolverCaptcha", "Có vao đoạn giải captcha");
												ResolverCaptcha();
												text2 = GetPageSource().ToLower();
											}
											if (!text2.ToLower().Contains("\"unrecognized login attempt\""))
											{
												int num7 = 5;
												while (text2.ToLower().Contains("no internet connection") && num7-- > 0)
												{
													Thread.Sleep(2000);
													list7 = cckDriver.FindElements(GetFunctionByKey("ce750a24a6e64f70b1c40ddabf5954e8"), GetPageSource());
													if (list7 != null && list7.Count > 0)
													{
														list7[list7.Count - 1].Click();
														Thread.Sleep(10000);
													}
													text2 = GetPageSource().ToLower();
												}
												text2 = GetPageSource().ToLower();
												if (!text2.ToLower().Contains("no internet connection"))
												{
													text2 = GetPageSource().ToLower();
													if (!text2.Contains("try again or log in with a different method") && !text2.Contains("username or password doesn't match our records") && !text2.Contains("too many attempts") && !text2.Contains("wrong account or password") && !text2.Contains("incorrect account or password") && !text2.Contains("maximum number of attempts reached"))
													{
														if (!text2.Contains("\"profile\""))
														{
															continue;
														}
														list7 = cckDriver.FindElements(GetFunctionByKey("74bb2d3f559c48b59a46176c0b0d363f"), text2);
														if (list7 == null || list7.Count <= 0)
														{
															continue;
														}
														list7[list7.Count - 1].Click();
														Thread.Sleep(2000);
														text2 = GetPageSource().ToLower();
														if (!text2.Contains("\"@\"") || !text2.Contains("\"edit profile\""))
														{
															if (!text2.Contains("@") || !text2.Contains("\"edit profile\""))
															{
																continue;
															}
															if (IsValidEmail(Info.Uid) && Info.Uid == Info.Email && Info.Email != "")
															{
																IWebElement webElement8 = ADBHelperCCK.AppGetObject("//*[contains(@text,\"@\")]", m_driver);
																if (webElement8 != null && webElement8.Text != "")
																{
																	string text4 = webElement8.Text.Trim().Replace("@", "");
																	sql.ExecuteQuery($"Update Account set id='{text4}' where email='{Info.Email}'");
																	sql.UpdateTrangThai(Info.Uid, "Login Success", "Live");
																	Info.Uid = text4;
																}
															}
															else if (!text2.Contains(Info.Uid) && Info.Email != "")
															{
																IWebElement webElement9 = ADBHelperCCK.AppGetObject("//*[contains(@text,'@')]", m_driver);
																string uid = ((webElement9 != null) ? webElement9.Text.Replace("@", "").Trim() : "");
																List<IWebElement> list9 = ADBHelperCCK.AppGetObjects("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout/android.widget.ImageView", m_driver);
																if (list9 == null || list9.Count == 0)
																{
																	outMessage = "Login Failed";
																	return false;
																}
																IWebElement webElement10 = list9[list9.Count - 1];
																if (webElement10 != null)
																{
																	webElement10.Click();
																	Thread.Sleep(1000);
																	webElement10 = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
																	if (webElement10 != null)
																	{
																		webElement10.Click();
																		Thread.Sleep(1000);
																		webElement10 = ADBHelperCCK.AppGetObject("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout[2]/X.038/android.view.ViewGroup/android.widget.TextView", m_driver);
																		if (webElement10 != null)
																		{
																			webElement10.Click();
																			Thread.Sleep(1000);
																			webElement10 = ADBHelperCCK.WaitMe("//*[@text=\"Account information\"]", m_driver);
																			if (webElement10 != null)
																			{
																				webElement10.Click();
																				Thread.Sleep(3000);
																				IWebElement webElement11 = ADBHelperCCK.AppGetObject("//*[contains(@text,'@')]", m_driver);
																				if (webElement11 != null)
																				{
																					string text5 = webElement11.Text.Replace("Your email ", "").Split('@')[0];
																					string text6 = Info.Email.Split('@')[0];
																					if (text5[0] == text6[0] && text5[text5.Length - 1] == text6[text6.Length - 1])
																					{
																						Info.Uid = uid;
																						if (sql == null)
																						{
																							sql = new SQLiteUtils();
																						}
																						sql.ExecuteQuery($"Update Account set id='{Info.Uid}' where email = '{Info.Email}'");
																						sql.UpdateTrangThai(Info.Uid, "Login Successful", "Live");
																					}
																				}
																				text2 = GetPageSource().ToLower();
																			}
																		}
																	}
																}
															}
															outMessage = "Login Successful";
															sql.UpdateTrangThai(Info.Uid, "Login Successful", "Live");
															Info.LoggedIn = true;
															BackupAccountTiktok();
															GotoTab(Tabs.Home);
															CloseCaptcha = true;
															return true;
														}
														sql.UpdateTrangThai(Info.Uid, "Account Die", "Error");
														CloseCaptcha = true;
														outMessage = "Login Failed";
														return false;
													}
													if (recoverpass)
													{
														bool result;
														if (!(result = RecoverPass()))
														{
															text2 = GetPageSource().ToLower();
															if (text2.Contains("email address isn'''t registered yet"))
															{
																outMessage = "Email address isn't registered yet";
																AddLogByAction("Email address isn't registered yet", isAppend: false);
															}
															else
															{
																outMessage = "Recover passsword fail";
																AddLogByAction("Recover passsword fail", isAppend: false);
															}
															CloseCaptcha = true;
															outMessage = "Login Failed";
															return false;
														}
														outMessage = "Login Successful";
														return result;
													}
													if (!text2.Contains("try again or log in with a different method"))
													{
														if (!text2.Contains("too many attempts") && !text2.Contains("maximum number of attempts reached"))
														{
															if (text2.Contains("wrong account or password"))
															{
																outMessage = "Wrong account or password";
																AddLogByAction("Wrong account or password", isAppend: false);
															}
															else if (text2.Contains("incorrect account or password"))
															{
																outMessage = "Incorrect account or password";
																AddLogByAction("Incorrect account or password", isAppend: false);
															}
														}
														else
														{
															outMessage = "Too many attempts";
															AddLogByAction("Too many attempts", isAppend: false);
														}
													}
													else
													{
														outMessage = "Try again or log in with a different method";
														AddLogByAction("Try again or log in with a different method", isAppend: false);
													}
													CloseCaptcha = true;
													outMessage = "Login Failed";
													return false;
												}
												outMessage = "No internet connection";
												AddLogByAction("Phone - (" + p_DeviceId + ") - No internet connection", isAppend: false);
												CloseCaptcha = true;
												outMessage = "Login Failed";
												return false;
											}
											text2 = GetPageSource().ToLower();
											AddLogByAction("Unrecognized Login Attempt / Chặn Login từ thiết bị khác", isAppend: false);
											return false;
										}
									}
									text2 = GetPageSource();
									if (text2.Contains("Maximum number of attempts reached"))
									{
										break;
									}
								}
								if (text2.Contains("you can no longer appeal this decision"))
								{
									Info.LoggedIn = false;
									sql.UpdateTrangThai(Info.Uid, "Appeal deadline expired", "Error");
									CloseCaptcha = true;
									outMessage = "Login Failed";
									return false;
								}
								if (text2.Contains("incorrect account or password") || text2.Contains("wrong account or password") || text2.Contains("Maximum number of attempts reached"))
								{
									bool flag = false;
									if (recoverpass)
									{
										flag = RecoverPass(needRecover: true);
									}
									Info.LoggedIn = flag;
									if (flag)
									{
										sql.UpdateTrangThai(Info.Uid, "Login Success", "Live");
										outMessage = "Login Successful";
										CloseCaptcha = true;
										return true;
									}
									sql.UpdateTrangThai(Info.Uid, "Wrong account or password", "Error");
									CloseCaptcha = true;
									outMessage = "Login Failed";
									return false;
								}
							}
							text2 = GetPageSource().ToLower();
							if (!text2.Contains("try again or log in with a different method") && !text2.Contains("too many attempts") && !text2.Contains("wrong account or password") && !text2.Contains("Maximum number of attempts reached"))
							{
								continue;
							}
							if (!recoverpass)
							{
								if (text2.Contains("try again or log in with a different method"))
								{
									outMessage = "Try again or log in with a different method";
									AddLogByAction("Try again or log in with a different method", isAppend: false);
								}
								else if (!text2.Contains("too many attempts"))
								{
									if (text2.Contains("wrong account or password"))
									{
										outMessage = "Wrong account or password";
										AddLogByAction("Wrong account or password", isAppend: false);
									}
								}
								else
								{
									outMessage = "Too many attempts";
									AddLogByAction("Too many attempts", isAppend: false);
								}
								CloseCaptcha = true;
								return false;
							}
							bool result2;
							if (!(result2 = RecoverPass()))
							{
								outMessage = "Recover passsword fail";
								AddLogByAction("Recover passsword fail", isAppend: false);
								CloseCaptcha = true;
								return false;
							}
							return result2;
						}
						sql.UpdateTrangThai(Info.Uid, "", "Live");
						if (deviceEntity.Rooted)
						{
							BackupAccountTiktok();
						}
						if (IsValidEmail(Info.Uid) || !text2.Contains(Info.Uid))
						{
							_ = Info.Uid;
							IWebElement webElement12 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"@\")]", m_driver);
							if (webElement12 == null || !(webElement12.Text != ""))
							{
								Utils.CCKLog("Khong lay duoc UID nick", Info.Uid);
							}
						}
						IWebElement webElement13 = ADBHelperCCK.AppGetObject("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.TextView", m_driver);
						if (webElement13 != null)
						{
							if (string.IsNullOrWhiteSpace(Info.FirstName) && webElement13.Text.Length > 5)
							{
								Info.FirstName = webElement13.Text;
								if (Info.FirstName != "")
								{
									sql.ExecuteQuery(string.Format("Update Account set Name='{0}' where id='@{1}' or id='{1}'", webElement13.Text, Info.Uid));
								}
							}
							IWebElement webElement14 = ADBHelperCCK.AppGetObject("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.view.ViewGroup/android.widget.TextView[1]", m_driver);
							if (webElement14 != null && webElement14.Text != "")
							{
								int num8 = Utils.Convert2Int(webElement14.Text);
								sql.ExecuteQuery(string.Format("Update Account set friend_count='{0}' where id='@{1}' or id='{1}'", num8, Info.Uid));
							}
							if (!Info.Avatar)
							{
								webElement14 = ADBHelperCCK.AppGetObject("//*[@text='Edit profile']", m_driver);
								if (webElement14 != null)
								{
									webElement14.Click();
									text2 = GetPageSource();
									if (text2.Contains("Change photo") && text2.Contains("Change video"))
									{
										sql.UpdateAvaInfo(Info.Uid, "Yes");
										ADBHelperCCK.Back(p_DeviceId);
									}
								}
							}
							sql.UpdateTrangThai(Info.Uid, "Login Success", "Live");
							Info.LoggedIn = true;
							BackupAccountTiktok();
							GotoTab(Tabs.Home);
							CloseCaptcha = true;
							outMessage = "Login Successful";
							return true;
						}
						BackupAccountTiktok();
						Utils.CCKLog("Khong lay duoc ten ", Info.Uid);
						outMessage = "Login Successful";
						return true;
					}
					sql.UpdateTrangThai(Info.Uid, "Account Die", "Error");
					outMessage = "Login Failed";
					return false;
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Login " + Info.Uid, ex.Message);
			}
			outMessage = "Login Failed";
			CloseCaptcha = true;
			return false;
		}

		private void EnableRootedDebugging(bool OnOff)
		{
			GetPageSource();
			ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -a android.settings.SETTINGS");
			Thread.Sleep(500);
			IWebElement webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[@text=\"System\" or @content-desc=\"System\"]", m_driver, p_DeviceId);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(500);
			IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text), 'advanced')]", m_driver);
			if (webElement2 == null)
			{
				return;
			}
			webElement2.Click();
			Thread.Sleep(200);
			webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[@text=\"Developer options\" or @content-desc=\"Developer options\"]", m_driver, p_DeviceId);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1500);
			webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[@text=\"Rooted debugging\" or @content-desc=\"Rooted debugging\"]", m_driver, p_DeviceId, 10);
			if (webElement == null)
			{
				return;
			}
			string pageSource = GetPageSource();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(pageSource);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//*[@text=\"Rooted debugging\" or @content-desc=\"Rooted debugging\"]");
			if (xmlNodeList != null && xmlNodeList.Count > 0)
			{
				XmlNode firstChild = xmlNodeList[0].ParentNode.NextSibling.FirstChild;
				if ((firstChild.Attributes["text"].Value == "OFF" && OnOff) || (firstChild.Attributes["text"].Value == "ON" && !OnOff))
				{
					webElement.Click();
					ADBHelperCCK.ExecuteCMD(p_DeviceId, "root");
					Thread.Sleep(2000);
				}
			}
			Thread.Sleep(200);
			xmlDocument = null;
		}

		private void WaitCaptchaXoay(RemoteWebDriver driver)
		{
			Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_CAPTCHA)));
			if (!driver.PageSource.Contains("android.webkit.WebView index=\"0\""))
			{
				return;
			}
			IWebElement webElement = ADBHelperCCK.AppGetObject("//android.widget.FrameLayout[@resource-id=\"android:id/content\"]/android.widget.FrameLayout/android.widget.FrameLayout/android.webkit.WebView[@index=\"0\"]", driver);
			if (webElement != null && Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.UNLOCKCAPTCHA_BYHAND)))
			{
				DateTime dateTime = DateTime.Now.AddMinutes(2.0);
				while (webElement != null && dateTime > DateTime.Now)
				{
					ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Đang chờ giải captcha bằng tay");
					Thread.Sleep(10000);
					webElement = ADBHelperCCK.AppGetObject("//android.widget.FrameLayout[@resource-id=\"android:id/content\"]/android.widget.FrameLayout/android.widget.FrameLayout/android.webkit.WebView[@index=\"0\"]", driver);
				}
			}
		}

		public bool RecoverPass(bool needRecover = false)
		{
			Utils.LogFunction("RecoverPass", "");
			string text = GetPageSource().ToLower();
			bool result = false;
			if (text.Contains("try again or log in with a different method") || text.Contains("username or password doesn't match our records") || text.Contains("incorrect account or password") || text.Contains("too many attempts") || text.Contains("wrong account or password") || text.Contains("maximum number of attempts reached") || needRecover)
			{
				IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Forgot password?\" or @content-desc=\"Forgot password?\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a8d989fb4b924c7a8821212794cd0412"), m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000);
					}
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement == null || !(Info.Email != ""))
					{
						AddLogByAction("Không có Email để khôi phục");
						return false;
					}
					webElement.Click();
					webElement.Clear();
					webElement.SendKeys(Info.Email);
					Thread.Sleep(1000);
					List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("5e5207bc4b374d7eadacbaa3b81ee54c"), m_driver);
					if (list != null)
					{
						list[list.Count - 1].Click();
						Thread.Sleep(5000);
						LoadingWait(2);
						text = GetLocalSource();
						if (text.Contains("try again or log in with a different method"))
						{
							return false;
						}
						if (text.Contains("Email address isn't registered yet"))
						{
							AddLogByAction("Email address isn'''t registered yet", isAppend: false);
							return false;
						}
						while (true)
						{
							string code = emailUti.GetCode(Info.Email, Info.PassEmail);
							if (code != "" && Utils.IsNumber(code))
							{
								webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
								if (webElement != null && Utils.IsNumber(code))
								{
									webElement.Click();
									webElement.Clear();
									webElement.SendKeys(code);
									Thread.Sleep(10000);
									text = GetPageSource();
									if (text.Contains("Email verification code error"))
									{
										IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"resend code\"]", m_driver);
										if (webElement2 != null)
										{
											webElement2.Click();
											Thread.Sleep(5000);
											continue;
										}
									}
									webElement = ADBHelperCCK.WaitMeCount("//android.widget.EditText", m_driver, 20);
									if (webElement != null)
									{
										webElement.Click();
										webElement.SendKeys(code + "@CCK");
										Thread.Sleep(5000);
										sql.UpdatePass(Info.Uid, code + "@CCK");
										Utils.CCKLog("Log\\changpasshistory.txt", Info.Uid + "|" + code + "@CCK|" + Info.Pass);
										text = GetLocalSource();
										Thread.Sleep(1000);
										result = true;
									}
								}
								else
								{
									AddLogByAction(code, isAppend: false);
								}
								break;
							}
							if (code != "" && code == "LOGIN failed")
							{
								AddLogByAction("LOGIN Email failed", isAppend: false);
								return false;
							}
							AddLogByAction("Wrong account or password", isAppend: false);
							return false;
						}
					}
					webElement = null;
				}
				text = GetPageSource();
				while (text.Contains("Log in"))
				{
					List<CCKNode> list2 = cckDriver.FindElements("//*[@text=\"Log in\" and contains(@resource-id,'" + CaChuaConstant.PACKAGE_NAME + ":id')]", text);
					if (list2 != null && list2.Count > 0)
					{
						list2[list2.Count - 1].Click();
						Thread.Sleep(5000);
						LoadingWait(5);
					}
					text = GetPageSource();
					if (text.Contains("Too many attempts. Try again later"))
					{
						result = false;
					}
					if (text.ToLower().Contains("currently suspended"))
					{
						sql.UpdateTrangThai(Info.Uid, "Your account was currently suspended", "Die");
						result = false;
					}
				}
				text = GetPageSource();
				if (text.Contains("Sync your contacts") || text.Contains("friends list to connect with them on"))
				{
					CCKNode cCKNode = cckDriver.FindElement("//*[@text=\"Don't allow\"]", GetPageSource());
					if (cCKNode != null)
					{
						cCKNode.Click();
						Thread.Sleep(1000);
						text = GetPageSource();
					}
				}
				if (text.Contains("text=\"Follow\""))
				{
					CCKNode cCKNode2 = cckDriver.FindElement("//*[@text=\"Don't allow\"]", GetPageSource());
					if (cCKNode2 != null)
					{
						cCKNode2.Click();
						Thread.Sleep(1000);
						text = GetPageSource();
					}
				}
			}
			return result;
		}

		internal void GetFacebook()
		{
		}

		public static void LogByVideo(string p_DeviceId, string uid)
		{
			if (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.LOG_VIDEO)))
			{
				new Task(delegate
				{
					ADBHelperCCK.ExecuteCMD(p_DeviceId, $"shell screenrecord /sdcard/{uid}.mp4 --time-limit 90");
					ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format("pull /sdcard/{0}.mp4 \"{1}\\{0}.mp4", uid, Application.StartupPath + "\\ScreenCap"));
					ADBHelperCCK.ExecuteCMD(p_DeviceId, $"shell rm /sdcard/{uid}.mp4");
				}).Start();
			}
		}

		internal void Upvideo()
		{
			Utils.LogFunction("Upvideo", "");
			GotoTab(Tabs.Profile);
			try
			{
				GetPageSource();
				int num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.ARTICLES_COUNT));
				if (num < 1)
				{
					num = 1;
				}
				List<ArticleItem> list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(Utils.ReadTextFile(CaChuaConstant.ARTICLES));
				List<ArticleItem> list2 = list.FindAll((ArticleItem o) => o.Active && o.Uid == Info.Uid).ToList();
				if (list2 == null || list2.Count == 0)
				{
					list2 = list.FindAll((ArticleItem o) => o.Active && string.IsNullOrWhiteSpace(o.Uid)).ToList();
				}
				if (list2 != null)
				{
					list2 = list2.OrderBy((ArticleItem _) => Guid.NewGuid()).ToList();
					num = Math.Min(num, list2.Count);
					for (int i = 0; i < num; i++)
					{
						try
						{
							LogByVideo(p_DeviceId, Info.Uid + "_" + i);
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell ime set com.android.adbkeyboard/.AdbIME");
							GotoTab(Tabs.Profile);
							string pageSource = GetPageSource();
							IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a8974622d0fe4fdcafc99698a86ea055"), m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(5000);
								pageSource = GetPageSource().ToLower();
								while (pageSource.Contains("\"allow\""))
								{
									IWebElement webElement2 = ADBHelperCCK.AppGetObject(GetFunctionByKey("d1be509b26e945e094ae1cd46c303841"), m_driver);
									if (webElement2 != null)
									{
										webElement2.Click();
										Thread.Sleep(1000);
										pageSource = GetPageSource().ToLower();
									}
								}
								if (list2.Count == 0)
								{
									return;
								}
								if (!Directory.Exists(list2[i].PictureFolder) && !File.Exists(list2[i].PictureFolder))
								{
									Utils.CCKLog("Khong tim thay nut Upvideo", "Upvideo function");
								}
								else
								{
									string OutfileName = "";
									if (Directory.Exists(list2[i].PictureFolder))
									{
										PushRandomFile(list2[i].PictureFolder, out OutfileName, list2[i].NumOfPic, list2[i].DeletePhotoAfterUse);
									}
									else if (File.Exists(list2[i].PictureFolder))
									{
										string directoryName = Path.GetDirectoryName(list2[i].PictureFolder);
										PushRandomFile(directoryName, out OutfileName, list2[i].NumOfPic, list2[i].DeletePhotoAfterUse, list2[i].PictureFolder);
									}
									pageSource = GetPageSource();
									if (!list2[i].NonMusic && pageSource.Contains("\"Add sound\"") && (list2[i].Songname.Trim() != "" || list2[i].TopSong || list2[i].FavoriteSong))
									{
										IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='add sound']", m_driver);
										if (webElement3 != null)
										{
											webElement3.Click();
											Thread.Sleep(1000);
											webElement3 = null;
											pageSource = GetPageSource();
											if (pageSource.Contains("\"Got it\""))
											{
												webElement3 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='got it']", m_driver);
												if (webElement3 != null)
												{
													webElement3.Click();
													Thread.Sleep(1000);
												}
											}
											if (!list2[i].TopSong && !list2[i].FavoriteSong)
											{
												pageSource = GetPageSource();
												if (pageSource.Contains("\"Search\""))
												{
													webElement3 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='search']", m_driver);
													if (webElement3 != null)
													{
														Thread.Sleep(1000);
														webElement3.Click();
														Thread.Sleep(1000);
														string[] array = list2[i].Songname.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
														ADBHelperCCK.InputTextUnicode(p_DeviceId, Utils.Spin(array[rnd.Next(array.Length)]));
														Thread.Sleep(2000);
														webElement3 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='search']", m_driver);
														if (webElement3 != null)
														{
															webElement3.Click();
															Thread.Sleep(5000);
														}
														pageSource = GetPageSource();
														List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.TextView", m_driver);
														if (list3 != null && list3.Count > 0)
														{
															IWebElement webElement4 = ADBHelperCCK.AppGetObject("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.ImageView", m_driver);
															if (webElement4 != null)
															{
																webElement4.Click();
																Thread.Sleep(1000);
															}
															list3[0].Click();
															Thread.Sleep(1000);
															webElement3 = ADBHelperCCK.AppGetObject("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.ImageView", m_driver);
															if (webElement3 != null)
															{
																webElement3.Click();
																Thread.Sleep(2000);
																WaitMe("//*[@text='Upload']");
															}
														}
														else
														{
															ADBHelperCCK.Back(p_DeviceId);
														}
													}
												}
											}
											else
											{
												webElement3 = (list2[i].TopSong ? ADBHelperCCK.AppGetObject("//*[lower-case(@text)='see all']", m_driver) : ADBHelperCCK.AppGetObject("//*[lower-case(@text)='favorites']", m_driver));
												if (webElement3 != null)
												{
													webElement3.Click();
													Thread.Sleep(3000);
													pageSource = GetPageSource().ToLower();
													List<IWebElement> list4 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.ImageView", m_driver);
													if (list4 != null && list4.Count > 0)
													{
														IWebElement webElement5 = list4[rnd.Next(list4.Count)];
														int x = webElement5.Location.X;
														int y = webElement5.Location.Y;
														ADBHelperCCK.Tap(p_DeviceId, x / 2, y);
														Thread.Sleep(1000);
														IWebElement webElement6 = ADBHelperCCK.AppGetObject("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout", m_driver);
														if (webElement6 != null)
														{
															webElement6.Click();
															Thread.Sleep(2000);
														}
													}
													else
													{
														ADBHelperCCK.Back(p_DeviceId);
													}
												}
											}
										}
									}
									DateTime now = DateTime.Now;
									IWebElement webElement7 = ADBHelperCCK.WaitMe(GetFunctionByKey("e8f76eda5a564d7faeae422e2c1dfd4f"), m_driver);
									if (webElement7 != null)
									{
										_ = DateTime.Now.Subtract(now).TotalMilliseconds;
										now = DateTime.Now;
										webElement7.Click();
										_ = DateTime.Now.Subtract(now).TotalMilliseconds;
										Thread.Sleep(2000);
										pageSource = GetPageSource().ToLower();
										while (pageSource.Contains("\"allow\""))
										{
											IWebElement webElement8 = ADBHelperCCK.AppGetObject(GetFunctionByKey("d1be509b26e945e094ae1cd46c303841"), m_driver);
											if (webElement8 != null)
											{
												webElement8.Click();
												Thread.Sleep(5000);
												pageSource = GetPageSource().ToLower();
											}
										}
										pageSource = GetPageSource();
										if (pageSource.ToLower().Contains("\"ok\""))
										{
											webElement7 = ADBHelperCCK.AppGetObject(GetFunctionByKey("ef9cf1ccc3b9420cae25bb36063709fb"), m_driver);
											if (webElement7 != null)
											{
												webElement7.Click();
												Thread.Sleep(2000);
											}
											pageSource = GetPageSource();
										}
										if (pageSource.Contains("text=\"All\""))
										{
											webElement7 = ADBHelperCCK.AppGetObject(GetFunctionByKey("7413265538cf4f86b372166ca839c858"), m_driver);
											if (webElement7 != null)
											{
												webElement7.Click();
												Thread.Sleep(1000);
												webElement7 = ADBHelperCCK.AppGetObject(GetFunctionByKey("b0297293c16c41ec8ed33cb84b678cb2"), m_driver);
												if (webElement7 != null)
												{
													webElement7.Click();
													Thread.Sleep(1000);
												}
											}
										}
										webElement7 = ADBHelperCCK.WaitMe(GetFunctionByKey("45b99b04deba41e8b4d43be73dad4471"), m_driver);
										if (webElement7 != null)
										{
											webElement7.Click();
											Thread.Sleep(1000);
											pageSource = GetPageSource();
											bool flag = false;
											if (pageSource.Contains("No videos available"))
											{
												webElement7 = ADBHelperCCK.WaitMe("//*[@text='Photos']", m_driver);
												if (webElement7 != null)
												{
													webElement7.Click();
													flag = true;
													Thread.Sleep(1000);
												}
											}
											IWebElement webElement9 = ADBHelperCCK.WaitMe(GetFunctionByKey("a26eb5cab9ff471597d3914b7ec67fb2"), m_driver);
											if (webElement9 != null)
											{
												if (list2[i].NumOfPic == 1)
												{
													webElement9.Click();
												}
												Thread.Sleep(2000);
												LoadingWait(360, "\"Loading...\"");
												pageSource = GetPageSource();
												if (flag)
												{
													if (list2[i].NumOfPic > 1)
													{
														IWebElement webElement10 = ADBHelperCCK.AppGetObject(GetFunctionByKey("524dbd42263145bd8db54f4c236593f8"), m_driver);
														if (webElement10 != null)
														{
															IWebElement webElement11 = ADBHelperCCK.AppGetObject(GetFunctionByKey("3ed9277642bb43aa9d03aa98863ddf4e"), m_driver);
															if (webElement11 == null)
															{
																webElement10.Click();
															}
														}
													}
													List<IWebElement> list5 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("d026964942da4183ba2b024cbbd9536c"), m_driver);
													if (list5 != null)
													{
														for (int j = 0; j < list5.Count; j++)
														{
															list5[j].Click();
															Thread.Sleep(1000);
														}
														IWebElement webElement12 = ADBHelperCCK.AppGetObject(GetFunctionByKey("3b44c25c0b8c4a0482872bbe6e4fdc8a"), m_driver);
														if (webElement12 != null)
														{
															webElement12.Click();
															webElement12 = null;
															Thread.Sleep(2000);
														}
													}
													pageSource = GetPageSource();
												}
												ADBHelperCCK.WaitMe(GetFunctionByKey("1d17e5209f7149738787af67ce029be4"), m_driver);
												pageSource = GetPageSource();
												while (pageSource.Contains("\"Next\""))
												{
													List<IWebElement> list6 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("564e79474ce54d2784fcd036d7a03786"), m_driver);
													if (list6 == null)
													{
														continue;
													}
													list6[list6.Count - 1].Click();
													Thread.Sleep(5000);
													pageSource = GetPageSource();
													while (pageSource.Contains("text=\"Loading...\""))
													{
														pageSource = GetPageSource();
													}
													if (list2[i].KickView)
													{
														IWebElement webElement13 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("8ebd457d43984af59edfc3560f96e215"), m_driver, 60);
														if (webElement13 != null)
														{
															webElement13.Click();
															Thread.Sleep(3000);
															ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 4, screen.X / 3, screen.Y / 2, 300);
															webElement13 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("af8a244ac44f4249a7f292ff65ab6f38"), m_driver, 2);
															if (webElement13 != null)
															{
																webElement13.Click();
																Thread.Sleep(3000);
																webElement13 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("ea8f098525cf4add8daa693703420cc9"), m_driver);
																if (webElement13 != null)
																{
																	webElement13.Click();
																	Thread.Sleep(1000);
																	webElement13.Click();
																	Thread.Sleep(1000);
																	webElement13.Click();
																	Thread.Sleep(5000);
																	pageSource = GetPageSource();
																	List<IWebElement> list7 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("6b39c59cf2514793b6ce9518c2c6fef0"), m_driver);
																	if (list7 != null)
																	{
																		IWebElement webElement14 = list7[list7.Count - 1];
																		if (webElement14 != null)
																		{
																			webElement14.Click();
																			Thread.Sleep(1000);
																			webElement14 = ADBHelperCCK.AppGetObject(GetFunctionByKey("e1550ec748364cd6ae4bfe53fe4ab048"), m_driver);
																			if (webElement14 != null)
																			{
																				webElement14.Click();
																				Thread.Sleep(2000);
																				webElement14 = ADBHelperCCK.AppGetObject(GetFunctionByKey("f2a45bf4e6864db6a87b47b14ea85b8b"), m_driver);
																				if (webElement14 != null)
																				{
																					webElement14.Click();
																					Thread.Sleep(2000);
																					ADBHelperCCK.Back(p_DeviceId);
																				}
																			}
																		}
																		Thread.Sleep(3000);
																	}
																}
															}
														}
													}
													pageSource = GetPageSource();
													if (!pageSource.Contains("Adjust clips") || (!list2[i].TurnOffVideoVolumn && list2[i].MusicVolumn >= 100))
													{
														continue;
													}
													IWebElement webElement15 = ADBHelperCCK.AppGetObject(GetFunctionByKey("ebd7d5b17984454f8daa0457c31f5d52"), m_driver);
													if (webElement15 != null)
													{
														webElement15.Click();
														Thread.Sleep(2000);
														if (!list2[i].TurnOffVideoVolumn)
														{
															if (list2[i].MusicVolumn < 100)
															{
																webElement15 = ADBHelperCCK.AppGetObject(GetFunctionByKey("c091fe7f333946fca02628bd7be04aff"), m_driver);
																if (webElement15 != null)
																{
																	webElement15.Click();
																	Thread.Sleep(1000);
																	List<IWebElement> list8 = ADBHelperCCK.AppGetObjects("//android.widget.FrameLayout/android.view.ViewGroup/android.view.ViewGroup/android.widget.FrameLayout[4]/android.view.View", m_driver);
																	if (list8 != null && list8.Count > 0)
																	{
																		list8[0].Click();
																		Thread.Sleep(1000);
																		webElement15 = ADBHelperCCK.AppGetObject("//*[@text=\"Volume\" or @content-desc=\"Volume\"]", m_driver);
																		if (webElement15 != null)
																		{
																			webElement15.Click();
																			Thread.Sleep(1000);
																			list8 = ADBHelperCCK.AppGetObjects("//android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.view.View", m_driver);
																			if (list8 != null && list8.Count > 0)
																			{
																				ADBHelperCCK.Tap(p_DeviceId, list8[0].Location.X + list2[0].MusicVolumn * list8[0].Size.Width / 2 / 100, list8[0].Location.Y + list8[0].Size.Height / 2);
																				Thread.Sleep(1000);
																			}
																		}
																	}
																}
															}
														}
														else
														{
															webElement15 = ADBHelperCCK.AppGetObject(GetFunctionByKey("08efd1a93ffc4457ab26f884a7483b67"), m_driver);
															if (webElement15 != null)
															{
																webElement15.Click();
																Thread.Sleep(1000);
																pageSource = GetPageSource();
															}
														}
														webElement15 = ADBHelperCCK.AppGetObject(GetFunctionByKey("8c3747cdd0f5404d9799a861fb6bd6ec"), m_driver);
														if (webElement15 != null)
														{
															webElement15.Click();
															Thread.Sleep(1000);
															webElement15 = ADBHelperCCK.AppGetObject(GetFunctionByKey("65211f89cff149a9812c86ff168ced68"), m_driver);
															if (webElement15 != null)
															{
																webElement15.Click();
																Thread.Sleep(1000);
															}
														}
													}
													pageSource = GetPageSource();
												}
												ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 4, screen.X / 3, screen.Y * 3 / 5);
												IWebElement webElement16 = ADBHelperCCK.WaitMe(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
												if ((list2[i].Contents != "" || OutfileName != "") && pageSource.Contains("android.widget.EditText"))
												{
													webElement16 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
													if (webElement16 != null)
													{
														webElement16.Click();
														if (OutfileName != "" && list2[i].ContentInVideoFile)
														{
															webElement16.SendKeys(OutfileName);
														}
														else
														{
															webElement16.SendKeys(Utils.Spin(list2[i].Contents));
														}
														Thread.Sleep(1000);
														pageSource = GetPageSource();
													}
												}
												try
												{
													string location = list2[i].Location;
													if (pageSource.ToLower().Contains("\"Location".ToLower()) && location != "")
													{
														webElement16 = ADBHelperCCK.AppGetObject("//*[@text=\"Location\"]", m_driver);
														if (webElement16 != null)
														{
															webElement16.Click();
															Thread.Sleep(2000);
															pageSource = GetPageSource().ToLower();
															while (pageSource.Contains("\"allow\""))
															{
																IWebElement webElement17 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='allow']", m_driver);
																if (webElement17 != null)
																{
																	webElement17.Click();
																	Thread.Sleep(1000);
																	pageSource = GetPageSource().ToLower();
																}
															}
														}
														pageSource = GetPageSource().ToLower();
														IWebElement webElement18 = ADBHelperCCK.WaitMe("//*[@text=\"Search locations\"]", m_driver);
														if (webElement18 != null)
														{
															webElement18.Click();
															Thread.Sleep(2000);
															webElement18.SendKeys(location);
															Thread.Sleep(5000);
															pageSource = GetPageSource();
															List<IWebElement> list9 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout", m_driver);
															if (list9 != null && list9.Count > 0)
															{
																list9[rnd.Next(list9.Count)].Click();
																Thread.Sleep(2000);
															}
														}
														pageSource = GetPageSource().ToLower();
													}
												}
												catch
												{
												}
												if (pageSource.ToLower().Contains("\"Add link".ToLower()) && list2[i].ProductName != "")
												{
													try
													{
														Utils.CCKLog("Add Link", "1");
														List<string> list10 = list2[i].ProductName.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
														int num2 = 0;
														while (list10 != null && num2 < 3 && list10.Count > 0)
														{
															Utils.CCKLog("Add Link", "3");
															IWebElement webElement19 = ADBHelperCCK.WaitMe(GetFunctionByKey("e4fb4850132149ba82e00d26293c1b2d"), m_driver);
															if (webElement19 == null)
															{
																continue;
															}
															webElement19.Click();
															Thread.Sleep(2000);
															webElement19 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("a8552746ff404063a383c14597f869ec"), m_driver, 2);
															if (webElement19 != null)
															{
																Utils.CCKLog("Add Link", "3a");
																webElement19.Click();
																Thread.Sleep(1000);
																webElement19 = ADBHelperCCK.WaitMeCount(GetFunctionByKey("2cfbb616f4bd484b84d5facf1a92abff"), m_driver, 10);
																if (!list2[i].ProductOnShop)
																{
																	pageSource = GetPageSource();
																	if (pageSource.Contains("Add more products"))
																	{
																		webElement16 = ADBHelperCCK.AppGetObject(GetFunctionByKey("b772f5f08ea74d6093784167f1ae81a1"), m_driver);
																		if (webElement16 != null)
																		{
																			webElement16.Click();
																			Thread.Sleep(5000);
																			pageSource = GetPageSource();
																			List<IWebElement> list11 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("2e740761e64f4a99b6a70611b794069f"), m_driver);
																			if (list11 != null && list11.Count > 1)
																			{
																				list11[1].Click();
																				Thread.Sleep(5000);
																			}
																		}
																	}
																	pageSource = GetPageSource();
																	IWebElement webElement20 = ADBHelperCCK.AppGetObject("//com.lynx.tasm.behavior.ui.LynxFlattenUI[@content-desc=\"Search\"]", m_driver);
																	if (webElement20 == null)
																	{
																		continue;
																	}
																	webElement20.Click();
																	Thread.Sleep(5000);
																	ADBHelperCCK.AppGetObject("//com.bytedance.ies.xelement.input.LynxInputView", m_driver)?.Click();
																	string text = list10[0];
																	list10.RemoveAt(0);
																	webElement16 = ADBHelperCCK.AppGetObject(GetFunctionByKey("a0c7f673344b49cab413b65229183c6a"), m_driver);
																	if (webElement16 == null)
																	{
																		continue;
																	}
																	webElement16.Click();
																	Thread.Sleep(1000);
																	Utils.CCKLog("ToastActivity", text);
																	ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("9c29d751d1ee481c8a30245ef2630c5e"), text));
																	Thread.Sleep(1000);
																	ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent 279");
																	Thread.Sleep(1000);
																	ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent KEYCODE_ENTER");
																	Thread.Sleep(5000);
																	Utils.CCKLog("Add Link", "4");
																	pageSource = GetPageSource();
																	ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format("shell am start -n \"com.cck.support/.ToastActivity\"  --es copy '{0}'", ""));
																	IWebElement webElement21 = ADBHelperCCK.AppFindAndGetObjectDown("//*[@text=\"Add\" or @content-desc=\"Add\"]", m_driver, p_DeviceId, 10);
																	if (webElement21 != null)
																	{
																		webElement21.Click();
																		Thread.Sleep(3000);
																		pageSource = GetPageSource();
																		if (pageSource.Contains("android.widget.HorizontalScrollView"))
																		{
																			List<IWebElement> list12 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ca18651fd8f64fc49a8d41b5cc19425b"), m_driver);
																			if (list12 != null && list12.Count >= 2)
																			{
																				ADBHelperCCK.Tap(p_DeviceId, list12[0].Location.X + list12[0].Size.Width / 2, (list12[0].Location.Y + list12[1].Location.Y) / 2);
																				Thread.Sleep(5000);
																			}
																			list12 = null;
																		}
																		pageSource = GetPageSource();
																		if (list2[i].ProductShortName != "")
																		{
																			IWebElement webElement22 = ADBHelperCCK.AppGetObject("//com.bytedance.ies.xelement.input.LynxInputView", m_driver);
																			if (webElement22 != null)
																			{
																				Utils.CCKLog("Add Link", "5");
																				webElement22.Click();
																				bool flag2 = false;
																				try
																				{
																					webElement22.Clear();
																					webElement22.SendKeys(Utils.Spin(list2[i].ProductShortName));
																					flag2 = true;
																				}
																				catch
																				{
																				}
																				if (!flag2)
																				{
																					Utils.CCKLog("Add Link", "6");
																					for (int k = 0; k < 2; k++)
																					{
																						ADBHelperCCK.Swipe(p_DeviceId, webElement22.Location.X, webElement22.Location.Y + webElement22.Size.Height / 2, webElement22.Location.X + webElement22.Size.Width, webElement22.Location.Y + webElement22.Size.Height / 2, 1500);
																						Thread.Sleep(1000);
																						ADBHelperCCK.SendDEL(p_DeviceId);
																						Thread.Sleep(1000);
																					}
																					ADBHelperCCK.SendToPhoneClipboard(p_DeviceId, Utils.Spin(list2[i].ProductShortName));
																					Thread.Sleep(500);
																					ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent 279");
																				}
																				Thread.Sleep(5000);
																			}
																		}
																		for (int l = 0; l < 5; l++)
																		{
																			IWebElement webElement23 = ADBHelperCCK.WaitMe(GetFunctionByKey("2cfbb616f4bd484b84d5facf1a92abff"), m_driver);
																			if (webElement23 != null && webElement23.Enabled)
																			{
																				break;
																			}
																			Thread.Sleep(5000);
																		}
																		IWebElement webElement24 = ADBHelperCCK.WaitMe(GetFunctionByKey("2cfbb616f4bd484b84d5facf1a92abff"), m_driver);
																		if (webElement24 != null)
																		{
																			ADBHelperCCK.Tap(p_DeviceId, webElement24.Location.X + webElement24.Size.Width / 4, webElement24.Location.Y + webElement24.Size.Height / 2);
																			Thread.Sleep(5000);
																		}
																		else
																		{
																			Utils.CCKLog("KHoong tim thay nut Add san pham", "//*[@content-desc=\"Add\" or @text=\"Add\"]");
																		}
																		webElement21 = null;
																	}
																	else
																	{
																		Utils.CCKLog("Add Link", "7");
																		ADBHelperCCK.Back(p_DeviceId);
																	}
																	Thread.Sleep(5000);
																	webElement16 = null;
																	continue;
																}
																string text2 = list10[0];
																list10.RemoveAt(0);
																webElement16 = ADBHelperCCK.AppGetObject(GetFunctionByKey("a0c7f673344b49cab413b65229183c6a"), m_driver);
																if (webElement16 == null)
																{
																	continue;
																}
																webElement16.Click();
																Thread.Sleep(1000);
																Utils.CCKLog("ToastActivity", text2);
																ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(GetFunctionByKey("9c29d751d1ee481c8a30245ef2630c5e"), text2));
																Thread.Sleep(1000);
																ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent 279");
																Thread.Sleep(1000);
																ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent KEYCODE_ENTER");
																Thread.Sleep(5000);
																Utils.CCKLog("Add Link", "4");
																pageSource = GetPageSource();
																ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format("shell am start -n \"com.cck.support/.ToastActivity\"  --es copy '{0}'", ""));
																List<IWebElement> list13 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("2cfbb616f4bd484b84d5facf1a92abff"), m_driver);
																if (list13 != null)
																{
																	list13[0].Click();
																	Thread.Sleep(3000);
																	pageSource = GetPageSource();
																	if (pageSource.Contains("android.widget.HorizontalScrollView"))
																	{
																		List<IWebElement> list14 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("ca18651fd8f64fc49a8d41b5cc19425b"), m_driver);
																		if (list14 != null && list14.Count >= 2)
																		{
																			ADBHelperCCK.Tap(p_DeviceId, list14[0].Location.X + list14[0].Size.Width / 2, (list14[0].Location.Y + list14[1].Location.Y) / 2);
																			Thread.Sleep(5000);
																		}
																		list14 = null;
																		pageSource = GetPageSource();
																		if (!pageSource.Contains("No internet connection"))
																		{
																			if (pageSource.Contains("Product name contains"))
																			{
																				IWebElement webElement25 = ADBHelperCCK.AppGetObject("//*[@text=\"Retry\" or @content-desc=\"Retry\"]", m_driver);
																				if (webElement25 != null)
																				{
																					webElement25.Click();
																					Thread.Sleep(1000);
																				}
																			}
																		}
																		else
																		{
																			IWebElement webElement26 = ADBHelperCCK.AppGetObject("//*[@text=\"Retry\" or @content-desc=\"Retry\"]", m_driver);
																			if (webElement26 != null)
																			{
																				webElement26.Click();
																				Thread.Sleep(1000);
																			}
																		}
																	}
																	pageSource = GetPageSource();
																	if (list2[i].ProductShortName != "")
																	{
																		IWebElement webElement27 = ADBHelperCCK.AppGetObject("//com.bytedance.ies.xelement.input.LynxInputView", m_driver);
																		if (webElement27 != null)
																		{
																			Utils.CCKLog("Add Link", "5");
																			webElement27.Click();
																			bool flag3 = false;
																			try
																			{
																				webElement27.Clear();
																				webElement27.SendKeys(Utils.Spin(list2[i].ProductShortName));
																				flag3 = true;
																			}
																			catch
																			{
																			}
																			if (!flag3)
																			{
																				Utils.CCKLog("Add Link", "6");
																				for (int m = 0; m < 2; m++)
																				{
																					ADBHelperCCK.Swipe(p_DeviceId, webElement27.Location.X, webElement27.Location.Y + webElement27.Size.Height / 2, webElement27.Location.X + webElement27.Size.Width, webElement27.Location.Y + webElement27.Size.Height / 2, 1500);
																					Thread.Sleep(1000);
																					ADBHelperCCK.SendDEL(p_DeviceId);
																					Thread.Sleep(1000);
																				}
																				ADBHelperCCK.SendToPhoneClipboard(p_DeviceId, Utils.Spin(list2[i].ProductShortName));
																				Thread.Sleep(500);
																				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell input keyevent 279");
																			}
																			Thread.Sleep(5000);
																		}
																	}
																	for (int n = 0; n < 5; n++)
																	{
																		IWebElement webElement28 = ADBHelperCCK.WaitMe(GetFunctionByKey("2cfbb616f4bd484b84d5facf1a92abff"), m_driver);
																		if (webElement28 != null && webElement28.Enabled)
																		{
																			break;
																		}
																		Thread.Sleep(5000);
																	}
																	IWebElement webElement29 = ADBHelperCCK.WaitMe(GetFunctionByKey("2cfbb616f4bd484b84d5facf1a92abff"), m_driver);
																	if (webElement29 == null)
																	{
																		Utils.CCKLog("KHoong tim thay nut Add san pham", "//*[@content-desc=\"Add\" or @text=\"Add\"]");
																	}
																	else
																	{
																		ADBHelperCCK.Tap(p_DeviceId, webElement29.Location.X + webElement29.Size.Width / 4, webElement29.Location.Y + webElement29.Size.Height / 2);
																		Thread.Sleep(5000);
																	}
																	list13 = null;
																}
																else
																{
																	Utils.CCKLog("Add Link", "7");
																	ADBHelperCCK.Back(p_DeviceId);
																}
																Thread.Sleep(5000);
																webElement16 = null;
																continue;
															}
															Utils.CCKLog("Add Link", "3q");
															ADBHelperCCK.Back(p_DeviceId);
															break;
														}
													}
													catch (Exception ex)
													{
														Utils.CCKLog("Add Link", "8" + ex.Message);
													}
												}
												pageSource = GetPageSource().ToLower();
												if (pageSource.Contains("\"Post".ToLower()))
												{
													List<IWebElement> list15 = ADBHelperCCK.AppGetObjects("//*[lower-case(@text)='post now' or lower-case(@text)=\"post\" or lower-case(@content-desc)='post now' or lower-case(@content-desc)=\"post\"]", m_driver);
													if (list15 != null)
													{
														list15[list15.Count - 1].Click();
														Thread.Sleep(10000);
														pageSource = GetPageSource().ToLower();
													}
													File.AppendAllLines("Log\\logpost.txt", new List<string> { string.Format("{0} -> {1} -> {2}", DateTime.Now.ToString("yyyy-MM-dd"), Info.Uid, list2[i].ProductName) });
													if (pageSource.Contains("add to home screen"))
													{
														list15 = ADBHelperCCK.AppGetObjects("//*[lower-case(@text)='cancel' or lower-case(@content-desc)='cancel']", m_driver);
														if (list15 != null)
														{
															list15[list15.Count - 1].Click();
															Thread.Sleep(10000);
															pageSource = GetPageSource().ToLower();
														}
													}
													list15 = ADBHelperCCK.AppGetObjects("//*[lower-case(@text)='post now' or lower-case(@text)=\"post\" or lower-case(@content-desc)='post now' or lower-case(@content-desc)=\"post\"]", m_driver);
													if (list15 != null)
													{
														list15[list15.Count - 1].Click();
														Thread.Sleep(20000);
													}
												}
												string text3 = "";
												text3 = $"{Info.Uid} -> Bắt đầu nghỉ chờ đăng bài tiếp theo {list2[i].Delay} giây {Environment.NewLine} lúc {DateTime.Now.ToString()}";
												Thread.Sleep(1000 * list2[i].Delay);
												DateTime.Now.AddMinutes(2.0);
												Utils.CCKLog(text3 + " -> Nghỉ xong lúc " + DateTime.Now.ToString() + " giây", "");
												pageSource = GetPageSource().ToLower();
											}
										}
									}
									else
									{
										Utils.CCKLog("Khong tim thay nut upload", "Upvideo function");
									}
								}
							}
							else
							{
								Utils.CCKLog("Khoong tim tay nut Dang video", "Upvideo function");
							}
							GotoTab(Tabs.Home);
							Thread.Sleep(5000);
						}
						catch
						{
						}
					}
				}
				else
				{
					Utils.CCKLog("Khong co du lieu bai dang", "Upvideo function");
				}
			}
			catch (Exception ex2)
			{
				Utils.CCKLog("Upvideo", ex2.Message);
			}
		}

		internal void SeedingVideo(bool dontUseLink = false)
		{
			Utils.LogFunction("SeedingVideo", "");
			GotoTab(Tabs.Home);
			if (!File.Exists(CaChuaConstant.SEEDING_LIVE_CONFIG))
			{
				return;
			}
			string pageSource = GetPageSource();
			LiveConfigEntity liveConfigEntity = new JavaScriptSerializer().Deserialize<LiveConfigEntity>(Utils.ReadTextFile(CaChuaConstant.SEEDING_LIVE_CONFIG));
			string[] array = liveConfigEntity.Link.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return;
			}
			liveConfigEntity.Link = array[rnd.Next(array.Length)];
			if (!((liveConfigEntity != null && liveConfigEntity.Link != "") || dontUseLink))
			{
				return;
			}
			int num = rnd.Next(liveConfigEntity.NumOfClick_From, liveConfigEntity.NumOfClick_To);
			int num2 = liveConfigEntity.TymRepeatCount - 1;
			dontUseLink = liveConfigEntity.Type == ViewType.Inbox;
			DateTime dateTime = DateTime.Now.AddSeconds(rnd.Next(liveConfigEntity.TimeFrom, liveConfigEntity.TimeTo));
			try
			{
				if (liveConfigEntity.Type != ViewType.Link)
				{
					if (liveConfigEntity.Type == ViewType.Inbox)
					{
						GotoTab(Tabs.Home);
						pageSource = GetPageSource();
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Following\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(5000);
							pageSource = GetPageSource();
							if (pageSource.Contains("androidx.recyclerview.widget.RecyclerView"))
							{
								List<string> list = new List<string>();
								int num3 = 0;
								while (true)
								{
									bool flag = false;
									List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.view.ViewGroup/android.widget.TextView[2]", m_driver);
									if (list2 != null)
									{
										for (int i = 0; i < list2.Count; i++)
										{
											list.Add(list2[i].Text.ToLower());
											if (list2[i].Text.ToLower().Contains(liveConfigEntity.ChannelName.Trim().ToLower()))
											{
												list2[i].Click();
												Thread.Sleep(2000);
												GetPageSource();
												flag = true;
												break;
											}
										}
									}
									if (flag)
									{
										break;
									}
									IWebElement webElement2 = ADBHelperCCK.AppGetObject("//androidx.recyclerview.widget.RecyclerView", m_driver);
									Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
									ADBHelperCCK.Swipe(p_DeviceId, ((Size)screenResolution).Width * 3 / 4, webElement2.Location.Y + webElement2.Size.Height / 2, 0, webElement2.Location.Y + webElement2.Size.Height / 2, 1000);
									if (num3 == list.Distinct().ToList().Count)
									{
										break;
									}
									num3 = list.Distinct().ToList().Count;
								}
							}
						}
					}
					else if (liveConfigEntity.Type == ViewType.Search)
					{
						pageSource = GetPageSource();
						IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[@text='Search' or @content-desc='Search'] | //android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[1]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView[2] | //androidx.viewpager.widget.ViewPager/android.widget.FrameLayout/android.view.ViewGroup/android.widget.ImageView", m_driver);
						if (webElement3 != null && liveConfigEntity.Keyword != "")
						{
							webElement3.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							IWebElement webElement4 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
							if (webElement4 != null)
							{
								webElement4.Click();
								webElement4.Clear();
								List<string> list3 = liveConfigEntity.Keyword.Split("|;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
								webElement4.SendKeys(list3[rnd.Next(list3.Count)].Trim());
								Thread.Sleep(5000);
								pageSource = GetPageSource();
								webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Search\" or @content-desc=\"Search\"]", m_driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(5000);
								}
								pageSource = GetPageSource();
								IWebElement webElement5 = WaitMe("//androidx.recyclerview.widget.RecyclerView/android.view.ViewGroup/android.widget.FrameLayout/android.widget.FrameLayout/android.view.View | //androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout[1]/android.widget.FrameLayout/android.widget.FrameLayout/com.lynx.tasm.behavior.ui.LynxFlattenUI[21]");
								if (webElement5 != null)
								{
									webElement5.Click();
									Thread.Sleep(5000);
								}
								webElement3 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"LIVE\" or upper-case(@content-desc)=\"LIVE\"]", m_driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(2000);
								}
							}
						}
					}
					goto IL_0692;
				}
				if (liveConfigEntity.Link.ToLower().Contains("/live"))
				{
					string text = liveConfigEntity.Link.Trim().Split('?')[0];
					text = (liveConfigEntity.Link = text.Substring(0, text.Length - 5));
				}
				ADBHelperCCK.OpenLink(p_DeviceId, liveConfigEntity.Link);
				Thread.Sleep(2000);
				WaitMe("//*[contains(@text,'@')]");
				pageSource = GetPageSource();
				IWebElement webElement6 = ADBHelperCCK.WaitMeCount("//*[@text=\"LIVE\"]", m_driver, 30);
				if (webElement6 != null)
				{
					webElement6.Click();
					Thread.Sleep(5000);
					pageSource = GetPageSource();
					IWebElement webElement7 = ADBHelperCCK.WaitMe("//*[@text=\"Add comment\" or @content-desc=\"Add comment\" or @text=\"Add comment...\" or @content-desc=\"Add comment...\"]", m_driver);
					if (webElement7 != null)
					{
						List<IWebElement> list4 = ADBHelperCCK.AppGetObjects($"//*[contains(@bounds,',{webElement7.Location.Y}][')]", m_driver);
						if (list4 != null && list4.Count > 0)
						{
							list4[list4.Count - 1].Click();
							Thread.Sleep(1000);
							IWebElement webElement8 = ADBHelperCCK.AppGetObject("//*[@text=\"Report\" or @content-desc=\"Report\"]", m_driver);
							if (webElement8 != null)
							{
								Point screenResolution2 = ADBHelperCCK.GetScreenResolution(p_DeviceId);
								ADBHelperCCK.Swipe(p_DeviceId, webElement8.Location.X + screenResolution2.X / 2, webElement8.Location.Y, webElement8.Location.X, webElement8.Location.Y);
								Thread.Sleep(500);
								IWebElement webElement9 = ADBHelperCCK.AppGetObject("//*[@text=\"Settings\" or @content-desc=\"Settings\"]", m_driver);
								if (webElement9 != null)
								{
									webElement9.Click();
									Thread.Sleep(500);
									pageSource = GetPageSource();
									IWebElement webElement10 = ADBHelperCCK.AppGetObject("//android.widget.Switch", m_driver);
									if (webElement10 != null)
									{
										webElement10.Click();
										Thread.Sleep(1000 * liveConfigEntity.DelayLiveView);
										webElement10.Click();
										ADBHelperCCK.Back(p_DeviceId);
									}
								}
							}
						}
					}
					goto IL_0692;
				}
				Screenshot screenshot = m_driver.GetScreenshot();
				screenshot.SaveAsFile($"Log\\{p_DeviceId}_{Info.Uid}_{DateTime.Now.ToFileTime()}.png", ScreenshotImageFormat.Png);
				Utils.CCKLog("Livestream", "Khong tim thay nut Live");
				ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Khong tim thay nut Live");
				dateTime = DateTime.Now.AddDays(-1.0);
				goto end_IL_00f8;
				IL_0c97:
				if (!liveConfigEntity.ViewShoppingCard || dateTime < DateTime.Now)
				{
					return;
				}
				pageSource = GetPageSource();
				IWebElement webElement11 = ADBHelperCCK.AppGetObject("//*[@text=\"Shop\"]", m_driver);
				if (webElement11 != null)
				{
					webElement11.Click();
					Thread.Sleep(2000);
					WaitMe("//*[upper-case(@text)=\"SHOP LIVE\" or upper-case(@content-desc)=\"SHOP LIVE\"]");
					pageSource = GetPageSource();
					if (pageSource.Contains("text=\"Buy\""))
					{
						IWebElement webElement12 = ADBHelperCCK.AppGetObject("//*[@text=\"Buy\"]", m_driver);
						if (webElement12 != null)
						{
							ADBHelperCCK.Tap(p_DeviceId, webElement12.Location.X, webElement12.Location.Y - webElement12.Size.Height);
							Thread.Sleep(1000 * rnd.Next(4, 10));
							ADBHelperCCK.Back(p_DeviceId);
						}
					}
				}
				ADBHelperCCK.Back(p_DeviceId);
				goto end_IL_00f8;
				IL_0692:
				pageSource = GetPageSource();
				if (pageSource.Contains("text=\"Buy\""))
				{
					IWebElement webElement13 = ADBHelperCCK.AppGetObject("//*[@text=\"Buy\"]", m_driver);
					if (webElement13 != null)
					{
						webElement13.Click();
						Thread.Sleep(5000);
						ADBHelperCCK.Back(p_DeviceId);
					}
				}
				WaitMe("//*[contains(lower-case(@text),'add comment')]");
				Point screen = ADBHelperCCK.GetScreenResolution(p_DeviceId);
				for (int j = 0; j < num; j++)
				{
					new Task(delegate
					{
						ADBHelperCCK.Tap(p_DeviceId, screen.X / 3, screen.Y / 4);
					}).Start();
					Thread.Sleep(50);
				}
				Thread.Sleep(10000);
				WaitMe("//*[contains(lower-case(@text),'add comment')]");
				for (int k = 0; k < liveConfigEntity.CommentRepeatCount; k++)
				{
					if (dateTime < DateTime.Now)
					{
						return;
					}
					pageSource = GetPageSource();
					IWebElement webElement14 = ADBHelperCCK.WaitMe("//*[contains(lower-case(@text),'add comment')]", m_driver);
					if (webElement14 == null)
					{
						continue;
					}
					List<string> list5 = Utils.GetFirstItemFromFile(CaChuaConstant.SEEDING_LIVE_COMMENT, liveConfigEntity.RemoveComment).Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
					if (list5 == null || list5.Count <= 0)
					{
						continue;
					}
					webElement14.Click();
					Thread.Sleep(1000);
					pageSource = GetPageSource();
					if (pageSource.Contains("text=\"Buy\""))
					{
						IWebElement webElement15 = ADBHelperCCK.AppGetObject("//*[@text=\"Buy\"]", m_driver);
						if (webElement15 != null)
						{
							webElement15.Click();
							Thread.Sleep(2000);
							ADBHelperCCK.Back(p_DeviceId);
						}
					}
					webElement14 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement14 == null)
					{
						continue;
					}
					webElement14.Click();
					if (list5 != null && list5.Count > 1)
					{
						for (int l = 0; l < list5.Count; l++)
						{
							webElement14 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),'add comment')]", m_driver);
							webElement14.Click();
							webElement14 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
							webElement14.SendKeys(list5[l].ToString());
							List<IWebElement> list6 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
							if (list6 != null && list6.Count == 2)
							{
								list6[list6.Count - 1].Click();
								Thread.Sleep(500);
							}
						}
						Thread.Sleep(1000 * rnd.Next(liveConfigEntity.CommentDelayFrom, liveConfigEntity.CommentDelayTo));
					}
					else if (list5 != null && list5.Count == 1)
					{
						webElement14.SendKeys(list5[0]);
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						List<IWebElement> list7 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
						if (list7 != null && list7.Count >= 2)
						{
							list7[list7.Count - 1].Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							Thread.Sleep(1000 * rnd.Next(liveConfigEntity.CommentDelayFrom, liveConfigEntity.CommentDelayTo));
						}
					}
				}
				if (liveConfigEntity.CopyLink)
				{
					pageSource = GetPageSource();
					List<IWebElement> list8 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
					if (list8 != null && list8.Count > 2)
					{
						for (int num4 = list8.Count - 1; num4 >= 0; num4--)
						{
							IWebElement webElement16 = list8[num4];
							if (webElement16.Location.X != 0 && webElement16.Location.Y != 0)
							{
								webElement16.Click();
								Thread.Sleep(1000);
								IWebElement webElement17 = ADBHelperCCK.AppGetObject("//*[@text=\"Copy link\"]", m_driver);
								if (webElement17 != null)
								{
									webElement17.Click();
									Thread.Sleep(2000);
									break;
								}
							}
						}
					}
				}
				if (liveConfigEntity.Follow)
				{
					pageSource = GetPageSource();
					if (pageSource.Contains("text=\"Follow\""))
					{
						List<IWebElement> list9 = ADBHelperCCK.AppGetObjects("//*[@text=\"Follow\"]", m_driver);
						if (list9 != null && list9.Count > 0)
						{
							list9[list9.Count - 1].Click();
							Thread.Sleep(3000);
						}
					}
				}
				pageSource = GetPageSource();
				if (liveConfigEntity.ShareCount <= 0)
				{
					goto IL_0c97;
				}
				if (dateTime < DateTime.Now)
				{
					return;
				}
				ADBHelperCCK.Tap(p_DeviceId, screen.X / 3, screen.Y / 2);
				ADBHelperCCK.TapLong(p_DeviceId, screen.X / 3, screen.Y / 2);
				pageSource = GetPageSource();
				int num5 = 0;
				bool flag2 = false;
				while (pageSource.Contains("\"Send\""))
				{
					List<IWebElement> list10 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.view.ViewGroup/android.widget.TextView[2]", m_driver);
					for (int m = 0; m < list10.Count; m++)
					{
						list10[m].Click();
						num5++;
						if (num5 <= liveConfigEntity.ShareCount)
						{
							Thread.Sleep(1000);
							continue;
						}
						flag2 = true;
						break;
					}
					if (!flag2)
					{
						ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y * 3 / 5);
						pageSource = GetPageSource();
						if (dateTime < DateTime.Now)
						{
							return;
						}
						continue;
					}
					ADBHelperCCK.Back(p_DeviceId);
					break;
				}
				ADBHelperCCK.Back(p_DeviceId);
				goto IL_0c97;
				end_IL_00f8:;
			}
			catch
			{
			}
			finally
			{
				DateTime now = DateTime.Now;
				while (dateTime > DateTime.Now)
				{
					pageSource = GetPageSource();
					if (pageSource.Contains("text=\"Buy\""))
					{
						IWebElement webElement18 = ADBHelperCCK.AppGetObject("//*[@text=\"Buy\"]", m_driver);
						if (webElement18 != null)
						{
							webElement18.Click();
							Thread.Sleep(1000 * rnd.Next(4, 10));
							ADBHelperCCK.Back(p_DeviceId);
						}
					}
					num = rnd.Next(liveConfigEntity.NumOfClick_From, liveConfigEntity.NumOfClick_To);
					if (num2 > 0)
					{
						num2--;
						for (int n = 0; n < num; n++)
						{
							new Task(delegate
							{
								ADBHelperCCK.Tap(p_DeviceId, this.screen.X / 3, this.screen.Y / 4);
							}).Start();
							Thread.Sleep(50);
						}
					}
					if (DateTime.Now.Subtract(now).TotalSeconds > 20.0)
					{
						now = DateTime.Now;
						ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Waiting: " + dateTime.Subtract(DateTime.Now).TotalSeconds);
						ADBHelperCCK.Tap(p_DeviceId, this.screen.X / 3, this.screen.Y / 4);
					}
					if (liveConfigEntity.ClickGimProduct)
					{
						ADBHelperCCK.OpenLink(p_DeviceId, liveConfigEntity.Link);
						Thread.Sleep(2000);
						WaitMe("//*[contains(@text,'@')]");
						pageSource = GetPageSource();
						IWebElement webElement19 = ADBHelperCCK.WaitMeCount("//*[@text=\"LIVE\"]", m_driver, 30);
						if (webElement19 != null)
						{
							webElement19.Click();
							Thread.Sleep(1000 * rnd.Next(liveConfigEntity.DelayGimFrom, liveConfigEntity.DelayGimto));
						}
					}
				}
			}
		}

		private void Scroll2Element(IWebElement element, ChromeDriver driver)
		{
			((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", new object[1] { element });
			((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", new object[1] { element });
			Thread.Sleep(500);
		}

		internal void RegisterAccountChrome(frmMain frmMain)
		{
			FBChrome fBChrome = new FBChrome();
			fBChrome.Init("https://www.tiktok.com/login", Guid.NewGuid().ToString("N"), capchat: true, "", 0, fullScreen: true);
			ChromeDriver driver = fBChrome.m_driver;
			driver.Navigate().GoToUrl("https://www.tiktok.com/login");
			driver.Navigate().GoToUrl("https://www.tiktok.com/signup");
			driver.Navigate().GoToUrl("https://www.tiktok.com/signup/phone-or-email");
			driver.Navigate().GoToUrl("https://www.tiktok.com/signup/phone-or-email/email");
			Thread.Sleep(5000);
			string pageSource = driver.PageSource;
			if (!pageSource.Contains("date-selector"))
			{
				if (pageSource.Contains("DivAgeSelector"))
				{
					ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(By.XPath("//div[contains(@class, 'DivSelector')]"));
					if (readOnlyCollection != null && readOnlyCollection.Count == 3)
					{
						Scroll2Element(readOnlyCollection[0], driver);
						readOnlyCollection[0].Click();
						Thread.Sleep(1000);
						ReadOnlyCollection<IWebElement> readOnlyCollection2 = readOnlyCollection[0].FindElements(By.XPath("div[contains(@class, 'DivOptionsWarpper')]/div"));
						IWebElement element = readOnlyCollection2[new Random().Next(readOnlyCollection2.Count)];
						Scroll2Element(element, driver);
						Scroll2Element(readOnlyCollection[1], driver);
						readOnlyCollection[1].Click();
						Thread.Sleep(1000);
						readOnlyCollection2 = readOnlyCollection[1].FindElements(By.XPath("div[contains(@class, 'DivOptionsWarpper')]/div"));
						IWebElement element2 = readOnlyCollection2[new Random().Next(10)];
						Scroll2Element(element2, driver);
						Scroll2Element(readOnlyCollection[2], driver);
						readOnlyCollection[2].Click();
						Thread.Sleep(1000);
						readOnlyCollection2 = readOnlyCollection[2].FindElements(By.XPath("div[contains(@class, 'DivOptionsWarpper')]/div"));
						int num = new Random().Next(1980, 2000);
						for (int i = 0; i < readOnlyCollection2.Count; i++)
						{
							if (readOnlyCollection2[i].Text == num.ToString())
							{
								Scroll2Element(readOnlyCollection2[i], driver);
								break;
							}
						}
					}
				}
			}
			else
			{
				ReadOnlyCollection<IWebElement> readOnlyCollection3 = driver.FindElements(By.XPath("//div[contains(@class, 'date-selector')]/div/div"));
				if (readOnlyCollection3 != null && readOnlyCollection3.Count == 3)
				{
					Scroll2Element(readOnlyCollection3[0], driver);
					Thread.Sleep(1000);
					ReadOnlyCollection<IWebElement> readOnlyCollection4 = readOnlyCollection3[0].FindElements(By.XPath("../ul/li/span"));
					IWebElement element3 = readOnlyCollection4[new Random().Next(readOnlyCollection4.Count)];
					Scroll2Element(element3, driver);
					Scroll2Element(readOnlyCollection3[1], driver);
					Thread.Sleep(1000);
					readOnlyCollection4 = readOnlyCollection3[1].FindElements(By.XPath("../ul/li/span"));
					IWebElement element4 = readOnlyCollection4[new Random().Next(10)];
					Scroll2Element(element4, driver);
					Scroll2Element(readOnlyCollection3[2], driver);
					readOnlyCollection3[2].Click();
					Thread.Sleep(1000);
					readOnlyCollection4 = readOnlyCollection3[2].FindElements(By.XPath("../ul/li/span"));
					int num2 = new Random().Next(1980, 2000);
					for (int j = 0; j < readOnlyCollection4.Count; j++)
					{
						if (readOnlyCollection4[j].Text == num2.ToString())
						{
							Scroll2Element(readOnlyCollection4[j], driver);
							break;
						}
					}
				}
			}
			DongVanFB dongVanFB = new DongVanFB(Utils.ReadTextFile(CaChuaConstant.DONGVANFB));
			Account account = dongVanFB.GetAccount();
			string status = "";
			IWebElement webElement = driver.FindElementByName("email");
			if (!(account.User != "") || webElement == null)
			{
				return;
			}
			Thread.Sleep(2000);
			webElement.Click();
			webElement.Clear();
			webElement.SendKeys(account.User);
			IWebElement webElement2 = driver.FindElement(By.XPath("//input[@type='password']"));
			Thread.Sleep(2000);
			if (webElement2 != null)
			{
				webElement2.Click();
				webElement2.Clear();
				webElement2.SendKeys(account.Pass + "@");
			}
			Thread.Sleep(5000);
			ReadOnlyCollection<IWebElement> readOnlyCollection5 = driver.FindElementsByXPath("//button[text()='Send code']");
			if (readOnlyCollection5 != null)
			{
				Thread.Sleep(1000);
				readOnlyCollection5[readOnlyCollection5.Count - 1].Click();
				Thread.Sleep(5000);
				string text = "";
				while (text == "")
				{
					text = dongVanFB.GetEmail(account.User, account.Pass, out status);
					Thread.Sleep(5000);
				}
			}
		}

		internal void ChangeAvatar()
		{
			Utils.LogFunction("ChangeAvatar", "");
			try
			{
				GotoTab(Tabs.Profile);
				string text = Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER);
				if (string.IsNullOrEmpty(text))
				{
					text = Application.StartupPath + "\\Data\\Avatar\\";
				}
				if (!Directory.Exists(text) || Directory.GetFiles(text).Length == 0)
				{
					string fileName = text + Guid.NewGuid().ToString("N") + ".jpg";
					int num = 0;
					while (num++ < 10)
					{
						string text2 = "";
						using (HttpClient httpClient = new HttpClient())
						{
							try
							{
								httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0");
								string requestUri = "https://boredhumans.com/api_faces2.php";
								HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
								if (result.IsSuccessStatusCode)
								{
									text2 = result.Content.ReadAsStringAsync().Result;
									Regex regex = new Regex("src=\"(.*?)\"");
									Match match = regex.Match(text2);
									if (match.Success)
									{
										string text3 = match.Groups[1].Value;
										if (text3.StartsWith("/"))
										{
											text3 = "https://boredhumans.com" + text3;
										}
										new WebClient().DownloadFile(text3, fileName);
										goto IL_0163;
									}
								}
							}
							catch (Exception)
							{
							}
						}
						Thread.Sleep(1000);
					}
				}
				goto IL_0163;
				IL_02e7:
				IWebElement webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"select from gallery\"]", m_driver);
				if (webElement == null)
				{
					return;
				}
				webElement.Click();
				Thread.Sleep(1000);
				string pageSource = GetPageSource();
				if (pageSource.Contains("\"All media\""))
				{
					IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"All media\" or @content-desc=\"All media\"]", m_driver);
					if (webElement2 != null)
					{
						webElement2.Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						if (!pageSource.Contains("Pictures"))
						{
							if (pageSource.Contains("All media"))
							{
								webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"All media\" or @content-desc=\"All media\"]", m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(1000);
								}
							}
						}
						else
						{
							webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Pictures\" or @content-desc=\"Pictures\"]", m_driver);
							if (webElement2 != null)
							{
								webElement2.Click();
								Thread.Sleep(1000);
							}
						}
					}
				}
				pageSource = GetPageSource();
				List<IWebElement> list = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout/android.view.View", m_driver);
				if (list == null || list.Count <= 0)
				{
					return;
				}
				IWebElement webElement3 = list[list.Count - 1];
				webElement3.Click();
				Thread.Sleep(1000);
				webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"confirm\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(2000);
				}
				webElement = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"save\")]", m_driver);
				if (webElement == null)
				{
					return;
				}
				webElement.Click();
				Thread.Sleep(5000);
				webElement = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"save\")]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					int num2 = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER_DELAY));
					if (num2 == 0)
					{
						num2 = 10;
					}
					Thread.Sleep(1000 * num2);
				}
				AddLogByAction("Change Avatar Successfully", isAppend: false);
				goto end_IL_0014;
				IL_0163:
				GetPageSource();
				GotoTab(Tabs.Profile);
				GetPageSource();
				ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y / 4, screen.X / 3, screen.Y * 3 / 5);
				IWebElement webElement4 = ADBHelperCCK.WaitMeCount("//*[@text=\"Edit profile\" or @text=\"Set up profile\"]", m_driver);
				if (webElement4 != null)
				{
					webElement4.Click();
					Thread.Sleep(1000);
					webElement4 = null;
				}
				webElement = ADBHelperCCK.AppGetObject("//android.widget.ScrollView/android.view.ViewGroup/android.widget.RelativeLayout/android.widget.FrameLayout/android.widget.ImageView[3]", m_driver);
				if (webElement == null)
				{
					webElement = ADBHelperCCK.AppGetObject("//android.widget.ScrollView/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.FrameLayout/android.widget.ImageView[2]", m_driver);
				}
				if (webElement == null)
				{
					return;
				}
				webElement.Click();
				Thread.Sleep(1000);
				pageSource = GetPageSource();
				if (pageSource.Contains("\"Change photo\""))
				{
					List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
					if (list2 != null && list2.Count == 3)
					{
						list2[1].Click();
						Thread.Sleep(1000);
					}
					pageSource = GetPageSource();
				}
				if (pageSource.ToLower().Contains("\"select from gallery\""))
				{
					if (!Directory.Exists(text) || Directory.GetFiles(text).Length == 0)
					{
						goto IL_02e7;
					}
					string OutfileName = "";
					if (PushRandomFile(text, out OutfileName, 1, Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER_DELETE))))
					{
						goto IL_02e7;
					}
					AddLogByAction("No Picture", isAppend: false);
				}
				end_IL_0014:;
			}
			catch (Exception ex2)
			{
				Utils.CCKLog("Upvideo", ex2.Message);
			}
		}

		private static string CalculateMD5(byte[] inputBytes)
		{
			using MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(inputBytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private bool PushRandomFile(string folder, out string OutfileName, int elementsCount = 1, bool deleteAfterPush = false, string fileName = "")
		{
			OutfileName = "";
			ADBHelperCCK.SetStoragePermission(p_DeviceId);
			try
			{
				if (File.Exists(folder))
				{
					fileName = folder;
					folder = folder.Substring(0, folder.LastIndexOf("\\") + 1);
				}
			}
			catch
			{
			}
			Thread.Sleep(500);
			List<string> list = new List<string>();
			if (Directory.Exists(folder))
			{
				List<string> list2 = (from s in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
					where s.EndsWith(".mp4") || s.EndsWith(".wmv") || s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".gif") || s.EndsWith(".jpeg") || s.EndsWith(".tif") || s.EndsWith(".tiff") || s.EndsWith(".bmp")
					select s).ToList();
				if (list2.Count <= 0)
				{
					return false;
				}
				if (fileName != "" && File.Exists(fileName))
				{
					list.Add(fileName);
				}
				else
				{
					list = list2.OrderBy((string arg) => Guid.NewGuid()).Take(elementsCount).ToList();
				}
			}
			string text = ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell mkdir /sdcard/Pictures");
			text = ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell ls /sdcard/Pictures");
			List<string> list3 = (from x in text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None)
				where !string.IsNullOrEmpty(x)
				select x).ToList();
			text = ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell ls /sdcard/");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell rm -rf /sdcard/Albums/");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell rm -rf /sdcard/DCIM/");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///mnt/sdcard/DCIM/");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///mnt/sdcard/DCIM/Camera");
			List<string> list4 = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None).ToList();
			foreach (string item in list4)
			{
				if (item.ToLower().EndsWith(".mp4"))
				{
					list3.Add(item);
				}
			}
			foreach (string item2 in list3)
			{
				ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell rm -rf /sdcard/Pictures/" + item2);
				ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///mnt/sdcard/Pictures/" + item2);
			}
			ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///mnt/sdcard/");
			string text2 = "";
			for (int i = 0; i < list.Count; i++)
			{
				string text3 = list[i];
				OutfileName = Path.GetFileNameWithoutExtension(text3);
				string extension = Path.GetExtension(text3);
				text2 = DateTime.Now.ToFileTime() + extension;
				string text4 = ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format(" push \"{0}\" \"{1}\"", text3, "/sdcard/Pictures/0" + text2));
				if (text4.Contains("push"))
				{
					ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Copy file " + text2);
				}
				text4 = ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///mnt/sdcard/Pictures/");
				text4 = ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///storage/emulated/0/Pictures/");
				text4 = ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///mnt/sdcard/Pictures/0" + text2);
				text4 = ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///storage/emulated/0/Pictures/0" + text2);
				Thread.Sleep(500);
				if (deleteAfterPush)
				{
					try
					{
						File.Delete(text3);
					}
					catch
					{
					}
				}
			}
			Thread.Sleep(2000);
			return true;
		}

		internal void ChangeBio()
		{
			Utils.LogFunction("ChangeBio", "");
			GotoTab(Tabs.Profile);
			string firstItemFromFile = Utils.GetFirstItemFromFile(CaChuaConstant.BIO, remove: false);
			string pageSource = GetPageSource();
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Edit profile\" or @text=\"Set up profile\"]", m_driver);
			if (webElement == null || !(firstItemFromFile != ""))
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			pageSource = GetPageSource().ToLower();
			if (!pageSource.Contains("add a bio to your profile") && !pageSource.Contains("text=\"bio\"") && !pageSource.Contains("\"add a bio\""))
			{
				return;
			}
			webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"add a bio to your profile\" or lower-case(@text)=\"bio\" or @text=\"add a bio\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			pageSource = GetPageSource();
			IWebElement webElement2 = ADBHelperCCK.WaitMe(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
			if (webElement2 != null)
			{
				webElement2.Clear();
				webElement2.SendKeys(firstItemFromFile);
				Thread.Sleep(1000);
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Save\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
				}
			}
		}

		internal void RegChangePass()
		{
			Utils.LogFunction("RegChangePass", "");
			Account account = new Account();
			try
			{
				GotoTab(Tabs.Profile);
				string pageSource = GetPageSource();
				IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
				if (webElement == null)
				{
					GotoTab(Tabs.Profile);
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
				}
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("f5f9364f18b7431586be3871c1693aee"), m_driver);
						if (list != null)
						{
							list[list.Count - 1].Click();
							Thread.Sleep(3000);
							string text = "";
							if (!(Utils.ReadTextFile(CaChuaConstant.EMAIL_REG_VERIFY) != ""))
							{
								_ = 1;
							}
							else
								new JavaScriptSerializer().Deserialize<EmailVerifyType>(Utils.ReadTextFile(CaChuaConstant.EMAIL_REG_VERIFY));
							webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("3ee9058fc4d444ffa88474df6b0c11b7"), m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(3000);
								pageSource = GetPageSource();
								IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"send code\"]", m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(5000);
									text = ReadEmailGoogleApp();
								}
								bool flag = true;
								while (true)
								{
									pageSource = GetPageSource();
									if (text == null || !(text != ""))
									{
										if (flag)
										{
											IWebElement webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("5e5207bc4b374d7eadacbaa3b81ee54c"), m_driver);
											if (webElement3 != null)
											{
												webElement3.Click();
												Thread.Sleep(15000);
											}
											flag = false;
											Thread.Sleep(15000);
											continue;
										}
										account.success = false;
										AddLogByAction("Email not Active", isAppend: false);
										break;
									}
									pageSource = GetPageSource();
									webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
									if (webElement == null)
									{
										break;
									}
									webElement.SendKeys(text);
									Thread.Sleep(5000);
									pageSource = GetPageSource();
									webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("aa5c62215ad547a2b30e730a303f90e5"), m_driver);
									if (webElement != null)
									{
										webElement.Click();
										Thread.Sleep(1000);
									}
									string text2 = Utils.ReadTextFile(CaChuaConstant.PASS).Trim();
									string text3 = ((!string.IsNullOrWhiteSpace(text2)) ? text2 : CreatePassword(10, uppercase: true, specialCharactor: true));
									while (true)
									{
										webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
										if (webElement != null)
										{
											webElement.Clear();
											webElement.SendKeys(text3);
											Info.Pass = text3;
											Thread.Sleep(2000);
											webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Next\"]", m_driver);
											if (webElement != null)
											{
												webElement.Click();
												Thread.Sleep(5000);
												pageSource = GetPageSource();
												if (pageSource.Contains("be the same as your old password"))
												{
													text3 = text2 + CreatePassword(3);
													continue;
												}
												sql.UpdateEmailAndPassAcount(Info.Uid, Info.Pass, Info.Email, Info.PassEmail);
												AddLogByAction("Change Pass Successfully", isAppend: false);
												break;
											}
											break;
										}
										break;
									}
									break;
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			if (!account.success)
			{
				File.AppendAllLines("Config\\reg_error_email.txt", new List<string> { account.accounts });
				File.AppendAllLines(CaChuaConstant.GOOGLE_APP_DOMAIM, new List<string> { account.accounts });
			}
		}

		internal void RegChangePassLite()
		{
			Utils.LogFunction("RegChangePassLite", "");
			Account account = new Account();
			new JavaScriptSerializer().Deserialize<EmailRegisterType>(Utils.ReadTextFile(CaChuaConstant.VERI_TYPE));
			try
			{
				GotoTab(Tabs.Me, AppFull: false);
				string pageSource = GetPageSource();
				IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("6309c629e6fa4dad87ed0adfc600bf82"), m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("efd57c906b184727b294c0caaa42091a"), m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000);
						webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("129e950e50d74653885a478ad0359318"), m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							EmailVerifyType emailVerifyType = ((!(Utils.ReadTextFile(CaChuaConstant.EMAIL_REG_VERIFY) != "")) ? EmailVerifyType.OneSec : new JavaScriptSerializer().Deserialize<EmailVerifyType>(Utils.ReadTextFile(CaChuaConstant.EMAIL_REG_VERIFY)));
							List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("9f9a748711eb4eb58f40effb0a691bee"), m_driver);
							if (list != null)
							{
								list[list.Count - 1].Click();
								Thread.Sleep(3000);
								string text = "";
								account = emailUti.GetEmail(Info, (emailVerifyType == EmailVerifyType.OneSec) ? EmailRegisterType.Onesecmail : EmailRegisterType.HotmailFromFile);
								ADBHelperCCK.EnableKey(p_DeviceId);
								webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("b88c4fe451724f32b2ed1deae9a709d5"), m_driver);
								if (webElement != null)
								{
									webElement.Click();
									Utils.CCKLog("Get Email:", account.User);
									ADBHelperCCK.InputText(p_DeviceId, account.User);
									Info.Email = account.User;
									Info.PassEmail = account.Pass;
									Thread.Sleep(1000);
									pageSource = GetPageSource();
									if (pageSource.Contains("\"Send code\""))
									{
										IWebElement webElement2 = ADBHelperCCK.AppGetObject(GetFunctionByKey("24f6bbbc0ee44e40b51889c6bfa3617f"), m_driver);
										if (webElement2 != null)
										{
											webElement2.Click();
											Thread.Sleep(5000);
										}
									}
									Info.Email = account.User;
									Info.PassEmail = account.Pass;
									for (int i = 0; i < 3; i++)
									{
										text = emailUti.GetCode(Info.Email, Info.PassEmail, "Tiktok", (emailVerifyType == EmailVerifyType.OneSec) ? EmailRegisterType.Onesecmail : EmailRegisterType.HotmailFromFile);
										if (text == "")
										{
											webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("18545e65e05d468fa9121a526f82631c"), m_driver);
											if (webElement != null)
											{
												webElement.Click();
												Thread.Sleep(3000);
												text = emailUti.GetCode(Info.Email, Info.PassEmail, "Tiktok", (emailVerifyType == EmailVerifyType.OneSec) ? EmailRegisterType.Onesecmail : EmailRegisterType.HotmailFromFile);
											}
										}
										if (!(text == "LOGIN failed"))
										{
											if (text != "")
											{
												break;
											}
											continue;
										}
										account.success = false;
										return;
									}
									if (text != null && text != "")
									{
										while (true)
										{
											pageSource = GetPageSource();
											webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("b88c4fe451724f32b2ed1deae9a709d5"), m_driver);
											if (webElement == null)
											{
												break;
											}
											for (int j = 0; j < 3; j++)
											{
												webElement.Click();
												ADBHelperCCK.EnableKey(p_DeviceId);
											}
											char[] array = text.ToCharArray();
											for (int k = 0; k < array.Length; k++)
											{
												ADBHelperCCK.EnableKey(p_DeviceId);
												ADBHelperCCK.InputText(p_DeviceId, array[k].ToString());
											}
											Thread.Sleep(10000);
											ADBHelperCCK.WaitMe(GetFunctionByKey("39983ca76d28475993375524f5458570"), m_driver);
											Thread.Sleep(2000);
											pageSource = GetPageSource();
											webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("e5030d3afa8e4733acccd39cfd551efc"), m_driver);
											if (webElement != null)
											{
												webElement.Click();
												Thread.Sleep(15000);
											}
											text = emailUti.GetCode(account.User, account.Pass, "Tiktok", EmailRegisterType.Onesecmail);
											if (text == "")
											{
												webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("18545e65e05d468fa9121a526f82631c"), m_driver);
												if (webElement != null)
												{
													webElement.Click();
													Thread.Sleep(3000);
													text = emailUti.GetCode(account.User, account.Pass, "Tiktok", EmailRegisterType.Onesecmail);
												}
											}
											if (text != null && text != "")
											{
												ADBHelperCCK.EnableKey(p_DeviceId);
												webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("b88c4fe451724f32b2ed1deae9a709d5"), m_driver);
												if (webElement != null)
												{
													for (int l = 0; l < 3; l++)
													{
														webElement.Click();
														ADBHelperCCK.EnableKey(p_DeviceId);
													}
													Thread.Sleep(2000);
													ADBHelperCCK.EnableKey(p_DeviceId);
													array = text.ToCharArray();
													for (int m = 0; m < array.Length; m++)
													{
														ADBHelperCCK.EnableKey(p_DeviceId);
														ADBHelperCCK.InputText(p_DeviceId, array[m].ToString());
													}
													Thread.Sleep(4000);
												}
												pageSource = GetPageSource();
												string text2 = Utils.ReadTextFile(CaChuaConstant.PASS).Trim();
												string text3 = ((!string.IsNullOrWhiteSpace(text2)) ? text2 : CreatePassword(10, uppercase: true, specialCharactor: true));
												while (true)
												{
													pageSource = GetPageSource();
													webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("b88c4fe451724f32b2ed1deae9a709d5"), m_driver);
													if (webElement == null)
													{
														break;
													}
													webElement.Click();
													webElement.Clear();
													ADBHelperCCK.InputText(p_DeviceId, text3);
													Info.Pass = text3;
													Thread.Sleep(2000);
													webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("55db77a4905f4f078e820888e7a2450a"), m_driver);
													if (webElement != null)
													{
														webElement.Click();
														Thread.Sleep(5000);
														pageSource = GetPageSource();
														if (pageSource.Contains("be the same as your old password"))
														{
															text3 = text2 + CreatePassword(3);
															continue;
														}
														if (!pageSource.Contains("Something went wrong. Please try again later."))
														{
															sql.UpdateEmailAndPassAcount(Info.Uid, Info.Pass, Info.Email, Info.PassEmail);
															AddLogByAction("Change password successfully", isAppend: false);
														}
														else
														{
															sql.UpdateEmailAndPassAcount(Info.Uid, Info.Pass, Info.Email, Info.PassEmail);
															AddLogByAction("Password change failed", isAppend: false);
														}
													}
													goto end_IL_002f;
												}
												continue;
											}
											account.success = false;
											AddLogByAction("Đổi Pass lỗi không có code email", isAppend: false);
											return;
										}
									}
									else
									{
										account.success = false;
										AddLogByAction("Email not Active", isAppend: false);
									}
								}
								else
								{
									account.success = false;
								}
							}
						}
					}
				}
				end_IL_002f:;
			}
			catch (Exception ex)
			{
				Utils.CCKLog("RegChangePassLite", ex.Message);
			}
			if (!account.success)
			{
				File.AppendAllLines("Config\\reg_error_email.txt", new List<string> { account.accounts });
				File.AppendAllLines(CaChuaConstant.GOOGLE_APP_DOMAIM, new List<string> { account.accounts });
			}
		}

		public string GetFunctionByKey(string key)
		{
			try
			{
				if (dicFunctionKey.ContainsKey(key))
				{
					return dicFunctionKey[key];
				}
				string text = "";
				WebClient webClient = new WebClient();
				webClient.Headers.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_3_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.5 Mobile/15E148 Snapchat/10.77.0.54 (like Safari/604.1)");
				webClient.Encoding = Encoding.UTF8;
                text = webClient.DownloadString(string.Format("https://selltool.net/private/xpath.php?session={0}&key={1}", "V9QWWAU3ZH9QBNGNZ86QI", key));
                //text = webClient.DownloadString(string.Format(Utils.ApiLocation + "/api/xpath.ashx?session={0}&key={1}", SessionString, key));
                if (text != "" && !dicFunctionKey.ContainsKey(key))
				{
					dicFunctionKey.Add(key, text);
				}
				return text;
			}
			catch
			{
			}
			return "";
		}

		private string ReadEmailGoogleApp()
		{
			if (ADBHelperCCK.IsInstallApp(p_DeviceId, "com.google.android.gm"))
			{
				ADBHelperCCK.OpenApp(p_DeviceId, "com.google.android.gm");
				Thread.Sleep(5000);
				string pageSource = GetPageSource();
				bool flag = true;
				while (flag)
				{
					flag = false;
					pageSource = GetPageSource();
					if (pageSource.ToUpper().Contains("\"GOT IT\""))
					{
						ADBHelperCCK.AppGetObject(GetFunctionByKey("30ee29001d744687bd07ce9d8fbd6c7e"), m_driver)?.Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						flag = true;
					}
					if (pageSource.Contains("\"SKIP\""))
					{
						ADBHelperCCK.AppGetObject(GetFunctionByKey("48acc5ea92b24e228355645242181918"), m_driver)?.Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						flag = true;
					}
					if (pageSource.ToUpper().Contains("\"TAKE ME TO GMAIL\""))
					{
						ADBHelperCCK.AppGetObject(GetFunctionByKey("b23d89759a02411cada363e4cd1750e1"), m_driver)?.Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						flag = true;
					}
					if (pageSource.Contains("\"Next\""))
					{
						List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("264f33da991c47ed99a33d15accbe874"), m_driver);
						if (list != null && list.Count > 0)
						{
							IWebElement webElement = list[list.Count - 1];
							ADBHelperCCK.Tap(p_DeviceId, webElement.Location.X + webElement.Size.Width / 2, webElement.Location.Y + webElement.Size.Height / 2);
						}
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						flag = true;
					}
					if (pageSource.Contains("\"Ok\""))
					{
						ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'OK')]", m_driver)?.Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						flag = true;
					}
					if (pageSource.Contains("\"Done\""))
					{
						ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'DONE')]", m_driver)?.Click();
						Thread.Sleep(1000);
						pageSource = GetPageSource();
						flag = true;
					}
				}
				pageSource = GetPageSource();
				if (!pageSource.Contains("Unread"))
				{
					ADBHelperCCK.CloseApp(p_DeviceId, "com.google.android.gm");
					Thread.Sleep(2000);
					ADBHelperCCK.OpenApp(p_DeviceId, "com.google.android.gm");
				}
				string text = "";
				Thread.Sleep(5000);
				pageSource = GetPageSource();
				DateTime dateTime = DateTime.Now.AddMinutes(2.0);
				DateTime dateTime2 = DateTime.Now.AddMinutes(1.0);
				while (dateTime > DateTime.Now)
				{
					pageSource = GetPageSource();
					try
					{
						if (pageSource.Contains("\"GOT IT\""))
						{
							ADBHelperCCK.AppGetObject(GetFunctionByKey("30ee29001d744687bd07ce9d8fbd6c7e"), m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
						}
						if (pageSource.Contains("\"SKIP\""))
						{
							ADBHelperCCK.AppGetObject(GetFunctionByKey("48acc5ea92b24e228355645242181918"), m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
						}
						if (pageSource.Contains("\"TAKE ME TO GMAIL\""))
						{
							ADBHelperCCK.AppGetObject(GetFunctionByKey("b23d89759a02411cada363e4cd1750e1"), m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
						}
						if (pageSource.Contains("\"Next\""))
						{
							List<IWebElement> list2 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("264f33da991c47ed99a33d15accbe874"), m_driver);
							if (list2 != null && list2.Count > 0)
							{
								IWebElement webElement = list2[list2.Count - 1];
								ADBHelperCCK.Tap(p_DeviceId, webElement.Location.X + webElement.Size.Width / 2, webElement.Location.Y + webElement.Size.Height / 2);
							}
							Thread.Sleep(1000);
							pageSource = GetPageSource();
						}
						if (pageSource.Contains("\"Ok\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'OK')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
						}
						if (pageSource.Contains("\"Done\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'DONE')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
						}
						if (pageSource.Contains("=\"Unread"))
						{
							IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("c87b963248384be2b18a9fd4ec2bb815"), m_driver);
							if (webElement != null)
							{
								Regex regex = new Regex(GetFunctionByKey("cea39fc787894821a66cd57580ab3d19"));
								Match match = regex.Match(webElement.Text);
								if (match.Success)
								{
									text = match.Groups[1].Value;
								}
								webElement.Click();
								Thread.Sleep(2000);
								if (text == "")
								{
									pageSource = GetPageSource();
									regex = new Regex("([0-9]+) is your");
									match = regex.Match(pageSource);
									if (match.Success)
									{
										text = match.Groups[1].Value;
									}
								}
							}
							if (text != "")
							{
								ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
								Thread.Sleep(2000);
								pageSource = GetPageSource();
								return text;
							}
						}
					}
					catch (Exception ex)
					{
						Utils.CCKLog("ReadEmailGoogleApp", ex.Message);
					}
					Regex regex2 = new Regex(GetFunctionByKey("cea39fc787894821a66cd57580ab3d19"));
					Match match2 = regex2.Match(pageSource);
					if (match2.Success)
					{
						text = match2.Groups[1].Value;
						if (text != "")
						{
							ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
							Thread.Sleep(2000);
							pageSource = GetPageSource();
						}
					}
					if (text != "")
					{
						break;
					}
					if (dateTime2 < DateTime.Now)
					{
						ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
						dateTime2 = DateTime.Now.AddYears(1);
						IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[contains(@text,\"Resend code\") or contains(@content-desc,\"Resend code\")]", m_driver);
						if (webElement2 != null)
						{
							webElement2.Click();
							Thread.Sleep(3000);
							ADBHelperCCK.OpenApp(p_DeviceId, "com.google.android.gm");
							Thread.Sleep(5000);
							Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
							ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 2, screenResolution.Y / 2, screenResolution.X / 2, screenResolution.Y / 4);
						}
					}
				}
				return text;
			}
			return "";
		}

		internal void ChangePass()
		{
			Utils.LogFunction("ChangePass", "");
			GotoTab(Tabs.Profile);
			string pageSource = GetPageSource();
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Profile menu\" or @text=\"Profile menu\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			pageSource = GetPageSource();
			string text = "";
			List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Manage account\") or contains(@text,\"Account\")]", m_driver);
			if (list == null)
			{
				return;
			}
			list[list.Count - 1].Click();
			Thread.Sleep(3000);
			webElement = ADBHelperCCK.WaitMe("//*[@text=\"Password\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(3000);
			pageSource = GetPageSource();
			if (pageSource.Contains("Enter phone number"))
			{
				IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[contains(@text,'verify your phone? Use email')]", m_driver);
				if (webElement2 != null)
				{
					ADBHelperCCK.Tap(p_DeviceId, webElement2.Location.X + webElement2.Size.Width * 9 / 10, webElement2.Location.Y + webElement2.Size.Height / 2);
					Thread.Sleep(1000);
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement != null)
					{
						webElement.SendKeys(Info.Email);
						Thread.Sleep(1000);
						webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Send code\" or @content-desc=\"Send code\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[contains(@text,'Use email')]", m_driver);
							if (webElement3 != null)
							{
								ADBHelperCCK.Tap(p_DeviceId, (double)webElement3.Location.X + (double)webElement3.Size.Width * 0.9, webElement3.Location.Y);
								Thread.Sleep(2000);
								webElement3 = ADBHelperCCK.AppGetObject("//*[@text='Email address' or @content-desc='Email address']", m_driver);
								if (webElement3 != null)
								{
									webElement3.SendKeys(Info.Email);
									Thread.Sleep(2000);
									webElement3 = ADBHelperCCK.AppGetObject("//*[@text='Send code' or @content-desc='Send code']", m_driver);
									if (webElement3 != null)
									{
										webElement3.Click();
										Thread.Sleep(2000);
										DateTime dateTime = DateTime.Now.AddSeconds(180.0);
										string text2 = "org.lineageos.jelly";
										ADBHelperCCK.CloseApp(p_DeviceId, text2);
										while (dateTime > DateTime.Now)
										{
											Thread.Sleep(1000);
											ADBHelperCCK.OpenLink(p_DeviceId, "https://gmail.com", text2);
											Thread.Sleep(5000);
											IWebElement webElement4 = ADBHelperCCK.WaitMeCount("//*[@text=\"Use the web version\" or @text=\"I am not interested\"]", m_driver, 30);
											if (webElement4 != null)
											{
												webElement4.Click();
												Thread.Sleep(5000);
											}
											IWebElement webElement5 = ADBHelperCCK.AppGetObject("//android.widget.Button[contains(@text,'Unread. TikTok')]", m_driver);
											if (webElement5 == null)
											{
												continue;
											}
											string text3 = webElement5.Text;
											webElement5.Click();
											Regex regex = new Regex("TikTok ([0-9]+) is your verification code");
											Match match = regex.Match(text3);
											if (match.Success && match.Groups.Count > 0)
											{
												text = match.Groups[1].Value;
												ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
												pageSource = GetPageSource();
												webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
												if (webElement != null)
												{
													webElement.SendKeys(text);
													break;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (pageSource.Contains("\"Send code\""))
			{
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Send code\" or @content-desc=\"Send code\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
				}
			}
			if (text == "")
			{
				text = emailUti.GetCode(Info.Email, Info.PassEmail);
				if (text == "googleapp")
				{
					if (ADBHelperCCK.IsInstallApp(p_DeviceId, "com.google.android.gm"))
					{
						ADBHelperCCK.CloseApp(p_DeviceId, "com.google.android.gm");
						ADBHelperCCK.OpenApp(p_DeviceId, "com.google.android.gm");
					}
					Thread.Sleep(5000);
					pageSource = GetPageSource();
					bool flag = true;
					while (flag)
					{
						flag = false;
						pageSource = GetPageSource();
						if (pageSource.Contains("\"Next\"") || pageSource.Contains("\"NEXT\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'NEXT')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							flag = true;
						}
						if (pageSource.Contains("\"GOT IT\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'GOT IT')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							flag = true;
						}
						if (pageSource.Contains("\"SKIP\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'SKIP')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							flag = true;
						}
						if (pageSource.Contains("\"TAKE ME TO GMAIL\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'TAKE ME TO GMAIL')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							flag = true;
						}
						if (pageSource.Contains("\"Next\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'NEXT')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							flag = true;
						}
						if (pageSource.Contains("\"Ok\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'OK')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							flag = true;
						}
						if (pageSource.Contains("\"Done\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(upper-case(@text),'DONE')]", m_driver)?.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource();
							flag = true;
						}
					}
					Thread.Sleep(5000);
					pageSource = GetPageSource();
					if (pageSource.Contains("text=\"Unread"))
					{
						IWebElement webElement6 = ADBHelperCCK.AppGetObject("//*[contains(@text,'Unread') and contains(@text,'TikTok')]", m_driver);
						if (webElement6 != null)
						{
							Regex regex2 = new Regex("([0-9]+) is your verification");
							Match match2 = regex2.Match(webElement6.Text);
							if (match2.Success)
							{
								text = match2.Groups[1].Value;
							}
							webElement6.Click();
						}
						if (text != "")
						{
							ADBHelperCCK.OpenApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
							Thread.Sleep(2000);
							pageSource = GetPageSource();
						}
					}
				}
			}
			if (text == "")
			{
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Resend code\" or @content-desc=\"Resend code\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
					text = emailUti.GetCode(Info.Email, Info.PassEmail);
				}
			}
			if (text != null && text != "")
			{
				pageSource = GetPageSource();
				webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
				if (webElement == null)
				{
					return;
				}
				webElement.SendKeys(text);
				Thread.Sleep(5000);
				pageSource = GetPageSource();
				if (pageSource.Contains("\"No thanks\""))
				{
					webElement = ADBHelperCCK.AppGetObject("//*[@text=\"No thanks\" or @content-desc=\"No thanks\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000);
					}
				}
				string text4 = Utils.ReadTextFile(CaChuaConstant.PASS).Trim();
				string text5 = ((!string.IsNullOrWhiteSpace(text4)) ? text4 : CreatePassword(10, uppercase: true, specialCharactor: true));
				while (true)
				{
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement != null)
					{
						webElement.Clear();
						webElement.SendKeys(text5);
						Info.Pass = text5;
						Thread.Sleep(2000);
						webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Next\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(5000);
							pageSource = GetPageSource();
							if (pageSource.Contains("be the same as your old password"))
							{
								text5 = text4 + CreatePassword(3);
								continue;
							}
							sql.UpdatePass(Info.Uid, Info.Pass);
							AddLogByAction("Change Password Successfully", isAppend: false);
							break;
						}
						break;
					}
					break;
				}
			}
			else
			{
				AddLogByAction("Email not Active", isAppend: false);
			}
		}

		internal void Rename(string rename = "")
		{
			string pageSource = GetPageSource();
			GotoTab(Tabs.Profile);
			string text = rename.Trim();
			pageSource = GetPageSource();
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Edit profile\" or @text=\"Set up profile\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			pageSource = GetPageSource();
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Name\"]", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(1000);
				IWebElement webElement2 = ADBHelperCCK.WaitMe("//android.widget.EditText", m_driver);
				if (webElement2 != null)
				{
					webElement2.Clear();
					if (rename.Trim() == "")
					{
						VNNameEntity vNNameEntity = new VNNameEntity();
						if (!File.Exists(CaChuaConstant.VN_Name))
						{
							text = "No Name";
						}
						else
						{
							vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
							text = (vNNameEntity.FirstName[rnd.Next(vNNameEntity.FirstName.Count)] + " " + vNNameEntity.LastName[rnd.Next(vNNameEntity.LastName.Count)]).Trim();
						}
						vNNameEntity = null;
					}
					else
					{
						text = rename;
					}
					webElement2.Click();
					webElement2.SendKeys(text);
					Thread.Sleep(1000);
					webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Save\"]", m_driver);
					if (webElement != null)
					{
						sql.ExecuteQuery(string.Format("Update Account set name='{0}' where id='{1}' or id='@{1}'", text, Info.Uid.TrimStart('@')));
						webElement.Click();
						Thread.Sleep(3000);
						pageSource = GetPageSource();
						if (pageSource.Contains("\"Confirm\""))
						{
							webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Confirm\"]", m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(5000);
							}
						}
					}
				}
			}
			_ = Info.Uid;
			if (!Info.Uid.StartsWith("user"))
			{
				return;
			}
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Username\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(2000);
			IWebElement webElement3 = ADBHelperCCK.WaitMe(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
			if (webElement3 == null)
			{
				return;
			}
			webElement3.Clear();
			Info.Uid = Utils.UnicodeToKoDau(text);
			webElement3.SendKeys(Info.Uid);
			Thread.Sleep(5000);
			pageSource = GetPageSource();
			if (!pageSource.Contains("a suggested username"))
			{
				webElement3 = ADBHelperCCK.WaitMe(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
				if (webElement3 != null)
				{
					webElement3.Clear();
					Info.Uid = Utils.UnicodeToKoDau(text) + rnd.Next(0, 999);
					webElement3.SendKeys(Info.Uid);
				}
			}
			else
			{
				List<IWebElement> list = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.TextView", m_driver);
				if (list != null && list.Count > 1)
				{
					IWebElement webElement4 = list[rnd.Next(1, list.Count)];
					Info.Uid = webElement4.Text.TrimStart('@');
					webElement4.Click();
					Thread.Sleep(1000);
				}
			}
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Save\"]", m_driver);
			while (!webElement.Enabled)
			{
				Thread.Sleep(2000);
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Save\"]", m_driver);
				pageSource = GetPageSource();
				if (pageSource.Contains("a suggested username"))
				{
					List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.TextView", m_driver);
					if (list2 != null && list2.Count > 1)
					{
						IWebElement webElement5 = list2[rnd.Next(1, list2.Count)];
						Info.Uid = webElement5.Text.TrimStart('@');
						webElement5.Click();
						Thread.Sleep(1000);
					}
				}
				else
				{
					webElement3 = ADBHelperCCK.WaitMe(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement3 != null)
					{
						webElement3.Clear();
						Info.Uid = Utils.UnicodeToKoDau(text) + rnd.Next(0, 999);
						webElement3.SendKeys(Info.Uid);
					}
				}
			}
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Save\"]", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(3000);
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Set username\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
				}
				pageSource = GetPageSource();
				if (Info.Uid != "" && pageSource.Contains(Info.Uid) && IsValidEmail(Info.Uid) && IsValidEmail(Info.Uid))
				{
					sql.ExecuteQuery($"Update Account set id='{Info.Uid}' where id='{Info.Email}'");
				}
			}
			text = "";
		}

		internal void RenameLite(string rename = "")
		{
		}

		private bool IsValidEmail(string email)
		{
			string text = email.Trim();
			if (!text.EndsWith("."))
			{
				try
				{
					MailAddress mailAddress = new MailAddress(email);
					return mailAddress.Address == text;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		internal void Change2FA()
		{
			GotoTab(Tabs.Profile);
			Guid.NewGuid().ToString().Replace("-", " ");
			GetPageSource();
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.zhiliaoapp.musically:id/nav_end\"]/android.widget.ImageView", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Security and login\" or @text=\"Security\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(3000);
			GetPageSource();
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"2-step verification\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(3000);
			List<IWebElement> list = ADBHelperCCK.AppGetObjects("//android.widget.CheckBox[@checked=\"true\"]", m_driver);
			if (list != null)
			{
				int num = 0;
				foreach (IWebElement item in list)
				{
					if (num % 2 == 0)
					{
						item.Click();
					}
					num++;
				}
			}
			List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[contains(@text,'Authenticator') or contains(@text,'Password')]", m_driver);
			foreach (IWebElement item2 in list2)
			{
				item2.Click();
			}
			GetPageSource();
			webElement = ADBHelperCCK.AppGetObject("//*[@text='Turn on']", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(5000);
			}
			webElement = ADBHelperCCK.AppGetObject("//*[@text='Skip']", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(2000);
			}
			Info.TwoFA = "TurnOn";
			sql.Update2FA(Info);
		}

		internal void ChangeEmail(bool change = true)
		{
			GotoTab(Tabs.Profile);
			Guid.NewGuid().ToString().Replace("-", " ");
			string pageSource = GetPageSource();
			if (!IsLogin())
			{
				return;
			}
			List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
			if (list == null || list.Count == 0)
			{
				return;
			}
			IWebElement webElement = list[list.Count - 1];
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			Thread.Sleep(1000);
			ADBHelperCCK.WaitMe("//*[contains(@text,\"Account\")]", m_driver);
			list = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Account\")]", m_driver);
			if (list != null)
			{
				list[list.Count - 1].Click();
				Thread.Sleep(3000);
			}
			webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"account information\"]", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(2000);
			}
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Email\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(3000);
			if (change)
			{
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Change email\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
				}
				pageSource = GetPageSource();
				if (!pageSource.Contains("Change email address"))
				{
					string code = emailUti.GetCode(Info.Email, Info.PassEmail);
					if (code == null || !(code != ""))
					{
						return;
					}
					pageSource = GetPageSource();
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement != null)
					{
						webElement.SendKeys(code);
						Thread.Sleep(5000);
						pageSource = GetPageSource();
					}
				}
				EmailVerifyType emailVerifyType = ((!(Utils.ReadTextFile(CaChuaConstant.EMAIL_REG_VERIFY) != "")) ? EmailVerifyType.OneSec : new JavaScriptSerializer().Deserialize<EmailVerifyType>(Utils.ReadTextFile(CaChuaConstant.EMAIL_REG_VERIFY)));
				EmailRegisterType emailRegisterType = EmailRegisterType.HotmailFromFile;
				emailRegisterType = emailVerifyType switch
				{
					EmailVerifyType.Hotmail => EmailRegisterType.HotmailFromFile, 
					EmailVerifyType.OneSec => EmailRegisterType.Onesecmail, 
					_ => EmailRegisterType.Getnada, 
				};
				Account email = emailUti.GetEmail(Info, emailRegisterType);
				webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
				if (webElement == null)
				{
					return;
				}
				webElement.SendKeys(email.User);
				Info.Email = email.User;
				Info.PassEmail = email.Pass;
				Thread.Sleep(2000);
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Send code\"]", m_driver);
				if (webElement == null)
				{
					return;
				}
				webElement.Click();
				Thread.Sleep(5000);
				string text = "";
				for (int i = 0; i < 3; i++)
				{
					text = emailUti.GetCode(Info.Email, Info.PassEmail, "Tiktok", emailRegisterType);
					if (text == "")
					{
						pageSource = GetPageSource();
						ADBHelperCCK.AppGetObject("//*[@text=\"Resend code\"]", m_driver)?.Click();
					}
					else if (text != "")
					{
						break;
					}
				}
				webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
				if (webElement != null && text != "")
				{
					webElement.SendKeys(text);
					Thread.Sleep(5000);
					sql.ExecuteQuery($"Update Account set Email='{Info.Email}', PassEmail='{Info.PassEmail}' where Id='{Info.Uid}'");
					AddLogByAction("Change Email Successfully");
				}
				else
				{
					AddLogByAction("Email change failed");
				}
				pageSource = GetPageSource();
				if (pageSource.Contains("email was added"))
				{
					sql.ExecuteQuery($"Update Account set Email='{Info.Email}', PassEmail='{Info.PassEmail}' where Id='{Info.Uid}'");
				}
				return;
			}
			pageSource = GetPageSource();
			if (pageSource.Contains("Verify email"))
			{
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Verify email\"]", m_driver);
				if (webElement == null)
				{
					return;
				}
				webElement.Click();
				Thread.Sleep(3000);
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Send code\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
				}
				string code2 = emailUti.GetCode(Info.Email, Info.PassEmail);
				if (code2 != null && code2 != "")
				{
					pageSource = GetPageSource();
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement != null)
					{
						webElement.SendKeys(code2);
						Thread.Sleep(5000);
					}
				}
				return;
			}
			pageSource = GetPageSource();
			if (pageSource.Contains("Change email") && pageSource.Contains("Unlink email"))
			{
				List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//android.widget.ImageView", m_driver);
				if (list2 != null && list2.Count > 0)
				{
					list2[list2.Count - 1].Click();
					Thread.Sleep(1000);
					for (int j = 0; j < 3; j++)
					{
						ADBHelperCCK.Back(p_DeviceId);
					}
					GotoTab(Tabs.Home);
				}
			}
			else
			{
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Email address\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
				}
				pageSource = GetPageSource();
				Account email2 = emailUti.GetEmail(Info);
				webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
				if (webElement != null)
				{
					webElement.SendKeys(email2.User);
					Info.Email = email2.User;
					Info.PassEmail = email2.Pass;
					Thread.Sleep(2000);
					webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Send code\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(5000);
						string code3 = emailUti.GetCode(Info.Email, Info.PassEmail);
						webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
						if (webElement != null && code3 != "")
						{
							webElement.SendKeys(code3);
							Thread.Sleep(5000);
							sql.ExecuteQuery($"Update Account set Email='{Info.Email}', PassEmail='{Info.PassEmail}' where Id='{Info.Uid}'");
							AddLogByAction("Add Email Successfully");
						}
						else
						{
							AddLogByAction("Add Email Failed");
						}
						pageSource = GetPageSource();
						if (pageSource.Contains("email was added"))
						{
							sql.ExecuteQuery($"Update Account set Email='{Info.Email}', PassEmail='{Info.PassEmail}' where Id='{Info.Uid}'");
						}
					}
				}
			}
			GotoTab(Tabs.Home);
		}

		private bool UpdateBioTSD()
		{
			Utils.LogFunction("UpdateBioTSD", "");
			GotoTab(Tabs.Profile);
			string text = GetPageSource().ToLower();
			if (text.Contains("\"profile\""))
			{
				TDSEntity tDSEntity = new TDSEntity();
				List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"profile\")]", m_driver);
				if (list != null && list.Count > 0)
				{
					list[list.Count - 1].Click();
					Thread.Sleep(1000);
					text = GetPageSource().ToLower();
					if (text.Contains("text=\"@"))
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//android.widget.TextView[@content-desc=\"Edit profile\" or @content-desc=\"Set up profile\" or @text=\"Edit profile\" or @text=\"Set up profile\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(2000);
							webElement = ADBHelperCCK.AppGetObject("//android.widget.TextView[@content-desc=\"Bio\" or @text=\"Bio\"]", m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(2000);
								webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
								if (webElement != null)
								{
									webElement.Clear();
									webElement.Click();
									Thread.Sleep(500);
									webElement.SendKeys(DataEncrypterDecrypter.GetMD5(tDSEntity.user_name));
									Thread.Sleep(500);
									webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Save\"]", m_driver);
									if (webElement != null)
									{
										webElement.Click();
										Thread.Sleep(500);
										webElement = null;
										sql.ExecuteQuery($"Update Account set uidlaybai='{tDSEntity.user_name}' where id='{Info.Uid}'");
										return true;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		private TDSEntity GetTokenByDevice(string device_id)
		{
			SQLiteUtils sQLiteUtils = new SQLiteUtils();
			DataTable dataTable = sQLiteUtils.ExecuteQuery($"Select * from TDSConfig where device_id='{device_id}'");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string token = Utils.Convert2String(dataTable.Rows[0]["token"]);
				string proxy = Utils.Convert2String(dataTable.Rows[0]["proxy"]);
				sQLiteUtils = null;
				return new TDSEntity
				{
					token = token,
					proxy = proxy
				};
			}
			return new TDSEntity
			{
				token = "",
				proxy = ""
			};
		}

		internal void TDSTym()
		{
			Utils.LogFunction("TDSTym", "");
			if (Info.TdsItem.token == "")
			{
				Info.TdsItem = GetTokenByDevice(p_DeviceId);
				if (Info.TdsItem.token == "")
				{
					ShowMessage(CurrentRow, "Chưa có API TDS");
					AddLogByAction("Chưa có API TDS", isAppend: false);
					return;
				}
			}
			string tDS_TYM = CaChuaConstant.TDS_TYM;
			TDSConfig tDSConfig = new TDSConfig();
			if (File.Exists(tDS_TYM))
			{
				tDSConfig = js.Deserialize<TDSConfig>(Utils.ReadTextFile(tDS_TYM));
			}
			TDS tDS = new TDS(Info.TdsItem);
			string outMessage = "";
			int num = 0;
			bool flag;
			do
			{
				IL_010c:
				if (flag = tDS.AccAccount(Info.Uid.Replace("@", ""), out outMessage))
				{
					continue;
				}
				if (outMessage.Contains("Vui lòng cập nhật ảnh đại diện không sử đụng ảnh mặc định"))
				{
					num++;
					ChangeAvatar();
					PublicInfo();
					if (num < 2)
					{
						goto IL_010c;
					}
				}
				AddLogByAction(outMessage);
				return;
			}
			while (!flag && outMessage.Contains("md5") && UpdateBioTSD());
			tDS.Run(Info.Uid.Replace("@", ""));
			int num2 = 0;
			int xu = tDS.GetXu();
			int num3 = xu;
			int stop = tDSConfig.Stop;
			int num4 = 5;
			while (num2 < tDSConfig.Count)
			{
				TDS.TaskTypeData task;
				while (true)
				{
					task = tDS.GetTask("tiktok_like");
					if (task.data == null || task.data.Count != 0)
					{
						break;
					}
					if (num4-- > 0)
					{
						ShowMessage(CurrentRow, "Đang chờ lấy việc");
						Thread.Sleep(5000);
						continue;
					}
					JobResult.Add("Không có việc để Tym");
					return;
				}
				foreach (TDS.TaskTypeData.TaskTypeSubData datum in task.data)
				{
					try
					{
						string link = datum.link;
						ADBHelperCCK.OpenLink(p_DeviceId, link);
						Thread.Sleep(2000);
						string pageSource = GetPageSource();
						if (pageSource.Contains("Sensitive content"))
						{
							ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"watch anyway\"]", m_driver)?.Click();
						}
						CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
						CCKNode cCKNode = cCKDriver.FindElement(GetFunctionByKey("45829ffb6c65498ea4396162f4695bfa"), pageSource);
						if (cCKNode == null)
						{
							continue;
						}
						File.AppendAllLines(p_DeviceId + ".txt", new List<string> { $"X={cCKNode.Location.X} - Y={cCKNode.Location.Y}" });
						cCKNode.Click();
						Thread.Sleep(1000 * rnd.Next(tDSConfig.DelayFrom, tDSConfig.DelayTo));
						num2 = tDS.UpdateTask(datum.id, "TIKTOK_LIKE_CACHE");
						if (num2 >= tDSConfig.NhanTien)
						{
							num2 = 0;
							tDS.GetXu("TIKTOK_LIKE_API", "tiktok_like".ToUpper());
							int xu2 = tDS.GetXu();
							if (num3 != xu2)
							{
								ShowMessage(CurrentRow, "Tym kiếm được " + GetStringFormat(xu2 - num3) + " xu");
								num3 = xu2;
								stop = tDSConfig.Stop;
							}
							else if (stop-- <= 0)
							{
								JobResult.Add("Tym " + (xu2 - xu).ToString("#,###") + " xu");
								return;
							}
						}
					}
					catch (Exception ex)
					{
						Utils.CCKLog("TDSTym: " + Info.Uid, ex.Message);
					}
				}
				if (num2 >= tDSConfig.NhanTien)
				{
					tDS.GetXu("TIKTOK_LIKE_API", "tiktok_like".ToUpper());
				}
			}
			int xu3 = tDS.GetXu();
			JobResult.Add("Tym " + GetStringFormat(xu3 - xu) + " xu");
		}

		internal void TDSComment()
		{
			Utils.LogFunction("TDSComment", "");
			if (Info.TdsItem.token == "")
			{
				Info.TdsItem = GetTokenByDevice(p_DeviceId);
				if (Info.TdsItem.token == "")
				{
					ShowMessage(CurrentRow, "Chưa có API TDS");
					AddLogByAction("Chưa có API TDS", isAppend: false);
					return;
				}
			}
			string tDS_COMMENT = CaChuaConstant.TDS_COMMENT;
			TDSConfig tDSConfig = new TDSConfig();
			if (File.Exists(tDS_COMMENT))
			{
				tDSConfig = js.Deserialize<TDSConfig>(Utils.ReadTextFile(tDS_COMMENT));
			}
			TDS tDS = new TDS(Info.TdsItem);
			string outMessage = "";
			int num = 0;
			do
			{
				if (!tDS.AccAccount(Info.Uid, out outMessage))
				{
					if (!outMessage.Contains("Vui lòng cập nhật ảnh đại diện không sử đụng ảnh mặc định"))
					{
						break;
					}
					num++;
					ChangeAvatar();
					PublicInfo();
					continue;
				}
				if (!tDS.Run(Info.Uid.Replace("@", "")))
				{
					AddLogByAction("Khởi tạo nick chạy Comment lỗi", isAppend: false);
					return;
				}
				int num2 = 0;
				int xu = tDS.GetXu();
				int num3 = xu;
				int stop = tDSConfig.Stop;
				while (num2 < tDSConfig.Count)
				{
					List<TDS.TaskTypeCommentData> taskComment = tDS.GetTaskComment();
					foreach (TDS.TaskTypeCommentData item in taskComment)
					{
						try
						{
							string link = item.link;
							string pageSource = GetPageSource();
							ADBHelperCCK.OpenLink(p_DeviceId, link);
							Thread.Sleep(2000);
							pageSource = GetPageSource();
							if (pageSource.Contains("Video isn't available"))
							{
								continue;
							}
							if (pageSource.Contains("Sensitive content"))
							{
								IWebElement webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"watch anyway\"]", m_driver);
								if (webElement != null)
								{
									webElement.Click();
									pageSource = GetPageSource();
								}
							}
							CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
							ADBHelperCCK.WaitMe(GetFunctionByKey("c2c5ad24244341d689fa85f7fd0c5a1a"), m_driver);
							CCKNode cCKNode = cCKDriver.FindElement(GetFunctionByKey("c2c5ad24244341d689fa85f7fd0c5a1a"), pageSource);
							if (cCKNode == null)
							{
								continue;
							}
							cCKNode.Click();
							Thread.Sleep(2000);
							IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"add comment\")]", m_driver);
							if (webElement2 == null)
							{
								continue;
							}
							webElement2.Click();
							Thread.Sleep(1000);
							CCKNode cCKNode2 = cckDriver.FindElement("//android.widget.EditText", GetPageSource());
							if (cCKNode2 != null)
							{
								cCKNode2.CCKSendKeys(item.noidung);
								Thread.Sleep(1000);
							}
							cCKNode2 = cckDriver.FindElement("//android.widget.EditText", GetPageSource());
							if (cCKNode2 == null)
							{
								continue;
							}
							List<CCKNode> list = cCKNode2.FindElements("./..");
							if (list == null || list.Count <= 0)
							{
								continue;
							}
							cCKNode2 = list[0];
							List<CCKNode> list2 = cCKNode2.FindElements("//*[@content-desc=\"Post comment\" or @text=\"Post comment\"]");
							if (list2.Count <= 0)
							{
								continue;
							}
							list2[list2.Count - 1].Click();
							Thread.Sleep(1000 * rnd.Next(tDSConfig.DelayFrom, tDSConfig.DelayTo));
							ADBHelperCCK.Back(p_DeviceId);
							tDS.UpdateTask(item.id, "TIKTOK_COMMENT_CACHE");
							tDS.GetXu(item.id, "tiktok_comment".ToUpper());
							int xu2 = tDS.GetXu();
							if (num3 == xu2)
							{
								if (stop-- <= 0)
								{
									JobResult.Add("Comment " + GetStringFormat(xu2 - xu) + " xu");
									return;
								}
							}
							else
							{
								ShowMessage(CurrentRow, "Comment kiếm được " + GetStringFormat(xu2 - num3) + " xu");
								num3 = xu2;
								stop = tDSConfig.Stop;
							}
							num2++;
							if (num2 <= tDSConfig.Count)
							{
								continue;
							}
							goto IL_0472;
						}
						catch (Exception ex)
						{
							Utils.CCKLog("TDSComment: " + Info.Uid, ex.Message);
						}
					}
					IL_0472:;
				}
				int xu3 = tDS.GetXu();
				JobResult.Add("Comment " + GetStringFormat(xu3 - xu) + " xu");
				return;
			}
			while (num < 2);
			AddLogByAction(outMessage, isAppend: false);
		}

		private string GetStringFormat(int value)
		{
			return (value == 0) ? "0" : value.ToString("#,###");
		}

		internal void TDSFollow()
		{
			Utils.LogFunction("TDSFollow", "");
			if (Info.TdsItem.token == "")
			{
				Info.TdsItem = GetTokenByDevice(p_DeviceId);
				if (Info.TdsItem.token == "")
				{
					ShowMessage(CurrentRow, "Chưa có API TDS");
					AddLogByAction("Chưa có API TDS", isAppend: false);
					return;
				}
			}
			string tDS_FOLLOW = CaChuaConstant.TDS_FOLLOW;
			TDSConfig tDSConfig = new TDSConfig();
			if (File.Exists(tDS_FOLLOW))
			{
				tDSConfig = js.Deserialize<TDSConfig>(Utils.ReadTextFile(tDS_FOLLOW));
			}
			TDS tDS = new TDS(Info.TdsItem);
			string outMessage = "";
			int num = 0;
			while (!tDS.AccAccount(Info.Uid, out outMessage))
			{
				if (outMessage.Contains("Vui lòng cập nhật ảnh đại diện không sử đụng ảnh mặc định"))
				{
					num++;
					ChangeAvatar();
					PublicInfo();
					if (num < 2)
					{
						continue;
					}
				}
				AddLogByAction(outMessage, isAppend: false);
				return;
			}
			if (tDS.Run(Info.Uid.Replace("@", "")))
			{
				int num2 = 0;
				int xu = tDS.GetXu();
				int num3 = xu;
				int stop = tDSConfig.Stop;
				for (; num2 < tDSConfig.Count; tDS.GetXu("TIKTOK_FOLLOW_API", "tiktok_follow"))
				{
					TDS.TaskTypeData task = tDS.GetTask("tiktok_follow");
					foreach (TDS.TaskTypeData.TaskTypeSubData datum in task.data)
					{
						try
						{
							string link = datum.link;
							ADBHelperCCK.OpenLink(p_DeviceId, link);
							Thread.Sleep(2000);
							IWebElement webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("b5b64146242f4e39be6d2d7c3d3d5613"), m_driver);
							if (webElement == null)
							{
								continue;
							}
							webElement.Click();
							Thread.Sleep(1000 * rnd.Next(tDSConfig.DelayFrom, tDSConfig.DelayTo));
							tDS.UpdateTask(datum.id, "TIKTOK_FOLLOW_CACHE");
							string pageSource = GetPageSource();
							if (pageSource.Contains("text=\"Message\""))
							{
								IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Message\"]", m_driver);
								if (webElement2 != null)
								{
									webElement2.Click();
									Thread.Sleep(1000);
									webElement2 = null;
								}
							}
							num2++;
							if (num2 % tDSConfig.NhanTien == 0)
							{
								tDS.GetXu("TIKTOK_FOLLOW_API", "tiktok_follow".ToUpper());
								int xu2 = tDS.GetXu();
								if (num3 != xu2)
								{
									ShowMessage(CurrentRow, "Follow kiếm được " + GetStringFormat(xu2 - num3) + " xu");
									num3 = xu2;
									stop = tDSConfig.Stop;
								}
								else if (stop-- <= 0)
								{
									JobResult.Add("Follow " + GetStringFormat(xu2 - xu) + " xu");
									return;
								}
								goto IL_0345;
							}
						}
						catch (Exception ex)
						{
							Utils.CCKLog("TDSFollow: " + Info.Uid, ex.Message);
						}
					}
					IL_0345:;
				}
				int xu3 = tDS.GetXu();
				JobResult.Add("Follow " + GetStringFormat(xu3 - xu) + " xu");
			}
			else
			{
				AddLogByAction("Thiết đặt Account chạy Follow lỗi", isAppend: false);
			}
		}

		internal void PublicInfo()
		{
			Utils.LogFunction("PublicInfo", "");
			GotoTab(Tabs.Profile);
			IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(1000);
				string pageSource = GetPageSource();
				webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Privacy\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(3000);
						Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
						ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 2, screenResolution.Y * 3 / 4, screenResolution.X / 2, screenResolution.Y / 4);
						pageSource = GetPageSource();
						List<IWebElement> list = ADBHelperCCK.AppGetObjects("//*[@text=\"Following list\" or @text=\"Following list\"]", m_driver);
						if (list != null && list.Count > 0)
						{
							list[list.Count - 1].Click();
							Thread.Sleep(1000);
							webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Everyone\"]", m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(2000);
								WaitLoading();
								ADBHelperCCK.Back(p_DeviceId);
							}
						}
						pageSource = GetPageSource();
						if (pageSource.Contains("text=\"Liked videos\"") || pageSource.Contains("text=\"Friends\""))
						{
							list = ADBHelperCCK.AppGetObjects("//*[@text=\"Liked videos\" or @text=\"Friends\"]", m_driver);
							if (list != null && list.Count > 0)
							{
								list[list.Count - 1].Click();
								Thread.Sleep(1000);
								webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Everyone\"]", m_driver);
								if (webElement != null)
								{
									webElement.Click();
									Thread.Sleep(2000);
									WaitLoading();
									ADBHelperCCK.Back(p_DeviceId);
								}
							}
							pageSource = GetPageSource();
						}
						sql.ExecuteQuery($"Update Account set profile='Everyone' where id='{Info.Uid}'");
					}
				}
			}
			GotoTab(Tabs.Profile);
			webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(1000);
				GetPageSource();
				webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[contains(@text,\"Account\")]", m_driver);
					if (list2 != null)
					{
						list2[list2.Count - 1].Click();
						Thread.Sleep(2000);
						webElement = ADBHelperCCK.WaitMe("//*[@text=\"Account information\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							GetPageSource();
							IWebElement webElement2 = ADBHelperCCK.AppGetObject("//android.widget.Button[contains(@content-desc,\"Date of birth\")]", m_driver);
							if (webElement2 != null)
							{
								try
								{
									string arg = webElement2.GetAttribute("content-desc").ToString().Replace("Date of birth, ", "");
									sql.ExecuteQuery($"Update Account set Year='{arg}' where id='{Info.Uid}'");
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			GotoTab(Tabs.Profile);
			webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(1000);
				GetPageSource();
				webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Creator tools\" or @text=\"Creator center\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
					Thread.Sleep(3000);
					List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//android.widget.Switch", m_driver);
					if (list3 != null && list3.Count > 0)
					{
						try
						{
							IWebElement webElement3 = list3[list3.Count - 1];
							string text = webElement3.Text.Trim();
							if (text.ToString().Contains("OFF"))
							{
								webElement3.Click();
								Thread.Sleep(500);
							}
						}
						catch
						{
						}
					}
				}
			}
			GotoTab(Tabs.Profile);
			if (Info.Avatar)
			{
				return;
			}
			IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[@text='Edit profile']", m_driver);
			if (webElement4 != null)
			{
				webElement4.Click();
				string pageSource2 = GetPageSource();
				if (pageSource2.Contains("Change photo") && pageSource2.Contains("Change video"))
				{
					sql.UpdateAvaInfo(Info.Uid, "Yes");
					ADBHelperCCK.Back(p_DeviceId);
				}
			}
		}

		private List<string> GetData(string fileUrl, string account)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			if (File.Exists(fileUrl))
			{
				string[] array = File.ReadAllLines(fileUrl);
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split("|;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (array3 != null && array3.Length == 2)
					{
						string key = array3[1];
						string item = array3[0];
						if (!dictionary.ContainsKey(key))
						{
							dictionary.Add(key, new List<string> { item });
						}
						else
						{
							dictionary[key].Add(item);
						}
					}
				}
			}
			return dictionary.ContainsKey(account) ? dictionary[account] : new List<string>();
		}

		public void AddProduct()
		{
			Utils.LogFunction("AddProduct", "");
			try
			{
				int num;
				while (true)
				{
					num = 0;
					LogByVideo(p_DeviceId, Info.Uid);
					if (!File.Exists(CaChuaConstant.LINK_PRODUCT))
					{
						break;
					}
					AddLinkEntity addLinkEntity = new JavaScriptSerializer().Deserialize<AddLinkEntity>(Utils.ReadTextFile(CaChuaConstant.LINK_PRODUCT));
					if (addLinkEntity == null)
					{
						break;
					}
					screen = ADBHelperCCK.GetScreenResolution(p_DeviceId);
					GotoTab(Tabs.Profile);
					string pageSource = GetPageSource();
					File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "1. Profile menu" });
					IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
					if (webElement == null)
					{
						List<IWebElement> list = ADBHelperCCK.AppGetObjects(GetFunctionByKey("3be7baf0e52b4a1ebcf88ceefb1466ab"), m_driver);
						if (list != null && list.Count > 0)
						{
							webElement = list[0];
							if (screen.X != 1080)
							{
								ADBHelperCCK.Tap(p_DeviceId, screen.X * 9 / 10, webElement.Location.Y);
							}
							else
							{
								ADBHelperCCK.Tap(p_DeviceId, 1006.0, 131.0);
							}
							Thread.Sleep(3000);
						}
					}
					else
					{
						webElement.Click();
						Thread.Sleep(1000);
						webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("8af17f5104134853991957780610d2f0"), m_driver);
						if (webElement == null && screen.X == 1080 && screen.Y == 1920)
						{
							ADBHelperCCK.Tap(p_DeviceId, 1006.0, 131.0);
							Utils.CCKLog("Click vào Nút ( 975 + 1038) / 2, (100 + 163) / 2", "Add Product");
						}
					}
					webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("8af17f5104134853991957780610d2f0"), m_driver);
					if (webElement != null)
					{
						File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "2. Creator tools" });
						webElement.Click();
						Thread.Sleep(1000);
						ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4);
						pageSource = GetPageSource();
						webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("a33413fc95bf4129bcad950a63ac8a6f"), m_driver);
						if (webElement != null)
						{
							File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "3. TikTok Shop" });
							webElement.Click();
							Thread.Sleep(5000);
						}
						else
						{
							File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "3a. TikTok Shop" });
							ADBHelperCCK.Back(p_DeviceId);
							List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[@resource-id=\"" + CaChuaConstant.PACKAGE_NAME + ":id/cat\"]", m_driver);
							if (list2 != null && list2.Count == 5)
							{
								list2[1].Click();
								Thread.Sleep(3000);
								pageSource = GetPageSource();
								if (pageSource.Contains("TikTok Shop"))
								{
									List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//*[@text=\"Enter\" or @text=\"TikTok Shop\" or @content-desc=\"TikTok Shop\"]", m_driver);
									if (list3 != null)
									{
										list3[list3.Count - 1].Click();
										Thread.Sleep(3000);
									}
								}
							}
						}
						pageSource = GetPageSource();
						webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("b3892cc519e94b658ee2f20089b4077f"), m_driver);
						if (webElement != null)
						{
							File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "4. product marketplace" });
							webElement.Click();
							Thread.Sleep(5000);
							ADBHelperCCK.WaitMe(GetFunctionByKey("50ecdfc20b1f42d7884368f2eb2c5de6"), m_driver);
							List<string> list4 = (addLinkEntity.LinkAndName ? GetData(addLinkEntity.FileUrl, Info.Uid) : Utils.ReadTextFile(addLinkEntity.FileUrl).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList());
							if (list4.Count > 0)
							{
								int num2 = 0;
								while (num2 < list4.Count)
								{
									pageSource = GetPageSource();
									List<IWebElement> list5 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("48022b182f3a40f28d92db0cc81511d6"), m_driver);
									if (list5 != null && list5.Count > 0)
									{
										File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "5. Product Marketplace" });
										IWebElement webElement2 = list5[0];
										ADBHelperCCK.Tap(p_DeviceId, (double)screen.X * 0.95, webElement2.Location.Y + webElement2.Size.Height / 2);
										Thread.Sleep(2000);
										pageSource = GetPageSource();
										List<IWebElement> list6 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("2e740761e64f4a99b6a70611b794069f"), m_driver);
										if (list6 != null && list6.Count == 3)
										{
											File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "6. android.widget.ImageView" });
											list6[list6.Count - 1].Click();
											Thread.Sleep(5000);
										}
										else
										{
											File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string>
											{
												"6a. khong thay android.widget.ImageView",
												GetPageSource()
											});
											Utils.CCKLog("Khong tim thay", "android.widget.ImageView");
										}
										IWebElement webElement3 = ADBHelperCCK.WaitMe(GetFunctionByKey("11cc1a7aff9c486dbe596fc5b002d89f"), m_driver);
										if (webElement3 != null)
										{
											string text = list4[num2];
											if (text != "")
											{
												webElement3.Click();
												ADBHelperCCK.SendToPhoneClipboard(p_DeviceId, text);
												ADBHelperCCK.PasteClipboard(p_DeviceId);
												ADBHelperCCK.SendToPhoneClipboard(p_DeviceId, "");
												Thread.Sleep(5000);
												IWebElement webElement4 = ADBHelperCCK.WaitMe(GetFunctionByKey("1d978c6f363b453e872d55ee6ad093ba"), m_driver);
												if (webElement4 == null)
												{
													Utils.CCKLog("Khong tim thay", "Nut Add");
													pageSource = GetPageSource();
													IWebElement webElement5 = ADBHelperCCK.AppGetObject("//android.widget.ScrollView/android.widget.HorizontalScrollView/android.widget.LinearLayout", m_driver);
													if (webElement5 != null)
													{
														IWebElement webElement6 = ADBHelperCCK.AppGetObject("//android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup", m_driver);
														IWebElement webElement7 = ADBHelperCCK.AppGetObject("//android.widget.FrameLayout/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup", m_driver);
														if (webElement7 != null && webElement6 != null)
														{
															int num3 = webElement7.Location.X + webElement7.Size.Width / 2;
															int num4 = webElement6.Location.Y + (webElement7.Location.Y + webElement7.Size.Height - webElement6.Location.Y) / 2;
															ADBHelperCCK.Tap(p_DeviceId, num3, num4);
															Thread.Sleep(2000);
															ADBHelperCCK.Back(p_DeviceId);
														}
													}
												}
												else
												{
													pageSource = GetPageSource();
													if (!pageSource.Contains("text=\"Added\""))
													{
														List<IWebElement> list7 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("dd11e4b55a924d24920bd2606d17b342"), m_driver);
														if (list7 != null && list7.Count > 0)
														{
															webElement4 = list7[list7.Count - 1];
															int num5 = webElement4.Location.X + webElement4.Size.Width * 2 / 3;
															int num6 = webElement4.Location.Y + webElement4.Size.Height / 2;
															ADBHelperCCK.Tap(p_DeviceId, num5, num6);
															Thread.Sleep(5000);
														}
														else
														{
															Utils.CCKLog("Khong tim thay", "Add to showcase");
														}
														Thread.Sleep(2000);
														pageSource = GetPageSource();
														if (pageSource.Contains("\"Continue\"") && pageSource.Contains("Product link from Affiliate"))
														{
															webElement4 = ADBHelperCCK.WaitMe(GetFunctionByKey("820eebbea8f749639172696f5e20b02c"), m_driver);
															if (webElement4 != null)
															{
																int num7 = webElement4.Location.X + webElement4.Size.Width * 2 / 3;
																int num8 = webElement4.Location.Y + webElement4.Size.Height / 2;
																ADBHelperCCK.Tap(p_DeviceId, num7, num8);
																Thread.Sleep(5000);
																pageSource = GetPageSource();
															}
														}
														if (pageSource.Contains("text=\"Added\""))
														{
															File.AppendAllLines("Log\\addlink.txt", new List<string> { string.Format("{0} -> {1} -> {2}", DateTime.Now.ToString("yyyy-MM-dd"), Info.Uid, text) });
															AddLogByAction("Add successful link", isAppend: false);
															num++;
														}
													}
													pageSource = GetPageSource();
													ADBHelperCCK.Back(p_DeviceId);
													Thread.Sleep(3000);
												}
											}
										}
										num2++;
										continue;
									}
									goto IL_09b6;
								}
							}
							else
							{
								Utils.CCKLog("Product Marketplace", "Khong co link sản pham");
							}
						}
						else
						{
							File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string> { "4a. khong thay product marketplace" });
							Utils.CCKLog("Product Marketplace", "Khong tim thay");
						}
					}
					else
					{
						Utils.CCKLog("Khong tim thay", "Creator tools");
					}
					break;
					IL_09b6:
					File.AppendAllLines("Log\\" + p_DeviceId + ".txt", new List<string>
					{
						"5a. Product Marketplace",
						GetPageSource()
					});
					GotoTab(Tabs.Profile);
					Utils.CCKLog("Khong tim thay", "Product Marketplace");
				}
				AddLogByAction($"Thêm thành công {num} link", isAppend: false);
			}
			catch (Exception ex)
			{
				Utils.CCKLog(ex.Message, "AddProduct");
			}
		}

		public void RemoveRedundanceAccount()
		{
			GotoTab(Tabs.Profile);
			string pageSource = GetPageSource();
			if (!pageSource.Contains("\"@") || !pageSource.Contains("\"Edit profile\""))
			{
				return;
			}
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),\"@\")]", m_driver);
			if (webElement == null || !(webElement.Text != ""))
			{
				return;
			}
			webElement.Text.Replace("@", "").Trim();
			List<CCKNode> list = ADBHelperCCK.AppGetObjects("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.TextView", pageSource, p_DeviceId);
			if (list == null || list.Count != 1)
			{
				return;
			}
			if (list[0].Text.Length <= 2)
			{
				ADBHelperCCK.Tap(p_DeviceId, (double)list[0].Location.X + (double)list[0].Size.Width * 0.35, list[0].Location.Y);
			}
			else
			{
				list[0].Click();
				Thread.Sleep(1000);
				ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 2);
			}
			Thread.Sleep(1000);
			pageSource = GetPageSource();
			List<string> list2 = new List<string>();
			for (int i = 0; i < 2; i++)
			{
				List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.view.ViewGroup/android.widget.LinearLayout/android.widget.TextView[1]", m_driver);
				if (list3 != null)
				{
					for (int j = 0; j < list3.Count; j++)
					{
						string text = list3[j].Text.ToLower();
						if (text != "" && text != "add account" && !list2.Contains(text))
						{
							list2.Add(text);
						}
					}
				}
				ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 2);
			}
			List<string> list4 = new List<string>();
			if (sql == null)
			{
				sql = new SQLiteUtils();
			}
			DataTable dataTable = sql.ExecuteQuery($"Select * from Account where device='{p_DeviceId}'");
			if (dataTable != null)
			{
				List<string> list5 = new List<string>();
				foreach (DataRow row in dataTable.Rows)
				{
					string item = row["id"].ToString().ToLower();
					list5.Add(item);
				}
				foreach (string item2 in list2)
				{
					if (!list5.Contains(item2))
					{
						list4.Add(item2);
					}
				}
			}
			foreach (string item3 in list4)
			{
				GotoTab(Tabs.Home);
				Thread.Sleep(1000);
				GotoTab(Tabs.Profile);
				pageSource = GetPageSource();
				list = ADBHelperCCK.AppGetObjects("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.TextView", pageSource, p_DeviceId);
				if (list == null || list.Count != 1)
				{
					continue;
				}
				if (list[0].Text.Length > 2)
				{
					list[0].Click();
					Thread.Sleep(1000);
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 2);
				}
				else
				{
					ADBHelperCCK.Tap(p_DeviceId, (double)list[0].Location.X + (double)list[0].Size.Width * 0.35, list[0].Location.Y);
				}
				pageSource = GetPageSource();
				if (!pageSource.Contains(item3))
				{
					continue;
				}
				webElement = ADBHelperCCK.AppGetObject($"//*[contains(lower-case(@text),\"{item3}\")]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
				}
				else
				{
					ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 2);
					Thread.Sleep(1000);
					ADBHelperCCK.AppGetObject($"//*[contains(lower-case(@text),\"{item3}\")]", m_driver)?.Click();
				}
				Thread.Sleep(1000);
				GotoTab(Tabs.Profile);
				pageSource = GetPageSource();
				if (!pageSource.Contains(item3))
				{
					continue;
				}
				List<IWebElement> list6 = ADBHelperCCK.AppGetObjects("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout/android.widget.ImageView", m_driver);
				if (list6 == null || list6.Count <= 0)
				{
					continue;
				}
				IWebElement webElement2 = list6[list6.Count - 1];
				webElement2.Click();
				Thread.Sleep(1000);
				webElement2 = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
				if (webElement2 == null)
				{
					continue;
				}
				webElement2.Click();
				Thread.Sleep(1000);
				IWebElement webElement3 = ADBHelperCCK.AppFindLogout(By.XPath("//*[@text='Log out']"), m_driver, p_DeviceId);
				if (webElement3 == null)
				{
					continue;
				}
				webElement3.Click();
				Thread.Sleep(1000);
				webElement3 = ADBHelperCCK.AppFindLogout(By.XPath("//*[@text='Log out']"), m_driver, p_DeviceId);
				if (webElement3 != null)
				{
					webElement3.Click();
					Thread.Sleep(1000);
					pageSource = GetPageSource();
					webElement3 = ADBHelperCCK.AppFindLogout(By.XPath("//*[@text='Log out']"), m_driver, p_DeviceId);
					if (webElement3 != null)
					{
						webElement3.Click();
						Thread.Sleep(5000);
						pageSource = GetPageSource();
					}
				}
			}
		}

		public void FollowCheo()
		{
			Utils.LogFunction("FollowCheo", "");
			if (!File.Exists(CaChuaConstant.FOLLOW_CHEO))
			{
				return;
			}
			FollowCheoEntity followCheoEntity = js.Deserialize<FollowCheoEntity>(Utils.ReadTextFile(CaChuaConstant.FOLLOW_CHEO));
			for (int i = 0; i < followCheoEntity.Number; i++)
			{
				string text = "";
				if (followCheoEntity.FollowType != FollowCheoType.File)
				{
					if (followCheoEntity.FollowType == FollowCheoType.Selected)
					{
						text = "";
					}
					else if (followCheoEntity.FollowType == FollowCheoType.Full)
					{
						text = "";
					}
				}
				else
				{
					text = Utils.GetFirstItemFromFile(followCheoEntity.File, remove: false);
				}
				if (!text.StartsWith("https://"))
				{
					text = "https://www.tiktok.com/@" + text.Replace("@", "");
				}
				ADBHelperCCK.OpenLink(p_DeviceId, text);
				Thread.Sleep(3000);
				string pageSource = GetPageSource();
				if (pageSource.Contains("text=\"Follow\""))
				{
					IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Follow\" or @content-desc=\"Follow\"]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000 * followCheoEntity.Delay);
					}
				}
			}
		}

		public void AcceptShop()
		{
			Utils.LogFunction("AcceptShop", "");
			GotoTab(Tabs.Inbox);
			Thread.Sleep(5000);
			string pageSource = GetPageSource();
			if (!pageSource.Contains("text=\"Monetization:"))
			{
				return;
			}
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[contains(@text,\"Monetization:\")]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(2000);
			webElement = ADBHelperCCK.WaitMe("//*[contains(@text,\"New request:\")]", m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(5000);
			webElement = ADBHelperCCK.WaitMe("//*[contains(@text,\"Accept request\")]", m_driver);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(5000);
				webElement = ADBHelperCCK.AppGetObject("//*[contains(@text,\"Link account\")]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(5000);
				}
			}
		}

		public void KiemXu()
		{
			Utils.LogFunction("KiemXu", "");
			string pageSource = GetPageSource();
			string firstItemFromFile = Utils.GetFirstItemFromFile(CaChuaConstant.ShopLink, remove: false);
			if (firstItemFromFile == "")
			{
				return;
			}
			ADBHelperCCK.OpenLink(p_DeviceId, firstItemFromFile);
			pageSource = GetPageSource();
			IWebElement webElement = ADBHelperCCK.WaitMe("//*[@content-desc=\"Videos\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			if (!pageSource.Contains("\"Shop\""))
			{
				int x = webElement.Location.X;
				int y = webElement.Location.Y;
				int width = webElement.Size.Width;
				int height = webElement.Size.Height;
				ADBHelperCCK.Tap(p_DeviceId, x + width * 3 / 2, y + height / 2);
				Thread.Sleep(5000);
			}
			else
			{
				webElement = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Shop\" or @text=\"Shop\"]", m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(5000);
				}
			}
			ADBHelperCCK.WaitMeCount("//com.lynx.tasm.behavior.ui.LynxFlattenUI[string-length(@content-desc) > 0]", m_driver);
			pageSource = GetPageSource();
			List<IWebElement> list = ADBHelperCCK.AppGetObjects("//X.0Go/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/com.lynx.tasm.behavior.ui.view.UIView[string-length(@content-desc) > 0]", m_driver);
			if (list == null)
			{
				list = ADBHelperCCK.AppGetObjects("//com.lynx.tasm.behavior.ui.LynxFlattenUI[string-length(@content-desc) > 0]", m_driver);
			}
			if (list == null || list.Count <= 0)
			{
				return;
			}
			int index = ((list != null && list.Count > 1) ? 1 : 0);
			list[index].Click();
			Thread.Sleep(1000 * Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_CAPTCHA)));
			pageSource = GetPageSource().ToLower();
			if (!pageSource.Contains("new customer") && !pageSource.Contains("flash sale"))
			{
				if (pageSource.Contains("buy now"))
				{
					sql.UpdateNote(Info.Uid, "0");
					return;
				}
				sql.UpdateNote(Info.Uid, "0");
				Thread.Sleep(2000);
				return;
			}
			ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 3 / 5, screen.X / 3, screen.Y / 4, 500);
			pageSource = GetPageSource();
			if (!pageSource.Contains("Coupons"))
			{
				sql.UpdateNote(Info.Uid, "0");
				return;
			}
			Regex regex = new Regex("Get (.*?)₫ off");
			Match match = regex.Match(pageSource);
			if (match != null && match.Success)
			{
				if (sql == null)
				{
					sql = new SQLiteUtils();
				}
				sql.UpdateNote(Info.Uid, match.Groups[1].Value.ToString());
			}
			else if (pageSource.Contains("More coupons"))
			{
				IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"More coupons\" or @content-desc=\"More coupons\"]", m_driver);
				if (webElement2 != null)
				{
					webElement2.Click();
					sql.UpdateNote(Info.Uid, "0");
				}
			}
		}

		public void Promotion()
		{
			Utils.LogFunction("Promotion", "");
			GetPageSource();
			GotoTab(Tabs.Profile);
			GetPageSource();
			IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("36f65f4d6a424d7590639581a5585e80"), m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("e3ba73940c59460cb4ddec72f604dc31"), m_driver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(1000);
			GetPageSource();
			webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("248ae3baaa1f43489d1fce5d63268bba"), m_driver);
			if (webElement == null)
			{
				return;
			}
			GetPageSource();
			IWebElement webElement2 = ADBHelperCCK.WaitMe(GetFunctionByKey("248ae3baaa1f43489d1fce5d63268bba"), m_driver);
			if (webElement2 != null)
			{
				webElement2.Click();
				Thread.Sleep(3000);
				webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("f254fa9f41f043d1b5240adbfa159a88"), m_driver);
				webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("deb6a89af94d4d129adc3321ee44c99c"), m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("f254fa9f41f043d1b5240adbfa159a88"), m_driver);
					if (webElement != null)
					{
						int num = Utils.Convert2Int(webElement.Text.Replace("--", "").Replace("Orders", ""));
						sql.ExecuteQuery($"Update Account set Today='{num}' where id='{Info.Uid}'");
					}
				}
				webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("7b315a7e49a04343811d2f45504b7f84"), m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
					webElement = ADBHelperCCK.WaitMe(GetFunctionByKey("f254fa9f41f043d1b5240adbfa159a88"), m_driver);
					if (webElement != null)
					{
						int num2 = Utils.Convert2Int(webElement.Text.Replace("--", "").Replace("Orders", ""));
						sql.ExecuteQuery($"Update Account set Yesterday='{num2}' where id='{Info.Uid}'");
					}
				}
			}
			ADBHelperCCK.Back(p_DeviceId);
			ADBHelperCCK.Swipe(p_DeviceId, screen.X / 3, screen.Y * 2 / 4, screen.X / 3, screen.Y / 4, 300);
			IWebElement webElement3 = ADBHelperCCK.AppGetObject(GetFunctionByKey("c0a561b5b37d437c97b828e86d98616a"), m_driver);
			if (webElement3 != null)
			{
				webElement3.Click();
				Thread.Sleep(3000);
				webElement3 = ADBHelperCCK.WaitMe(GetFunctionByKey("b2948605ef9142599eea9d90e2513102"), m_driver);
				if (webElement3 != null)
				{
					webElement3.Click();
					Thread.Sleep(3000);
					ADBHelperCCK.WaitMe(GetFunctionByKey("b2948605ef9142599eea9d90e2513102"), m_driver);
				}
				GetPageSource();
				IWebElement webElement4 = ADBHelperCCK.AppGetObject("//android.view.View[@resource-id=\"root\"]/android.view.View/android.view.View/android.widget.TextView", m_driver);
				if (webElement4 != null)
				{
					string text = webElement4.Text;
					sql.ExecuteQuery($"Update Account set banhang='{text.Trim()}' where id='{Info.Uid}'");
				}
			}
			AddLogByAction("Đã kiểm tra xong đơn hàng", isAppend: false);
		}

		public void UpdateProfileInfo()
		{
			Utils.LogFunction("UpdateProfileInfo", "");
			GotoTab(Tabs.Profile);
			string pageSource = GetPageSource();
			if (!File.Exists(CaChuaConstant.UpdateInfo))
			{
				return;
			}
			UpdateInfoField updateInfoField = new JavaScriptSerializer().Deserialize<UpdateInfoField>(Utils.ReadTextFile(CaChuaConstant.UpdateInfo));
			if (updateInfoField != null && updateInfoField.XoaLichSuDangNhap)
			{
				IWebElement webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(1000);
					pageSource = GetPageSource();
					webElement = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000);
						webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Security\"]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(2000);
							webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Manage devices\"]", m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(1000);
								WaitMe("//*[@text=\"Manage devices\"]", 15);
							}
							Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
							ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 2, screenResolution.Y * 3 / 4, screenResolution.X / 2, screenResolution.Y / 4);
							pageSource = GetPageSource();
							List<IWebElement> list = ADBHelperCCK.AppGetObjects("//android.view.View[@resource-id=\"root\"]/android.view.View/android.widget.Image", m_driver);
							while (list != null && list.Count > 0)
							{
								list[0].Click();
								Thread.Sleep(1000);
								pageSource = GetPageSource();
								if (pageSource.Contains("\"Remove\""))
								{
									webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Remove\" or @content-desc=\"Remove\"]", m_driver);
									if (webElement != null)
									{
										webElement.Click();
										Thread.Sleep(5000);
										pageSource = GetPageSource();
										if (pageSource.Contains("Verify Account"))
										{
											string code = emailUti.GetCode(Info.Email, Info.PassEmail);
											if (code != "" && Utils.IsNumber(code))
											{
												List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//android.view.View[@resource-id=\"root\"]/android.view.View/android.view.View/android.view.View[5]/android.view.View", m_driver);
												if (list2 != null && Utils.IsNumber(code))
												{
													char[] array = code.ToCharArray();
													for (int i = 0; i < array.Length; i++)
													{
														IWebElement webElement2 = list2[i];
														ADBHelperCCK.Tap(p_DeviceId, webElement2.Location.X + webElement2.Size.Width / 2, webElement2.Location.Y + webElement2.Size.Height / 2);
														ADBHelperCCK.InputText(p_DeviceId, array[i].ToString());
													}
													Thread.Sleep(5000);
												}
											}
											else
											{
												sql.UpdateNote(Info.Uid, code);
											}
										}
									}
								}
								list = ADBHelperCCK.AppGetObjects("//android.view.View[@resource-id=\"root\"]/android.view.View/android.widget.Image", m_driver);
							}
						}
					}
				}
			}
			if (updateInfoField != null && updateInfoField.PublicInfo)
			{
				PublicInfo();
			}
			GotoTab(Tabs.Profile);
			IWebElement webElement3 = ADBHelperCCK.AppGetObject("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout[2]/android.widget.TextView", m_driver);
			if (webElement3 != null)
			{
				if (string.IsNullOrWhiteSpace(Info.FirstName) && webElement3.Text.Length > 5)
				{
					Info.FirstName = webElement3.Text;
					if (Info.FirstName != "")
					{
						sql.ExecuteQuery(string.Format("Update Account set Name='{0}' where id='@{1}' or id='{1}'", webElement3.Text, Info.Uid));
					}
				}
				IWebElement webElement4 = ADBHelperCCK.AppGetObject("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.view.ViewGroup/android.widget.TextView[1]", m_driver);
				if (webElement4 != null && webElement4.Text != "")
				{
					int num = Utils.Convert2Int(webElement4.Text);
					sql.ExecuteQuery(string.Format("Update Account set friend_count='{0}' where id='@{1}' or id='{1}'", num, Info.Uid));
				}
				IWebElement webElement5 = ADBHelperCCK.AppGetObject("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.widget.LinearLayout[2]/android.widget.TextView[1]", m_driver);
				if (webElement5 != null && webElement5.Text != "")
				{
					int num2 = Utils.Convert2Int(webElement5.Text);
					sql.ExecuteQuery(string.Format("Update Account set likecount='{0}' where id='@{1}' or id='{1}'", num2, Info.Uid));
				}
				if (!Info.Avatar)
				{
					webElement4 = ADBHelperCCK.AppGetObject("//*[@text='Edit profile']", m_driver);
					if (webElement4 != null)
					{
						webElement4.Click();
						pageSource = GetPageSource();
						if (pageSource.Contains("Change photo") && pageSource.Contains("Change video"))
						{
							sql.UpdateAvaInfo(Info.Uid, "Yes");
							ADBHelperCCK.Back(p_DeviceId);
						}
					}
				}
				GotoTab(Tabs.Home);
			}
			GotoTab(Tabs.Profile);
			if (updateInfoField == null || !updateInfoField.LogOut)
			{
				return;
			}
			IWebElement webElement6 = ADBHelperCCK.AppGetObject(GetFunctionByKey("a62e5e77506044af9383e7775c28b49d"), m_driver);
			if (webElement6 == null)
			{
				return;
			}
			webElement6.Click();
			Thread.Sleep(1000);
			pageSource = GetPageSource();
			webElement6 = ADBHelperCCK.AppGetObject(GetFunctionByKey("abca84353df54080b26aa63e60ca3c18"), m_driver);
			if (webElement6 == null)
			{
				return;
			}
			webElement6.Click();
			Thread.Sleep(1000);
			webElement6 = ADBHelperCCK.AppFindAndGetObjectDown("//*[lower-case(@text)=\"log out\"]", m_driver, p_DeviceId, 20);
			if (webElement6 != null)
			{
				webElement6.Click();
				Thread.Sleep(2000);
				webElement6 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"log out\"]", m_driver);
				if (webElement6 != null)
				{
					webElement6.Click();
					Thread.Sleep(5000);
					WaitMe("//*[@text=\"Profile\"]", 15);
				}
				GotoTab(Tabs.Profile);
				pageSource = GetPageSource();
				if (pageSource.Contains("Log into existing account"))
				{
					AddLogByAction("Đăng xuất thành công", isAppend: false);
				}
			}
		}

		internal void SeachKeyword()
		{
			Utils.LogFunction("SeachKeyword", "");
			try
			{
				Utils.CCKLog("Vào 1", "SeachKeyword");
				string vIEW_BY_KEYWORD = CaChuaConstant.VIEW_BY_KEYWORD;
				if (!File.Exists(vIEW_BY_KEYWORD))
				{
					return;
				}
				GotoTab(Tabs.Home);
				ViewByKeyword viewByKeyword = new JavaScriptSerializer().Deserialize<ViewByKeyword>(Utils.ReadTextFile(vIEW_BY_KEYWORD));
				string pageSource = GetPageSource();
				int androidVersionNumber = ADBHelperCCK.GetAndroidVersionNumber(p_DeviceId);
				if (androidVersionNumber <= 8)
				{
					pageSource = GetPageSource();
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(pageSource);
					XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//android.widget.HorizontalScrollView");
					if (xmlNodeList != null && xmlNodeList.Count > 0)
					{
						XmlNode nextSibling = xmlNodeList[0].ParentNode.NextSibling;
						if (nextSibling != null)
						{
							string input = nextSibling.Attributes["bounds"].Value.ToString();
							Regex regex = new Regex("([0-9]+)");
							MatchCollection matchCollection = regex.Matches(input);
							int num = 0;
							int num2 = 0;
							if (matchCollection.Count == 4)
							{
								num = (Utils.Convert2Int(matchCollection[0].Value) + Utils.Convert2Int(matchCollection[2].Value)) / 2;
								num2 = (Utils.Convert2Int(matchCollection[1].Value) + Utils.Convert2Int(matchCollection[3].Value)) / 2;
							}
							ADBHelperCCK.Tap(p_DeviceId, num, num2);
							xmlDocument = null;
						}
					}
					else
					{
						IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text='Discover' or @text='Friends' or @content-desc='Discover' or @content-desc='Friends'] | //android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[1]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView[2]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							GetPageSource();
							Thread.Sleep(1000);
							IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text='Search' or @text='Search' or @content-desc='Search' or @content-desc='Search'] | //android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[1]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView[2]", m_driver);
							if (webElement2 != null)
							{
								webElement2.Click();
								Thread.Sleep(1000);
							}
						}
					}
				}
				else
				{
					Utils.CCKLog("Vào 2", "SeachKeyword");
					pageSource = GetPageSource();
					XmlDocument xmlDocument2 = new XmlDocument();
					xmlDocument2.LoadXml(pageSource);
					XmlNodeList xmlNodeList2 = xmlDocument2.SelectNodes("//android.widget.HorizontalScrollView");
					if (xmlNodeList2 != null && xmlNodeList2.Count > 0)
					{
						Utils.CCKLog("Vào 33", "SeachKeyword");
						XmlNode nextSibling2 = xmlNodeList2[xmlNodeList2.Count - 1].NextSibling;
						if (nextSibling2 == null)
						{
							nextSibling2 = xmlNodeList2[xmlNodeList2.Count - 1].ParentNode.NextSibling;
						}
						if (nextSibling2 != null)
						{
							Utils.CCKLog("Vào 33a", "SeachKeyword");
							string input2 = nextSibling2.Attributes["bounds"].Value.ToString();
							Regex regex2 = new Regex("([0-9]+)");
							MatchCollection matchCollection2 = regex2.Matches(input2);
							int num3 = 0;
							int num4 = 0;
							if (matchCollection2.Count == 4)
							{
								num3 = (Utils.Convert2Int(matchCollection2[0].Value) + Utils.Convert2Int(matchCollection2[2].Value)) / 2;
								num4 = (Utils.Convert2Int(matchCollection2[1].Value) + Utils.Convert2Int(matchCollection2[3].Value)) / 2;
							}
							ADBHelperCCK.Tap(p_DeviceId, num3, num4);
						}
						else
						{
							Utils.CCKLog("Vào 33b", "SeachKeyword");
						}
						xmlDocument2 = null;
					}
					else
					{
						Utils.CCKLog("Vào 44", "SeachKeyword");
						CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
						CCKNode cCKNode = cCKDriver.FindAndWait("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[2]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView[2] | //android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[2]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView[2]");
						if (cCKNode == null)
						{
							IWebElement webElement3 = ADBHelperCCK.WaitMeCount("//android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[2]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView[2] | //android.widget.TabHost/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[2]/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ImageView[2]", m_driver);
							if (webElement3 != null)
							{
								webElement3.Click();
								Thread.Sleep(1000);
							}
						}
						else
						{
							cCKNode.Click();
							Thread.Sleep(1000);
						}
					}
				}
				if (viewByKeyword.Keyword != "")
				{
					Utils.CCKLog("Vào 55", "SeachKeyword");
					pageSource = GetPageSource();
					IWebElement webElement4 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
					if (webElement4 != null)
					{
						Utils.CCKLog("Vào 55a", "SeachKeyword");
						webElement4.Click();
						webElement4.Clear();
						List<string> list = viewByKeyword.Keyword.Split("|;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
						string text = list[rnd.Next(list.Count)].Trim();
						webElement4.SendKeys(text);
						Thread.Sleep(1000 * viewByKeyword.SuggestionDelay);
						pageSource = GetPageSource();
						if (viewByKeyword.Type == ViewByKeyword_Type.Shop)
						{
							IWebElement webElement5 = ADBHelperCCK.AppGetObject("//*[@text=\"Search\"]", m_driver);
							if (webElement5 != null)
							{
								webElement5.Click();
								Thread.Sleep(5000);
								webElement5 = ADBHelperCCK.AppGetObject("//*[@text=\"Users\"]", m_driver);
								if (webElement5 != null)
								{
									try
									{
										webElement5.Click();
										Thread.Sleep(2000);
										ADBHelperCCK.WaitMe("//*[contains(@text,\"" + text + "\")]", m_driver);
										pageSource = GetPageSource();
										if (pageSource.Contains("\"" + text + "\""))
										{
											List<IWebElement> list2 = ADBHelperCCK.AppGetObjects("//*[contains(lower-case(@text),\"" + text.ToLower() + "\")]", m_driver);
											if (list2 != null && list2.Count > 1)
											{
												list2[1].Click();
												Thread.Sleep(5000);
												if (viewByKeyword.FollowShop)
												{
													pageSource = GetPageSource();
													if (pageSource.Contains("\"Follow\""))
													{
														IWebElement webElement6 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Follow\" or @text=\"Follow\"]", m_driver);
														if (webElement6 != null)
														{
															webElement6.Click();
															Thread.Sleep(2000);
														}
													}
												}
												DateTime dateTime = DateTime.Now.AddSeconds(rnd.Next(viewByKeyword.ViewVideoFrom, viewByKeyword.ViewVideoTo));
												Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
												bool flag = false;
												while (DateTime.Now < dateTime)
												{
													ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4, 1000);
													Thread.Sleep(1000);
													pageSource = GetPageSource();
													List<IWebElement> list3 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout/android.widget.ImageView", m_driver);
													if (list3 != null && list3.Count > 0)
													{
														flag = true;
														list3[0].Click();
														Thread.Sleep(2000);
													}
													if (!flag)
													{
														continue;
													}
													int num5 = rnd.Next(viewByKeyword.FollowFrom, viewByKeyword.FollowTo);
													for (int i = 0; i < num5; i++)
													{
														pageSource = GetPageSource();
														if (viewByKeyword.IsTym)
														{
															ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format("shell \"input tap {0} {1} & sleep 0.2; input tap {0} {1} & sleep 0.2; input tap {0} {1} & sleep 0.2; input tap {0} {1}\"", screenResolution.X / 3, screenResolution.Y / 3));
															Thread.Sleep(1000 * viewByKeyword.FollowDelay);
															if (viewByKeyword.Repost)
															{
																pageSource = GetPageSource();
																if (pageSource.Contains("\"Repost\""))
																{
																	IWebElement webElement7 = ADBHelperCCK.AppGetObject("//*[@text=\"Repost\"]", m_driver);
																	if (webElement7 != null)
																	{
																		webElement7.Click();
																		Thread.Sleep(1000);
																	}
																}
															}
														}
														string firstItemFromFile = Utils.GetFirstItemFromFile(CaChuaConstant.VIEW_BY_KEYWORD_COMMENT, viewByKeyword.RemoveComment);
														if (firstItemFromFile != "")
														{
															IWebElement webElement8 = ADBHelperCCK.AppGetObject("//android.widget.EditText[contains(lower-case(@text),\"add comment\")]", m_driver);
															if (webElement8 != null && firstItemFromFile != "")
															{
																webElement8.Click();
																Thread.Sleep(1000);
																if (viewByKeyword.Repost)
																{
																	pageSource = GetPageSource();
																	if (pageSource.Contains("\"Repost\""))
																	{
																		webElement8 = ADBHelperCCK.AppGetObject("//*[@text=\"Repost\"]", m_driver);
																		if (webElement8 != null)
																		{
																			webElement8.Click();
																			Thread.Sleep(1000);
																		}
																	}
																}
																webElement8 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
																if (firstItemFromFile.Contains("@"))
																{
																	try
																	{
																		string[] array = firstItemFromFile.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
																		int num6 = -1;
																		for (int j = 0; j < array.Length; j++)
																		{
																			if (array[j].Contains("@"))
																			{
																				num6 = j;
																				break;
																			}
																		}
																		string text2 = ((num6 > 0) ? string.Join(" ", array, 0, num6).ToString() : "");
																		string text3 = array[num6];
																		string text4 = ((num6 < 0 || num6 >= array.Length - 1) ? "" : string.Join(" ", array, num6 + 1, array.Length - num6 - 1).ToString());
																		webElement8.Click();
																		if (text2.Trim() != "")
																		{
																			ADBHelperCCK.InputUnicode(p_DeviceId, Utils.Spin(text2.Trim()) + " ");
																		}
																		if (text3.Trim() != "")
																		{
																			ADBHelperCCK.InputUnicode(p_DeviceId, text3.Trim());
																			Thread.Sleep(5000);
																			IWebElement webElement9 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='" + text3.ToLower().Trim().Replace("@", "") + "']", m_driver);
																			if (webElement9 != null)
																			{
																				webElement9.Click();
																				Thread.Sleep(1000);
																			}
																		}
																		if (text4.Trim() != "")
																		{
																			ADBHelperCCK.InputUnicode(p_DeviceId, Utils.Spin(text4.Trim()));
																		}
																	}
																	catch
																	{
																		webElement8.SendKeys(Utils.Spin(firstItemFromFile));
																	}
																}
																else
																{
																	webElement8.SendKeys(Utils.Spin(firstItemFromFile));
																}
																Thread.Sleep(2000);
																IWebElement webElement10 = ADBHelperCCK.AppGetObject("//android.widget.ImageView[@content-desc=\"Post comment\"]", m_driver);
																if (webElement10 != null)
																{
																	webElement10.Click();
																	Thread.Sleep(2000);
																}
																Thread.Sleep(1000 * viewByKeyword.CommentDelay);
															}
														}
														if (viewByKeyword.IsFavorite)
														{
															IWebElement webElement11 = ADBHelperCCK.AppGetObject("//android.widget.Button[@content-desc=\"Add or remove this video from Favorites.\"]/android.widget.FrameLayout/android.widget.ImageView", m_driver);
															if (webElement11 != null)
															{
																ADBHelperCCK.Tap(p_DeviceId, webElement11.Location.X + webElement11.Size.Width / 2, webElement11.Location.Y + webElement11.Size.Height / 2);
																Thread.Sleep(1000);
																GetPageSource();
															}
														}
														ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 3, screenResolution.Y * 3 / 5, screenResolution.X / 3, screenResolution.Y / 4, 200);
													}
												}
											}
										}
										else
										{
											Utils.CCKLog("View By Shop - Khong tim thay", "");
										}
									}
									catch (Exception ex)
									{
										Utils.CCKLog("View By Shop - Khong tim thay", ex.Message);
									}
								}
							}
							webElement5 = null;
						}
						else if (viewByKeyword.Type == ViewByKeyword_Type.Keyword)
						{
							Utils.CCKLog("Vào 66", "SeachKeyword");
							List<IWebElement> list4 = ADBHelperCCK.AppGetObjects(GetFunctionByKey("33ad278c36184d3e8035ce787eae9889"), m_driver);
							if (list4 != null && list4.Count > 0)
							{
								list4[rnd.Next(list4.Count / 2)].Click();
								Thread.Sleep(2000);
								pageSource = GetPageSource();
								IWebElement webElement12 = ADBHelperCCK.WaitMe("//*[@text=\"Top\"]", m_driver);
								if (webElement12 != null)
								{
									webElement12.Click();
									DateTime dateTime2 = DateTime.Now.AddSeconds(rnd.Next(viewByKeyword.ViewVideoFrom, viewByKeyword.ViewVideoTo));
									Point screenResolution2 = ADBHelperCCK.GetScreenResolution(p_DeviceId);
									bool flag2 = false;
									while (DateTime.Now < dateTime2)
									{
										try
										{
											Utils.CCKLog("Vào 88", "SeachKeyword");
											ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 3, screenResolution2.Y / 2, screenResolution2.X / 3, screenResolution2.Y / 4, 200);
											Thread.Sleep(1000);
											pageSource = GetPageSource();
											List<IWebElement> list5 = ADBHelperCCK.AppGetObjects("//androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup", m_driver);
											if (list5 != null && list5.Count > 0)
											{
												flag2 = true;
												list5[rnd.Next(list5.Count)].Click();
												Thread.Sleep(2000);
												if (viewByKeyword.FollowShop)
												{
													pageSource = GetPageSource();
													if (pageSource.Contains("\"Follow\""))
													{
														IWebElement webElement13 = ADBHelperCCK.AppGetObject("//*[contains(@content-desc,\"Follow\") or contains(@text,\"Follow\")]", m_driver);
														if (webElement13 != null)
														{
															webElement13.Click();
															Thread.Sleep(2000);
														}
													}
												}
											}
											if (!flag2)
											{
												continue;
											}
											int num7 = rnd.Next(viewByKeyword.FollowFrom, viewByKeyword.FollowTo);
											for (int k = 0; k < num7; k++)
											{
												ADBHelperCCK.Swipe(p_DeviceId, screenResolution2.X / 3, screenResolution2.Y / 2, screenResolution2.X / 3, screenResolution2.Y / 4);
												GetPageSource();
												if (pageSource.Contains("\"Follow\""))
												{
													IWebElement webElement14 = ADBHelperCCK.AppGetObject("//*[contains(@content-desc,\"Follow\") or contains(@text,\"Follow\")]", m_driver);
													if (webElement14 != null)
													{
														webElement14.Click();
														Thread.Sleep(1000 * viewByKeyword.FollowDelay);
													}
												}
												if (viewByKeyword.IsTym)
												{
													ADBHelperCCK.ExecuteCMD(p_DeviceId, string.Format("shell \"input tap {0} {1} & sleep 0.2; input tap {0} {1} & sleep 0.2; input tap {0} {1} & sleep 0.2; input tap {0} {1}\"", screenResolution2.X / 3, screenResolution2.Y / 3));
												}
												Thread.Sleep(1000);
												string firstItemFromFile2 = Utils.GetFirstItemFromFile(CaChuaConstant.VIEW_BY_KEYWORD_COMMENT, viewByKeyword.RemoveComment);
												if (firstItemFromFile2 != "")
												{
													ADBHelperCCK.AppGetObject("//android.widget.EditText[contains(lower-case(@text),\"add comment\")]", m_driver)?.Click();
													IWebElement webElement15 = ADBHelperCCK.AppGetObject(GetFunctionByKey("43c48fc80271444c928e22f4bc89b3e8"), m_driver);
													if (webElement15 != null)
													{
														if (firstItemFromFile2.Contains("@"))
														{
															try
															{
																string[] array2 = firstItemFromFile2.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
																int num8 = -1;
																for (int l = 0; l < array2.Length; l++)
																{
																	if (array2[l].Contains("@"))
																	{
																		num8 = l;
																		break;
																	}
																}
																string text5 = ((num8 > 0) ? string.Join(" ", array2, 0, num8).ToString() : "");
																string text6 = array2[num8];
																string text7 = ((num8 < 0 || num8 >= array2.Length - 1) ? "" : string.Join(" ", array2, num8 + 1, array2.Length - num8 - 1).ToString());
																webElement15.Click();
																if (text5.Trim() != "")
																{
																	ADBHelperCCK.InputUnicode(p_DeviceId, Utils.Spin(text5.Trim()) + " ");
																}
																if (text6.Trim() != "")
																{
																	ADBHelperCCK.InputUnicode(p_DeviceId, text6.Trim());
																	Thread.Sleep(5000);
																	IWebElement webElement16 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)='" + text6.ToLower().Trim().Replace("@", "") + "']", m_driver);
																	if (webElement16 != null)
																	{
																		webElement16.Click();
																		Thread.Sleep(1000);
																	}
																}
																if (text7.Trim() != "")
																{
																	ADBHelperCCK.InputUnicode(p_DeviceId, Utils.Spin(text7.Trim()));
																}
															}
															catch
															{
																webElement15.SendKeys(Utils.Spin(firstItemFromFile2));
															}
														}
														else
														{
															webElement15.SendKeys(Utils.Spin(firstItemFromFile2));
														}
													}
													Thread.Sleep(1000);
													IWebElement webElement17 = ADBHelperCCK.AppGetObject("//android.widget.ImageView[@content-desc=\"Post comment\"]", m_driver);
													if (webElement17 != null)
													{
														webElement17.Click();
														Thread.Sleep(2000);
													}
													Thread.Sleep(1000 * viewByKeyword.CommentDelay);
												}
												if (viewByKeyword.IsFavorite)
												{
													IWebElement webElement18 = ADBHelperCCK.AppGetObject("//android.widget.Button[@content-desc=\"Add or remove this video from Favorites.\"]/android.widget.FrameLayout/android.widget.ImageView", m_driver);
													if (webElement18 != null)
													{
														ADBHelperCCK.Tap(p_DeviceId, webElement18.Location.X + webElement18.Size.Width / 2, webElement18.Location.Y + webElement18.Size.Height / 2);
														Thread.Sleep(1000);
														GetPageSource();
													}
												}
											}
										}
										catch (Exception ex2)
										{
											Utils.CCKLog("SeachKeyword", ex2.Message);
										}
									}
								}
							}
							else
							{
								Utils.CCKLog("Vào 77", "SeachKeyword");
							}
						}
						else if (viewByKeyword.Type == ViewByKeyword_Type.User)
						{
							IWebElement webElement19 = ADBHelperCCK.AppGetObject("//*[@text=\"Search\"]", m_driver);
							if (webElement19 != null)
							{
								webElement19.Click();
								Thread.Sleep(2000);
								pageSource = GetPageSource();
								IWebElement webElement20 = ADBHelperCCK.WaitMe("//*[@text=\"Users\"]", m_driver);
								if (webElement20 != null)
								{
									webElement20.Click();
									Thread.Sleep(5000);
									IWebElement webElement21 = ADBHelperCCK.AppGetObject("//*[@text=\"Follow\" or @content-desc=\"Follow\"]", m_driver);
									if (webElement21 != null)
									{
										webElement21.Click();
										Thread.Sleep(1000);
									}
								}
							}
						}
					}
					else
					{
						Utils.CCKLog("Vào 55b", "SeachKeyword");
					}
				}
				Utils.CCKLog("Vào 5", "SeachKeyword");
			}
			catch (Exception ex3)
			{
				Utils.CCKLog(ex3.Message, "SeachKeyword");
			}
		}
	}
}
