using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmKeyword : Form
	{
		private IContainer components = null;

		private CheckBox cbxTym;

		private Button btnSave;

		private Label label4;

		private NumericUpDown nudViewVideoTo;

		private Label label3;

		private NumericUpDown nudViewVideoFrom;

		private Label label1;

		private TextBox txtKeyword;

		private Label label2;

		private Label label5;

		private CheckBox cbxFavorite;

		private CheckBox cbxFollowShop;

		private NumericUpDown nudFollowFrom;

		private NumericUpDown nudFollowTo;

		private Label label6;

		private Label label7;

		private Label label8;

		private TextBox txtComment;

		private Label label9;

		private Label label10;

		private NumericUpDown nudCommentFrom;

		private NumericUpDown nudCommentTo;

		private Label label11;

		private Label label12;

		private Label label13;

		private NumericUpDown nudCommentDelay;

		private NumericUpDown nudFollowDelay;

		private CheckBox cbxCommentRemove;

		private RadioButton rbtKeyword;

		private RadioButton rbtShopname;

		private CheckBox cbxRepost;

		private Label label14;

		private NumericUpDown nudDelaySearch;

		private Label label15;

		private RadioButton rbtUser;

		public frmKeyword()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			ViewByKeyword viewByKeyword = new ViewByKeyword();
			viewByKeyword.Keyword = txtKeyword.Text;
			viewByKeyword.Repost = cbxRepost.Checked;
			viewByKeyword.Type = (rbtKeyword.Checked ? ViewByKeyword_Type.Keyword : (rbtShopname.Checked ? ViewByKeyword_Type.Shop : ViewByKeyword_Type.User));
			viewByKeyword.FollowShop = cbxFollowShop.Checked;
			viewByKeyword.FollowFrom = Convert.ToInt32(nudFollowFrom.Value);
			viewByKeyword.FollowTo = Convert.ToInt32(nudFollowTo.Value);
			viewByKeyword.FollowDelay = Convert.ToInt32(nudFollowDelay.Value);
			viewByKeyword.ViewVideoFrom = Convert.ToInt32(nudViewVideoFrom.Value);
			viewByKeyword.ViewVideoTo = Convert.ToInt32(nudViewVideoTo.Value);
			viewByKeyword.RemoveComment = cbxCommentRemove.Checked;
			viewByKeyword.IsTym = cbxTym.Checked;
			viewByKeyword.IsFavorite = cbxFavorite.Checked;
			viewByKeyword.SuggestionDelay = Convert.ToInt32(nudDelaySearch.Value);
			viewByKeyword.CommentFrom = Convert.ToInt32(nudCommentFrom.Value);
			viewByKeyword.CommentTo = Convert.ToInt32(nudCommentTo.Value);
			viewByKeyword.CommentDelay = Convert.ToInt32(nudCommentDelay.Value);
			File.WriteAllText(CaChuaConstant.VIEW_BY_KEYWORD_COMMENT, txtComment.Text);
			File.WriteAllText(CaChuaConstant.VIEW_BY_KEYWORD, new JavaScriptSerializer().Serialize(viewByKeyword));
			Close();
		}

		private void frmKeyword_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.VIEW_BY_KEYWORD))
			{
				ViewByKeyword viewByKeyword = new JavaScriptSerializer().Deserialize<ViewByKeyword>(Utils.ReadTextFile(CaChuaConstant.VIEW_BY_KEYWORD));
				txtKeyword.Text = viewByKeyword.Keyword;
				cbxFollowShop.Checked = viewByKeyword.FollowShop;
				nudFollowFrom.Value = viewByKeyword.FollowFrom;
				nudFollowTo.Value = viewByKeyword.FollowTo;
				nudFollowDelay.Value = viewByKeyword.FollowDelay;
				nudDelaySearch.Value = viewByKeyword.SuggestionDelay;
				nudViewVideoFrom.Value = viewByKeyword.ViewVideoFrom;
				cbxRepost.Checked = viewByKeyword.Repost;
				nudViewVideoTo.Value = viewByKeyword.ViewVideoTo;
				cbxCommentRemove.Checked = viewByKeyword.RemoveComment;
				cbxTym.Checked = viewByKeyword.IsTym;
				cbxFavorite.Checked = viewByKeyword.IsFavorite;
				txtComment.Text = Utils.ReadTextFile(CaChuaConstant.VIEW_BY_KEYWORD_COMMENT);
				nudCommentFrom.Value = viewByKeyword.CommentFrom;
				nudCommentTo.Value = viewByKeyword.CommentTo;
				nudCommentDelay.Value = viewByKeyword.CommentDelay;
				rbtShopname.Checked = viewByKeyword.Type == ViewByKeyword_Type.Shop;
				rbtKeyword.Checked = viewByKeyword.Type == ViewByKeyword_Type.Keyword;
				rbtUser.Checked = viewByKeyword.Type == ViewByKeyword_Type.User;
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
			cbxTym = new System.Windows.Forms.CheckBox();
			btnSave = new System.Windows.Forms.Button();
			label4 = new System.Windows.Forms.Label();
			nudViewVideoTo = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			nudViewVideoFrom = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			txtKeyword = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			cbxFavorite = new System.Windows.Forms.CheckBox();
			cbxFollowShop = new System.Windows.Forms.CheckBox();
			nudFollowFrom = new System.Windows.Forms.NumericUpDown();
			nudFollowTo = new System.Windows.Forms.NumericUpDown();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			txtComment = new System.Windows.Forms.TextBox();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			nudCommentFrom = new System.Windows.Forms.NumericUpDown();
			nudCommentTo = new System.Windows.Forms.NumericUpDown();
			label11 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			label13 = new System.Windows.Forms.Label();
			nudCommentDelay = new System.Windows.Forms.NumericUpDown();
			nudFollowDelay = new System.Windows.Forms.NumericUpDown();
			cbxCommentRemove = new System.Windows.Forms.CheckBox();
			rbtKeyword = new System.Windows.Forms.RadioButton();
			rbtShopname = new System.Windows.Forms.RadioButton();
			cbxRepost = new System.Windows.Forms.CheckBox();
			label14 = new System.Windows.Forms.Label();
			nudDelaySearch = new System.Windows.Forms.NumericUpDown();
			label15 = new System.Windows.Forms.Label();
			rbtUser = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)nudViewVideoTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudViewVideoFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFollowFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFollowTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudCommentFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudCommentTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudCommentDelay).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFollowDelay).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelaySearch).BeginInit();
			SuspendLayout();
			cbxTym.AutoSize = true;
			cbxTym.Location = new System.Drawing.Point(125, 205);
			cbxTym.Name = "cbxTym";
			cbxTym.Size = new System.Drawing.Size(64, 17);
			cbxTym.TabIndex = 24;
			cbxTym.Text = "Thả tym";
			cbxTym.UseVisualStyleBackColor = true;
			btnSave.Location = new System.Drawing.Point(244, 506);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(107, 33);
			btnSave.TabIndex = 23;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(209, 164);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(26, 13);
			label4.TabIndex = 22;
			label4.Text = "đến";
			nudViewVideoTo.Location = new System.Drawing.Point(253, 164);
			nudViewVideoTo.Maximum = new decimal(new int[4] { 99999, 0, 0, 0 });
			nudViewVideoTo.Name = "nudViewVideoTo";
			nudViewVideoTo.Size = new System.Drawing.Size(53, 20);
			nudViewVideoTo.TabIndex = 21;
			nudViewVideoTo.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(32, 166);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(69, 13);
			label3.TabIndex = 20;
			label3.Text = "Xem video từ";
			nudViewVideoFrom.Location = new System.Drawing.Point(125, 162);
			nudViewVideoFrom.Maximum = new decimal(new int[4] { 99999, 0, 0, 0 });
			nudViewVideoFrom.Name = "nudViewVideoFrom";
			nudViewVideoFrom.Size = new System.Drawing.Size(56, 20);
			nudViewVideoFrom.TabIndex = 19;
			nudViewVideoFrom.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(122, 103);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(182, 13);
			label1.TabIndex = 18;
			label1.Text = "Tìm kênh theo từ khóa hoặc hashtag";
			txtKeyword.Location = new System.Drawing.Point(123, 74);
			txtKeyword.MaxLength = 32767000;
			txtKeyword.Name = "txtKeyword";
			txtKeyword.Size = new System.Drawing.Size(410, 20);
			txtKeyword.TabIndex = 17;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(50, 76);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(49, 13);
			label2.TabIndex = 25;
			label2.Text = "Tìm kiếm";
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(325, 166);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(26, 13);
			label5.TabIndex = 26;
			label5.Text = "giây";
			cbxFavorite.AutoSize = true;
			cbxFavorite.Location = new System.Drawing.Point(246, 205);
			cbxFavorite.Name = "cbxFavorite";
			cbxFavorite.Size = new System.Drawing.Size(73, 17);
			cbxFavorite.TabIndex = 27;
			cbxFavorite.Text = "Yêu thích";
			cbxFavorite.UseVisualStyleBackColor = true;
			cbxFollowShop.AutoSize = true;
			cbxFollowShop.Location = new System.Drawing.Point(127, 238);
			cbxFollowShop.Name = "cbxFollowShop";
			cbxFollowShop.Size = new System.Drawing.Size(84, 17);
			cbxFollowShop.TabIndex = 28;
			cbxFollowShop.Text = "Follow Shop";
			cbxFollowShop.UseVisualStyleBackColor = true;
			nudFollowFrom.Location = new System.Drawing.Point(253, 237);
			nudFollowFrom.Maximum = new decimal(new int[4] { 99999, 0, 0, 0 });
			nudFollowFrom.Name = "nudFollowFrom";
			nudFollowFrom.Size = new System.Drawing.Size(56, 20);
			nudFollowFrom.TabIndex = 19;
			nudFollowFrom.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			nudFollowTo.Location = new System.Drawing.Point(347, 237);
			nudFollowTo.Name = "nudFollowTo";
			nudFollowTo.Size = new System.Drawing.Size(53, 20);
			nudFollowTo.TabIndex = 21;
			nudFollowTo.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(315, 240);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(26, 13);
			label6.TabIndex = 22;
			label6.Text = "đến";
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(411, 240);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(62, 13);
			label7.TabIndex = 26;
			label7.Text = "Shop Delay";
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(227, 240);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(20, 13);
			label8.TabIndex = 29;
			label8.Text = "Từ";
			txtComment.Location = new System.Drawing.Point(125, 277);
			txtComment.MaxLength = 32767000;
			txtComment.Multiline = true;
			txtComment.Name = "txtComment";
			txtComment.Size = new System.Drawing.Size(318, 128);
			txtComment.TabIndex = 30;
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(14, 277);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(94, 13);
			label9.TabIndex = 31;
			label9.Text = "Random Comment";
			label10.AutoSize = true;
			label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			label10.Location = new System.Drawing.Point(151, 29);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(173, 20);
			label10.TabIndex = 32;
			label10.Text = "Tương tác theo từ khóa";
			nudCommentFrom.Location = new System.Drawing.Point(200, 415);
			nudCommentFrom.Name = "nudCommentFrom";
			nudCommentFrom.Size = new System.Drawing.Size(56, 20);
			nudCommentFrom.TabIndex = 19;
			nudCommentFrom.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			nudCommentTo.Location = new System.Drawing.Point(298, 415);
			nudCommentTo.Name = "nudCommentTo";
			nudCommentTo.Size = new System.Drawing.Size(53, 20);
			nudCommentTo.TabIndex = 21;
			nudCommentTo.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(266, 418);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(26, 13);
			label11.TabIndex = 22;
			label11.Text = "đến";
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(362, 418);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(78, 13);
			label12.TabIndex = 26;
			label12.Text = "comment delay";
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(126, 417);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(63, 13);
			label13.TabIndex = 29;
			label13.Text = "Random Từ";
			nudCommentDelay.Location = new System.Drawing.Point(446, 416);
			nudCommentDelay.Name = "nudCommentDelay";
			nudCommentDelay.Size = new System.Drawing.Size(56, 20);
			nudCommentDelay.TabIndex = 33;
			nudCommentDelay.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			nudFollowDelay.Location = new System.Drawing.Point(479, 238);
			nudFollowDelay.Name = "nudFollowDelay";
			nudFollowDelay.Size = new System.Drawing.Size(56, 20);
			nudFollowDelay.TabIndex = 34;
			nudFollowDelay.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			cbxCommentRemove.AutoSize = true;
			cbxCommentRemove.Location = new System.Drawing.Point(126, 446);
			cbxCommentRemove.Name = "cbxCommentRemove";
			cbxCommentRemove.Size = new System.Drawing.Size(116, 17);
			cbxCommentRemove.TabIndex = 35;
			cbxCommentRemove.Text = "Comment xong xóa";
			cbxCommentRemove.UseVisualStyleBackColor = true;
			rbtKeyword.AutoSize = true;
			rbtKeyword.Checked = true;
			rbtKeyword.Location = new System.Drawing.Point(123, 129);
			rbtKeyword.Name = "rbtKeyword";
			rbtKeyword.Size = new System.Drawing.Size(65, 17);
			rbtKeyword.TabIndex = 36;
			rbtKeyword.TabStop = true;
			rbtKeyword.Text = "Từ khóa";
			rbtKeyword.UseVisualStyleBackColor = true;
			rbtShopname.AutoSize = true;
			rbtShopname.Location = new System.Drawing.Point(190, 129);
			rbtShopname.Name = "rbtShopname";
			rbtShopname.Size = new System.Drawing.Size(70, 17);
			rbtShopname.TabIndex = 37;
			rbtShopname.Text = "Tên shop";
			rbtShopname.UseVisualStyleBackColor = true;
			cbxRepost.AutoSize = true;
			cbxRepost.Location = new System.Drawing.Point(347, 205);
			cbxRepost.Name = "cbxRepost";
			cbxRepost.Size = new System.Drawing.Size(60, 17);
			cbxRepost.TabIndex = 38;
			cbxRepost.Text = "Repost";
			cbxRepost.UseVisualStyleBackColor = true;
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(355, 132);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(118, 13);
			label14.TabIndex = 40;
			label14.Text = "Nhập từ khóa chờ gợi ý";
			nudDelaySearch.Location = new System.Drawing.Point(479, 129);
			nudDelaySearch.Name = "nudDelaySearch";
			nudDelaySearch.Size = new System.Drawing.Size(56, 20);
			nudDelaySearch.TabIndex = 39;
			nudDelaySearch.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(541, 131);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(26, 13);
			label15.TabIndex = 26;
			label15.Text = "giây";
			rbtUser.AutoSize = true;
			rbtUser.Location = new System.Drawing.Point(266, 129);
			rbtUser.Name = "rbtUser";
			rbtUser.Size = new System.Drawing.Size(52, 17);
			rbtUser.TabIndex = 41;
			rbtUser.Text = "Users";
			rbtUser.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(607, 579);
			base.Controls.Add(rbtUser);
			base.Controls.Add(label14);
			base.Controls.Add(nudDelaySearch);
			base.Controls.Add(cbxRepost);
			base.Controls.Add(rbtShopname);
			base.Controls.Add(rbtKeyword);
			base.Controls.Add(cbxCommentRemove);
			base.Controls.Add(nudFollowDelay);
			base.Controls.Add(nudCommentDelay);
			base.Controls.Add(label10);
			base.Controls.Add(label9);
			base.Controls.Add(txtComment);
			base.Controls.Add(label13);
			base.Controls.Add(label8);
			base.Controls.Add(cbxFollowShop);
			base.Controls.Add(cbxFavorite);
			base.Controls.Add(label12);
			base.Controls.Add(label7);
			base.Controls.Add(label15);
			base.Controls.Add(label5);
			base.Controls.Add(label2);
			base.Controls.Add(cbxTym);
			base.Controls.Add(btnSave);
			base.Controls.Add(label11);
			base.Controls.Add(label6);
			base.Controls.Add(nudCommentTo);
			base.Controls.Add(label4);
			base.Controls.Add(nudFollowTo);
			base.Controls.Add(nudCommentFrom);
			base.Controls.Add(nudViewVideoTo);
			base.Controls.Add(nudFollowFrom);
			base.Controls.Add(label3);
			base.Controls.Add(nudViewVideoFrom);
			base.Controls.Add(label1);
			base.Controls.Add(txtKeyword);
			base.Name = "frmKeyword";
			Text = "Tương tác theo từ khóa";
			base.Load += new System.EventHandler(frmKeyword_Load);
			((System.ComponentModel.ISupportInitialize)nudViewVideoTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudViewVideoFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFollowFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFollowTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudCommentFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudCommentTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudCommentDelay).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFollowDelay).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelaySearch).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
