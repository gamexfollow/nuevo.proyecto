using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.Entity;

namespace CCKTiktok.Component
{
	public class frmFollowers : Form
	{
		private string sourceFile = "";

		private string Title = "";

		private IContainer components = null;

		private Button btnSave;

		private NumericUpDown nudTimeFrom;

		private NumericUpDown nudTimeTo;

		private NumericUpDown nudFollowFrom;

		private NumericUpDown nudFollowTo;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label7;

		private NumericUpDown nudDelayFrom;

		private NumericUpDown nudDelayTo;

		private Label label8;

		private Label label9;

		private Label label10;

		public frmFollowers(string sourceFile, string Title)
		{
			this.sourceFile = sourceFile;
			this.Title = Title;
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			FollowEntity followEntity = new FollowEntity();
			followEntity.WorkingFrom = Convert.ToInt32(nudTimeFrom.Value);
			followEntity.WorkingTo = Convert.ToInt32(nudTimeTo.Value);
			followEntity.FollowFrom = Convert.ToInt32(nudFollowFrom.Value);
			followEntity.FollowTo = Convert.ToInt32(nudFollowTo.Value);
			followEntity.DelayFrom = Convert.ToInt32(nudDelayFrom.Value);
			followEntity.DelayTo = Convert.ToInt32(nudDelayTo.Value);
			File.WriteAllText(sourceFile, new JavaScriptSerializer().Serialize(followEntity));
			Close();
		}

		private void frmFollowers_Load(object sender, EventArgs e)
		{
			if (File.Exists(sourceFile))
			{
				FollowEntity followEntity = new JavaScriptSerializer().Deserialize<FollowEntity>(Utils.ReadTextFile(sourceFile));
				nudTimeFrom.Value = followEntity.WorkingFrom;
				nudTimeTo.Value = followEntity.WorkingTo;
				nudFollowFrom.Value = followEntity.FollowFrom;
				nudFollowTo.Value = followEntity.FollowTo;
				nudDelayFrom.Value = followEntity.DelayFrom;
				nudDelayTo.Value = followEntity.DelayTo;
				label1.Text = Title;
			}
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
			nudTimeFrom = new System.Windows.Forms.NumericUpDown();
			nudTimeTo = new System.Windows.Forms.NumericUpDown();
			nudFollowFrom = new System.Windows.Forms.NumericUpDown();
			nudFollowTo = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			nudDelayFrom = new System.Windows.Forms.NumericUpDown();
			nudDelayTo = new System.Windows.Forms.NumericUpDown();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudTimeFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudTimeTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFollowFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFollowTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).BeginInit();
			SuspendLayout();
			btnSave.Location = new System.Drawing.Point(228, 230);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(96, 46);
			btnSave.TabIndex = 0;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			nudTimeFrom.Location = new System.Drawing.Point(176, 93);
			nudTimeFrom.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudTimeFrom.Name = "nudTimeFrom";
			nudTimeFrom.Size = new System.Drawing.Size(66, 20);
			nudTimeFrom.TabIndex = 1;
			nudTimeTo.Location = new System.Drawing.Point(327, 93);
			nudTimeTo.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudTimeTo.Name = "nudTimeTo";
			nudTimeTo.Size = new System.Drawing.Size(66, 20);
			nudTimeTo.TabIndex = 1;
			nudFollowFrom.Location = new System.Drawing.Point(176, 141);
			nudFollowFrom.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudFollowFrom.Name = "nudFollowFrom";
			nudFollowFrom.Size = new System.Drawing.Size(66, 20);
			nudFollowFrom.TabIndex = 1;
			nudFollowTo.Location = new System.Drawing.Point(327, 141);
			nudFollowTo.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudFollowTo.Name = "nudFollowTo";
			nudFollowTo.Size = new System.Drawing.Size(66, 20);
			nudFollowTo.TabIndex = 1;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(228, 36);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(96, 13);
			label1.TabIndex = 2;
			label1.Text = "Cấu hình Followers";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(98, 97);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(63, 13);
			label2.TabIndex = 3;
			label2.Text = "Thời gian từ";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(277, 95);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(26, 13);
			label3.TabIndex = 4;
			label3.Text = "đến";
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(399, 95);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(26, 13);
			label4.TabIndex = 5;
			label4.Text = "giây";
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(112, 143);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(49, 13);
			label5.TabIndex = 3;
			label5.Text = "Follow từ";
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(277, 143);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(26, 13);
			label6.TabIndex = 4;
			label6.Text = "đến";
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(399, 143);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(33, 13);
			label7.TabIndex = 5;
			label7.Text = "người";
			nudDelayFrom.Location = new System.Drawing.Point(176, 180);
			nudDelayFrom.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudDelayFrom.Name = "nudDelayFrom";
			nudDelayFrom.Size = new System.Drawing.Size(66, 20);
			nudDelayFrom.TabIndex = 1;
			nudDelayTo.Location = new System.Drawing.Point(327, 180);
			nudDelayTo.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudDelayTo.Name = "nudDelayTo";
			nudDelayTo.Size = new System.Drawing.Size(66, 20);
			nudDelayTo.TabIndex = 1;
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(112, 182);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(46, 13);
			label8.TabIndex = 3;
			label8.Text = "Delay từ";
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(277, 182);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(26, 13);
			label9.TabIndex = 4;
			label9.Text = "đến";
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(399, 182);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(26, 13);
			label10.TabIndex = 5;
			label10.Text = "giây";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(555, 330);
			base.Controls.Add(label10);
			base.Controls.Add(label7);
			base.Controls.Add(label9);
			base.Controls.Add(label6);
			base.Controls.Add(label4);
			base.Controls.Add(label8);
			base.Controls.Add(label5);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(nudTimeTo);
			base.Controls.Add(nudDelayTo);
			base.Controls.Add(nudDelayFrom);
			base.Controls.Add(nudFollowTo);
			base.Controls.Add(nudFollowFrom);
			base.Controls.Add(nudTimeFrom);
			base.Controls.Add(btnSave);
			base.Name = "frmFollowers";
			Text = "frmFollowers";
			base.Load += new System.EventHandler(frmFollowers_Load);
			((System.ComponentModel.ISupportInitialize)nudTimeFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudTimeTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFollowFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFollowTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
