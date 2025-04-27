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
	public class frmCommentRandom : Form
	{
		private IContainer components = null;

		private Label label3;

		private Label label2;

		private Label label1;

		private NumericUpDown nudDelay;

		private NumericUpDown nudLike;

		private TextBox txtLinks;

		private Button btnSave;

		private TextBox txtFileComment;

		private Label label4;

		private TextBox txtTags;

		private Label label5;

		private Label label6;

		private Label label7;

		private Button button1;

		private CheckBox cbxRemove;

		private RadioButton rbtMultiLine;

		private RadioButton rbtSingline;

		private NumericUpDown nudViewLink;

		private Label label8;

		private Label label9;

		public frmCommentRandom()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			CommentItems commentItems = new CommentItems();
			commentItems.FileComment = txtFileComment.Text;
			commentItems.CommentCount = Convert.ToInt32(nudLike.Value);
			commentItems.Delay = Convert.ToInt32(nudDelay.Value);
			commentItems.Deleted = cbxRemove.Checked;
			commentItems.ListID = txtLinks.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			commentItems.Tags = txtTags.Text;
			commentItems.DelayViewLink = (int)nudViewLink.Value;
			commentItems.MultiLine = rbtMultiLine.Checked;
			File.WriteAllText(CaChuaConstant.RANDOM_COMMENT, new JavaScriptSerializer().Serialize(commentItems));
			Close();
		}

		private void frmCommentRandom_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.RANDOM_COMMENT))
			{
				CommentItems commentItems = new JavaScriptSerializer().Deserialize<CommentItems>(Utils.ReadTextFile(CaChuaConstant.RANDOM_COMMENT));
				txtFileComment.Text = commentItems.FileComment;
				nudLike.Value = commentItems.CommentCount;
				nudDelay.Value = commentItems.Delay;
				txtTags.Text = commentItems.Tags;
				nudViewLink.Value = commentItems.DelayViewLink;
				cbxRemove.Checked = commentItems.Deleted;
				txtLinks.Text = string.Join(Environment.NewLine, commentItems.ListID);
				rbtMultiLine.Checked = commentItems.MultiLine;
				rbtSingline.Checked = !rbtMultiLine.Checked;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = "TXT files (*.txt)|*.txt";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtFileComment.Text = openFileDialog.FileName;
			}
		}

		private void rbtMultiLine_CheckedChanged(object sender, EventArgs e)
		{
			cbxRemove.Checked = false;
			cbxRemove.Enabled = false;
		}

		private void rbtSingline_CheckedChanged(object sender, EventArgs e)
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
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			nudLike = new System.Windows.Forms.NumericUpDown();
			txtLinks = new System.Windows.Forms.TextBox();
			btnSave = new System.Windows.Forms.Button();
			txtFileComment = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			txtTags = new System.Windows.Forms.TextBox();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			button1 = new System.Windows.Forms.Button();
			cbxRemove = new System.Windows.Forms.CheckBox();
			rbtMultiLine = new System.Windows.Forms.RadioButton();
			rbtSingline = new System.Windows.Forms.RadioButton();
			nudViewLink = new System.Windows.Forms.NumericUpDown();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudLike).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudViewLink).BeginInit();
			SuspendLayout();
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(35, 24);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(157, 13);
			label3.TabIndex = 13;
			label3.Text = "Danh sách Link, mỗi bài 1 dòng";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(152, 359);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(34, 13);
			label2.TabIndex = 12;
			label2.Text = "Delay";
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(43, 324);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(133, 13);
			label1.TabIndex = 11;
			label1.Text = "Số lượng bài cần comment";
			nudDelay.Location = new System.Drawing.Point(202, 357);
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(67, 20);
			nudDelay.TabIndex = 10;
			nudLike.Location = new System.Drawing.Point(202, 324);
			nudLike.Name = "nudLike";
			nudLike.Size = new System.Drawing.Size(67, 20);
			nudLike.TabIndex = 9;
			txtLinks.Location = new System.Drawing.Point(35, 53);
			txtLinks.Multiline = true;
			txtLinks.Name = "txtLinks";
			txtLinks.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			txtLinks.Size = new System.Drawing.Size(726, 119);
			txtLinks.TabIndex = 8;
			btnSave.Location = new System.Drawing.Point(654, 321);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(115, 56);
			btnSave.TabIndex = 7;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			txtFileComment.Location = new System.Drawing.Point(135, 246);
			txtFileComment.Name = "txtFileComment";
			txtFileComment.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			txtFileComment.Size = new System.Drawing.Size(542, 20);
			txtFileComment.TabIndex = 14;
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(35, 184);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(245, 13);
			label4.TabIndex = 15;
			label4.Text = "Nội dung bình luận ngẫu nhiên, hỗ trợ Spin {A|B|C}";
			txtTags.Location = new System.Drawing.Point(349, 321);
			txtTags.Multiline = true;
			txtTags.Name = "txtTags";
			txtTags.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			txtTags.Size = new System.Drawing.Size(299, 56);
			txtTags.TabIndex = 16;
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(303, 324);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(31, 13);
			label5.TabIndex = 17;
			label5.Text = "Tags";
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(346, 389);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(109, 13);
			label6.TabIndex = 18;
			label6.Text = "Mỗi username 1 dòng";
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(35, 246);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(94, 13);
			label7.TabIndex = 19;
			label7.Text = "File chứa nội dung";
			button1.Location = new System.Drawing.Point(683, 244);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(78, 24);
			button1.TabIndex = 20;
			button1.Text = "Chọn file";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);
			cbxRemove.AutoSize = true;
			cbxRemove.Location = new System.Drawing.Point(463, 282);
			cbxRemove.Name = "cbxRemove";
			cbxRemove.Size = new System.Drawing.Size(214, 17);
			cbxRemove.TabIndex = 23;
			cbxRemove.Text = "Bình luận xong xóa tránh trùng nội dung";
			cbxRemove.UseVisualStyleBackColor = true;
			rbtMultiLine.AutoSize = true;
			rbtMultiLine.Location = new System.Drawing.Point(135, 282);
			rbtMultiLine.Name = "rbtMultiLine";
			rbtMultiLine.Size = new System.Drawing.Size(203, 17);
			rbtMultiLine.TabIndex = 26;
			rbtMultiLine.TabStop = true;
			rbtMultiLine.Text = "Nội dung chứa nhiều dòng hỗ trợ spin";
			rbtMultiLine.UseVisualStyleBackColor = true;
			rbtMultiLine.CheckedChanged += new System.EventHandler(rbtMultiLine_CheckedChanged);
			rbtSingline.AutoSize = true;
			rbtSingline.Location = new System.Drawing.Point(349, 282);
			rbtSingline.Name = "rbtSingline";
			rbtSingline.Size = new System.Drawing.Size(99, 17);
			rbtSingline.TabIndex = 27;
			rbtSingline.TabStop = true;
			rbtSingline.Text = "Mỗi câu 1 dòng";
			rbtSingline.UseVisualStyleBackColor = true;
			rbtSingline.CheckedChanged += new System.EventHandler(rbtSingline_CheckedChanged);
			nudViewLink.Location = new System.Drawing.Point(135, 209);
			nudViewLink.Name = "nudViewLink";
			nudViewLink.Size = new System.Drawing.Size(67, 20);
			nudViewLink.TabIndex = 10;
			nudViewLink.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(95, 213);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(28, 13);
			label8.TabIndex = 12;
			label8.Text = "Xem";
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(210, 211);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(157, 13);
			label9.TabIndex = 28;
			label9.Text = "giây xong mới bắt đầu bình luận";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(797, 465);
			base.Controls.Add(label9);
			base.Controls.Add(rbtSingline);
			base.Controls.Add(rbtMultiLine);
			base.Controls.Add(cbxRemove);
			base.Controls.Add(button1);
			base.Controls.Add(label7);
			base.Controls.Add(label6);
			base.Controls.Add(label5);
			base.Controls.Add(txtTags);
			base.Controls.Add(label4);
			base.Controls.Add(txtFileComment);
			base.Controls.Add(label3);
			base.Controls.Add(label8);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(nudViewLink);
			base.Controls.Add(nudDelay);
			base.Controls.Add(nudLike);
			base.Controls.Add(txtLinks);
			base.Controls.Add(btnSave);
			base.Name = "frmCommentRandom";
			Text = "Comment Random";
			base.Load += new System.EventHandler(frmCommentRandom_Load);
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			((System.ComponentModel.ISupportInitialize)nudLike).EndInit();
			((System.ComponentModel.ISupportInitialize)nudViewLink).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
