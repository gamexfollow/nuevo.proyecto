using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.DAL;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CCKTiktok.Bussiness
{
	public class FBChrome
	{
		public ChromeDriver m_driver = null;

		public static int WindowCount;

		public bool Init(string createurl, string uid, bool capchat = false, string proxyIp = "", int proxyPort = 0, bool fullScreen = false, bool hiddenmode = true)
		{
			try
			{
				string text = Utils.ReadTextFile(Application.StartupPath + "\\Authentication\\" + uid + "\\cck_cookie.txt");
				WindowCount++;
				string text2 = Application.StartupPath + "\\ChromeProfile";
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				string text3 = text2 + "\\" + uid;
				if (!Directory.Exists(text3))
				{
					Directory.CreateDirectory(text3);
				}
				_ = Application.ExecutablePath;
				ChromeOptions chromeOptions = new ChromeOptions();
				chromeOptions.AddArgument("--disable-notifications");
				chromeOptions.AddArgument("--disable-popup-blocking");
				chromeOptions.AddArgument("--user-data-dir=" + text3);
				chromeOptions.AddArgument("--disable-infobars");
				chromeOptions.AddArgument("--window-position=-32000,-32000");
				if (proxyIp != "" && proxyPort > 0)
				{
					chromeOptions.AddArgument($"--proxy-server={proxyIp}:{proxyPort}");
				}
				chromeOptions.AddArgument("user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 14_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) FxiOS/33.0 Mobile/15E148 Safari/605.1.15");
				chromeOptions.AddArguments("--disable-notifications", "start-maximized", "--no-sandbox", "--disable-gpu", "--disable-dev-shm-usage", "--disable-web-security", "--disable-rtc-smoothness-algorithm", "--disable-webrtc-hw-decoding", "--disable-webrtc-hw-encoding", "--disable-webrtc-multiple-routes", "--disable-webrtc-hw-vp8-encoding", "--enforce-webrtc-ip-permission-check", "--force-webrtc-ip-handling-policy", "--ignore-certificate-errors", "--disable-infobars", "--disable-popup-blocking");
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.notifications", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.plugins", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.popups", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.auto_select_certificate", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.mixed_script", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.media_stream", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.media_stream_mic", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.media_stream_camera", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.protocol_handlers", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.midi_sysex", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.push_messaging", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.ssl_cert_decisions", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.metro_switch_to_desktop", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.protected_media_identifier", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.site_engagement", 1);
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.durable_storage", 1);
				chromeOptions.AddUserProfilePreference("useAutomationExtension", true);
				chromeOptions.AddArguments("--disable-3d-apis", "--disable-background-networking", "--disable-bundled-ppapi-flash", "--disable-client-side-phishing-detection", "--disable-default-apps", "--disable-hang-monitor", "--disable-prompt-on-repost", "--disable-sync", "--disable-webgl", "--enable-blink-features=ShadowDOMV0", "--enable-logging", "--disable-notifications", "--no-sandbox", "--disable-gpu", "--disable-dev-shm-usage", "--disable-web-security", "--disable-rtc-smoothness-algorithm", "--disable-webrtc-hw-decoding", "--disable-webrtc-hw-encoding", "--disable-webrtc-multiple-routes", "--disable-webrtc-hw-vp8-encoding", "--enforce-webrtc-ip-permission-check", "--force-webrtc-ip-handling-policy", "--ignore-certificate-errors", "--disable-infobars", "--disable-blink-features=\"BlockCredentialedSubresources\"", "--disable-popup-blocking");
				chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 0);
				chromeOptions.AddArgument("--mute-audio");
				chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
				chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
				chromeOptions.AddExcludedArgument("enable-automation");
				chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
				if (!File.Exists("Devices\\AnyCaptchaExtensions.crx"))
				{
					new WebClient().DownloadFile("https://cck.vn/Download/Utils/AnyCaptchaExtensions.crx.rar", Application.StartupPath + "\\Devices\\AnyCaptchaExtensions.crx");
				}
				if (capchat && File.Exists(Application.StartupPath + "\\Devices\\AnyCaptchaExtensions.crx"))
				{
					chromeOptions.AddExtensions(Application.StartupPath + "\\Devices\\AnyCaptchaExtensions.crx");
				}
				try
				{
					if (uid != "")
					{
						string text4 = Application.StartupPath + $"\\ChromeProfile\\{uid}\\Default\\Preferences";
						if (File.Exists(text4))
						{
							string text5 = Utils.ReadTextFile(text4);
							text5 = text5.Replace("\"exit_type\":\"Crashed\"", "\"exit_type\":\"None\"");
							File.Delete(text4);
							File.WriteAllText(text4, text5);
						}
					}
				}
				catch (Exception)
				{
				}
				ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
				chromeDriverService.HideCommandPromptWindow = true;
				m_driver = new ChromeDriver(chromeDriverService, chromeOptions);
				_ = m_driver.Capabilities;
				if (!fullScreen)
				{
					Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
					int num = workingArea.Width / 3;
					int num2 = workingArea.Height / 2;
					int num3 = WindowCount % 2;
					int num4 = WindowCount % 3;
					m_driver.Manage().Window.Size = new Size(num, num2);
					m_driver.Manage().Window.Position = new Point(num4 * num, num3 * num2);
				}
				m_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(5.0);
				m_driver.Navigate().GoToUrl(createurl);
				if (text != "")
				{
					new List<TiktokCookies>();
					List<TiktokCookies> list = new JavaScriptSerializer().Deserialize<List<TiktokCookies>>(text);
					foreach (TiktokCookies item in list)
					{
						OpenQA.Selenium.Cookie cookie = new OpenQA.Selenium.Cookie(item.name, item.value, item.domain, item.path, DateTime.Now.AddDays(360.0));
						m_driver.Manage().Cookies.AddCookie(cookie);
					}
				}
				SetLange();
				Thread.Sleep(2000);
				List<string> list2 = new List<string>(m_driver.WindowHandles);
				if (list2.Count > 0)
				{
					m_driver.SwitchTo().Window(list2[0]);
				}
				if (capchat && File.Exists(CaChuaConstant.MMOCAPTCHA))
				{
					m_driver.SwitchTo().Window(list2[0]);
					Thread.Sleep(1000);
					WriteScript($"document.getElementsByName('apiKey')[0].value='{Utils.ReadTextFile(CaChuaConstant.MMOCAPTCHA)}'; document.getElementById('connect').click();");
					Thread.Sleep(1000);
					m_driver.SwitchTo().Alert().Accept();
					m_driver.Close();
					m_driver.SwitchTo().Window(list2[1]);
					Thread.Sleep(1000);
				}
				WaitForPageLoad();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public void Unlock956()
		{
			SetLange();
			string input = m_driver.Url.ToString();
			Regex regex = new Regex("facebook.com/checkpoint/([0-9]+)956/");
			if (!regex.Match(input).Success)
			{
			}
		}

		public void Unlock282()
		{
			SetLange();
			string input = m_driver.Url.ToString();
			Regex regex = new Regex("facebook.com/checkpoint/([0-9]+)282/");
			if (regex.Match(input).Success)
			{
				m_driver.FindElementByXPath("//*[contains(@text,'Yêu cầu xem xét lại')]")?.Click();
			}
		}

		private void SetLange()
		{
			try
			{
				((IJavaScriptExecutor)m_driver).ExecuteScript("require('IntlUtils').setLocale(null, 'www_list_selector', 'en_US');", Array.Empty<object>());
			}
			catch
			{
			}
		}

		private void WriteScript(string script)
		{
			try
			{
				((IJavaScriptExecutor)m_driver).ExecuteScript(script, Array.Empty<object>());
			}
			catch
			{
			}
		}

		public void Login(string uid, string pass, string _2fa)
		{
			try
			{
				SetLange();
				WaitForPageLoad();
				int num = 0;
				while (!m_driver.PageSource.Contains("id=\"email\"") && num < 30)
				{
					num++;
					Thread.Sleep(3000);
				}
				Thread.Sleep(1000);
				IWebElement webElement = m_driver.FindElementById("email");
				if (webElement != null)
				{
					webElement.SendKeys(uid);
					Thread.Sleep(1000);
				}
				IWebElement webElement2 = m_driver.FindElementById("pass");
				if (webElement2 != null)
				{
					Thread.Sleep(1000);
					webElement2.SendKeys(pass);
					Thread.Sleep(1000);
				}
				IWebElement webElement3 = m_driver.FindElementByName("login");
				if (webElement3 == null)
				{
					return;
				}
				Thread.Sleep(1000);
				webElement3.Click();
				Thread.Sleep(2000);
				WaitForPageLoad();
				string pageSource = m_driver.PageSource;
				if (pageSource.Contains("approvals_code") && _2fa != "")
				{
					IWebElement webElement4 = m_driver.FindElementById("approvals_code");
					string currentOtp = TimeSensitivePassCode.GetCurrentOtp(_2fa);
					webElement4.SendKeys(currentOtp);
					Thread.Sleep(1000);
					while (m_driver.PageSource.Contains("checkpointSubmitButton"))
					{
						IWebElement webElement5 = m_driver.FindElementById("checkpointSubmitButton");
						if (webElement5 != null)
						{
							webElement5.Click();
							Thread.Sleep(2000);
						}
					}
				}
				Thread.Sleep(2000);
				string text = m_driver.Url.ToString();
				if (text.Contains("/checkpoint/disabled/"))
				{
					new SQLiteUtils().LoginStatus(uid, "CheckPoint Khỏi gỡ");
				}
				else
				{
					if (text.Contains("282/"))
					{
						new SQLiteUtils().LoginStatus(uid, "CheckPoint 282");
						if (!m_driver.PageSource.Contains("Request a Review"))
						{
							return;
						}
						IWebElement @object = GetObject(By.XPath("//span[text()='Request a Review']"));
						if (@object == null)
						{
							return;
						}
						@object.Click();
						Thread.Sleep(2000);
						num = 60;
						bool flag = false;
						while (num-- > 0)
						{
							IWebElement object2 = GetObject(By.XPath("//iframe[contains(@src,'/common/referer_frame.php')]"));
							if (object2.FindElements(By.ClassName("captcha-solver-info")).Count <= 0)
							{
								Thread.Sleep(2000);
								continue;
							}
							object2.FindElements(By.ClassName("captcha-solver-info"))[0].Click();
							flag = true;
							break;
						}
						if (flag)
						{
							Thread.Sleep(30000);
						}
						return;
					}
					if (!text.Contains("956/"))
					{
						return;
					}
					new SQLiteUtils().LoginStatus(uid, "CheckPoint 956");
					string pageSource2 = m_driver.PageSource;
					if (pageSource2.Contains("Bắt đầu") || pageSource2.Contains("GetStart"))
					{
						IWebElement object3 = GetObject(By.XPath("//*[text()='Bắt đầu' or text() ='GetStart']"));
						if (object3 != null)
						{
							object3.Click();
							Thread.Sleep(2000);
							SetLange();
						}
					}
					pageSource2 = m_driver.PageSource;
					if (!pageSource2.Contains("Tiếp") && !pageSource2.Contains("GetStart"))
					{
						return;
					}
					IWebElement object4 = GetObject(By.XPath("//*[text()='Tiếp' or text() ='GetStart']"));
					if (object4 != null)
					{
						object4.Click();
						Thread.Sleep(2000);
						pageSource2 = m_driver.PageSource;
						string text2 = "Nhận mã trên điện thoại";
						if (pageSource2.Contains(text2))
						{
							new SQLiteUtils().LoginStatus(uid, text2);
						}
					}
					return;
				}
			}
			catch
			{
			}
		}

		public IWebElement GetObject(By b)
		{
			try
			{
				ReadOnlyCollection<IWebElement> readOnlyCollection = m_driver.FindElements(b);
				if (readOnlyCollection != null && readOnlyCollection.Count > 0)
				{
					return readOnlyCollection[0];
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		protected void WaitForPageLoad()
		{
			try
			{
				WebDriverWait webDriverWait = new WebDriverWait(m_driver, TimeSpan.FromSeconds(60.0));
				webDriverWait.Until((IWebDriver driver) => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			}
			catch
			{
			}
		}

		private void LoginFbByCookie(string _cookies, ChromeDriver m_driver)
		{
			string script = "function setCookie(t) { \r\n                                            var list = t.split(';'); console.log(list); \r\n                                            for (var i = list.length - 1; i >= 0; i--) { \r\n                                                var cname = list[i].split('=')[0]; \r\n                                                var cvalue = list[i].split('=')[1]; \r\n                                                var d = new Date(); \r\n                                                d.setTime(d.getTime() + (7*24*60*60*1000)); \r\n                                                var expires = ';domain=.facebook.com;expires='+ d.toUTCString(); document.cookie = cname + '=' + cvalue + '; ' + expires; \r\n                                            }\r\n                                        } \r\n                                        function hex2a(hex) { \r\n                                            var str = ''; \r\n                                            for (var i = 0; i < hex.length; i += 2) { \r\n                                                var v = parseInt(hex.substr(i, 2), 16);\r\n                                                if (v) str += String.fromCharCode(v); \r\n                                            }\r\n                                        return str; } \r\n                                        var cookie = '" + _cookies + "';\r\n                                        setCookie(cookie);\r\n                                        document.location='https://www.tiktok.com/'";
			_ = (string)((IJavaScriptExecutor)m_driver).ExecuteScript(script, Array.Empty<object>());
			Thread.Sleep(1000);
			m_driver.Navigate().Refresh();
		}

		public bool CheckPage5(string uid)
		{
			m_driver.Navigate().GoToUrl("https://www.facebook.com/pages/creation/?ref_type=launch_point");
			Thread.Sleep(5000);
			string pageSource = m_driver.PageSource;
			if ((pageSource.Contains("Tên Trang") && pageSource.Contains("0 người theo dõi")) || (pageSource.ToLower().Contains("0 follower") && pageSource.ToLower().Contains("page name")))
			{
				return true;
			}
			return false;
		}
	}
}
