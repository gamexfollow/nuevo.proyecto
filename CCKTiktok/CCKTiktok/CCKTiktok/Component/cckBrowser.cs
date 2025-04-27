using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CefSharp;
using CefSharp.WinForms;

namespace CCKTiktok.Component
{
	public class cckBrowser : Form
	{
		private static bool isInitCef;

		public ChromiumWebBrowser browser;

		private IContainer components = null;

		public cckBrowser()
		{
			InitializeComponent();
		}

		public cckBrowser(string url, int with = 0, int height = 0)
		{
			InitializeComponent();
			if (!isInitCef)
			{
				isInitCef = true;
				Cef.Initialize(new CefSettings());
			}
			string text = Utils.ReadTextFile(Application.StartupPath + "\\Config\\acc.txt");
			if (text != "")
			{
				text.Split("|".ToCharArray());
			}
			browser = new ChromiumWebBrowser(url);
			browser.FrameLoadEnd += browser_FrameLoadEnd;
			base.Controls.Add(browser);
			browser.Dock = DockStyle.Fill;
			if (with > 0 && height > 0)
			{
				MinimumSize = new Size(with, height);
				MaximumSize = new Size(with, height);
				base.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - base.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - base.Height) / 2);
			}
		}

		private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs args)
		{
			if (args.Frame.IsMain)
			{
				args.Browser.MainFrame.ExecuteJavaScriptAsync("document.body.style.overflow = 'hidden'");
			}
		}

		private void cckBrowser_Load(object sender, EventArgs e)
		{
			base.StartPosition = FormStartPosition.CenterScreen;
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
			SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(784, 561);
			base.Name = "cckBrowser";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "CCK Browser";
			base.Load += new System.EventHandler(cckBrowser_Load);
			ResumeLayout(false);
		}
	}
}
