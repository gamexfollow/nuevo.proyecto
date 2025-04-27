using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Helper;
using CCKTiktok.Properties;
using MetroFramework.Controls;

namespace CCKTiktok.Component
{
	public class tdsConfig : Form
	{
		private static SQLiteUtils sql = new SQLiteUtils();

		private IContainer components = null;

		private TextBox txtAccount;

		private Button btnSave;

		private Label label1;

		private DataGridView dataGridView1;

		private Label label2;

		private TextBox txtDeice;

		private Label label3;

		private Label lblApiCount;

		private Label lblDeviceCount;

		private Button button1;

		private MetroContextMenu metroContextMenu;

		private ToolStripMenuItem deleteToolStripMenuItem;

		private ToolStripMenuItem editToolStripMenuItem;

		private Button btnXu;

		private Label label4;

		public tdsConfig()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			List<string> list = txtAccount.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			if (list != null && list.Count > 0)
			{
				foreach (string item in list)
				{
					string[] array = item.Split("|".ToCharArray());
					if (array.Length >= 3)
					{
						sql.ExecuteQuery(string.Format("Insert Into TDSConfig(username, password, token, device_id, proxy) values ('{0}','{1}','{2}','{3}','{4}')", array[0].Trim(), array[1].Trim(), array[2].Trim(), (array.Length > 3) ? array[3].Trim() : "", (array.Length > 4) ? array[4].Trim() : ""));
					}
				}
			}
			BindData();
		}

		private void BindData()
		{
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			txtDeice.Text = string.Join(Environment.NewLine, listSerialDevice);
			DataTable dataTable = new SQLiteUtils().ExecuteQuery("select * from TDSConfig");
			if (dataTable != null && !dataTable.Columns.Contains("Xu"))
			{
				sql.ExecuteQuery("ALTER TABLE TDSConfig ADD COLUMN Xu int default 0");
				sql.ExecuteQuery("ALTER TABLE TDSConfig ADD COLUMN LastUpdate text");
			}
			dataGridView1.DataSource = dataTable;
			dataGridView1.Columns["Xu"].DefaultCellStyle.Format = "#,###";
			if (dataGridView1.Columns.Count > 0)
			{
				dataGridView1.Columns[0].ReadOnly = true;
			}
			dataGridView1.CellFormatting += dataGridView1_CellFormatting;
		}

		private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (dataGridView1.Columns[e.ColumnIndex].Name == "Xu" && e.Value != null && int.TryParse(e.Value.ToString(), out var result))
			{
				if (result != 0)
				{
					e.Value = result.ToString("#,###");
				}
				else
				{
					e.Value = "0";
				}
				e.FormattingApplied = true;
				e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			}
		}

		private void tdsConfig_Load(object sender, EventArgs e)
		{
			sql.CheckTableExists("TDSConfig", "CREATE TABLE \"TDSConfig\"(\"username\" TEXT, \"password\" TEXT, \"token\" TEXT, \"device_id\" TEXT, \"Xu\" int default 0, \"LastUpdate\" text, \"Proxy\" text);");
			BindData();
		}

		private void txtDeice_TextChanged(object sender, EventArgs e)
		{
			lblDeviceCount.Text = txtDeice.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length.ToString();
		}

		private void txtAccount_TextChanged(object sender, EventArgs e)
		{
			lblApiCount.Text = txtAccount.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length.ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string[] array = txtDeice.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			TDSConfiguration tDSConfiguration = new TDSConfiguration();
			string[] array2 = array;
			foreach (string text in array2)
			{
				TDSEntity tDSEntity = tDSConfiguration.CreateAccount(new TDSEntity
				{
					password = text.Trim(),
					user_name = text.Trim(),
					token = "",
					proxy = ""
				});
				if (tDSEntity != null && tDSEntity.IsSuccess)
				{
					sql.ExecuteQuery($"Insert Into TDSConfig(username, password, token, device_id, proxy) values ('{tDSEntity.user_name}','{tDSEntity.password.Trim()}','{tDSEntity.token.Trim()}','{text.ToString()}','{tDSEntity.proxy}')");
				}
				else if (tDSEntity.IsSuccess)
				{
				}
			}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				SQLiteUtils sQLiteUtils = new SQLiteUtils();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["username"].Value != null)
					{
						sQLiteUtils.ExecuteQuery(string.Format("Delete from TDSConfig where username='{0}'", selectedRow.Cells["username"].Value));
					}
				}
				sQLiteUtils = null;
				BindData();
			}).Start();
		}

		private void editToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private async void btnXu_Click(object sender, EventArgs e)
		{
			btnXu.Enabled = false;
			new List<Task>();
			SQLiteUtils sqla = new SQLiteUtils();
			sqla.OpenConnection();
			DataGridViewRowCollection rows = dataGridView1.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				DataGridViewRow row = rows[i];
				try
				{
					if (row.Cells["token"].Value == null)
					{
						continue;
					}
					string token = row.Cells["token"].Value.ToString();
					if (!(token != ""))
					{
						continue;
					}
					try
					{
						string xu = GetXu(token);
						if (xu == "")
						{
							xu = "0";
						}
						row.Cells["Xu"].Value = Utils.Convert2Int(xu);
						sqla.BatchUpdateQuery(string.Format("Update TDSConfig set Xu='{0}', LastUpdate='{1}' where token='{2}'", xu, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), row.Cells["token"].Value));
					}
					catch (Exception)
					{
					}
				}
				catch
				{
				}
			}
			MessageBox.Show("Kiểm tra thành công");
			sqla.CloseConnection();
			btnXu.Enabled = true;
		}

		private string GetXu(string token)
		{
			try
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				string address = "https://traodoisub.com/api/?fields=profile&access_token=" + token;
				using WebClient webClient = new WebClient();
				string input = webClient.DownloadString(address);
				dynamic val = javaScriptSerializer.DeserializeObject(input);
				return val["data"]["xu"].ToString();
			}
			catch (Exception)
			{
			}
			return "";
		}

		private void btnSaveUpdate_Click(object sender, EventArgs e)
		{
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
			txtAccount = new System.Windows.Forms.TextBox();
			btnSave = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			dataGridView1 = new System.Windows.Forms.DataGridView();
			metroContextMenu = new MetroFramework.Controls.MetroContextMenu(components);
			deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			label2 = new System.Windows.Forms.Label();
			txtDeice = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			lblApiCount = new System.Windows.Forms.Label();
			lblDeviceCount = new System.Windows.Forms.Label();
			button1 = new System.Windows.Forms.Button();
			btnXu = new System.Windows.Forms.Button();
			label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			metroContextMenu.SuspendLayout();
			SuspendLayout();
			txtAccount.Location = new System.Drawing.Point(52, 37);
			txtAccount.MaxLength = 32767000;
			txtAccount.Multiline = true;
			txtAccount.Name = "txtAccount";
			txtAccount.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtAccount.Size = new System.Drawing.Size(443, 117);
			txtAccount.TabIndex = 0;
			txtAccount.TextChanged += new System.EventHandler(txtAccount_TextChanged);
			btnSave.Location = new System.Drawing.Point(475, 171);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(75, 23);
			btnSave.TabIndex = 1;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(50, 16);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(104, 13);
			label1.TabIndex = 3;
			label1.Text = "Danh sách API TDS";
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.ContextMenuStrip = metroContextMenu;
			dataGridView1.Location = new System.Drawing.Point(53, 209);
			dataGridView1.Name = "dataGridView1";
			dataGridView1.ReadOnly = true;
			dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridView1.Size = new System.Drawing.Size(985, 342);
			dataGridView1.TabIndex = 4;
			metroContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { deleteToolStripMenuItem, editToolStripMenuItem });
			metroContextMenu.Name = "metroContextMenu";
			metroContextMenu.Size = new System.Drawing.Size(108, 48);
			deleteToolStripMenuItem.Image = CCKTiktok.Properties.Resources.delete_25px;
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			deleteToolStripMenuItem.Text = "Delete";
			deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
			editToolStripMenuItem.Image = CCKTiktok.Properties.Resources.checked_checkbox_25px;
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			editToolStripMenuItem.Text = "Edit";
			editToolStripMenuItem.Click += new System.EventHandler(editToolStripMenuItem_Click);
			label2.AutoSize = true;
			label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			label2.ForeColor = System.Drawing.Color.Red;
			label2.Location = new System.Drawing.Point(173, 16);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(267, 13);
			label2.TabIndex = 3;
			label2.Text = "Username | Password | Api | Device ID | Proxy";
			txtDeice.Location = new System.Drawing.Point(528, 37);
			txtDeice.MaxLength = 32767000;
			txtDeice.Multiline = true;
			txtDeice.Name = "txtDeice";
			txtDeice.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtDeice.Size = new System.Drawing.Size(510, 117);
			txtDeice.TabIndex = 6;
			txtDeice.TextChanged += new System.EventHandler(txtDeice_TextChanged);
			label3.AutoSize = true;
			label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			label3.ForeColor = System.Drawing.Color.Red;
			label3.Location = new System.Drawing.Point(722, 15);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(111, 13);
			label3.TabIndex = 7;
			label3.Text = "Danh sách thiết bị";
			lblApiCount.AutoSize = true;
			lblApiCount.Location = new System.Drawing.Point(439, 16);
			lblApiCount.Name = "lblApiCount";
			lblApiCount.Size = new System.Drawing.Size(13, 13);
			lblApiCount.TabIndex = 8;
			lblApiCount.Text = "0";
			lblDeviceCount.AutoSize = true;
			lblDeviceCount.Location = new System.Drawing.Point(1000, 15);
			lblDeviceCount.Name = "lblDeviceCount";
			lblDeviceCount.Size = new System.Drawing.Size(13, 13);
			lblDeviceCount.TabIndex = 9;
			lblDeviceCount.Text = "0";
			button1.Location = new System.Drawing.Point(623, 171);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(313, 23);
			button1.TabIndex = 10;
			button1.Text = "Tạo ngẫu nhiên tài khoản theo phone";
			button1.UseVisualStyleBackColor = true;
			button1.Visible = false;
			button1.Click += new System.EventHandler(button1_Click);
			btnXu.Location = new System.Drawing.Point(432, 576);
			btnXu.Name = "btnXu";
			btnXu.Size = new System.Drawing.Size(185, 23);
			btnXu.TabIndex = 5;
			btnXu.Text = "Cập nhật thông tin XU";
			btnXu.UseVisualStyleBackColor = true;
			btnXu.Click += new System.EventHandler(btnXu_Click);
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			label4.ForeColor = System.Drawing.Color.Blue;
			label4.Location = new System.Drawing.Point(50, 176);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(292, 13);
			label4.TabIndex = 7;
			label4.Text = "Do TDS chặn nhiều IP truy cập nên anh em cần dùng Proxy";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1092, 672);
			base.Controls.Add(button1);
			base.Controls.Add(lblDeviceCount);
			base.Controls.Add(lblApiCount);
			base.Controls.Add(label4);
			base.Controls.Add(label3);
			base.Controls.Add(txtDeice);
			base.Controls.Add(btnXu);
			base.Controls.Add(dataGridView1);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(btnSave);
			base.Controls.Add(txtAccount);
			base.Name = "tdsConfig";
			Text = "Cấu hình trao đổi sub";
			base.Load += new System.EventHandler(tdsConfig_Load);
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			metroContextMenu.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
