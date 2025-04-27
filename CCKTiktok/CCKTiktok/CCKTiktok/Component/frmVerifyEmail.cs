using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Entity;

namespace CCKTiktok.Component
{
	public class frmVerifyEmail : Form
	{
		private IContainer components = null;

		private Button btnSAve;

		private TextBox txtFirst;

		private TextBox txtLast;

		private Label label3;

		private Label label4;

		private Button btnConfigEmail;

		private ComboBox lstemail;

		private TextBox txtEmail;

		private GroupBox groupBox5;

		private RadioButton fbtFile;

		private RadioButton rbtMailDomain;

		private GroupBox groupBox7;

		private TextBox txtPassword;

		private RadioButton cbxFixpass;

		private RadioButton cbxRandomPass;

		private NumericUpDown nudCaptchaDelay;

		private Label label1;

		private TextBox txtcaptcha;

		private LinkLabel linkLabel3;

		private NumericUpDown nudAgeFrom;

		private Label label5;

		private NumericUpDown nudAgeTo;

		private Label label6;

		private GroupBox groupBox2;

		private CheckBox cbxUnlockCaptcha;

		private GroupBox groupBox3;

		private RadioButton rbtContinue;

		private RadioButton rbtStop;

		private Label label8;

		private NumericUpDown nudErrorDelay;

		private Label label7;

		private GroupBox groupBox4;

		private Label label9;

		private GroupBox groupBox1;

		private RadioButton rbtFull;

		private RadioButton rbt8Nick;

		private CheckBox cbxShowIp;

		private GroupBox groupBox10;

		private RadioButton rbtSmartOtp;

		private RadioButton rbtViotp;

		private LinkLabel linkLabel2;

		private TextBox txtSmartOtp;

		private LinkLabel linkLabel4;

		private TextBox txtViotp;

		private GroupBox groupBox6;

		private RadioButton rbtRegMail;

		private RadioButton rbtRegPhone;

		private LinkLabel linkLabel1;

		private TextBox txtAhichaptcha;

		private ToolTip toolTipachi;

		private Label label2;

		private Label label10;

		private Label lblAhi;

		private Label lblOmo;

		private TextBox txtGoogleAppDomain;

		private RadioButton rbtDomainGoogle;

		private RadioButton rbt1Secmail;

		private GroupBox groupBox8;

		private RadioButton rbtAppMain;

		private RadioButton rbtLite;

		private GroupBox groupBox9;

		private Label label14;

		private NumericUpDown nudDelayTo;

		private Label label12;

		private Label label13;

		private NumericUpDown nudGmailDelay;

		private CheckBox cbxUseMailName;

		private GroupBox groupBox11;

		private RadioButton rbtVerifyHotmail;

		private ComboBox cbxEmailType;

		private LinkLabel linkLabel5;

		private TextBox txtDongVan;

		private RadioButton rbtdongvanfb;

		private GroupBox groupBox12;

		private ComboBox cbxAppName;

		private CheckBox cbxCheckVerrsion;

		private CheckBox cbxStop;

		private CheckBox cbxRemovegmail;

		private GroupBox groupBox13;

		private Label label15;

		private Label label17;

		private Label label11;

		private NumericUpDown nudDelayRename;

		private Label label16;

		private NumericUpDown nudDelayBirthday;

		private Button btnLogEmail;

		private Button RemoveLog;

		private GroupBox groupBox14;

		private CheckBox cbxImportContact;

		private NumericUpDown nudNumberContact;

		private Label label18;

		private Button btnFile;

		private Label label19;

		private Label label20;

		private TextBox txtFileContact;

		private RadioButton rbtGetnada;

		private CheckBox cbxClearapp;

		public frmVerifyEmail()
		{
			InitializeComponent();
		}

		private void btnSAve_Click(object sender, EventArgs e)
		{
			if (!(txtFirst.Text.Trim() == ""))
			{
				if (txtLast.Text.Trim() == "")
				{
					MessageBox.Show("Last Name không được để trống");
					return;
				}
				VNNameEntity vNNameEntity = new VNNameEntity();
				vNNameEntity.FirstName = ((txtFirst.Text.Trim() != "") ? txtFirst.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());
				vNNameEntity.LastName = ((txtLast.Text.Trim() != "") ? txtLast.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());
				File.WriteAllText(CaChuaConstant.VN_Name, new JavaScriptSerializer().Serialize(vNNameEntity));
				File.WriteAllText(CaChuaConstant.EMAIL_FROMFILE, txtEmail.Text.Trim());
				File.WriteAllText(CaChuaConstant.GOOGLE_APP_DOMAIM, txtGoogleAppDomain.Text.Trim());
				File.WriteAllText(CaChuaConstant.DONGVANFB, txtDongVan.Text.Trim());
				File.WriteAllText(CaChuaConstant.OTP, txtSmartOtp.Text.Trim());
				File.WriteAllText(CaChuaConstant.VIOTP, txtViotp.Text.Trim());
				if (txtDongVan.Text != "" && cbxEmailType.SelectedIndex != -1)
				{
					File.WriteAllText(CaChuaConstant.DONGVANFB_TYPE, cbxEmailType.SelectedValue.ToString());
				}
				if (lstemail.Items.Count > 0 && lstemail.SelectedItem.ToString() != "")
				{
					File.WriteAllText(CaChuaConstant.DOMAIN_CONFIG, lstemail.SelectedItem.ToString().Trim());
				}
				File.WriteAllText(CaChuaConstant.DELAY_CAPTCHA, nudCaptchaDelay.Value.ToString().Trim());
				if (!rbtdongvanfb.Checked)
				{
					if (!rbtMailDomain.Checked)
					{
						if (!fbtFile.Checked)
						{
							if (rbtDomainGoogle.Checked)
							{
								File.WriteAllText(CaChuaConstant.VERI_TYPE, new JavaScriptSerializer().Serialize(EmailRegisterType.GoogleAppDomain));
							}
							else if (rbt1Secmail.Checked)
							{
								File.WriteAllText(CaChuaConstant.VERI_TYPE, new JavaScriptSerializer().Serialize(EmailRegisterType.Onesecmail));
							}
						}
						else
						{
							File.WriteAllText(CaChuaConstant.VERI_TYPE, new JavaScriptSerializer().Serialize(EmailRegisterType.HotmailFromFile));
						}
					}
					else
					{
						File.WriteAllText(CaChuaConstant.VERI_TYPE, new JavaScriptSerializer().Serialize(EmailRegisterType.MailDomain));
					}
				}
				else
				{
					File.WriteAllText(CaChuaConstant.VERI_TYPE, new JavaScriptSerializer().Serialize(EmailRegisterType.DongVanFB));
				}
				if (!rbtViotp.Checked)
				{
					if (rbtSmartOtp.Checked)
					{
						File.WriteAllText(CaChuaConstant.VERI_PHONE_TYPE, new JavaScriptSerializer().Serialize(VerifyPhoneType.SmartOtp));
					}
				}
				else
				{
					File.WriteAllText(CaChuaConstant.VERI_PHONE_TYPE, new JavaScriptSerializer().Serialize(VerifyPhoneType.Viotp));
				}
				if (File.Exists(CaChuaConstant.SHOW_IP))
				{
					cbxShowIp.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.SHOW_IP));
				}
				if (File.Exists(CaChuaConstant.REG_TYPE))
				{
					rbtRegPhone.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.REG_TYPE));
					rbtRegMail.Checked = rbtRegPhone.Checked;
				}
				RegNickEntity regNickEntity = new RegNickEntity();
				if (!cbxRandomPass.Checked)
				{
					regNickEntity.PasswordDefault = txtPassword.Text;
				}
				else
				{
					regNickEntity.PasswordDefault = "";
				}
				regNickEntity.RegType = (rbtRegPhone.Checked ? RegAccountType.Phone : RegAccountType.Email);
				regNickEntity.AgeFrom = Utils.Convert2Int(nudAgeFrom.Value.ToString());
				regNickEntity.AgeTo = Utils.Convert2Int(nudAgeTo.Value.ToString());
				regNickEntity.Register8AccountOnly = rbt8Nick.Checked;
				regNickEntity.ErrorStop = rbtStop.Checked;
				regNickEntity.ErrorContinue = rbtContinue.Checked;
				regNickEntity.ErrorDelay = Convert.ToInt32(nudErrorDelay.Value);
				regNickEntity.GmailDelayFrom = Convert.ToInt32(nudGmailDelay.Value);
				regNickEntity.GmailDelayTo = Convert.ToInt32(nudDelayTo.Value);
				File.WriteAllText(CaChuaConstant.PASS, txtPassword.Text);
				File.WriteAllText(CaChuaConstant.REG_CONFIG, new JavaScriptSerializer().Serialize(regNickEntity));
				ImportContact importContact = new ImportContact();
				importContact.Number = (int)nudNumberContact.Value;
				importContact.Status = cbxImportContact.Checked;
				Close();
			}
			else
			{
				MessageBox.Show("First Name không được để trống");
			}
		}

		private void frmVerifyEmail_Load(object sender, EventArgs e)
		{
			if (!File.Exists(CaChuaConstant.VN_Name))
			{
				new WebClient().DownloadFile("https://cck.vn/Download/Utils/vn_name.txt", CaChuaConstant.VN_Name);
			}
			if (File.Exists(CaChuaConstant.VN_Name))
			{
				VNNameEntity vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
				txtFirst.Text = string.Join(Environment.NewLine, vNNameEntity.FirstName.Distinct().ToList());
				txtLast.Text = string.Join(Environment.NewLine, vNNameEntity.LastName.Distinct().ToList());
			}
			txtGoogleAppDomain.Text = Utils.ReadTextFile(CaChuaConstant.GOOGLE_APP_DOMAIM);
			cbxStop.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.STOP_APP));
			cbxClearapp.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.CLEAR_APP));
			nudDelayBirthday.Value = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_BIRTHDAY), 30);
			nudDelayRename.Value = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.DELAY_RENAME), 30);
			cbxRemovegmail.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.REMOVE_GMAIL));
			cbxShowIp.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.SHOW_IP));
			if (File.Exists(CaChuaConstant.UNLOCKCAPTCHA_BYHAND))
			{
				cbxUnlockCaptcha.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.UNLOCKCAPTCHA_BYHAND));
			}
			if (File.Exists(CaChuaConstant.EMAIL_FROMFILE))
			{
				txtEmail.Text = Utils.ReadTextFile(CaChuaConstant.EMAIL_FROMFILE);
			}
			cbxUseMailName.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.EMAIL_NAME));
			if (File.Exists(CaChuaConstant.DONGVANFB))
			{
				txtDongVan.Text = Utils.ReadTextFile(CaChuaConstant.DONGVANFB);
			}
			txtcaptcha.Text = Utils.ReadTextFile(CaChuaConstant.MMOCAPTCHA);
			if (File.Exists(CaChuaConstant.DELAY_CAPTCHA))
			{
				string s = Utils.ReadTextFile(CaChuaConstant.DELAY_CAPTCHA);
				nudCaptchaDelay.Value = Utils.Convert2Int(s);
			}
			txtSmartOtp.Text = Utils.ReadTextFile(CaChuaConstant.OTP);
			txtViotp.Text = Utils.ReadTextFile(CaChuaConstant.VIOTP);
			try
			{
				if (Utils.ReadTextFile(CaChuaConstant.APP_PACKAGE) == "")
				{
					cbxAppName.SelectedIndex = 0;
				}
				else
				{
					cbxAppName.SelectedItem = Utils.ReadTextFile(CaChuaConstant.APP_PACKAGE);
				}
				CaChuaConstant.PACKAGE_NAME = Utils.ReadTextFile(CaChuaConstant.APP_PACKAGE);
			}
			catch
			{
				cbxAppName.SelectedIndex = 0;
			}
			if (File.Exists(CaChuaConstant.VERI_TYPE))
			{
				switch (new JavaScriptSerializer().Deserialize<EmailRegisterType>(Utils.ReadTextFile(CaChuaConstant.VERI_TYPE)))
				{
				case EmailRegisterType.GoogleAppDomain:
					rbtDomainGoogle.Checked = true;
					break;
				case EmailRegisterType.DongVanFB:
					rbtdongvanfb.Checked = true;
					break;
				case EmailRegisterType.MailDomain:
					rbtMailDomain.Checked = true;
					break;
				case EmailRegisterType.HotmailFromFile:
					fbtFile.Checked = true;
					break;
				}
			}
			if (File.Exists(CaChuaConstant.VERI_PHONE_TYPE))
			{
				switch (new JavaScriptSerializer().Deserialize<VerifyPhoneType>(Utils.ReadTextFile(CaChuaConstant.VERI_PHONE_TYPE)))
				{
				case VerifyPhoneType.SmartOtp:
					rbtSmartOtp.Checked = true;
					break;
				case VerifyPhoneType.Viotp:
					rbtViotp.Checked = true;
					break;
				}
			}
			if (File.Exists(CaChuaConstant.EMAIL_REG_VERIFY))
			{
				switch (new JavaScriptSerializer().Deserialize<EmailVerifyType>(Utils.ReadTextFile(CaChuaConstant.EMAIL_REG_VERIFY)))
				{
				case EmailVerifyType.OneSec:
					rbt1Secmail.Checked = true;
					break;
				case EmailVerifyType.Hotmail:
					rbtVerifyHotmail.Checked = true;
					break;
				case EmailVerifyType.Getnada:
					rbtGetnada.Checked = true;
					break;
				}
			}
			bool flag = Utils.ReadTextFile(CaChuaConstant.REG_APP_NAME) == "TiktokLite";
			rbtAppMain.Checked = !flag;
			rbtLite.Checked = flag;
			txtAhichaptcha.Text = Utils.ReadTextFile(CaChuaConstant.AHICAPTCHA);
			cbxCheckVerrsion.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.CHECK_VERSION_APP));
			BindList();
			if (File.Exists(CaChuaConstant.REG_CONFIG))
			{
				RegNickEntity regNickEntity = new JavaScriptSerializer().Deserialize<RegNickEntity>(Utils.ReadTextFile(CaChuaConstant.REG_CONFIG));
				if (regNickEntity != null)
				{
					if (!(regNickEntity.PasswordDefault != ""))
					{
						cbxRandomPass.Checked = !cbxFixpass.Checked;
					}
					else
					{
						cbxFixpass.Checked = true;
						txtPassword.Text = regNickEntity.PasswordDefault;
					}
				}
				nudAgeFrom.Value = regNickEntity.AgeFrom;
				nudAgeTo.Value = regNickEntity.AgeTo;
				rbt8Nick.Checked = regNickEntity.Register8AccountOnly;
				rbtStop.Checked = regNickEntity.ErrorStop;
				rbtContinue.Checked = regNickEntity.ErrorContinue;
				nudErrorDelay.Value = regNickEntity.ErrorDelay;
				rbtFull.Checked = !regNickEntity.Register8AccountOnly;
				rbtRegMail.Checked = regNickEntity.RegType == RegAccountType.Email;
				rbtRegPhone.Checked = !rbtRegMail.Checked;
				nudGmailDelay.Value = regNickEntity.GmailDelayFrom;
				nudDelayTo.Value = regNickEntity.GmailDelayTo;
			}
			if (!(txtDongVan.Text != ""))
			{
				return;
			}
			Task.Run(delegate
			{
				try
				{
					DongVanFB dongVanFB = new DongVanFB(Utils.ReadTextFile(CaChuaConstant.DONGVANFB));
					DataTable emailType = dongVanFB.GetEmailType();
					if (emailType != null)
					{
						cbxEmailType.DataSource = emailType;
						cbxEmailType.DisplayMember = "name";
						cbxEmailType.ValueMember = "id";
					}
					string selectedValue = "1";
					if (File.Exists(CaChuaConstant.DONGVANFB_TYPE) && cbxEmailType.Items.Count > 0)
					{
						selectedValue = Utils.ReadTextFile(CaChuaConstant.DONGVANFB_TYPE);
					}
					cbxEmailType.SelectedValue = selectedValue;
				}
				catch
				{
				}
			});
		}

		private void btnConfigEmail_Click(object sender, EventArgs e)
		{
			frmMailDomain frmMailDomain2 = new frmMailDomain();
			frmMailDomain2.StartPosition = FormStartPosition.CenterScreen;
			frmMailDomain2.ShowDialog();
			BindList();
		}

		private void BindList()
		{
			List<string> dataSource = (from r in new SQLiteUtils().ExecuteQuery("Select domain from EmailSystem").AsEnumerable()
				select r.Field<string>("domain")).ToList();
			lstemail.DataSource = dataSource;
			if (File.Exists(CaChuaConstant.DOMAIN_CONFIG))
			{
				string text = Utils.ReadTextFile(CaChuaConstant.DOMAIN_CONFIG);
				bool flag = false;
				for (int i = 0; i < lstemail.Items.Count; i++)
				{
					if (lstemail.Items[i].ToString() == text)
					{
						lstemail.SelectedIndex = i;
						flag = true;
						break;
					}
				}
				if (!flag && lstemail.Items.Count > 0)
				{
					lstemail.SelectedIndex = 0;
				}
			}
			if (lstemail.Items.Count == 0)
			{
				lstemail.DataSource = new List<string> { "@getnada.com" };
			}
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://primeotp.me/register.php?ref=cachuake");
		}

		private void txtPrimeOtp_TextChanged(object sender, EventArgs e)
		{
		}

		private void btnChange_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void rbtEnglish_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void groupBox5_Enter(object sender, EventArgs e)
		{
		}

		private void rbtGmail_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string gMAIL_LIST = CaChuaConstant.GMAIL_LIST;
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "List of Gmail");
			frmInputControl2.Text = "List of Gmail";
			if (File.Exists(gMAIL_LIST))
			{
				frmInputControl2.Result = Utils.ReadTextFile(gMAIL_LIST);
				frmInputControl2.SetText(frmInputControl2.Result);
			}
			frmInputControl2.ShowDialog();
			string result = frmInputControl2.Result;
			File.WriteAllText(gMAIL_LIST, result);
		}

		private void rbtMaxClone_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://SmartOtp.com/");
		}

		private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://omocaptcha.com");
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://otptextnow.com/");
		}

		private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://1stcaptcha.com");
		}

		private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://dongvanfb.com");
		}

		private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://tempcode.co");
		}

		private void txtDongVan_TextChanged(object sender, EventArgs e)
		{
			if (!(txtDongVan.Text != ""))
			{
				return;
			}
			Task.Run(delegate
			{
				try
				{
					DongVanFB dongVanFB = new DongVanFB(Utils.ReadTextFile(CaChuaConstant.DONGVANFB));
					DataTable emailType = dongVanFB.GetEmailType();
					if (emailType != null)
					{
						cbxEmailType.DataSource = emailType;
						cbxEmailType.DisplayMember = "name";
						cbxEmailType.ValueMember = "id";
					}
					string selectedValue = "1";
					if (File.Exists(CaChuaConstant.DONGVANFB_TYPE) && cbxEmailType.Items.Count > 0)
					{
						selectedValue = Utils.ReadTextFile(CaChuaConstant.DONGVANFB_TYPE);
					}
					cbxEmailType.SelectedValue = selectedValue;
				}
				catch
				{
				}
			});
		}

		private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://supersim247.com");
		}

		private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (!Directory.Exists(Application.StartupPath + "\\Tmp\\"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\Tmp\\");
			}
			Process.Start(Application.StartupPath + "\\Tmp\\");
		}

		private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://hotmailbox.me");
		}

		private void cbxEmailType_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void cbxhotmailbox_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void cbUnlockByHand_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void rbtVN_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void txtcaptcha_TextChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.MMOCAPTCHA, txtcaptcha.Text.Trim());
			Task.Run(delegate
			{
				OmoCaptcha omoCaptcha = new OmoCaptcha(txtcaptcha.Text.Trim());
				lblOmo.Text = omoCaptcha.GetBalance().ToString("#,###");
			});
		}

		private void groupBox2_Enter(object sender, EventArgs e)
		{
		}

		private void cbxUnlockCaptcha_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.UNLOCKCAPTCHA_BYHAND, cbxUnlockCaptcha.Checked.ToString());
		}

		private void cbxShowIp_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.SHOW_IP, cbxShowIp.Checked.ToString());
		}

		private void linkLabel4_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://smartotp.net");
		}

		private void linkLabel2_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://viotp.com/");
		}

		private void linkLabel1_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://achicaptcha.com/");
		}

		private void txtAhichaptcha_TextChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.AHICAPTCHA, txtAhichaptcha.Text.Trim());
			Task.Run(delegate
			{
				achicaptcha achicaptcha = new achicaptcha(txtAhichaptcha.Text.Trim());
				lblAhi.Text = achicaptcha.GetBalance();
			});
		}

		private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://superteam.info");
		}

		private void linkLabel7_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://sellgmail.com");
		}

		private void cbxRandomPass_CheckedChanged(object sender, EventArgs e)
		{
			txtPassword.Text = "";
		}

		private void rbtSellGmail_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void rbtDomainGoogle_CheckedChanged(object sender, EventArgs e)
		{
			if (!rbtDomainGoogle.Checked)
			{
				rbtLite.Enabled = false;
				rbtAppMain.Enabled = true;
				rbtAppMain.Checked = true;
			}
			else
			{
				rbtLite.Enabled = true;
				rbtAppMain.Enabled = true;
			}
		}

		private void rbtAppMain_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.REG_APP_NAME, "Tiktok");
		}

		private void rbtLite_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.REG_APP_NAME, "TiktokLite");
		}

		private void nudGmailDelay_ValueChanged(object sender, EventArgs e)
		{
			nudDelayTo.Value = nudGmailDelay.Value + 1m;
		}

		private void nudDelayTo_ValueChanged(object sender, EventArgs e)
		{
			if (nudDelayTo.Value <= nudGmailDelay.Value)
			{
				nudDelayTo.Value = nudGmailDelay.Value + 1m;
			}
		}

		private void nudAgeFrom_ValueChanged(object sender, EventArgs e)
		{
		}

		private void cbxUseMailName_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.EMAIL_NAME, cbxUseMailName.Checked.ToString());
		}

		private void rbt1Secmail_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.EMAIL_REG_VERIFY, new JavaScriptSerializer().Serialize(EmailVerifyType.OneSec));
		}

		private void rbtVerifyHotmail_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.EMAIL_REG_VERIFY, new JavaScriptSerializer().Serialize(EmailVerifyType.Hotmail));
		}

		private void cbxAppName_SelectedIndexChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.APP_PACKAGE, cbxAppName.SelectedItem.ToString());
		}

		private void cbxCheckVerrsion_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.CHECK_VERSION_APP, cbxCheckVerrsion.Checked.ToString());
		}

		private void cbxStop_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.STOP_APP, cbxStop.Checked.ToString());
		}

		private void txtGoogleAppDomain_TextChanged(object sender, EventArgs e)
		{
		}

		private void cbxRemovegmail_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.REMOVE_GMAIL, cbxRemovegmail.Checked.ToString());
		}

		private void nudDelayBirthday_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.DELAY_BIRTHDAY, nudDelayBirthday.Value.ToString());
		}

		private void nudDelayRename_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.DELAY_RENAME, nudDelayRename.Value.ToString());
		}

		private void btnLogEmail_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "Email List");
			frmInputControl2.Text = "Email List";
			if (File.Exists(Application.StartupPath + "\\Config\\cck_log_email.txt"))
			{
				frmInputControl2.SetText(Utils.ReadTextFile(Application.StartupPath + "\\Config\\cck_log_email.txt"));
			}
			frmInputControl2.ShowDialog();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> list = frmInputControl2.Result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			if (list == null || list.Count <= 0)
			{
				return;
			}
			foreach (string item in list)
			{
				if (item != "")
				{
					string key = item.Split('|')[0].ToLower();
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, item);
					}
				}
			}
			List<string> list2 = new List<string>();
			DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select distinct Email as Email from Account");
			if (dataTable != null)
			{
				DataTable dataTable2 = new SQLiteUtils().ExecuteQuery("Select distinct EmailRecovery as Email from Account");
				if (dataTable2 != null)
				{
					dataTable.Merge(dataTable2);
				}
				foreach (DataRow row in dataTable.Rows)
				{
					string key2 = row[0].ToString().Trim().ToLower();
					if (dictionary.ContainsKey(key2))
					{
						dictionary.Remove(key2);
					}
				}
			}
			foreach (KeyValuePair<string, string> item2 in dictionary)
			{
				list2.Add(item2.Value);
			}
			File.WriteAllLines("tmp.txt", list2);
			Thread.Sleep(1000);
			Process.Start("tmp.txt");
		}

		private void RemoveLog_Click(object sender, EventArgs e)
		{
			string path = Application.StartupPath + "\\Config\\cck_log_email.txt";
			if (MessageBox.Show("Bạn có muốn xóa backup mail đã đăng ký không", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes && File.Exists(path))
			{
				File.Delete(path);
				MessageBox.Show("Xóa backup mail đã đăng ký thành công");
			}
		}

		private void btnFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Text|*.txt|All|*.*";
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = openFileDialog.FileName;
				txtFileContact.Text = fileName;
			}
		}

		private void rbtGmailAppDomain_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.EMAIL_REG_VERIFY, new JavaScriptSerializer().Serialize(EmailVerifyType.Getnada));
		}

		private void cbxClearapp_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.CLEAR_APP, cbxClearapp.Checked.ToString());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			btnSAve = new System.Windows.Forms.Button();
			txtFirst = new System.Windows.Forms.TextBox();
			txtLast = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			btnConfigEmail = new System.Windows.Forms.Button();
			lstemail = new System.Windows.Forms.ComboBox();
			txtEmail = new System.Windows.Forms.TextBox();
			groupBox7 = new System.Windows.Forms.GroupBox();
			txtPassword = new System.Windows.Forms.TextBox();
			cbxFixpass = new System.Windows.Forms.RadioButton();
			cbxRandomPass = new System.Windows.Forms.RadioButton();
			cbxUnlockCaptcha = new System.Windows.Forms.CheckBox();
			nudCaptchaDelay = new System.Windows.Forms.NumericUpDown();
			txtcaptcha = new System.Windows.Forms.TextBox();
			linkLabel3 = new System.Windows.Forms.LinkLabel();
			label1 = new System.Windows.Forms.Label();
			groupBox5 = new System.Windows.Forms.GroupBox();
			txtGoogleAppDomain = new System.Windows.Forms.TextBox();
			fbtFile = new System.Windows.Forms.RadioButton();
			cbxEmailType = new System.Windows.Forms.ComboBox();
			linkLabel5 = new System.Windows.Forms.LinkLabel();
			rbtDomainGoogle = new System.Windows.Forms.RadioButton();
			txtDongVan = new System.Windows.Forms.TextBox();
			rbtdongvanfb = new System.Windows.Forms.RadioButton();
			rbtMailDomain = new System.Windows.Forms.RadioButton();
			rbt1Secmail = new System.Windows.Forms.RadioButton();
			cbxShowIp = new System.Windows.Forms.CheckBox();
			nudAgeFrom = new System.Windows.Forms.NumericUpDown();
			label5 = new System.Windows.Forms.Label();
			nudAgeTo = new System.Windows.Forms.NumericUpDown();
			label6 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox3 = new System.Windows.Forms.GroupBox();
			cbxRemovegmail = new System.Windows.Forms.CheckBox();
			cbxStop = new System.Windows.Forms.CheckBox();
			cbxUseMailName = new System.Windows.Forms.CheckBox();
			label8 = new System.Windows.Forms.Label();
			rbtContinue = new System.Windows.Forms.RadioButton();
			nudErrorDelay = new System.Windows.Forms.NumericUpDown();
			rbtStop = new System.Windows.Forms.RadioButton();
			label7 = new System.Windows.Forms.Label();
			groupBox4 = new System.Windows.Forms.GroupBox();
			lblAhi = new System.Windows.Forms.Label();
			lblOmo = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			txtAhichaptcha = new System.Windows.Forms.TextBox();
			label9 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			rbtFull = new System.Windows.Forms.RadioButton();
			rbt8Nick = new System.Windows.Forms.RadioButton();
			groupBox10 = new System.Windows.Forms.GroupBox();
			rbtSmartOtp = new System.Windows.Forms.RadioButton();
			rbtViotp = new System.Windows.Forms.RadioButton();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			txtSmartOtp = new System.Windows.Forms.TextBox();
			linkLabel4 = new System.Windows.Forms.LinkLabel();
			txtViotp = new System.Windows.Forms.TextBox();
			groupBox6 = new System.Windows.Forms.GroupBox();
			rbtRegMail = new System.Windows.Forms.RadioButton();
			rbtRegPhone = new System.Windows.Forms.RadioButton();
			toolTipachi = new System.Windows.Forms.ToolTip(components);
			groupBox8 = new System.Windows.Forms.GroupBox();
			rbtAppMain = new System.Windows.Forms.RadioButton();
			rbtLite = new System.Windows.Forms.RadioButton();
			groupBox9 = new System.Windows.Forms.GroupBox();
			label14 = new System.Windows.Forms.Label();
			nudDelayTo = new System.Windows.Forms.NumericUpDown();
			label12 = new System.Windows.Forms.Label();
			label13 = new System.Windows.Forms.Label();
			nudGmailDelay = new System.Windows.Forms.NumericUpDown();
			groupBox11 = new System.Windows.Forms.GroupBox();
			rbtGetnada = new System.Windows.Forms.RadioButton();
			rbtVerifyHotmail = new System.Windows.Forms.RadioButton();
			groupBox12 = new System.Windows.Forms.GroupBox();
			cbxAppName = new System.Windows.Forms.ComboBox();
			cbxCheckVerrsion = new System.Windows.Forms.CheckBox();
			groupBox13 = new System.Windows.Forms.GroupBox();
			label15 = new System.Windows.Forms.Label();
			label17 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			nudDelayRename = new System.Windows.Forms.NumericUpDown();
			label16 = new System.Windows.Forms.Label();
			nudDelayBirthday = new System.Windows.Forms.NumericUpDown();
			btnLogEmail = new System.Windows.Forms.Button();
			RemoveLog = new System.Windows.Forms.Button();
			groupBox14 = new System.Windows.Forms.GroupBox();
			txtFileContact = new System.Windows.Forms.TextBox();
			cbxImportContact = new System.Windows.Forms.CheckBox();
			nudNumberContact = new System.Windows.Forms.NumericUpDown();
			label19 = new System.Windows.Forms.Label();
			label20 = new System.Windows.Forms.Label();
			label18 = new System.Windows.Forms.Label();
			btnFile = new System.Windows.Forms.Button();
			cbxClearapp = new System.Windows.Forms.CheckBox();
			groupBox7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudCaptchaDelay).BeginInit();
			groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudAgeFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudAgeTo).BeginInit();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudErrorDelay).BeginInit();
			groupBox4.SuspendLayout();
			groupBox1.SuspendLayout();
			groupBox10.SuspendLayout();
			groupBox6.SuspendLayout();
			groupBox8.SuspendLayout();
			groupBox9.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudGmailDelay).BeginInit();
			groupBox11.SuspendLayout();
			groupBox12.SuspendLayout();
			groupBox13.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudDelayRename).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayBirthday).BeginInit();
			groupBox14.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudNumberContact).BeginInit();
			SuspendLayout();
			btnSAve.Location = new System.Drawing.Point(214, 607);
			btnSAve.Name = "btnSAve";
			btnSAve.Size = new System.Drawing.Size(157, 33);
			btnSAve.TabIndex = 0;
			btnSAve.Text = "Save";
			btnSAve.UseVisualStyleBackColor = true;
			btnSAve.Click += new System.EventHandler(btnSAve_Click);
			txtFirst.Location = new System.Drawing.Point(205, 46);
			txtFirst.MaxLength = 999999999;
			txtFirst.Multiline = true;
			txtFirst.Name = "txtFirst";
			txtFirst.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtFirst.Size = new System.Drawing.Size(140, 198);
			txtFirst.TabIndex = 3;
			txtLast.Location = new System.Drawing.Point(363, 46);
			txtLast.MaxLength = 999999999;
			txtLast.Multiline = true;
			txtLast.Name = "txtLast";
			txtLast.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtLast.Size = new System.Drawing.Size(153, 198);
			txtLast.TabIndex = 4;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(244, 20);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(57, 13);
			label3.TabIndex = 7;
			label3.Text = "First Name";
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(412, 20);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(58, 13);
			label4.TabIndex = 8;
			label4.Text = "Last Name";
			btnConfigEmail.Location = new System.Drawing.Point(306, 289);
			btnConfigEmail.Margin = new System.Windows.Forms.Padding(2);
			btnConfigEmail.Name = "btnConfigEmail";
			btnConfigEmail.Size = new System.Drawing.Size(70, 24);
			btnConfigEmail.TabIndex = 124;
			btnConfigEmail.Text = "Add Config";
			btnConfigEmail.UseVisualStyleBackColor = true;
			btnConfigEmail.Click += new System.EventHandler(btnConfigEmail_Click);
			lstemail.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			lstemail.FormattingEnabled = true;
			lstemail.Location = new System.Drawing.Point(142, 290);
			lstemail.Name = "lstemail";
			lstemail.Size = new System.Drawing.Size(159, 21);
			lstemail.TabIndex = 126;
			txtEmail.Location = new System.Drawing.Point(30, 160);
			txtEmail.MaxLength = 999999999;
			txtEmail.Multiline = true;
			txtEmail.Name = "txtEmail";
			txtEmail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtEmail.Size = new System.Drawing.Size(158, 84);
			txtEmail.TabIndex = 132;
			groupBox7.Controls.Add(txtPassword);
			groupBox7.Controls.Add(cbxFixpass);
			groupBox7.Controls.Add(cbxRandomPass);
			groupBox7.Location = new System.Drawing.Point(567, 280);
			groupBox7.Name = "groupBox7";
			groupBox7.Size = new System.Drawing.Size(370, 73);
			groupBox7.TabIndex = 162;
			groupBox7.TabStop = false;
			groupBox7.Text = "Password type";
			txtPassword.Location = new System.Drawing.Point(165, 42);
			txtPassword.Name = "txtPassword";
			txtPassword.Size = new System.Drawing.Size(113, 20);
			txtPassword.TabIndex = 161;
			cbxFixpass.AutoSize = true;
			cbxFixpass.Location = new System.Drawing.Point(22, 43);
			cbxFixpass.Name = "cbxFixpass";
			cbxFixpass.Size = new System.Drawing.Size(86, 17);
			cbxFixpass.TabIndex = 160;
			cbxFixpass.Text = "Fix password";
			cbxFixpass.UseVisualStyleBackColor = true;
			cbxRandomPass.AutoSize = true;
			cbxRandomPass.Checked = true;
			cbxRandomPass.Location = new System.Drawing.Point(22, 19);
			cbxRandomPass.Name = "cbxRandomPass";
			cbxRandomPass.Size = new System.Drawing.Size(113, 17);
			cbxRandomPass.TabIndex = 159;
			cbxRandomPass.TabStop = true;
			cbxRandomPass.Text = "Random password";
			cbxRandomPass.UseVisualStyleBackColor = true;
			cbxRandomPass.CheckedChanged += new System.EventHandler(cbxRandomPass_CheckedChanged);
			cbxUnlockCaptcha.AutoSize = true;
			cbxUnlockCaptcha.Location = new System.Drawing.Point(114, 106);
			cbxUnlockCaptcha.Name = "cbxUnlockCaptcha";
			cbxUnlockCaptcha.Size = new System.Drawing.Size(151, 17);
			cbxUnlockCaptcha.TabIndex = 164;
			cbxUnlockCaptcha.Text = "Gỡ captcha xoay bằng tay";
			cbxUnlockCaptcha.UseVisualStyleBackColor = true;
			cbxUnlockCaptcha.CheckedChanged += new System.EventHandler(cbxUnlockCaptcha_CheckedChanged);
			nudCaptchaDelay.Location = new System.Drawing.Point(151, 24);
			nudCaptchaDelay.Name = "nudCaptchaDelay";
			nudCaptchaDelay.Size = new System.Drawing.Size(54, 20);
			nudCaptchaDelay.TabIndex = 162;
			nudCaptchaDelay.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			txtcaptcha.Location = new System.Drawing.Point(114, 54);
			txtcaptcha.Name = "txtcaptcha";
			txtcaptcha.Size = new System.Drawing.Size(122, 20);
			txtcaptcha.TabIndex = 161;
			txtcaptcha.TextChanged += new System.EventHandler(txtcaptcha_TextChanged);
			linkLabel3.AutoSize = true;
			linkLabel3.Location = new System.Drawing.Point(17, 55);
			linkLabel3.Name = "linkLabel3";
			linkLabel3.Size = new System.Drawing.Size(91, 13);
			linkLabel3.TabIndex = 137;
			linkLabel3.TabStop = true;
			linkLabel3.Text = "Omocaptcha.com";
			linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked_1);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 26);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(137, 13);
			label1.TabIndex = 6;
			label1.Text = "Thời gian chờ hiện captcha";
			groupBox5.Controls.Add(txtGoogleAppDomain);
			groupBox5.Controls.Add(txtEmail);
			groupBox5.Controls.Add(fbtFile);
			groupBox5.Controls.Add(cbxEmailType);
			groupBox5.Controls.Add(linkLabel5);
			groupBox5.Controls.Add(btnConfigEmail);
			groupBox5.Controls.Add(txtLast);
			groupBox5.Controls.Add(label4);
			groupBox5.Controls.Add(rbtDomainGoogle);
			groupBox5.Controls.Add(label3);
			groupBox5.Controls.Add(lstemail);
			groupBox5.Controls.Add(txtDongVan);
			groupBox5.Controls.Add(txtFirst);
			groupBox5.Controls.Add(rbtdongvanfb);
			groupBox5.Controls.Add(rbtMailDomain);
			groupBox5.Location = new System.Drawing.Point(26, 5);
			groupBox5.Name = "groupBox5";
			groupBox5.Size = new System.Drawing.Size(529, 327);
			groupBox5.TabIndex = 160;
			groupBox5.TabStop = false;
			groupBox5.Text = "Cấu hình Email Reg";
			groupBox5.Enter += new System.EventHandler(groupBox5_Enter);
			txtGoogleAppDomain.Location = new System.Drawing.Point(30, 49);
			txtGoogleAppDomain.MaxLength = 999999999;
			txtGoogleAppDomain.Multiline = true;
			txtGoogleAppDomain.Name = "txtGoogleAppDomain";
			txtGoogleAppDomain.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtGoogleAppDomain.Size = new System.Drawing.Size(160, 79);
			txtGoogleAppDomain.TabIndex = 132;
			txtGoogleAppDomain.TextChanged += new System.EventHandler(txtGoogleAppDomain_TextChanged);
			fbtFile.AutoSize = true;
			fbtFile.Enabled = false;
			fbtFile.Location = new System.Drawing.Point(19, 134);
			fbtFile.Name = "fbtFile";
			fbtFile.Size = new System.Drawing.Size(130, 17);
			fbtFile.TabIndex = 135;
			fbtFile.Text = "Hotmail / Outlook Mail";
			fbtFile.UseVisualStyleBackColor = true;
			cbxEmailType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxEmailType.FormattingEnabled = true;
			cbxEmailType.Location = new System.Drawing.Point(224, 261);
			cbxEmailType.Name = "cbxEmailType";
			cbxEmailType.Size = new System.Drawing.Size(292, 21);
			cbxEmailType.TabIndex = 139;
			cbxEmailType.SelectedIndexChanged += new System.EventHandler(cbxEmailType_SelectedIndexChanged);
			linkLabel5.AutoSize = true;
			linkLabel5.Location = new System.Drawing.Point(34, 260);
			linkLabel5.Name = "linkLabel5";
			linkLabel5.Size = new System.Drawing.Size(81, 13);
			linkLabel5.TabIndex = 138;
			linkLabel5.TabStop = true;
			linkLabel5.Text = "dongvanfb.com";
			linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel5_LinkClicked);
			rbtDomainGoogle.AutoSize = true;
			rbtDomainGoogle.Checked = true;
			rbtDomainGoogle.Location = new System.Drawing.Point(19, 21);
			rbtDomainGoogle.Name = "rbtDomainGoogle";
			rbtDomainGoogle.Size = new System.Drawing.Size(120, 17);
			rbtDomainGoogle.TabIndex = 135;
			rbtDomainGoogle.TabStop = true;
			rbtDomainGoogle.Text = "Google App Domain";
			rbtDomainGoogle.UseVisualStyleBackColor = true;
			rbtDomainGoogle.CheckedChanged += new System.EventHandler(rbtDomainGoogle_CheckedChanged);
			txtDongVan.Location = new System.Drawing.Point(142, 261);
			txtDongVan.Name = "txtDongVan";
			txtDongVan.Size = new System.Drawing.Size(76, 20);
			txtDongVan.TabIndex = 13;
			txtDongVan.TextChanged += new System.EventHandler(txtDongVan_TextChanged);
			rbtdongvanfb.AutoSize = true;
			rbtdongvanfb.Enabled = false;
			rbtdongvanfb.Location = new System.Drawing.Point(19, 261);
			rbtdongvanfb.Name = "rbtdongvanfb";
			rbtdongvanfb.Size = new System.Drawing.Size(14, 13);
			rbtdongvanfb.TabIndex = 135;
			rbtdongvanfb.UseVisualStyleBackColor = true;
			rbtMailDomain.AutoSize = true;
			rbtMailDomain.Enabled = false;
			rbtMailDomain.Location = new System.Drawing.Point(19, 291);
			rbtMailDomain.Name = "rbtMailDomain";
			rbtMailDomain.Size = new System.Drawing.Size(108, 17);
			rbtMailDomain.TabIndex = 135;
			rbtMailDomain.Text = "Veri Main Domain";
			rbtMailDomain.UseVisualStyleBackColor = true;
			rbt1Secmail.AutoSize = true;
			rbt1Secmail.Checked = true;
			rbt1Secmail.Location = new System.Drawing.Point(15, 22);
			rbt1Secmail.Name = "rbt1Secmail";
			rbt1Secmail.Size = new System.Drawing.Size(119, 17);
			rbt1Secmail.TabIndex = 135;
			rbt1Secmail.TabStop = true;
			rbt1Secmail.Text = "1secmail.com (Free)";
			rbt1Secmail.UseVisualStyleBackColor = true;
			rbt1Secmail.CheckedChanged += new System.EventHandler(rbt1Secmail_CheckedChanged);
			cbxShowIp.AutoSize = true;
			cbxShowIp.Location = new System.Drawing.Point(207, 17);
			cbxShowIp.Name = "cbxShowIp";
			cbxShowIp.Size = new System.Drawing.Size(124, 17);
			cbxShowIp.TabIndex = 164;
			cbxShowIp.Text = "Show IP khi sử dụng";
			cbxShowIp.UseVisualStyleBackColor = true;
			cbxShowIp.CheckedChanged += new System.EventHandler(cbxShowIp_CheckedChanged);
			nudAgeFrom.Location = new System.Drawing.Point(76, 19);
			nudAgeFrom.Minimum = new decimal(new int[4] { 18, 0, 0, 0 });
			nudAgeFrom.Name = "nudAgeFrom";
			nudAgeFrom.Size = new System.Drawing.Size(39, 20);
			nudAgeFrom.TabIndex = 165;
			nudAgeFrom.Value = new decimal(new int[4] { 18, 0, 0, 0 });
			nudAgeFrom.ValueChanged += new System.EventHandler(nudAgeFrom_ValueChanged);
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(44, 22);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(20, 13);
			label5.TabIndex = 164;
			label5.Text = "Từ";
			nudAgeTo.Location = new System.Drawing.Point(177, 19);
			nudAgeTo.Minimum = new decimal(new int[4] { 20, 0, 0, 0 });
			nudAgeTo.Name = "nudAgeTo";
			nudAgeTo.Size = new System.Drawing.Size(38, 20);
			nudAgeTo.TabIndex = 167;
			nudAgeTo.Value = new decimal(new int[4] { 20, 0, 0, 0 });
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(132, 22);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(26, 13);
			label6.TabIndex = 166;
			label6.Text = "đến";
			groupBox2.Controls.Add(nudAgeTo);
			groupBox2.Controls.Add(label6);
			groupBox2.Controls.Add(label5);
			groupBox2.Controls.Add(nudAgeFrom);
			groupBox2.Location = new System.Drawing.Point(567, 417);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(370, 52);
			groupBox2.TabIndex = 164;
			groupBox2.TabStop = false;
			groupBox2.Text = "Tuổi";
			groupBox2.Enter += new System.EventHandler(groupBox2_Enter);
			groupBox3.Controls.Add(cbxClearapp);
			groupBox3.Controls.Add(cbxRemovegmail);
			groupBox3.Controls.Add(cbxStop);
			groupBox3.Controls.Add(cbxUseMailName);
			groupBox3.Controls.Add(label8);
			groupBox3.Controls.Add(rbtContinue);
			groupBox3.Controls.Add(nudErrorDelay);
			groupBox3.Controls.Add(rbtStop);
			groupBox3.Controls.Add(label7);
			groupBox3.Controls.Add(cbxShowIp);
			groupBox3.Location = new System.Drawing.Point(567, 5);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(370, 128);
			groupBox3.TabIndex = 165;
			groupBox3.TabStop = false;
			groupBox3.Text = "Cấu hình khi gặp lỗi";
			cbxRemovegmail.AutoSize = true;
			cbxRemovegmail.Location = new System.Drawing.Point(207, 86);
			cbxRemovegmail.Name = "cbxRemovegmail";
			cbxRemovegmail.Size = new System.Drawing.Size(142, 17);
			cbxRemovegmail.TabIndex = 166;
			cbxRemovegmail.Text = "Xóa gmail trước khi chạy";
			cbxRemovegmail.UseVisualStyleBackColor = true;
			cbxRemovegmail.CheckedChanged += new System.EventHandler(cbxRemovegmail_CheckedChanged);
			cbxStop.AutoSize = true;
			cbxStop.Location = new System.Drawing.Point(207, 64);
			cbxStop.Name = "cbxStop";
			cbxStop.Size = new System.Drawing.Size(136, 17);
			cbxStop.TabIndex = 165;
			cbxStop.Text = "Không tắt App khi Stop";
			cbxStop.UseVisualStyleBackColor = true;
			cbxStop.CheckedChanged += new System.EventHandler(cbxStop_CheckedChanged);
			cbxUseMailName.AutoSize = true;
			cbxUseMailName.Location = new System.Drawing.Point(207, 39);
			cbxUseMailName.Name = "cbxUseMailName";
			cbxUseMailName.Size = new System.Drawing.Size(142, 17);
			cbxUseMailName.TabIndex = 165;
			cbxUseMailName.Text = "Không dùng tên từ Email";
			cbxUseMailName.UseVisualStyleBackColor = true;
			cbxUseMailName.CheckedChanged += new System.EventHandler(cbxUseMailName_CheckedChanged);
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(162, 89);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(26, 13);
			label8.TabIndex = 163;
			label8.Text = "giây";
			rbtContinue.AutoSize = true;
			rbtContinue.Checked = true;
			rbtContinue.Location = new System.Drawing.Point(23, 56);
			rbtContinue.Name = "rbtContinue";
			rbtContinue.Size = new System.Drawing.Size(152, 17);
			rbtContinue.TabIndex = 161;
			rbtContinue.TabStop = true;
			rbtContinue.Text = "Gặp lỗi thì nghỉ rồi Reg tiếp";
			rbtContinue.UseVisualStyleBackColor = true;
			nudErrorDelay.Location = new System.Drawing.Point(102, 87);
			nudErrorDelay.Maximum = new decimal(new int[4] { 1410065407, 2, 0, 0 });
			nudErrorDelay.Name = "nudErrorDelay";
			nudErrorDelay.Size = new System.Drawing.Size(54, 20);
			nudErrorDelay.TabIndex = 162;
			nudErrorDelay.Value = new decimal(new int[4] { 30, 0, 0, 0 });
			rbtStop.AutoSize = true;
			rbtStop.Location = new System.Drawing.Point(23, 29);
			rbtStop.Name = "rbtStop";
			rbtStop.Size = new System.Drawing.Size(99, 17);
			rbtStop.TabIndex = 160;
			rbtStop.Text = "Gặp lỗi thì dừng";
			rbtStop.UseVisualStyleBackColor = true;
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(22, 89);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(74, 13);
			label7.TabIndex = 6;
			label7.Text = "Thời gian nghỉ";
			groupBox4.Controls.Add(lblAhi);
			groupBox4.Controls.Add(lblOmo);
			groupBox4.Controls.Add(label10);
			groupBox4.Controls.Add(label2);
			groupBox4.Controls.Add(linkLabel1);
			groupBox4.Controls.Add(txtAhichaptcha);
			groupBox4.Controls.Add(label9);
			groupBox4.Controls.Add(cbxUnlockCaptcha);
			groupBox4.Controls.Add(label1);
			groupBox4.Controls.Add(nudCaptchaDelay);
			groupBox4.Controls.Add(linkLabel3);
			groupBox4.Controls.Add(txtcaptcha);
			groupBox4.Location = new System.Drawing.Point(567, 139);
			groupBox4.Name = "groupBox4";
			groupBox4.Size = new System.Drawing.Size(370, 136);
			groupBox4.TabIndex = 166;
			groupBox4.TabStop = false;
			groupBox4.Text = "Cấu hình Captcha";
			lblAhi.AutoSize = true;
			lblAhi.Location = new System.Drawing.Point(318, 83);
			lblAhi.Name = "lblAhi";
			lblAhi.Size = new System.Drawing.Size(13, 13);
			lblAhi.TabIndex = 171;
			lblAhi.Text = "0";
			lblOmo.AutoSize = true;
			lblOmo.Location = new System.Drawing.Point(318, 57);
			lblOmo.Name = "lblOmo";
			lblOmo.Size = new System.Drawing.Size(13, 13);
			lblOmo.TabIndex = 170;
			lblOmo.Text = "0";
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(244, 83);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(72, 13);
			label10.TabIndex = 169;
			label10.Text = "Captcha xoay";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(244, 57);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(68, 13);
			label2.TabIndex = 168;
			label2.Text = "Captcha chữ";
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(19, 83);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(89, 13);
			linkLabel1.TabIndex = 166;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "achicaptcha.com";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked_2);
			txtAhichaptcha.Location = new System.Drawing.Point(114, 80);
			txtAhichaptcha.Name = "txtAhichaptcha";
			txtAhichaptcha.Size = new System.Drawing.Size(122, 20);
			txtAhichaptcha.TabIndex = 167;
			toolTipachi.SetToolTip(txtAhichaptcha, "Gỡ captcha xoay và captcha 2 đối tượng");
			txtAhichaptcha.TextChanged += new System.EventHandler(txtAhichaptcha_TextChanged);
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(210, 27);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(26, 13);
			label9.TabIndex = 165;
			label9.Text = "giây";
			groupBox1.Controls.Add(rbtFull);
			groupBox1.Controls.Add(rbt8Nick);
			groupBox1.Location = new System.Drawing.Point(567, 359);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(370, 52);
			groupBox1.TabIndex = 167;
			groupBox1.TabStop = false;
			groupBox1.Text = "Số lượng nick cần đăng ký";
			rbtFull.AutoSize = true;
			rbtFull.Checked = true;
			rbtFull.Location = new System.Drawing.Point(184, 20);
			rbtFull.Name = "rbtFull";
			rbtFull.Size = new System.Drawing.Size(175, 17);
			rbtFull.TabIndex = 162;
			rbtFull.TabStop = true;
			rbtFull.Text = "Đăng ký liên tục không giới hạn";
			rbtFull.UseVisualStyleBackColor = true;
			rbt8Nick.AutoSize = true;
			rbt8Nick.Location = new System.Drawing.Point(43, 22);
			rbt8Nick.Name = "rbt8Nick";
			rbt8Nick.Size = new System.Drawing.Size(114, 17);
			rbt8Nick.TabIndex = 161;
			rbt8Nick.Text = "Chỉ đăng ký 8 nick";
			rbt8Nick.UseVisualStyleBackColor = true;
			groupBox10.Controls.Add(rbtSmartOtp);
			groupBox10.Controls.Add(rbtViotp);
			groupBox10.Controls.Add(linkLabel2);
			groupBox10.Controls.Add(txtSmartOtp);
			groupBox10.Controls.Add(linkLabel4);
			groupBox10.Controls.Add(txtViotp);
			groupBox10.Location = new System.Drawing.Point(26, 417);
			groupBox10.Name = "groupBox10";
			groupBox10.Size = new System.Drawing.Size(301, 110);
			groupBox10.TabIndex = 177;
			groupBox10.TabStop = false;
			groupBox10.Text = "SMS Config";
			rbtSmartOtp.AutoSize = true;
			rbtSmartOtp.Checked = true;
			rbtSmartOtp.Location = new System.Drawing.Point(30, 27);
			rbtSmartOtp.Name = "rbtSmartOtp";
			rbtSmartOtp.Size = new System.Drawing.Size(67, 17);
			rbtSmartOtp.TabIndex = 135;
			rbtSmartOtp.TabStop = true;
			rbtSmartOtp.Text = "Smartotp";
			rbtSmartOtp.UseVisualStyleBackColor = true;
			rbtViotp.AutoSize = true;
			rbtViotp.Location = new System.Drawing.Point(30, 58);
			rbtViotp.Name = "rbtViotp";
			rbtViotp.Size = new System.Drawing.Size(49, 17);
			rbtViotp.TabIndex = 135;
			rbtViotp.Text = "Viotp";
			rbtViotp.UseVisualStyleBackColor = true;
			linkLabel2.AutoSize = true;
			linkLabel2.Location = new System.Drawing.Point(217, 59);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(56, 13);
			linkLabel2.TabIndex = 137;
			linkLabel2.TabStop = true;
			linkLabel2.Text = "Open Link";
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked_2);
			txtSmartOtp.Location = new System.Drawing.Point(103, 26);
			txtSmartOtp.Name = "txtSmartOtp";
			txtSmartOtp.Size = new System.Drawing.Size(104, 20);
			txtSmartOtp.TabIndex = 128;
			linkLabel4.AutoSize = true;
			linkLabel4.Location = new System.Drawing.Point(217, 30);
			linkLabel4.Name = "linkLabel4";
			linkLabel4.Size = new System.Drawing.Size(56, 13);
			linkLabel4.TabIndex = 136;
			linkLabel4.TabStop = true;
			linkLabel4.Text = "Open Link";
			linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel4_LinkClicked_1);
			txtViotp.Location = new System.Drawing.Point(103, 55);
			txtViotp.Name = "txtViotp";
			txtViotp.Size = new System.Drawing.Size(104, 20);
			txtViotp.TabIndex = 128;
			groupBox6.Controls.Add(rbtRegMail);
			groupBox6.Controls.Add(rbtRegPhone);
			groupBox6.Location = new System.Drawing.Point(567, 475);
			groupBox6.Name = "groupBox6";
			groupBox6.Size = new System.Drawing.Size(370, 52);
			groupBox6.TabIndex = 178;
			groupBox6.TabStop = false;
			groupBox6.Text = "Reg Type";
			rbtRegMail.AutoSize = true;
			rbtRegMail.Checked = true;
			rbtRegMail.Location = new System.Drawing.Point(205, 19);
			rbtRegMail.Name = "rbtRegMail";
			rbtRegMail.Size = new System.Drawing.Size(82, 17);
			rbtRegMail.TabIndex = 163;
			rbtRegMail.TabStop = true;
			rbtRegMail.Text = "Reg By Mail";
			rbtRegMail.UseVisualStyleBackColor = true;
			rbtRegPhone.AutoSize = true;
			rbtRegPhone.Location = new System.Drawing.Point(28, 22);
			rbtRegPhone.Name = "rbtRegPhone";
			rbtRegPhone.Size = new System.Drawing.Size(94, 17);
			rbtRegPhone.TabIndex = 162;
			rbtRegPhone.Text = "Reg By Phone";
			rbtRegPhone.UseVisualStyleBackColor = true;
			rbtRegPhone.Visible = false;
			toolTipachi.ToolTipTitle = "Gỡ captcha xoay và captcha 2 đối tượng";
			groupBox8.Controls.Add(rbtAppMain);
			groupBox8.Controls.Add(rbtLite);
			groupBox8.Location = new System.Drawing.Point(336, 417);
			groupBox8.Name = "groupBox8";
			groupBox8.Size = new System.Drawing.Size(219, 52);
			groupBox8.TabIndex = 177;
			groupBox8.TabStop = false;
			groupBox8.Text = "App Type";
			rbtAppMain.AutoSize = true;
			rbtAppMain.Checked = true;
			rbtAppMain.Location = new System.Drawing.Point(21, 21);
			rbtAppMain.Name = "rbtAppMain";
			rbtAppMain.Size = new System.Drawing.Size(77, 17);
			rbtAppMain.TabIndex = 135;
			rbtAppMain.TabStop = true;
			rbtAppMain.Text = "Tiktok App";
			rbtAppMain.UseVisualStyleBackColor = true;
			rbtAppMain.CheckedChanged += new System.EventHandler(rbtAppMain_CheckedChanged);
			rbtLite.AutoSize = true;
			rbtLite.Location = new System.Drawing.Point(112, 21);
			rbtLite.Name = "rbtLite";
			rbtLite.Size = new System.Drawing.Size(75, 17);
			rbtLite.TabIndex = 135;
			rbtLite.Text = "Tiktok Lite";
			rbtLite.UseVisualStyleBackColor = true;
			rbtLite.CheckedChanged += new System.EventHandler(rbtLite_CheckedChanged);
			groupBox9.Controls.Add(label14);
			groupBox9.Controls.Add(nudDelayTo);
			groupBox9.Controls.Add(label12);
			groupBox9.Controls.Add(label13);
			groupBox9.Controls.Add(nudGmailDelay);
			groupBox9.Location = new System.Drawing.Point(336, 475);
			groupBox9.Name = "groupBox9";
			groupBox9.Size = new System.Drawing.Size(219, 52);
			groupBox9.TabIndex = 179;
			groupBox9.TabStop = false;
			groupBox9.Text = "Login Gmail Delay";
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(180, 25);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(26, 13);
			label14.TabIndex = 168;
			label14.Text = "giây";
			nudDelayTo.Location = new System.Drawing.Point(136, 22);
			nudDelayTo.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudDelayTo.Minimum = new decimal(new int[4] { 6, 0, 0, 0 });
			nudDelayTo.Name = "nudDelayTo";
			nudDelayTo.Size = new System.Drawing.Size(38, 20);
			nudDelayTo.TabIndex = 167;
			nudDelayTo.Value = new decimal(new int[4] { 6, 0, 0, 0 });
			nudDelayTo.ValueChanged += new System.EventHandler(nudDelayTo_ValueChanged);
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(102, 25);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(26, 13);
			label12.TabIndex = 166;
			label12.Text = "đến";
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(24, 25);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(20, 13);
			label13.TabIndex = 164;
			label13.Text = "Từ";
			nudGmailDelay.Location = new System.Drawing.Point(57, 22);
			nudGmailDelay.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudGmailDelay.Minimum = new decimal(new int[4] { 5, 0, 0, 0 });
			nudGmailDelay.Name = "nudGmailDelay";
			nudGmailDelay.Size = new System.Drawing.Size(39, 20);
			nudGmailDelay.TabIndex = 165;
			nudGmailDelay.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			nudGmailDelay.ValueChanged += new System.EventHandler(nudGmailDelay_ValueChanged);
			groupBox11.Controls.Add(rbtGetnada);
			groupBox11.Controls.Add(rbtVerifyHotmail);
			groupBox11.Controls.Add(rbt1Secmail);
			groupBox11.Location = new System.Drawing.Point(26, 338);
			groupBox11.Name = "groupBox11";
			groupBox11.Size = new System.Drawing.Size(273, 73);
			groupBox11.TabIndex = 180;
			groupBox11.TabStop = false;
			groupBox11.Text = "Xác thực bằng Email";
			rbtGetnada.AutoSize = true;
			rbtGetnada.Location = new System.Drawing.Point(142, 22);
			rbtGetnada.Name = "rbtGetnada";
			rbtGetnada.Size = new System.Drawing.Size(89, 17);
			rbtGetnada.TabIndex = 135;
			rbtGetnada.Text = "Getnada.com";
			rbtGetnada.UseVisualStyleBackColor = true;
			rbtGetnada.CheckedChanged += new System.EventHandler(rbtGmailAppDomain_CheckedChanged);
			rbtVerifyHotmail.AutoSize = true;
			rbtVerifyHotmail.Location = new System.Drawing.Point(15, 46);
			rbtVerifyHotmail.Name = "rbtVerifyHotmail";
			rbtVerifyHotmail.Size = new System.Drawing.Size(108, 17);
			rbtVerifyHotmail.TabIndex = 135;
			rbtVerifyHotmail.Text = "Hotmail / Outlook";
			rbtVerifyHotmail.UseVisualStyleBackColor = true;
			rbtVerifyHotmail.CheckedChanged += new System.EventHandler(rbtVerifyHotmail_CheckedChanged);
			groupBox12.Controls.Add(cbxAppName);
			groupBox12.Controls.Add(cbxCheckVerrsion);
			groupBox12.Location = new System.Drawing.Point(315, 338);
			groupBox12.Name = "groupBox12";
			groupBox12.Size = new System.Drawing.Size(240, 73);
			groupBox12.TabIndex = 180;
			groupBox12.TabStop = false;
			groupBox12.Text = "Package Name";
			cbxAppName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxAppName.FormattingEnabled = true;
			cbxAppName.Items.AddRange(new object[2] { "com.ss.android.ugc.trill", "com.zhiliaoapp.musically" });
			cbxAppName.Location = new System.Drawing.Point(17, 20);
			cbxAppName.Name = "cbxAppName";
			cbxAppName.Size = new System.Drawing.Size(210, 21);
			cbxAppName.TabIndex = 126;
			cbxAppName.SelectedIndexChanged += new System.EventHandler(cbxAppName_SelectedIndexChanged);
			cbxCheckVerrsion.AutoSize = true;
			cbxCheckVerrsion.Location = new System.Drawing.Point(17, 50);
			cbxCheckVerrsion.Name = "cbxCheckVerrsion";
			cbxCheckVerrsion.Size = new System.Drawing.Size(217, 17);
			cbxCheckVerrsion.TabIndex = 164;
			cbxCheckVerrsion.Text = "Không kiểm tra phiên bản trước khi chạy";
			cbxCheckVerrsion.UseVisualStyleBackColor = true;
			cbxCheckVerrsion.CheckedChanged += new System.EventHandler(cbxCheckVerrsion_CheckedChanged);
			groupBox13.Controls.Add(label15);
			groupBox13.Controls.Add(label17);
			groupBox13.Controls.Add(label11);
			groupBox13.Controls.Add(nudDelayRename);
			groupBox13.Controls.Add(label16);
			groupBox13.Controls.Add(nudDelayBirthday);
			groupBox13.Location = new System.Drawing.Point(26, 533);
			groupBox13.Name = "groupBox13";
			groupBox13.Size = new System.Drawing.Size(376, 52);
			groupBox13.TabIndex = 179;
			groupBox13.TabStop = false;
			groupBox13.Text = "Delay Rename";
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(170, 22);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(26, 13);
			label15.TabIndex = 169;
			label15.Text = "giây";
			label17.AutoSize = true;
			label17.Location = new System.Drawing.Point(207, 23);
			label17.Name = "label17";
			label17.Size = new System.Drawing.Size(86, 13);
			label17.TabIndex = 168;
			label17.Text = "Nghỉ chờ đổi tên";
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(343, 22);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(26, 13);
			label11.TabIndex = 168;
			label11.Text = "giây";
			nudDelayRename.Location = new System.Drawing.Point(299, 19);
			nudDelayRename.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudDelayRename.Minimum = new decimal(new int[4] { 6, 0, 0, 0 });
			nudDelayRename.Name = "nudDelayRename";
			nudDelayRename.Size = new System.Drawing.Size(38, 20);
			nudDelayRename.TabIndex = 167;
			nudDelayRename.Value = new decimal(new int[4] { 30, 0, 0, 0 });
			nudDelayRename.ValueChanged += new System.EventHandler(nudDelayRename_ValueChanged);
			label16.AutoSize = true;
			label16.Location = new System.Drawing.Point(13, 25);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(98, 13);
			label16.TabIndex = 164;
			label16.Text = "Nghỉ chờ ngày sinh";
			nudDelayBirthday.Location = new System.Drawing.Point(117, 20);
			nudDelayBirthday.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudDelayBirthday.Minimum = new decimal(new int[4] { 5, 0, 0, 0 });
			nudDelayBirthday.Name = "nudDelayBirthday";
			nudDelayBirthday.Size = new System.Drawing.Size(39, 20);
			nudDelayBirthday.TabIndex = 165;
			nudDelayBirthday.Value = new decimal(new int[4] { 30, 0, 0, 0 });
			nudDelayBirthday.ValueChanged += new System.EventHandler(nudDelayBirthday_ValueChanged);
			btnLogEmail.Location = new System.Drawing.Point(377, 607);
			btnLogEmail.Name = "btnLogEmail";
			btnLogEmail.Size = new System.Drawing.Size(187, 33);
			btnLogEmail.TabIndex = 0;
			btnLogEmail.Text = "Lọc Email Chưa Sử Dụng";
			btnLogEmail.UseVisualStyleBackColor = true;
			btnLogEmail.Click += new System.EventHandler(btnLogEmail_Click);
			RemoveLog.Location = new System.Drawing.Point(570, 607);
			RemoveLog.Name = "RemoveLog";
			RemoveLog.Size = new System.Drawing.Size(134, 33);
			RemoveLog.TabIndex = 0;
			RemoveLog.Text = "Xóa Log Email";
			RemoveLog.UseVisualStyleBackColor = true;
			RemoveLog.Click += new System.EventHandler(RemoveLog_Click);
			groupBox14.Controls.Add(txtFileContact);
			groupBox14.Controls.Add(cbxImportContact);
			groupBox14.Controls.Add(nudNumberContact);
			groupBox14.Controls.Add(label19);
			groupBox14.Controls.Add(label20);
			groupBox14.Controls.Add(label18);
			groupBox14.Controls.Add(btnFile);
			groupBox14.Location = new System.Drawing.Point(408, 534);
			groupBox14.Name = "groupBox14";
			groupBox14.Size = new System.Drawing.Size(529, 51);
			groupBox14.TabIndex = 181;
			groupBox14.TabStop = false;
			groupBox14.Text = "Tạo danh bạ từ số điện thoại";
			txtFileContact.Location = new System.Drawing.Point(245, 19);
			txtFileContact.Name = "txtFileContact";
			txtFileContact.Size = new System.Drawing.Size(36, 20);
			txtFileContact.TabIndex = 161;
			cbxImportContact.AutoSize = true;
			cbxImportContact.Location = new System.Drawing.Point(14, 21);
			cbxImportContact.Name = "cbxImportContact";
			cbxImportContact.Size = new System.Drawing.Size(94, 17);
			cbxImportContact.TabIndex = 166;
			cbxImportContact.Text = "Nhập danh bạ";
			cbxImportContact.UseVisualStyleBackColor = true;
			nudNumberContact.Location = new System.Drawing.Point(417, 18);
			nudNumberContact.Maximum = new decimal(new int[4] { 1410065407, 2, 0, 0 });
			nudNumberContact.Name = "nudNumberContact";
			nudNumberContact.Size = new System.Drawing.Size(54, 20);
			nudNumberContact.TabIndex = 162;
			nudNumberContact.Value = new decimal(new int[4] { 30, 0, 0, 0 });
			label19.AutoSize = true;
			label19.Location = new System.Drawing.Point(476, 20);
			label19.Name = "label19";
			label19.Size = new System.Drawing.Size(33, 13);
			label19.TabIndex = 163;
			label19.Text = "người";
			label20.AutoSize = true;
			label20.Location = new System.Drawing.Point(287, 22);
			label20.Name = "label20";
			label20.Size = new System.Drawing.Size(13, 13);
			label20.TabIndex = 163;
			label20.Text = "0";
			label18.AutoSize = true;
			label18.Location = new System.Drawing.Point(360, 21);
			label18.Name = "label18";
			label18.Size = new System.Drawing.Size(49, 13);
			label18.TabIndex = 163;
			label18.Text = "Số lượng";
			btnFile.Location = new System.Drawing.Point(114, 17);
			btnFile.Margin = new System.Windows.Forms.Padding(2);
			btnFile.Name = "btnFile";
			btnFile.Size = new System.Drawing.Size(126, 24);
			btnFile.TabIndex = 124;
			btnFile.Text = "Chọn file số điện thoại";
			btnFile.UseVisualStyleBackColor = true;
			btnFile.Click += new System.EventHandler(btnFile_Click);
			cbxClearapp.AutoSize = true;
			cbxClearapp.Location = new System.Drawing.Point(207, 107);
			cbxClearapp.Name = "cbxClearapp";
			cbxClearapp.Size = new System.Drawing.Size(144, 17);
			cbxClearapp.TabIndex = 166;
			cbxClearapp.Text = "Clear app bằng giao diện";
			cbxClearapp.UseVisualStyleBackColor = true;
			cbxClearapp.CheckedChanged += new System.EventHandler(cbxClearapp_CheckedChanged);
			base.AcceptButton = btnSAve;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(960, 652);
			base.Controls.Add(groupBox14);
			base.Controls.Add(groupBox12);
			base.Controls.Add(groupBox11);
			base.Controls.Add(groupBox13);
			base.Controls.Add(groupBox9);
			base.Controls.Add(groupBox6);
			base.Controls.Add(groupBox8);
			base.Controls.Add(groupBox10);
			base.Controls.Add(groupBox1);
			base.Controls.Add(groupBox4);
			base.Controls.Add(groupBox3);
			base.Controls.Add(groupBox2);
			base.Controls.Add(groupBox7);
			base.Controls.Add(RemoveLog);
			base.Controls.Add(btnLogEmail);
			base.Controls.Add(btnSAve);
			base.Controls.Add(groupBox5);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmVerifyEmail";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Cấu hình đăng ký tài khoản";
			base.Load += new System.EventHandler(frmVerifyEmail_Load);
			groupBox7.ResumeLayout(false);
			groupBox7.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudCaptchaDelay).EndInit();
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudAgeFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudAgeTo).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudErrorDelay).EndInit();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox10.ResumeLayout(false);
			groupBox10.PerformLayout();
			groupBox6.ResumeLayout(false);
			groupBox6.PerformLayout();
			groupBox8.ResumeLayout(false);
			groupBox8.PerformLayout();
			groupBox9.ResumeLayout(false);
			groupBox9.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudGmailDelay).EndInit();
			groupBox11.ResumeLayout(false);
			groupBox11.PerformLayout();
			groupBox12.ResumeLayout(false);
			groupBox12.PerformLayout();
			groupBox13.ResumeLayout(false);
			groupBox13.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudDelayRename).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayBirthday).EndInit();
			groupBox14.ResumeLayout(false);
			groupBox14.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudNumberContact).EndInit();
			ResumeLayout(false);
		}
	}
}
