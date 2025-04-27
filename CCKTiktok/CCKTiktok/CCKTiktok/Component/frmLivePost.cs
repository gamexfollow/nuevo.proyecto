using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmLivePost : Form
	{
		private IContainer components = null;

		private CheckBox checkBox1;

		private Button btnSave;

		private Label label6;

		private TextBox txtComment;

		private Label label3;

		private TextBox txtLink;

		private NumericUpDown nudTo;

		private Label label2;

		private NumericUpDown nudFrom;

		private Label label1;

		private CheckBox checkBox2;

		public frmLivePost()
		{
			InitializeComponent();
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
			checkBox1 = new System.Windows.Forms.CheckBox();
			btnSave = new System.Windows.Forms.Button();
			label6 = new System.Windows.Forms.Label();
			txtComment = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			txtLink = new System.Windows.Forms.TextBox();
			nudTo = new System.Windows.Forms.NumericUpDown();
			label2 = new System.Windows.Forms.Label();
			nudFrom = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			checkBox2 = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)nudTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFrom).BeginInit();
			SuspendLayout();
			checkBox1.AutoSize = true;
			checkBox1.Location = new System.Drawing.Point(364, 120);
			checkBox1.Name = "checkBox1";
			checkBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			checkBox1.Size = new System.Drawing.Size(64, 17);
			checkBox1.TabIndex = 39;
			checkBox1.Text = "Thả tym";
			checkBox1.UseVisualStyleBackColor = true;
			btnSave.Location = new System.Drawing.Point(175, 392);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(98, 43);
			btnSave.TabIndex = 33;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(73, 147);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(96, 13);
			label6.TabIndex = 32;
			label6.Text = "Nội dung comment";
			txtComment.Location = new System.Drawing.Point(76, 175);
			txtComment.Multiline = true;
			txtComment.Name = "txtComment";
			txtComment.Size = new System.Drawing.Size(352, 201);
			txtComment.TabIndex = 31;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(42, 50);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(56, 13);
			label3.TabIndex = 27;
			label3.Text = "Link video";
			txtLink.Location = new System.Drawing.Point(102, 47);
			txtLink.Name = "txtLink";
			txtLink.Size = new System.Drawing.Size(326, 20);
			txtLink.TabIndex = 26;
			nudTo.Location = new System.Drawing.Point(355, 88);
			nudTo.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudTo.Name = "nudTo";
			nudTo.Size = new System.Drawing.Size(73, 20);
			nudTo.TabIndex = 23;
			nudTo.Value = new decimal(new int[4] { 2, 0, 0, 0 });
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(257, 91);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(95, 13);
			label2.TabIndex = 19;
			label2.Text = "Thời gian xem đến";
			nudFrom.Location = new System.Drawing.Point(175, 88);
			nudFrom.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudFrom.Name = "nudFrom";
			nudFrom.Size = new System.Drawing.Size(73, 20);
			nudFrom.TabIndex = 25;
			nudFrom.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(84, 90);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(85, 13);
			label1.TabIndex = 17;
			label1.Text = "Thời gian xem từ";
			checkBox2.AutoSize = true;
			checkBox2.Location = new System.Drawing.Point(192, 120);
			checkBox2.Name = "checkBox2";
			checkBox2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			checkBox2.Size = new System.Drawing.Size(145, 17);
			checkBox2.TabIndex = 39;
			checkBox2.Text = "Thêm vào mục yêu thích";
			checkBox2.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(504, 458);
			base.Controls.Add(checkBox2);
			base.Controls.Add(checkBox1);
			base.Controls.Add(btnSave);
			base.Controls.Add(label6);
			base.Controls.Add(txtComment);
			base.Controls.Add(label3);
			base.Controls.Add(txtLink);
			base.Controls.Add(nudTo);
			base.Controls.Add(label2);
			base.Controls.Add(nudFrom);
			base.Controls.Add(label1);
			base.Name = "frmLivePost";
			Text = "frmLivePost";
			((System.ComponentModel.ISupportInitialize)nudTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFrom).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
