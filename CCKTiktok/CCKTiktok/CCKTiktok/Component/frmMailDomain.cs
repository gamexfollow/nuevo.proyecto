using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmMailDomain : Form
	{
		private IContainer components = null;

		private Button benDel;

		private DataGridView dataGridView;

		private Button btnAdd;

		private TextBox email_account;

		private TextBox email_domain;

		private TextBox email_pass;

		private Label label1;

		private Label label6;

		private Label label7;

		private Button btncheck;

		private LinkLabel linkLabel1;

		private LinkLabel linkLabel2;

		private LinkLabel linkLabel3;

		private Label label2;

		public frmMailDomain()
		{
			InitializeComponent();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			try
			{
				EmailSource emailSource = new EmailSource
				{
					domain = email_domain.Text.Trim(),
					pass = email_pass.Text.Trim(),
					gmail = email_account.Text.Trim()
				};
				bool flag = EmailUtils.CheckConfig(emailSource.gmail, emailSource.pass);
				if (emailSource.domain != "" && flag)
				{
					emailSource.Insert();
					dataGridView.DataSource = emailSource.GetAll();
				}
				else
				{
					MessageBox.Show("Cấu hình tên miền chưa đúng, kiểm tra lại Pop3, User hoặc Pass");
				}
			}
			catch
			{
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
		}

		private void frmMailDomain_Load(object sender, EventArgs e)
		{
			EmailSource emailSource = new EmailSource();
			dataGridView.DataSource = emailSource.GetAll();
		}

		private void benDel_Click(object sender, EventArgs e)
		{
			EmailSource emailSource = new EmailSource();
			if (dataGridView.SelectedRows.Count != 0)
			{
				if (MessageBox.Show("Do you want delete these item?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				foreach (DataGridViewRow selectedRow in dataGridView.SelectedRows)
				{
					emailSource.Delete(selectedRow.Cells["domain"].Value.ToString());
				}
				dataGridView.DataSource = emailSource.GetAll();
			}
			else
			{
				MessageBox.Show("Chưa chọn domain xóa");
			}
		}

		private void btncheck_Click(object sender, EventArgs e)
		{
			string text = email_account.Text.Trim();
			string pass = email_pass.Text.Trim();
			email_domain.Text.Trim();
			MessageBox.Show(EmailUtils.CheckConfig(text, pass) ? ("Cấu hình Email " + text + " đã thành công") : ("Cấu hình Email " + text + " chưa chuẩn"));
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://bit.ly/cck-yandex");
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://bit.ly/cck-gmail-app");
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://bit.ly/cck-verify-domain");
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
			benDel = new System.Windows.Forms.Button();
			dataGridView = new System.Windows.Forms.DataGridView();
			btnAdd = new System.Windows.Forms.Button();
			email_account = new System.Windows.Forms.TextBox();
			email_domain = new System.Windows.Forms.TextBox();
			email_pass = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			btncheck = new System.Windows.Forms.Button();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			linkLabel3 = new System.Windows.Forms.LinkLabel();
			label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
			SuspendLayout();
			benDel.Location = new System.Drawing.Point(180, 131);
			benDel.Margin = new System.Windows.Forms.Padding(2);
			benDel.Name = "benDel";
			benDel.Size = new System.Drawing.Size(83, 25);
			benDel.TabIndex = 70;
			benDel.Text = "Del";
			benDel.UseVisualStyleBackColor = true;
			benDel.Click += new System.EventHandler(benDel_Click);
			dataGridView.AllowUserToAddRows = false;
			dataGridView.AllowUserToDeleteRows = false;
			dataGridView.AllowUserToResizeColumns = false;
			dataGridView.AllowUserToResizeRows = false;
			dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView.Location = new System.Drawing.Point(32, 177);
			dataGridView.MultiSelect = false;
			dataGridView.Name = "dataGridView";
			dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridView.Size = new System.Drawing.Size(453, 304);
			dataGridView.TabIndex = 69;
			btnAdd.Location = new System.Drawing.Point(93, 131);
			btnAdd.Margin = new System.Windows.Forms.Padding(2);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new System.Drawing.Size(83, 25);
			btnAdd.TabIndex = 68;
			btnAdd.Text = "Add Account";
			btnAdd.UseVisualStyleBackColor = true;
			btnAdd.Click += new System.EventHandler(btnAdd_Click);
			email_account.Location = new System.Drawing.Point(93, 36);
			email_account.Margin = new System.Windows.Forms.Padding(2);
			email_account.Name = "email_account";
			email_account.Size = new System.Drawing.Size(166, 20);
			email_account.TabIndex = 67;
			email_domain.Location = new System.Drawing.Point(93, 97);
			email_domain.Margin = new System.Windows.Forms.Padding(2);
			email_domain.Name = "email_domain";
			email_domain.Size = new System.Drawing.Size(166, 20);
			email_domain.TabIndex = 66;
			email_pass.Location = new System.Drawing.Point(93, 67);
			email_pass.Margin = new System.Windows.Forms.Padding(2);
			email_pass.Name = "email_pass";
			email_pass.PasswordChar = '*';
			email_pass.Size = new System.Drawing.Size(166, 20);
			email_pass.TabIndex = 65;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(40, 98);
			label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(43, 13);
			label1.TabIndex = 64;
			label1.Text = "Domain";
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(40, 71);
			label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(30, 13);
			label6.TabIndex = 63;
			label6.Text = "Pass";
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(40, 42);
			label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(32, 13);
			label7.TabIndex = 62;
			label7.Text = "Email";
			btncheck.Location = new System.Drawing.Point(267, 131);
			btncheck.Margin = new System.Windows.Forms.Padding(2);
			btncheck.Name = "btncheck";
			btncheck.Size = new System.Drawing.Size(83, 25);
			btncheck.TabIndex = 71;
			btncheck.Text = "Check Config";
			btncheck.UseVisualStyleBackColor = true;
			btncheck.Click += new System.EventHandler(btncheck_Click);
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(295, 57);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(93, 13);
			linkLabel1.TabIndex = 72;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "Tạo Email Domain";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			linkLabel2.AutoSize = true;
			linkLabel2.Location = new System.Drawing.Point(295, 80);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(78, 13);
			linkLabel2.TabIndex = 72;
			linkLabel2.TabStop = true;
			linkLabel2.Text = "Cấu hình Gmail";
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
			linkLabel3.AutoSize = true;
			linkLabel3.Location = new System.Drawing.Point(296, 102);
			linkLabel3.Name = "linkLabel3";
			linkLabel3.Size = new System.Drawing.Size(70, 13);
			linkLabel3.TabIndex = 72;
			linkLabel3.TabStop = true;
			linkLabel3.Text = "Verify domain";
			linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel3_LinkClicked);
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(296, 36);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(88, 13);
			label2.TabIndex = 73;
			label2.Text = "Video hướng dẫn";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(519, 513);
			base.Controls.Add(label2);
			base.Controls.Add(linkLabel3);
			base.Controls.Add(linkLabel2);
			base.Controls.Add(linkLabel1);
			base.Controls.Add(btncheck);
			base.Controls.Add(benDel);
			base.Controls.Add(dataGridView);
			base.Controls.Add(btnAdd);
			base.Controls.Add(email_account);
			base.Controls.Add(email_domain);
			base.Controls.Add(email_pass);
			base.Controls.Add(label1);
			base.Controls.Add(label6);
			base.Controls.Add(label7);
			base.Name = "frmMailDomain";
			Text = "frmMailDomain";
			base.Load += new System.EventHandler(frmMailDomain_Load);
			((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
