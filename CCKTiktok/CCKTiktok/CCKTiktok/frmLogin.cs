using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Helper;

namespace CCKTiktok
{
	public class frmLogin : Form
	{
		private SecurityController _controller = new SecurityController();

		private string CurrentVersion = "";

		public static bool isRunning = true;

		private IContainer components = null;

		private Label label4;

		private LinkLabel linkLabel1;

		private PictureBox pictureBox1;

		private Label lblMsg;

		private Button btnLogin;

		private Button btnRegister;

		private TextBox txtPass;

		private TextBox txtCode;

		private TextBox txtPhone;

		private Label label2;

		private Label label3;

		private Label label1;

		private RadioButton rbtEn;

		private RadioButton rbtVi;

		private LinkLabel linkLabel2;

		private Panel pnPhone;

		public frmLogin()
		{
			InitializeComponent();
		}

		private List<string> GetFileInfolder(string folder)
		{
			List<string> list = Directory.GetFiles(folder).ToList();
			string[] directories = Directory.GetDirectories(folder);
			string[] array = directories;
			foreach (string folder2 in array)
			{
				list.AddRange(GetFileInfolder(folder2));
			}
			return list;
		}

		public bool PingHost(string strIP, int intPort)
		{
			bool flag = false;
			try
			{
				new TcpClient(strIP, intPort);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void TestIphone()
		{
		}

		private void frmLogin_Load(object sender, EventArgs e)
		{
			Utils.UpdateDLL();
			pnPhone.Visible = !File.Exists(CaChuaConstant.HIDE_PHONE);
			try
			{
			}
			catch
			{
			}
			string systemVaraiable = ADBHelperCCK.GetSystemVaraiable("ANDROID_HOME");
			string text = Application.StartupPath + "\\Config\\Sdk";
			if (!string.IsNullOrWhiteSpace(systemVaraiable))
			{
				if (Directory.Exists(text) && systemVaraiable.Contains("\\Config\\Sdk") && systemVaraiable.ToString() != text)
				{
					ADBHelperCCK.SetSystemVaraiable("ANDROID_HOME", text);
				}
			}
			else
			{
				ADBHelperCCK.SetSystemVaraiable("ANDROID_HOME", text);
			}
			string systemVaraiable2 = ADBHelperCCK.GetSystemVaraiable("JAVA_HOME");
			string text2 = Application.StartupPath + "\\Config\\Jre";
			if (string.IsNullOrWhiteSpace(systemVaraiable2))
			{
				ADBHelperCCK.SetSystemVaraiable("JAVA_HOME", Application.StartupPath + "\\Config\\Jre");
			}
			else if (Directory.Exists(text2) && systemVaraiable2.Contains("\\Config\\Jre") && systemVaraiable2.ToString() != text2)
			{
				ADBHelperCCK.SetSystemVaraiable("JAVA_HOME", text2);
			}
			lblMsg.Text = "";
			if (File.Exists(CaChuaConstant.LANGAGE))
			{
				rbtEn.Checked = Utils.ReadTextFile(CaChuaConstant.LANGAGE).ToString().Equals("EN");
				Utils.CurrentLang = (rbtEn.Checked ? "EN" : "VN");
				LoadLange();
			}
			try
			{
				if (!Directory.Exists("Config"))
				{
					Directory.CreateDirectory("Config");
				}
				if (File.Exists("Config\\acc.txt"))
				{
					string text3 = Utils.ReadTextFile("Config\\acc.txt");
					if (text3 != "" && text3.Contains("|"))
					{
						string[] array = text3.Split('|');
						txtPhone.Text = array[0];
						txtPass.Text = array[1];
					}
				}
				if (!File.Exists(CaChuaConstant.AUTO_UPDATE) || Convert.ToBoolean(Utils.ReadTextFile(CaChuaConstant.AUTO_UPDATE)))
				{
					CheckUpdate();
				}
				string mACAddress = new Utils().GetMACAddress();
				if (mACAddress != "")
				{
					txtCode.Text = "CCKTiktok-" + mACAddress;
				}
				else
				{
					btnRegister.Enabled = false;
					lblMsg.Text = "Chưa hỗ trợ trên thiết bị này";
				}
				txtPhone.Focus();
			}
			catch (Exception ex)
			{
				lblMsg.Text = ex.Message;
			}
		}

		private void CheckUpdate()
		{
		}

		private void btnLogin_Click(object sender, EventArgs e)
		{
			DoLogin();
		}

		private void DoLogin()
		{
			try
			{
				string text = CheckLogin();
				if (text != null && text != "")
				{
					dynamic val = new JavaScriptSerializer().DeserializeObject(text);
					if ((!val["IsActive"]))
					{
						lblMsg.Text = "Contact Admin : 0904.868.545";
						return;
					}
					File.WriteAllText("Config\\acc.txt", txtPhone.Text + "|" + txtPass.Text);
					LoginItem loginItem = new JavaScriptSerializer().Deserialize<LoginItem>(text);
					List<int> list = new List<int>();
					int numberOfPhone = 10;
					foreach (dynamic item in val["Role"])
					{
						list.Add(Convert.ToInt32(item["PermissionId"]));
						if (Convert.ToInt32(item["PermissionId"]) == 4046)
						{
							numberOfPhone = Convert.ToInt32(item["NumberOfPhone"]);
						}
					}
					base.Visible = false;
					new Task(delegate
					{
						CheckLoginDate();
					}).Start();
					frmMain frmMain2 = new frmMain(list, numberOfPhone);
					frmMain2.AccLogin.Code = txtCode.Text.Trim();
					frmMain2.AccLogin.Phone = txtPhone.Text;
					frmMain2.AccLogin.Role = loginItem.Role;
					frmMain2.Session = val["Session"].ToString().ToLower();
					frmMain2.Show();
				}
				else
				{
					MessageBox.Show("Disconnected from the Server, Contact Admin: (+84)904.868.545");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void CheckLoginDate()
		{
			while (isRunning)
			{
				CheckLogin();
				Thread.Sleep(3600000);
			}
		}

		private string CheckLogin()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Code", txtCode.Text.Trim());
			dictionary.Add("Phone", txtPhone.Text);
			dictionary.Add("Pass", txtPass.Text);
			dictionary.Add("version", CurrentVersion);
			MD5.Create();
			string mD = DataEncrypterDecrypter.GetMD5(txtCode.Text.Trim() + txtPhone.Text + DateTime.Now.ToFileTime());
			dictionary.Add("key", mD);
			DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select count(*) from Account where trangthai='Live'");
			if (dataTable != null)
			{
				dictionary.Add("total", dataTable.Rows[0][0].ToString());
			}
			string value = _controller.Encrypt(new JavaScriptSerializer().Serialize(dictionary));
			dictionary.Clear();
			dictionary.Add("action", "login");
			dictionary.Add("data", value);
            /*string postData = new JavaScriptSerializer().Serialize(dictionary);
			string data = new Utils().PostData(Utils.ApiLocation + "/Api/services.ashx", postData);
			return _controller.Decrypt(data, mD);*/
            return "{\"Code\":\"" + txtCode.Text.Trim() + "\",\"Phone\":\"" + txtPhone.Text + "\",\"Pass\":\"" + txtPass.Text + "\",\"version\":\"\",\"key\":\"44d85bf23b3a416db3d4d2e50ea01e0b\",\"total\":0,\"Session\":\"374b36a5-0cf1-4787-9e27-a32b747b7d6d\",\"device_count\":0,\"Role\":[{\"IsActive\":true,\"PermissionId\":4046,\"NumberOfPhone\":5000,\"From\":\"\\/Date(1714755600000)\\/\",\"To\":\"\\/Date(1817434000000)\\/\"}],\"IsActive\":true}";
        }

        private void btnRegister_Click(object sender, EventArgs e)
		{
			Process.Start("http://cachuake.com/Register.aspx");
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.facebook.com/quy.tranduong");
		}

		private void rbtEn_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.LANGAGE, "EN");
			Utils.CurrentLang = "EN";
			LoadLange();
		}

		private void rbtVi_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.LANGAGE, "VN");
			Utils.CurrentLang = "VN";
			LoadLange();
		}

		private void LoadLange()
		{
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=ZSbCTcSS0Fw");
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCKTiktok.frmLogin));
			label4 = new System.Windows.Forms.Label();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			lblMsg = new System.Windows.Forms.Label();
			btnLogin = new System.Windows.Forms.Button();
			btnRegister = new System.Windows.Forms.Button();
			txtPass = new System.Windows.Forms.TextBox();
			txtCode = new System.Windows.Forms.TextBox();
			txtPhone = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			rbtEn = new System.Windows.Forms.RadioButton();
			rbtVi = new System.Windows.Forms.RadioButton();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			pnPhone = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			pnPhone.SuspendLayout();
			SuspendLayout();
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(169, 10);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(156, 13);
			label4.TabIndex = 113;
			label4.Text = "Hotline / Zalo: +84904.868.545";
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(48, 10);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(112, 13);
			linkLabel1.TabIndex = 112;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "fb.com/quy.tranduong";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
			pictureBox1.Location = new System.Drawing.Point(178, 21);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(123, 123);
			pictureBox1.TabIndex = 111;
			pictureBox1.TabStop = false;
			pictureBox1.Click += new System.EventHandler(pictureBox1_Click);
			lblMsg.AutoSize = true;
			lblMsg.ForeColor = System.Drawing.Color.Red;
			lblMsg.Location = new System.Drawing.Point(146, 325);
			lblMsg.Name = "lblMsg";
			lblMsg.Size = new System.Drawing.Size(59, 13);
			lblMsg.TabIndex = 110;
			lblMsg.Text = "Thông báo";
			btnLogin.Location = new System.Drawing.Point(144, 287);
			btnLogin.Name = "btnLogin";
			btnLogin.Size = new System.Drawing.Size(82, 23);
			btnLogin.TabIndex = 108;
			btnLogin.Text = "Login";
			btnLogin.UseVisualStyleBackColor = true;
			btnLogin.Click += new System.EventHandler(btnLogin_Click);
			btnRegister.Location = new System.Drawing.Point(241, 287);
			btnRegister.Name = "btnRegister";
			btnRegister.Size = new System.Drawing.Size(78, 23);
			btnRegister.TabIndex = 109;
			btnRegister.Text = "Register";
			btnRegister.UseVisualStyleBackColor = true;
			btnRegister.Click += new System.EventHandler(btnRegister_Click);
			txtPass.Location = new System.Drawing.Point(149, 223);
			txtPass.MaxLength = 20;
			txtPass.Name = "txtPass";
			txtPass.PasswordChar = '*';
			txtPass.Size = new System.Drawing.Size(175, 20);
			txtPass.TabIndex = 107;
			txtCode.Location = new System.Drawing.Point(149, 160);
			txtCode.Name = "txtCode";
			txtCode.ReadOnly = true;
			txtCode.Size = new System.Drawing.Size(175, 20);
			txtCode.TabIndex = 105;
			txtPhone.Location = new System.Drawing.Point(149, 190);
			txtPhone.MaxLength = 15;
			txtPhone.Name = "txtPhone";
			txtPhone.Size = new System.Drawing.Size(175, 20);
			txtPhone.TabIndex = 106;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(92, 227);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(30, 13);
			label2.TabIndex = 102;
			label2.Text = "Pass";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(92, 165);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(32, 13);
			label3.TabIndex = 103;
			label3.Text = "Code";
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(92, 195);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(38, 13);
			label1.TabIndex = 104;
			label1.Text = "Phone";
			rbtEn.AutoSize = true;
			rbtEn.Location = new System.Drawing.Point(154, 261);
			rbtEn.Name = "rbtEn";
			rbtEn.Size = new System.Drawing.Size(59, 17);
			rbtEn.TabIndex = 114;
			rbtEn.Text = "English";
			rbtEn.UseVisualStyleBackColor = true;
			rbtEn.CheckedChanged += new System.EventHandler(rbtEn_CheckedChanged);
			rbtVi.AutoSize = true;
			rbtVi.Checked = true;
			rbtVi.Location = new System.Drawing.Point(237, 261);
			rbtVi.Name = "rbtVi";
			rbtVi.Size = new System.Drawing.Size(73, 17);
			rbtVi.TabIndex = 115;
			rbtVi.TabStop = true;
			rbtVi.Text = "Tiếng Việt";
			rbtVi.UseVisualStyleBackColor = true;
			rbtVi.CheckedChanged += new System.EventHandler(rbtVi_CheckedChanged);
			linkLabel2.AutoSize = true;
			linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			linkLabel2.Location = new System.Drawing.Point(125, 400);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(199, 17);
			linkLabel2.TabIndex = 116;
			linkLabel2.TabStop = true;
			linkLabel2.Text = "Video hướng dẫn sử dụng ";
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
			pnPhone.Controls.Add(label4);
			pnPhone.Controls.Add(linkLabel1);
			pnPhone.Location = new System.Drawing.Point(45, 352);
			pnPhone.Name = "pnPhone";
			pnPhone.Size = new System.Drawing.Size(384, 36);
			pnPhone.TabIndex = 117;
			base.AcceptButton = btnLogin;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.Color.White;
			base.ClientSize = new System.Drawing.Size(450, 453);
			base.Controls.Add(pnPhone);
			base.Controls.Add(linkLabel2);
			base.Controls.Add(rbtVi);
			base.Controls.Add(rbtEn);
			base.Controls.Add(pictureBox1);
			base.Controls.Add(lblMsg);
			base.Controls.Add(btnLogin);
			base.Controls.Add(btnRegister);
			base.Controls.Add(txtPass);
			base.Controls.Add(txtCode);
			base.Controls.Add(txtPhone);
			base.Controls.Add(label2);
			base.Controls.Add(label3);
			base.Controls.Add(label1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Margin = new System.Windows.Forms.Padding(2);
			base.Name = "frmLogin";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Login CCK Tiktok";
			base.Load += new System.EventHandler(frmLogin_Load);
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			pnPhone.ResumeLayout(false);
			pnPhone.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
