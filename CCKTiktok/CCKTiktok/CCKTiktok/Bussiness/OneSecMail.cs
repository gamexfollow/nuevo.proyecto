using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class OneSecMail
	{
		public class EmailData
		{
			public int Id { get; set; }

			public string From { get; set; }

			public string Subject { get; set; }

			public string Date { get; set; }
		}

		public class EmailContent
		{
			public int Id { get; set; }

			public string From { get; set; }

			public string Subject { get; set; }

			public string Date { get; set; }

			public List<Attachment> Attachments { get; set; }

			public string Body { get; set; }

			public string TextBody { get; set; }

			public string HtmlBody { get; set; }
		}

		public class Attachment
		{
			public string Filename { get; set; }

			public string ContentType { get; set; }

			public int Size { get; set; }
		}

		public string GetEmail()
		{
			string text = "https://www.1secmail.com/api/v1/?action=genRandomMailbox&count=1";
			DateTime dateTime = DateTime.Now.AddMinutes(2.0);
			using (WebClient webClient = new WebClient())
			{
				try
				{
					Utils.CCKLog("GetEmail - 1secmail", text);
					string text2 = "";
					while (DateTime.Now < dateTime)
					{
						text2 = webClient.DownloadString(text);
						if (!string.IsNullOrWhiteSpace(text2))
						{
							List<string> list = new JavaScriptSerializer().Deserialize<List<string>>(text2);
							if (list != null && list.Count > 0)
							{
								Utils.CCKLog("GetEmail - 1secmail", list[0]);
								return list[0];
							}
						}
					}
				}
				catch (Exception ex)
				{
					Utils.CCKLog("GetEmail - 1secmail", ex.Message);
				}
			}
			return "";
		}

		public string GetCode(string email)
		{
			if (email.Contains("@"))
			{
				if (!Directory.Exists("Log"))
				{
					Directory.CreateDirectory("Log");
				}
				if (!Directory.Exists("Log\\Mail"))
				{
					Directory.CreateDirectory("Log\\Mail");
				}
				if (!Directory.Exists("Log\\ErrorMail"))
				{
					Directory.CreateDirectory("Log\\ErrorMail");
				}
				string text = "Log\\ErrorMail\\" + email.Split('@')[0] + ".txt";
				if (!File.Exists(text))
				{
					File.AppendAllLines(text, new List<string> { "Bắt đầu tạo file và đọc code lúc: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm") });
				}
				else
				{
					File.AppendAllLines(text, new List<string> { "Resend lại code lúc: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm") });
				}
				string address = "https://www.1secmail.com/api/v1/?action=getMessages&login=" + email.Split('@')[0] + "&domain=" + email.Split('@')[1];
				DateTime dateTime = DateTime.Now.AddMinutes(1.0);
				using (WebClient webClient = new WebClient())
				{
					try
					{
						string text2 = "";
						while (DateTime.Now < dateTime)
						{
							text2 = webClient.DownloadString(address);
							if (!(text2 != ""))
							{
								continue;
							}
							List<EmailData> list = new JavaScriptSerializer().Deserialize<List<EmailData>>(text2);
							if (list != null && list.Count > 0)
							{
								string text3 = "Log\\Mail\\" + email.Split('@')[0] + ".txt";
								if (Utils.ReadTextFile(text3).Contains(list[0].Id.ToString()))
								{
									Thread.Sleep(5000);
									continue;
								}
								string address2 = $"https://www.1secmail.com/api/v1/?action=readMessage&login={email.Split('@')[0]}&domain={email.Split('@')[1]}&id={list[0].Id}";
								text2 = webClient.DownloadString(address2);
								EmailContent emailContent = new JavaScriptSerializer().Deserialize<EmailContent>(text2);
								if (emailContent != null && emailContent.Body != "" && emailContent.Body.Contains("To verify your account, enter this code in TikTok"))
								{
									Regex regex = new Regex("<p style=\"font-family:arial;color:blue;font-size:20px;\"> ([0-9]+)</p>");
									Match match = regex.Match(emailContent.Body);
									if (match != null && match.Success)
									{
										File.AppendAllLines(text3, new List<string> { list[0].Id.ToString() });
										Utils.DeleteFile(text);
										return match.Groups[1].Value;
									}
								}
								if (list[0].Subject.Contains("is your") && list[0].Subject.Contains("code"))
								{
									Regex regex2 = new Regex("([0-9]+) is your (verification|TikTok) code");
									Match match2 = regex2.Match(list[0].Subject);
									if (match2 != null && match2.Success)
									{
										File.AppendAllLines(text3, new List<string> { list[0].Id.ToString() });
										Utils.DeleteFile(text);
										return match2.Groups[1].Value;
									}
								}
							}
							Thread.Sleep(5000);
						}
					}
					catch (Exception ex)
					{
						File.AppendAllLines(text, new List<string> { "Lỗi khi tìm code lúc: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm") + " - " + ex.Message });
					}
				}
				File.AppendAllLines(text, new List<string> { "Không tìm thấy code lúc: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm") });
				return "";
			}
			return "";
		}
	}
}
