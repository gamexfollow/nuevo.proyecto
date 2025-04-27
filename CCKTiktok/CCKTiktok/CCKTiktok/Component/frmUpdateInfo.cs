using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmUpdateInfo : Form
	{
		private IContainer components = null;

		private CheckBox cbxLoginHistory;

		private Button btnSave;

		private CheckBox cbxAvatar;

		private CheckBox cbxFollow;

		private CheckBox cbxYear;

		private CheckBox cbxPublicInfo;

		private CheckBox cbxLike;

		private CheckBox cbxLogOut;

		public frmUpdateInfo()
		{
			InitializeComponent();
		}

		private void frmUpdateInfo_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.UpdateInfo))
			{
				UpdateInfoField updateInfoField = new JavaScriptSerializer().Deserialize<UpdateInfoField>(Utils.ReadTextFile(CaChuaConstant.UpdateInfo));
				if (updateInfoField != null)
				{
					cbxLoginHistory.Checked = updateInfoField.XoaLichSuDangNhap;
					cbxPublicInfo.Checked = updateInfoField.PublicInfo;
					cbxLike.Checked = updateInfoField.GetLike;
					cbxAvatar.Checked = updateInfoField.CheckAvatar;
					cbxFollow.Checked = updateInfoField.GetFollow;
					cbxYear.Checked = updateInfoField.GetYear;
					cbxLogOut.Checked = updateInfoField.LogOut;
				}
			}
			Utils.ChangeLanguage(this, new List<Type> { typeof(CheckBox) });
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			UpdateInfoField updateInfoField = new UpdateInfoField();
			updateInfoField.XoaLichSuDangNhap = cbxLoginHistory.Checked;
			updateInfoField.PublicInfo = cbxPublicInfo.Checked;
			updateInfoField.GetLike = cbxLike.Checked;
			updateInfoField.CheckAvatar = cbxAvatar.Checked;
			updateInfoField.GetFollow = cbxFollow.Checked;
			updateInfoField.GetYear = cbxYear.Checked;
			updateInfoField.LogOut = cbxLogOut.Checked;
			File.WriteAllText(CaChuaConstant.UpdateInfo, new JavaScriptSerializer().Serialize(updateInfoField));
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
			cbxLoginHistory = new System.Windows.Forms.CheckBox();
			btnSave = new System.Windows.Forms.Button();
			cbxAvatar = new System.Windows.Forms.CheckBox();
			cbxFollow = new System.Windows.Forms.CheckBox();
			cbxYear = new System.Windows.Forms.CheckBox();
			cbxPublicInfo = new System.Windows.Forms.CheckBox();
			cbxLike = new System.Windows.Forms.CheckBox();
			cbxLogOut = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			cbxLoginHistory.AutoSize = true;
			cbxLoginHistory.Location = new System.Drawing.Point(68, 43);
			cbxLoginHistory.Name = "cbxLoginHistory";
			cbxLoginHistory.Size = new System.Drawing.Size(133, 17);
			cbxLoginHistory.TabIndex = 0;
			cbxLoginHistory.Tag = "Clear login history";
			cbxLoginHistory.Text = "Xóa lịch sử đăng nhập";
			cbxLoginHistory.UseVisualStyleBackColor = true;
			btnSave.Location = new System.Drawing.Point(202, 219);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(106, 41);
			btnSave.TabIndex = 1;
			btnSave.Tag = "Lưu lại";
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			cbxAvatar.AutoSize = true;
			cbxAvatar.Location = new System.Drawing.Point(68, 78);
			cbxAvatar.Name = "cbxAvatar";
			cbxAvatar.Size = new System.Drawing.Size(98, 17);
			cbxAvatar.TabIndex = 2;
			cbxAvatar.Tag = "Check Avatar";
			cbxAvatar.Text = "Kiểm tra Avatar";
			cbxAvatar.UseVisualStyleBackColor = true;
			cbxFollow.AutoSize = true;
			cbxFollow.Location = new System.Drawing.Point(68, 113);
			cbxFollow.Name = "cbxFollow";
			cbxFollow.Size = new System.Drawing.Size(148, 17);
			cbxFollow.TabIndex = 3;
			cbxFollow.Tag = "Check Follow";
			cbxFollow.Text = "Kiểm tra số người theo dõi";
			cbxFollow.UseVisualStyleBackColor = true;
			cbxYear.AutoSize = true;
			cbxYear.Location = new System.Drawing.Point(320, 41);
			cbxYear.Name = "cbxYear";
			cbxYear.Size = new System.Drawing.Size(152, 17);
			cbxYear.TabIndex = 4;
			cbxYear.Tag = "Created Year";
			cbxYear.Text = "Kiểm tra năm tạo tài khoản";
			cbxYear.UseVisualStyleBackColor = true;
			cbxPublicInfo.AutoSize = true;
			cbxPublicInfo.Location = new System.Drawing.Point(320, 78);
			cbxPublicInfo.Name = "cbxPublicInfo";
			cbxPublicInfo.Size = new System.Drawing.Size(118, 17);
			cbxPublicInfo.TabIndex = 5;
			cbxPublicInfo.Tag = "Public Infor";
			cbxPublicInfo.Text = "Công khai thông tin";
			cbxPublicInfo.UseVisualStyleBackColor = true;
			cbxLike.AutoSize = true;
			cbxLike.Location = new System.Drawing.Point(320, 113);
			cbxLike.Name = "cbxLike";
			cbxLike.Size = new System.Drawing.Size(126, 17);
			cbxLike.TabIndex = 6;
			cbxLike.Tag = "Like Count";
			cbxLike.Text = "Kiểm tra số lượt thích";
			cbxLike.UseVisualStyleBackColor = true;
			cbxLogOut.AutoSize = true;
			cbxLogOut.Location = new System.Drawing.Point(68, 148);
			cbxLogOut.Name = "cbxLogOut";
			cbxLogOut.Size = new System.Drawing.Size(75, 17);
			cbxLogOut.TabIndex = 7;
			cbxLogOut.Tag = "Check Follow";
			cbxLogOut.Text = "Đăng xuất";
			cbxLogOut.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(573, 322);
			base.Controls.Add(cbxLogOut);
			base.Controls.Add(cbxLike);
			base.Controls.Add(cbxPublicInfo);
			base.Controls.Add(cbxYear);
			base.Controls.Add(cbxFollow);
			base.Controls.Add(cbxAvatar);
			base.Controls.Add(btnSave);
			base.Controls.Add(cbxLoginHistory);
			base.Name = "frmUpdateInfo";
			Text = "Update Information / Cập nhật thông tin cá nhân";
			base.Load += new System.EventHandler(frmUpdateInfo_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
