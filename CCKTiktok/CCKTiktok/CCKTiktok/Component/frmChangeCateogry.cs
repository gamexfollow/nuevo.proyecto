using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCKTiktok.DAL;

namespace CCKTiktok.Component
{
	public class frmChangeCateogry : Form
	{
		private IContainer components = null;

		private ListBox cbxCategory;

		private Button btnOk;

		private ProgressBar progressBar1;

		private Button btnAdd;

		private List<string> AccountList { get; set; }

		public frmChangeCateogry(List<string> AccountList)
		{
			InitializeComponent();
			this.AccountList = AccountList;
		}

		private void frmChangeCateogry_Load(object sender, EventArgs e)
		{
			DataTable dataSource = new SQLiteUtils().ExecuteQuery("Select * from DanhMuc");
			cbxCategory.DataSource = new BindingSource(dataSource, null);
			cbxCategory.DisplayMember = "tendanhmuc";
			cbxCategory.ValueMember = "id_danhmuc";
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				string cat = cbxCategory.SelectedValue.ToString();
				SQLiteUtils sQLiteUtils = new SQLiteUtils();
				progressBar1.Minimum = 0;
				progressBar1.Value = 0;
				progressBar1.Maximum = AccountList.Count;
				List<string> list = new List<string>();
				for (int i = 0; i < AccountList.Count; i++)
				{
					progressBar1.Value++;
					list.Add(AccountList[i]);
					if (i == AccountList.Count - 1 || list.Count >= 20)
					{
						sQLiteUtils.UpdateCategory(list, cat);
						list.Clear();
					}
				}
				progressBar1.Value = progressBar1.Maximum;
				Close();
			}).Start();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			frmCategory frmCategory2 = new frmCategory();
			frmCategory2.ShowDialog();
			frmChangeCateogry_Load(null, null);
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
			cbxCategory = new System.Windows.Forms.ListBox();
			btnOk = new System.Windows.Forms.Button();
			progressBar1 = new System.Windows.Forms.ProgressBar();
			btnAdd = new System.Windows.Forms.Button();
			SuspendLayout();
			cbxCategory.FormattingEnabled = true;
			cbxCategory.Location = new System.Drawing.Point(26, 29);
			cbxCategory.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			cbxCategory.Name = "cbxCategory";
			cbxCategory.Size = new System.Drawing.Size(335, 147);
			cbxCategory.TabIndex = 0;
			btnOk.Location = new System.Drawing.Point(65, 190);
			btnOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			btnOk.Name = "btnOk";
			btnOk.Size = new System.Drawing.Size(118, 34);
			btnOk.TabIndex = 1;
			btnOk.Text = "Ok";
			btnOk.UseVisualStyleBackColor = true;
			btnOk.Click += new System.EventHandler(btnOk_Click);
			progressBar1.Location = new System.Drawing.Point(-1, 237);
			progressBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new System.Drawing.Size(395, 21);
			progressBar1.TabIndex = 2;
			btnAdd.Location = new System.Drawing.Point(187, 190);
			btnAdd.Margin = new System.Windows.Forms.Padding(2);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new System.Drawing.Size(118, 34);
			btnAdd.TabIndex = 3;
			btnAdd.Text = "Add Category";
			btnAdd.UseVisualStyleBackColor = true;
			btnAdd.Click += new System.EventHandler(btnAdd_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(393, 257);
			base.Controls.Add(btnAdd);
			base.Controls.Add(progressBar1);
			base.Controls.Add(btnOk);
			base.Controls.Add(cbxCategory);
			base.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			base.Name = "frmChangeCateogry";
			Text = "Chuyển danh mục";
			base.Load += new System.EventHandler(frmChangeCateogry_Load);
			ResumeLayout(false);
		}
	}
}
