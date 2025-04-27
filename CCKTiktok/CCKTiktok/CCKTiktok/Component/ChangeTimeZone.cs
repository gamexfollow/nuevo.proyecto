using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class ChangeTimeZone : Form
	{
		private IContainer components = null;

		private Button btnSave;

		private TextBox txtTimeZone;

		private LinkLabel linkLabel1;

		private LinkLabel linkLabel2;

		private LinkLabel linkLabel3;

		private LinkLabel linkLabel4;

		private LinkLabel linkLabel5;

		public ChangeTimeZone()
		{
			InitializeComponent();
		}

		private void ChangeTimeZone_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.TIME_ZONE))
			{
				string text = Utils.ReadTextFile(CaChuaConstant.TIME_ZONE);
				txtTimeZone.Text = text;
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.TIME_ZONE, txtTimeZone.Text.ToString());
			Close();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			txtTimeZone.Text = linkLabel1.Text;
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			txtTimeZone.Text = linkLabel3.Text;
		}

		private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			txtTimeZone.Text = linkLabel4.Text;
		}

		private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			txtTimeZone.Text = linkLabel5.Text;
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			txtTimeZone.Text = linkLabel2.Text;
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
			btnSave = new System.Windows.Forms.Button();
			txtTimeZone = new System.Windows.Forms.TextBox();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			linkLabel3 = new System.Windows.Forms.LinkLabel();
			linkLabel4 = new System.Windows.Forms.LinkLabel();
			linkLabel5 = new System.Windows.Forms.LinkLabel();
			SuspendLayout();
			btnSave.Location = new System.Drawing.Point(205, 287);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(75, 23);
			btnSave.TabIndex = 1;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			txtTimeZone.Location = new System.Drawing.Point(119, 46);
			txtTimeZone.Name = "txtTimeZone";
			txtTimeZone.Size = new System.Drawing.Size(263, 20);
			txtTimeZone.TabIndex = 2;
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(116, 83);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(112, 13);
			linkLabel1.TabIndex = 3;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "United States|Chicago";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			linkLabel2.AutoSize = true;
			linkLabel2.Location = new System.Drawing.Point(270, 83);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(45, 13);
			linkLabel2.TabIndex = 3;
			linkLabel2.TabStop = true;
			linkLabel2.Text = "Vietnam";
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
			linkLabel3.AutoSize = true;
			linkLabel3.Location = new System.Drawing.Point(116, 112);
			linkLabel3.Name = "linkLabel3";
			linkLabel3.Size = new System.Drawing.Size(131, 13);
			linkLabel3.TabIndex = 4;
			linkLabel3.TabStop = true;
			linkLabel3.Text = "United States|Los Angeles";
			linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel3_LinkClicked);
			linkLabel4.AutoSize = true;
			linkLabel4.Location = new System.Drawing.Point(116, 146);
			linkLabel4.Name = "linkLabel4";
			linkLabel4.Size = new System.Drawing.Size(98, 13);
			linkLabel4.TabIndex = 5;
			linkLabel4.TabStop = true;
			linkLabel4.Text = "United States|Adak";
			linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel4_LinkClicked);
			linkLabel5.AutoSize = true;
			linkLabel5.Location = new System.Drawing.Point(116, 180);
			linkLabel5.Name = "linkLabel5";
			linkLabel5.Size = new System.Drawing.Size(111, 13);
			linkLabel5.TabIndex = 6;
			linkLabel5.TabStop = true;
			linkLabel5.Text = "United States|Phoenix";
			linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel5_LinkClicked);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(485, 347);
			base.Controls.Add(linkLabel5);
			base.Controls.Add(linkLabel4);
			base.Controls.Add(linkLabel3);
			base.Controls.Add(linkLabel2);
			base.Controls.Add(linkLabel1);
			base.Controls.Add(txtTimeZone);
			base.Controls.Add(btnSave);
			base.Name = "ChangeTimeZone";
			Text = "ChangeTimeZone";
			base.Load += new System.EventHandler(ChangeTimeZone_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
