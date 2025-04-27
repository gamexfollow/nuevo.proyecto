using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmDownload : Form
	{
		private IContainer components = null;

		private ProgressBar progressBar;

		private Button btnExit;

		private Label lblMessage;

		public string Source { get; set; }

		public string Destination { get; set; }

		public bool DownloadCompleted { get; set; }

		public frmDownload()
		{
			InitializeComponent();
			DownloadCompleted = false;
			base.StartPosition = FormStartPosition.CenterScreen;
		}

		public bool ChangeFileName(string source, string desc)
		{
			bool result = false;
			try
			{
				if (File.Exists(source))
				{
					File.Move(source, desc);
					result = true;
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		public void Download(string source, string desc)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				WebClient webClient = new WebClient();
				webClient.DownloadProgressChanged += client_DownloadProgressChanged;
				webClient.DownloadFileCompleted += client_DownloadFileCompleted;
				webClient.DownloadFileAsync(new Uri(source), desc);
			});
			thread.Start();
		}

		private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			BeginInvoke((MethodInvoker)delegate
			{
				double num = double.Parse(e.BytesReceived.ToString());
				double num2 = double.Parse(e.TotalBytesToReceive.ToString());
				double d = num / num2 * 100.0;
				lblMessage.Text = "Downloaded " + e.BytesReceived.ToString("#,###") + " of " + e.TotalBytesToReceive.ToString("#,###");
				progressBar.Value = int.Parse(Math.Truncate(d).ToString());
			});
		}

		private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			BeginInvoke((MethodInvoker)delegate
			{
				lblMessage.Text = "Completed";
				DownloadCompleted = true;
				Close();
			});
		}

		private void frmDownload_Load(object sender, EventArgs e)
		{
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
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
			progressBar = new System.Windows.Forms.ProgressBar();
			btnExit = new System.Windows.Forms.Button();
			lblMessage = new System.Windows.Forms.Label();
			SuspendLayout();
			progressBar.Location = new System.Drawing.Point(12, 26);
			progressBar.Name = "progressBar";
			progressBar.Size = new System.Drawing.Size(390, 23);
			progressBar.TabIndex = 0;
			btnExit.Location = new System.Drawing.Point(157, 90);
			btnExit.Name = "btnExit";
			btnExit.Size = new System.Drawing.Size(75, 23);
			btnExit.TabIndex = 1;
			btnExit.Text = "Exit";
			btnExit.UseVisualStyleBackColor = true;
			btnExit.Click += new System.EventHandler(btnExit_Click);
			lblMessage.AutoSize = true;
			lblMessage.Location = new System.Drawing.Point(13, 56);
			lblMessage.Name = "lblMessage";
			lblMessage.Size = new System.Drawing.Size(27, 13);
			lblMessage.TabIndex = 2;
			lblMessage.Text = "Msg";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(414, 138);
			base.ControlBox = false;
			base.Controls.Add(lblMessage);
			base.Controls.Add(btnExit);
			base.Controls.Add(progressBar);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmDownload";
			Text = "Downloading";
			base.Load += new System.EventHandler(frmDownload_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
