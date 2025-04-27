using System;
using System.Collections.ObjectModel;
using System.Threading;
using CCKTiktok.DAL;
using OpenQA.Selenium;

namespace CCKTiktok.Bussiness
{
	public class Hotmail
	{
		private LoginItem loginItem;

		public string Email { get; set; }

		public string Password { get; set; }

		public string RecoveryEmail { get; set; }

		public FBChrome chrome { get; set; }

		public Hotmail()
		{
			chrome = new FBChrome();
			Email = "";
			Password = "";
			RecoveryEmail = "";
		}

		public Hotmail(LoginItem loginItem, FBChrome chrome, string email, string password, string recoveryEmail = "")
		{
			this.loginItem = loginItem;
			this.chrome = chrome;
			Email = email;
			Password = password;
			RecoveryEmail = recoveryEmail;
		}

		public void Login()
		{
		}

		public bool AddRecoveryPhone(string phoneNumber)
		{
			return true;
		}

		public bool AddRecoveryEmail(string emailUid, string uid)
		{
			chrome.m_driver.Navigate().GoToUrl("https://account.microsoft.com/security");
			Thread.Sleep(5000);
			IWebElement webElement = chrome.m_driver.FindElementByXPath("//*[contains(@href, '/proofs/manage/additional')]");
			if (webElement != null)
			{
				SrcollTrueView(chrome, webElement);
				webElement.Click();
			}
			else
			{
				chrome.m_driver.Navigate().GoToUrl("https://account.live.com/proofs/manage/additional?mkt=en-US&refd=account.microsoft.com&refp=security&client_flight=shhelp,shhidefam");
			}
			Thread.Sleep(2000);
			EmailUtils emailUtils = new EmailUtils();
			string text = emailUtils.CreateGenedaEmail(emailUid);
			if (GetPageSource(chrome).Contains("EmailAddress"))
			{
				IWebElement webElement2 = GetWebElement(chrome, By.Id("EmailAddress"));
				SrcollTrueView(chrome, webElement2);
				webElement2.SendKeys(text);
				Thread.Sleep(2000);
				SQLiteUtils sQLiteUtils = new SQLiteUtils();
				IWebElement webElement3 = GetWebElement(chrome, By.Id("iNext"));
				if (webElement3 != null)
				{
					SrcollTrueView(chrome, webElement3);
					webElement3.Click();
					sQLiteUtils.ExecuteQuery(string.Format("Update Account Set EmailRecovery='{0}' where Email= '{1}'; Insert Into LogEmail(uid,email,passemail,date,emailrecovery) values ('{4}','{1}','{2}','{3}','{0}')", text, Email, Password, uid, DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
					Thread.Sleep(2000);
				}
				string text2 = "";
				for (int i = 0; i < 5; i++)
				{
					bool flag = false;
					foreach (string getneda in EmailUtils.getnedaList)
					{
						if (text.Contains(getneda))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						text2 = emailUtils.GetGenedaCode(text, 4);
						sQLiteUtils.ExecuteQuery(string.Format("Update Account Set EmailRecovery='{0}' where Email= '{1}'; Insert Into LogEmail(uid,email,passemail,date,emailrecovery) values ('{4}','{1}','{2}','{3}','{0}')", text, Email, Password, uid, DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
						sQLiteUtils = null;
					}
					else
					{
						text2 = emailUtils.ReadEmail(text, "hotmail");
					}
					if (text2 != "")
					{
						break;
					}
				}
				Thread.Sleep(1000);
				IWebElement webElement4 = GetWebElement(chrome, By.Id("iOttText"));
				if (webElement4 == null || !(text2 != ""))
				{
					return false;
				}
				SrcollTrueView(chrome, webElement4);
				webElement4.Click();
				webElement4.SendKeys(text2);
				Thread.Sleep(1000);
				IWebElement webElement5 = GetWebElement(chrome, By.XPath("//*[@value=\"Next\"]"));
				if (webElement5 != null)
				{
					SrcollTrueView(chrome, webElement5);
					webElement5.Click();
					Thread.Sleep(3000);
					string pageSource = GetPageSource(chrome);
					if (pageSource.Contains("@" + text.Split('@')[1]))
					{
						IWebElement webElement6 = GetWebElement(chrome, By.XPath("//*[@aria-describedby=\"idDiv_SAOTCS_Title\"]"));
						if (webElement6 != null && webElement6.Enabled)
						{
							SrcollTrueView(chrome, webElement6);
							webElement6.Click();
							Thread.Sleep(1000);
							IWebElement webElement7 = GetWebElement(chrome, By.Id("idTxtBx_SAOTCS_ProofConfirmation"));
							webElement7.SendKeys(text);
							Thread.Sleep(1000);
							webElement7 = GetWebElement(chrome, By.Id("idSubmit_SAOTCS_SendCode"));
							webElement7.Click();
							Thread.Sleep(2000);
							for (int j = 0; j < 10; j++)
							{
								bool flag2 = false;
								foreach (string getneda2 in EmailUtils.getnedaList)
								{
									if (text.Contains(getneda2))
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									text2 = emailUtils.GetGenedaCode(text, 7);
									if (text2 != "")
									{
										if (sQLiteUtils == null)
										{
											sQLiteUtils = new SQLiteUtils();
										}
										sQLiteUtils.ExecuteQuery(string.Format("Update Account Set EmailRecovery='{0}' where Email= '{1}';Update LogEmail Set EmailRecovery='{0}' where Email= '{1}'", text, Email));
									}
								}
								else
								{
									text2 = emailUtils.ReadEmail(text, "hotmail", 7);
								}
								if (text2 != "")
								{
									break;
								}
								Thread.Sleep(3000);
							}
							if (!(text2 != ""))
							{
								return false;
							}
							webElement7 = GetWebElement(chrome, By.Id("idTxtBx_SAOTCC_OTC"));
							webElement7.SendKeys(text2);
							Thread.Sleep(1000);
							webElement7 = GetWebElement(chrome, By.Id("idSubmit_SAOTCC_Continue"));
							webElement7.Click();
							Thread.Sleep(5000);
							IWebElement webElement8 = GetWebElement(chrome, By.Id("iCancel"));
							if (webElement8 != null)
							{
								SrcollTrueView(chrome, webElement8);
								webElement8.Click();
								Thread.Sleep(5000);
							}
							chrome.m_driver.Navigate().GoToUrl("https://account.live.com/proofs/Manage?mkt=en-us&uiflavor=web&id=38936");
							return true;
						}
					}
				}
			}
			else
			{
				string arg = text.Split('@')[1];
				if (GetPageSource(chrome).Contains(text.Split('@')[1]))
				{
					IWebElement webElement9 = GetWebElement(chrome, By.XPath($"//*[contains(text(),'{arg}')]"));
					if (webElement9 != null)
					{
						IWebElement webElement10 = GetWebElement(chrome, By.Id("idDiv_SAOTCS_Proofs"));
						if (webElement10 != null)
						{
							SrcollTrueView(chrome, webElement10);
							webElement10.Click();
							Thread.Sleep(2000);
							IWebElement webElement11 = GetWebElement(chrome, By.Id("idTxtBx_SAOTCS_ProofConfirmation"));
							if (webElement11 != null)
							{
								SrcollTrueView(chrome, webElement11);
								webElement11.SendKeys(text);
								Thread.Sleep(1000);
								IWebElement webElement12 = GetWebElement(chrome, By.Id("idSubmit_SAOTCS_SendCode"));
								if (webElement12 != null)
								{
									SrcollTrueView(chrome, webElement12);
									webElement12.Click();
									Thread.Sleep(5000);
									string text3 = "";
									for (int k = 0; k < 5; k++)
									{
										bool flag3 = false;
										foreach (string getneda3 in EmailUtils.getnedaList)
										{
											if (text.Contains(getneda3))
											{
												flag3 = true;
												break;
											}
										}
										text3 = ((!flag3) ? emailUtils.ReadEmail(text, "hotmail", 7) : emailUtils.GetGenedaCode(text, 7));
										if (text3 != "")
										{
											break;
										}
									}
									if (text3 != "")
									{
										IWebElement webElement13 = GetWebElement(chrome, By.Id("idTxtBx_SAOTCC_OTC"));
										if (webElement13 != null)
										{
											SrcollTrueView(chrome, webElement13);
											webElement13.SendKeys(text3);
											Thread.Sleep(5000);
											IWebElement webElement14 = GetWebElement(chrome, By.Id("idSubmit_SAOTCC_Continue"));
											if (webElement14 != null)
											{
												webElement14.Click();
												Thread.Sleep(2000);
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					string pageSource2 = GetPageSource(chrome);
					if (pageSource2.Contains("idA_SAOTCS_LostProofs"))
					{
						IWebElement webElement15 = GetWebElement(chrome, By.Id("idA_SAOTCS_LostProofs"));
						if (webElement15 != null)
						{
							webElement15.Click();
							Thread.Sleep(1000);
							webElement15 = GetWebElement(chrome, By.Id("idSubmit_SAOTCS_SendCode"));
							if (webElement15 != null)
							{
								webElement15.Click();
								Thread.Sleep(1000);
								if (GetPageSource(chrome).Contains("have access to your current security"))
								{
									IWebElement webElement16 = GetWebElement(chrome, By.Id("iProofOptions"));
									if (webElement16 != null)
									{
										webElement16.Click();
										Thread.Sleep(1000);
										ReadOnlyCollection<IWebElement> readOnlyCollection = webElement16.FindElements(By.TagName("option"));
										if (readOnlyCollection != null && readOnlyCollection.Count > 0)
										{
											readOnlyCollection[readOnlyCollection.Count - 1].Click();
											Thread.Sleep(1000);
											webElement16 = GetWebElement(chrome, By.Id("DisplayPhoneCountryISO"));
											if (webElement16 != null)
											{
												webElement16.Click();
												Thread.Sleep(1000);
												readOnlyCollection = webElement16.FindElements(By.TagName("option"));
												if (readOnlyCollection != null && readOnlyCollection.Count > 0)
												{
													int num = readOnlyCollection.Count - 1;
													while (num > 0)
													{
														if (!readOnlyCollection[num].GetAttribute("value").ToString().Equals("VN"))
														{
															num--;
															continue;
														}
														readOnlyCollection[num].Click();
														break;
													}
													Thread.Sleep(1000);
												}
											}
											string text4 = "";
											IWebElement webElement17 = GetWebElement(chrome, By.Id("DisplayPhoneNumber"));
											if (webElement17 != null)
											{
												CodeResult phone = PhoneUtils.GetPhone(ServiceType.Microsoft);
												webElement17.SendKeys(phone.PhoneOrEmail);
												text4 = phone.SessionId;
												Thread.Sleep(1000);
												webElement17 = GetWebElement(chrome, By.Id("iCollectProofAction"));
												SrcollTrueView(chrome, webElement17);
												if (webElement17 != null)
												{
													webElement17.Click();
													Thread.Sleep(2000);
													if (GetPageSource(chrome).Contains("iOttText"))
													{
														string text5 = "";
														int num2 = 0;
														while (text5 == "" && num2++ < 20)
														{
															text5 = PhoneUtils.GetCode(text4).Code;
															if (text5 != "")
															{
																break;
															}
															Thread.Sleep(5000);
														}
														if (text5 != "")
														{
															GetWebElement(chrome, By.Id("iOttText")).SendKeys(text5);
															Thread.Sleep(1000);
															GetWebElement(chrome, By.Id("iVerifyProofAction")).Click();
															Thread.Sleep(1000);
															GetWebElement(chrome, By.Id("iStartWaitAction")).Click();
															Thread.Sleep(1000);
															if (!GetPageSource(chrome).Contains("loginfmt"))
															{
																return true;
															}
															return false;
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
			return true;
		}

		public bool ChangePassword(string newPassword)
		{
			chrome.m_driver.Navigate().GoToUrl("https://account.live.com/password/Change?mkt=en-US&refd=account.microsoft.com&refp=security&client_flight=shhelp,shhidefam");
			Thread.Sleep(5000);
			if (GetPageSource(chrome).Contains("Change your password"))
			{
				IWebElement webElement = GetWebElement(chrome, By.Id("iCurPassword"));
				if (webElement != null)
				{
					SrcollTrueView(chrome, webElement);
					webElement.SendKeys(Password);
					Thread.Sleep(2000);
				}
				newPassword = (string.IsNullOrEmpty(newPassword) ? Guid.NewGuid().ToString("N").Substring(0, 10) : newPassword);
				IWebElement webElement2 = GetWebElement(chrome, By.Id("iPassword"));
				if (webElement2 != null)
				{
					SrcollTrueView(chrome, webElement2);
					webElement2.SendKeys(newPassword);
					Thread.Sleep(2000);
				}
				IWebElement webElement3 = GetWebElement(chrome, By.Id("iRetypePassword"));
				if (webElement3 != null)
				{
					SrcollTrueView(chrome, webElement3);
					webElement3.SendKeys(newPassword);
					Thread.Sleep(2000);
				}
				IWebElement webElement4 = GetWebElement(chrome, By.Id("UpdatePasswordAction"));
				if (webElement4 != null)
				{
					SrcollTrueView(chrome, webElement4);
					webElement4.Click();
					SQLiteUtils sQLiteUtils = new SQLiteUtils();
					sQLiteUtils.ExecuteQuery($"Update Account Set PassEmail='{newPassword}' where Email= '{Email}'");
					Thread.Sleep(10000);
				}
			}
			return true;
		}

		private void SrcollTrueView(FBChrome chrome, IWebElement elm)
		{
			((IJavaScriptExecutor)chrome.m_driver).ExecuteScript("arguments[0].scrollIntoView(true);", new object[1] { elm });
			Thread.Sleep(500);
		}

		public bool RecoveryByMail(Hotmail email)
		{
			return true;
		}

		private IWebElement GetWebElement(FBChrome chrome, By by)
		{
			try
			{
				return chrome.m_driver.FindElement(by);
			}
			catch
			{
				return null;
			}
		}

		private string GetPageSource(FBChrome chrome)
		{
			string pageSource = chrome.m_driver.PageSource;
			if (pageSource.Contains(">Continue<"))
			{
				IWebElement webElement = GetWebElement(chrome, By.XPath("//span[text()='Continue']"));
				if (webElement != null)
				{
					webElement.Click();
					Thread.Sleep(3000);
					pageSource = chrome.m_driver.PageSource;
				}
				webElement = null;
			}
			return pageSource;
		}
	}
}
