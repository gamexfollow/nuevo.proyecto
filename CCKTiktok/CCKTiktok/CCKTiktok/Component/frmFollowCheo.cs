using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmFollowCheo : Form
	{
		private JavaScriptSerializer js = new JavaScriptSerializer();

		private IContainer components = null;

		private Button btnSave;

		private TextBox txtFile;

		private RadioButton rbtFollowList;

		private RadioButton rbtFollowRunning;

		private RadioButton rbtFollowRandom;

		private NumericUpDown nudNumber;

		private Label label1;

		private Button btnFile;

		private Label label2;

		private Label label3;

		private NumericUpDown nudDelay;

		private Label label4;

		public frmFollowCheo()
		{
			InitializeComponent();
		}

		private void btnFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Text File";
			openFileDialog.Filter = "TXT files|*.txt";
			openFileDialog.InitialDirectory = "C:\\";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtFile.Text = openFileDialog.FileName.ToString();
			}
		}

		private void frmFollowCheo_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.FOLLOW_CHEO))
			{
				FollowCheoEntity followCheoEntity = js.Deserialize<FollowCheoEntity>(Utils.ReadTextFile(CaChuaConstant.FOLLOW_CHEO));
				if (followCheoEntity != null)
				{
					nudDelay.Value = followCheoEntity.Delay;
					nudNumber.Value = followCheoEntity.Number;
					txtFile.Text = followCheoEntity.File;
					rbtFollowList.Checked = followCheoEntity.FollowType == FollowCheoType.File;
					rbtFollowRandom.Checked = followCheoEntity.FollowType == FollowCheoType.Full;
					rbtFollowRunning.Checked = followCheoEntity.FollowType == FollowCheoType.Selected;
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			FollowCheoEntity followCheoEntity = new FollowCheoEntity();
			followCheoEntity.Delay = Utils.Convert2Int(nudDelay.Value.ToString());
			followCheoEntity.Number = Utils.Convert2Int(nudNumber.Value.ToString());
			followCheoEntity.FollowType = (rbtFollowRandom.Checked ? FollowCheoType.Full : ((!rbtFollowRunning.Checked) ? FollowCheoType.File : FollowCheoType.Selected));
			if (followCheoEntity.FollowType != FollowCheoType.File)
			{
				followCheoEntity.File = "";
			}
			else
			{
				followCheoEntity.File = txtFile.Text;
			}
			File.WriteAllText(CaChuaConstant.FOLLOW_CHEO, js.Serialize(followCheoEntity));
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
			btnSave = new System.Windows.Forms.Button();
			txtFile = new System.Windows.Forms.TextBox();
			rbtFollowList = new System.Windows.Forms.RadioButton();
			rbtFollowRunning = new System.Windows.Forms.RadioButton();
			rbtFollowRandom = new System.Windows.Forms.RadioButton();
			nudNumber = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			btnFile = new System.Windows.Forms.Button();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudNumber).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			SuspendLayout();
			btnSave.Location = new System.Drawing.Point(281, 283);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(75, 23);
			btnSave.TabIndex = 0;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			txtFile.Location = new System.Drawing.Point(257, 114);
			txtFile.Name = "txtFile";
			txtFile.Size = new System.Drawing.Size(180, 20);
			txtFile.TabIndex = 1;
			rbtFollowList.AutoSize = true;
			rbtFollowList.Location = new System.Drawing.Point(105, 114);
			rbtFollowList.Name = "rbtFollowList";
			rbtFollowList.Size = new System.Drawing.Size(146, 17);
			rbtFollowList.TabIndex = 2;
			rbtFollowList.Text = "Follow theo danh sách ID";
			rbtFollowList.UseVisualStyleBackColor = true;
			rbtFollowRunning.AutoSize = true;
			rbtFollowRunning.Location = new System.Drawing.Point(105, 143);
			rbtFollowRunning.Name = "rbtFollowRunning";
			rbtFollowRunning.Size = new System.Drawing.Size(219, 17);
			rbtFollowRunning.TabIndex = 3;
			rbtFollowRunning.Text = "Follow chéo trong những nick đang chạy";
			rbtFollowRunning.UseVisualStyleBackColor = true;
			rbtFollowRandom.AutoSize = true;
			rbtFollowRandom.Checked = true;
			rbtFollowRandom.Location = new System.Drawing.Point(105, 173);
			rbtFollowRandom.Name = "rbtFollowRandom";
			rbtFollowRandom.Size = new System.Drawing.Size(213, 17);
			rbtFollowRandom.TabIndex = 3;
			rbtFollowRandom.TabStop = true;
			rbtFollowRandom.Text = "Follow ngẫu nhiên nick trong phần mềm";
			rbtFollowRandom.UseVisualStyleBackColor = true;
			nudNumber.Location = new System.Drawing.Point(281, 202);
			nudNumber.Name = "nudNumber";
			nudNumber.Size = new System.Drawing.Size(79, 20);
			nudNumber.TabIndex = 4;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(125, 204);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(150, 13);
			label1.TabIndex = 5;
			label1.Text = "Số lượng tài khoản cần Follow";
			btnFile.Location = new System.Drawing.Point(443, 112);
			btnFile.Name = "btnFile";
			btnFile.Size = new System.Drawing.Size(75, 23);
			btnFile.TabIndex = 0;
			btnFile.Text = "Chọn file";
			btnFile.UseVisualStyleBackColor = true;
			btnFile.Click += new System.EventHandler(btnFile_Click);
			label2.AutoSize = true;
			label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			label2.ForeColor = System.Drawing.Color.Red;
			label2.Location = new System.Drawing.Point(208, 47);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(200, 25);
			label2.TabIndex = 5;
			label2.Text = "Cấu hình Follow chéo";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(125, 230);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(148, 13);
			label3.TabIndex = 7;
			label3.Text = "Thời gian delay sau mỗi follow";
			nudDelay.Location = new System.Drawing.Point(281, 228);
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(79, 20);
			nudDelay.TabIndex = 6;
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(370, 230);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(26, 13);
			label4.TabIndex = 8;
			label4.Text = "giây";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(609, 340);
			base.Controls.Add(label4);
			base.Controls.Add(label3);
			base.Controls.Add(nudDelay);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(nudNumber);
			base.Controls.Add(rbtFollowRandom);
			base.Controls.Add(rbtFollowRunning);
			base.Controls.Add(rbtFollowList);
			base.Controls.Add(txtFile);
			base.Controls.Add(btnFile);
			base.Controls.Add(btnSave);
			base.Name = "frmFollowCheo";
			Text = "Follow Cheo";
			base.Load += new System.EventHandler(frmFollowCheo_Load);
			((System.ComponentModel.ISupportInitialize)nudNumber).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
