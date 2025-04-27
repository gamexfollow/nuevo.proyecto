using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmHMA : Form
	{
		private IContainer components = null;

		private Label label1;

		private TextBox txtUsser;

		private RadioButton rbtHMALogged;

		private RadioButton rbtHMA_New;

		private Button btnSave;

		private NumericUpDown nudDelay;

		private Label label2;

		public frmHMA()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.HMA, txtUsser.Text.Trim());
			File.WriteAllText(CaChuaConstant.HMA_TYPE, rbtHMALogged.Checked.ToString());
			File.WriteAllText(CaChuaConstant.HMA_DELAY, nudDelay.Value.ToString());
			Close();
		}

		private void frmHMA_Load(object sender, EventArgs e)
		{
			txtUsser.Text = Utils.ReadTextFile(CaChuaConstant.HMA);
			rbtHMALogged.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.HMA_TYPE));
			rbtHMA_New.Checked = !rbtHMALogged.Checked;
			nudDelay.Value = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.HMA_DELAY));
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
			txtUsser = new System.Windows.Forms.TextBox();
			rbtHMALogged = new System.Windows.Forms.RadioButton();
			rbtHMA_New = new System.Windows.Forms.RadioButton();
			btnSave = new System.Windows.Forms.Button();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(184, 66);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(86, 13);
			label1.TabIndex = 9;
			label1.Text = "Email | Password";
			txtUsser.Location = new System.Drawing.Point(35, 96);
			txtUsser.Multiline = true;
			txtUsser.Name = "txtUsser";
			txtUsser.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtUsser.Size = new System.Drawing.Size(398, 88);
			txtUsser.TabIndex = 8;
			rbtHMALogged.AutoSize = true;
			rbtHMALogged.Checked = true;
			rbtHMALogged.Location = new System.Drawing.Point(140, 27);
			rbtHMALogged.Name = "rbtHMALogged";
			rbtHMALogged.Size = new System.Drawing.Size(100, 17);
			rbtHMALogged.TabIndex = 6;
			rbtHMALogged.TabStop = true;
			rbtHMALogged.Text = "HMA Logged In";
			rbtHMALogged.UseVisualStyleBackColor = true;
			rbtHMA_New.AutoSize = true;
			rbtHMA_New.Location = new System.Drawing.Point(35, 27);
			rbtHMA_New.Name = "rbtHMA_New";
			rbtHMA_New.Size = new System.Drawing.Size(74, 17);
			rbtHMA_New.TabIndex = 7;
			rbtHMA_New.Text = "HMA New";
			rbtHMA_New.UseVisualStyleBackColor = true;
			btnSave.Location = new System.Drawing.Point(187, 207);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(75, 23);
			btnSave.TabIndex = 5;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			nudDelay.Location = new System.Drawing.Point(380, 27);
			nudDelay.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(53, 20);
			nudDelay.TabIndex = 10;
			nudDelay.Value = new decimal(new int[4] { 10, 0, 0, 0 });
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(340, 29);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(34, 13);
			label2.TabIndex = 11;
			label2.Text = "Delay";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(484, 275);
			base.Controls.Add(label2);
			base.Controls.Add(nudDelay);
			base.Controls.Add(label1);
			base.Controls.Add(txtUsser);
			base.Controls.Add(rbtHMALogged);
			base.Controls.Add(rbtHMA_New);
			base.Controls.Add(btnSave);
			base.Name = "frmHMA";
			Text = "frmHMA";
			base.Load += new System.EventHandler(frmHMA_Load);
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
