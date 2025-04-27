using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CCKTiktok.DAL;

namespace CCKTiktok.Component
{
	public class frmCategory : Form
	{
		private List<string> lst = new List<string>();

		private IContainer components = null;

		private Label lblMessage;

		private Button button9;

		private Button button8;

		private Button button7;

		private TextBox txtName;

		private ListBox listBox1;

		public bool IsAdd { get; set; }

		public frmCategory()
		{
			InitializeComponent();
			Control.CheckForIllegalCrossThreadCalls = false;
			IsAdd = false;
		}

		public frmCategory(List<string> lst)
		{
			InitializeComponent();
			Control.CheckForIllegalCrossThreadCalls = false;
			IsAdd = false;
			this.lst = lst;
		}

		private void button1_Click(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
		}

		private void button8_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(txtName.Text))
			{
				new SQLiteUtils().CreateFolder(txtName.Text);
				txtName.Text = "";
				frmCategory_Load(null, null);
				lblMessage.Text = "Thêm Xong";
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			int num = Convert.ToInt32(listBox1.SelectedValue.ToString());
			string arg = txtName.Text;
			new SQLiteUtils().ExecuteQuery($"Update DanhMuc set tendanhmuc='{arg}' where id_danhmuc={num}");
			txtName.Text = "";
			frmCategory_Load(null, null);
			lblMessage.Text = "Saved";
		}

		private void button9_Click(object sender, EventArgs e)
		{
			object selectedValue = listBox1.SelectedValue;
			DataTable dataTable = new SQLiteUtils().ExecuteQuery($"Select count(*) from Account  where id_danhmuc={selectedValue}");
			if (dataTable != null && Convert.ToInt32(dataTable.Rows[0][0]) > 0)
			{
				lblMessage.Text = "Mục này vẫn còn Tài khoản";
				return;
			}
			new SQLiteUtils().ExecuteQuery($"Delete from DanhMuc  where id_danhmuc={selectedValue}");
			lblMessage.Text = "Successfully completed";
			BindCategory();
		}

		private void button4_Click(object sender, EventArgs e)
		{
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtName.Text = listBox1.Text;
		}

		private void BindCategory()
		{
			DataTable dataSource = new SQLiteUtils().ExecuteQuery("Select * from danhmuc order by tendanhmuc asc");
			listBox1.DataSource = dataSource;
			listBox1.DisplayMember = "tendanhmuc";
			listBox1.ValueMember = "id_danhmuc";
		}

		private void txtUid_TextChanged(object sender, EventArgs e)
		{
		}

		private void frmCategory_Load(object sender, EventArgs e)
		{
			BindCategory();
			if (IsAdd)
			{
				txtName.Text = "";
				txtName.Focus();
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
			lblMessage = new System.Windows.Forms.Label();
			button9 = new System.Windows.Forms.Button();
			button8 = new System.Windows.Forms.Button();
			button7 = new System.Windows.Forms.Button();
			txtName = new System.Windows.Forms.TextBox();
			listBox1 = new System.Windows.Forms.ListBox();
			SuspendLayout();
			lblMessage.AutoSize = true;
			lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 16f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			lblMessage.ForeColor = System.Drawing.Color.Red;
			lblMessage.Location = new System.Drawing.Point(57, 484);
			lblMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			lblMessage.Name = "lblMessage";
			lblMessage.Size = new System.Drawing.Size(0, 26);
			lblMessage.TabIndex = 26;
			button9.Location = new System.Drawing.Point(205, 360);
			button9.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			button9.Name = "button9";
			button9.Size = new System.Drawing.Size(73, 40);
			button9.TabIndex = 25;
			button9.Text = "Delete";
			button9.UseVisualStyleBackColor = true;
			button9.Click += new System.EventHandler(button9_Click);
			button8.Location = new System.Drawing.Point(22, 360);
			button8.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			button8.Name = "button8";
			button8.Size = new System.Drawing.Size(73, 40);
			button8.TabIndex = 24;
			button8.Text = "Insert";
			button8.UseVisualStyleBackColor = true;
			button8.Click += new System.EventHandler(button8_Click);
			button7.Location = new System.Drawing.Point(105, 360);
			button7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			button7.Name = "button7";
			button7.Size = new System.Drawing.Size(91, 40);
			button7.TabIndex = 23;
			button7.Text = "Update";
			button7.UseVisualStyleBackColor = true;
			button7.Click += new System.EventHandler(button7_Click);
			txtName.Location = new System.Drawing.Point(22, 326);
			txtName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			txtName.Name = "txtName";
			txtName.Size = new System.Drawing.Size(257, 20);
			txtName.TabIndex = 22;
			listBox1.FormattingEnabled = true;
			listBox1.Location = new System.Drawing.Point(22, 55);
			listBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			listBox1.Name = "listBox1";
			listBox1.Size = new System.Drawing.Size(257, 264);
			listBox1.TabIndex = 21;
			listBox1.SelectedIndexChanged += new System.EventHandler(listBox1_SelectedIndexChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(307, 421);
			base.Controls.Add(lblMessage);
			base.Controls.Add(button9);
			base.Controls.Add(button8);
			base.Controls.Add(button7);
			base.Controls.Add(txtName);
			base.Controls.Add(listBox1);
			base.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			base.Name = "frmCategory";
			Text = "Category management";
			base.Load += new System.EventHandler(frmCategory_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
