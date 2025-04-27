using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmAddProduct : Form
	{
		private IContainer components = null;

		private Button btnSave;

		private RadioButton rbtLinkOnly;

		private RadioButton rbtLinkAndName;

		private TextBox txtLink;

		private Label label1;

		private NumericUpDown numOfLink;

		private Button btnFile;

		private Label label3;

		private Label label2;

		public frmAddProduct()
		{
			InitializeComponent();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			AddLinkEntity addLinkEntity = new AddLinkEntity();
			addLinkEntity.FileUrl = txtLink.Text;
			addLinkEntity.LinkOnly = rbtLinkOnly.Checked;
			addLinkEntity.LinkAndName = rbtLinkAndName.Checked;
			addLinkEntity.NumOfLink = Convert.ToInt32(numOfLink.Value);
			File.WriteAllText(CaChuaConstant.LINK_PRODUCT, new JavaScriptSerializer().Serialize(addLinkEntity));
			Close();
		}

		private void frmAddProduct_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.LINK_PRODUCT))
			{
				AddLinkEntity addLinkEntity = new JavaScriptSerializer().Deserialize<AddLinkEntity>(Utils.ReadTextFile(CaChuaConstant.LINK_PRODUCT));
				if (addLinkEntity != null)
				{
					txtLink.Text = addLinkEntity.FileUrl;
					rbtLinkOnly.Checked = addLinkEntity.LinkOnly;
					rbtLinkAndName.Checked = addLinkEntity.LinkAndName;
					numOfLink.Value = addLinkEntity.NumOfLink;
				}
			}
		}

		private void btnFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Text File";
			openFileDialog.Filter = "TXT files|*.txt";
			openFileDialog.InitialDirectory = "C:\\";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtLink.Text = openFileDialog.FileName.ToString();
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
			rbtLinkOnly = new System.Windows.Forms.RadioButton();
			rbtLinkAndName = new System.Windows.Forms.RadioButton();
			txtLink = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			numOfLink = new System.Windows.Forms.NumericUpDown();
			btnFile = new System.Windows.Forms.Button();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)numOfLink).BeginInit();
			SuspendLayout();
			btnSave.Location = new System.Drawing.Point(199, 158);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(117, 35);
			btnSave.TabIndex = 0;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			rbtLinkOnly.AutoSize = true;
			rbtLinkOnly.Checked = true;
			rbtLinkOnly.Location = new System.Drawing.Point(50, 69);
			rbtLinkOnly.Name = "rbtLinkOnly";
			rbtLinkOnly.Size = new System.Drawing.Size(112, 17);
			rbtLinkOnly.TabIndex = 1;
			rbtLinkOnly.TabStop = true;
			rbtLinkOnly.Text = "Chỉ Link sản phẩm";
			rbtLinkOnly.UseVisualStyleBackColor = true;
			rbtLinkAndName.AutoSize = true;
			rbtLinkAndName.Location = new System.Drawing.Point(216, 69);
			rbtLinkAndName.Name = "rbtLinkAndName";
			rbtLinkAndName.Size = new System.Drawing.Size(166, 17);
			rbtLinkAndName.TabIndex = 2;
			rbtLinkAndName.Text = "Link sản phẩm và ID Account";
			rbtLinkAndName.UseVisualStyleBackColor = true;
			txtLink.Location = new System.Drawing.Point(122, 99);
			txtLink.MaxLength = 32767000;
			txtLink.Name = "txtLink";
			txtLink.Size = new System.Drawing.Size(281, 20);
			txtLink.TabIndex = 3;
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			label1.Location = new System.Drawing.Point(145, 19);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(227, 20);
			label1.TabIndex = 4;
			label1.Text = "Thêm Link sản phẩm vào Shop";
			numOfLink.Location = new System.Drawing.Point(122, 130);
			numOfLink.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			numOfLink.Name = "numOfLink";
			numOfLink.Size = new System.Drawing.Size(64, 20);
			numOfLink.TabIndex = 5;
			numOfLink.Visible = false;
			btnFile.Location = new System.Drawing.Point(409, 98);
			btnFile.Name = "btnFile";
			btnFile.Size = new System.Drawing.Size(75, 23);
			btnFile.TabIndex = 7;
			btnFile.Text = "Chọn File";
			btnFile.UseVisualStyleBackColor = true;
			btnFile.Click += new System.EventHandler(btnFile_Click);
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(47, 102);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(69, 13);
			label3.TabIndex = 8;
			label3.Text = "File chứa link";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 132);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(106, 13);
			label2.TabIndex = 6;
			label2.Text = "Số Link cho mỗi nick";
			label2.Visible = false;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(524, 239);
			base.Controls.Add(label3);
			base.Controls.Add(btnFile);
			base.Controls.Add(label2);
			base.Controls.Add(numOfLink);
			base.Controls.Add(label1);
			base.Controls.Add(txtLink);
			base.Controls.Add(rbtLinkAndName);
			base.Controls.Add(rbtLinkOnly);
			base.Controls.Add(btnSave);
			base.Name = "frmAddProduct";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Add Product";
			base.Load += new System.EventHandler(frmAddProduct_Load);
			((System.ComponentModel.ISupportInitialize)numOfLink).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
