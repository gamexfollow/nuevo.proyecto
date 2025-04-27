using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmUpdate : Form
	{
		private IContainer components = null;

		private Button btnUpdate;

		private Button btnCancel;

		private TextBox txtMsg;

		public frmUpdate(string msg)
		{
			InitializeComponent();
			txtMsg.Text = msg;
		}

		private void frmUpdate_Load(object sender, EventArgs e)
		{
			try
			{
				string text = new WebClient().DownloadString("https://cck.vn/Download/Update/History/updatelog_tiktok.txt");
				txtMsg.Text = text;
			}
			catch
			{
				txtMsg.Text = "Chưa có lịch sử Update";
			}
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			string text = "Config\\version.txt";
			if (!File.Exists(text))
			{
				File.WriteAllText(text, $"{typeof(frmLogin).Assembly.GetName().Version}");
			}
			string text2 = Utils.ReadTextFile(text).Trim();
			if (text2 != typeof(frmLogin).Assembly.GetName().Version.ToString())
			{
				text2 = typeof(frmLogin).Assembly.GetName().Version.ToString();
			}
			if (!Directory.Exists(Application.StartupPath + "\\Backup\\"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\Backup\\");
			}
			if (!Directory.Exists(Application.StartupPath + "\\Backup\\" + text2))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\Backup\\" + text2);
			}
			File.Copy(Application.StartupPath + "\\CCKTiktok.exe", Application.StartupPath + string.Format("\\Backup\\{1}\\CCKTiktok_{0}.exe", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), text2));
			ProcessStartInfo processStartInfo = new ProcessStartInfo("AutoUpdateT.exe");
			processStartInfo.UseShellExecute = true;
			processStartInfo.Verb = "runas";
			Process.Start(processStartInfo);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cbxMsg_CheckedChanged(object sender, EventArgs e)
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
			btnUpdate = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			txtMsg = new System.Windows.Forms.TextBox();
			SuspendLayout();
			btnUpdate.Location = new System.Drawing.Point(357, 563);
			btnUpdate.Name = "btnUpdate";
			btnUpdate.Size = new System.Drawing.Size(75, 23);
			btnUpdate.TabIndex = 1;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = true;
			btnUpdate.Click += new System.EventHandler(btnUpdate_Click);
			btnCancel.Location = new System.Drawing.Point(469, 563);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new System.Drawing.Size(75, 23);
			btnCancel.TabIndex = 2;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += new System.EventHandler(btnCancel_Click);
			txtMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
			txtMsg.Location = new System.Drawing.Point(13, 13);
			txtMsg.Multiline = true;
			txtMsg.Name = "txtMsg";
			txtMsg.Size = new System.Drawing.Size(893, 533);
			txtMsg.TabIndex = 4;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.Color.White;
			base.ClientSize = new System.Drawing.Size(919, 599);
			base.Controls.Add(txtMsg);
			base.Controls.Add(btnCancel);
			base.Controls.Add(btnUpdate);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.Name = "frmUpdate";
			base.Padding = new System.Windows.Forms.Padding(10);
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Update New Version";
			base.Load += new System.EventHandler(frmUpdate_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
