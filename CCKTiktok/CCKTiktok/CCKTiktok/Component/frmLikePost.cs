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
	public class frmLikePost : Form
	{
		private IContainer components = null;

		private Button btnSave;

		private TextBox txtLinks;

		private NumericUpDown nudLike;

		private NumericUpDown nudDelay;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label5;

		private NumericUpDown nudDelayView;

		private NumericUpDown nudViewCount;

		private Label label6;

		private Label label7;

		private CheckBox cbxLike;

		private CheckBox cbxViewProduct;

		private NumericUpDown nudShopDelay;

		private Label label8;

		private Label label9;

		public frmLikePost()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			LikePostItem likePostItem = new LikePostItem();
			likePostItem.ListPost = txtLinks.Text;
			likePostItem.LikeCount = Convert.ToInt32(nudLike.Value);
			likePostItem.Delay = Convert.ToInt32(nudDelay.Value);
			likePostItem.ViewCount = Convert.ToInt32(nudViewCount.Value);
			likePostItem.ViewDelay = Convert.ToInt32(nudDelayView.Value);
			likePostItem.ViewProductDelay = Convert.ToInt32(nudShopDelay.Value);
			likePostItem.ViewProduct = cbxViewProduct.Checked;
			likePostItem.ViewOnly = cbxLike.Checked;
			File.WriteAllText(CaChuaConstant.LIKE_POST, new JavaScriptSerializer().Serialize(likePostItem));
			Close();
		}

		private void frmLikePost_Load(object sender, EventArgs e)
		{
			string text = Utils.ReadTextFile(CaChuaConstant.LIKE_POST);
			if (!string.IsNullOrWhiteSpace(text))
			{
				LikePostItem likePostItem = new LikePostItem();
				try
				{
					likePostItem = new JavaScriptSerializer().Deserialize<LikePostItem>(text);
					txtLinks.Text = likePostItem.ListPost;
					nudLike.Value = likePostItem.LikeCount;
					nudDelay.Value = likePostItem.Delay;
					nudDelayView.Value = likePostItem.ViewDelay;
					nudViewCount.Value = likePostItem.ViewCount;
					nudShopDelay.Value = likePostItem.ViewProductDelay;
					cbxViewProduct.Checked = likePostItem.ViewProduct;
					cbxLike.Checked = likePostItem.ViewOnly;
				}
				catch
				{
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
			btnSave = new System.Windows.Forms.Button();
			txtLinks = new System.Windows.Forms.TextBox();
			nudLike = new System.Windows.Forms.NumericUpDown();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			nudDelayView = new System.Windows.Forms.NumericUpDown();
			nudViewCount = new System.Windows.Forms.NumericUpDown();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			cbxLike = new System.Windows.Forms.CheckBox();
			cbxViewProduct = new System.Windows.Forms.CheckBox();
			nudShopDelay = new System.Windows.Forms.NumericUpDown();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudLike).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayView).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudViewCount).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudShopDelay).BeginInit();
			SuspendLayout();
			btnSave.Location = new System.Drawing.Point(252, 330);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(115, 35);
			btnSave.TabIndex = 0;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			txtLinks.Location = new System.Drawing.Point(12, 44);
			txtLinks.Multiline = true;
			txtLinks.Name = "txtLinks";
			txtLinks.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			txtLinks.Size = new System.Drawing.Size(584, 119);
			txtLinks.TabIndex = 1;
			nudLike.Location = new System.Drawing.Point(182, 181);
			nudLike.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudLike.Name = "nudLike";
			nudLike.Size = new System.Drawing.Size(67, 20);
			nudLike.TabIndex = 2;
			nudLike.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			nudDelay.Location = new System.Drawing.Point(389, 183);
			nudDelay.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(67, 20);
			nudDelay.TabIndex = 3;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(104, 183);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(72, 13);
			label1.TabIndex = 4;
			label1.Text = "Số lượng Like";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(339, 185);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(34, 13);
			label2.TabIndex = 5;
			label2.Text = "Delay";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 15);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(157, 13);
			label3.TabIndex = 6;
			label3.Text = "Danh sách Link, mỗi bài 1 dòng";
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(339, 213);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(34, 13);
			label4.TabIndex = 10;
			label4.Text = "Delay";
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(68, 213);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(101, 13);
			label5.TabIndex = 9;
			label5.Text = "Tym xong xem thêm";
			nudDelayView.Location = new System.Drawing.Point(389, 211);
			nudDelayView.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudDelayView.Name = "nudDelayView";
			nudDelayView.Size = new System.Drawing.Size(67, 20);
			nudDelayView.TabIndex = 8;
			nudDelayView.Value = new decimal(new int[4] { 10, 0, 0, 0 });
			nudViewCount.Location = new System.Drawing.Point(182, 209);
			nudViewCount.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudViewCount.Name = "nudViewCount";
			nudViewCount.Size = new System.Drawing.Size(67, 20);
			nudViewCount.TabIndex = 7;
			nudViewCount.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(255, 213);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(21, 13);
			label6.TabIndex = 11;
			label6.Text = "lần";
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(462, 216);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(26, 13);
			label7.TabIndex = 12;
			label7.Text = "giây";
			cbxLike.AutoSize = true;
			cbxLike.Location = new System.Drawing.Point(182, 264);
			cbxLike.Name = "cbxLike";
			cbxLike.Size = new System.Drawing.Size(115, 17);
			cbxLike.TabIndex = 13;
			cbxLike.Text = "Chỉ xem không tym";
			cbxLike.UseVisualStyleBackColor = true;
			cbxViewProduct.AutoSize = true;
			cbxViewProduct.Location = new System.Drawing.Point(182, 240);
			cbxViewProduct.Name = "cbxViewProduct";
			cbxViewProduct.Size = new System.Drawing.Size(144, 17);
			cbxViewProduct.TabIndex = 14;
			cbxViewProduct.Text = "Lướt xem sản phẩm shop";
			cbxViewProduct.UseVisualStyleBackColor = true;
			nudShopDelay.Location = new System.Drawing.Point(389, 237);
			nudShopDelay.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudShopDelay.Name = "nudShopDelay";
			nudShopDelay.Size = new System.Drawing.Size(67, 20);
			nudShopDelay.TabIndex = 15;
			nudShopDelay.Value = new decimal(new int[4] { 10, 0, 0, 0 });
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(340, 240);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(34, 13);
			label8.TabIndex = 16;
			label8.Text = "Delay";
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(462, 240);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(26, 13);
			label9.TabIndex = 17;
			label9.Text = "giây";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(618, 392);
			base.Controls.Add(label9);
			base.Controls.Add(label8);
			base.Controls.Add(nudShopDelay);
			base.Controls.Add(cbxViewProduct);
			base.Controls.Add(cbxLike);
			base.Controls.Add(label7);
			base.Controls.Add(label6);
			base.Controls.Add(label4);
			base.Controls.Add(label5);
			base.Controls.Add(nudDelayView);
			base.Controls.Add(nudViewCount);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(nudDelay);
			base.Controls.Add(nudLike);
			base.Controls.Add(txtLinks);
			base.Controls.Add(btnSave);
			base.Name = "frmLikePost";
			Text = "Like Post";
			base.Load += new System.EventHandler(frmLikePost_Load);
			((System.ComponentModel.ISupportInitialize)nudLike).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayView).EndInit();
			((System.ComponentModel.ISupportInitialize)nudViewCount).EndInit();
			((System.ComponentModel.ISupportInitialize)nudShopDelay).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
