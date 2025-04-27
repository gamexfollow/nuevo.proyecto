using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.Component;
using CCKTiktok.DAL;
using CCKTiktok.Entity;

namespace CCKTiktok
{
	public class frmAccountInfo : Form
	{
		private int pasttype = 0;

		private IContainer components = null;

		private Button btnUpdate;

		private Label label1;

		private TextBox txtUid;

		private Label label2;

		private TextBox txtEmail;

		private Label label3;

		private Label label4;

		private TextBox txtName;

		private Label label5;

		private TextBox txtPhone;

		private Label label6;

		private TextBox txt2fa;

		private TextBox txtCookie;

		private Label label7;

		private DataGridView dataGridView;

		private Button btnBatch;

		private ProgressBar progressBar1;

		private TabControl tabsingle;

		private TabPage tabPage2;

		private TabPage tabPage1;

		private Label label8;

		private TextBox txttoken;

		private Label label9;

		private TextBox txtProxy;

		private Label label10;

		private TextBox txtBrand;

		private Label label11;

		private TextBox txtPass;

		private TextBox txtPassmail;

		private Label label12;

		private Label label13;

		private TextBox txtUidLayBai;

		private Label label14;

		private ComboBox cbxCategory;

		private Label label15;

		private ComboBox cbxDanhMuc2;

		private Label label16;

		private TextBox txtBirthday;

		private Button button1;

		public string Uid { get; set; }

		public int catid { get; set; }

		public frmAccountInfo()
		{
			Uid = "";
			catid = 0;
			InitializeComponent();
			tabsingle.Show();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData != Keys.Escape)
			{
				return base.ProcessCmdKey(ref msg, keyData);
			}
			Close();
			return true;
		}

		private void frmAccountInfo_Load(object sender, EventArgs e)
		{
			cbxCategory.DataSource = new BindingSource(new SQLiteUtils().ExecuteQuery("Select Id_danhmuc,tendanhmuc from Danhmuc"), null);
			cbxCategory.DisplayMember = "tendanhmuc";
			cbxCategory.ValueMember = "id_danhmuc";
			cbxDanhMuc2.DataSource = new BindingSource(new SQLiteUtils().ExecuteQuery("Select Id_danhmuc,tendanhmuc from Danhmuc"), null);
			cbxDanhMuc2.DisplayMember = "tendanhmuc";
			cbxDanhMuc2.ValueMember = "id_danhmuc";
			DataRow dataRow = ((Uid != "") ? new SQLiteUtils().GetAccountById(Uid) : null);
			if (dataRow != null && Uid != "")
			{
				txtUid.Text = Uid;
				txtUid.ReadOnly = true;
				txtName.Text = dataRow["name"].ToString();
				txtPass.Text = dataRow["password"].ToString();
				txtPhone.Text = dataRow["mobile_phone"].ToString();
				txtEmail.Text = dataRow["email"].ToString();
				txt2fa.Text = dataRow["privatekey"].ToString();
				txtCookie.Text = dataRow["cookies"].ToString();
				txttoken.Text = dataRow["token"].ToString();
				txtProxy.Text = ((dataRow["proxy"] != null) ? dataRow["proxy"].ToString() : "");
				txtBrand.Text = ((dataRow["Brand"] != null) ? dataRow["Brand"].ToString() : "");
				txtPassmail.Text = ((dataRow["passemail"] != null) ? dataRow["passemail"].ToString() : "");
				txtUidLayBai.Text = ((dataRow["uidlaybai"] != null) ? dataRow["uidlaybai"].ToString() : "");
				txtBirthday.Text = ((dataRow["birthday"] != null) ? dataRow["birthday"].ToString() : "");
				cbxDanhMuc2.SelectedValue = dataRow["id_danhmuc"].ToString();
			}
			else
			{
				tabsingle.SelectedIndex = 1;
			}
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			catid = Convert.ToInt32(cbxDanhMuc2.SelectedValue);
			if (catid != 0)
			{
				DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select Id from Account");
				List<string> list = new List<string>();
				foreach (DataRow row in dataTable.Rows)
				{
					list.Add(row["id"].ToString());
				}
				if (Uid != "")
				{
					new SQLiteUtils().UpdateInfo(new TTItems
					{
						TwoFA = txt2fa.Text.Trim(),
						FirstName = txtName.Text.Trim(),
						Email = txtEmail.Text.Trim(),
						Pass = txtPass.Text.Trim(),
						Uid = txtUid.Text.Trim(),
						Phone = txtPhone.Text.Trim(),
						Cookie = txtCookie.Text.Trim(),
						Token = txttoken.Text.Trim(),
						MyProxy = new ProxyInfo(txtProxy.Text),
						Brand = txtBrand.Text,
						PassEmail = txtPassmail.Text,
						UidLayBai = txtUidLayBai.Text,
						FolderId = Convert.ToInt32(cbxDanhMuc2.SelectedValue),
						DateOfBirth = txtBirthday.Text
					});
				}
				else if (!list.Contains(txtUid.Text.Trim()))
				{
					new SQLiteUtils().Insert(new TTItems
					{
						TwoFA = txt2fa.Text.Trim(),
						FirstName = txtName.Text.Trim(),
						Email = txtEmail.Text.Trim(),
						Pass = txtPass.Text.Trim(),
						Uid = txtUid.Text.Trim(),
						TrangThai = "Live",
						Phone = txtPhone.Text.Trim(),
						MyTempId = txtUid.Text.Trim(),
						Cookie = txtCookie.Text.Trim(),
						MyProxy = new ProxyInfo(txtProxy.Text),
						Brand = txtBrand.Text,
						PassEmail = txtPassmail.Text,
						UidLayBai = txtUidLayBai.Text,
						DateOfBirth = txtBirthday.Text
					}, new ProxyInfo(), Convert.ToInt32(cbxDanhMuc2.SelectedValue));
				}
				Close();
			}
			else
			{
				MessageBox.Show("Bạn chưa chọn chuyên mục để thêm vào");
			}
		}

		private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
		}

		private void dataGridView_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenu contextMenu = new ContextMenu();
			MenuItem menuItem = new MenuItem("UID, Pass");
			menuItem.Name = "type1";
			contextMenu.MenuItems.Add(menuItem);
			MenuItem menuItem2 = new MenuItem("Paste - UID, Pass, Email, Pass Email");
			menuItem2.Name = "type2";
			contextMenu.MenuItems.Add(menuItem2);
			menuItem2 = new MenuItem("Email, Pass Tiktok, Pass Email");
			menuItem2.Name = "type3";
			contextMenu.MenuItems.Add(menuItem2);
			menuItem2 = new MenuItem("Paste - UID, Pass, Email, Pass Email, Device Info");
			menuItem2.Name = "type4";
			contextMenu.MenuItems.Add(menuItem2);
			MenuItem menuItem3 = new MenuItem("Import CCK File");
			menuItem3.Name = "type8";
			contextMenu.MenuItems.Add(menuItem3);
			menuItem3 = new MenuItem("Option");
			menuItem3.Name = "type7";
			contextMenu.MenuItems.Add(menuItem3);
			foreach (MenuItem menuItem4 in contextMenu.MenuItems)
			{
				menuItem4.Click += i_Click;
			}
			contextMenu.Show(dataGridView, new Point(e.X, e.Y));
		}

		private void i_Click(object sender, EventArgs e)
		{
			string text = Clipboard.GetText();
			string[] array = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string key = text2.Split('|')[0];
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, 1);
				}
				else
				{
					dictionary[key]++;
				}
			}
			DataTable dataTable = new DataTable();
			if (!dataTable.Columns.Contains("stt"))
			{
				dataTable.Columns.Add("stt");
			}
			if (!dataTable.Columns.Contains("uid"))
			{
				dataTable.Columns.Add("uid");
			}
			if (!dataTable.Columns.Contains("pass"))
			{
				dataTable.Columns.Add("pass");
			}
			if (!dataTable.Columns.Contains("privatekey"))
			{
				dataTable.Columns.Add("privatekey");
			}
			if (!dataTable.Columns.Contains("email"))
			{
				dataTable.Columns.Add("email");
			}
			if (!dataTable.Columns.Contains("passemail"))
			{
				dataTable.Columns.Add("passemail");
			}
			if (!dataTable.Columns.Contains("note"))
			{
				dataTable.Columns.Add("note");
			}
			if (!dataTable.Columns.Contains("Brand"))
			{
				dataTable.Columns.Add("Brand");
			}
			switch (((MenuItem)sender).Name)
			{
			case "type5":
				pasttype = 5;
				break;
			case "type8":
			{
				array = new string[0];
				pasttype = 8;
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Multiselect = false;
				openFileDialog.Filter = "CCK files (*.cck)|*.cck";
				if (openFileDialog.ShowDialog() != DialogResult.OK)
				{
					break;
				}
				new SQLiteUtils();
				string input = File.ReadAllText(openFileDialog.FileName);
				List<CCKBackupItem> data = new List<CCKBackupItem>();
				try
				{
					data = new JavaScriptSerializer().Deserialize<List<CCKBackupItem>>(input);
				}
				catch
				{
				}
				DataTable dataTable2 = data.ToDataTable();
				foreach (DataRow row in dataTable2.Rows)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["uid"] = row["uid"];
					dataRow2["pass"] = row["Password"];
					dataRow2["privatekey"] = row["privatekey"];
					dataRow2["Email"] = ((row["Email"] != null) ? row["Email"] : "");
					dataRow2["PassEmail"] = ((row["PassEmail"] != null) ? row["PassEmail"] : "");
					dataRow2["Brand"] = row["Brand"];
					dataTable.Rows.Add(dataRow2);
				}
				dataTable.AcceptChanges();
				array = new string[0];
				break;
			}
			case "type7":
			{
				pasttype = 7;
				frmAddAccountOrder frmAddAccountOrder = new frmAddAccountOrder();
				frmAddAccountOrder.StartPosition = FormStartPosition.CenterScreen;
				frmAddAccountOrder.ShowDialog();
				dataTable = frmAddAccountOrder.RetTable;
				array = new string[0];
				break;
			}
			case "type4":
				pasttype = 4;
				break;
			case "type1":
				pasttype = 1;
				break;
			case "type6":
				pasttype = 6;
				break;
			case "type2":
				pasttype = 2;
				break;
			case "type3":
				pasttype = 3;
				break;
			}
			int num = 1;
			string[] array3 = array;
			foreach (string text3 in array3)
			{
				DataRow dataRow3 = dataTable.NewRow();
				string[] array4 = text3.Split('|');
				dataRow3["stt"] = num++;
				if (dictionary.ContainsKey(array4[0]) && dictionary[array4[0]] > 1)
				{
					dataRow3["note"] = dictionary[array4[0]];
				}
				if (array4.Length >= 1)
				{
					dataRow3["uid"] = array4[0];
				}
				if (array4.Length >= 2)
				{
					dataRow3["pass"] = array4[1];
				}
				if (pasttype == 1 && array4.Length >= 3)
				{
					dataRow3["privatekey"] = array4[2];
				}
				else if (pasttype == 3)
				{
					if (array4.Length >= 2)
					{
						dataRow3["uid"] = array4[0];
					}
					if (array4.Length >= 2)
					{
						dataRow3["pass"] = array4[1];
					}
					if (array4.Length >= 2)
					{
						dataRow3["email"] = array4[0];
					}
					if (array4.Length >= 2)
					{
						dataRow3["passemail"] = array4[2];
					}
				}
				else if (pasttype != 2)
				{
					if (pasttype == 4)
					{
						if (array4.Length >= 2)
						{
							dataRow3["uid"] = array4[0];
						}
						if (array4.Length >= 2)
						{
							dataRow3["pass"] = array4[1];
						}
						if (array4.Length >= 2)
						{
							dataRow3["email"] = array4[2];
						}
						if (array4.Length >= 2)
						{
							dataRow3["passemail"] = array4[3];
						}
						if (array4.Length >= 5)
						{
							dataRow3["brand"] = array4[4];
						}
					}
					else if (pasttype == 5)
					{
						if (array4.Length != 0)
						{
							dataRow3["Uid"] = array4[0];
						}
						if (array4.Length >= 1)
						{
							dataRow3["pass"] = array4[1];
						}
						if (array4.Length >= 0)
						{
							dataRow3["email"] = array4[0];
						}
					}
					else if (pasttype != 6)
					{
						if (pasttype == 8 && array4.Length == 4)
						{
							dataRow3["privatekey"] = array4[2];
							dataRow3["brand"] = array4[3];
						}
					}
					else
					{
						if (array4.Length >= 3)
						{
							dataRow3["token"] = array4[2];
						}
						if (array4.Length >= 4)
						{
							dataRow3["cookie"] = array4[3];
						}
						if (array4.Length >= 5)
						{
							dataRow3["privatekey"] = array4[4];
						}
						if (array4.Length >= 6)
						{
							dataRow3["email"] = array4[5];
						}
						if (array4.Length >= 7)
						{
							dataRow3["passemail"] = array4[6];
						}
						if (array4.Length >= 8)
						{
							dataRow3["proxy"] = array4[7];
						}
					}
				}
				else
				{
					if (array4.Length >= 3)
					{
						dataRow3["email"] = array4[2];
					}
					if (array4.Length >= 4)
					{
						dataRow3["passemail"] = array4[3];
					}
				}
				dataTable.Rows.Add(dataRow3);
				dataTable.AcceptChanges();
			}
			dataGridView.DataSource = dataTable;
		}

		private void Insert(int categoryId, string uid, string pass, string token = "", string _2fa = "", string cookie = "", string email = "", string proxy = "", string passemail = "", string birthday = "", string brand = "")
		{
			try
			{
				txt2fa.Text = _2fa;
				txtName.Text = "";
				txtUid.Text = uid;
				txtPass.Text = pass;
				txtCookie.Text = cookie;
				txtEmail.Text = email;
				txtPassmail.Text = passemail;
				txtBirthday.Text = birthday;
				txtProxy.Text = proxy;
				txtBirthday.Text = brand;
				new SQLiteUtils().Insert(new TTItems
				{
					TwoFA = txt2fa.Text.Trim(),
					FirstName = "",
					LastName = "",
					DateOfBirth = birthday,
					Email = txtEmail.Text.Trim(),
					TrangThai = "Live",
					Pass = txtPass.Text.Trim(),
					Uid = txtUid.Text.Trim(),
					Phone = txtPhone.Text.Trim(),
					MyTempId = txtUid.Text.Trim(),
					Cookie = txtCookie.Text.Trim(),
					PassEmail = txtPassmail.Text,
					MyProxy = new ProxyInfo(txtProxy.Text),
					Brand = ((brand != "") ? brand.Replace("^", Environment.NewLine) : "")
				}, new ProxyInfo(proxy), categoryId);
			}
			catch
			{
				Thread.Sleep(5);
			}
		}

		private void btnBatch_Click(object sender, EventArgs e)
		{
			if (Utils.Convert2Int(cbxCategory.SelectedValue.ToString()) != 0)
			{
				new Task(delegate
				{
					DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select Id from Account");
					List<string> list = new List<string>();
					foreach (DataRow row in dataTable.Rows)
					{
						list.Add(row["id"].ToString());
					}
					List<string> list2 = new List<string>();
					if (dataGridView.Rows.Count > 0)
					{
						progressBar1.Maximum = dataGridView.Rows.Count - 1;
						progressBar1.Minimum = 0;
						foreach (DataGridViewRow item in (IEnumerable)dataGridView.Rows)
						{
							if (progressBar1.Value < dataGridView.Rows.Count - 1)
							{
								progressBar1.Value++;
								if (list.Contains(item.Cells["uid"].Value.ToString()))
								{
									list2.Add(item.Cells["uid"].Value.ToString());
								}
								else
								{
									Insert(Convert.ToInt32(cbxCategory.SelectedValue), item.Cells["uid"].Value.ToString(), item.Cells["pass"].Value.ToString(), "", item.Cells["privatekey"].Value.ToString(), "", item.Cells["Email"].Value.ToString(), "", item.Cells["PassEmail"].Value.ToString(), "", item.Cells["Brand"].Value.ToString());
								}
							}
						}
						progressBar1.Value = progressBar1.Maximum;
						if (list2.Count > 0)
						{
							File.WriteAllLines("cck_acc.txt", list2);
							MessageBox.Show("Some Accounts already in the system, not added this time");
							Process.Start("cck_acc.txt");
							list2.Clear();
						}
						MessageBox.Show("Successfully");
						if (File.Exists("cck_acc.txt"))
						{
							File.Delete("cck_acc.txt");
						}
						Close();
					}
					else
					{
						MessageBox.Show("Thêm nick vào đi bạn, không thêm thì click gì");
					}
				}).Start();
			}
			else
			{
				MessageBox.Show("Please select category");
				cbxCategory.Focus();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
		}

		private void tabPage2_Click(object sender, EventArgs e)
		{
		}

		private void btnPaste_Click(object sender, EventArgs e)
		{
			i_Click(null, null);
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			btnBatch_Click(null, null);
			Close();
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
			btnUpdate = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			txtUid = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			txtEmail = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			txtName = new System.Windows.Forms.TextBox();
			label5 = new System.Windows.Forms.Label();
			txtPhone = new System.Windows.Forms.TextBox();
			label6 = new System.Windows.Forms.Label();
			txt2fa = new System.Windows.Forms.TextBox();
			txtCookie = new System.Windows.Forms.TextBox();
			label7 = new System.Windows.Forms.Label();
			dataGridView = new System.Windows.Forms.DataGridView();
			btnBatch = new System.Windows.Forms.Button();
			progressBar1 = new System.Windows.Forms.ProgressBar();
			tabsingle = new System.Windows.Forms.TabControl();
			tabPage2 = new System.Windows.Forms.TabPage();
			label16 = new System.Windows.Forms.Label();
			txtBirthday = new System.Windows.Forms.TextBox();
			label15 = new System.Windows.Forms.Label();
			cbxDanhMuc2 = new System.Windows.Forms.ComboBox();
			label13 = new System.Windows.Forms.Label();
			txtUidLayBai = new System.Windows.Forms.TextBox();
			txtPass = new System.Windows.Forms.TextBox();
			label10 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			txtBrand = new System.Windows.Forms.TextBox();
			txtProxy = new System.Windows.Forms.TextBox();
			txtPassmail = new System.Windows.Forms.TextBox();
			label12 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			txttoken = new System.Windows.Forms.TextBox();
			tabPage1 = new System.Windows.Forms.TabPage();
			button1 = new System.Windows.Forms.Button();
			label14 = new System.Windows.Forms.Label();
			cbxCategory = new System.Windows.Forms.ComboBox();
			label11 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
			tabsingle.SuspendLayout();
			tabPage2.SuspendLayout();
			tabPage1.SuspendLayout();
			SuspendLayout();
			btnUpdate.Location = new System.Drawing.Point(278, 440);
			btnUpdate.Margin = new System.Windows.Forms.Padding(2);
			btnUpdate.Name = "btnUpdate";
			btnUpdate.Size = new System.Drawing.Size(119, 24);
			btnUpdate.TabIndex = 9;
			btnUpdate.Text = "Cập nhật";
			btnUpdate.UseVisualStyleBackColor = true;
			btnUpdate.Click += new System.EventHandler(btnUpdate_Click);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(128, 39);
			label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(26, 13);
			label1.TabIndex = 1;
			label1.Text = "UID";
			txtUid.Location = new System.Drawing.Point(166, 37);
			txtUid.Margin = new System.Windows.Forms.Padding(2);
			txtUid.Name = "txtUid";
			txtUid.Size = new System.Drawing.Size(412, 20);
			txtUid.TabIndex = 1;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(122, 97);
			label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(32, 13);
			label2.TabIndex = 1;
			label2.Text = "Email";
			txtEmail.Location = new System.Drawing.Point(166, 94);
			txtEmail.Margin = new System.Windows.Forms.Padding(2);
			txtEmail.Name = "txtEmail";
			txtEmail.Size = new System.Drawing.Size(412, 20);
			txtEmail.TabIndex = 2;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(96, 124);
			label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(58, 13);
			label3.TabIndex = 1;
			label3.Text = "Pass Email";
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(119, 341);
			label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(35, 13);
			label4.TabIndex = 1;
			label4.Text = "Name";
			label4.Visible = false;
			txtName.Location = new System.Drawing.Point(166, 339);
			txtName.Margin = new System.Windows.Forms.Padding(2);
			txtName.Name = "txtName";
			txtName.Size = new System.Drawing.Size(412, 20);
			txtName.TabIndex = 4;
			txtName.Visible = false;
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(116, 276);
			label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(38, 13);
			label5.TabIndex = 1;
			label5.Text = "Phone";
			label5.Visible = false;
			txtPhone.Location = new System.Drawing.Point(166, 274);
			txtPhone.Margin = new System.Windows.Forms.Padding(2);
			txtPhone.Name = "txtPhone";
			txtPhone.Size = new System.Drawing.Size(412, 20);
			txtPhone.TabIndex = 5;
			txtPhone.Visible = false;
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(93, 246);
			label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(61, 13);
			label6.TabIndex = 1;
			label6.Text = "Private Key";
			txt2fa.Location = new System.Drawing.Point(166, 244);
			txt2fa.Margin = new System.Windows.Forms.Padding(2);
			txt2fa.Name = "txt2fa";
			txt2fa.Size = new System.Drawing.Size(412, 20);
			txt2fa.TabIndex = 6;
			txtCookie.Location = new System.Drawing.Point(166, 209);
			txtCookie.Margin = new System.Windows.Forms.Padding(2);
			txtCookie.Name = "txtCookie";
			txtCookie.Size = new System.Drawing.Size(412, 20);
			txtCookie.TabIndex = 7;
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(109, 211);
			label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(45, 13);
			label7.TabIndex = 3;
			label7.Text = "Cookies";
			dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView.Location = new System.Drawing.Point(6, 50);
			dataGridView.Name = "dataGridView";
			dataGridView.Size = new System.Drawing.Size(643, 385);
			dataGridView.TabIndex = 5;
			dataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dataGridView_CellMouseClick);
			dataGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(dataGridView_MouseClick);
			btnBatch.Location = new System.Drawing.Point(129, 452);
			btnBatch.Margin = new System.Windows.Forms.Padding(2);
			btnBatch.Name = "btnBatch";
			btnBatch.Size = new System.Drawing.Size(119, 24);
			btnBatch.TabIndex = 6;
			btnBatch.Text = "Save";
			btnBatch.UseVisualStyleBackColor = true;
			btnBatch.Click += new System.EventHandler(btnBatch_Click);
			progressBar1.Location = new System.Drawing.Point(0, 496);
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new System.Drawing.Size(655, 15);
			progressBar1.TabIndex = 7;
			tabsingle.Controls.Add(tabPage2);
			tabsingle.Controls.Add(tabPage1);
			tabsingle.Location = new System.Drawing.Point(12, 12);
			tabsingle.Name = "tabsingle";
			tabsingle.SelectedIndex = 0;
			tabsingle.Size = new System.Drawing.Size(663, 537);
			tabsingle.TabIndex = 8;
			tabPage2.Controls.Add(label16);
			tabPage2.Controls.Add(txtBirthday);
			tabPage2.Controls.Add(label15);
			tabPage2.Controls.Add(cbxDanhMuc2);
			tabPage2.Controls.Add(label13);
			tabPage2.Controls.Add(txtUidLayBai);
			tabPage2.Controls.Add(txtPass);
			tabPage2.Controls.Add(label10);
			tabPage2.Controls.Add(label9);
			tabPage2.Controls.Add(txtBrand);
			tabPage2.Controls.Add(txtProxy);
			tabPage2.Controls.Add(label1);
			tabPage2.Controls.Add(txt2fa);
			tabPage2.Controls.Add(txtPassmail);
			tabPage2.Controls.Add(label12);
			tabPage2.Controls.Add(btnUpdate);
			tabPage2.Controls.Add(label3);
			tabPage2.Controls.Add(label6);
			tabPage2.Controls.Add(label8);
			tabPage2.Controls.Add(label7);
			tabPage2.Controls.Add(txtUid);
			tabPage2.Controls.Add(label4);
			tabPage2.Controls.Add(label5);
			tabPage2.Controls.Add(txtEmail);
			tabPage2.Controls.Add(label2);
			tabPage2.Controls.Add(txtPhone);
			tabPage2.Controls.Add(txtName);
			tabPage2.Controls.Add(txttoken);
			tabPage2.Controls.Add(txtCookie);
			tabPage2.Location = new System.Drawing.Point(4, 22);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new System.Windows.Forms.Padding(3);
			tabPage2.Size = new System.Drawing.Size(655, 511);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Information";
			tabPage2.UseVisualStyleBackColor = true;
			tabPage2.Click += new System.EventHandler(tabPage2_Click);
			label16.AutoSize = true;
			label16.Location = new System.Drawing.Point(106, 396);
			label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(45, 13);
			label16.TabIndex = 18;
			label16.Text = "Birthday";
			label16.Visible = false;
			txtBirthday.Location = new System.Drawing.Point(166, 393);
			txtBirthday.Margin = new System.Windows.Forms.Padding(2);
			txtBirthday.Name = "txtBirthday";
			txtBirthday.Size = new System.Drawing.Size(412, 20);
			txtBirthday.TabIndex = 17;
			txtBirthday.Visible = false;
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(98, 15);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(49, 13);
			label15.TabIndex = 16;
			label15.Text = "Category";
			cbxDanhMuc2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxDanhMuc2.FormattingEnabled = true;
			cbxDanhMuc2.Location = new System.Drawing.Point(166, 8);
			cbxDanhMuc2.Name = "cbxDanhMuc2";
			cbxDanhMuc2.Size = new System.Drawing.Size(300, 21);
			cbxDanhMuc2.TabIndex = 15;
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(106, 369);
			label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(48, 13);
			label13.TabIndex = 14;
			label13.Text = "Clone ID";
			label13.Visible = false;
			txtUidLayBai.Location = new System.Drawing.Point(166, 366);
			txtUidLayBai.Margin = new System.Windows.Forms.Padding(2);
			txtUidLayBai.Name = "txtUidLayBai";
			txtUidLayBai.Size = new System.Drawing.Size(412, 20);
			txtUidLayBai.TabIndex = 13;
			txtUidLayBai.Visible = false;
			txtPass.Location = new System.Drawing.Point(166, 67);
			txtPass.Margin = new System.Windows.Forms.Padding(2);
			txtPass.Name = "txtPass";
			txtPass.Size = new System.Drawing.Size(412, 20);
			txtPass.TabIndex = 12;
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(116, 184);
			label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(35, 13);
			label10.TabIndex = 11;
			label10.Text = "Brand";
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(121, 316);
			label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(33, 13);
			label9.TabIndex = 11;
			label9.Text = "Proxy";
			label9.Visible = false;
			txtBrand.Location = new System.Drawing.Point(166, 181);
			txtBrand.Margin = new System.Windows.Forms.Padding(2);
			txtBrand.Name = "txtBrand";
			txtBrand.Size = new System.Drawing.Size(412, 20);
			txtBrand.TabIndex = 10;
			txtProxy.Location = new System.Drawing.Point(166, 313);
			txtProxy.Margin = new System.Windows.Forms.Padding(2);
			txtProxy.Name = "txtProxy";
			txtProxy.Size = new System.Drawing.Size(412, 20);
			txtProxy.TabIndex = 10;
			txtProxy.Visible = false;
			txtPassmail.Location = new System.Drawing.Point(166, 124);
			txtPassmail.Margin = new System.Windows.Forms.Padding(2);
			txtPassmail.Name = "txtPassmail";
			txtPassmail.Size = new System.Drawing.Size(412, 20);
			txtPassmail.TabIndex = 3;
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(124, 67);
			label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(30, 13);
			label12.TabIndex = 1;
			label12.Text = "Pass";
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(116, 154);
			label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(38, 13);
			label8.TabIndex = 3;
			label8.Text = "Token";
			txttoken.Location = new System.Drawing.Point(166, 152);
			txttoken.Margin = new System.Windows.Forms.Padding(2);
			txttoken.Name = "txttoken";
			txttoken.Size = new System.Drawing.Size(412, 20);
			txttoken.TabIndex = 8;
			tabPage1.Controls.Add(button1);
			tabPage1.Controls.Add(dataGridView);
			tabPage1.Controls.Add(label14);
			tabPage1.Controls.Add(cbxCategory);
			tabPage1.Controls.Add(label11);
			tabPage1.Controls.Add(progressBar1);
			tabPage1.Controls.Add(btnBatch);
			tabPage1.Location = new System.Drawing.Point(4, 22);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new System.Windows.Forms.Padding(3);
			tabPage1.Size = new System.Drawing.Size(655, 511);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "Import multiple accounts";
			tabPage1.UseVisualStyleBackColor = true;
			button1.Location = new System.Drawing.Point(258, 452);
			button1.Margin = new System.Windows.Forms.Padding(2);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(119, 24);
			button1.TabIndex = 11;
			button1.Text = "Save and Close";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click_1);
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(9, 21);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(49, 13);
			label14.TabIndex = 10;
			label14.Text = "Category";
			cbxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxCategory.FormattingEnabled = true;
			cbxCategory.Location = new System.Drawing.Point(77, 17);
			cbxCategory.Name = "cbxCategory";
			cbxCategory.Size = new System.Drawing.Size(300, 21);
			cbxCategory.TabIndex = 9;
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(465, 458);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(166, 13);
			label11.TabIndex = 8;
			label11.Text = "Right into Grid to Import Accounts";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(683, 561);
			base.Controls.Add(tabsingle);
			base.Margin = new System.Windows.Forms.Padding(2);
			base.Name = "frmAccountInfo";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "frmAccountInfo";
			base.Load += new System.EventHandler(frmAccountInfo_Load);
			((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
			tabsingle.ResumeLayout(false);
			tabPage2.ResumeLayout(false);
			tabPage2.PerformLayout();
			tabPage1.ResumeLayout(false);
			tabPage1.PerformLayout();
			ResumeLayout(false);
		}
	}
}
