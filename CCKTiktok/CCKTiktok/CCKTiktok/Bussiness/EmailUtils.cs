using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Entity;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace CCKTiktok.Bussiness
{
	public class EmailUtils
	{
		public class EmailGetnataComResult
		{
			public class Message
			{
				public string uid { get; set; }

				public string f { get; set; }

				public string s { get; set; }

				public bool d { get; set; }

				public List<object> at { get; set; }

				public DateTime cr { get; set; }

				public int r { get; set; }

				public string ph { get; set; }

				public string rr { get; set; }

				public string ib { get; set; }
			}

			public class RootObject
			{
				public List<Message> msgs { get; set; }
			}
		}

		public class EmailResult
		{
			public string Email { get; set; }

			public string From { get; set; }

			public string DateReceive { get; set; }

			public string Title { get; set; }

			public string TextBody { get; set; }

			public string HtmlBody { get; set; }

			public EmailResult()
			{
				Email = "";
				From = "";
				DateReceive = string.Empty;
				Title = string.Empty;
				TextBody = string.Empty;
				HtmlBody = string.Empty;
			}
		}

		public static List<string> getnedaList = new List<string> { "@getnada.com" };

		private RegNickEntity entity;

		private static Random random = new Random();

		private List<string> lst1Sec = new List<string> { "1secmail.com", "1secmail.org", "1secmail.net", "icznn.com", "ezztt.com", "vjuum.com", "laafd.com", "txcct.com" };

		public const string Host = "pop.gmail.com";

		public const int Port = 995;

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public EmailUtils()
		{
			entity = new RegNickEntity();
			if (File.Exists(CaChuaConstant.REG_CONFIG))
			{
				entity = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				}.Deserialize<RegNickEntity>(Utils.ReadTextFile(CaChuaConstant.REG_CONFIG));
			}
		}

		public Account GetEmail(TTItems item)
		{
			return GetEmail(item, EmailRegisterType.Null);
		}

		public Account GetEmail(TTItems item, EmailRegisterType regtype)
		{
			if (regtype == EmailRegisterType.Null)
			{
				regtype = ((!File.Exists(CaChuaConstant.VERI_TYPE)) ? EmailRegisterType.MaxClone : new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				}.Deserialize<EmailRegisterType>(File.ReadAllText(CaChuaConstant.VERI_TYPE)));
			}
			Account account = new Account();
			switch (regtype)
			{
			case EmailRegisterType.Onesecmail:
			{
				OneSecMail oneSecMail = new OneSecMail();
				return new Account
				{
					accounts = oneSecMail.GetEmail() + "|"
				};
			}
			case EmailRegisterType.GoogleAppDomain:
				if (File.Exists(CaChuaConstant.GOOGLE_APP_DOMAIM))
				{
					account.accounts = Utils.GetFirstItemFromFile(CaChuaConstant.GOOGLE_APP_DOMAIM);
					File.AppendAllLines(Application.StartupPath + "\\Config\\cck_log_email.txt", new List<string> { account.accounts });
					return account;
				}
				goto default;
			case EmailRegisterType.DongVanFB:
			{
				DongVanFB dongVanFB = new DongVanFB(Utils.ReadTextFile(CaChuaConstant.DONGVANFB));
				Account account2 = dongVanFB.GetAccount();
				if (account2 != null)
				{
					account.accounts = account2.User + "|" + account2.Pass;
					File.AppendAllLines(Application.StartupPath + "\\Config\\cck_log_email.txt", new List<string> { account.accounts });
				}
				return account;
			}
			case EmailRegisterType.MailDomain:
			{
				string text = Utils.ReadTextFile(CaChuaConstant.DOMAIN_CONFIG);
				string uid = item.Uid;
				account = new Account();
				if (!(uid == ""))
				{
					account.accounts = RandomString(4).ToLower() + "_" + uid.Replace("@", "") + "@" + text.Replace("@", "") + "|";
				}
				else
				{
					account.accounts = Utils.UnicodeToKoDau(item.FirstName + item.LastName + "_" + Guid.NewGuid().ToString("N").Substring(0, random.Next(10, 15))
						.ToLower()).ToLower() + "@" + text.Replace("@", "") + "|";
				}
				File.AppendAllLines(Application.StartupPath + "\\Config\\cck_log_email.txt", new List<string> { account.accounts });
				return account;
			}
			case EmailRegisterType.SellGmail:
			{
				SellGmail sellGmail = new SellGmail(Utils.ReadTextFile(CaChuaConstant.SellGmail));
				return new Account
				{
					accounts = sellGmail.GetEmail()
				};
			}
			case EmailRegisterType.SuperTeamGmail:
			{
				superteam_info superteam_info2 = new superteam_info(Utils.ReadTextFile(CaChuaConstant.SuperTeamGmail));
				return new Account
				{
					accounts = superteam_info2.GetEmail()
				};
			}
			case EmailRegisterType.Getnada:
				return new Account
				{
					accounts = Guid.NewGuid().ToString("N").Substring(0, 16) + getnedaList[0] + "|"
				};
			case EmailRegisterType.MaxClone:
				if (File.Exists(CaChuaConstant.MAXCLONE))
				{
					MaxClone maxClone = new MaxClone(Utils.ReadTextFile(CaChuaConstant.MAXCLONE));
					MaxCloneEntity.Datas.EmailInfo email = maxClone.GetEmail();
					if (email != null)
					{
						account.accounts = email.Email + "|" + email.Password;
						File.AppendAllLines(Application.StartupPath + "\\Config\\cck_log_email.txt", new List<string> { account.accounts });
					}
					return account;
				}
				goto default;
			case EmailRegisterType.HotmailFromFile:
				if (File.Exists(CaChuaConstant.EMAIL_FROMFILE))
				{
					account.accounts = Utils.GetFirstItemFromFile(CaChuaConstant.EMAIL_FROMFILE);
					File.AppendAllLines(Application.StartupPath + "\\Config\\cck_log_email.txt", new List<string> { account.accounts });
					return account;
				}
				goto default;
			default:
				return null;
			}
		}

		public static string RandomString(int length)
		{
			return new string((from s in Enumerable.Repeat("abcdefghijklmnopqrstuvwxysABCDEFGHIJKLMNOPQRSTUVWXYZ", length)
				select s[random.Next(s.Length)]).ToArray());
		}

		public string CreateGenedaEmail(string mymail)
		{
			return mymail.Split('@')[0].ToString() + random.Next(1000, 9999) + getnedaList[0];
		}

		public string GetGenedaCode(string email, int length = 6)
		{
			DateTime dateTime = DateTime.Now.AddMinutes(1.0);
			string url = "https://inboxes.com/api/v2/inbox/" + email;
			string path = string.Format("Log\\" + email.Replace("@", "") + ".txt");
			List<string> list = new List<string>();
			if (File.Exists(path))
			{
				list = File.ReadAllLines(path).ToList();
			}
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			while (DateTime.Now < dateTime)
			{
				try
				{
					string response = Utils.GetResponse(url);
					EmailGetnataComResult.RootObject rootObject = javaScriptSerializer.Deserialize<EmailGetnataComResult.RootObject>(response);
					if (rootObject != null)
					{
						List<EmailGetnataComResult.Message> msgs = rootObject.msgs;
						if (msgs != null && msgs.Count > 0)
						{
							string uid = msgs[0].uid;
							if (uid != null && !list.Contains(uid))
							{
								string url2 = "https://inboxes.com/api/v2/message/" + uid;
								response = Utils.GetResponse(url2);
								if (response != "")
								{
									Regex regex = new Regex("([0-9]+) is your");
									Match match = regex.Match(response);
									if (match.Success)
									{
										File.AppendAllLines(path, new List<string> { uid });
										string text = match.Groups[1].Value.ToString();
										if (text.Length >= length || text.Length == length || text.Length == 6)
										{
											return text;
										}
									}
								}
							}
						}
					}
					Thread.Sleep(5000);
				}
				catch
				{
				}
			}
			return "";
		}

		public string GetCode(string email, string pass, string App = "Tiktok", EmailRegisterType regtype = EmailRegisterType.Null)
		{
			try
			{
				if (regtype == EmailRegisterType.Null)
				{
					regtype = ((!File.Exists(CaChuaConstant.VERI_TYPE)) ? EmailRegisterType.MaxClone : new JavaScriptSerializer
					{
						MaxJsonLength = int.MaxValue
					}.Deserialize<EmailRegisterType>(File.ReadAllText(CaChuaConstant.VERI_TYPE)));
				}
				string text = (email.Contains("@") ? email.Split('@')[1] : email);
				foreach (string item in lst1Sec)
				{
					if (item == text)
					{
						OneSecMail oneSecMail = new OneSecMail();
						return oneSecMail.GetCode(email);
					}
				}
				if (regtype != EmailRegisterType.Getnada && !email.ToLower().Trim().EndsWith("@getnada.com"))
				{
					if (!text.Contains("hotmail") && !text.Contains("outlook"))
					{
						if (regtype != EmailRegisterType.Onesecmail && (!email.Contains("@") || !lst1Sec.Contains(email.Split('@')[1])))
						{
							switch (regtype)
							{
							case EmailRegisterType.MailDomain:
							{
								string text3 = "";
								DateTime dateTime2 = DateTime.Now.AddSeconds(240.0);
								while (text3 == "" && DateTime.Now < dateTime2)
								{
									text3 = ReadEmail(email, App);
									if (text3 != "")
									{
										break;
									}
									Thread.Sleep(5000);
								}
								return text3;
							}
							case EmailRegisterType.SellGmail:
							{
								string text2 = "";
								SellGmail sellGmail = new SellGmail(Utils.ReadTextFile(CaChuaConstant.SellGmail));
								DateTime dateTime = DateTime.Now.AddSeconds(120.0);
								while (text2 == "" && DateTime.Now < dateTime)
								{
									text2 = sellGmail.GetCode(email);
									if (text2 != "")
									{
										break;
									}
									Thread.Sleep(5000);
								}
								return text2;
							}
							case EmailRegisterType.SuperTeamGmail:
							{
								superteam_info superteam_info2 = new superteam_info(Utils.ReadTextFile(CaChuaConstant.SuperTeamGmail));
								return superteam_info2.GetCode(email);
							}
							case EmailRegisterType.Getnada:
								return GetGenedaCode(email);
							default:
								return "";
							case EmailRegisterType.GoogleAppDomain:
								return "googleapp";
							}
						}
						OneSecMail oneSecMail2 = new OneSecMail();
						return oneSecMail2.GetCode(email);
					}
					DongVanFB dongVanFB = new DongVanFB("");
					string status = "";
					string text4 = "";
					DateTime dateTime3 = DateTime.Now.AddSeconds(240.0);
					while (text4 == "" && dateTime3 > DateTime.Now)
					{
						text4 = dongVanFB.GetEmail(email, pass, out status);
						if (!status.Contains("LOGIN failed"))
						{
							if (!(text4 != ""))
							{
								Thread.Sleep(5000);
								continue;
							}
							File.AppendAllLines(string.Format("Log\\code_email_get_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), new List<string> { email + "|" + pass + "|" + text4 + "|" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") });
							break;
						}
						return "LOGIN failed";
					}
					return text4;
				}
				return GetGenedaCode(email);
			}
			catch (Exception ex)
			{
				Utils.CCKLog("GetCode - Error: ", email + " - " + ex.Message);
			}
			return "";
		}

		public static bool CheckConfig(string gmail, string pass)
		{
			try
			{
				Pop3Client pop3Client = new Pop3Client();
				pop3Client.Connect("pop.gmail.com", 995, useSsl: true);
				pop3Client.Authenticate($"recent: {gmail}", pass);
				pop3Client.GetMessageCount();
				return true;
			}
			catch
			{
			}
			return false;
		}

		public string ReadEmail(string to, string socialName = "Facebook", int length = 0)
		{
			try
			{
				string email = to.Split('@')[1];
				EmailSource byEmail = new EmailSource().GetByEmail(email);
				if (byEmail == null)
				{
					return "";
				}
				using Pop3Client pop3Client = new Pop3Client();
				pop3Client.Connect("pop.gmail.com", 995, useSsl: true);
				pop3Client.Authenticate($"recent: {byEmail.gmail}", byEmail.pass);
				int messageCount = pop3Client.GetMessageCount();
				int num = messageCount;
				while (num > 0 && num > messageCount - 20)
				{
					OpenPop.Mime.Message message = pop3Client.GetMessage(num--);
					if (message.Headers.To == null || message.Headers.To.Count <= 0 || !(message.Headers.To[0].Address.ToLower() == to.ToLower()))
					{
						continue;
					}
					_ = message.MessagePart;
					string text = ((message.MessagePart.MessageParts != null) ? Encoding.UTF8.GetString(message.MessagePart.MessageParts[0].Body) : Encoding.UTF8.GetString(message.MessagePart.Body));
					if (text.Contains("Nếu bạn không cố đăng nhập") || text.Contains("đặt lại mật khẩu Facebook "))
					{
						Regex regex = new Regex(":(\\d{8})");
						Match match = regex.Match(text);
						if (!match.Success)
						{
							regex = new Regex(":(\\d{6})");
							match = regex.Match(text);
							if (match.Success)
							{
								return match.Groups[1].Value;
							}
							continue;
						}
						return match.Groups[1].Value;
					}
					if (socialName == "hotmail")
					{
						switch (length)
						{
						case 0:
						case 4:
						{
							Regex regex3 = new Regex(": (\\d{4})");
							Match match3 = regex3.Match(text);
							if (match3.Success)
							{
								return match3.Groups[1].Value;
							}
							break;
						}
						case 7:
						{
							Regex regex2 = new Regex(": (\\d{7})");
							Match match2 = regex2.Match(text);
							if (match2.Success)
							{
								return match2.Groups[1].Value;
							}
							break;
						}
						}
					}
					if (socialName == "Facebook")
					{
						Regex regex4 = new Regex(": (\\d{8})");
						Match match4 = regex4.Match(text);
						if (!match4.Success)
						{
							regex4 = new Regex("(\\d{6})");
							match4 = regex4.Match(message.Headers.Subject);
							if (!match4.Success)
							{
								regex4 = new Regex("c=(\\d{5})&");
								match4 = regex4.Match(text);
								if (match4.Success)
								{
									return match4.Groups[1].Value;
								}
								regex4 = new Regex("Confirmation code(\\d{6})");
								match4 = regex4.Match(text);
								if (match4.Success)
								{
									return match4.Groups[1].Value;
								}
								break;
							}
							string value = match4.Groups[1].Value;
							return value.Trim();
						}
						return match4.Groups[1].Value;
					}
					if (socialName == "Tiktok")
					{
						Regex regex5 = new Regex("(\\d{6})");
						Match match5 = regex5.Match(text);
						if (match5.Success)
						{
							return match5.Groups[1].Value;
						}
						regex5 = new Regex("(\\d{4})");
						match5 = regex5.Match(text);
						if (match5.Success)
						{
							return match5.Groups[1].Value;
						}
						break;
					}
					Regex regex6 = new Regex("confirmation code: ([^/]+)");
					Match match6 = regex6.Match(text);
					if (match6.Success)
					{
						return match6.Groups[1].Value;
					}
					break;
				}
			}
			catch (Exception ex)
			{
				File.AppendAllLines("Log\\logmail.txt", new List<string> { to + " - " + ex.Message });
			}
			return "";
		}

		internal void ReturnStock(Account mailItem)
		{
			File.AppendAllLines(CaChuaConstant.GOOGLE_APP_DOMAIM, new List<string> { mailItem.accounts });
		}
	}
}
