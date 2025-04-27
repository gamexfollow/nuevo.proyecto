using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmFilter : Form
	{
		private List<string> lst = new List<string>();

		private IContainer components = null;

		private ListBox listBoxItem;

		public string Selected { get; set; }

		public frmFilter(List<string> lst)
		{
			Selected = "";
			lst = lst.ToList();
			InitializeComponent();
		}

		private void frmFilter_Load(object sender, EventArgs e)
		{
			foreach (string item in lst)
			{
				listBoxItem.Items.Add(item);
			}
		}

		private void listBoxItem_SelectedIndexChanged(object sender, EventArgs e)
		{
			Selected = listBoxItem.SelectedItem.ToString();
			MessageBox.Show(Selected);
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
			listBoxItem = new System.Windows.Forms.ListBox();
			SuspendLayout();
			listBoxItem.Dock = System.Windows.Forms.DockStyle.Fill;
			listBoxItem.FormattingEnabled = true;
			listBoxItem.Location = new System.Drawing.Point(0, 0);
			listBoxItem.Name = "listBoxItem";
			listBoxItem.Size = new System.Drawing.Size(800, 450);
			listBoxItem.TabIndex = 0;
			listBoxItem.SelectedIndexChanged += new System.EventHandler(listBoxItem_SelectedIndexChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(800, 450);
			base.Controls.Add(listBoxItem);
			base.Name = "frmFilter";
			Text = "frmFilter";
			base.Load += new System.EventHandler(frmFilter_Load);
			ResumeLayout(false);
		}
	}
}
