using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmModule : Form
	{
		public List<string> lst = new List<string>();

		private IContainer components = null;

		private CheckBox cbxTurnOffModule;

		private CheckBox cbxadb_root;

		private CheckBox cbxRiru_zip;

		private CheckBox cbxRiruedXposedzip;

		private Button btnStart;

		private Label label1;

		public frmModule()
		{
			InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			lst.Clear();
			if (cbxTurnOffModule.Checked)
			{
				lst.Add("cbxTurnOffModule");
			}
			if (cbxadb_root.Checked)
			{
				lst.Add("adb_root.zip");
			}
			if (cbxRiru_zip.Checked)
			{
				lst.Add("Riru.zip");
			}
			if (cbxRiruedXposedzip.Checked)
			{
				lst.Add("Riru-edXposed.zip");
			}
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
			cbxTurnOffModule = new System.Windows.Forms.CheckBox();
			cbxadb_root = new System.Windows.Forms.CheckBox();
			cbxRiru_zip = new System.Windows.Forms.CheckBox();
			cbxRiruedXposedzip = new System.Windows.Forms.CheckBox();
			btnStart = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			SuspendLayout();
			cbxTurnOffModule.AutoSize = true;
			cbxTurnOffModule.Checked = true;
			cbxTurnOffModule.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxTurnOffModule.Location = new System.Drawing.Point(71, 93);
			cbxTurnOffModule.Name = "cbxTurnOffModule";
			cbxTurnOffModule.Size = new System.Drawing.Size(146, 17);
			cbxTurnOffModule.TabIndex = 0;
			cbxTurnOffModule.Text = "Turn Off  Change Module";
			cbxTurnOffModule.UseVisualStyleBackColor = true;
			cbxadb_root.AutoSize = true;
			cbxadb_root.Checked = true;
			cbxadb_root.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxadb_root.Location = new System.Drawing.Point(71, 116);
			cbxadb_root.Name = "cbxadb_root";
			cbxadb_root.Size = new System.Drawing.Size(84, 17);
			cbxadb_root.TabIndex = 1;
			cbxadb_root.Text = "adb_root.zip";
			cbxadb_root.UseVisualStyleBackColor = true;
			cbxRiru_zip.AutoSize = true;
			cbxRiru_zip.Checked = true;
			cbxRiru_zip.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxRiru_zip.Location = new System.Drawing.Point(71, 139);
			cbxRiru_zip.Name = "cbxRiru_zip";
			cbxRiru_zip.Size = new System.Drawing.Size(61, 17);
			cbxRiru_zip.TabIndex = 2;
			cbxRiru_zip.Text = "Riru.zip";
			cbxRiru_zip.UseVisualStyleBackColor = true;
			cbxRiruedXposedzip.AutoSize = true;
			cbxRiruedXposedzip.Checked = true;
			cbxRiruedXposedzip.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxRiruedXposedzip.Location = new System.Drawing.Point(71, 162);
			cbxRiruedXposedzip.Name = "cbxRiruedXposedzip";
			cbxRiruedXposedzip.Size = new System.Drawing.Size(112, 17);
			cbxRiruedXposedzip.TabIndex = 3;
			cbxRiruedXposedzip.Text = "Riru-edXposed.zip";
			cbxRiruedXposedzip.UseVisualStyleBackColor = true;
			btnStart.Location = new System.Drawing.Point(89, 209);
			btnStart.Name = "btnStart";
			btnStart.Size = new System.Drawing.Size(75, 23);
			btnStart.TabIndex = 4;
			btnStart.Text = "Start";
			btnStart.UseVisualStyleBackColor = true;
			btnStart.Click += new System.EventHandler(btnStart_Click);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(68, 61);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(80, 13);
			label1.TabIndex = 5;
			label1.Text = "Chose Modules";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(277, 295);
			base.Controls.Add(label1);
			base.Controls.Add(btnStart);
			base.Controls.Add(cbxRiruedXposedzip);
			base.Controls.Add(cbxRiru_zip);
			base.Controls.Add(cbxadb_root);
			base.Controls.Add(cbxTurnOffModule);
			base.Name = "frmModule";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "frmModule";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
