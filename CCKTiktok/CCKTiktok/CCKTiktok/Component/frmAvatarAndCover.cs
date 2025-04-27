using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmAvatarAndCover : Form
	{
		private IContainer components = null;

		private Label label1;

		private TextBox txtAvatar;

		private TextBox txtCover;

		private Label label2;

		private Label label3;

		private Button btnSave;

		private Button btnSelect;

		private Button button1;

		private CheckBox cbxAvatar;

		private CheckBox checkBox1;

		private GroupBox groupBox1;

		private GroupBox groupBox2;

		public frmAvatarAndCover()
		{
			InitializeComponent();
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			DialogResult dialogResult = folderBrowserDialog.ShowDialog();
			if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
			{
				txtAvatar.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			DialogResult dialogResult = folderBrowserDialog.ShowDialog();
			if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
			{
				txtCover.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void frmAvatarAndCover_Load(object sender, EventArgs e)
		{
		}

		private void btnSave_Click(object sender, EventArgs e)
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
			label1 = new System.Windows.Forms.Label();
			txtAvatar = new System.Windows.Forms.TextBox();
			txtCover = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			btnSave = new System.Windows.Forms.Button();
			btnSelect = new System.Windows.Forms.Button();
			button1 = new System.Windows.Forms.Button();
			cbxAvatar = new System.Windows.Forms.CheckBox();
			checkBox1 = new System.Windows.Forms.CheckBox();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			label1.Location = new System.Drawing.Point(173, 29);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(251, 20);
			label1.TabIndex = 0;
			label1.Text = "Chọn thư mục chứa Avatar - Cover";
			txtAvatar.Location = new System.Drawing.Point(123, 56);
			txtAvatar.Name = "txtAvatar";
			txtAvatar.Size = new System.Drawing.Size(275, 20);
			txtAvatar.TabIndex = 1;
			txtCover.Location = new System.Drawing.Point(122, 58);
			txtCover.Name = "txtCover";
			txtCover.Size = new System.Drawing.Size(275, 20);
			txtCover.TabIndex = 2;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(11, 59);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(104, 13);
			label2.TabIndex = 3;
			label2.Text = "Thư mục ảnh Avatar";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(15, 61);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(101, 13);
			label3.TabIndex = 4;
			label3.Text = "Thư mục ảnh Cover";
			btnSave.Location = new System.Drawing.Point(236, 311);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(75, 23);
			btnSave.TabIndex = 5;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			btnSelect.Location = new System.Drawing.Point(404, 54);
			btnSelect.Name = "btnSelect";
			btnSelect.Size = new System.Drawing.Size(103, 23);
			btnSelect.TabIndex = 6;
			btnSelect.Text = "Chọn thư mục";
			btnSelect.UseVisualStyleBackColor = true;
			btnSelect.Click += new System.EventHandler(btnSelect_Click);
			button1.Location = new System.Drawing.Point(403, 57);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(103, 23);
			button1.TabIndex = 7;
			button1.Text = "Chọn thư mục";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);
			cbxAvatar.AutoSize = true;
			cbxAvatar.Location = new System.Drawing.Point(123, 24);
			cbxAvatar.Name = "cbxAvatar";
			cbxAvatar.Size = new System.Drawing.Size(97, 17);
			cbxAvatar.TabIndex = 8;
			cbxAvatar.Text = "Đổi ảnh Avatar";
			cbxAvatar.UseVisualStyleBackColor = true;
			checkBox1.AutoSize = true;
			checkBox1.Location = new System.Drawing.Point(122, 21);
			checkBox1.Name = "checkBox1";
			checkBox1.Size = new System.Drawing.Size(94, 17);
			checkBox1.TabIndex = 9;
			checkBox1.Text = "Đổi ảnh Cover";
			checkBox1.UseVisualStyleBackColor = true;
			groupBox1.Controls.Add(btnSelect);
			groupBox1.Controls.Add(txtAvatar);
			groupBox1.Controls.Add(cbxAvatar);
			groupBox1.Controls.Add(label2);
			groupBox1.Location = new System.Drawing.Point(37, 65);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(526, 111);
			groupBox1.TabIndex = 10;
			groupBox1.TabStop = false;
			groupBox1.Text = "Avatar";
			groupBox2.Controls.Add(txtCover);
			groupBox2.Controls.Add(label3);
			groupBox2.Controls.Add(checkBox1);
			groupBox2.Controls.Add(button1);
			groupBox2.Location = new System.Drawing.Point(36, 182);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(526, 105);
			groupBox2.TabIndex = 11;
			groupBox2.TabStop = false;
			groupBox2.Text = "Cover";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(608, 375);
			base.Controls.Add(groupBox2);
			base.Controls.Add(groupBox1);
			base.Controls.Add(btnSave);
			base.Controls.Add(label1);
			base.Name = "frmAvatarAndCover";
			Text = "Avatar And Cover";
			base.Load += new System.EventHandler(frmAvatarAndCover_Load);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
