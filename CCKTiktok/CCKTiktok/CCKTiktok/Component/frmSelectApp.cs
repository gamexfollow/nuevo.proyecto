using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmSelectApp : Form
	{
		private IContainer components = null;

		private RadioButton rbtGlobal;

		private RadioButton rbtUs;

		private RadioButton tbnOther;

		private TextBox txtApp;

		private Button btnApp;

		public new string Text { get; set; }

		public frmSelectApp()
		{
			InitializeComponent();
		}

		private void rbtGlobal_CheckedChanged(object sender, EventArgs e)
		{
			txtApp.Text = rbtGlobal.Text;
		}

		private void rbtUs_CheckedChanged(object sender, EventArgs e)
		{
			txtApp.Text = rbtUs.Text;
		}

		private void tbnOther_CheckedChanged(object sender, EventArgs e)
		{
			txtApp.Text = "";
			txtApp.Focus();
		}

		private void btnApp_Click(object sender, EventArgs e)
		{
			Text = txtApp.Text;
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
			rbtGlobal = new System.Windows.Forms.RadioButton();
			rbtUs = new System.Windows.Forms.RadioButton();
			tbnOther = new System.Windows.Forms.RadioButton();
			txtApp = new System.Windows.Forms.TextBox();
			btnApp = new System.Windows.Forms.Button();
			SuspendLayout();
			rbtGlobal.AutoSize = true;
			rbtGlobal.Location = new System.Drawing.Point(71, 28);
			rbtGlobal.Name = "rbtGlobal";
			rbtGlobal.Size = new System.Drawing.Size(132, 17);
			rbtGlobal.TabIndex = 0;
			rbtGlobal.TabStop = true;
			rbtGlobal.Text = "com.ss.android.ugc.trill";
			rbtGlobal.UseVisualStyleBackColor = true;
			rbtGlobal.CheckedChanged += new System.EventHandler(rbtGlobal_CheckedChanged);
			rbtUs.AutoSize = true;
			rbtUs.Location = new System.Drawing.Point(71, 52);
			rbtUs.Name = "rbtUs";
			rbtUs.Size = new System.Drawing.Size(140, 17);
			rbtUs.TabIndex = 0;
			rbtUs.TabStop = true;
			rbtUs.Text = "com.zhiliaoapp.musically";
			rbtUs.UseVisualStyleBackColor = true;
			rbtUs.CheckedChanged += new System.EventHandler(rbtUs_CheckedChanged);
			tbnOther.AutoSize = true;
			tbnOther.Location = new System.Drawing.Point(71, 76);
			tbnOther.Name = "tbnOther";
			tbnOther.Size = new System.Drawing.Size(106, 17);
			tbnOther.TabIndex = 0;
			tbnOther.TabStop = true;
			tbnOther.Text = "App khác (Other)";
			tbnOther.UseVisualStyleBackColor = true;
			tbnOther.CheckedChanged += new System.EventHandler(tbnOther_CheckedChanged);
			txtApp.Location = new System.Drawing.Point(71, 113);
			txtApp.Name = "txtApp";
			txtApp.Size = new System.Drawing.Size(179, 20);
			txtApp.TabIndex = 1;
			btnApp.Location = new System.Drawing.Point(112, 156);
			btnApp.Name = "btnApp";
			btnApp.Size = new System.Drawing.Size(75, 23);
			btnApp.TabIndex = 2;
			btnApp.Text = "OK";
			btnApp.UseVisualStyleBackColor = true;
			btnApp.Click += new System.EventHandler(btnApp_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(322, 217);
			base.Controls.Add(btnApp);
			base.Controls.Add(txtApp);
			base.Controls.Add(tbnOther);
			base.Controls.Add(rbtUs);
			base.Controls.Add(rbtGlobal);
			base.Name = "frmSelectApp";
			Text = "frmSelectApp";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
