using System;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using AE.Net.Mail;
using Microsoft.Exchange.WebServices.Data;

namespace CCKTiktok.Bussiness
{
	public class DongVanFB
	{
		public string Api = "";

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public DongVanFB(string api)
		{
			Api = api;
		}

		public decimal GetBalance()
		{
			try
			{
				string address = "https://api.dongvanfb.net/user/balance?apikey=" + Api;
				string input = new WebClient().DownloadString(address);
				dynamic val = new JavaScriptSerializer().DeserializeObject(input);
				return Convert.ToDecimal(val["balance"]) + Convert.ToDecimal(val["balance_promo"]);
			}
			catch
			{
				return Convert.ToDecimal(0);
			}
		}

		public Account GetAccount()
		{
			if (GetBalance() > 2000m)
			{
				try
				{
					int num = 1;
					int num2 = 0;
					if (Utils.ReadTextFile(CaChuaConstant.DONGVANFB_TYPE) != "")
					{
						num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DONGVANFB_TYPE));
					}
					while (true)
					{
						string text = "";
						string address = $"https://api.dongvanfb.net/user/buy?apikey={Api}&account_type={num}&quality=1";
						string input = new WebClient().DownloadString(address);
						dynamic val = new JavaScriptSerializer().DeserializeObject(input);
						_ = val["data"]["order_code"];
						dynamic val2 = val["data"]["list_data"];
						if (val2 != null && val2.Length > 0)
						{
							text = val2[0];
						}
						if (!(text != "") || text.Length <= 5)
						{
							if (num2++ < 5)
							{
								Thread.Sleep(5000);
								continue;
							}
							break;
						}
						return new Account
						{
							accounts = text
						};
					}
				}
				catch
				{
				}
				return new Account();
			}
			return null;
		}

		public string ReadOutlookHotmail(string email = "elsafisuetamu@hotmail.com", string password = "T1uA9B45")
		{
			ExchangeService exchangeService = new ExchangeService();
			exchangeService.Credentials = new WebCredentials(email, password);
			exchangeService.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
			try
			{
				FindItemsResults<Item> findItemsResults = exchangeService.FindItems(WellKnownFolderName.Inbox, new ItemView(10));
				if (findItemsResults == null || findItemsResults.TotalCount == 0)
				{
					findItemsResults = exchangeService.FindItems(WellKnownFolderName.JunkEmail, new ItemView(10));
				}
				if (findItemsResults == null || findItemsResults.TotalCount == 0)
				{
					return "";
				}
				PropertySet propertySet = new PropertySet(BasePropertySet.FirstClassProperties);
				propertySet.Add(ItemSchema.Body);
				exchangeService.LoadPropertiesForItems(findItemsResults.Items, propertySet);
				foreach (Item item in findItemsResults.Items)
				{
					string address = ((EmailMessage)item).From.Address;
					if (item.Body == null)
					{
						continue;
					}
					try
					{
						item.Move(WellKnownFolderName.Notes);
					}
					catch
					{
					}
					if (address.ToString().Contains("tiktok.com"))
					{
						string text = item.Body.Text;
						Regex regex = new Regex(Environment.NewLine + "\\d{6}" + Environment.NewLine);
						Match match = regex.Match(text);
						if (match.Success && match.Groups[0].Value.Length >= 6)
						{
							return match.Groups[0].Value;
						}
						regex = new Regex(">\\d{6}<");
						match = regex.Match(text.Replace(" ", ""));
						if (match.Success && match.Groups[0].Value.Length >= 6)
						{
							string value = match.Groups[0].Value;
							return value.Replace(">", "").Replace("<", "");
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return "";
		}

		public string GetEmail(string username, string password, out string status, bool loginOnly = false)
		{
			status = "";
			try
			{
				using ImapClient imapClient = new ImapClient("outlook.office365.com", username, password, AuthMethods.Login, 993, secure: true);
				if (loginOnly)
				{
					status = "Success";
					return "";
				}
				try
				{
					imapClient.SelectMailbox("Inbox");
					int messageCount = imapClient.GetMessageCount();
					if (messageCount == 0)
					{
						imapClient.SelectMailbox("Junk");
						messageCount = imapClient.GetMessageCount();
					}
					if (messageCount == 0)
					{
						return "";
					}
					Lazy<MailMessage>[] array = imapClient.SearchMessages(SearchCondition.Unseen());
					if (array.Length == 0)
					{
						return "";
					}
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].Value == null)
						{
							continue;
						}
						try
						{
							imapClient.MoveMessage(array[i].Value.Uid, "Read");
						}
						catch (Exception)
						{
							try
							{
								imapClient.CreateMailbox("Read");
							}
							catch
							{
							}
						}
						if (array[i].Value.Date < DateTime.Now.AddMinutes(-5.0))
						{
							continue;
						}
						try
						{
							if (!array[i].Value.From.ToString().Contains("@facebookmail.com"))
							{
								goto IL_02c0;
							}
							string text = array[i].Value.Body.ToString();
							if (!text.Contains("find-friends") && !text.Contains("hacked/disavow"))
							{
								Regex regex = new Regex("(c|code)=([0-9]+)&(cuid|ext|z)");
								Match match = regex.Match(text);
								if (!match.Success)
								{
									regex = new Regex(" ([0-9]+) ");
									match = regex.Match(text);
									if (match.Success && match.Groups[1].Value.Length == 8)
									{
										return match.Groups[1].Value;
									}
									regex = new Regex("\\d{8}");
									match = regex.Match(text);
									if (match.Success && match.Groups[0].Value.Length == 8)
									{
										return match.Groups[0].Value;
									}
									regex = new Regex("\\d{6}");
									match = regex.Match(text);
									if (match.Success && match.Groups[0].Value.Length == 6)
									{
										return match.Groups[0].Value;
									}
									goto IL_02c0;
								}
								return match.Groups[2].Value;
							}
							goto end_IL_0119;
							IL_02c0:
							if (array[i].Value.From.ToString().Contains("tiktok.com"))
							{
								string text2 = array[i].Value.Body.ToString();
								Regex regex2 = new Regex(Environment.NewLine + "\\d{6}" + Environment.NewLine);
								Match match2 = regex2.Match(text2);
								if (match2.Success && match2.Groups[0].Value.Length >= 6)
								{
									return match2.Groups[0].Value;
								}
								regex2 = new Regex(">\\d{6}<");
								match2 = regex2.Match(text2.Replace(" ", ""));
								if (match2.Success && match2.Groups[0].Value.Length >= 6)
								{
									string value = match2.Groups[0].Value;
									return value.Replace(">", "").Replace("<", "");
								}
							}
							end_IL_0119:;
						}
						catch
						{
						}
					}
				}
				catch (Exception)
				{
					return ReadOutlookHotmail(username, password);
				}
			}
			catch (Exception ex3)
			{
				if (!ex3.Message.ToString().Contains("LOGIN failed"))
				{
					status = ex3.Message.ToString();
				}
				else
				{
					status = "LOGIN failed";
				}
			}
			return "";
		}

		internal DataTable GetEmailType()
		{
			try
			{
				string address = $"https://api.dongvanfb.net/user/account_type?apikey={Api}";
				string input = new WebClient().DownloadString(address);
				dynamic val = new JavaScriptSerializer().DeserializeObject(input);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("id");
				dataTable.Columns.Add("name");
				foreach (dynamic item in val["data"])
				{
					object value = item["id"];
					dynamic val2 = item["name"].ToString();
					dynamic val3 = item["quality"];
					dynamic val4 = item["price"];
					DataRow dataRow = dataTable.NewRow();
					dataRow["id"] = value;
					dataRow["name"] = (object)string.Format("{0} - {1} - {2}", val2, val3, val4);
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				return dataTable;
			}
			catch
			{
			}
			return new DataTable();
		}

		internal DataTable GetHotmailboxEmailType()
		{
			try
			{
				string address = "https://api.hotmailbox.me/mail/currentstock";
				string input = new WebClient().DownloadString(address);
				dynamic val = new JavaScriptSerializer().DeserializeObject(input);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("id");
				dataTable.Columns.Add("name");
				foreach (dynamic item in val["Data"])
				{
					object value = item["MailCode"];
					dynamic val2 = item["MailName"].ToString();
					dynamic val3 = item["Instock"];
					dynamic val4 = item["Price"];
					DataRow dataRow = dataTable.NewRow();
					dataRow["id"] = value;
					dataRow["name"] = (object)string.Format("{0} - {1} - {2}", val2, val3, val4);
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				return dataTable;
			}
			catch
			{
			}
			return new DataTable();
		}
	}
}
