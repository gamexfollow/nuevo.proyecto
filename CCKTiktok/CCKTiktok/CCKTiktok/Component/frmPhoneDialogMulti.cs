using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Helper;

namespace CCKTiktok.Component
{
	public class frmPhoneDialogMulti : Form
	{
		private static Dictionary<string, string> dicDevices = new Dictionary<string, string>();

		private IContainer components = null;

		private Button btnSave;

		private Label label1;

		private CheckedListBox cbxCategory;

		private CheckBox cbxSelectAll;

		public List<string> DeviceId { get; set; }

		public frmPhoneDialogMulti()
		{
			InitializeComponent();
			DeviceId = new List<string>();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			DeviceId.Clear();
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				if (cbxCategory.GetItemChecked(i))
				{
					string item = ((DeviceEntity)cbxCategory.Items[i]).DeviceId.ToString();
					DeviceId.Add(item);
				}
			}
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

		private void frmPhoneDialogMulti_Load(object sender, EventArgs e)
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
			cbxCategory.Items.Clear();
			cbxCategory.DisplayMember = "Name";
			cbxCategory.ValueMember = "DeviceId";
			foreach (DataRow row in dataTable.Rows)
			{
				cbxCategory.Items.Add(new DeviceEntity
				{
					DeviceId = row["Phone"].ToString(),
					Name = row["Name"].ToString()
				});
			}
		}

		private void cbxSelectAll_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				cbxCategory.SetItemChecked(i, cbxSelectAll.Checked);
			}
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
			btnSave = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			cbxCategory = new System.Windows.Forms.CheckedListBox();
			cbxSelectAll = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			btnSave.Location = new System.Drawing.Point(244, 372);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(102, 27);
			btnSave.TabIndex = 5;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			label1.Location = new System.Drawing.Point(207, 49);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(183, 20);
			label1.TabIndex = 4;
			label1.Text = "Điện thoại cần thêm nick";
			cbxCategory.CheckOnClick = true;
			cbxCategory.FormattingEnabled = true;
			cbxCategory.Location = new System.Drawing.Point(137, 122);
			cbxCategory.Margin = new System.Windows.Forms.Padding(2);
			cbxCategory.Name = "cbxCategory";
			cbxCategory.Size = new System.Drawing.Size(339, 229);
			cbxCategory.TabIndex = 13;
			cbxSelectAll.AutoSize = true;
			cbxSelectAll.Location = new System.Drawing.Point(140, 98);
			cbxSelectAll.Name = "cbxSelectAll";
			cbxSelectAll.Size = new System.Drawing.Size(81, 17);
			cbxSelectAll.TabIndex = 14;
			cbxSelectAll.Text = "Chọn tất cả";
			cbxSelectAll.UseVisualStyleBackColor = true;
			cbxSelectAll.CheckedChanged += new System.EventHandler(cbxSelectAll_CheckedChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(622, 450);
			base.Controls.Add(cbxSelectAll);
			base.Controls.Add(cbxCategory);
			base.Controls.Add(btnSave);
			base.Controls.Add(label1);
			base.Name = "frmPhoneDialogMulti";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "frmPhoneDialogMulti";
			base.Load += new System.EventHandler(frmPhoneDialogMulti_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
