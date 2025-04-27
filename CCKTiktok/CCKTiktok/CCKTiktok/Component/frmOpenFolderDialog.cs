using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmOpenFolderDialog : Form
	{
		private IContainer components = null;

		private Button btnSelect;

		private TextBox txtDialog;

		private Label label1;

		private Button btnSave;

		private CheckBox cbxDelete;

		private NumericUpDown nudDelay;

		private Label label2;

		private Label label3;

		private GroupBox groupBox1;

		private GroupBox groupBox2;

		private Label lblMsg;

		private NumericUpDown nhdHeight;

		private Label label6;

		private NumericUpDown nudWidth;

		private Label label5;

		private NumericUpDown nudNumber;

		private Label label4;

		private Button btnDownload;

		public frmOpenFolderDialog()
		{
			InitializeComponent();
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath = txtDialog.Text;
			DialogResult dialogResult = folderBrowserDialog.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				txtDialog.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void frmOpenFolderDialog_Load(object sender, EventArgs e)
		{
			if (!Directory.Exists(Application.StartupPath + "\\Data\\Avatar"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\Data\\Avatar");
			}
			new FolderBrowserDialog();
			if (File.Exists(CaChuaConstant.AVATAR_FOLDER) && Directory.Exists(Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER)))
			{
				txtDialog.Text = Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER);
			}
			else
			{
				txtDialog.Text = Application.StartupPath + "\\Data\\Avatar";
			}
			cbxDelete.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER_DELETE));
			int num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.AVATAR_FOLDER_DELAY));
			if (num == 0)
			{
				num = 5;
			}
			nudDelay.Value = num;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.AVATAR_FOLDER, txtDialog.Text);
			File.WriteAllText(CaChuaConstant.AVATAR_FOLDER_DELAY, nudDelay.Value.ToString());
			File.WriteAllText(CaChuaConstant.AVATAR_FOLDER_DELETE, cbxDelete.Text);
			Close();
		}

		private async Task<bool> Download(string folder)
		{
			try
			{
				string html = await GetResponse("https://boredhumans.com/api_faces2.php");
				Regex r = new Regex("src=\"(.*?)\"");
				Match i = r.Match(html);
				if (i.Success)
				{
					string src = i.Groups[1].Value;
					if (src.StartsWith("/"))
					{
						src = "https://boredhumans.com" + src;
					}
					if (Directory.Exists(src))
					{
						string file = src + "\\" + Guid.NewGuid().ToString("N") + ".jpg";
						new WebClient().DownloadFile(src, file);
						RenderImg(file);
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		private static async Task<string> GetResponse(string url)
		{
			using HttpClient client = new HttpClient();
			try
			{
				HttpResponseMessage response = await client.GetAsync(url);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsStringAsync();
				}
				return await response.Content.ReadAsStringAsync();
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		private void RenderImg(string imagePath, int w = 512, int h = 512)
		{
			try
			{
				using Bitmap image = new Bitmap(imagePath);
				Bitmap bitmap = new Bitmap(w, h);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.DrawImage(image, 0, 0, 512, 512);
				}
				string filename = imagePath.Replace(".jpg", "zoom.jpg");
				bitmap.Save(filename, ImageFormat.Jpeg);
				File.Delete(imagePath);
			}
			catch
			{
			}
		}

		private async void btnDownload_Click(object sender, EventArgs e)
		{
			decimal max = nudNumber.Value;
			int successCount = 0;
			for (int i = 0; (decimal)i < max; i++)
			{
				if (await Download(txtDialog.Text))
				{
					successCount++;
				}
				lblMsg.Text = $"Đã tải thành công {successCount}/{max}";
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
			btnSelect = new System.Windows.Forms.Button();
			txtDialog = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			btnSave = new System.Windows.Forms.Button();
			cbxDelete = new System.Windows.Forms.CheckBox();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			nudNumber = new System.Windows.Forms.NumericUpDown();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			nudWidth = new System.Windows.Forms.NumericUpDown();
			label6 = new System.Windows.Forms.Label();
			nhdHeight = new System.Windows.Forms.NumericUpDown();
			btnDownload = new System.Windows.Forms.Button();
			lblMsg = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudNumber).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudWidth).BeginInit();
			((System.ComponentModel.ISupportInitialize)nhdHeight).BeginInit();
			SuspendLayout();
			btnSelect.Location = new System.Drawing.Point(513, 60);
			btnSelect.Name = "btnSelect";
			btnSelect.Size = new System.Drawing.Size(75, 23);
			btnSelect.TabIndex = 0;
			btnSelect.Text = "Chọn";
			btnSelect.UseVisualStyleBackColor = true;
			btnSelect.Click += new System.EventHandler(btnSelect_Click);
			txtDialog.Location = new System.Drawing.Point(167, 61);
			txtDialog.Name = "txtDialog";
			txtDialog.Size = new System.Drawing.Size(334, 20);
			txtDialog.TabIndex = 1;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(58, 64);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(97, 13);
			label1.TabIndex = 2;
			label1.Text = "Thư mục chứa ảnh";
			btnSave.Location = new System.Drawing.Point(513, 98);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(75, 23);
			btnSave.TabIndex = 0;
			btnSave.Text = "Lưu";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			cbxDelete.AutoSize = true;
			cbxDelete.Location = new System.Drawing.Point(167, 98);
			cbxDelete.Name = "cbxDelete";
			cbxDelete.Size = new System.Drawing.Size(130, 17);
			cbxDelete.TabIndex = 3;
			cbxDelete.Text = "Xóa ảnh sau khi dùng";
			cbxDelete.UseVisualStyleBackColor = true;
			nudDelay.Location = new System.Drawing.Point(265, 127);
			nudDelay.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudDelay.Minimum = new decimal(new int[4] { 5, 0, 0, 0 });
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(64, 20);
			nudDelay.TabIndex = 4;
			nudDelay.Value = new decimal(new int[4] { 15, 0, 0, 0 });
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(164, 129);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(86, 13);
			label2.TabIndex = 2;
			label2.Text = "Nghỉ chờ up ảnh";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(337, 129);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(26, 13);
			label3.TabIndex = 2;
			label3.Text = "giây";
			groupBox1.Location = new System.Drawing.Point(34, 13);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(597, 171);
			groupBox1.TabIndex = 5;
			groupBox1.TabStop = false;
			groupBox1.Text = "Cấu hình Avatar";
			groupBox2.Controls.Add(lblMsg);
			groupBox2.Controls.Add(nhdHeight);
			groupBox2.Controls.Add(label6);
			groupBox2.Controls.Add(nudWidth);
			groupBox2.Controls.Add(label5);
			groupBox2.Controls.Add(nudNumber);
			groupBox2.Controls.Add(label4);
			groupBox2.Controls.Add(btnDownload);
			groupBox2.Location = new System.Drawing.Point(34, 201);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(597, 100);
			groupBox2.TabIndex = 6;
			groupBox2.TabStop = false;
			groupBox2.Text = "Tải ảnh mẫu từ Internet AI";
			nudNumber.Location = new System.Drawing.Point(103, 33);
			nudNumber.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudNumber.Minimum = new decimal(new int[4] { 5, 0, 0, 0 });
			nudNumber.Name = "nudNumber";
			nudNumber.Size = new System.Drawing.Size(64, 20);
			nudNumber.TabIndex = 4;
			nudNumber.Value = new decimal(new int[4] { 100, 0, 0, 0 });
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(36, 35);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(52, 13);
			label4.TabIndex = 2;
			label4.Text = "Số lượng ";
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(219, 37);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(35, 13);
			label5.TabIndex = 2;
			label5.Text = "Width";
			nudWidth.Location = new System.Drawing.Point(260, 33);
			nudWidth.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudWidth.Minimum = new decimal(new int[4] { 5, 0, 0, 0 });
			nudWidth.Name = "nudWidth";
			nudWidth.Size = new System.Drawing.Size(64, 20);
			nudWidth.TabIndex = 4;
			nudWidth.Value = new decimal(new int[4] { 512, 0, 0, 0 });
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(365, 37);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(38, 13);
			label6.TabIndex = 2;
			label6.Text = "Height";
			nhdHeight.Location = new System.Drawing.Point(409, 33);
			nhdHeight.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nhdHeight.Minimum = new decimal(new int[4] { 5, 0, 0, 0 });
			nhdHeight.Name = "nhdHeight";
			nhdHeight.Size = new System.Drawing.Size(64, 20);
			nhdHeight.TabIndex = 4;
			nhdHeight.Value = new decimal(new int[4] { 512, 0, 0, 0 });
			btnDownload.Location = new System.Drawing.Point(500, 31);
			btnDownload.Name = "btnDownload";
			btnDownload.Size = new System.Drawing.Size(75, 23);
			btnDownload.TabIndex = 0;
			btnDownload.Text = "Download";
			btnDownload.UseVisualStyleBackColor = true;
			btnDownload.Click += new System.EventHandler(btnDownload_Click);
			lblMsg.AutoSize = true;
			lblMsg.Location = new System.Drawing.Point(271, 73);
			lblMsg.Name = "lblMsg";
			lblMsg.Size = new System.Drawing.Size(24, 13);
			lblMsg.TabIndex = 5;
			lblMsg.Text = "0/0";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(658, 324);
			base.Controls.Add(groupBox2);
			base.Controls.Add(nudDelay);
			base.Controls.Add(cbxDelete);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(txtDialog);
			base.Controls.Add(btnSave);
			base.Controls.Add(btnSelect);
			base.Controls.Add(groupBox1);
			base.Name = "frmOpenFolderDialog";
			Text = "frmOpenFolderDialog";
			base.Load += new System.EventHandler(frmOpenFolderDialog_Load);
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudNumber).EndInit();
			((System.ComponentModel.ISupportInitialize)nudWidth).EndInit();
			((System.ComponentModel.ISupportInitialize)nhdHeight).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
