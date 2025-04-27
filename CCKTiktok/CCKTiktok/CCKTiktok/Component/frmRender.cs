using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmRender : Form
	{
		private IContainer components = null;

		private Button btnPicture;

		private Button btnVideo;

		private DataGridView dataGridView1;

		private DataGridViewTextBoxColumn Stt;

		private DataGridViewTextBoxColumn Link;

		private DataGridViewTextBoxColumn Status;

		private TextBox txtPicture;

		private TextBox txtVideo;

		private Button btnStart;

		private Button btnCrop;

		private NumericUpDown nudTop;

		private NumericUpDown nudHeight;

		private Label label1;

		private Label label2;

		private GroupBox groupBox1;

		private NumericUpDown nudVideoHeight;

		private Label label4;

		private NumericUpDown nudVideoWitdh;

		private Label label3;

		private Button btnOutput;

		private TextBox txtOutput;

		private GroupBox groupBox2;

		private Button btnClear;

		private NumericUpDown nudRatio;

		private Label label5;

		private GroupBox groupBox3;

		private Label label6;

		private NumericUpDown nudThread;

		public frmRender()
		{
			InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			DataGridViewRowCollection rows = dataGridView1.Rows;
			Queue<string> queue = new Queue<string>();
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].Cells[1].Value != null)
				{
					queue.Enqueue(rows[i].Cells[1].Value.ToString());
				}
			}
			string inputVideo = txtVideo.Text;
			int imgW = Utils.Convert2Int((int)nudVideoWitdh.Value * 8 / 10);
			string outputFolder = txtOutput.Text;
			while (queue.Count > 0)
			{
				RenderVideo(queue.Dequeue(), inputVideo, imgW, 0, outputFolder);
			}
		}

		private void btnPicture_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Chọn file ảnh";
			openFileDialog.Filter = "Ảnh|*.jpg;*.jpeg;*.png";
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string[] fileNames = openFileDialog.FileNames;
			if (fileNames.Length != 0)
			{
				string directoryName = Path.GetDirectoryName(fileNames[0]);
				Directory.GetFiles(directoryName);
				txtPicture.Text = directoryName;
				int num = 1;
				string[] array = fileNames;
				foreach (string text in array)
				{
					dataGridView1.Rows.Add(num++, text);
				}
			}
		}

		private string RenderPicture(string sourceImagePath, int targetWidth, int targetHeight)
		{
			Bitmap bitmap = new Bitmap(sourceImagePath);
			Bitmap bitmap2 = new Bitmap(targetWidth, targetHeight);
			using (Graphics graphics = Graphics.FromImage(bitmap2))
			{
				graphics.DrawImage(bitmap, 0, 0, targetWidth, targetHeight);
			}
			string text = $"{Path.GetFileNameWithoutExtension(sourceImagePath)}_{targetWidth}_{targetHeight}.{Path.GetExtension(sourceImagePath)}";
			bitmap2.Save(text);
			bitmap.Dispose();
			bitmap2.Dispose();
			return text;
		}

		private void btnVideo_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Chọn file video mp4";
			openFileDialog.Filter = "Video|*.mp4";
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string selectedFilePath = openFileDialog.FileName;
				txtVideo.Text = selectedFilePath;
				Task.Run(delegate
				{
					Size size = GetSize(selectedFilePath);
					nudVideoWitdh.Value = size.Width;
					nudVideoHeight.Value = size.Height;
				});
			}
		}

		private Size GetSize(string videoPath)
		{
			FileInfo fileInfo = new FileInfo(videoPath);
			Thread.Sleep(1000);
			MediaPlayer mediaPlayer = new MediaPlayer();
			mediaPlayer.Open(new Uri(fileInfo.FullName));
			Thread.Sleep(1000);
			int naturalVideoWidth = mediaPlayer.NaturalVideoWidth;
			int naturalVideoHeight = mediaPlayer.NaturalVideoHeight;
			Thread.Sleep(500);
			mediaPlayer.Close();
			return new Size(naturalVideoWidth, naturalVideoHeight);
		}

		private void RenderVideo(string inputPicture, string inputVideo, int imgW, int imgH, string outputFolder)
		{
			if (!Directory.Exists("BatFile"))
			{
				Directory.CreateDirectory("BatFile");
			}
			string text = outputFolder.TrimEnd('\\') + "\\" + Path.GetFileNameWithoutExtension(inputPicture) + ".mp4";
			using Image image = Image.FromFile(inputPicture);
			inputPicture = RenderPicture(inputPicture, imgW, Convert.ToInt32(image.Height * imgW / image.Width));
			string contents = "ffmpeg -y -i \"" + inputVideo + "\" -i \"" + inputPicture + "\" -filter_complex \"[0:v]scale = iw * 1:-1[bg]; [bg][1:v]overlay = (W - w) / 2:(H - h) / 2\" -c:a copy \"" + text + "\"";
			string text2 = "BatFile\\" + Guid.NewGuid().ToString() + ".bat";
			File.WriteAllText(text2, contents);
			Process.Start(text2);
		}

		private string CropPicture(string imagePath, int topPosition = 120, int croppedHeight = 830)
		{
			Path.GetExtension(imagePath);
			string directoryName = Path.GetDirectoryName(imagePath);
			if (!Directory.Exists(directoryName + "\\Crop"))
			{
				Directory.CreateDirectory(directoryName + "\\Crop");
			}
			string text = directoryName + "\\Crop\\" + Path.GetFileName(imagePath);
			Bitmap bitmap = new Bitmap(imagePath);
			Bitmap bitmap2 = new Bitmap(bitmap.Width, croppedHeight);
			using (Graphics graphics = Graphics.FromImage(bitmap2))
			{
				graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), new Rectangle(0, topPosition, bitmap.Width, croppedHeight), GraphicsUnit.Pixel);
			}
			bitmap2.Save(text);
			bitmap.Dispose();
			bitmap2.Dispose();
			return text;
		}

		private void btnCrop_Click(object sender, EventArgs e)
		{
			decimal value = nudTop.Value;
			decimal value2 = nudHeight.Value;
			DataGridViewRowCollection rows = dataGridView1.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].Cells[1].Value != null)
				{
					CropPicture(rows[i].Cells[1].Value.ToString(), Utils.Convert2Int(value), Utils.Convert2Int(value2));
				}
			}
		}

		private void btnOutput_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = false;
			openFileDialog.CheckFileExists = false;
			openFileDialog.CheckPathExists = true;
			openFileDialog.FileName = "Folder Selection.";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string directoryName = Path.GetDirectoryName(openFileDialog.FileName);
				txtOutput.Text = directoryName;
			}
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			dataGridView1.Rows.Clear();
		}

		private void nudVideoWitdh_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\videoW.txt", nudVideoWitdh.Value.ToString());
		}

		private void nudVideoHeight_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\videoH.txt", nudVideoHeight.Value.ToString());
		}

		private void nudTop_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\imgT.txt", nudTop.Value.ToString());
		}

		private void nudHeight_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\imgH.txt", nudHeight.Value.ToString());
		}

		private void frmRender_Load(object sender, EventArgs e)
		{
			nudVideoWitdh.Value = Utils.Convert2Int(Utils.ReadTextFile("Config\\videoW.txt"), 1080);
			nudVideoHeight.Value = Utils.Convert2Int(Utils.ReadTextFile("Config\\videoH.txt"), 1920);
			nudTop.Value = Utils.Convert2Int(Utils.ReadTextFile("Config\\imgT.txt"), 125);
			nudHeight.Value = Utils.Convert2Int(Utils.ReadTextFile("Config\\imgH.txt"), 800);
			nudRatio.Value = Utils.Convert2Int(Utils.ReadTextFile("Config\\ratio.txt"), 80);
			txtPicture.Text = Utils.ReadTextFile("Config\\txtPicture.txt");
			txtVideo.Text = Utils.ReadTextFile("Config\\txtVideo.txt");
			txtOutput.Text = Utils.ReadTextFile("Config\\txtOutput.txt");
		}

		private void txtPicture_TextChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\txtPicture.txt", txtPicture.Text.ToString());
		}

		private void txtVideo_TextChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\txtVideo.txt", txtVideo.Text.ToString());
		}

		private void txtOutput_TextChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\txtOutput.txt", txtOutput.Text.ToString());
		}

		private void nudRatio_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText("Config\\ratio.txt", nudRatio.Value.ToString());
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
			btnPicture = new System.Windows.Forms.Button();
			btnVideo = new System.Windows.Forms.Button();
			dataGridView1 = new System.Windows.Forms.DataGridView();
			Stt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			Link = new System.Windows.Forms.DataGridViewTextBoxColumn();
			Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
			txtPicture = new System.Windows.Forms.TextBox();
			txtVideo = new System.Windows.Forms.TextBox();
			btnStart = new System.Windows.Forms.Button();
			btnCrop = new System.Windows.Forms.Button();
			nudTop = new System.Windows.Forms.NumericUpDown();
			nudHeight = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			nudVideoHeight = new System.Windows.Forms.NumericUpDown();
			label4 = new System.Windows.Forms.Label();
			nudVideoWitdh = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			btnOutput = new System.Windows.Forms.Button();
			txtOutput = new System.Windows.Forms.TextBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			nudRatio = new System.Windows.Forms.NumericUpDown();
			label5 = new System.Windows.Forms.Label();
			btnClear = new System.Windows.Forms.Button();
			groupBox3 = new System.Windows.Forms.GroupBox();
			nudThread = new System.Windows.Forms.NumericUpDown();
			label6 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudTop).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudHeight).BeginInit();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudVideoHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudVideoWitdh).BeginInit();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudRatio).BeginInit();
			groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudThread).BeginInit();
			SuspendLayout();
			btnPicture.Location = new System.Drawing.Point(171, 11);
			btnPicture.Name = "btnPicture";
			btnPicture.Size = new System.Drawing.Size(146, 24);
			btnPicture.TabIndex = 0;
			btnPicture.Text = "Open Picture Folder";
			btnPicture.UseVisualStyleBackColor = true;
			btnPicture.Click += new System.EventHandler(btnPicture_Click);
			btnVideo.Location = new System.Drawing.Point(171, 43);
			btnVideo.Name = "btnVideo";
			btnVideo.Size = new System.Drawing.Size(146, 25);
			btnVideo.TabIndex = 0;
			btnVideo.Text = "Open Video File";
			btnVideo.UseVisualStyleBackColor = true;
			btnVideo.Click += new System.EventHandler(btnVideo_Click);
			dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Columns.AddRange(Stt, Link, Status);
			dataGridView1.Location = new System.Drawing.Point(12, 130);
			dataGridView1.Name = "dataGridView1";
			dataGridView1.Size = new System.Drawing.Size(905, 511);
			dataGridView1.TabIndex = 1;
			Stt.HeaderText = "Stt";
			Stt.Name = "Stt";
			Stt.Width = 50;
			Link.HeaderText = "Link";
			Link.Name = "Link";
			Link.Width = 700;
			Status.HeaderText = "Status";
			Status.Name = "Status";
			txtPicture.Location = new System.Drawing.Point(13, 14);
			txtPicture.Name = "txtPicture";
			txtPicture.Size = new System.Drawing.Size(152, 20);
			txtPicture.TabIndex = 2;
			txtPicture.TextChanged += new System.EventHandler(txtPicture_TextChanged);
			txtVideo.Location = new System.Drawing.Point(12, 46);
			txtVideo.Name = "txtVideo";
			txtVideo.Size = new System.Drawing.Size(153, 20);
			txtVideo.TabIndex = 2;
			txtVideo.TextChanged += new System.EventHandler(txtVideo_TextChanged);
			btnStart.Location = new System.Drawing.Point(813, 10);
			btnStart.Name = "btnStart";
			btnStart.Size = new System.Drawing.Size(104, 56);
			btnStart.TabIndex = 0;
			btnStart.Text = "Start";
			btnStart.UseVisualStyleBackColor = true;
			btnStart.Click += new System.EventHandler(btnStart_Click);
			btnCrop.Location = new System.Drawing.Point(172, 15);
			btnCrop.Name = "btnCrop";
			btnCrop.Size = new System.Drawing.Size(104, 56);
			btnCrop.TabIndex = 0;
			btnCrop.Text = "Crop Picture";
			btnCrop.UseVisualStyleBackColor = true;
			btnCrop.Click += new System.EventHandler(btnCrop_Click);
			nudTop.Location = new System.Drawing.Point(101, 17);
			nudTop.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudTop.Minimum = new decimal(new int[4] { 10, 0, 0, 0 });
			nudTop.Name = "nudTop";
			nudTop.Size = new System.Drawing.Size(56, 20);
			nudTop.TabIndex = 3;
			nudTop.Value = new decimal(new int[4] { 125, 0, 0, 0 });
			nudTop.ValueChanged += new System.EventHandler(nudTop_ValueChanged);
			nudHeight.Location = new System.Drawing.Point(101, 41);
			nudHeight.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudHeight.Minimum = new decimal(new int[4] { 100, 0, 0, 0 });
			nudHeight.Name = "nudHeight";
			nudHeight.Size = new System.Drawing.Size(56, 20);
			nudHeight.TabIndex = 3;
			nudHeight.Value = new decimal(new int[4] { 830, 0, 0, 0 });
			nudHeight.ValueChanged += new System.EventHandler(nudHeight_ValueChanged);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(69, 19);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(26, 13);
			label1.TabIndex = 4;
			label1.Text = "Top";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(57, 43);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(38, 13);
			label2.TabIndex = 4;
			label2.Text = "Height";
			groupBox1.Controls.Add(nudVideoHeight);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(nudVideoWitdh);
			groupBox1.Controls.Add(label3);
			groupBox1.Location = new System.Drawing.Point(323, 2);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(148, 71);
			groupBox1.TabIndex = 5;
			groupBox1.TabStop = false;
			groupBox1.Text = "Video Size";
			nudVideoHeight.Location = new System.Drawing.Point(64, 46);
			nudVideoHeight.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudVideoHeight.Minimum = new decimal(new int[4] { 100, 0, 0, 0 });
			nudVideoHeight.Name = "nudVideoHeight";
			nudVideoHeight.Size = new System.Drawing.Size(56, 20);
			nudVideoHeight.TabIndex = 3;
			nudVideoHeight.Value = new decimal(new int[4] { 830, 0, 0, 0 });
			nudVideoHeight.ValueChanged += new System.EventHandler(nudVideoHeight_ValueChanged);
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(20, 48);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(38, 13);
			label4.TabIndex = 4;
			label4.Text = "Height";
			nudVideoWitdh.Location = new System.Drawing.Point(64, 18);
			nudVideoWitdh.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudVideoWitdh.Minimum = new decimal(new int[4] { 10, 0, 0, 0 });
			nudVideoWitdh.Name = "nudVideoWitdh";
			nudVideoWitdh.Size = new System.Drawing.Size(56, 20);
			nudVideoWitdh.TabIndex = 3;
			nudVideoWitdh.Value = new decimal(new int[4] { 125, 0, 0, 0 });
			nudVideoWitdh.ValueChanged += new System.EventHandler(nudVideoWitdh_ValueChanged);
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(20, 22);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(35, 13);
			label3.TabIndex = 4;
			label3.Text = "Width";
			btnOutput.Location = new System.Drawing.Point(171, 78);
			btnOutput.Name = "btnOutput";
			btnOutput.Size = new System.Drawing.Size(146, 25);
			btnOutput.TabIndex = 0;
			btnOutput.Text = "Output Folder";
			btnOutput.UseVisualStyleBackColor = true;
			btnOutput.Click += new System.EventHandler(btnOutput_Click);
			txtOutput.Location = new System.Drawing.Point(12, 81);
			txtOutput.Name = "txtOutput";
			txtOutput.Size = new System.Drawing.Size(153, 20);
			txtOutput.TabIndex = 2;
			txtOutput.TextChanged += new System.EventHandler(txtOutput_TextChanged);
			groupBox2.Controls.Add(label1);
			groupBox2.Controls.Add(nudRatio);
			groupBox2.Controls.Add(label5);
			groupBox2.Controls.Add(nudHeight);
			groupBox2.Controls.Add(label2);
			groupBox2.Controls.Add(nudTop);
			groupBox2.Controls.Add(btnCrop);
			groupBox2.Location = new System.Drawing.Point(488, 2);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(282, 122);
			groupBox2.TabIndex = 6;
			groupBox2.TabStop = false;
			groupBox2.Text = "Crop Picture";
			nudRatio.Location = new System.Drawing.Point(101, 67);
			nudRatio.Minimum = new decimal(new int[4] { 20, 0, 0, 0 });
			nudRatio.Name = "nudRatio";
			nudRatio.Size = new System.Drawing.Size(56, 20);
			nudRatio.TabIndex = 3;
			nudRatio.Value = new decimal(new int[4] { 80, 0, 0, 0 });
			nudRatio.ValueChanged += new System.EventHandler(nudRatio_ValueChanged);
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(8, 71);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(88, 13);
			label5.TabIndex = 4;
			label5.Text = "Tỷ lệ ảnh / video";
			btnClear.Location = new System.Drawing.Point(813, 68);
			btnClear.Name = "btnClear";
			btnClear.Size = new System.Drawing.Size(104, 34);
			btnClear.TabIndex = 0;
			btnClear.Text = "Clear";
			btnClear.UseVisualStyleBackColor = true;
			btnClear.Click += new System.EventHandler(btnClear_Click);
			groupBox3.Controls.Add(label6);
			groupBox3.Controls.Add(nudThread);
			groupBox3.Location = new System.Drawing.Point(324, 74);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(147, 50);
			groupBox3.TabIndex = 7;
			groupBox3.TabStop = false;
			groupBox3.Text = "Số luồng chạy";
			nudThread.Location = new System.Drawing.Point(26, 21);
			nudThread.Maximum = new decimal(new int[4] { 30, 0, 0, 0 });
			nudThread.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
			nudThread.Name = "nudThread";
			nudThread.Size = new System.Drawing.Size(56, 20);
			nudThread.TabIndex = 3;
			nudThread.Value = new decimal(new int[4] { 10, 0, 0, 0 });
			nudThread.ValueChanged += new System.EventHandler(nudRatio_ValueChanged);
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(87, 24);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(37, 13);
			label6.TabIndex = 4;
			label6.Text = "Luồng";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(934, 677);
			base.Controls.Add(groupBox3);
			base.Controls.Add(groupBox1);
			base.Controls.Add(txtOutput);
			base.Controls.Add(txtVideo);
			base.Controls.Add(txtPicture);
			base.Controls.Add(btnOutput);
			base.Controls.Add(dataGridView1);
			base.Controls.Add(btnVideo);
			base.Controls.Add(btnClear);
			base.Controls.Add(btnStart);
			base.Controls.Add(btnPicture);
			base.Controls.Add(groupBox2);
			base.Name = "frmRender";
			Text = "frmRender";
			base.Load += new System.EventHandler(frmRender_Load);
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			((System.ComponentModel.ISupportInitialize)nudTop).EndInit();
			((System.ComponentModel.ISupportInitialize)nudHeight).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudVideoHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)nudVideoWitdh).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudRatio).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudThread).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
