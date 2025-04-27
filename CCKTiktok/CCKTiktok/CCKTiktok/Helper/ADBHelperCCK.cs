using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using CCKTiktok.BO;
using CCKTiktok.Bussiness;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace CCKTiktok.Helper
{
	public class ADBHelperCCK
	{
		public const string API_FILE_TEXT = "file_{0}.txt";

		public const string API_FILE_TEXT_RET = "ret_file_{0}.txt";

		private static readonly object locker = new object();

		public static List<Task> lstPhone = new List<Task>();

		private static bool showADBLog = false;

		public static List<string> lstPort = new List<string>();

		private static bool KillPort;

		public static string KEYCODE_HOME = "3";

		public static string KEYCODE_BACK = "4";

		public static string KEYCODE_CLEAR = "28";

		public static string KEYCODE_MOVE_END = "123";

		public static string KEYCODE_DEL = "67";

		public static string KEYCODE_0 = "7";

		public static string KEYCODE_1 = "8";

		public static string KEYCODE_2 = "9";

		public static string KEYCODE_3 = "10";

		public static string KEYCODE_4 = "11";

		public static string KEYCODE_5 = "12";

		public static string KEYCODE_6 = "13";

		public static string KEYCODE_7 = "14";

		public static string KEYCODE_8 = "15";

		public static string KEYCODE_9 = "16";

		public static string KEYCODE_TAB = "61";

		public static string KEYCODE_APP_SWITCH = "KEYCODE_APP_SWITCH";

		public static List<ProxyServer> lstStaticProxy = new List<ProxyServer>();

		public static string GetLocalIp()
		{
			string result = string.Empty;
			string machineName = Environment.MachineName;
			IPHostEntry hostEntry = Dns.GetHostEntry(machineName);
			IPAddress[] addressList = hostEntry.AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					result = Convert.ToString(iPAddress);
				}
			}
			return result;
		}

		public static void ForwardProxy(string proxy, int portForward)
		{
			string[] array = proxy.Split(':');
			ProxyServer proxyServer = new ProxyServer();
			proxyServer.TcpTimeWaitSeconds = 10;
			proxyServer.ConnectionTimeOutSeconds = 15;
			proxyServer.UpStreamHttpProxy = new ExternalProxy
			{
				HostName = "127.0.25.3",
				Port = 2200,
				ProxyType = ExternalProxyType.Http
			};
			proxyServer.UpStreamHttpsProxy = new ExternalProxy
			{
				HostName = "127.0.25.3",
				Port = 2200,
				ProxyType = ExternalProxyType.Http
			};
			SocksProxyEndPoint endPoint = new SocksProxyEndPoint(IPAddress.Any, Convert.ToInt32(portForward), decryptSsl: false);
			proxyServer.CertificateManager.CreateRootCertificate();
			proxyServer.CertificateManager.TrustRootCertificate(machineTrusted: true);
			proxyServer.CertificateManager.TrustRootCertificateAsAdmin(machineTrusted: true);
			proxyServer.AddEndPoint(endPoint);
			proxyServer.UpStreamHttpProxy = new ExternalProxy
			{
				HostName = array[0],
				Port = int.Parse(array[1]),
				UserName = array[2],
				Password = array[3],
				ProxyType = ExternalProxyType.Http
			};
			proxyServer.Start();
			if (!lstStaticProxy.Contains(proxyServer))
			{
				lstStaticProxy.Add(proxyServer);
			}
			Thread.Sleep(50);
		}

		public static bool CheckLiveProxy(string proxyAddress = "192.168.1.28:4001")
		{
			string[] array = proxyAddress.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array == null && array.Length < 2)
			{
				return false;
			}
			WebRequest webRequest = WebRequest.Create("http://cck.vn");
			webRequest.Proxy = new WebProxy(array[0] + ":" + array[1]);
			if (array != null && array.Length == 4)
			{
				webRequest.Proxy.Credentials = new NetworkCredential(array[2], array[3]);
			}
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					return true;
				}
				return false;
			}
			catch (WebException)
			{
				return false;
			}
		}

		private static string GetIpAddress(string input)
		{
			if (IPAddress.TryParse(input, out var address))
			{
				return address.ToString();
			}
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(input);
				return hostAddresses[0].ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error resolving domain to IP: " + ex.Message);
				return null;
			}
		}

		public static bool SetProxyNoRootClient(string deviceId, string proxy, RemoteWebDriver driver = null)
		{
			bool flag = Utils.ReadTextFile(CaChuaConstant.PROXY_TYPE) == "sock5";
			string text = "com.cryptoproxy.proxyclient";
			ClearAppData(deviceId, text);
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
				ExecuteCMD(deviceId, empty);
				if (array.Length == 2)
				{
					empty2 = array[0].Trim();
					empty3 = array[1].Trim();
					empty = string.Concat(new string[4] { "shell settings put global http_proxy ", empty2, ":", empty3 });
					ExecuteCMD(deviceId, empty);
					return true;
				}
				if (array.Length == 4)
				{
					string ipAddress = GetIpAddress(array[0].Trim());
					string text2 = array[1].Trim();
					string text3 = array[2].Trim();
					string text4 = array[3].Trim();
					if (!IsInstallApp(deviceId, text))
					{
						string text5 = "ProxyClientNoRoot.apk";
						if (!File.Exists(Application.StartupPath + "\\Devices\\" + text5))
						{
							try
							{
								new WebClient().DownloadFile($"https://cck.vn/Download/Utils/{text5}.rar", Application.StartupPath + "\\Devices\\" + text5);
							}
							catch (Exception)
							{
							}
						}
						InstallApp(deviceId, Application.StartupPath + "\\Devices\\" + text5);
						Thread.Sleep(2000);
					}
					ExecuteCMD(deviceId, string.Format("shell pm clear " + text));
					OpenApp(deviceId, text);
					Thread.Sleep(5000);
					IWebElement webElement = WaitMe("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/own_proxy_btn\"]", driver);
					if (webElement != null && driver != null)
					{
						string pageSource = GetPageSource(driver, null);
						if (pageSource.Contains("com.cryptoproxy.proxyclient:id/own_proxy_btn"))
						{
							IWebElement webElement2 = AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/own_proxy_btn\"]"), driver);
							if (webElement2 != null)
							{
								webElement2.Click();
								Thread.Sleep(1000);
							}
						}
						if (GetPageSource(driver, null).Contains("android.widget.EditText"))
						{
							IWebElement webElement3 = AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/enable_auth_box\"]"), driver);
							if (webElement3 != null)
							{
								webElement3.Click();
								Thread.Sleep(500);
							}
							pageSource = GetPageSource(driver, null);
							IWebElement webElement4 = AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/proxy_type_edit_btn\"]"), driver);
							if (webElement4 != null)
							{
								webElement4.Click();
								Thread.Sleep(500);
								if (!flag)
								{
									webElement4 = AppGetObject(By.XPath("//*[@text=\"HTTP_TUNNEL\"]"), driver);
									if (webElement4 != null)
									{
										webElement4.Click();
										Thread.Sleep(500);
									}
								}
								else
								{
									webElement4 = AppGetObject(By.XPath("//*[@text=\"SOCKS5\"]"), driver);
									if (webElement4 != null)
									{
										webElement4.Click();
										Thread.Sleep(500);
									}
								}
							}
							List<IWebElement> list2 = AppGetObjects(By.XPath("//android.widget.EditText"), driver);
							if (list2 != null && list2.Count == 4)
							{
								list2[0].SendKeys(ipAddress);
								Thread.Sleep(500);
								AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/proxy_ip_save_btn\"]"), driver)?.Click();
								Thread.Sleep(500);
							}
							list2 = AppGetObjects(By.XPath("//android.widget.EditText"), driver);
							if (list2 != null && list2.Count == 4)
							{
								list2[1].SendKeys(text2);
								Thread.Sleep(500);
								AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/proxy_port_save_btn\"]"), driver)?.Click();
								Thread.Sleep(500);
							}
							Point screenResolution = GetScreenResolution(deviceId);
							Swipe(deviceId, screenResolution.X / 2, screenResolution.Y / 4, screenResolution.X / 2, screenResolution.Y / 5);
							list2 = AppGetObjects(By.XPath("//android.widget.EditText"), driver);
							if (list2 != null && list2.Count == 4)
							{
								list2[2].SendKeys(text3);
								Thread.Sleep(500);
								AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/username_save_btn\"]"), driver)?.Click();
								Thread.Sleep(500);
							}
							list2 = AppGetObjects(By.XPath("//android.widget.EditText"), driver);
							if (list2 != null && list2.Count == 4)
							{
								list2[3].SendKeys(text4);
								Thread.Sleep(500);
								AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/password_save_btn\"]"), driver)?.Click();
								Thread.Sleep(500);
							}
							Swipe(deviceId, screenResolution.X / 2, screenResolution.Y / 5, screenResolution.X / 2, screenResolution.Y / 4);
							webElement3 = AppGetObject(By.XPath("//*[@resource-id=\"com.cryptoproxy.proxyclient:id/proxy_enabled_btn\"]"), driver);
							if (webElement3 != null)
							{
								webElement3.Click();
								Thread.Sleep(2000);
							}
							if (GetPageSource(driver, null).Contains("text=\"OK\""))
							{
								webElement3 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
								if (webElement3 != null)
								{
									webElement3.Click();
									Thread.Sleep(2000);
								}
							}
							BackToHome(deviceId);
							GetPageSource(driver, null);
							return true;
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Utils.CCKLog("SetProxy", ex2.Message + " - proxy " + proxy);
			}
			BackToHome(deviceId);
			return result;
		}

		public static bool Reboot(string deviceId, bool recovery = false, int delay = 5)
		{
			if (!recovery)
			{
				ExecuteCMD(deviceId, "reboot");
			}
			else
			{
				ExecuteCMD(deviceId, "reboot recovery");
			}
			int num = 0;
			while (num++ < 60)
			{
				string text = ExecuteCMD(deviceId, " devices");
				if (text.Contains(deviceId))
				{
					string[] array = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (!text2.Contains(deviceId) || !text2.Contains("recovery"))
						{
							if (text2.Contains(deviceId) && text2.Contains("device"))
							{
								ExecuteCMD(deviceId, " shell rm -rf /sdcard/Pictures/");
								ExecuteCMD(deviceId, " shell rm -rf /sdcard/Albums/");
								ExecuteCMD(deviceId, " shell rm -rf /sdcard/DCIM/");
								Thread.Sleep(1000 * delay);
								return true;
							}
							continue;
						}
						ExecuteCMD(deviceId, " shell rm -rf /sdcard/Pictures/");
						ExecuteCMD(deviceId, " shell rm -rf /sdcard/Albums/");
						ExecuteCMD(deviceId, " shell rm -rf /sdcard/DCIM/");
						Thread.Sleep(1000 * delay);
						return true;
					}
				}
				Thread.Sleep(1000 * delay);
			}
			return false;
		}

		public static string NormalizeDeviceName(string SerialNo)
		{
			return SerialNo.Replace(".", "").Replace(":", "");
		}

		internal static void SendToPhoneClipboard(string SerialNo, string msg)
		{
			ExecuteCMD(SerialNo, $"shell am start -n \"com.cck.support/.ToastActivity\"  --es copy '{msg}'");
		}

		public static bool StartServiceFake(string SerialNo)
		{
			string cmd = "shell am broadcast -a com.cck.support.CHANGE_DEVICE -n com.cck.support/com.cck.fake.CCKController";
			string text = ExecuteCMD(SerialNo, cmd);
			return text.Contains("Change Success!");
		}

		public static string GetBatteryInfo(string Serialno)
		{
			return ExecuteCMD(Serialno, "shell dumpsys battery");
		}

		public static void InputKeyEvent(string SerialNo, string key)
		{
			ExecuteCMD(SerialNo, "shell input keyevent \"" + key + "\"");
		}

		public static void CloseRecentApp(string SerialNo, RemoteWebDriver driver)
		{
			string value = "No recent items";
			for (int i = 0; i < 5; i++)
			{
				ExecuteCMD(SerialNo, "shell input keyevent KEYCODE_APP_SWITCH");
				Thread.Sleep(500);
				string pageSource = driver.PageSource;
				if (!pageSource.Contains(value))
				{
					ExecuteCMD(SerialNo, "shell input keyevent 20");
					Thread.Sleep(200);
					ExecuteCMD(SerialNo, "shell input keyevent DEL");
					Thread.Sleep(200);
					continue;
				}
				break;
			}
		}

		public static string RanString(int length = 10, bool so = true)
		{
			Random random = new Random();
			string text = "";
			string text2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			if (so)
			{
				text2 += "0123456789";
			}
			for (int i = 0; i < length; i++)
			{
				text += text2[random.Next(text2.Length)];
			}
			return text;
		}

		public static void InputUnicode(string SerialNo, string textpull, bool slowInput = true)
		{
			new Random();
			if (!slowInput)
			{
				ExecuteCMD(SerialNo, "shell am broadcast -a ADB_INPUT_B64 --es msg \"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(textpull)) + "\"");
				return;
			}
			char[] array = textpull.ToCharArray();
			char[] array2 = array;
			foreach (char c in array2)
			{
				ExecuteCMD(SerialNo, "shell am broadcast -a ADB_INPUT_B64 --es msg \"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(c.ToString())) + "\"");
			}
		}

		public static bool AddContactsForDevices(string deviceId, string fileName)
		{
			AddAndroidPermission(deviceId, new List<string> { "com.android.contacts", "com.android.providers.contacts", "android.permission.READ_EXTERNAL_STORAGE" });
			bool result = false;
			try
			{
				string text = ExecuteCMD(deviceId, $" shell am start -a \"android.intent.action.VIEW\" -t \"text/vcard\" -d \"file:///sdcard/Download/{fileName}\"");
				if (text.Contains("Starting: Intent"))
				{
					result = true;
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool AddAndroidPermission(string deviceId, List<string> package)
		{
			new Task(delegate
			{
				try
				{
					foreach (string item in package)
					{
						string cmd = string.Concat(new string[3]
						{
							" shell pm grant ",
							CaChuaConstant.PACKAGE_NAME,
							" " + item
						});
						ExecuteCMD(deviceId, cmd);
					}
				}
				catch (Exception)
				{
				}
			}).Start();
			return false;
		}

		public static int Connect2Wifi(string SerialNo, string userName, string passWord, string type = "WPA")
		{
			int result = 0;
			try
			{
				string cmd = "adb -s " + SerialNo + " shell settings put global http_proxy :0";
				ExecuteCMD(SerialNo, cmd);
				if (IsInstallApp(SerialNo, "com.steinwurf.adbjoinwifi"))
				{
					CloseApp(SerialNo, "com.steinwurf.adbjoinwifi");
					Thread.Sleep(750);
					string cmd2 = "adb -s " + SerialNo + " shell am start -n com.steinwurf.adbjoinwifi/.MainActivity -e ssid '" + userName + "' -e password_type " + type + " -e password '" + passWord + "'";
					ExecuteCMD(SerialNo, cmd2);
					result = 1;
					Thread.Sleep(500);
				}
				else
				{
					result = -1;
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static bool Wait(IWebDriver p_driver, string msg, int trycount)
		{
			int num = 0;
			while (num < trycount && !p_driver.PageSource.ToLower().Contains(msg.ToLower()))
			{
				num++;
				Thread.Sleep(1000);
			}
			return p_driver.PageSource.ToLower().Contains(msg.ToLower());
		}

		public static bool SetScreenTimeOut(string deviceId, int timeout = 600000)
		{
			bool result = false;
			try
			{
				ExecuteCMD(deviceId, " shell settings put system screen_off_timeout " + timeout);
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool IsScreenTurnOn(string deviceId)
		{
			bool result = false;
			try
			{
				string cmd = " shell dumpsys input_method";
				string text = ExecuteCMD(deviceId, cmd);
				if (!text.Contains("mInteractive=false") && !text.Contains("mScreenOn=false"))
				{
					if (!string.IsNullOrEmpty(text))
					{
						result = true;
						return result;
					}
					return result;
				}
				result = false;
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool TurnOnScreen(string deviceId)
		{
			bool result = false;
			try
			{
				string cmd = " shell \"dumpsys activity activities | grep mResumedActivity\"";
				string text = ExecuteCMD(deviceId, cmd);
				if (text.Contains("app.launcher") || text.Contains("android.MtpApplication"))
				{
					SENDKEY(deviceId, "3");
				}
				if (!IsScreenTurnOn(deviceId))
				{
					SENDKEY(deviceId, "26");
				}
				SetScreenTimeOut(deviceId, 300000);
				if (IsScreenTurnOn(deviceId))
				{
					result = true;
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static string GetSystemVaraiable(string name)
		{
			return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
		}

		public static void SetSystemVaraiable(string name, string value)
		{
			try
			{
				Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Machine);
			}
			catch
			{
			}
		}

		public static void ChangeLanguage2English(string deviceId, bool lang)
		{
			ExecuteCMD(deviceId, "adb shell am start -a android.settings.LOCALE_SETTINGS");
		}

		public static void InitPhoneInfo(string deviceId, int timeout = 60000, int brightness = 100)
		{
			ExecuteCMD(deviceId, "shell content insert --uri content://settings/system --bind name:s:accelerometer_rotation --bind value:i:0");
			Thread.Sleep(100);
			ExecuteCMD(deviceId, "shell settings put system screen_off_timeout " + timeout);
			Thread.Sleep(100);
			ExecuteCMD(deviceId, "shell settings put system screen_brightness " + brightness);
		}

		public static bool SetProxy(string deviceId, string proxy, RemoteWebDriver p_driver, DeviceEntity entity = null)
		{
			if (proxy != "" && proxy.Length > 5)
			{
				ShowTextMessageOnPhone(deviceId, "Proxy:" + proxy);
			}
			bool result = false;
			try
			{
				string empty = string.Empty;
				string[] array = proxy.Split(':');
				string empty2 = string.Empty;
				string empty3 = string.Empty;
				empty = " shell settings put global http_proxy :0";
				ExecuteCMD(deviceId, empty);
				if (array.Length == 2)
				{
					empty2 = array[0].Trim();
					empty3 = array[1].Trim();
					empty = string.Concat(new string[4] { "shell settings put global http_proxy ", empty2, ":", empty3 });
					ExecuteCMD(deviceId, empty);
					return true;
				}
				if (array.Length > 2)
				{
					string text = array[0].Trim();
					string text2 = array[1].Trim();
					string text3 = ((array.Length == 4) ? array[2].Trim() : "");
					string text4 = ((array.Length == 4) ? array[3].Trim() : "");
					if (!IsInstallApp(deviceId, "com.cell47.College_Proxy"))
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
								Utils.CCKLog("TaskList", ex.Message, "SetupPhone");
							}
						}
						InstallApp(deviceId, Application.StartupPath + "\\Devices\\" + text5);
						Thread.Sleep(2000);
					}
					while (true)
					{
						StopApp(deviceId, "com.cell47.College_Proxy");
						ExecuteCMD(deviceId, "shell pm clear com.cell47.College_Proxy");
						BackToHome(deviceId);
						OpenApp(deviceId, "com.cell47.College_Proxy");
						Thread.Sleep(2000);
						WaitMeXPath(By.XPath("//android.widget.EditText"), p_driver);
						if (p_driver == null)
						{
							break;
						}
						ShowTextMessageOnPhone(deviceId, "Proxy - " + text);
						Thread.Sleep(500);
						List<IWebElement> list = AppGetObjects(By.XPath("//android.widget.EditText"), p_driver);
						if (list != null && list.Count >= 4)
						{
							Regex regex = new Regex("([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+)");
							if (!regex.Match(text).Success)
							{
								string input = RunCommand("ping " + text);
								Match match = regex.Match(input);
								if (match.Success)
								{
									text = match.Groups[1].Value + "." + match.Groups[2].Value + "." + match.Groups[3].Value + "." + match.Groups[4].Value;
								}
							}
							list[0].SendKeys(text);
							Thread.Sleep(500);
							if (list != null && list.Count >= 4)
							{
								list[1].SendKeys(text2);
								Thread.Sleep(500);
								if (text3 != "")
								{
									if (list != null && list.Count >= 4)
									{
										list[2].SendKeys(text3);
										Thread.Sleep(500);
									}
									if (list != null && list.Count >= 4)
									{
										list[3].SendKeys(text4);
										Thread.Sleep(500);
									}
								}
								string text6 = "";
								IWebElement webElement = AppGetObject(By.XPath("//*[@resource-id=\"com.cell47.College_Proxy:id/proxy_start_button\"]"), p_driver);
								if (webElement != null)
								{
									webElement.Click();
									Thread.Sleep(2000);
									IWebElement webElement2 = WaitMeXPath(By.XPath("//*[@text='STOP PROXY SERVICE']"), p_driver);
									if (webElement2 != null)
									{
										return true;
									}
									text6 = GetPageSource(p_driver, entity);
									if (text6.Contains("text=\"OK\""))
									{
										webElement = AppGetObject(By.XPath("//*[@text=\"OK\"]"), p_driver);
										if (webElement != null)
										{
											webElement.Click();
											Thread.Sleep(5000);
											text6 = GetPageSource(p_driver, entity);
											if (text6.Contains("STOP PROXY SERVICE"))
											{
												return true;
											}
										}
									}
								}
								int num = 0;
								text6 = GetPageSource(p_driver, entity);
								while (!text6.Contains("text=\"Connected\"") && num++ < 5)
								{
									if (!text6.Contains("text=\"OK\""))
									{
										Thread.Sleep(2000);
									}
									else
									{
										webElement = AppGetObject(By.XPath("//*[@text=\"OK\"]"), p_driver);
										if (webElement != null)
										{
											webElement.Click();
											Thread.Sleep(1000);
											break;
										}
									}
									text6 = GetPageSource(p_driver, entity);
								}
								IWebElement webElement3 = WaitMeCount("//*[@text=\"Connected\"]", p_driver, 3);
								BackToHome(entity.DeviceId);
								return webElement3 != null;
							}
							ShowTextMessageOnPhone(deviceId, "Not found editText_port");
							Utils.CCKLog("SetProxy", "Not found editText_port");
						}
						else
						{
							ShowTextMessageOnPhone(deviceId, "Not found editText_address");
							Utils.CCKLog("SetProxy", "Not found editText_address");
						}
					}
					Utils.CCKLog("SetProxy", "Not found textView_address - Recall");
					return SetProxy(deviceId, proxy, p_driver, entity);
				}
			}
			catch (Exception ex2)
			{
				ShowTextMessageOnPhone(deviceId, "Proxy - Error ");
				Utils.CCKLog("SetProxy", ex2.Message);
			}
			return result;
		}

		public static bool SetProxy1(string deviceId, string proxy, RemoteWebDriver driver, DeviceEntity entity = null)
		{
			bool result = false;
			try
			{
				string empty = string.Empty;
				string[] array = proxy.Split(':');
				string empty2 = string.Empty;
				string empty3 = string.Empty;
				empty = " shell settings put global http_proxy :0";
				ExecuteCMD(deviceId, empty);
				if (array.Length == 2)
				{
					empty2 = array[0].Trim();
					empty3 = array[1].Trim();
					empty = string.Concat(new string[4] { "shell settings put global http_proxy ", empty2, ":", empty3 });
					ExecuteCMD(deviceId, empty);
					return true;
				}
				if (array.Length > 2)
				{
					string text = array[0].Trim();
					string text2 = array[1].Trim();
					string text3 = ((array.Length == 4) ? array[2].Trim() : "");
					string text4 = ((array.Length == 4) ? array[3].Trim() : "");
					if (!IsInstallApp(deviceId, "com.cell47.College_Proxy"))
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
						InstallApp(deviceId, Application.StartupPath + "\\Devices\\" + text5);
						Thread.Sleep(2000);
					}
					while (true)
					{
						StopApp(deviceId, "com.cell47.College_Proxy");
						ExecuteCMD(deviceId, "shell pm clear com.cell47.College_Proxy");
						BackToHome(deviceId);
						OpenApp(deviceId, "com.cell47.College_Proxy");
						Thread.Sleep(5000);
						if (driver == null)
						{
							break;
						}
						string pageSource = GetPageSource(driver, entity);
						int num = 0;
						while (!pageSource.Contains("textView_address") && num < 5)
						{
							num++;
							Thread.Sleep(5000);
							pageSource = GetPageSource(driver, entity);
						}
						ShowTextMessageOnPhone(deviceId, "Proxy - " + text);
						Thread.Sleep(2000);
						pageSource = GetPageSource(driver, entity);
						if (pageSource.Contains("textView_address"))
						{
							IWebElement webElement = AppGetObject(By.XPath("//*[@resource-id=\"com.cell47.College_Proxy:id/editText_address\"]"), driver);
							if (webElement != null)
							{
								webElement.SendKeys(text);
								Thread.Sleep(1000);
								webElement = AppGetObject(By.XPath("//*[@resource-id=\"com.cell47.College_Proxy:id/editText_port\"]"), driver);
								if (webElement != null)
								{
									webElement.SendKeys(text2);
									Thread.Sleep(1000);
									if (text3 != "")
									{
										webElement = AppGetObject(By.XPath("//*[@resource-id=\"com.cell47.College_Proxy:id/editText_username\"]"), driver);
										if (webElement != null)
										{
											webElement.SendKeys(text3);
											Thread.Sleep(1000);
										}
										webElement = AppGetObject(By.XPath("//*[@resource-id=\"com.cell47.College_Proxy:id/editText_password\"]"), driver);
										if (webElement != null)
										{
											webElement.SendKeys(text4);
											Thread.Sleep(1000);
										}
									}
									AppGetObject(By.XPath("//*[@resource-id=\"com.cell47.College_Proxy:id/proxy_start_button\"]"), driver)?.Click();
									pageSource = GetPageSource(driver, entity);
									num = 0;
									while (!pageSource.Contains("text=\"Connected\"") && num < 5)
									{
										num++;
										if (!pageSource.Contains("text=\"OK\""))
										{
											Thread.Sleep(3000);
										}
										else
										{
											IWebElement webElement2 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
											if (webElement2 != null)
											{
												webElement2.Click();
												Thread.Sleep(1000);
											}
										}
										pageSource = GetPageSource(driver, entity);
									}
									pageSource = GetPageSource(driver, entity);
									if (pageSource.Contains("text=\"OK\""))
									{
										IWebElement webElement2 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
										if (webElement2 != null)
										{
											webElement2.Click();
											Thread.Sleep(1000);
										}
									}
									return pageSource.Contains("Connected");
								}
								ShowTextMessageOnPhone(deviceId, "Not found editText_port");
								Utils.CCKLog("SetProxy", "Not found editText_port");
							}
							else
							{
								ShowTextMessageOnPhone(deviceId, "Not found editText_address");
								Utils.CCKLog("SetProxy", "Not found editText_address");
							}
							continue;
						}
						Utils.CCKLog("SetProxy", "Not found textView_address - Recall");
						return SetProxy(deviceId, proxy, driver, entity);
					}
				}
			}
			catch (Exception ex2)
			{
				ShowTextMessageOnPhone(deviceId, "Proxy - Error ");
				Utils.CCKLog("SetProxy", ex2.Message);
			}
			return result;
		}

		public static bool SetProxyDrony(string deviceId, string proxy, RemoteWebDriver driver, DeviceEntity entity = null)
		{
			//IL_013d: Incompatible stack heights: 1 vs 0
			//IL_0157: Incompatible stack heights: 1 vs 0
			//IL_0175: Incompatible stack heights: 0 vs 1
			bool result = false;
			try
			{
				string empty = string.Empty;
				string[] array = proxy.Split(':');
				string empty2 = string.Empty;
				string empty3 = string.Empty;
				empty = " shell settings put global http_proxy :0";
				ExecuteCMD(deviceId, empty);
				if (array.Length == 2 && proxy.Length < 10)
				{
					empty2 = array[0].Trim();
					empty3 = array[1].Trim();
					empty = string.Concat(new string[4] { "shell settings put global http_proxy ", empty2, ":", empty3 });
					ExecuteCMD(deviceId, empty);
					return result;
				}
				if (array.Length >= 2)
				{
					string text = array[0].Trim();
					string text2 = array[1].Trim();
					string text3 = ((array.Length == 4) ? array[2].Trim() : "");
					string text4 = ((array.Length == 4) ? array[3].Trim() : "");
					if (!IsInstallApp(deviceId, "org.sandrob.drony"))
					{
						string text5 = "org.sandrob.drony.apk";
						_ = -1794027198;
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
						InstallApp(deviceId, Application.StartupPath + "\\Devices\\" + text5);
						Thread.Sleep(2000);
					}
					ExecuteCMD(deviceId, "shell pm clear org.sandrob.drony");
					BackToHome(deviceId);
					OpenApp(deviceId, "org.sandrob.drony");
					Thread.Sleep(3000);
					if (driver != null)
					{
						string pageSource = GetPageSource(driver, entity);
						int num = 0;
						while (true)
						{
							if (!pageSource.Contains("text=\"SETTINGS\""))
							{
								if (num >= 10)
								{
									break;
								}
								Thread.Sleep(1000);
								if (pageSource.Contains("com.android.permissioncontroller:id/permission_allow_always_button"))
								{
									IWebElement webElement = AppGetObject(By.XPath("//*[@resource-id='com.android.permissioncontroller:id/permission_allow_always_button']"), driver);
									if (webElement != null)
									{
										webElement.Click();
										Thread.Sleep(1000);
										pageSource = GetPageSource(driver, entity);
										if (pageSource.Contains("com.android.permissioncontroller:id/permission_allow_button"))
										{
											webElement = AppGetObject(By.XPath("//*[@resource-id='com.android.permissioncontroller:id/permission_allow_button']"), driver);
											if (webElement != null)
											{
												webElement.Click();
												Thread.Sleep(1000);
												pageSource = GetPageSource(driver, entity);
											}
										}
										Point screenResolution = GetScreenResolution(deviceId);
										if (pageSource.Contains("text=\"SETTINGS\""))
										{
											IWebElement webElement2 = AppGetObject(By.XPath("//*[@text=\"SETTINGS\"]"), driver);
											if (webElement2 != null)
											{
												Swipe(deviceId, screenResolution.X - 10, screenResolution.Y / 2, screenResolution.X / 3, screenResolution.Y / 2, 1000);
												Thread.Sleep(1000);
												pageSource = GetPageSource(driver, entity);
												List<IWebElement> list = AppGetObjects(By.XPath("//*[@resource-id=\"android:id/summary\"]"), driver);
												if (list != null && list.Count == 3)
												{
													list[1].Click();
													Thread.Sleep(1000);
													IWebElement webElement3 = AppGetObject(By.XPath("//*[@resource-id=\"android:id/edit\"]"), driver);
													if (webElement3 != null)
													{
														webElement3.Clear();
														webElement3.SendKeys(text2);
														Thread.Sleep(1000);
														webElement3 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
														if (webElement3 != null)
														{
															webElement3.Click();
															Thread.Sleep(1000);
															IWebElement webElement4 = AppGetObject(By.XPath("//*[@text=\"Wi-Fi\"]"), driver);
															if (webElement4 != null)
															{
																webElement4.Click();
																Thread.Sleep(1000);
																webElement4 = AppGetObject(By.XPath("//*[@content-desc=\"Refresh\"]"), driver);
																if (webElement4 != null)
																{
																	webElement4.Click();
																	Thread.Sleep(2000);
																	IWebElement webElement5 = AppGetObject(By.XPath("//*[@resource-id=\"org.sandrob.drony:id/network_list_item_id_name_value\"]"), driver);
																	if (webElement5 != null)
																	{
																		webElement5.Click();
																		Thread.Sleep(1000);
																		webElement5 = AppGetObject(By.XPath("//*[@text=\"Hostname\"]"), driver);
																		if (webElement5 != null)
																		{
																			webElement5.Click();
																			Thread.Sleep(300);
																			webElement5 = AppGetObject(By.XPath("//android.widget.EditText"), driver);
																			if (webElement5 != null)
																			{
																				webElement5.SendKeys(text);
																				Thread.Sleep(300);
																				webElement3 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
																				if (webElement3 != null)
																				{
																					webElement3.Click();
																					Thread.Sleep(300);
																				}
																			}
																		}
																		webElement5 = AppGetObject(By.XPath("//*[@text=\"Port\"]"), driver);
																		if (webElement5 != null)
																		{
																			webElement5.Click();
																			Thread.Sleep(300);
																			webElement5 = AppGetObject(By.XPath("//android.widget.EditText"), driver);
																			if (webElement5 != null)
																			{
																				webElement5.Clear();
																				webElement5.SendKeys(text2);
																				Thread.Sleep(300);
																				webElement3 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
																				if (webElement3 != null)
																				{
																					webElement3.Click();
																					Thread.Sleep(300);
																				}
																			}
																		}
																		webElement5 = AppGetObject(By.XPath("//*[@text=\"Username\"]"), driver);
																		if (webElement5 != null && text3 != "")
																		{
																			webElement5.Click();
																			Thread.Sleep(300);
																			webElement5 = AppGetObject(By.XPath("//android.widget.EditText"), driver);
																			if (webElement5 != null)
																			{
																				webElement5.Clear();
																				webElement5.SendKeys(text3);
																				Thread.Sleep(300);
																				webElement3 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
																				if (webElement3 != null)
																				{
																					webElement3.Click();
																					Thread.Sleep(300);
																				}
																			}
																		}
																		webElement5 = AppGetObject(By.XPath("//*[@text=\"Password\"]"), driver);
																		if (webElement5 != null && text4 != "")
																		{
																			webElement5.Click();
																			Thread.Sleep(300);
																			webElement5 = AppGetObject(By.XPath("//android.widget.EditText"), driver);
																			if (webElement5 != null)
																			{
																				webElement5.Clear();
																				webElement5.SendKeys(text4);
																				Thread.Sleep(300);
																				webElement3 = AppGetObject(By.XPath("//*[@text=\"OK\"]"), driver);
																				if (webElement3 != null)
																				{
																					webElement3.Click();
																					Thread.Sleep(300);
																				}
																			}
																		}
																		webElement5 = AppGetObject(By.XPath("//*[@text=\"Proxy type\"]"), driver);
																		if (webElement5 != null)
																		{
																			webElement5.Click();
																			Thread.Sleep(300);
																			webElement5 = AppGetObject(By.XPath("//*[@text=\"Manual\"]"), driver);
																			if (webElement5 != null)
																			{
																				webElement5.Click();
																				Thread.Sleep(300);
																				for (int i = 0; i < 2; i++)
																				{
																					webElement5 = AppGetObject(By.XPath("//*[@content-desc=\"Navigate up\"]"), driver);
																					if (webElement5 != null)
																					{
																						webElement5.Click();
																						Thread.Sleep(200);
																					}
																				}
																				Swipe(deviceId, 10, screenResolution.Y / 2, screenResolution.X * 3 / 4, screenResolution.Y / 2, 1000);
																				Thread.Sleep(500);
																				webElement5 = AppGetObject(By.XPath("//*[@resource-id=\"org.sandrob.drony:id/toggleButtonOnOff\"]"), driver);
																				if (webElement5 != null && webElement5.GetAttribute("text") == "OFF")
																				{
																					webElement5.Click();
																					Thread.Sleep(2000);
																					result = true;
																				}
																				else if (webElement5 != null && webElement5.GetAttribute("text") == "ON")
																				{
																					result = true;
																					webElement5.Click();
																					Thread.Sleep(1000);
																					webElement5.Click();
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
										}
									}
								}
								num++;
								Thread.Sleep(1000);
								pageSource = GetPageSource(driver, entity);
								continue;
							}
							return result;
						}
						return result;
					}
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		private void OpenCollectProxy(string proxy)
		{
			List<string> list = proxy.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			if (list.Count != 4)
			{
			}
		}

		public static void EnalbeWifi(string DeviceId)
		{
			ExecuteCMD(DeviceId, "shell \"svc wifi enable\"");
		}

		public static void SetVolumnMute(string deviceId)
		{
			try
			{
				new Task(delegate
				{
					ExecuteCMD(deviceId, "shell media volume --stream 5 --set 0");
					ExecuteCMD(deviceId, "shell media volume --stream 3 --set 0");
					ExecuteCMD(deviceId, "shell media volume --stream 1 --set 0");
				}).Start();
			}
			catch (Exception)
			{
			}
		}

		public static bool ClearProxy(string deviceId, bool delete = false)
		{
			bool result = false;
			try
			{
				string empty = string.Empty;
				empty = " shell settings delete global http_proxy";
				ExecuteCMD(deviceId, empty);
				empty = " shell settings put global http_proxy :0";
				ExecuteCMD(deviceId, empty);
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool PushInfoFile(string SerialNo, string Destination)
		{
			string cmd = " push " + Destination;
			ExecuteCMD(SerialNo, cmd);
			return true;
		}

		public static void SetStoragePermission(string SerialNo)
		{
			new Task(delegate
			{
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME} android.permission.CAMERA");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME} android.permission.READ_CONTACTS");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME} android.permission.CALL_PHONE");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME} android.permission.READ_EXTERNAL_STORAGE");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME} android.permission.WRITE_EXTERNAL_STORAGE");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME} android.permission.RECORD_AUDIO");
			}).Start();
			Task.Run(delegate
			{
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME_LITE} android.permission.CAMERA");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME_LITE} android.permission.READ_CONTACTS");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME_LITE} android.permission.CALL_PHONE");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME_LITE} android.permission.READ_EXTERNAL_STORAGE");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME_LITE} android.permission.WRITE_EXTERNAL_STORAGE");
				ExecuteCMD(SerialNo, $"shell  pm grant {CaChuaConstant.PACKAGE_NAME_LITE} android.permission.RECORD_AUDIO");
			});
		}

		public static bool Reconnect2Device(string deviceId)
		{
			if (deviceId.Contains(":"))
			{
				Utils.CCKLogByDevice("Reconnect2Device", deviceId, deviceId.Replace(":", "").Replace(".", ""));
				ExecuteCMD(" connect " + deviceId);
				Thread.Sleep(1000);
			}
			return true;
		}

		public static void GetProfileInfoByToken(string SerialNo, string uid)
		{
			string accessTokenFromBackup = GetAccessTokenFromBackup(uid);
			if (!(accessTokenFromBackup != ""))
			{
			}
		}

		public static string HttpRequestByToken(string SerialNo, string uid, string urlWithtoken = "")
		{
			string accessTokenFromBackup = GetAccessTokenFromBackup(uid);
			string arg = ((urlWithtoken == "") ? $"https://graph.facebook.com/v8.0/me?access_token={accessTokenFromBackup}&fields=name,mobile_phone,id,birthday,gender,email,friends" : urlWithtoken);
			string text = $"file_{uid}.txt";
			ExecuteCMD(SerialNo, $"shell su rm -r /sdcard/{text}\"");
			string contents = $"{{\"Url\" : \"{arg}\" , \"FileName\" : \"{text}\"}}";
			if (!Directory.Exists(Application.StartupPath + "\\Temp"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\Temp");
			}
			File.WriteAllText(Application.StartupPath + "\\Temp\\" + $"file_{uid}.txt", contents, Encoding.UTF8);
			string text2 = ExecuteCMD(SerialNo, "shell su rm -r /sdcard/cck_api.txt\"");
			Thread.Sleep(500);
			text2 = ExecuteCMD(SerialNo, string.Format("push \"{0}\" /sdcard/cck_api.txt", Application.StartupPath + "\\Temp\\" + string.Format(text, uid)));
			if (text2.Contains("bytes in"))
			{
				ExecuteCMD(SerialNo, "shell su -c am startservice com.cck.support/.CckService");
				for (int i = 0; i < 5; i++)
				{
					text2 = ExecuteCMD(SerialNo, "pull /sdcard/" + text + " \"" + Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt" + "\"");
					if (!text2.Contains("bytes in"))
					{
						Thread.Sleep(1000);
						continue;
					}
					ExecuteCMD(SerialNo, "shell su rm -r /sdcard/" + text + " \"");
					if (!File.Exists(Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt"))
					{
						Thread.Sleep(1000);
						continue;
					}
					string result = Utils.ReadTextFile(Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt");
					Thread.Sleep(300);
					File.Delete(Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt");
					return result;
				}
			}
			return "";
		}

		public static void AddContact2Phone(string SerialNo, string Uid)
		{
			string path = string.Format(CaChuaConstant.VCF_FileFormat, Uid);
			if (File.Exists(path))
			{
				string text = ExecuteCMD(SerialNo, string.Format("push \"{0}\" /sdcard/contacts.txt", Application.StartupPath + "\\Vcf\\" + $"{Uid}.vcf"));
				if (text.Contains("bytes in"))
				{
					ExecuteCMD(SerialNo, "shell pm clear com.android.providers.contacts");
					Thread.Sleep(1000);
					ExecuteCMD(SerialNo, "shell su -c am startservice com.cck.support/.CckServiceContacts");
				}
			}
		}

		public static string HttpRequestByCookie(string SerialNo, string uid, string urlWithtoken = "")
		{
			string accessTokenFromBackup = GetAccessTokenFromBackup(uid);
			string arg = ((urlWithtoken == "") ? $"https://graph.facebook.com/v8.0/me?access_token={accessTokenFromBackup}&fields=name,id,birthday,gender,email,friends" : urlWithtoken);
			string text = $"file_{uid}.txt";
			string contents = $"{{\"Url\" : \"{arg}\" , \"FileName\" : \"{text}\"}}";
			if (!Directory.Exists(Application.StartupPath + "\\Temp"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\Temp");
			}
			File.WriteAllText(Application.StartupPath + "\\Temp\\" + $"file_{uid}.txt", contents, Encoding.UTF8);
			string text2 = ExecuteCMD(SerialNo, string.Format("push \"{0}\" /sdcard/cck_api.txt", Application.StartupPath + "\\Temp\\" + string.Format(text, uid)));
			if (text2.Contains("bytes in"))
			{
				ExecuteCMD(SerialNo, "shell su -c am startservice com.cck.support/.CckService");
				for (int i = 0; i < 5; i++)
				{
					text2 = ExecuteCMD(SerialNo, "pull /sdcard/" + text + " \"" + Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt" + "\"");
					if (!text2.Contains("bytes in"))
					{
						Thread.Sleep(1000);
						continue;
					}
					ExecuteCMD(SerialNo, "shell su rm -r /sdcard/" + text + " \"");
					if (!File.Exists(Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt"))
					{
						Thread.Sleep(1000);
						continue;
					}
					string result = Utils.ReadTextFile(Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt");
					Thread.Sleep(300);
					File.Delete(Application.StartupPath + "\\Temp\\" + $"ret_file_{uid}.txt");
					return result;
				}
			}
			return "";
		}

		internal static IWebElement AppGetObject(string xpath, RemoteWebDriver driver)
		{
			return AppGetObject(By.XPath(xpath), driver);
		}

		public static string GetAccessTokenFromBackup(string uid)
		{
			string text = Application.StartupPath + "\\\\Authentication\\\\" + uid + "\\\\com.facebook.katana\\\\authentication";
			if (!File.Exists(text))
			{
				return "";
			}
			string text2 = Utils.ReadTextFile(text);
			string[] array = text2.Split(new string[1] { "access_token" }, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array[1].Split(new string[1] { "uid" }, StringSplitOptions.RemoveEmptyEntries);
			string text3 = array2[0].Substring(2);
			text3 = text3.Replace("\u0005\0\u000f", "");
			text3 = text3.Replace("\u0005\0\u000f", "");
			return text3.Replace("\u0005\0\u0003", "");
		}

		public bool NavigateUrl(string deviceId, string uid, string type = "page")
		{
			bool result = false;
			try
			{
				string url = "";
				if (type == "page")
				{
					url = "fb://page/" + uid;
				}
				else if (!(type == "profile"))
				{
					if (type == "group")
					{
						url = "fb://group/" + uid;
					}
					else if (type == "video")
					{
						url = "fb://fullscreen_video/" + uid + "?loop=0";
					}
					else if (type == "feed")
					{
						url = "fb://dbl_login_activity";
					}
					else
					{
						switch (type)
						{
						case "notifications":
							url = "fb://notifications";
							break;
						case "invite_friends_like_page":
							url = "fb://page/" + uid + "/invite_friends_to_like_page";
							break;
						case "registration":
							url = "fb://registration";
							break;
						case "notification_settings_email":
							url = "fb://notification_settings_email";
							break;
						case "post":
							url = "fb://faceweb/f?href=http://facebook.com/" + uid;
							break;
						case "feed":
							url = "fb://feed";
							break;
						case "security_settings":
							url = "fb://security_settings";
							break;
						case "url":
							url = "fb://facewebmodal/f?href=" + uid;
							break;
						case "notification_settings_phone_number":
							url = "fb://notification_settings_phone_number";
							break;
						case "profile_edit":
							url = "fb://profile_edit";
							break;
						case "settings":
							url = "fb://settings";
							break;
						case "dbl_login_activity":
							url = "fb://dbl_login_activity";
							break;
						case "logout_account":
							url = "fb://logged_out_push_interstitial";
							break;
						case "requests":
							url = "fb://requests";
							break;
						case "watch":
							url = "fb://top_live_videos";
							break;
						}
					}
				}
				else
				{
					url = "fb://profile/" + uid;
				}
				FBNavigateUrl(deviceId, url);
				result = true;
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static string GetUidFromBackup(string phone)
		{
			string text = Application.StartupPath + "\\\\Authentication\\\\" + phone + "\\\\com.facebook.katana\\\\authentication";
			if (File.Exists(text))
			{
				string input = Utils.ReadTextFile(text);
				Regex regex = new Regex(":\"c_user\",\"value\":\"([0-9]+)\"");
				Match match = regex.Match(input);
				if (match.Success)
				{
					return match.Groups[1].Value;
				}
			}
			return "";
		}

		internal static IWebElement WaitMe(string xpath, RemoteWebDriver m_driver)
		{
			return WaitMeXPath(By.XPath(xpath), m_driver, 5);
		}

		internal static IWebElement WaitMeCount(string xpath, RemoteWebDriver m_driver, int retry = 5)
		{
			return WaitMeXPath(By.XPath(xpath), m_driver, retry);
		}

		public static void BackToHome(string SerialNo, bool aOpenApp = true)
		{
			string cmd = "shell input keyevent KEYCODE_HOME";
			ExecuteCMD(SerialNo, cmd);
			if (aOpenApp)
			{
				OpenApp(SerialNo, CaChuaConstant.PACKAGE_NAME);
			}
		}

		public static void Back(string SerialNo)
		{
			string cmd = "shell input keyevent KEYCODE_BACK";
			ExecuteCMD(SerialNo, cmd);
		}

		public static void RecentApps(string SerialNo)
		{
			string cmd = "shell input keyevent KEYCODE_APP_SWITCH";
			ExecuteCMD(SerialNo, cmd);
		}

		public static void SendPadDown(string SerialNo)
		{
			string cmd = "shell input keyevent 20";
			ExecuteCMD(SerialNo, cmd);
		}

		public static void SendDEL(string SerialNo)
		{
			string cmd = "shell input keyevent DEL";
			ExecuteCMD(SerialNo, cmd);
		}

		public static void SendTab(string SerialNo)
		{
			InputKeyEvent(SerialNo, "61");
		}

		public static void SendDel(string SerialNo)
		{
			InputKeyEvent(SerialNo, "DEL");
		}

		public static void SendSpace(string SerialNo)
		{
			InputKeyEvent(SerialNo, "KEYCODE_SPACE");
		}

		public static void SendEnter(string SerialNo)
		{
			InputKeyEvent(SerialNo, "66");
		}

		public static bool IsScreenOn(string SerialNo)
		{
			string text = ExecuteCMD(SerialNo, "shell dumpsys power | findstr mHoldingDisplaySuspendBlocker");
			if (!text.Contains("true"))
			{
				return false;
			}
			return true;
		}

		public static string CheckFileExist(string SerialNo, string cmd)
		{
			return ExecuteCMD(SerialNo, cmd);
		}

		public static bool IsInstallApp(string SerialNo, string appName)
		{
			bool result = false;
			int num = 0;
			while (num < 3)
			{
				if (!ExecuteCMD(SerialNo, "shell pm list packages").Trim().Contains(appName))
				{
					num++;
					Thread.Sleep(1000);
					continue;
				}
				result = true;
				break;
			}
			return result;
		}

		public static bool IsUnInstallApp(string SerialNo, string appName)
		{
			while (ExecuteCMD(SerialNo, "shell pm list packages").Trim().Contains(appName))
			{
				Thread.Sleep(2000);
			}
			return true;
		}

		public static void InstallADBkeyBoard(string SerialNo)
		{
			ExecuteCMD(SerialNo, " install \"" + Application.StartupPath + "\\Devices\\ADBKeyboard.apk\"");
		}

		public static void InstallCCKChangePhone(string SerialNo, RemoteWebDriver driver = null)
		{
		}

		public static void InstallApp(string SerialNo, string app)
		{
			ExecuteCMD(SerialNo, " install \"" + app + "\"");
		}

		public static void UnInstallAppFB(string SerialNo)
		{
			ExecuteCMD(SerialNo, " uninstall com.facebook.katana");
		}

		public static void RebootDevice(string SerialNo)
		{
			try
			{
				try
				{
					string startupPath = Application.StartupPath;
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = " -s " + SerialNo + " shell reboot",
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void TurnScreenOffDevice(string SerialNo)
		{
			if (!IsScreenTurnOn(SerialNo))
			{
				return;
			}
			try
			{
				try
				{
					string startupPath = Application.StartupPath;
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = " -s " + SerialNo + " shell input keyevent 26",
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void TurnScreenOnDevice(string SerialNo)
		{
			try
			{
				try
				{
					string startupPath = Application.StartupPath;
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = " -s " + SerialNo + " shell input keyevent KEYCODE_WAKEUP",
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void TurnOnDevice(string IDDevice)
		{
			try
			{
				try
				{
					string startupPath = Application.StartupPath;
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = " -s " + IDDevice + " shell reboot -p",
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void TurnOffDevice(string IDDevice)
		{
			try
			{
				try
				{
					string startupPath = Application.StartupPath;
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = " -s " + IDDevice + " shell reboot -p",
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static Dictionary<string, string> GetListSerialDeviceWithName()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = Application.StartupPath + "\\adb.exe",
					Arguments = " devices -l",
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				};
				process.Start();
				StreamReader standardOutput = process.StandardOutput;
				string text = standardOutput.ReadToEnd();
				string[] array = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (!text2.Contains("device"))
					{
						continue;
					}
					Regex regex = new Regex("([^/]+) device product:([^/]+) model");
					if (!regex.Match(text2).Success)
					{
						if (string.Compare(text2.Trim(), "", ignoreCase: false) == 0 || text2.Contains("List of devices attached"))
						{
							continue;
						}
						string[] array3 = text2.Split("\\t ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						string value = "";
						for (int j = 0; j < array3.Length; j++)
						{
							if (array3[j].StartsWith("model:"))
							{
								value = array3[j].Replace("model:", "");
							}
						}
						dictionary.Add(array3[0], value);
					}
					else
					{
						Match match = regex.Match(text2);
						dictionary.Add(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
					}
				}
				return dictionary;
			}
			catch (Exception)
			{
			}
			return dictionary;
		}

		public static List<string> GetSerialDevice()
		{
			List<string> list = new List<string>();
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = Application.StartupPath + "\\adb.exe",
					Arguments = " devices -l",
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				};
				process.Start();
				StreamReader standardOutput = process.StandardOutput;
				string text = standardOutput.ReadToEnd();
				string[] array = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (string.Compare(text2.Trim(), "", ignoreCase: false) != 0 && !text2.Contains("List of devices attached"))
					{
						text2.Split("\\t ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						list.Add(text2.Replace("device", "").Trim());
					}
				}
			}
			catch (Exception)
			{
			}
			return list;
		}

		public static string ExecuteCMDReconnectOffline()
		{
			string result = "";
			try
			{
				using Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = Application.StartupPath + "\\Config\\Sdk\\platform-tools\\adb.exe",
					Arguments = "adb reconnect offline",
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				};
				process.Start();
				StreamReader standardOutput = process.StandardOutput;
				result = standardOutput.ReadToEnd();
				process.WaitForExit();
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static List<string> GetListSerialDevice(bool fullInfo = false)
		{
			List<string> list = new List<string>();
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = Application.StartupPath + "\\adb.exe",
					Arguments = " devices",
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				};
				process.Start();
				StreamReader standardOutput = process.StandardOutput;
				string text = standardOutput.ReadToEnd();
				string[] array = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (!fullInfo && !text2.Contains("device"))
					{
						continue;
					}
					Regex regex = new Regex("([^/]+) device product:([^/]+) model");
					if (!regex.Match(text2).Success)
					{
						if (string.Compare(text2.Trim(), "", ignoreCase: false) != 0 && !text2.Contains("List of devices attached"))
						{
							text2.Split("\\t ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
							list.Add(text2.Replace("device", "").Trim());
						}
					}
					else
					{
						Match match = regex.Match(text2);
						list.Add(match.Groups[1].Value.Trim());
					}
				}
			}
			catch (Exception)
			{
			}
			return list;
		}

		public static string GetDeviceName(string SerialNo)
		{
			string result = "";
			try
			{
				string text = ExecuteCMD(SerialNo, " shell settings get global device_name");
				string[] array = text.Split('\r');
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (string.Compare(text2.Trim(), "", ignoreCase: false) != 0)
					{
						result = text2.Trim();
					}
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static string ExecuteCMD(string index, string cmd)
		{
			string result = "";
			try
			{
				string startupPath = Application.StartupPath;
				string text = "";
				try
				{
					Process process = new Process();
					cmd = string.Concat(new string[4]
					{
						" -s ",
						"\"" + index + "\"",
						" ",
						cmd
					});
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = cmd,
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					StreamReader standardOutput = process.StandardOutput;
					text = standardOutput.ReadToEnd();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
				result = text;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static string ExecuteCMD(string cmd)
		{
			string result = "";
			try
			{
				if (cmd.Contains("connect"))
				{
					Utils.CCKLogByDevice("Reconnect2Device", cmd, "xxx");
				}
				string startupPath = Application.StartupPath;
				string text = "";
				try
				{
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = cmd,
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					StreamReader standardOutput = process.StandardOutput;
					text = standardOutput.ReadToEnd();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
				result = text;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static string ExecuteCMDCommand(string index, string cmd)
		{
			string startupPath = Application.StartupPath;
			string result = "";
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = startupPath + "\\adb.exe",
					Arguments = cmd,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				};
				process.Start();
				StreamReader standardOutput = process.StandardOutput;
				result = standardOutput.ReadToEnd();
				process.WaitForExit();
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static void ClearAppData(string IDDevice, string app)
		{
			try
			{
				ExecuteCMD(IDDevice, $"shell pm clear {app}");
			}
			catch
			{
			}
		}

		public static void ClearAppStorage(string IDDevice, RemoteWebDriver m_driver, string app)
		{
			try
			{
				if (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.CLEAR_APP)))
				{
					ExecuteCMD(IDDevice, $"shell am start -a android.settings.APPLICATION_DETAILS_SETTINGS -d package:{app}");
					GetPageSource(m_driver, new DeviceEntity());
					IWebElement webElement = WaitMe("//*[lower-case(@text)= \"storage & cache\" or lower-case(@content-desc)= \"storage & cache\" or  contains(lower-case(@text), \"storage\") or contains(lower-case(@content-desc), \"storage\")]", m_driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000);
						webElement = AppGetObject("//*[contains(lower-case(@text), \"clear storage\") or contains(lower-case(@content-desc),\"clear storage\")]", m_driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							webElement = AppGetObject("//*[contains(lower-case(@text), \"ok\") or contains(lower-case(@content-desc),\"ok\")]", m_driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(1000);
							}
						}
					}
				}
				ClearAppData(IDDevice, app);
			}
			catch
			{
			}
		}

		public static void TapLong(string IDDevice, double x, double y)
		{
			try
			{
				ExecuteCMD(IDDevice, string.Format("shell input swipe {0} {1} {0} {1} 1500", x, y));
			}
			catch (Exception)
			{
			}
		}

		public static void TapLong(string IDDevice, double x, double y, double x1, double y1)
		{
			try
			{
				ExecuteCMD(IDDevice, $"shell input touchscreen swipe {x} {y} {x1} {y1} 1000");
			}
			catch (Exception)
			{
			}
		}

		public static void Tap(string IDDevice, double x, double y)
		{
			try
			{
				ExecuteCMD(IDDevice, $"shell input touchscreen tap {x} {y}");
			}
			catch (Exception)
			{
			}
		}

		public static void TapByPercent(string IDDevice, double x, double y)
		{
			checked
			{
				try
				{
					Point screenResolution = GetScreenResolution(IDDevice);
					int num = (int)Math.Round(x * ((double)screenResolution.X * 1.0 / 100.0));
					int num2 = (int)Math.Round(y * ((double)screenResolution.Y * 1.0 / 100.0));
					string cmd = $"shell input tap {num} {num2}";
					ExecuteCMD(IDDevice, cmd);
				}
				catch (Exception)
				{
				}
			}
		}

		public static void WriteLog(string form, string mess)
		{
			lock (locker)
			{
				using FileStream stream = new FileStream(Application.StartupPath + "\\data\\log.txt", FileMode.Append, FileAccess.Write);
				using StreamWriter streamWriter = new StreamWriter(stream);
				streamWriter.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "  " + form + ": " + mess);
			}
		}

		public static string AddQuotesIfRequired(string path)
		{
			return string.IsNullOrWhiteSpace(path) ? string.Empty : ((!path.Contains(" ") || path.StartsWith("\"") || path.EndsWith("\"")) ? path : ("\"" + path + "\""));
		}

		public static Bitmap ScreenShoot(string IDDevice, bool isDeleteImageAfterCapture = true, string fileName = "mypicture.png")
		{
			Bitmap result = null;
			try
			{
				if (!Directory.Exists("ScreenCap"))
				{
					Directory.CreateDirectory("ScreenCap");
				}
				string text = Path.GetFileNameWithoutExtension(fileName) + Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);
				string text2 = Application.StartupPath + "\\ScreenCap\\" + text;
				AddQuotesIfRequired(Application.StartupPath);
				while (File.Exists(text2))
				{
					try
					{
						File.Delete(text2);
					}
					catch (Exception)
					{
					}
				}
				ExecuteCMD(IDDevice, string.Format("shell rm -f \"{0}\"", "/sdcard/" + text));
				ExecuteCMD(IDDevice, string.Format("shell screencap -p \"{0}\"", "/sdcard/" + text));
				ExecuteCMD(IDDevice, string.Format("pull \"{0}\" \"{1}\"", "/sdcard/" + text, Application.StartupPath + "\\ScreenCap\\" + text));
				ExecuteCMD(IDDevice, string.Format("shell rm -f \"{0}\"", "/sdcard/" + text));
				Thread.Sleep(1000);
				using (Bitmap original = new Bitmap(text2))
				{
					result = new Bitmap(original);
				}
				if (isDeleteImageAfterCapture)
				{
					try
					{
						File.Delete(text2);
					}
					catch (Exception)
					{
					}
				}
				return result;
			}
			catch (Exception ex3)
			{
				WriteLog("ScreenShoot : LDPlayer-" + IDDevice, ex3.Message);
			}
			GC.Collect();
			return result;
		}

		public static Bitmap CropImage(string deviceId, string filename, string dest, int x, int y, int width, int height)
		{
			try
			{
				if (File.Exists(dest))
				{
					File.Delete(dest);
				}
				using Image image = ScreenShoot(deviceId, isDeleteImageAfterCapture: true, filename);
				Rectangle srcRect = new Rectangle(x, y, width, height);
				Bitmap bitmap = new Bitmap(srcRect.Width, srcRect.Height);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), srcRect, GraphicsUnit.Pixel);
				}
				bitmap.Save(dest);
				return bitmap;
			}
			catch (Exception)
			{
			}
			return null;
		}

		public static void PasteClipboard(string SerialNo)
		{
			try
			{
				try
				{
					ExecuteCMD(SerialNo, " shell input keyevent 279 ");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void DisplayPhone(string deviceId, string title, int x, int y, int w, int h)
		{
			new Task(delegate
			{
				string cmd = $"scrcpy --serial \"{deviceId}\" --window-title \"{title}\" --window-y {y + 80} --window-x {x + 5} --window-width {w} --window-height {h} --max-size 1024 --bit-rate 4M --max-fps 15";
				RunCommand(cmd);
			}).Start();
		}

		public static void SetClipboard(string SerialNo, string str)
		{
			try
			{
				try
				{
					ExecuteCMD(SerialNo, " shell am startservice -a eu.micer.ClipboardService -e text \"" + str + "\"");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static object TextToBase64(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return Convert.ToBase64String(bytes);
		}

		public static void InputTextNormal(string SerialNo, string str)
		{
			try
			{
				try
				{
					ExecuteCMD(SerialNo, " shell ime set com.android.adbkeyboard/.AdbIME");
					Thread.Sleep(1000);
					ExecuteCMD(SerialNo, " shell am broadcast -a  ADB_INPUT_TEXT --es msg '" + str + "'");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void EnableKey(string SerialNo)
		{
			ExecuteCMD(SerialNo, " shell ime set com.android.adbkeyboard/.AdbIME");
		}

		public static void InputText(string SerialNo, string str)
		{
			try
			{
				try
				{
					ExecuteCMD(SerialNo, " shell ime set com.android.adbkeyboard/.AdbIME");
					string text = Convert.ToString(TextToBase64(str));
					ExecuteCMD(SerialNo, " shell am broadcast -a ADB_INPUT_B64 --es msg '" + text + "'");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void InputTextUnicode(string IDDevice, string str)
		{
			try
			{
				try
				{
					ExecuteCMD(IDDevice, "shell am broadcast -a ADB_INPUT_TEXT --es msg '" + str + "'");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static bool OpenLink(string IDDevice, string Link, string app = "")
		{
			bool result = false;
			try
			{
				bool flag;
				try
				{
					string cmd = "shell am start -a android.intent.action.VIEW -d \"" + Link.Replace("&", "\\&") + " " + CaChuaConstant.PACKAGE_NAME;
					ExecuteCMD(IDDevice, cmd);
					Thread.Sleep(2000);
					flag = true;
				}
				catch (Exception)
				{
					flag = false;
				}
				result = flag;
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool OpenLinkWeb(string IDDevice, string Link)
		{
			bool result = false;
			try
			{
				bool flag;
				try
				{
					string cmd = "shell am start -a android.intent.action.VIEW -d \"" + Link.Replace("&", "\\&");
					ExecuteCMD(IDDevice, cmd);
					Thread.Sleep(2000);
					flag = true;
				}
				catch (Exception)
				{
					flag = false;
				}
				result = flag;
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static void OpenFacebookLite(string index, string LDplayerPath)
		{
			try
			{
				try
				{
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = LDplayerPath + "\\dnconsole.exe",
						Arguments = "runapp --index " + index + " --packagename com.facebook.lite",
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void OpenApp(string index, string AppPackage)
		{
			ExecuteCMD(index, $"shell monkey -p '{AppPackage}' -c android.intent.category.LAUNCHER 1");
			Task.Run(delegate
			{
				SetPortrait(index);
			});
		}

		public static void OpenApp(string index, List<string> AppPackage)
		{
			foreach (string item in AppPackage)
			{
				ExecuteCMD(index, $"shell monkey -p '{item}' -c android.intent.category.LAUNCHER 1");
			}
			Task.Run(delegate
			{
				SetPortrait(index);
			});
		}

		public static void CloseApp(string index, string AppPackage)
		{
			ExecuteCMD(index, "shell am force-stop " + AppPackage);
			Task.Run(delegate
			{
				SetPortrait(index);
			});
		}

		public static void CloseApp(string index, List<string> AppPackage)
		{
			foreach (string item in AppPackage)
			{
				ExecuteCMD(index, "shell am force-stop " + item);
			}
			Task.Run(delegate
			{
				SetPortrait(index);
			});
		}

		public static void KillFacebookLite(string index)
		{
			try
			{
				try
				{
					ExecuteCMD(index, "shell am force-stop com.facebook.lite");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void StopApp(string SerialNo)
		{
			try
			{
				try
				{
					ExecuteCMD(SerialNo, " shell am force-stop com.facebook.katana");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void StopApp(string SerialNo, string app)
		{
			try
			{
				try
				{
					ExecuteCMD(SerialNo, " shell am force-stop " + app);
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void ClearDataFacebookApp(string SerialNo)
		{
			try
			{
				try
				{
					ExecuteCMD(SerialNo, $"shell pm clear {CaChuaConstant.PACKAGE_NAME}");
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static void ClearDataFacebookLite(string index)
		{
			try
			{
				try
				{
					ExecuteCMD(index, string.Format("shell pm clear {0}", "com.facebook.lite"));
				}
				catch (Exception)
				{
				}
			}
			catch (Exception)
			{
			}
		}

		public static Point GetScreenResolution(string IDDevice)
		{
			Point result = default(Point);
			try
			{
				Point point = default(Point);
				try
				{
					string text = ExecuteCMD(IDDevice, " shell dumpsys display");
					string[] array = text.Split('\r');
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (string.Compare(text2, "", ignoreCase: false) != -1 && text2.Contains("mCurrentDisplayRect=Rect"))
						{
							string[] array3 = text2.Replace("mCurrentDisplayRect=Rect", "").Replace("(", "").Replace(")", "")
								.Split('-');
							string[] array4 = array3[1].Split(',');
							int x = Convert.ToInt32(array4[0].Trim());
							int y = Convert.ToInt32(array4[1].Trim());
							point = new Point(x, y);
						}
					}
				}
				catch (Exception)
				{
					point = new Point(0, 0);
				}
				result = point;
				return result;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static void SwipeByPercent(string IDdevice, double x1, double y1, double x2, double y2, int duration = 100)
		{
			checked
			{
				try
				{
					Point screenResolution = GetScreenResolution(IDdevice);
					int num = (int)Math.Round(x1 * ((double)screenResolution.X * 1.0 / 100.0));
					int num2 = (int)Math.Round(y1 * ((double)screenResolution.Y * 1.0 / 100.0));
					int num3 = (int)Math.Round(x2 * ((double)screenResolution.X * 1.0 / 100.0));
					int num4 = (int)Math.Round(y2 * ((double)screenResolution.Y * 1.0 / 100.0));
					string cmd = $"shell input swipe {num} {num2} {num3} {num4} {duration}";
					ExecuteCMD(IDdevice, cmd);
				}
				catch (Exception)
				{
				}
			}
		}

		public static void Swipe(string IDdevice, int x1, int y1, int x2, int y2, int duration = 100)
		{
			if (duration > 500)
			{
				duration = 500;
			}
			try
			{
				string cmd = $"shell input swipe {x1} {y1} {x2} {y2} {duration}";
				ExecuteCMD(IDdevice, cmd);
			}
			catch (Exception)
			{
			}
		}

		public static string DumpXMLSource(string deviceID)
		{
			ExecuteCMD(deviceID, " shell uiautomator dump /data/local/tmp/uidump.xml");
			ExecuteCMD(deviceID, " pull /data/local/tmp/uidump.xml LDPlayer" + deviceID + ".xml");
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(File.ReadAllText("LDPlayer" + deviceID + ".xml"));
				File.Delete("LDPlayer" + deviceID + ".xml");
			}
			catch (Exception)
			{
			}
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlDocument.WriteContentTo(xmlTextWriter);
			xmlTextWriter.Flush();
			memoryStream.Flush();
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			string result = streamReader.ReadToEnd();
			memoryStream.Close();
			xmlTextWriter.Close();
			return result;
		}

		public static string DumpXml(string IDDevice, bool isDeleteImageAfterCapture = true, string fileName = "dump.xml")
		{
			string result = null;
			try
			{
				string text = Path.GetFileNameWithoutExtension(fileName) + IDDevice + Path.GetExtension(fileName);
				string path = Application.StartupPath + "\\" + text;
				AddQuotesIfRequired(Application.StartupPath);
				while (File.Exists(path))
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception)
					{
					}
				}
				while (true)
				{
					string text2 = ExecuteCMD(IDDevice, string.Format("shell uiautomator dump \"{0}\"", "/sdcard/" + text));
					if (!text2.Contains("UI hierchary dumped to"))
					{
					}
					text2 = ExecuteCMD(IDDevice, string.Format("pull \"{0}\" \"{1}\"", "/sdcard/" + text, Application.StartupPath));
					if (!text2.Contains("failed to stat remote"))
					{
						text2 = ExecuteCMD(IDDevice, string.Format("shell rm -f \"{0}\"", "/sdcard/" + text));
						if (File.Exists(text))
						{
							break;
						}
					}
				}
				try
				{
					result = Utils.ReadTextFile(text);
				}
				catch
				{
				}
				if (isDeleteImageAfterCapture)
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception)
					{
					}
				}
				return result;
			}
			catch (Exception ex3)
			{
				WriteLog("ScreenShoot : LDPlayer-" + IDDevice, ex3.Message);
			}
			GC.Collect();
			return result;
		}

		public static string DumpXmlFile(string SerialNo, string fileName = "dump.xml", bool captureScreen = false)
		{
			string text = null;
			try
			{
				string text2 = Path.GetFileNameWithoutExtension(fileName) + SerialNo + Path.GetExtension(fileName);
				text = Application.StartupPath + "\\Dump\\" + text2 + ".xml";
				AddQuotesIfRequired(Application.StartupPath);
				while (File.Exists(text))
				{
					try
					{
						File.Delete(text);
					}
					catch (Exception)
					{
					}
				}
				if (!Directory.Exists(Application.StartupPath + "\\Dump\\"))
				{
					Directory.CreateDirectory(Application.StartupPath + "\\Dump\\");
				}
				string text3 = ExecuteCMD(SerialNo, string.Format("shell uiautomator dump \"{0}\"", "/sdcard/" + text2));
				if (text3.Contains("hierchary dumped to"))
				{
					text3 = ExecuteCMD(SerialNo, string.Format("pull \"{0}\" \"{1}\"", "/sdcard/" + text2, text));
					if (text3.Contains("file pulled"))
					{
						ExecuteCMD(SerialNo, string.Format("shell rm -f \"{0}\"", "/sdcard/" + text2));
					}
				}
				if (captureScreen)
				{
					text3 = ExecuteCMD(SerialNo, "shell screencap -p /sdcard/screencap.png");
					text3 = ExecuteCMD(SerialNo, string.Format("pull \"{0}\" \"{1}\"", "/sdcard/screencap.png", text.Replace(".xml", ".png")));
					if (!text3.Contains("file pulled"))
					{
					}
				}
				return text;
			}
			catch (Exception ex2)
			{
				WriteLog("ScreenShoot : LDPlayer-" + SerialNo, ex2.Message);
			}
			return text;
		}

		public static string PrintXML(string xml)
		{
			string result = "";
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(xml);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlDocument.WriteContentTo(xmlTextWriter);
				xmlTextWriter.Flush();
				memoryStream.Flush();
				memoryStream.Position = 0L;
				StreamReader streamReader = new StreamReader(memoryStream);
				string text = streamReader.ReadToEnd();
				result = text;
			}
			catch (XmlException)
			{
			}
			memoryStream.Close();
			xmlTextWriter.Close();
			return result;
		}

		public static int[] GetCordinateElementXml(string SerialNo, string Value, bool isDeleteImageAfterCapture = true)
		{
			//IL_0041: Incompatible stack heights: 1 vs 0
			//IL_0044: Incompatible stack heights: 1 vs 0
			//IL_0062: Incompatible stack heights: 1 vs 0
			//IL_006d: Incompatible stack heights: 0 vs 1
			//IL_00e5: Incompatible stack heights: 1 vs 0
			int[] result = new int[0];
			string text = DumpXmlFile(SerialNo);
			string contents = PrintXML(Utils.ReadTextFile(text));
			File.WriteAllText(text, contents);
			_ = 1602080255;
			List<string> list = File.ReadAllLines(text).ToList();
			if (isDeleteImageAfterCapture)
			{
				try
				{
					File.Delete(text);
				}
				catch (Exception)
				{
				}
			}
			foreach (string item in list)
			{
				if (item.Contains(Value))
				{
					int num = item.IndexOf("bounds");
					item.IndexOf("/>");
					string input = item.Substring(num + 7).Replace("/>", "").Trim();
					result = (from m in Regex.Matches(input, "\\d+").OfType<Match>()
						select int.Parse(m.Value)).ToArray();
					break;
				}
			}
			return result;
		}

		public static List<IWebElement> FindElements(RemoteWebDriver driver, By by)
		{
			try
			{
				return driver.FindElements(by).ToList();
			}
			catch
			{
				return null;
			}
		}

		public static void FBNavigateUrl(string deviceId, string url)
		{
			ExecuteCMD(deviceId, $"shell am start -n com.facebook.katana/.IntentUriHandler \"{url}\"");
		}

		public static bool TurnOnAirplane(string deviceId, RemoteWebDriver driver)
		{
			if (driver == null)
			{
				return false;
			}
			bool result = false;
			try
			{
				if (IsRooted(deviceId))
				{
					string cmd = " shell settings put global airplane_mode_on 1";
					string text = ExecuteCMD(deviceId, cmd);
					Thread.Sleep(1000);
					cmd = "  shell am broadcast -a android.intent.action.AIRPLANE_MODE --ez state true";
					text = ExecuteCMD(deviceId, cmd);
					if (!string.IsNullOrEmpty(text))
					{
						return true;
					}
				}
				else
				{
					ExecuteCMD(deviceId, "shell am start -a android.settings.AIRPLANE_MODE_SETTINGS");
					Thread.Sleep(2000);
					int num = 0;
					while (driver.PageSource.ToLower().Contains("text=\"OFF\"".ToLower()) && num++ < 10)
					{
						ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(By.XPath("//*[upper-case(@text)=\"OFF\"]"));
						if (readOnlyCollection == null || readOnlyCollection.Count <= 0)
						{
							Thread.Sleep(1000);
							continue;
						}
						readOnlyCollection[readOnlyCollection.Count - 1].Click();
						Thread.Sleep(5000);
						break;
					}
					Thread.Sleep(1000);
					while (driver.PageSource.ToLower().Contains("text=\"ON\"".ToLower()) && num++ < 10)
					{
						ReadOnlyCollection<IWebElement> readOnlyCollection2 = driver.FindElements(By.XPath("//*[upper-case(@text)=\"ON\"]"));
						if (readOnlyCollection2 == null || readOnlyCollection2.Count <= 0)
						{
							Thread.Sleep(1000);
							continue;
						}
						readOnlyCollection2[readOnlyCollection2.Count - 1].Click();
						Thread.Sleep(1000);
						break;
					}
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static bool TurnOffAirplane(string deviceId)
		{
			bool result = false;
			try
			{
				int num = 10;
				bool flag = IS_AIRPLANE_MODE_ON(deviceId);
				while (true)
				{
					if (flag)
					{
						if (num-- > 0)
						{
							Thread.Sleep(500);
							string cmd = " shell settings put global airplane_mode_on 1";
							string text = ExecuteCMD(deviceId, cmd);
							Thread.Sleep(500);
							cmd = " shell settings put global airplane_mode_on 0";
							text = ExecuteCMD(deviceId, cmd);
							Thread.Sleep(500);
							cmd = "  shell am broadcast -a android.intent.action.AIRPLANE_MODE --ez state false";
							text = ExecuteCMD(deviceId, cmd);
							if (!string.IsNullOrEmpty(text))
							{
								result = true;
							}
							Thread.Sleep(500);
							flag = IS_AIRPLANE_MODE_ON(deviceId);
							continue;
						}
						break;
					}
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool AIRPLANE_MODE_SETTINGS(string deviceId)
		{
			bool result = false;
			try
			{
				string cmd = " shell am start -a android.settings.AIRPLANE_MODE_SETTINGS";
				string text = ExecuteCMD(deviceId, cmd);
				if (text.Contains("AIRPLANE_MODE_SETTINGS"))
				{
					result = true;
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool IS_AIRPLANE_MODE_ON(string deviceId)
		{
			bool result = false;
			try
			{
				string cmd = " shell settings get global airplane_mode_on";
				string text = ExecuteCMD(deviceId, cmd);
				text = text.Trim();
				if (text.Contains("1"))
				{
					result = true;
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static bool SENDKEY(string deviceId, string cmd)
		{
			bool result = false;
			try
			{
				cmd = $"shell input keyevent {cmd}";
				ExecuteCMD(deviceId, cmd);
				return true;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static void Shutdown()
		{
			Process.Start("shutdown.exe", "-s -f -t 05");
		}

		private static string GetPageSource(RemoteWebDriver driver, DeviceEntity entity)
		{
			try
			{
				return driver.PageSource;
			}
			catch (Exception ex)
			{
				if (ex.Message.ToLower().Contains("session"))
				{
					Reconnect2Device(entity.DeviceId);
				}
				for (int i = 0; i < 5; i++)
				{
					try
					{
						driver = StartPhone(entity);
						if (driver == null)
						{
							continue;
						}
						return driver.PageSource;
					}
					catch
					{
					}
				}
				return "";
			}
		}

		public static void StartInstallRoot(DeviceEntity device)
		{
			string deviceId = device.DeviceId;
			RemoteWebDriver remoteWebDriver = IsReady(device);
			if (remoteWebDriver == null)
			{
				return;
			}
			BackToHome(deviceId);
			OpenApp(deviceId, "com.topjohnwu.magisk");
			Thread.Sleep(2000);
			string pageSource = GetPageSource(remoteWebDriver, device);
			if (!pageSource.Contains("content-desc=\"Modules\""))
			{
				return;
			}
			List<string> list = new List<string> { "Riru.zip", "Riru-edXposed.zip" };
			if (Utils.Convert2Int(GetAndroidVersion(device.DeviceId)) > 8)
			{
				list.Insert(0, "adb_root.zip");
			}
			for (int i = 0; i < list.Count; i++)
			{
				IWebElement webElement = AppGetObject(By.XPath("//*[@content-desc=\"Modules\"]"), remoteWebDriver);
				if (webElement == null)
				{
					continue;
				}
				webElement.Click();
				Thread.Sleep(2000);
				Swipe(deviceId, 200, 100, 200, 800);
				pageSource = remoteWebDriver.PageSource;
				if (!pageSource.Contains("Install from storage"))
				{
					continue;
				}
				IWebElement webElement2 = AppGetObject(By.XPath("//*[@text=\"Install from storage\"]"), remoteWebDriver);
				if (webElement2 == null)
				{
					continue;
				}
				webElement2.Click();
				Thread.Sleep(2000);
				if (!remoteWebDriver.PageSource.Contains("content-desc=\"Search\""))
				{
					continue;
				}
				webElement2 = AppGetObject(By.XPath("//*[@content-desc=\"Search\"]"), remoteWebDriver);
				if (webElement2 == null)
				{
					continue;
				}
				webElement2.Click();
				Thread.Sleep(1000);
				pageSource = remoteWebDriver.PageSource;
				if (!pageSource.Contains("text=\"Search…\""))
				{
					continue;
				}
				webElement2 = AppGetObject(By.XPath("//*[@text=\"Search…\"]"), remoteWebDriver);
				if (webElement2 == null)
				{
					continue;
				}
				webElement2.SendKeys(list[i]);
				Thread.Sleep(2000);
				pageSource = remoteWebDriver.PageSource;
				List<IWebElement> list2 = AppGetObjects(By.XPath($"//*[@text=\"{list[i]}\"]"), remoteWebDriver);
				if (list2 == null)
				{
					continue;
				}
				list2[list2.Count - 1].Click();
				int num = 0;
				while (!remoteWebDriver.PageSource.Contains("text=\"Done!\"") && num < 10)
				{
					Thread.Sleep(2000);
					num++;
				}
				Thread.Sleep(2000);
				if (i >= list.Count - 1)
				{
					list2 = AppGetObjects(By.XPath(string.Format("//*[@text=\"{0}\"]", "Reboot")), remoteWebDriver);
					if (list2 != null && list2.Count > 0)
					{
						list2[list2.Count - 1].Click();
					}
				}
				else
				{
					Back(deviceId);
				}
			}
		}

		public static RemoteWebDriver IsReady(DeviceEntity device)
		{
			string deviceId = device.DeviceId;
			device.Version = GetAndroidVersion(deviceId);
			RemoteWebDriver remoteWebDriver = null;
			int num = 0;
			while (remoteWebDriver == null && num < 30)
			{
				remoteWebDriver = StartPhone(device);
				if (remoteWebDriver != null)
				{
					break;
				}
				num++;
				Thread.Sleep(2000);
			}
			return remoteWebDriver;
		}

		public static bool OnOffModule(string SerialNo, bool state)
		{
			if (SerialNo.Contains("emulator"))
			{
				return true;
			}
			CloseApp(SerialNo, "org.meowcat.edxposed.manager");
			Thread.Sleep(500);
			EnableModules(SerialNo);
			Thread.Sleep(500);
			OpenApp(SerialNo, "org.meowcat.edxposed.manager");
			Thread.Sleep(1000);
			try
			{
				DeviceEntity deviceEntity = new DeviceEntity
				{
					DeviceId = SerialNo,
					Name = SerialNo,
					Version = GetAndroidVersion(SerialNo),
					Port = 4723,
					SystemPort = 8201
				};
				AndroidDriver<AndroidElement> driver = StartPhone(deviceEntity);
				string text = "";
				int num = 0;
				bool flag = false;
				while (num < 5)
				{
					text = GetPageSource(driver, deviceEntity);
					int num2 = 5;
					while (text == "" && num2-- > 0)
					{
						Reconnect2Device(deviceEntity.DeviceId);
						text = GetPageSource(driver, deviceEntity);
						Thread.Sleep(500);
					}
					if (text.Contains("android:id/checkbox"))
					{
						IWebElement webElement = AppGetObject(By.XPath("//*[@resource-id=\"android:id/checkbox\"]"), driver);
						if (webElement != null)
						{
							Thread.Sleep(500);
							webElement.Click();
						}
						text = GetPageSource(driver, deviceEntity);
					}
					num++;
					Thread.Sleep(500);
					if (text.Contains("org.meowcat.edxposed.manager:id/md_buttonDefaultPositive"))
					{
						List<IWebElement> list = FindElements(driver, By.Id("org.meowcat.edxposed.manager:id/md_buttonDefaultPositive"));
						if (list != null)
						{
							list[0].Click();
							Thread.Sleep(1000);
							text = GetPageSource(driver, deviceEntity);
						}
					}
					if (!text.Contains("EdXposed Framework is not (properly) installed"))
					{
						if (text.Contains("Open navigation drawer"))
						{
							List<IWebElement> list2 = AppGetObjects(By.XPath("//*[@content-desc=\"Open navigation drawer\"]"), driver);
							if (list2 != null && list2.Count > 0)
							{
								flag = true;
								list2[0].Click();
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
					List<IWebElement> list3 = FindElements(driver, By.XPath("//android.widget.CheckedTextView[@text=\"Modules\"]"));
					if (list3 != null && list3.Count > 0)
					{
						list3[0].Click();
						Thread.Sleep(1000);
						List<IWebElement> list4 = FindElements(driver, By.XPath("//android.widget.Switch[@resource-id=\"org.meowcat.edxposed.manager:id/checkbox\"]"));
						foreach (IWebElement item in list4)
						{
							if (state && item.GetAttribute("text").ToUpper().Equals("ON"))
							{
								item.Click();
								Thread.Sleep(1000);
								if (item.GetAttribute("text").ToUpper().Equals("OFF"))
								{
									item.Click();
									Thread.Sleep(1000);
								}
							}
							if (item.GetAttribute("text").ToUpper().Equals(state ? "OFF" : "ON"))
							{
								item.Click();
								Thread.Sleep(200);
							}
						}
					}
				}
				if (!state)
				{
					CloseApp(SerialNo, "org.meowcat.edxposed.manager");
				}
				Thread.Sleep(500);
			}
			catch (Exception ex)
			{
				File.WriteAllText(CaChuaConstant.LOG_ACTION, "org.meowcat.edxposed.manager: " + SerialNo + ": " + ex.Message);
			}
			return true;
		}

		public static bool OnOffModule(string SerialNo, bool state, int time)
		{
			if (SerialNo.Contains("emulator"))
			{
				return true;
			}
			CloseApp(SerialNo, "org.meowcat.edxposed.manager");
			Thread.Sleep(500);
			EnableModules(SerialNo);
			Thread.Sleep(500);
			OpenApp(SerialNo, "org.meowcat.edxposed.manager");
			Thread.Sleep(1000);
			try
			{
				DeviceEntity deviceEntity = new DeviceEntity
				{
					DeviceId = SerialNo,
					Name = SerialNo,
					Version = GetAndroidVersion(SerialNo),
					Port = 4723,
					SystemPort = 8201
				};
				AndroidDriver<AndroidElement> driver = StartPhone(deviceEntity);
				string text = "";
				int num = 0;
				bool flag = false;
				while (num < 5)
				{
					text = GetPageSource(driver, deviceEntity);
					int num2 = 5;
					while (text == "" && num2-- > 0)
					{
						Reconnect2Device(deviceEntity.DeviceId);
						text = GetPageSource(driver, deviceEntity);
						Thread.Sleep(500);
					}
					if (text.Contains("android:id/checkbox"))
					{
						IWebElement webElement = AppGetObject(By.XPath("//*[@resource-id=\"android:id/checkbox\"]"), driver);
						if (webElement != null)
						{
							Thread.Sleep(500);
							webElement.Click();
						}
						text = GetPageSource(driver, deviceEntity);
					}
					num++;
					Thread.Sleep(500);
					if (text.Contains("org.meowcat.edxposed.manager:id/md_buttonDefaultPositive"))
					{
						List<IWebElement> list = FindElements(driver, By.Id("org.meowcat.edxposed.manager:id/md_buttonDefaultPositive"));
						if (list != null)
						{
							list[0].Click();
							Thread.Sleep(1000);
							text = GetPageSource(driver, deviceEntity);
						}
					}
					if (!text.Contains("EdXposed Framework is not (properly) installed"))
					{
						if (text.Contains("Open navigation drawer"))
						{
							List<IWebElement> list2 = AppGetObjects(By.XPath("//*[@content-desc=\"Open navigation drawer\"]"), driver);
							if (list2 != null && list2.Count > 0)
							{
								flag = true;
								list2[0].Click();
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
					List<IWebElement> list3 = FindElements(driver, By.XPath("//android.widget.CheckedTextView[@text=\"Modules\"]"));
					if (list3 != null && list3.Count > 0)
					{
						list3[0].Click();
						Thread.Sleep(1000);
						List<IWebElement> list4 = FindElements(driver, By.XPath("//android.widget.Switch[@resource-id=\"org.meowcat.edxposed.manager:id/checkbox\"]"));
						foreach (IWebElement item in list4)
						{
							for (int i = 0; i < time; i++)
							{
								if (state && item.GetAttribute("text").ToUpper().Equals("ON"))
								{
									item.Click();
									Thread.Sleep(1000);
									if (item.GetAttribute("text").ToUpper().Equals("OFF"))
									{
										item.Click();
										Thread.Sleep(1000);
									}
								}
								if (item.GetAttribute("text").ToUpper().Equals(state ? "OFF" : "ON"))
								{
									item.Click();
									Thread.Sleep(200);
								}
							}
						}
						List<IWebElement> list5 = AppGetObjects(By.XPath("//*[@content-desc=\"Open navigation drawer\"]"), driver);
						if (list5 != null && list5.Count > 0)
						{
							list5[0].Click();
							Thread.Sleep(1000);
							list5 = AppGetObjects(By.XPath("//*[@content-desc=\"EdXposed\" or @text=\"EdXposed\"]"), driver);
							if (list5 != null && list5.Count > 0)
							{
								list5[0].Click();
								Thread.Sleep(2000);
								list5 = AppGetObjects(By.XPath("//android.widget.TextView[@content-desc=\"Reboot…\"]"), driver);
								if (list5 != null && list5.Count > 0)
								{
									list5[0].Click();
									Thread.Sleep(1000);
									list5 = AppGetObjects(By.XPath("//android.widget.TextView[@resource-id=\"org.meowcat.edxposed.manager:id/title\"]"), driver);
									if (list5 != null && list5.Count > 0)
									{
										list5[0].Click();
										Thread.Sleep(1000);
										list5 = AppGetObjects(By.XPath("//*[@text=\"OK\"]"), driver);
										if (list5 != null && list5.Count > 0)
										{
											list5[0].Click();
											Thread.Sleep(1000);
										}
									}
								}
							}
						}
					}
				}
				if (!state)
				{
					CloseApp(SerialNo, "org.meowcat.edxposed.manager");
				}
				Thread.Sleep(500);
			}
			catch (Exception ex)
			{
				File.WriteAllText(CaChuaConstant.LOG_ACTION, "org.meowcat.edxposed.manager: " + SerialNo + ": " + ex.Message);
			}
			return true;
		}

		public static string GetAndroidVersion(string SerialNo)
		{
			string text = string.Empty;
			try
			{
				string cmd = " shell getprop ro.build.version.release ";
				text = ExecuteCMD(SerialNo, cmd);
				text = text.Trim();
				if (string.IsNullOrEmpty(text))
				{
					text = "9";
				}
			}
			catch (Exception)
			{
			}
			return text.Split('.')[0];
		}

		public static string GetAndroidVersionString(string SerialNo)
		{
			string text = string.Empty;
			try
			{
				string cmd = " shell getprop ro.build.version.release ";
				text = ExecuteCMD(SerialNo, cmd);
				text = text.Trim();
				if (string.IsNullOrEmpty(text))
				{
					text = "9";
				}
			}
			catch (Exception)
			{
			}
			return text;
		}

		public static void EnableModules(string SerialNo)
		{
			string contents = "<?xml version='1.0' encoding='utf-8' standalone='yes' ?><map><int name=\"com.cck.support\" value=\"1\" /></map>";
			if (!File.Exists(Application.StartupPath + "\\enabled_modules_cck.xml"))
			{
				File.WriteAllText(Application.StartupPath + "\\enabled_modules_cck.xml", contents);
			}
			string text = ExecuteCMD(SerialNo, " push \"" + Application.StartupPath + "\\enabled_modules_cck.xml\" \"/data/data/org.meowcat.edxposed.manager/shared_prefs/enabled_modules.xml\"");
			if (text.Contains("file pushed"))
			{
			}
		}

		internal static void DeleteInfoDevice(string p_DeviceId)
		{
			string path = Application.StartupPath + "\\Devices\\CCKInfo_" + NormalizeDeviceName(p_DeviceId) + ".txt";
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static bool SCROLL(string deviceId, int left, int top, int width, int height, int delay = -1)
		{
			bool result = false;
			try
			{
				string cmd = $"adb -s {deviceId} shell input swipe {left} {top} {width} {height}";
				if (delay > 0)
				{
					cmd = $"adb -s {deviceId} shell input swipe {left} {top} {width} {height} {delay}";
				}
				RunCommand(cmd);
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static void SetPortrait(string deviceId)
		{
			new Task(delegate
			{
				ExecuteCMD(deviceId, " shell settings put system accelerometer_rotation 0");
			}).Start();
		}

		public static bool IsRooted(string SerialNo)
		{
			string text = Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA);
			if (text == "")
			{
				return false;
			}
			PhoneMode phoneMode = new JavaScriptSerializer().Deserialize<PhoneMode>(Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA));
			string text2 = ExecuteCMD(SerialNo, "root");
			return text2.Contains("adbd is already running as root") && phoneMode == PhoneMode.Root;
		}

		public static TTItems GetKatanaCookieOnDevice(string DeviceId)
		{
			TTItems tTItems = new TTItems();
			try
			{
				string cmd = "shell su -c \"cat /data/data/com.facebook.katana/app_light_prefs/com.facebook.katana/authentication\"";
				string input = ExecuteCMD(DeviceId, cmd);
				Regex regex = new Regex("access_token(.*?)analytics_claim");
				Match match = regex.Match(input);
				if (match.Success)
				{
					match.Groups[1].Value.Trim();
					string[] array = match.Groups[1].Value.Split("\u0005\0\u000f\u0003".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					string text = "";
					char[] array2 = array[0].ToCharArray();
					for (int i = 0; i < array2.Length; i++)
					{
						char value = array2[i];
						if (Utils.validChar.Contains(value))
						{
							text += value;
						}
					}
					tTItems.Token = text;
					if (array.Length > 2)
					{
						tTItems.Uid = array[2];
					}
					if (array.Length > 5)
					{
						string text2 = array[5].Substring(1);
						if (!text2.Contains("c_user"))
						{
							text2 = array[4].Substring(1);
						}
						dynamic val = new JavaScriptSerializer().DeserializeObject(text2);
						List<string> list = new List<string>();
						for (int j = 0; j < val.Length; j++)
						{
							dynamic val2 = val[j]["name"];
							dynamic val3 = val[j]["value"];
							list.Add(val2 + "=" + val3);
						}
						tTItems.Cookie = string.Join("; ", list);
					}
				}
			}
			catch
			{
			}
			return tTItems;
		}

		public static IWebElement GetObjectByText(string text, RemoteWebDriver driver, bool retry = true)
		{
			return AppGetObject(By.XPath(string.Format("//*[lower-case(@text)=\"{0}\" or lower-case(@content-desc)=\"{0}\")]", text.ToLower())), driver, retry);
		}

		public static List<IWebElement> GetObjectsByText(string text, RemoteWebDriver driver, bool retry = true)
		{
			return AppGetObjects(By.XPath(string.Format("//*[lower-case(@text)=\"{0}\" or lower-case(@content-desc)=\"{0}\")]", text.ToLower())), driver);
		}

		public static IWebElement AppGetObject(string xpath, RemoteWebDriver driver, bool retry = true)
		{
			return AppGetObject(By.XPath(xpath), driver, retry);
		}

		public static IWebElement AppGetObject(By by, RemoteWebDriver driver, bool retry = true)
		{
			try
			{
				ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(by);
				if (readOnlyCollection != null && readOnlyCollection.Count > 0)
				{
					return readOnlyCollection[0];
				}
				return null;
			}
			catch (Exception)
			{
				DeviceEntity deviceEntity = new DeviceEntity
				{
					DeviceId = driver.Capabilities.GetCapability("deviceName").ToString(),
					Name = driver.Capabilities.GetCapability("deviceName").ToString(),
					SystemPort = Utils.Convert2Int(driver.Capabilities.GetCapability("systemPort").ToString()),
					Port = Utils.Convert2Int(driver.Capabilities.GetCapability("devicePort").ToString())
				};
				_ = deviceEntity.Name;
				driver = StartPhone(deviceEntity);
				if (driver == null)
				{
					return null;
				}
				return AppGetObject(by, driver, retry: false);
			}
		}

		public static IWebElement AppFindAndGetObjectDown(string xpath, RemoteWebDriver driver, string deviceId, int tryfindcount = 3)
		{
			By by = By.XPath(xpath);
			try
			{
				int i = 0;
				Point point = default(Point);
				for (; i < tryfindcount; i++)
				{
					ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(by);
					if (readOnlyCollection == null || readOnlyCollection.Count <= 0)
					{
						point = GetScreenResolution(deviceId);
						Swipe(deviceId, point.X / 2, point.Y * 3 / 5, point.X / 2, point.Y / 2, 200);
						continue;
					}
					return readOnlyCollection[0];
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static List<IWebElement> AppFindAndGetObjectsDown(string xpath, RemoteWebDriver driver, string deviceId, int tryfindcount = 3)
		{
			By by = By.XPath(xpath);
			try
			{
				int i = 0;
				Point point = default(Point);
				for (; i < tryfindcount; i++)
				{
					ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(by);
					if (readOnlyCollection == null || readOnlyCollection.Count <= 0)
					{
						point = GetScreenResolution(deviceId);
						Swipe(deviceId, point.X / 2, point.Y * 3 / 5, point.X / 2, point.Y / 2, 200);
						continue;
					}
					return readOnlyCollection.ToList();
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static IWebElement AppFindLogout(By by, RemoteWebDriver driver, string deviceId)
		{
			try
			{
				int i = 0;
				Point point = default(Point);
				for (; i < 3; i++)
				{
					ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(by);
					if (readOnlyCollection == null || readOnlyCollection.Count <= 0)
					{
						point = GetScreenResolution(deviceId);
						Swipe(deviceId, point.X / 10, point.Y * 2 / 3, point.X / 10, point.Y / 3);
						continue;
					}
					return readOnlyCollection[0];
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static bool FindImage(string index, Bitmap LinkIMG, int exitwhile, bool capx = true, bool tap = true, int x = 0, int y = 0)
		{
			int num = 0;
			while (true)
			{
				try
				{
					using Bitmap mainBitmap = ScreenShoot(index);
					Point? point = ImageScanOpenCV.FindOutPoint(mainBitmap, LinkIMG, 0.75);
					if (point.HasValue)
					{
						if (tap)
						{
							Tap(index, point.Value.X + x, point.Value.Y + y);
						}
						return true;
					}
					num++;
					if (num == exitwhile)
					{
						return false;
					}
					Thread.Sleep(1500);
				}
				catch (Exception ex)
				{
					Utils.CCKLog("FindImage", ex.Message + Environment.NewLine + ex.InnerException.Message);
				}
			}
		}

		public static Point FindImages(string source1, string source2, int exitwhile, bool capx = true)
		{
			int num = 0;
			while (true)
			{
				try
				{
					double percent = 0.965;
					List<Point> list = ImageScanOpenCV.FindOutPoints(source1, source2, percent);
					if (list != null && list.Count > 0)
					{
						return list[list.Count - 1];
					}
					num++;
					if (num >= exitwhile)
					{
						return new Point(0, 0);
					}
					Thread.Sleep(1500);
				}
				catch (Exception ex)
				{
					Utils.CCKLog("FindImage", ex.Message + Environment.NewLine);
				}
			}
		}

		public static bool KillProcessByPort(int port)
		{
			try
			{
				List<int> processIdByPort = GetProcessIdByPort(port);
				if (processIdByPort != null)
				{
					foreach (int item in processIdByPort)
					{
						try
						{
							Process.GetProcessById(item);
						}
						catch (Exception)
						{
							Utils.CCKLog("Kill Port" + port + " error", DateTime.Now.ToString());
						}
					}
					processIdByPort = GetProcessIdByPort(port);
					return true;
				}
				return false;
			}
			catch (ArgumentException)
			{
				Utils.CCKLog("Kill Port 2" + port + " error", DateTime.Now.ToString());
				return false;
			}
		}

		public static List<int> GetProcessIdByPort(int port)
		{
			List<int> list = new List<int>();
			string text = RunCommand($"netstat -a -n -o | find \"{port}\"").Trim();
			int num = 0;
			if (text != "")
			{
				string[] array = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					string[] array3 = text2.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (array3 != null && array3.Length == 5 && text2.Contains("LISTENING"))
					{
						num = Utils.Convert2Int(array3[4]);
						if (num > 0)
						{
							list.Add(num);
						}
					}
				}
			}
			return list.Distinct().ToList();
		}

		public static int GetAvailablePort(int startingPort)
		{
			List<int> list = new List<int>();
			IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			TcpConnectionInformation[] activeTcpConnections = iPGlobalProperties.GetActiveTcpConnections();
			list.AddRange(from n in activeTcpConnections
				where n.LocalEndPoint.Port >= startingPort
				select n.LocalEndPoint.Port);
			IPEndPoint[] activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
			list.AddRange(from n in activeTcpListeners
				where n.Port >= startingPort
				select n.Port);
			activeTcpListeners = iPGlobalProperties.GetActiveUdpListeners();
			list.AddRange(from n in activeTcpListeners
				where n.Port >= startingPort
				select n.Port);
			list.Sort();
			int num = startingPort;
			while (true)
			{
				if (num < 65535)
				{
					if (!list.Contains(num))
					{
						break;
					}
					num++;
					continue;
				}
				return 0;
			}
			return num;
		}

		public static AndroidDriver<AndroidElement> StartPhone(DeviceEntity device)
		{
			SetPortrait(device.DeviceId);
			AndroidDriver<AndroidElement> androidDriver;
			while (true)
			{
				androidDriver = null;
				AppiumOptions appiumOptions = new AppiumOptions();
				appiumOptions.AddAdditionalCapability("platformName", "Android");
				appiumOptions.AddAdditionalCapability("automationName", "UiAutomator2");
				appiumOptions.AddAdditionalCapability("uiautomator2ServerInstallTimeout", 180000);
				appiumOptions.AddAdditionalCapability("platformVersion", GetAndroidVersionString(device.DeviceId));
				appiumOptions.AddAdditionalCapability("deviceName", device.DeviceId);
				appiumOptions.AddAdditionalCapability("unicodeKeyboard", true);
				appiumOptions.AddAdditionalCapability("systemPort", device.SystemPort);
				appiumOptions.AddAdditionalCapability("udid", device.DeviceId);
				appiumOptions.AddAdditionalCapability("devicePort", device.Port);
				appiumOptions.AddAdditionalCapability("overrideSession", true);
				appiumOptions.AddAdditionalCapability("noReset", true);
				appiumOptions.AddAdditionalCapability("disableWindowAnimation", true);
				appiumOptions.AddAdditionalCapability("skipServerInstallation", false);
				appiumOptions.AddAdditionalCapability("skipDeviceInitialization", false);
				appiumOptions.AddAdditionalCapability("skipLogcatCapture", true);
				appiumOptions.AddAdditionalCapability("dontStopAppOnReset", true);
				appiumOptions.AddAdditionalCapability("ignoreHiddenApiPolicyError", true);
				appiumOptions.AddAdditionalCapability("noSign", false);
				appiumOptions.AddAdditionalCapability("newCommandTimeout", 0);
				try
				{
					List<string> allPortListeningOnComputer = GetAllPortListeningOnComputer();
					int num = 0;
					while (num < 5 && !allPortListeningOnComputer.Contains(device.Port.ToString()))
					{
						num++;
						if (!allPortListeningOnComputer.Contains(device.Port.ToString()))
						{
							AppiumInfo appiumInfo = START_APPIUM_SERVER(device.DeviceId, device.Port);
							if (appiumInfo.is_started)
							{
								allPortListeningOnComputer = GetAllPortListeningOnComputer();
								break;
							}
						}
						Thread.Sleep(1000);
					}
					if (!allPortListeningOnComputer.Contains(device.Port.ToString()))
					{
						break;
					}
					try
					{
						androidDriver = new AndroidDriver<AndroidElement>(new Uri($"http://127.0.0.1:{device.Port}/wd/hub"), appiumOptions);
					}
					catch (Exception ex)
					{
						Utils.CCKLog(" Start Server: ", ex.Message + " -->" + device.DeviceId);
						if (!ex.Message.Contains("device offline"))
						{
						}
						if (!ex.Message.Contains("connection could be made because the target machine actively refused") && !ex.Message.Contains("timed out after 60"))
						{
							if (ex.Message.Contains("was not in the list of connected devices"))
							{
								Thread.Sleep(5000);
								List<string> listSerialDevice = GetListSerialDevice();
								if (listSerialDevice.Contains(device.DeviceId))
								{
									listSerialDevice = GetListSerialDevice();
									if (listSerialDevice.Contains(device.DeviceId))
									{
										Utils.CCKLog(" Reconnect OK: ", Environment.NewLine + device.DeviceId);
										continue;
									}
									Utils.CCKLog(" Mất kết nối với điện thoại ", Environment.NewLine + device.DeviceId + " không còn trong danh sach thiết bị ");
									if (!Reconnect2Device(device.DeviceId))
									{
										continue;
									}
									Utils.CCKLog("Kết nối lại ", Environment.NewLine + device.DeviceId);
									bool flag = false;
									Thread.Sleep(2000);
									for (int i = 0; i < 10; i++)
									{
										androidDriver = StartPhone(device);
										if (androidDriver == null)
										{
											if (!Reconnect2Device(device.DeviceId))
											{
												Utils.CCKLog("Kết nối lại khong thành công  ", device.DeviceId + " - " + ExecuteCMD(device.DeviceId, " reconnect").ToString());
											}
											Thread.Sleep(5000);
											continue;
										}
										Utils.CCKLog("Kết nối lại thành công  ", Environment.NewLine + device.DeviceId);
										flag = true;
										return androidDriver;
									}
									if (!flag)
									{
										Utils.CCKLog("Kết nối lại lỗi rồi anh ơi ", Environment.NewLine + device.DeviceId);
									}
									continue;
								}
								if (Reconnect2Device(device.DeviceId))
								{
									Thread.Sleep(2000);
									for (int j = 0; j < 5; j++)
									{
										androidDriver = StartPhone(device);
										if (androidDriver == null)
										{
											Thread.Sleep(5000);
											continue;
										}
										return androidDriver;
									}
								}
							}
							if (ex.Message.Contains("Could not proxy command to the remote server"))
							{
								KillProcessByPort(device.SystemPort);
								KillProcessByPort(device.Port);
							}
							else if (!ex.Message.Contains("Could not find a connected Android device"))
							{
								if (ex.Message.Contains("UiAutomator2 Server cannot start because the local port"))
								{
									KillProcessByPort(device.SystemPort);
									KillProcessByPort(device.Port);
								}
								else if (ex.Message.Contains("The session identified by"))
								{
									UnInstallApp(device.DeviceId, "io.appium.settings");
									UnInstallApp(device.DeviceId, "io.appium.uiautomator2.server.test");
									UnInstallApp(device.DeviceId, "io.appium.uiautomator2.server");
								}
								else
								{
									Utils.CCKLog(" StartPhone ", Environment.NewLine + device.DeviceId + " : " + ex.Message);
								}
							}
						}
						else if (!KillPort)
						{
							KillPort = true;
							KillProcessByPort(device.SystemPort);
							KillProcessByPort(device.Port);
							Thread.Sleep(5000);
							Utils.CCKLog(" StartPhone ", Environment.NewLine + device.DeviceId + " : Kill Port ");
							KillPort = false;
						}
						continue;
					}
					return androidDriver;
				}
				catch (Exception ex2)
				{
					Utils.CCKLog("TaskList", "StartPhone: " + ex2.Message);
				}
				break;
			}
			return androidDriver;
		}

		public static string RunCommand(string cmd)
		{
			string empty = string.Empty;
			try
			{
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%") + "\\System32\\cmd.exe",
					WorkingDirectory = Application.StartupPath,
					Arguments = "/c " + cmd,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					Verb = "runas",
					RedirectStandardError = true
				});
				empty = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				process.Close();
				process.Dispose();
			}
			catch (Exception)
			{
				empty = "404";
			}
			return empty;
		}

		public static void ExecuteLDP(string cmd, string pathLD)
		{
			Process process = new Process();
			process.StartInfo.FileName = pathLD;
			process.StartInfo.Arguments = cmd;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.EnableRaisingEvents = true;
			process.Start();
			process.WaitForExit();
			process.Close();
		}

		public static List<string> GetAllPortListeningOnComputer()
		{
			List<string> list = new List<string>();
			try
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				empty = "netstat -an |find /i \"listening\"";
				empty2 = RunCommand(empty);
				string[] array = empty2.Split('\n');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (!text.Contains("127.0.0") && !text.Contains("0.0.0"))
					{
						continue;
					}
					MatchCollection matchCollection = Regex.Matches(text, "(.*?)\\:(.*?) ", RegexOptions.Singleline);
					if (matchCollection.Count > 0)
					{
						string text2 = matchCollection[0].Groups[2].Value.Trim();
						if (Utils.IsNumber(text2))
						{
							list.Add(text2);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("GetAllPortListeningOnComputer", ex.Message);
			}
			return list;
		}

		public static void SetBrightness(string deviceId, int number)
		{
			if (number < 0)
			{
				number = 50;
			}
			if (number > 255)
			{
				number = 255;
			}
			ExecuteCMD(deviceId, "shell settings put system screen_brightness " + number);
		}

		public static void APPIUM_REBOOT(int port)
		{
			RunCommand(" appium --reboot --port " + port);
		}

		private static bool IsPortOpen(string host, int port, TimeSpan timeout)
		{
			try
			{
				using TcpClient tcpClient = new TcpClient();
				IAsyncResult asyncResult = tcpClient.BeginConnect(host, port, null, null);
				bool result = asyncResult.AsyncWaitHandle.WaitOne(timeout);
				tcpClient.EndConnect(asyncResult);
				return result;
			}
			catch
			{
				return false;
			}
		}

		public static AppiumInfo START_APPIUM_SERVER(string deviceId, int port = 4723)
		{
			AppiumInfo appiumInfo = new AppiumInfo();
			appiumInfo.pid = deviceId;
			appiumInfo.port = port;
			appiumInfo.is_started = false;
			appiumInfo.Id = 0;
			appiumInfo.SesionId = 0;
			try
			{
				string cmd = " appium --port " + port;
				int num = 0;
				new Task(delegate
				{
					string text = RunCommand(cmd);
					if (text.Contains("The requested port may already be in use"))
					{
						KillProcessByPort(port);
						RunCommand(cmd);
					}
				}).Start();
				Thread.Sleep(2000);
				while (num < 20)
				{
					List<string> allPortListeningOnComputer = GetAllPortListeningOnComputer();
					Thread.Sleep(2000);
					num++;
					if (allPortListeningOnComputer.Contains(port.ToString()))
					{
						appiumInfo.is_started = true;
						appiumInfo.Message = "Success";
						return appiumInfo;
					}
				}
			}
			catch (Exception ex)
			{
				appiumInfo.Message = ex.Message;
			}
			return appiumInfo;
		}

		public static List<IWebElement> AppGetObjects(By by, RemoteWebDriver driver)
		{
			try
			{
				List<IWebElement> list = driver.FindElements(by).ToList();
				if (list != null && list.Count > 0)
				{
					return list;
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static List<CCKNode> AppGetObjects(string xpath, string source, string p_DeviceId)
		{
			try
			{
				CCKDriver cCKDriver = new CCKDriver(p_DeviceId);
				List<CCKNode> list = cCKDriver.FindElements(xpath, source).ToList();
				if (list != null && list.Count > 0)
				{
					return list;
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		public static IWebElement WaitMeXPath(By by, RemoteWebDriver driver, int timeout = 30, DeviceEntity entity = null)
		{
			by.ToString();
			int num = 0;
			while (num < timeout)
			{
				IWebElement webElement = AppGetObject(by, driver);
				if (webElement == null)
				{
					num++;
					Thread.Sleep(1000);
					continue;
				}
				return webElement;
			}
			return null;
		}

		public static List<IWebElement> WaitMes(By by, RemoteWebDriver driver, int timeout = 30)
		{
			int num = 0;
			List<IWebElement> list;
			while (true)
			{
				if (num < timeout)
				{
					list = AppGetObjects(by, driver);
					if (list != null)
					{
						break;
					}
					num++;
					Thread.Sleep(1000);
					continue;
				}
				return null;
			}
			return list;
		}

		internal static void DoAction(string p_DeviceId, string action)
		{
			ExecuteCMD(p_DeviceId, $"shell am start -a android.intent.action.VIEW -d {action}");
			Thread.Sleep(new Random().Next(3, 10) * 1000);
		}

		internal static void ShowTextMessageOnPhone(string SerialNo, string msg)
		{
			ExecuteCMD(SerialNo, $"shell am start -n \"com.cck.support/.ToastActivity\"  --es sms '{msg}'");
		}

		internal static void GotoProfile(string p_DeviceId, string uid)
		{
			OpenLink(p_DeviceId, "fb://profile/" + uid);
		}

		internal static void UnInstallApp(string p_DeviceId, string app)
		{
			ExecuteCMD(p_DeviceId, " uninstall " + app);
		}

		internal static void KillSCRCPY()
		{
			new Task(delegate
			{
				RunCommand("taskkill /f /im scrcpy.exe");
			}).Start();
			Thread.Sleep(1000);
		}

		internal static void RemoveFileFromSDcard(string deviceId, string fileName)
		{
			ExecuteCMD(deviceId, $"shell su rm -r \"/sdcard/{fileName}\"");
		}

		internal static bool WaitDeviceReady(string deviceId)
		{
			Thread.Sleep(60000);
			int num = 60;
			while (true)
			{
				if (num-- > 0)
				{
					List<string> listSerialDevice = GetListSerialDevice();
					if (listSerialDevice.Contains(deviceId))
					{
						break;
					}
					Thread.Sleep(5000);
					continue;
				}
				return false;
			}
			Thread.Sleep(5000);
			return true;
		}

		internal static List<IWebElement> AppGetObjects(string p, RemoteWebDriver m_driver)
		{
			return AppGetObjects(By.XPath(p), m_driver);
		}

		internal static List<IWebElement> AppGetObjectsByText(string p, RemoteWebDriver m_driver)
		{
			return AppGetObjects(By.XPath(string.Format("//*[lower-case(@text) = '{0}' or lower-case(@content-desc) = '{0}']", p.ToLower().Trim())), m_driver);
		}

		internal static IWebElement AppGetObjectByText(string p, RemoteWebDriver m_driver)
		{
			return AppGetObject(By.XPath(string.Format("//*[lower-case(@text) = '{0}' or lower-case(@content-desc) = '{0}']", p.ToLower().Trim())), m_driver);
		}

		internal static int GetAndroidVersionNumber(string p_DeviceId)
		{
			string text = string.Empty;
			try
			{
				string cmd = " shell getprop ro.build.version.release ";
				text = ExecuteCMD(p_DeviceId, cmd);
				text = text.Trim();
				if (string.IsNullOrEmpty(text))
				{
					return 9;
				}
			}
			catch (Exception)
			{
			}
			return Utils.Convert2Int(text.Split('.')[0]);
		}

		internal static string GetTiktokVersion(string SerialNo)
		{
			string text = ExecuteCMD(SerialNo, "shell dumpsys package " + CaChuaConstant.PACKAGE_NAME + " | grep versionName");
			List<string> list = text.Split('=').ToList();
			if (list.Count == 2)
			{
				return list[1];
			}
			return text;
		}
	}
}
