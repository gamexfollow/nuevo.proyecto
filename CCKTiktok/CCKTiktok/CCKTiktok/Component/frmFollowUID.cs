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
	public class frmFollowUID : Form
	{
		private IContainer components = null;

		private Label label4;

		private NumericUpDown nudDelay;

		private Label label3;

		private NumericUpDown nudNum;

		private Label label1;

		private TextBox txtUid;

		private Button btnSave;

		private CheckBox cbxRemove;

		public frmFollowUID()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			FollowFriendEntity followFriendEntity = new FollowFriendEntity();
			followFriendEntity.Delay = nudDelay.Value;
			followFriendEntity.Number = nudNum.Value;
			File.WriteAllText(CaChuaConstant.FOLLOW_UID_DATA, txtUid.Text);
			followFriendEntity.RemoveAfterFollow = cbxRemove.Checked;
			File.WriteAllText(CaChuaConstant.FOLLOW_UID, new JavaScriptSerializer().Serialize(followFriendEntity));
			Close();
		}

		private void frmFollowUID_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.FOLLOW_UID))
			{
				FollowFriendEntity followFriendEntity = new JavaScriptSerializer().Deserialize<FollowFriendEntity>(Utils.ReadTextFile(CaChuaConstant.FOLLOW_UID));
				if (followFriendEntity != null)
				{
					txtUid.Text = Utils.ReadTextFile(CaChuaConstant.FOLLOW_UID_DATA);
					nudDelay.Value = followFriendEntity.Delay;
					nudNum.Value = followFriendEntity.Number;
					cbxRemove.Checked = followFriendEntity.RemoveAfterFollow;
				}
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
			label4 = new System.Windows.Forms.Label();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			nudNum = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			txtUid = new System.Windows.Forms.TextBox();
			btnSave = new System.Windows.Forms.Button();
			cbxRemove = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudNum).BeginInit();
			SuspendLayout();
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(343, 332);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(34, 13);
			label4.TabIndex = 14;
			label4.Text = "Delay";
			nudDelay.Location = new System.Drawing.Point(383, 328);
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(53, 20);
			nudDelay.TabIndex = 13;
			nudDelay.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(40, 330);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(182, 13);
			label3.TabIndex = 12;
			label3.Text = "Số lượng UID sẽ được theo dõi tố đa";
			nudNum.Location = new System.Drawing.Point(228, 328);
			nudNum.Name = "nudNum";
			nudNum.Size = new System.Drawing.Size(56, 20);
			nudNum.TabIndex = 11;
			nudNum.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(37, 21);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(143, 13);
			label1.TabIndex = 10;
			label1.Text = "Danh sách UID muốn Follow";
			txtUid.Location = new System.Drawing.Point(37, 48);
			txtUid.MaxLength = 32767000;
			txtUid.Multiline = true;
			txtUid.Name = "txtUid";
			txtUid.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtUid.Size = new System.Drawing.Size(396, 258);
			txtUid.TabIndex = 9;
			btnSave.Location = new System.Drawing.Point(187, 393);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(107, 23);
			btnSave.TabIndex = 15;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			cbxRemove.AutoSize = true;
			cbxRemove.Location = new System.Drawing.Point(43, 359);
			cbxRemove.Name = "cbxRemove";
			cbxRemove.Size = new System.Drawing.Size(124, 17);
			cbxRemove.TabIndex = 16;
			cbxRemove.Text = "Remove After Follow";
			cbxRemove.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(490, 448);
			base.Controls.Add(cbxRemove);
			base.Controls.Add(btnSave);
			base.Controls.Add(label4);
			base.Controls.Add(nudDelay);
			base.Controls.Add(label3);
			base.Controls.Add(nudNum);
			base.Controls.Add(label1);
			base.Controls.Add(txtUid);
			base.Name = "frmFollowUID";
			Text = "frmFollowUID";
			base.Load += new System.EventHandler(frmFollowUID_Load);
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			((System.ComponentModel.ISupportInitialize)nudNum).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
