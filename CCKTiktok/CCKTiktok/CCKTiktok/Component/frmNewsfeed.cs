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
	public class frmNewsfeed : Form
	{
		private IContainer components = null;

		private TextBox txtComments;

		private Button btnSave;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label5;

		private NumericUpDown nudFrom;

		private NumericUpDown nudTo;

		private NumericUpDown nudLike;

		private NumericUpDown nudRate;

		private RadioButton rptSpin;

		private RadioButton rpt1line;

		private CheckBox cbxRemove;

		private Label label6;

		private TextBox txtShop;

		private Label label7;

		private Label label8;

		private Label label9;

		private NumericUpDown nudViewVideoFrom;

		private NumericUpDown nudViewVideoTo;

		private Label label10;

		private GroupBox groupBox1;

		private Label label13;

		private NumericUpDown nudFavoriteViewVideoTo;

		private Label label11;

		private Label label12;

		private NumericUpDown nudFavoriteViewVideoFrom;

		private CheckBox cbxFollowing;

		private CheckBox cbxForYou;

		private CheckBox cbxRepost;

		private NumericUpDown nudRepost;

		private ToolTip ttRepost;

		private CheckBox cbxFollow;

		private Label label14;

		private NumericUpDown nudFollowUpTo;

		private NumericUpDown nudFollowUpFrom;

		private Label label15;

		private Label label16;

		public frmNewsfeed()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			string nEWSFEED = CaChuaConstant.NEWSFEED;
			NewsFeedItem newsFeedItem = new NewsFeedItem();
			newsFeedItem.IsSpin = rptSpin.Checked;
			newsFeedItem.TimeFrom = Utils.Convert2Int(nudFrom.Text);
			newsFeedItem.TimeTo = Utils.Convert2Int(nudTo.Text);
			newsFeedItem.LikeCount = Utils.Convert2Int(nudLike.Text);
			newsFeedItem.Percent = Utils.Convert2Int(nudRate.Text);
			newsFeedItem.Following = cbxFollowing.Checked;
			newsFeedItem.ForYou = cbxForYou.Checked;
			newsFeedItem.RemoveComment = cbxRemove.Checked;
			newsFeedItem.Follow = cbxFollow.Checked;
			newsFeedItem.FollowFrom = (int)nudFollowUpFrom.Value;
			newsFeedItem.FollowTo = (int)nudFollowUpTo.Value;
			newsFeedItem.Repost = cbxRepost.Checked;
			newsFeedItem.PercentRepost = Convert.ToInt32(nudRepost.Value);
			newsFeedItem.ShopName = txtShop.Text.Trim();
			newsFeedItem.ViewVideoTimeFrom = Utils.Convert2Int(nudViewVideoFrom.Text);
			newsFeedItem.ViewVideoTimeTo = Utils.Convert2Int(nudViewVideoTo.Text);
			newsFeedItem.FavoriteViewVideoTimeFrom = Utils.Convert2Int(nudFavoriteViewVideoFrom.Text);
			newsFeedItem.FavoriteViewVideoTimeTo = Utils.Convert2Int(nudFavoriteViewVideoTo.Text);
			File.WriteAllText(CaChuaConstant.NEWSFEED_COMMENT, txtComments.Text);
			File.WriteAllText(nEWSFEED, new JavaScriptSerializer().Serialize(newsFeedItem));
			Close();
		}

		private void frmNewsfeed_Load(object sender, EventArgs e)
		{
			string nEWSFEED = CaChuaConstant.NEWSFEED;
			if (File.Exists(nEWSFEED))
			{
				NewsFeedItem newsFeedItem = new JavaScriptSerializer().Deserialize<NewsFeedItem>(Utils.ReadTextFile(nEWSFEED));
				rptSpin.Checked = newsFeedItem.IsSpin;
				rpt1line.Checked = !rptSpin.Checked;
				nudFrom.Value = newsFeedItem.TimeFrom;
				nudTo.Value = newsFeedItem.TimeTo;
				nudLike.Value = newsFeedItem.LikeCount;
				nudRate.Value = newsFeedItem.Percent;
				cbxForYou.Checked = newsFeedItem.ForYou;
				cbxFollowing.Checked = newsFeedItem.Following;
				nudViewVideoFrom.Value = newsFeedItem.ViewVideoTimeFrom;
				nudViewVideoTo.Value = newsFeedItem.ViewVideoTimeTo;
				txtShop.Text = newsFeedItem.ShopName;
				nudFavoriteViewVideoFrom.Value = newsFeedItem.FavoriteViewVideoTimeFrom;
				nudFavoriteViewVideoTo.Value = newsFeedItem.FavoriteViewVideoTimeTo;
				cbxRepost.Checked = newsFeedItem.Repost;
				nudRepost.Value = newsFeedItem.PercentRepost;
				txtComments.Text = Utils.ReadTextFile(CaChuaConstant.NEWSFEED_COMMENT);
				cbxRemove.Checked = newsFeedItem.RemoveComment;
				cbxFollow.Checked = newsFeedItem.Follow;
				nudFollowUpFrom.Value = newsFeedItem.FollowFrom;
				nudFollowUpTo.Value = newsFeedItem.FollowTo;
			}
		}

		private void rptSpin_CheckedChanged(object sender, EventArgs e)
		{
			cbxRemove.Checked = false;
			cbxRemove.Enabled = false;
		}

		private void rpt1line_CheckedChanged(object sender, EventArgs e)
		{
			cbxRemove.Enabled = true;
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
			components = new System.ComponentModel.Container();
			txtComments = new System.Windows.Forms.TextBox();
			btnSave = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			nudFrom = new System.Windows.Forms.NumericUpDown();
			nudTo = new System.Windows.Forms.NumericUpDown();
			nudLike = new System.Windows.Forms.NumericUpDown();
			nudRate = new System.Windows.Forms.NumericUpDown();
			rptSpin = new System.Windows.Forms.RadioButton();
			rpt1line = new System.Windows.Forms.RadioButton();
			cbxRemove = new System.Windows.Forms.CheckBox();
			label6 = new System.Windows.Forms.Label();
			txtShop = new System.Windows.Forms.TextBox();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			nudViewVideoFrom = new System.Windows.Forms.NumericUpDown();
			nudViewVideoTo = new System.Windows.Forms.NumericUpDown();
			label10 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label13 = new System.Windows.Forms.Label();
			nudFavoriteViewVideoTo = new System.Windows.Forms.NumericUpDown();
			label11 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			nudFavoriteViewVideoFrom = new System.Windows.Forms.NumericUpDown();
			cbxFollowing = new System.Windows.Forms.CheckBox();
			cbxForYou = new System.Windows.Forms.CheckBox();
			cbxRepost = new System.Windows.Forms.CheckBox();
			nudRepost = new System.Windows.Forms.NumericUpDown();
			ttRepost = new System.Windows.Forms.ToolTip(components);
			cbxFollow = new System.Windows.Forms.CheckBox();
			label14 = new System.Windows.Forms.Label();
			nudFollowUpTo = new System.Windows.Forms.NumericUpDown();
			nudFollowUpFrom = new System.Windows.Forms.NumericUpDown();
			label15 = new System.Windows.Forms.Label();
			label16 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudLike).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudRate).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudViewVideoFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudViewVideoTo).BeginInit();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudFavoriteViewVideoTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFavoriteViewVideoFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudRepost).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFollowUpTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFollowUpFrom).BeginInit();
			SuspendLayout();
			txtComments.Location = new System.Drawing.Point(145, 158);
			txtComments.Multiline = true;
			txtComments.Name = "txtComments";
			txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtComments.Size = new System.Drawing.Size(397, 136);
			txtComments.TabIndex = 6;
			btnSave.Location = new System.Drawing.Point(195, 498);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(155, 38);
			btnSave.TabIndex = 130;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(24, 66);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(112, 13);
			label1.TabIndex = 6;
			label1.Text = "Tổng  thời gian xem từ";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(228, 66);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(26, 13);
			label2.TabIndex = 7;
			label2.Text = "đến";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(64, 130);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(72, 13);
			label3.TabIndex = 8;
			label3.Text = "Số lượng Like";
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(221, 131);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(41, 13);
			label4.TabIndex = 9;
			label4.Text = "Tỷ lệ %";
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(24, 153);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(97, 13);
			label5.TabIndex = 10;
			label5.Text = "Nội dung Comment";
			nudFrom.Location = new System.Drawing.Point(145, 63);
			nudFrom.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudFrom.Name = "nudFrom";
			nudFrom.Size = new System.Drawing.Size(66, 20);
			nudFrom.TabIndex = 0;
			nudTo.Location = new System.Drawing.Point(269, 63);
			nudTo.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudTo.Name = "nudTo";
			nudTo.Size = new System.Drawing.Size(66, 20);
			nudTo.TabIndex = 1;
			nudLike.Location = new System.Drawing.Point(145, 127);
			nudLike.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudLike.Name = "nudLike";
			nudLike.Size = new System.Drawing.Size(66, 20);
			nudLike.TabIndex = 4;
			nudRate.Location = new System.Drawing.Point(268, 127);
			nudRate.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudRate.Name = "nudRate";
			nudRate.Size = new System.Drawing.Size(66, 20);
			nudRate.TabIndex = 5;
			rptSpin.AutoSize = true;
			rptSpin.Checked = true;
			rptSpin.Location = new System.Drawing.Point(139, 310);
			rptSpin.Name = "rptSpin";
			rptSpin.Size = new System.Drawing.Size(90, 17);
			rptSpin.TabIndex = 7;
			rptSpin.TabStop = true;
			rptSpin.Text = "Spin nội dung";
			rptSpin.UseVisualStyleBackColor = true;
			rptSpin.CheckedChanged += new System.EventHandler(rptSpin_CheckedChanged);
			rpt1line.AutoSize = true;
			rpt1line.Location = new System.Drawing.Point(243, 310);
			rpt1line.Name = "rpt1line";
			rpt1line.Size = new System.Drawing.Size(124, 17);
			rpt1line.TabIndex = 8;
			rpt1line.Text = "Mỗi bình luận 1 dòng";
			rpt1line.UseVisualStyleBackColor = true;
			rpt1line.CheckedChanged += new System.EventHandler(rpt1line_CheckedChanged);
			cbxRemove.AutoSize = true;
			cbxRemove.Location = new System.Drawing.Point(387, 310);
			cbxRemove.Name = "cbxRemove";
			cbxRemove.Size = new System.Drawing.Size(155, 17);
			cbxRemove.TabIndex = 9;
			cbxRemove.Text = "Xóa bình luận sau khi dùng";
			cbxRemove.UseVisualStyleBackColor = true;
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(37, 30);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(75, 13);
			label6.TabIndex = 16;
			label6.Text = "Shop ưa thích";
			txtShop.Location = new System.Drawing.Point(121, 27);
			txtShop.Name = "txtShop";
			txtShop.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtShop.Size = new System.Drawing.Size(375, 20);
			txtShop.TabIndex = 10;
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(341, 66);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(26, 13);
			label7.TabIndex = 18;
			label7.Text = "giây";
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(47, 98);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(89, 13);
			label8.TabIndex = 6;
			label8.Text = "Xem mỗi Video từ";
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(228, 98);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(26, 13);
			label9.TabIndex = 7;
			label9.Text = "đến";
			nudViewVideoFrom.Location = new System.Drawing.Point(145, 95);
			nudViewVideoFrom.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudViewVideoFrom.Name = "nudViewVideoFrom";
			nudViewVideoFrom.Size = new System.Drawing.Size(66, 20);
			nudViewVideoFrom.TabIndex = 2;
			nudViewVideoTo.Location = new System.Drawing.Point(269, 95);
			nudViewVideoTo.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudViewVideoTo.Name = "nudViewVideoTo";
			nudViewVideoTo.Size = new System.Drawing.Size(66, 20);
			nudViewVideoTo.TabIndex = 3;
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(341, 98);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(26, 13);
			label10.TabIndex = 18;
			label10.Text = "giây";
			groupBox1.Controls.Add(label6);
			groupBox1.Controls.Add(label13);
			groupBox1.Controls.Add(txtShop);
			groupBox1.Controls.Add(nudFavoriteViewVideoTo);
			groupBox1.Controls.Add(label11);
			groupBox1.Controls.Add(label12);
			groupBox1.Controls.Add(nudFavoriteViewVideoFrom);
			groupBox1.Location = new System.Drawing.Point(27, 374);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(515, 100);
			groupBox1.TabIndex = 19;
			groupBox1.TabStop = false;
			groupBox1.Text = "Shop yêu thích";
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(325, 60);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(26, 13);
			label13.TabIndex = 18;
			label13.Text = "giây";
			nudFavoriteViewVideoTo.Location = new System.Drawing.Point(244, 56);
			nudFavoriteViewVideoTo.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudFavoriteViewVideoTo.Name = "nudFavoriteViewVideoTo";
			nudFavoriteViewVideoTo.Size = new System.Drawing.Size(66, 20);
			nudFavoriteViewVideoTo.TabIndex = 12;
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(22, 59);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(89, 13);
			label11.TabIndex = 6;
			label11.Text = "Xem mỗi Video từ";
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(203, 59);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(26, 13);
			label12.TabIndex = 7;
			label12.Text = "đến";
			nudFavoriteViewVideoFrom.Location = new System.Drawing.Point(120, 56);
			nudFavoriteViewVideoFrom.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudFavoriteViewVideoFrom.Name = "nudFavoriteViewVideoFrom";
			nudFavoriteViewVideoFrom.Size = new System.Drawing.Size(66, 20);
			nudFavoriteViewVideoFrom.TabIndex = 11;
			cbxFollowing.AutoSize = true;
			cbxFollowing.Enabled = false;
			cbxFollowing.Location = new System.Drawing.Point(145, 28);
			cbxFollowing.Name = "cbxFollowing";
			cbxFollowing.Size = new System.Drawing.Size(70, 17);
			cbxFollowing.TabIndex = 131;
			cbxFollowing.Text = "Following";
			cbxFollowing.UseVisualStyleBackColor = true;
			cbxForYou.AutoSize = true;
			cbxForYou.Checked = true;
			cbxForYou.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxForYou.Location = new System.Drawing.Point(264, 28);
			cbxForYou.Name = "cbxForYou";
			cbxForYou.Size = new System.Drawing.Size(63, 17);
			cbxForYou.TabIndex = 132;
			cbxForYou.Text = "For You";
			cbxForYou.UseVisualStyleBackColor = true;
			cbxRepost.AutoSize = true;
			cbxRepost.Location = new System.Drawing.Point(402, 65);
			cbxRepost.Name = "cbxRepost";
			cbxRepost.Size = new System.Drawing.Size(97, 17);
			cbxRepost.TabIndex = 133;
			cbxRepost.Text = "Repost Tỷ lệ %";
			ttRepost.SetToolTip(cbxRepost, "Tỷ lệ phần trăm bài viết sẽ Repost lên tường cá nhân khi lướt tường");
			cbxRepost.UseVisualStyleBackColor = true;
			nudRepost.Location = new System.Drawing.Point(502, 63);
			nudRepost.Name = "nudRepost";
			nudRepost.Size = new System.Drawing.Size(40, 20);
			nudRepost.TabIndex = 4;
			cbxFollow.AutoSize = true;
			cbxFollow.Enabled = false;
			cbxFollow.Location = new System.Drawing.Point(139, 342);
			cbxFollow.Name = "cbxFollow";
			cbxFollow.Size = new System.Drawing.Size(99, 17);
			cbxFollow.TabIndex = 134;
			cbxFollow.Text = "Random Follow";
			cbxFollow.UseVisualStyleBackColor = true;
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(508, 345);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(33, 13);
			label14.TabIndex = 138;
			label14.Text = "người";
			nudFollowUpTo.Location = new System.Drawing.Point(436, 342);
			nudFollowUpTo.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudFollowUpTo.Name = "nudFollowUpTo";
			nudFollowUpTo.Size = new System.Drawing.Size(66, 20);
			nudFollowUpTo.TabIndex = 136;
			nudFollowUpFrom.Location = new System.Drawing.Point(312, 342);
			nudFollowUpFrom.Maximum = new decimal(new int[4] { 10000000, 0, 0, 0 });
			nudFollowUpFrom.Name = "nudFollowUpFrom";
			nudFollowUpFrom.Size = new System.Drawing.Size(66, 20);
			nudFollowUpFrom.TabIndex = 135;
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(395, 345);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(26, 13);
			label15.TabIndex = 137;
			label15.Text = "đến";
			label16.AutoSize = true;
			label16.Location = new System.Drawing.Point(280, 344);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(20, 13);
			label16.TabIndex = 139;
			label16.Text = "Từ";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(570, 564);
			base.Controls.Add(label16);
			base.Controls.Add(label14);
			base.Controls.Add(nudFollowUpTo);
			base.Controls.Add(nudFollowUpFrom);
			base.Controls.Add(label15);
			base.Controls.Add(cbxFollow);
			base.Controls.Add(cbxRepost);
			base.Controls.Add(cbxForYou);
			base.Controls.Add(cbxFollowing);
			base.Controls.Add(groupBox1);
			base.Controls.Add(label10);
			base.Controls.Add(label7);
			base.Controls.Add(cbxRemove);
			base.Controls.Add(rpt1line);
			base.Controls.Add(rptSpin);
			base.Controls.Add(nudRate);
			base.Controls.Add(nudRepost);
			base.Controls.Add(nudLike);
			base.Controls.Add(nudViewVideoTo);
			base.Controls.Add(nudTo);
			base.Controls.Add(nudViewVideoFrom);
			base.Controls.Add(nudFrom);
			base.Controls.Add(label5);
			base.Controls.Add(label4);
			base.Controls.Add(label3);
			base.Controls.Add(label9);
			base.Controls.Add(label2);
			base.Controls.Add(label8);
			base.Controls.Add(label1);
			base.Controls.Add(btnSave);
			base.Controls.Add(txtComments);
			base.Name = "frmNewsfeed";
			Text = "Cấu hình Newsfeed";
			base.Load += new System.EventHandler(frmNewsfeed_Load);
			((System.ComponentModel.ISupportInitialize)nudFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudLike).EndInit();
			((System.ComponentModel.ISupportInitialize)nudRate).EndInit();
			((System.ComponentModel.ISupportInitialize)nudViewVideoFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudViewVideoTo).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudFavoriteViewVideoTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFavoriteViewVideoFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudRepost).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFollowUpTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFollowUpFrom).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
