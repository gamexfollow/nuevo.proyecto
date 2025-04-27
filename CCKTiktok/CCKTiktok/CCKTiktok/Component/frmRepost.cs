using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmRepost : Form
	{
		private IContainer components = null;

		private GroupBox groupBox1;

		private Label label13;

		private TextBox txtAccount;

		private GroupBox groupBox2;

		private Label label1;

		private TextBox txtLink;

		private Button button1;

		private RadioButton rbtRepostAccount;

		private RadioButton rbtRepostLink;

		private Label label2;

		private NumericUpDown nudDelay;

		private Label label3;

		private Label label4;

		public frmRepost()
		{
			InitializeComponent();
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
			groupBox1 = new System.Windows.Forms.GroupBox();
			label13 = new System.Windows.Forms.Label();
			txtAccount = new System.Windows.Forms.TextBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label1 = new System.Windows.Forms.Label();
			txtLink = new System.Windows.Forms.TextBox();
			button1 = new System.Windows.Forms.Button();
			rbtRepostAccount = new System.Windows.Forms.RadioButton();
			rbtRepostLink = new System.Windows.Forms.RadioButton();
			label2 = new System.Windows.Forms.Label();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			SuspendLayout();
			groupBox1.Controls.Add(label13);
			groupBox1.Controls.Add(txtAccount);
			groupBox1.Location = new System.Drawing.Point(26, 89);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(467, 166);
			groupBox1.TabIndex = 53;
			groupBox1.TabStop = false;
			groupBox1.Text = "Cấu hình đăng lại";
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(11, 21);
			label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(214, 13);
			label13.TabIndex = 51;
			label13.Text = "Đăng lại bài ngẫu nhiên của tài khoản khác";
			txtAccount.Location = new System.Drawing.Point(19, 47);
			txtAccount.Margin = new System.Windows.Forms.Padding(2);
			txtAccount.MaxLength = 3276700;
			txtAccount.Multiline = true;
			txtAccount.Name = "txtAccount";
			txtAccount.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtAccount.Size = new System.Drawing.Size(429, 104);
			txtAccount.TabIndex = 45;
			groupBox2.Controls.Add(label1);
			groupBox2.Controls.Add(txtLink);
			groupBox2.Location = new System.Drawing.Point(26, 304);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(467, 166);
			groupBox2.TabIndex = 54;
			groupBox2.TabStop = false;
			groupBox2.Text = "Cấu hình đăng lại";
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(11, 21);
			label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(210, 13);
			label1.TabIndex = 51;
			label1.Text = "Đăng lại bài từ Link bài của tài khoản khác";
			txtLink.Location = new System.Drawing.Point(14, 45);
			txtLink.Margin = new System.Windows.Forms.Padding(2);
			txtLink.MaxLength = 3276700;
			txtLink.Multiline = true;
			txtLink.Name = "txtLink";
			txtLink.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtLink.Size = new System.Drawing.Size(429, 104);
			txtLink.TabIndex = 45;
			button1.Location = new System.Drawing.Point(190, 551);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(135, 23);
			button1.TabIndex = 55;
			button1.Text = "Save";
			button1.UseVisualStyleBackColor = true;
			rbtRepostAccount.AutoSize = true;
			rbtRepostAccount.Checked = true;
			rbtRepostAccount.Location = new System.Drawing.Point(26, 59);
			rbtRepostAccount.Name = "rbtRepostAccount";
			rbtRepostAccount.Size = new System.Drawing.Size(232, 17);
			rbtRepostAccount.TabIndex = 56;
			rbtRepostAccount.TabStop = true;
			rbtRepostAccount.Text = "Đăng lại bài ngẫu nhiên của tài khoản khác";
			rbtRepostAccount.UseVisualStyleBackColor = true;
			rbtRepostLink.AutoSize = true;
			rbtRepostLink.Location = new System.Drawing.Point(26, 271);
			rbtRepostLink.Name = "rbtRepostLink";
			rbtRepostLink.Size = new System.Drawing.Size(116, 17);
			rbtRepostLink.TabIndex = 57;
			rbtRepostLink.Text = "Đăng lại bài từ Link";
			rbtRepostLink.UseVisualStyleBackColor = true;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(26, 489);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(100, 13);
			label2.TabIndex = 58;
			label2.Text = "Xem trước khi đăng";
			nudDelay.Location = new System.Drawing.Point(140, 488);
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(70, 20);
			nudDelay.TabIndex = 59;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(222, 491);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(26, 13);
			label3.TabIndex = 60;
			label3.Text = "giây";
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			label4.Location = new System.Drawing.Point(186, 21);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(156, 20);
			label4.TabIndex = 61;
			label4.Text = "Cấu hình đăng lại bài";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(536, 608);
			base.Controls.Add(label4);
			base.Controls.Add(label3);
			base.Controls.Add(nudDelay);
			base.Controls.Add(label2);
			base.Controls.Add(rbtRepostLink);
			base.Controls.Add(rbtRepostAccount);
			base.Controls.Add(button1);
			base.Controls.Add(groupBox2);
			base.Controls.Add(groupBox1);
			base.Name = "frmRepost";
			Text = "frmRepost";
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
