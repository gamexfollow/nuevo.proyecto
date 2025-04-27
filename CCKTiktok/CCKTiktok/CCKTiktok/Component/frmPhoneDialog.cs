using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CCKTiktok.DAL;
using CCKTiktok.Helper;

namespace CCKTiktok.Component
{
	public class frmPhoneDialog : Form
	{
		private static Dictionary<string, string> dicDevices = new Dictionary<string, string>();

		private IContainer components = null;

		private Label label1;

		private Button btnSave;

		private ComboBox cbxdata;

		public string DeviceId { get; set; }

		public frmPhoneDialog()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			DeviceId = cbxdata.SelectedValue.ToString();
			Close();
		}

		private void LoadDevices()
		{
			try
			{
				dicDevices.Clear();
				DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select deviceId,Name from tblDevices");
				if (dataTable == null)
				{
					return;
				}
				foreach (DataRow row in dataTable.Rows)
				{
					string key = row["deviceId"].ToString();
					string value = row["name"].ToString();
					if (!dicDevices.ContainsKey(key))
					{
						dicDevices.Add(key, value);
					}
				}
			}
			catch
			{
			}
		}

		private void frmPhoneDialog_Load(object sender, EventArgs e)
		{
			LoadDevices();
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Stt");
			dataTable.Columns.Add("Phone");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Status");
			dataTable.Columns.Add("Port");
			dataTable.Columns.Add("SystemPort");
			List<string> list = new List<string>();
			int num = 0;
			foreach (string item in listSerialDevice)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Stt"] = ++num;
				dataRow["Phone"] = item;
				dataRow["Name"] = (dicDevices.ContainsKey(item) ? dicDevices[item] : item);
				dataRow["Status"] = "Live";
				dataRow["Port"] = 4723 + num;
				dataRow["SystemPort"] = 8200 + num;
				dataTable.Rows.Add(dataRow);
				list.Add(item);
			}
			dataTable.AcceptChanges();
			DataView defaultView = dataTable.DefaultView;
			defaultView.Sort = "Name asc";
			dataTable = defaultView.ToTable();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataTable.Rows[i]["Stt"] = i + 1;
			}
			dataTable.AcceptChanges();
			cbxdata.DataSource = new BindingSource(dataTable, null);
			cbxdata.DisplayMember = "Name";
			cbxdata.ValueMember = "Phone";
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
			label1 = new System.Windows.Forms.Label();
			btnSave = new System.Windows.Forms.Button();
			cbxdata = new System.Windows.Forms.ComboBox();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			label1.Location = new System.Drawing.Point(117, 40);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(183, 20);
			label1.TabIndex = 1;
			label1.Text = "Điện thoại cần thêm nick";
			btnSave.Location = new System.Drawing.Point(163, 141);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(102, 27);
			btnSave.TabIndex = 2;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			cbxdata.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxdata.FormattingEnabled = true;
			cbxdata.Location = new System.Drawing.Point(25, 92);
			cbxdata.Name = "cbxdata";
			cbxdata.Size = new System.Drawing.Size(383, 21);
			cbxdata.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(433, 182);
			base.Controls.Add(btnSave);
			base.Controls.Add(label1);
			base.Controls.Add(cbxdata);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmPhoneDialog";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Phone Dialog";
			base.Load += new System.EventHandler(frmPhoneDialog_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
