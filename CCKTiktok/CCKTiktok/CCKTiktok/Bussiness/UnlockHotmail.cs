using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.DAL;
using CCKTiktok.Entity;
using OpenQA.Selenium;

namespace CCKTiktok.Bussiness
{
	public class UnlockHotmail
	{
		public void ChangePassword(FBChrome chrome, string id, string email, string pass)
		{
			chrome.m_driver.Navigate().GoToUrl("https://account.live.com/password/Change?mkt=en-US&refd=account.microsoft.com&refp=security&client_flight=shhelp,shhidefam");
			Thread.Sleep(5000);
			if (GetPageSource(chrome).Contains("Change your password"))
			{
				IWebElement webElement = GetWebElement(chrome, By.Id("iCurPassword"));
				if (webElement != null)
				{
					SrcollTrueView(chrome, webElement);
					webElement.SendKeys(pass);
					Thread.Sleep(2000);
				}
				pass = Guid.NewGuid().ToString("N").Substring(0, 10);
				IWebElement webElement2 = GetWebElement(chrome, By.Id("iPassword"));
				if (webElement2 != null)
				{
					SrcollTrueView(chrome, webElement2);
					webElement2.SendKeys(pass);
					Thread.Sleep(2000);
				}
				IWebElement webElement3 = GetWebElement(chrome, By.Id("iRetypePassword"));
				if (webElement3 != null)
				{
					SrcollTrueView(chrome, webElement3);
					webElement3.SendKeys(pass);
					Thread.Sleep(2000);
				}
				IWebElement webElement4 = GetWebElement(chrome, By.Id("UpdatePasswordAction"));
				if (webElement4 != null)
				{
					SrcollTrueView(chrome, webElement4);
					webElement4.Click();
					SQLiteUtils sQLiteUtils = new SQLiteUtils();
					sQLiteUtils.ExecuteQuery($"Update Account Set PassEmail='{pass}' where Email= '{email}'");
					Thread.Sleep(10000);
				}
			}
		}

		public void SrcollTrueView(FBChrome chrome, IWebElement elm)
		{
			((IJavaScriptExecutor)chrome.m_driver).ExecuteScript("arguments[0].scrollIntoView(true);", new object[1] { elm });
			Thread.Sleep(500);
		}

		public void UpdateStatus(string searchValue, string msg, bool showwDate = false, string column = "Status", DataGridViewRow row = null)
		{
			try
			{
				row.Cells[column].Value = msg;
				if (showwDate)
				{
					row.Cells["Date"].Value = DateTime.Now;
				}
			}
			catch (Exception)
			{
			}
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

		public void DoUnlock(bool addrecover, DataGridViewRow SelectedRow, bool changepass)
		{
			Utils.CCKLog("ccccc", "ddddd");
			if (SelectedRow == null)
			{
				return;
			}
			FBChrome fBChrome = new FBChrome();
			string text = "";
			string text2 = "";
			string text3 = "";
			try
			{
				if (SelectedRow.Cells["Email"].Value == null)
				{
					return;
				}
				string text4 = SelectedRow.Cells["Email"].Value.ToString();
				text2 = SelectedRow.Cells["Pass"].Value.ToString();
				text = SelectedRow.Cells["Email"].Value.ToString();
				try
				{
					text3 = SelectedRow.Cells["EmailRecovery"].Value.ToString();
				}
				catch
				{
				}
				if (File.Exists(CaChuaConstant.NETWORK))
				{
					JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
					javaScriptSerializer.MaxJsonLength = int.MaxValue;
					javaScriptSerializer.Deserialize<Networking>(Utils.ReadTextFile(CaChuaConstant.NETWORK));
				}
				string proxyIp = "";
				fBChrome.Init("https://login.live.com/", text4, capchat: false, proxyIp);
				if (GetPageSource(fBChrome).Contains("loginfmt"))
				{
					while (true)
					{
						IWebElement webElement = GetWebElement(fBChrome, By.Name("loginfmt"));
						if (webElement == null)
						{
							break;
						}
						SrcollTrueView(fBChrome, webElement);
						webElement.SendKeys(text4);
						IWebElement webElement2 = GetWebElement(fBChrome, By.XPath("//*[@value=\"Next\"]"));
						if (webElement2 == null)
						{
							break;
						}
						SrcollTrueView(fBChrome, webElement2);
						webElement2.Click();
						int num = 0;
						while (num < 60)
						{
							num++;
							Thread.Sleep(1000);
							IWebElement webElement3 = GetWebElement(fBChrome, By.Name("passwd"));
							if (webElement3 != null)
							{
								SrcollTrueView(fBChrome, webElement3);
								webElement3.SendKeys(text2);
								break;
							}
						}
						webElement2 = GetWebElement(fBChrome, By.XPath("//*[@value=\"Sign in\"]"));
						if (webElement2 == null)
						{
							break;
						}
						SrcollTrueView(fBChrome, webElement2);
						webElement2.Click();
						Thread.Sleep(5000);
						string pageSource = GetPageSource(fBChrome);
						if (pageSource.Contains(">Continue<"))
						{
							webElement2 = GetWebElement(fBChrome, By.XPath("//span[text()='Continue']"));
							if (webElement2 != null)
							{
								webElement2.Click();
								Thread.Sleep(3000);
								pageSource = GetPageSource(fBChrome);
							}
						}
						if (pageSource.Contains("updating our terms"))
						{
							webElement2 = GetWebElement(fBChrome, By.Id("iNext"));
							if (webElement2 != null)
							{
								webElement2.Click();
								Thread.Sleep(1000);
								pageSource = GetPageSource(fBChrome);
							}
						}
						if (!pageSource.Contains("Your account or password is incorrect"))
						{
							if (!pageSource.Contains("Your account has been locked") || !pageSource.Contains("Unlocking your account") || !pageSource.Contains("aka.ms/compliancelock"))
							{
								if (pageSource.Contains("Your account has been locked") && pageSource.Contains("Unlocking your account"))
								{
									IWebElement webElement4 = GetWebElement(fBChrome, By.Id("StartAction"));
									if (webElement4 != null && webElement4.Enabled && webElement4.Displayed)
									{
										SrcollTrueView(fBChrome, webElement4);
										webElement4.Click();
										Thread.Sleep(1000);
										num = 0;
										while (!GetPageSource(fBChrome).Contains("Enter your phone number") && num < 30)
										{
											num++;
											Thread.Sleep(2000);
										}
										UnlockPhone(fBChrome, SelectedRow);
									}
								}
								else if (pageSource.Contains("Help us secure your account") && pageSource.Contains("iLandingViewAction"))
								{
									IWebElement webElement5 = GetWebElement(fBChrome, By.Id("iLandingViewAction"));
									if (webElement5 != null)
									{
										SrcollTrueView(fBChrome, webElement5);
										webElement5.Click();
										Thread.Sleep(1000);
										pageSource = GetPageSource(fBChrome);
										SQLiteUtils sQLiteUtils = new SQLiteUtils();
										IWebElement webElement6 = GetWebElement(fBChrome, By.Id("iProofEmail"));
										if (webElement6 != null && pageSource.Contains("complete the hidden part and click"))
										{
											SrcollTrueView(fBChrome, webElement6);
											EmailUtils emailUtils = new EmailUtils();
											string text5 = "";
											bool flag = false;
											string text6 = "";
											foreach (string getneda in EmailUtils.getnedaList)
											{
												if (pageSource.Contains(getneda))
												{
													flag = true;
													text6 = getneda;
													break;
												}
											}
											DataTable dataTable = sQLiteUtils.ExecuteQuery($"Select EmailRecovery from Account where Email= '{text4}'");
											string text7 = text4.Split('@')[0] + text6;
											if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0]["EmailRecovery"].ToString().Length > 0)
											{
												text7 = dataTable.Rows[0]["EmailRecovery"].ToString();
											}
											if (GetPageSource(fBChrome).Contains("name=\"proof\""))
											{
												IWebElement webElement7 = GetWebElement(fBChrome, By.Name("proof"));
												if (webElement7 != null)
												{
													webElement7.Click();
													Thread.Sleep(1000);
												}
											}
											webElement6 = GetWebElement(fBChrome, By.Id("iProofEmail"));
											webElement6.SendKeys(text7.Split('@')[0]);
											Thread.Sleep(1000);
											IWebElement webElement8 = GetWebElement(fBChrome, By.Id("iSelectProofAction"));
											if (webElement8 != null)
											{
												SrcollTrueView(fBChrome, webElement8);
												webElement8.Click();
												Thread.Sleep(5000);
												if (flag && text6 != "")
												{
													UpdateStatus(text4, "Waiting code getnada.com", showwDate: false, "Note", SelectedRow);
													text5 = emailUtils.GetGenedaCode(text7, 7);
													if (text5 == "")
													{
														UpdateStatus(text4, "No code from email", showwDate: false, "Note", SelectedRow);
													}
												}
												else
												{
													string to = text4.Split('@')[0] + "@" + Utils.ReadTextFile(CaChuaConstant.DOMAIN_CONFIG).Replace("@", "");
													for (int i = 0; i < 5; i++)
													{
														UpdateStatus(text4, "Waiting code hotmail", showwDate: false, "Note", SelectedRow);
														text5 = emailUtils.ReadEmail(to, "hotmail", 7);
														if (text5 != "")
														{
															break;
														}
													}
												}
												IWebElement webElement9 = GetWebElement(fBChrome, By.Id("iOttText"));
												if (webElement9 != null && text5 != "")
												{
													SrcollTrueView(fBChrome, webElement9);
													webElement9.SendKeys(text5);
													Thread.Sleep(1000);
													IWebElement webElement10 = GetWebElement(fBChrome, By.Id("iVerifyCodeAction"));
													if (webElement10 != null)
													{
														SrcollTrueView(fBChrome, webElement10);
														webElement10.Click();
														Thread.Sleep(2000);
														IWebElement webElement11 = GetWebElement(fBChrome, By.Id("iPassword"));
														if (webElement11 != null)
														{
															SrcollTrueView(fBChrome, webElement11);
															webElement11.Click();
															text2 = Guid.NewGuid().ToString("N").Substring(0, 10);
															SrcollTrueView(fBChrome, webElement11);
															webElement11.SendKeys(text2);
															Thread.Sleep(1000);
															webElement10 = GetWebElement(fBChrome, By.Id("iPasswordViewAction"));
															if (webElement10 != null)
															{
																SrcollTrueView(fBChrome, webElement10);
																webElement10.Click();
																Thread.Sleep(3000);
																File.AppendAllLines("outlook.txt", new List<string> { text4 + "|" + text2 });
																webElement10 = GetWebElement(fBChrome, By.Id("iReviewProofsViewAction"));
																if (webElement10 != null)
																{
																	SrcollTrueView(fBChrome, webElement10);
																	webElement10.Click();
																	sQLiteUtils.ExecuteQuery($"Update Account Set PassEmail='{text2}' where Email= '{text4}'");
																	Thread.Sleep(2000);
																	if (GetPageSource(fBChrome).Contains("iCollectProofsViewAlternate"))
																	{
																		webElement10 = GetWebElement(fBChrome, By.Id("iCollectProofsViewAlternate"));
																		if (webElement10 != null)
																		{
																			SrcollTrueView(fBChrome, webElement10);
																			webElement10.Click();
																			Thread.Sleep(1000);
																			webElement10 = GetWebElement(fBChrome, By.Id("iFinishViewAction"));
																			if (webElement10 != null)
																			{
																				SrcollTrueView(fBChrome, webElement10);
																				webElement10.Click();
																				Thread.Sleep(1000);
																				pageSource = GetPageSource(fBChrome);
																				if (pageSource.Contains("you need to verify your password"))
																				{
																					Thread.Sleep(2000);
																					IWebElement webElement12 = GetWebElement(fBChrome, By.Name("passwd"));
																					if (webElement12 != null)
																					{
																						SrcollTrueView(fBChrome, webElement12);
																						webElement12.SendKeys(text2);
																						SelectedRow.Cells["Pass"].Value = text2;
																						Thread.Sleep(1000);
																						IWebElement webElement13 = GetWebElement(fBChrome, By.XPath("//*[contains(@id,'idSIButton')]"));
																						if (webElement13 != null)
																						{
																							SrcollTrueView(fBChrome, webElement13);
																							webElement13.Click();
																							Thread.Sleep(2000);
																							webElement13 = GetWebElement(fBChrome, By.XPath("//*[contains(@id,'idSIButton')]"));
																							if (webElement13 != null)
																							{
																								SrcollTrueView(fBChrome, webElement13);
																								webElement13.Click();
																								Thread.Sleep(2000);
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																	fBChrome.m_driver.Navigate().GoToUrl("https://outlook.live.com/mail/0/");
																}
															}
														}
													}
												}
												else if (text5 == "")
												{
												}
											}
										}
										else if (pageSource.Contains("DisplayPhoneNumber"))
										{
											Thread.Sleep(1000);
											IWebElement webElement14 = GetWebElement(fBChrome, By.TagName("select"));
											if (webElement14 != null)
											{
												SrcollTrueView(fBChrome, webElement14);
												ReadOnlyCollection<IWebElement> readOnlyCollection = webElement14.FindElements(By.TagName("option"));
												for (int j = 0; j < readOnlyCollection.Count; j++)
												{
													if (readOnlyCollection[j].Text.Contains("Vietnam"))
													{
														readOnlyCollection[j].Click();
														break;
													}
												}
												string text8 = "";
												num = 0;
												while (true)
												{
													IWebElement webElement15 = GetWebElement(fBChrome, By.Id("DisplayPhoneNumber"));
													SrcollTrueView(fBChrome, webElement15);
													webElement15.Click();
													webElement15.Clear();
													CodeResult phone = PhoneUtils.GetPhone(ServiceType.Microsoft);
													if (!(phone.PhoneOrEmail != ""))
													{
														break;
													}
													text8 = phone.SessionId;
													webElement15.SendKeys(phone.PhoneOrEmail);
													Thread.Sleep(3000);
													IWebElement webElement16 = GetWebElement(fBChrome, By.Id("iCollectPhoneViewAction"));
													if (webElement16 != null)
													{
														SrcollTrueView(fBChrome, webElement16);
														webElement16.Click();
														Thread.Sleep(5000);
														pageSource = GetPageSource(fBChrome);
														string text9 = "";
														num = 0;
														while (text9 == "" && num < 60)
														{
															num++;
															text9 = PhoneUtils.GetCode(text8).Code;
															if (text9 != "")
															{
																break;
															}
															Thread.Sleep(3000);
														}
														if (!(text9 != ""))
														{
															IWebElement webElement17 = GetWebElement(fBChrome, By.XPath("//*[contains(@id,'wlspispHIPNotGetCode')]"));
															if (webElement17 != null)
															{
																SrcollTrueView(fBChrome, webElement17);
																webElement17.Click();
																Thread.Sleep(5000);
																continue;
															}
															break;
														}
														webElement15 = GetWebElement(fBChrome, By.Id("iOttText"));
														SrcollTrueView(fBChrome, webElement15);
														webElement15.SendKeys(text9);
														Thread.Sleep(1000);
														IWebElement webElement18 = GetWebElement(fBChrome, By.Id("iVerifyPhoneViewAction"));
														if (webElement18 == null)
														{
															break;
														}
														SrcollTrueView(fBChrome, webElement18);
														webElement18.Click();
														Thread.Sleep(10000);
														text2 = Guid.NewGuid().ToString("N").Substring(0, 10);
														IWebElement webElement19 = GetWebElement(fBChrome, By.Id("iPassword"));
														if (webElement19 == null)
														{
															break;
														}
														SrcollTrueView(fBChrome, webElement19);
														webElement19.Click();
														webElement19.SendKeys(text2);
														Thread.Sleep(3000);
														IWebElement webElement20 = GetWebElement(fBChrome, By.Id("iPasswordViewAction"));
														if (webElement20 == null)
														{
															break;
														}
														SrcollTrueView(fBChrome, webElement20);
														webElement20.Click();
														Thread.Sleep(3000);
														webElement20 = GetWebElement(fBChrome, By.Id("iReviewProofsViewAction"));
														if (webElement20 == null)
														{
															break;
														}
														SrcollTrueView(fBChrome, webElement20);
														webElement20.Click();
														Thread.Sleep(3000);
														try
														{
															SelectedRow.Cells["Pass"].Value = text2;
														}
														catch
														{
														}
														sQLiteUtils.ExecuteQuery($"Update Account Set PassEmail='{text2}' where Email= '{text4}'");
														pageSource = GetPageSource(fBChrome);
														if (pageSource.Contains("Add more security info"))
														{
															EmailUtils emailUtils2 = new EmailUtils();
															string text10 = emailUtils2.CreateGenedaEmail(text4);
															IWebElement webElement21 = GetWebElement(fBChrome, By.Id("EmailAddress"));
															webElement21.SendKeys(text10);
															Thread.Sleep(2000);
															IWebElement webElement22 = GetWebElement(fBChrome, By.Id("iCollectProofsViewAction"));
															if (webElement22 != null)
															{
																SrcollTrueView(fBChrome, webElement22);
																webElement22.Click();
																Thread.Sleep(2000);
																pageSource = GetPageSource(fBChrome);
																if (pageSource.Contains("Password updated"))
																{
																	webElement22 = GetWebElement(fBChrome, By.Id("iFinishViewAction"));
																	if (webElement22 != null)
																	{
																		SrcollTrueView(fBChrome, webElement22);
																		webElement22.Click();
																		Thread.Sleep(2000);
																		IWebElement webElement23 = GetWebElement(fBChrome, By.Name("passwd"));
																		if (webElement23 != null)
																		{
																			SrcollTrueView(fBChrome, webElement23);
																			webElement23.SendKeys(text2);
																			Thread.Sleep(1000);
																			IWebElement webElement24 = GetWebElement(fBChrome, By.XPath("//*[contains(@id,'idSIButton')]"));
																			if (webElement24 != null)
																			{
																				SrcollTrueView(fBChrome, webElement24);
																				webElement24.Click();
																				Thread.Sleep(2000);
																				webElement24 = GetWebElement(fBChrome, By.XPath("//*[contains(@id,'idSIButton')]"));
																				if (webElement24 != null)
																				{
																					SrcollTrueView(fBChrome, webElement24);
																					webElement24.Click();
																					Thread.Sleep(2000);
																				}
																			}
																		}
																	}
																}
															}
															text9 = "";
														}
														fBChrome.m_driver.Navigate().GoToUrl("https://outlook.live.com/mail/0/");
														break;
													}
													if (GetPageSource(fBChrome).Contains("wlspispHIPInvalidRequest"))
													{
														fBChrome.m_driver.Quit();
													}
													break;
												}
												return;
											}
										}
									}
									else if (pageSource.Contains("DisplayPhoneNumber"))
									{
										Thread.Sleep(1000);
										IWebElement webElement25 = GetWebElement(fBChrome, By.TagName("select"));
										if (webElement25 != null)
										{
											SrcollTrueView(fBChrome, webElement25);
											ReadOnlyCollection<IWebElement> readOnlyCollection2 = webElement25.FindElements(By.TagName("option"));
											for (int k = 0; k < readOnlyCollection2.Count; k++)
											{
												if (readOnlyCollection2[k].Text.Contains("Vietnam"))
												{
													readOnlyCollection2[k].Click();
													break;
												}
											}
											string text11 = "";
											num = 0;
											while (true)
											{
												IWebElement webElement26 = GetWebElement(fBChrome, By.Id("DisplayPhoneNumber"));
												SrcollTrueView(fBChrome, webElement26);
												webElement26.Click();
												webElement26.Clear();
												CodeResult phone2 = PhoneUtils.GetPhone(ServiceType.Microsoft);
												if (phone2 != null && phone2.PhoneOrEmail != "")
												{
													text11 = phone2.SessionId;
													SrcollTrueView(fBChrome, webElement26);
													webElement26.SendKeys(phone2.PhoneOrEmail);
													Thread.Sleep(1000);
													IWebElement webElement27 = GetWebElement(fBChrome, By.PartialLinkText("Send code"));
													if (webElement27 == null)
													{
														break;
													}
													SrcollTrueView(fBChrome, webElement27);
													webElement27.Click();
													Thread.Sleep(5000);
													pageSource = GetPageSource(fBChrome);
													if (pageSource.Contains("display: none;\">Usage limit exceeded. Try again tomorrow.<"))
													{
														string text12 = "";
														num = 0;
														while (text12 == "" && num < 60)
														{
															num++;
															text12 = PhoneUtils.GetCode(text11).Code;
															if (text12 != "")
															{
																break;
															}
															Thread.Sleep(3000);
														}
														if (text12 != "")
														{
															webElement26.SendKeys(text12);
															Thread.Sleep(1000);
															IWebElement webElement28 = GetWebElement(fBChrome, By.Id("ProofAction"));
															if (webElement28 != null)
															{
																SrcollTrueView(fBChrome, webElement28);
																webElement28.Click();
																Thread.Sleep(3000);
																webElement28 = GetWebElement(fBChrome, By.Id("FinishAction"));
																if (webElement28 != null)
																{
																	SrcollTrueView(fBChrome, webElement28);
																	webElement28.Click();
																	Thread.Sleep(3000);
																}
															}
															break;
														}
														IWebElement webElement29 = GetWebElement(fBChrome, By.XPath("//*[contains(@id,'wlspispHIPNotGetCode')]"));
														if (webElement29 == null)
														{
															break;
														}
														SrcollTrueView(fBChrome, webElement29);
														webElement29.Click();
														Thread.Sleep(5000);
													}
													else
													{
														Thread.Sleep(5000);
														num++;
														if (num >= 5)
														{
															break;
														}
													}
													continue;
												}
												webElement26.SendKeys("Hết số");
												Thread.Sleep(5000);
												break;
											}
											return;
										}
									}
								}
								else if ((pageSource.Contains("Help us protect your account") && pageSource.Contains("iProof0")) || pageSource.Contains("Verify your identity"))
								{
									IWebElement webElement30 = GetWebElement(fBChrome, By.Id("iProof0"));
									if (webElement30 != null)
									{
										SrcollTrueView(fBChrome, webElement30);
										webElement30.Click();
										Thread.Sleep(1000);
										IWebElement webElement31 = GetWebElement(fBChrome, By.Id("iProofEmail"));
										if (webElement31 == null)
										{
											return;
										}
										SrcollTrueView(fBChrome, webElement31);
										if (text3 != "")
										{
											text4 = text3.Split('@')[0];
										}
										webElement31.SendKeys(text4.Split('@')[0]);
										Thread.Sleep(1000);
										IWebElement webElement32 = GetWebElement(fBChrome, By.Id("iSelectProofAction"));
										if (webElement32 != null)
										{
											SrcollTrueView(fBChrome, webElement32);
											webElement32.Click();
											Thread.Sleep(5000);
											string text13 = "";
											string text14 = GetWebElement(fBChrome, By.Id("iEnterSubhead")).Text;
											List<string> list = text14.Split(" ".ToCharArray()).ToList();
											foreach (string item in list)
											{
												if (item.Contains("@"))
												{
													text13 = item;
													break;
												}
											}
											EmailUtils emailUtils3 = new EmailUtils();
											string text15 = "";
											bool flag2 = false;
											foreach (string getneda2 in EmailUtils.getnedaList)
											{
												if (text13.Contains(getneda2))
												{
													flag2 = true;
													break;
												}
											}
											if (flag2)
											{
												text15 = emailUtils3.GetGenedaCode(text13, 7);
												new SQLiteUtils();
											}
											else
											{
												string[] array = text13.Split("@".ToCharArray());
												if (array.Length == 2)
												{
													for (int l = 0; l < 5; l++)
													{
														text15 = emailUtils3.ReadEmail(text13, "hotmail", 7);
														if (text15 != "")
														{
															break;
														}
													}
												}
											}
											if (text15 != "")
											{
												GetWebElement(fBChrome, By.Id("iOttText"))?.SendKeys(text15);
												Thread.Sleep(1000);
												GetWebElement(fBChrome, By.Id("iVerifyCodeAction"))?.Click();
												Thread.Sleep(1000);
												GetWebElement(fBChrome, By.XPath("//*[contains(@id, 'idSIButton')]"))?.Click();
												Thread.Sleep(1000);
											}
										}
									}
								}
								else if (!pageSource.Contains("security info change is still pending"))
								{
								}
								pageSource = GetPageSource(fBChrome);
								if (!pageSource.Contains("Stay signed in"))
								{
									break;
								}
								webElement2 = GetWebElement(fBChrome, By.XPath("//*[@value=\"Yes\"]"));
								if (webElement2 == null)
								{
									break;
								}
								SrcollTrueView(fBChrome, webElement2);
								webElement2.Click();
								Thread.Sleep(2000);
								fBChrome.m_driver.Navigate().GoToUrl("https://account.microsoft.com/");
								Thread.Sleep(2000);
								if (!addrecover)
								{
									break;
								}
								SelectedRow.Cells["Note"].Value = "Add mail recovery";
								fBChrome.m_driver.Navigate().GoToUrl("https://account.microsoft.com/security");
								Thread.Sleep(5000);
								IWebElement webElement33 = fBChrome.m_driver.FindElementByXPath("//*[contains(@href, '/proofs/manage/additional')]");
								if (webElement33 != null)
								{
									SrcollTrueView(fBChrome, webElement33);
									webElement33.Click();
								}
								else
								{
									fBChrome.m_driver.Navigate().GoToUrl("https://account.live.com/proofs/manage/additional?mkt=en-US&refd=account.microsoft.com&refp=security&client_flight=shhelp,shhidefam");
								}
								Thread.Sleep(2000);
								EmailUtils emailUtils4 = new EmailUtils();
								string text16 = emailUtils4.CreateGenedaEmail(text4);
								if (!GetPageSource(fBChrome).Contains("EmailAddress"))
								{
									string arg = text16.Split('@')[1];
									if (!GetPageSource(fBChrome).Contains(text16.Split('@')[1]))
									{
										pageSource = GetPageSource(fBChrome);
										if (!pageSource.Contains("idA_SAOTCS_LostProofs"))
										{
											break;
										}
										IWebElement webElement34 = GetWebElement(fBChrome, By.Id("idA_SAOTCS_LostProofs"));
										if (webElement34 == null)
										{
											break;
										}
										webElement34.Click();
										Thread.Sleep(1000);
										webElement34 = GetWebElement(fBChrome, By.Id("idSubmit_SAOTCS_SendCode"));
										if (webElement34 == null)
										{
											break;
										}
										webElement34.Click();
										Thread.Sleep(1000);
										if (!GetPageSource(fBChrome).Contains("have access to your current security"))
										{
											break;
										}
										IWebElement webElement35 = GetWebElement(fBChrome, By.Id("iProofOptions"));
										if (webElement35 == null)
										{
											break;
										}
										webElement35.Click();
										Thread.Sleep(1000);
										ReadOnlyCollection<IWebElement> readOnlyCollection3 = webElement35.FindElements(By.TagName("option"));
										if (readOnlyCollection3 == null || readOnlyCollection3.Count <= 0)
										{
											break;
										}
										readOnlyCollection3[readOnlyCollection3.Count - 1].Click();
										Thread.Sleep(1000);
										webElement35 = GetWebElement(fBChrome, By.Id("DisplayPhoneCountryISO"));
										if (webElement35 != null)
										{
											webElement35.Click();
											Thread.Sleep(1000);
											readOnlyCollection3 = webElement35.FindElements(By.TagName("option"));
											if (readOnlyCollection3 != null && readOnlyCollection3.Count > 0)
											{
												int num2 = readOnlyCollection3.Count - 1;
												while (num2 > 0)
												{
													if (!readOnlyCollection3[num2].GetAttribute("value").ToString().Equals("VN"))
													{
														num2--;
														continue;
													}
													readOnlyCollection3[num2].Click();
													break;
												}
												Thread.Sleep(1000);
											}
										}
										string text17 = "";
										IWebElement webElement36 = GetWebElement(fBChrome, By.Id("DisplayPhoneNumber"));
										if (webElement36 == null)
										{
											break;
										}
										CodeResult phone3 = PhoneUtils.GetPhone(ServiceType.Microsoft);
										webElement36.SendKeys(phone3.PhoneOrEmail);
										text17 = phone3.SessionId;
										if (phone3.PhoneOrEmail == "")
										{
											UpdateStatus(text4, "Out of numbers", showwDate: false, "Note", SelectedRow);
										}
										Thread.Sleep(1000);
										webElement36 = GetWebElement(fBChrome, By.Id("iCollectProofAction"));
										SrcollTrueView(fBChrome, webElement36);
										if (webElement36 == null)
										{
											break;
										}
										webElement36.Click();
										Thread.Sleep(2000);
										if (!GetPageSource(fBChrome).Contains("iOttText"))
										{
											break;
										}
										string text18 = "";
										num = 0;
										while (text18 == "" && num++ < 20)
										{
											text18 = PhoneUtils.GetCode(text17).Code;
											if (text18 != "")
											{
												break;
											}
											Thread.Sleep(5000);
										}
										if (!(text18 != ""))
										{
											break;
										}
										GetWebElement(fBChrome, By.Id("iOttText")).SendKeys(text18);
										Thread.Sleep(1000);
										GetWebElement(fBChrome, By.Id("iVerifyProofAction")).Click();
										Thread.Sleep(1000);
										GetWebElement(fBChrome, By.Id("iStartWaitAction")).Click();
										Thread.Sleep(1000);
										if (!GetPageSource(fBChrome).Contains("loginfmt"))
										{
											UpdateStatus(text4, "Unlocked successfully", showwDate: false, "Note", SelectedRow);
											break;
										}
										continue;
									}
									IWebElement webElement37 = GetWebElement(fBChrome, By.XPath($"//*[contains(text(),'{arg}')]"));
									if (webElement37 == null)
									{
										break;
									}
									IWebElement webElement38 = GetWebElement(fBChrome, By.Id("idDiv_SAOTCS_Proofs"));
									if (webElement38 == null)
									{
										break;
									}
									SrcollTrueView(fBChrome, webElement38);
									webElement38.Click();
									Thread.Sleep(2000);
									IWebElement webElement39 = GetWebElement(fBChrome, By.Id("idTxtBx_SAOTCS_ProofConfirmation"));
									if (webElement39 == null)
									{
										break;
									}
									SrcollTrueView(fBChrome, webElement39);
									webElement39.SendKeys(text16);
									Thread.Sleep(1000);
									IWebElement webElement40 = GetWebElement(fBChrome, By.Id("idSubmit_SAOTCS_SendCode"));
									if (webElement40 == null)
									{
										break;
									}
									SrcollTrueView(fBChrome, webElement40);
									webElement40.Click();
									Thread.Sleep(5000);
									string text19 = "";
									for (int m = 0; m < 5; m++)
									{
										bool flag3 = false;
										foreach (string getneda3 in EmailUtils.getnedaList)
										{
											if (text16.Contains(getneda3))
											{
												flag3 = true;
												break;
											}
										}
										text19 = ((!flag3) ? emailUtils4.ReadEmail(text16, "hotmail", 7) : emailUtils4.GetGenedaCode(text16, 7));
										if (text19 != "")
										{
											break;
										}
									}
									if (!(text19 != ""))
									{
										break;
									}
									IWebElement webElement41 = GetWebElement(fBChrome, By.Id("idTxtBx_SAOTCC_OTC"));
									if (webElement41 != null)
									{
										SrcollTrueView(fBChrome, webElement41);
										webElement41.SendKeys(text19);
										Thread.Sleep(5000);
										IWebElement webElement42 = GetWebElement(fBChrome, By.Id("idSubmit_SAOTCC_Continue"));
										if (webElement42 != null)
										{
											webElement42.Click();
											Thread.Sleep(2000);
										}
									}
									break;
								}
								IWebElement webElement43 = GetWebElement(fBChrome, By.Id("EmailAddress"));
								SrcollTrueView(fBChrome, webElement43);
								webElement43.SendKeys(text16);
								Thread.Sleep(2000);
								SQLiteUtils sQLiteUtils2 = new SQLiteUtils();
								IWebElement webElement44 = GetWebElement(fBChrome, By.Id("iNext"));
								if (webElement44 != null)
								{
									SrcollTrueView(fBChrome, webElement44);
									webElement44.Click();
									sQLiteUtils2.ExecuteQuery(string.Format("Update Account Set EmailRecovery='{0}' where Email= '{1}'; Insert Into LogEmail(uid,email,passemail,date,emailrecovery) values ('{4}','{1}','{2}','{3}','{0}')", text16, text4, text2, text4, DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
									Thread.Sleep(2000);
								}
								string text20 = "";
								for (int n = 0; n < 5; n++)
								{
									bool flag4 = false;
									foreach (string getneda4 in EmailUtils.getnedaList)
									{
										if (text16.Contains(getneda4))
										{
											flag4 = true;
											break;
										}
									}
									if (flag4)
									{
										text20 = emailUtils4.GetGenedaCode(text16, 4);
										sQLiteUtils2 = new SQLiteUtils();
										sQLiteUtils2.ExecuteQuery(string.Format("Update Account Set EmailRecovery='{0}' where Email= '{1}'; Insert Into LogEmail(uid,email,passemail,date,emailrecovery) values ('{4}','{1}','{2}','{3}','{0}')", text16, text4, text2, text4, DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
										sQLiteUtils2 = null;
									}
									else
									{
										text20 = emailUtils4.ReadEmail(text16, "hotmail");
									}
									if (text20 != "")
									{
										break;
									}
								}
								Thread.Sleep(1000);
								IWebElement webElement45 = GetWebElement(fBChrome, By.Id("iOttText"));
								if (webElement45 != null && text20 != "")
								{
									SrcollTrueView(fBChrome, webElement45);
									webElement45.Click();
									webElement45.SendKeys(text20);
									Thread.Sleep(1000);
									webElement2 = GetWebElement(fBChrome, By.XPath("//*[@value=\"Next\"]"));
									if (webElement2 == null)
									{
										break;
									}
									SrcollTrueView(fBChrome, webElement2);
									webElement2.Click();
									Thread.Sleep(3000);
									string pageSource2 = GetPageSource(fBChrome);
									if (!pageSource2.Contains("@" + text16.Split('@')[1]))
									{
										break;
									}
									IWebElement webElement46 = GetWebElement(fBChrome, By.XPath("//*[@aria-describedby=\"idDiv_SAOTCS_Title\"]"));
									if (webElement46 == null || !webElement46.Enabled)
									{
										break;
									}
									SrcollTrueView(fBChrome, webElement46);
									webElement46.Click();
									Thread.Sleep(1000);
									IWebElement webElement47 = GetWebElement(fBChrome, By.Id("idTxtBx_SAOTCS_ProofConfirmation"));
									webElement47.SendKeys(text16);
									Thread.Sleep(1000);
									webElement47 = GetWebElement(fBChrome, By.Id("idSubmit_SAOTCS_SendCode"));
									webElement47.Click();
									Thread.Sleep(2000);
									for (int num3 = 0; num3 < 10; num3++)
									{
										bool flag5 = false;
										foreach (string getneda5 in EmailUtils.getnedaList)
										{
											if (text16.Contains(getneda5))
											{
												flag5 = true;
												break;
											}
										}
										if (flag5)
										{
											text20 = emailUtils4.GetGenedaCode(text16, 7);
											if (text20 != "")
											{
												if (sQLiteUtils2 == null)
												{
													sQLiteUtils2 = new SQLiteUtils();
												}
												sQLiteUtils2.ExecuteQuery(string.Format("Update Account Set EmailRecovery='{0}' where Email= '{1}';Update LogEmail Set EmailRecovery='{0}' where Email= '{1}'", text16, text4));
											}
										}
										else
										{
											text20 = emailUtils4.ReadEmail(text16, "hotmail", 7);
										}
										if (text20 != "")
										{
											break;
										}
										Thread.Sleep(3000);
									}
									if (!(text20 != ""))
									{
										SelectedRow.Cells["Status"].Value = "No Get Code";
										break;
									}
									webElement47 = GetWebElement(fBChrome, By.Id("idTxtBx_SAOTCC_OTC"));
									webElement47.SendKeys(text20);
									Thread.Sleep(1000);
									webElement47 = GetWebElement(fBChrome, By.Id("idSubmit_SAOTCC_Continue"));
									webElement47.Click();
									Thread.Sleep(5000);
									IWebElement webElement48 = GetWebElement(fBChrome, By.Id("iCancel"));
									if (webElement48 != null)
									{
										SrcollTrueView(fBChrome, webElement48);
										webElement48.Click();
										Thread.Sleep(5000);
									}
									fBChrome.m_driver.Navigate().GoToUrl("https://account.live.com/proofs/Manage?mkt=en-us&uiflavor=web&id=38936");
									SelectedRow.Cells["Pass"].Value = text2;
									SelectedRow.Cells["Status"].Value = "Added successful recovery email";
								}
								else
								{
									SelectedRow.Cells["Note"].Value = "No code email";
								}
								break;
							}
							SelectedRow.Cells["Email"].Value = "Your account has been locked";
							UpdateStatus(text4, "Your account has been locked", showwDate: false, "Note", SelectedRow);
							fBChrome.m_driver.Close();
							return;
						}
						UpdateStatus(text4, "Password is incorrect", showwDate: false, "Note", SelectedRow);
						if (!(text3 != "") && !GetPageSource(fBChrome).Contains("@getnada."))
						{
							return;
						}
						IWebElement webElement49 = GetWebElement(fBChrome, By.Id("idA_PWD_ForgotPassword"));
						if (webElement49 != null)
						{
							webElement49.Click();
							Thread.Sleep(1000);
							pageSource = GetPageSource(fBChrome);
							if (pageSource.Contains("We need to verify your identity"))
							{
								IWebElement webElement50 = GetWebElement(fBChrome, By.Id("proofOption0"));
								if (webElement50 != null)
								{
									webElement50.Click();
									Thread.Sleep(1000);
									IWebElement webElement51 = GetWebElement(fBChrome, By.Id("proofInput0"));
									if (webElement51 != null)
									{
										webElement51.Click();
										Thread.Sleep(1000);
										string text21 = text3.Split('@')[0];
										if (text21 == "")
										{
											text21 = text4.Split('@')[0];
										}
										webElement51.SendKeys(text21);
										Thread.Sleep(1000);
										IWebElement webElement52 = GetWebElement(fBChrome, By.Id("iSelectProofAction"));
										if (webElement52 != null)
										{
											SrcollTrueView(fBChrome, webElement52);
											webElement52.Click();
											Thread.Sleep(5000);
											string genedaCode = new EmailUtils().GetGenedaCode(text3, 7);
											if (genedaCode != "")
											{
												IWebElement webElement53 = GetWebElement(fBChrome, By.Id("iVerifyText"));
												webElement53.SendKeys(genedaCode);
												Thread.Sleep(1000);
												IWebElement webElement54 = GetWebElement(fBChrome, By.Id("iVerifyIdentityAction"));
												if (webElement54 != null)
												{
													webElement54.Click();
													Thread.Sleep(5000);
													string text22 = Guid.NewGuid().ToString("N").Substring(0, 10);
													IWebElement webElement55 = GetWebElement(fBChrome, By.Id("iPassword"));
													if (webElement55 != null)
													{
														webElement55.SendKeys(text22);
														Thread.Sleep(1000);
													}
													IWebElement webElement56 = GetWebElement(fBChrome, By.Id("iRetypePassword"));
													if (webElement56 != null)
													{
														webElement56.SendKeys(text22);
														Thread.Sleep(1000);
													}
													IWebElement webElement57 = GetWebElement(fBChrome, By.Id("iResetPasswordAction"));
													if (webElement57 != null)
													{
														webElement57.Click();
														Thread.Sleep(1000);
													}
													continue;
												}
											}
										}
									}
								}
							}
						}
						fBChrome.m_driver.Close();
						fBChrome.m_driver.Quit();
						return;
					}
				}
				fBChrome.m_driver.Navigate().GoToUrl("https://outlook.live.com/mail/0/");
				Thread.Sleep(5000);
				if (changepass)
				{
					fBChrome.m_driver.Navigate().GoToUrl("https://account.microsoft.com/");
					ChangePassword(fBChrome, text4, text, text2);
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog("Unlock hotmail Loi ", ex.Message);
				try
				{
					fBChrome.m_driver.Close();
					fBChrome.m_driver.Quit();
				}
				catch
				{
				}
			}
			finally
			{
				try
				{
					if (fBChrome.m_driver != null)
					{
						fBChrome.m_driver.Quit();
					}
				}
				catch
				{
				}
				fBChrome = null;
			}
		}

		public void UnlockPhone(FBChrome chrome, DataGridViewRow row)
		{
			int num = 0;
			string text = "";
			IWebElement webElement = GetWebElement(chrome, By.TagName("select"));
			if (webElement == null)
			{
				return;
			}
			SrcollTrueView(chrome, webElement);
			ReadOnlyCollection<IWebElement> readOnlyCollection = webElement.FindElements(By.TagName("option"));
			for (int i = 0; i < readOnlyCollection.Count; i++)
			{
				if (readOnlyCollection[i].Text.Contains("Vietnam"))
				{
					readOnlyCollection[i].Click();
					break;
				}
			}
			string text2 = "";
			num = 0;
			while (true)
			{
				ReadOnlyCollection<IWebElement> readOnlyCollection2 = chrome.m_driver.FindElementsByXPath("//input[@type='text']");
				int num2 = 0;
				while (true)
				{
					if (num2 >= readOnlyCollection2.Count)
					{
						return;
					}
					if (readOnlyCollection2[0].Enabled)
					{
						SrcollTrueView(chrome, readOnlyCollection2[0]);
						readOnlyCollection2[0].Click();
						readOnlyCollection2[0].Clear();
						CodeResult phone = PhoneUtils.GetPhone(ServiceType.Microsoft);
						if (!(phone.PhoneOrEmail != ""))
						{
							row.Cells["Note"].Value = "No Phone Number";
							return;
						}
						text2 = phone.SessionId;
						readOnlyCollection2[num2].Clear();
						readOnlyCollection2[num2].SendKeys(phone.PhoneOrEmail);
						row.Cells["Note"].Value = phone;
						Thread.Sleep(1000);
						IWebElement webElement2 = GetWebElement(chrome, By.PartialLinkText("Send code"));
						if (webElement2 == null)
						{
							return;
						}
						SrcollTrueView(chrome, webElement2);
						webElement2.Click();
						Thread.Sleep(5000);
						text = GetPageSource(chrome);
						if (!text.Contains("display: none;\">Usage limit exceeded. Try again tomorrow.<") && !text.Contains("display: inline;\">We cannot send a text message to this number"))
						{
							Thread.Sleep(5000);
							num++;
							if (num < 5)
							{
								break;
							}
						}
						else
						{
							if (!text.Contains("display: inline;\">We cannot send a text message to this number") && !text.Contains("display: inline;\">Invalid request.  Please try again."))
							{
								string text3 = "";
								num = 0;
								while (text3 == "" && num < 30)
								{
									num++;
									text3 = PhoneUtils.GetCode(text2).Code;
									if (text3 != "")
									{
										break;
									}
									Thread.Sleep(3000);
								}
								if (!(text3 != ""))
								{
									IWebElement webElement3 = GetWebElement(chrome, By.XPath("//*[contains(@id,'wlspispHIPNotGetCode')]"));
									if (webElement3 != null)
									{
										SrcollTrueView(chrome, webElement3);
										webElement3.Click();
										Thread.Sleep(5000);
										break;
									}
									return;
								}
								row.Cells["Note"].Value = "Code:" + text3;
								SrcollTrueView(chrome, readOnlyCollection2[1]);
								readOnlyCollection2[1].SendKeys(text3);
								Thread.Sleep(1000);
								IWebElement webElement4 = GetWebElement(chrome, By.Id("ProofAction"));
								if (webElement4 != null)
								{
									SrcollTrueView(chrome, webElement4);
									webElement4.Click();
									Thread.Sleep(3000);
									webElement4 = GetWebElement(chrome, By.Id("FinishAction"));
									if (webElement4 != null)
									{
										webElement4.Click();
										Thread.Sleep(3000);
										row.Cells["Note"].Value = "Add phone successfully";
									}
								}
								return;
							}
							Thread.Sleep(5000);
							num++;
							if (num < 5)
							{
								break;
							}
						}
					}
					num2++;
				}
			}
		}
	}
}
