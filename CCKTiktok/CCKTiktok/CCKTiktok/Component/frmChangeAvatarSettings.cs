using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmChangeAvatarSettings : Form
	{
		private IContainer components = null;

		private Label label1;

		private TextBox txtDialog;

		private Button btnSave;

		private Button btnSelect;

		private CheckBox cbxDelete;

		public frmChangeAvatarSettings()
		{
			InitializeComponent();
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
			txtDialog = new System.Windows.Forms.TextBox();
			btnSave = new System.Windows.Forms.Button();
			btnSelect = new System.Windows.Forms.Button();
			cbxDelete = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(24, 33);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(97, 13);
			label1.TabIndex = 6;
			label1.Text = "Thư mục chứa ảnh";
			txtDialog.Location = new System.Drawing.Point(133, 30);
			txtDialog.Name = "txtDialog";
			txtDialog.Size = new System.Drawing.Size(334, 20);
			txtDialog.TabIndex = 5;
			btnSave.Location = new System.Drawing.Point(479, 67);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(75, 23);
			btnSave.TabIndex = 3;
			btnSave.Text = "Lưu";
			btnSave.UseVisualStyleBackColor = true;
			btnSelect.Location = new System.Drawing.Point(479, 29);
			btnSelect.Name = "btnSelect";
			btnSelect.Size = new System.Drawing.Size(75, 23);
			btnSelect.TabIndex = 4;
			btnSelect.Text = "Chọn";
			btnSelect.UseVisualStyleBackColor = true;
			cbxDelete.AutoSize = true;
			cbxDelete.Location = new System.Drawing.Point(133, 67);
			cbxDelete.Name = "cbxDelete";
			cbxDelete.Size = new System.Drawing.Size(131, 17);
			cbxDelete.TabIndex = 7;
			cbxDelete.Text = "Xóa ảnh sau khi đăng";
			cbxDelete.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(596, 139);
			base.Controls.Add(cbxDelete);
			base.Controls.Add(label1);
			base.Controls.Add(txtDialog);
			base.Controls.Add(btnSave);
			base.Controls.Add(btnSelect);
			base.Name = "frmChangeAvatarSettings";
			Text = "Đổi ảnh Avatar";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
