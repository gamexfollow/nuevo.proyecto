using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.Entity;

namespace CCKTiktok.Component
{
	public class frmSeedingLive : Form
	{
		private IContainer components = null;

		private Label label1;

		private NumericUpDown nudFrom;

		private Label label2;

		private NumericUpDown nudTo;

		private TextBox txtLink;

		private Label label4;

		private NumericUpDown nudClickFrom;

		private Label label5;

		private NumericUpDown nudClickTo;

		private TextBox txtComment;

		private Label label6;

		private Button btnSave;

		private NumericUpDown nudRepeat;

		private Label label7;

		private Label label8;

		private NumericUpDown nudDelayFrom;

		private Label label9;

		private NumericUpDown nudDelayTo;

		private Label label10;

		private Label label11;

		private NumericUpDown nudShareFollow;

		private CheckBox cbxShop;

		private CheckBox cbxProduct;

		private Label label12;

		private NumericUpDown nudTymRepeat;

		private TextBox txtChannelName;

		private RadioButton rbtLinkKenh;

		private RadioButton rbtInbox;

		private CheckBox cbxRemoveAfterUse;

		private TextBox txtKeyword;

		private RadioButton rbtSearch;

		private Label label3;

		private Button button1;

		private CheckBox cbxCopy;

		private CheckBox cbxFollowShop;

		private Label label13;

		private LinkLabel linkLabel1;

		private NumericUpDown nudDelayLiveView;

		private Label label14;

		private Label label15;

		private CheckBox cbxGhim;

		private NumericUpDown nudDelayFromGim;

		private Label label16;

		private NumericUpDown nudDelayToGim;

		private Label label17;

		public frmSeedingLive()
		{
			InitializeComponent();
		}

		private void frmSeedingLive_Load(object sender, EventArgs e)
		{
			txtComment.Text = Utils.ReadTextFile(CaChuaConstant.SEEDING_LIVE_COMMENT);
			if (File.Exists(CaChuaConstant.SEEDING_LIVE_CONFIG))
			{
				LiveConfigEntity liveConfigEntity = new JavaScriptSerializer().Deserialize<LiveConfigEntity>(Utils.ReadTextFile(CaChuaConstant.SEEDING_LIVE_CONFIG));
				if (liveConfigEntity != null)
				{
					txtLink.Text = liveConfigEntity.Link;
					nudDelayFrom.Value = liveConfigEntity.CommentDelayFrom;
					nudDelayTo.Value = liveConfigEntity.CommentDelayTo;
					nudFrom.Value = liveConfigEntity.TimeFrom;
					nudTo.Value = liveConfigEntity.TimeTo;
					cbxProduct.Checked = liveConfigEntity.ViewProduct;
					cbxShop.Checked = liveConfigEntity.ViewShoppingCard;
					nudShareFollow.Value = liveConfigEntity.ShareCount;
					nudClickFrom.Value = liveConfigEntity.NumOfClick_From;
					nudClickTo.Value = liveConfigEntity.NumOfClick_To;
					nudTymRepeat.Value = liveConfigEntity.TymRepeatCount;
					nudRepeat.Value = liveConfigEntity.CommentRepeatCount;
					txtChannelName.Text = liveConfigEntity.ChannelName;
					rbtInbox.Checked = liveConfigEntity.Type == ViewType.Inbox;
					rbtLinkKenh.Checked = liveConfigEntity.Type == ViewType.Link;
					rbtSearch.Checked = liveConfigEntity.Type == ViewType.Search;
					cbxRemoveAfterUse.Checked = liveConfigEntity.RemoveComment;
					nudDelayFromGim.Value = liveConfigEntity.DelayGimFrom;
					nudDelayToGim.Value = liveConfigEntity.DelayGimto;
					txtKeyword.Text = liveConfigEntity.Keyword;
					cbxCopy.Checked = liveConfigEntity.CopyLink;
					cbxFollowShop.Checked = liveConfigEntity.Follow;
					nudDelayLiveView.Value = liveConfigEntity.DelayLiveView;
					cbxGhim.Checked = liveConfigEntity.ClickGimProduct;
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			string sEEDING_LIVE_COMMENT = CaChuaConstant.SEEDING_LIVE_COMMENT;
			File.WriteAllText(sEEDING_LIVE_COMMENT, txtComment.Text.Trim());
			LiveConfigEntity liveConfigEntity = new LiveConfigEntity();
			liveConfigEntity.Link = txtLink.Text.Trim();
			liveConfigEntity.CopyLink = cbxCopy.Checked;
			liveConfigEntity.Follow = cbxFollowShop.Checked;
			liveConfigEntity.Keyword = txtKeyword.Text.Trim();
			liveConfigEntity.RemoveComment = cbxRemoveAfterUse.Checked;
			liveConfigEntity.ChannelName = txtChannelName.Text;
			liveConfigEntity.TymRepeatCount = Convert.ToInt32(nudTymRepeat.Value);
			liveConfigEntity.CommentDelayFrom = Convert.ToInt16(nudDelayFrom.Value);
			liveConfigEntity.CommentDelayTo = Convert.ToInt16(nudDelayTo.Value);
			liveConfigEntity.TimeFrom = Convert.ToInt16(nudFrom.Value);
			liveConfigEntity.TimeTo = Convert.ToInt16(nudTo.Value);
			liveConfigEntity.ViewProduct = cbxProduct.Checked;
			liveConfigEntity.ClickGimProduct = cbxGhim.Checked;
			liveConfigEntity.ViewShoppingCard = cbxShop.Checked;
			liveConfigEntity.ShareCount = Convert.ToInt16(nudShareFollow.Value);
			liveConfigEntity.NumOfClick_From = Convert.ToInt16(nudClickFrom.Value);
			liveConfigEntity.NumOfClick_To = Convert.ToInt16(nudClickTo.Value);
			liveConfigEntity.CommentRepeatCount = Convert.ToInt16(nudRepeat.Value);
			liveConfigEntity.DelayLiveView = Convert.ToInt32(nudDelayLiveView.Value);
			liveConfigEntity.DelayGimFrom = Convert.ToInt32(nudDelayFromGim.Value);
			liveConfigEntity.DelayGimto = Convert.ToInt32(nudDelayToGim.Value);
			liveConfigEntity.Type = (rbtSearch.Checked ? ViewType.Search : (rbtLinkKenh.Checked ? ViewType.Link : ViewType.Inbox));
			File.WriteAllText(CaChuaConstant.SEEDING_LIVE_CONFIG, new JavaScriptSerializer().Serialize(liveConfigEntity));
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			IOrderedEnumerable<string> values = from s in txtComment.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList()
				orderby Guid.NewGuid()
				select s;
			txtComment.Text = string.Join(Environment.NewLine, values);
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
			nudFrom = new System.Windows.Forms.NumericUpDown();
			label2 = new System.Windows.Forms.Label();
			nudTo = new System.Windows.Forms.NumericUpDown();
			txtLink = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			nudClickFrom = new System.Windows.Forms.NumericUpDown();
			label5 = new System.Windows.Forms.Label();
			nudClickTo = new System.Windows.Forms.NumericUpDown();
			txtComment = new System.Windows.Forms.TextBox();
			label6 = new System.Windows.Forms.Label();
			btnSave = new System.Windows.Forms.Button();
			nudRepeat = new System.Windows.Forms.NumericUpDown();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			nudDelayFrom = new System.Windows.Forms.NumericUpDown();
			label9 = new System.Windows.Forms.Label();
			nudDelayTo = new System.Windows.Forms.NumericUpDown();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			nudShareFollow = new System.Windows.Forms.NumericUpDown();
			cbxShop = new System.Windows.Forms.CheckBox();
			cbxProduct = new System.Windows.Forms.CheckBox();
			label12 = new System.Windows.Forms.Label();
			nudTymRepeat = new System.Windows.Forms.NumericUpDown();
			txtChannelName = new System.Windows.Forms.TextBox();
			rbtLinkKenh = new System.Windows.Forms.RadioButton();
			rbtInbox = new System.Windows.Forms.RadioButton();
			cbxRemoveAfterUse = new System.Windows.Forms.CheckBox();
			txtKeyword = new System.Windows.Forms.TextBox();
			rbtSearch = new System.Windows.Forms.RadioButton();
			label3 = new System.Windows.Forms.Label();
			button1 = new System.Windows.Forms.Button();
			cbxCopy = new System.Windows.Forms.CheckBox();
			cbxFollowShop = new System.Windows.Forms.CheckBox();
			label13 = new System.Windows.Forms.Label();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			nudDelayLiveView = new System.Windows.Forms.NumericUpDown();
			label14 = new System.Windows.Forms.Label();
			label15 = new System.Windows.Forms.Label();
			cbxGhim = new System.Windows.Forms.CheckBox();
			nudDelayFromGim = new System.Windows.Forms.NumericUpDown();
			label16 = new System.Windows.Forms.Label();
			nudDelayToGim = new System.Windows.Forms.NumericUpDown();
			label17 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudClickFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudClickTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudRepeat).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudShareFollow).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudTymRepeat).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayLiveView).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayFromGim).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayToGim).BeginInit();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(121, 196);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(85, 13);
			label1.TabIndex = 0;
			label1.Text = "Thời gian xem từ";
			nudFrom.Location = new System.Drawing.Point(212, 194);
			nudFrom.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudFrom.Name = "nudFrom";
			nudFrom.Size = new System.Drawing.Size(73, 20);
			nudFrom.TabIndex = 1;
			nudFrom.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(294, 197);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(95, 13);
			label2.TabIndex = 0;
			label2.Text = "Thời gian xem đến";
			nudTo.Location = new System.Drawing.Point(392, 194);
			nudTo.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudTo.Name = "nudTo";
			nudTo.Size = new System.Drawing.Size(73, 20);
			nudTo.TabIndex = 1;
			nudTo.Value = new decimal(new int[4] { 2, 0, 0, 0 });
			txtLink.Location = new System.Drawing.Point(139, 12);
			txtLink.Multiline = true;
			txtLink.Name = "txtLink";
			txtLink.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtLink.Size = new System.Drawing.Size(326, 48);
			txtLink.TabIndex = 2;
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(67, 231);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(139, 13);
			label4.TabIndex = 4;
			label4.Text = "Số lượng tim click liên tục từ";
			nudClickFrom.Location = new System.Drawing.Point(212, 229);
			nudClickFrom.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudClickFrom.Name = "nudClickFrom";
			nudClickFrom.Size = new System.Drawing.Size(73, 20);
			nudClickFrom.TabIndex = 1;
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(323, 233);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(26, 13);
			label5.TabIndex = 6;
			label5.Text = "đến";
			nudClickTo.Location = new System.Drawing.Point(392, 229);
			nudClickTo.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudClickTo.Name = "nudClickTo";
			nudClickTo.Size = new System.Drawing.Size(73, 20);
			nudClickTo.TabIndex = 5;
			txtComment.Location = new System.Drawing.Point(487, 36);
			txtComment.Multiline = true;
			txtComment.Name = "txtComment";
			txtComment.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtComment.Size = new System.Drawing.Size(352, 386);
			txtComment.TabIndex = 7;
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(489, 9);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(96, 13);
			label6.TabIndex = 8;
			label6.Text = "Nội dung comment";
			btnSave.Location = new System.Drawing.Point(487, 433);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(98, 43);
			btnSave.TabIndex = 9;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			nudRepeat.Location = new System.Drawing.Point(212, 292);
			nudRepeat.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudRepeat.Name = "nudRepeat";
			nudRepeat.Size = new System.Drawing.Size(73, 20);
			nudRepeat.TabIndex = 10;
			nudRepeat.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(93, 294);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(113, 13);
			label7.TabIndex = 11;
			label7.Text = "Số lần lặp lại comment";
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(67, 334);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(181, 13);
			label8.TabIndex = 0;
			label8.Text = "Thời gian nghỉ giữa các lần bình luận";
			nudDelayFrom.Location = new System.Drawing.Point(257, 333);
			nudDelayFrom.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudDelayFrom.Name = "nudDelayFrom";
			nudDelayFrom.Size = new System.Drawing.Size(53, 20);
			nudDelayFrom.TabIndex = 1;
			nudDelayFrom.Value = new decimal(new int[4] { 3, 0, 0, 0 });
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(323, 335);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(26, 13);
			label9.TabIndex = 0;
			label9.Text = "đến";
			nudDelayTo.Location = new System.Drawing.Point(366, 333);
			nudDelayTo.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudDelayTo.Name = "nudDelayTo";
			nudDelayTo.Size = new System.Drawing.Size(51, 20);
			nudDelayTo.TabIndex = 1;
			nudDelayTo.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(428, 335);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(26, 13);
			label10.TabIndex = 12;
			label10.Text = "giây";
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(93, 374);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(153, 13);
			label11.TabIndex = 14;
			label11.Text = "Số lượng share người đã follow";
			nudShareFollow.Location = new System.Drawing.Point(254, 370);
			nudShareFollow.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudShareFollow.Name = "nudShareFollow";
			nudShareFollow.Size = new System.Drawing.Size(56, 20);
			nudShareFollow.TabIndex = 13;
			nudShareFollow.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			cbxShop.AutoSize = true;
			cbxShop.Location = new System.Drawing.Point(81, 433);
			cbxShop.Name = "cbxShop";
			cbxShop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			cbxShop.Size = new System.Drawing.Size(91, 17);
			cbxShop.TabIndex = 15;
			cbxShop.Text = "Lướt giỏ hàng";
			cbxShop.UseVisualStyleBackColor = true;
			cbxProduct.AutoSize = true;
			cbxProduct.Location = new System.Drawing.Point(189, 433);
			cbxProduct.Name = "cbxProduct";
			cbxProduct.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			cbxProduct.Size = new System.Drawing.Size(96, 17);
			cbxProduct.TabIndex = 16;
			cbxProduct.Text = "Xem sản phẩm";
			cbxProduct.UseVisualStyleBackColor = true;
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(93, 263);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(104, 13);
			label12.TabIndex = 18;
			label12.Text = "Số lần lặp lại thả tym";
			nudTymRepeat.Location = new System.Drawing.Point(212, 261);
			nudTymRepeat.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudTymRepeat.Name = "nudTymRepeat";
			nudTymRepeat.Size = new System.Drawing.Size(73, 20);
			nudTymRepeat.TabIndex = 17;
			nudTymRepeat.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			txtChannelName.Location = new System.Drawing.Point(139, 99);
			txtChannelName.Name = "txtChannelName";
			txtChannelName.Size = new System.Drawing.Size(326, 20);
			txtChannelName.TabIndex = 19;
			rbtLinkKenh.AutoSize = true;
			rbtLinkKenh.Checked = true;
			rbtLinkKenh.Location = new System.Drawing.Point(49, 26);
			rbtLinkKenh.Name = "rbtLinkKenh";
			rbtLinkKenh.Size = new System.Drawing.Size(72, 17);
			rbtLinkKenh.TabIndex = 20;
			rbtLinkKenh.TabStop = true;
			rbtLinkKenh.Text = "Link kênh";
			rbtLinkKenh.UseVisualStyleBackColor = true;
			rbtInbox.AutoSize = true;
			rbtInbox.Location = new System.Drawing.Point(49, 101);
			rbtInbox.Name = "rbtInbox";
			rbtInbox.Size = new System.Drawing.Size(69, 17);
			rbtInbox.TabIndex = 21;
			rbtInbox.Text = "Following";
			rbtInbox.UseVisualStyleBackColor = true;
			cbxRemoveAfterUse.AutoSize = true;
			cbxRemoveAfterUse.Checked = true;
			cbxRemoveAfterUse.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxRemoveAfterUse.Location = new System.Drawing.Point(305, 433);
			cbxRemoveAfterUse.Name = "cbxRemoveAfterUse";
			cbxRemoveAfterUse.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			cbxRemoveAfterUse.Size = new System.Drawing.Size(160, 17);
			cbxRemoveAfterUse.TabIndex = 16;
			cbxRemoveAfterUse.Text = "Comment xong xóa nội dung";
			cbxRemoveAfterUse.UseVisualStyleBackColor = true;
			txtKeyword.Location = new System.Drawing.Point(139, 129);
			txtKeyword.Name = "txtKeyword";
			txtKeyword.Size = new System.Drawing.Size(326, 20);
			txtKeyword.TabIndex = 19;
			rbtSearch.AutoSize = true;
			rbtSearch.Location = new System.Drawing.Point(49, 131);
			rbtSearch.Name = "rbtSearch";
			rbtSearch.Size = new System.Drawing.Size(59, 17);
			rbtSearch.TabIndex = 21;
			rbtSearch.Text = "Search";
			rbtSearch.UseVisualStyleBackColor = true;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(136, 159);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(228, 13);
			label3.TabIndex = 22;
			label3.Text = "Từ khóa hoặc hashtag cách nhau dấu phẩy (,)";
			button1.Location = new System.Drawing.Point(604, 433);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(235, 43);
			button1.TabIndex = 23;
			button1.Text = "Random Comment";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);
			cbxCopy.AutoSize = true;
			cbxCopy.Location = new System.Drawing.Point(99, 461);
			cbxCopy.Name = "cbxCopy";
			cbxCopy.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			cbxCopy.Size = new System.Drawing.Size(73, 17);
			cbxCopy.TabIndex = 24;
			cbxCopy.Text = "Copy Link";
			cbxCopy.UseVisualStyleBackColor = true;
			cbxFollowShop.AutoSize = true;
			cbxFollowShop.Location = new System.Drawing.Point(229, 461);
			cbxFollowShop.Name = "cbxFollowShop";
			cbxFollowShop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			cbxFollowShop.Size = new System.Drawing.Size(56, 17);
			cbxFollowShop.TabIndex = 25;
			cbxFollowShop.Text = "Follow";
			cbxFollowShop.UseVisualStyleBackColor = true;
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(136, 70);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(117, 13);
			label13.TabIndex = 26;
			label13.Text = "Copy link shop như sau";
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(254, 70);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(156, 13);
			linkLabel1.TabIndex = 27;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "https://tiktok.com/@cachuake";
			nudDelayLiveView.Location = new System.Drawing.Point(255, 403);
			nudDelayLiveView.Name = "nudDelayLiveView";
			nudDelayLiveView.Size = new System.Drawing.Size(53, 20);
			nudDelayLiveView.TabIndex = 29;
			nudDelayLiveView.Value = new decimal(new int[4] { 3, 0, 0, 0 });
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(89, 405);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(157, 13);
			label14.TabIndex = 28;
			label14.Text = "Thời gian nghỉ chờ hiện mắt live";
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(323, 405);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(26, 13);
			label15.TabIndex = 30;
			label15.Text = "giây";
			cbxGhim.AutoSize = true;
			cbxGhim.Location = new System.Drawing.Point(164, 491);
			cbxGhim.Name = "cbxGhim";
			cbxGhim.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			cbxGhim.Size = new System.Drawing.Size(121, 17);
			cbxGhim.TabIndex = 31;
			cbxGhim.Text = "Click sản phẩm GIM";
			cbxGhim.UseVisualStyleBackColor = true;
			nudDelayFromGim.Location = new System.Drawing.Point(305, 488);
			nudDelayFromGim.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudDelayFromGim.Name = "nudDelayFromGim";
			nudDelayFromGim.Size = new System.Drawing.Size(44, 20);
			nudDelayFromGim.TabIndex = 1;
			nudDelayFromGim.Value = new decimal(new int[4] { 3, 0, 0, 0 });
			label16.AutoSize = true;
			label16.Location = new System.Drawing.Point(353, 490);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(26, 13);
			label16.TabIndex = 0;
			label16.Text = "đến";
			nudDelayToGim.Location = new System.Drawing.Point(382, 488);
			nudDelayToGim.Maximum = new decimal(new int[4] { 99999999, 0, 0, 0 });
			nudDelayToGim.Name = "nudDelayToGim";
			nudDelayToGim.Size = new System.Drawing.Size(51, 20);
			nudDelayToGim.TabIndex = 1;
			nudDelayToGim.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label17.AutoSize = true;
			label17.Location = new System.Drawing.Point(444, 490);
			label17.Name = "label17";
			label17.Size = new System.Drawing.Size(26, 13);
			label17.TabIndex = 12;
			label17.Text = "giây";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(868, 542);
			base.Controls.Add(cbxGhim);
			base.Controls.Add(label15);
			base.Controls.Add(nudDelayLiveView);
			base.Controls.Add(label14);
			base.Controls.Add(linkLabel1);
			base.Controls.Add(label13);
			base.Controls.Add(cbxFollowShop);
			base.Controls.Add(cbxCopy);
			base.Controls.Add(button1);
			base.Controls.Add(label3);
			base.Controls.Add(rbtSearch);
			base.Controls.Add(rbtInbox);
			base.Controls.Add(rbtLinkKenh);
			base.Controls.Add(txtKeyword);
			base.Controls.Add(txtChannelName);
			base.Controls.Add(label12);
			base.Controls.Add(nudTymRepeat);
			base.Controls.Add(cbxRemoveAfterUse);
			base.Controls.Add(cbxProduct);
			base.Controls.Add(cbxShop);
			base.Controls.Add(label11);
			base.Controls.Add(nudShareFollow);
			base.Controls.Add(label17);
			base.Controls.Add(label10);
			base.Controls.Add(label7);
			base.Controls.Add(nudRepeat);
			base.Controls.Add(btnSave);
			base.Controls.Add(label6);
			base.Controls.Add(txtComment);
			base.Controls.Add(label5);
			base.Controls.Add(nudClickTo);
			base.Controls.Add(label4);
			base.Controls.Add(txtLink);
			base.Controls.Add(nudDelayToGim);
			base.Controls.Add(label16);
			base.Controls.Add(nudDelayTo);
			base.Controls.Add(label9);
			base.Controls.Add(nudTo);
			base.Controls.Add(nudDelayFromGim);
			base.Controls.Add(label2);
			base.Controls.Add(nudDelayFrom);
			base.Controls.Add(nudClickFrom);
			base.Controls.Add(label8);
			base.Controls.Add(nudFrom);
			base.Controls.Add(label1);
			base.Name = "frmSeedingLive";
			Text = "Seeding Live";
			base.Load += new System.EventHandler(frmSeedingLive_Load);
			((System.ComponentModel.ISupportInitialize)nudFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudClickFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudClickTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudRepeat).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudShareFollow).EndInit();
			((System.ComponentModel.ISupportInitialize)nudTymRepeat).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayLiveView).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayFromGim).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayToGim).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
