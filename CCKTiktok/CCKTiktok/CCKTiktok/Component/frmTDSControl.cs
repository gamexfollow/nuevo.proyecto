using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmTDSControl : Form
	{
		private string sourceData = "";

		private IContainer components = null;

		private Label label2;

		private NumericUpDown nudDelayFrom;

		private NumericUpDown nudNumber;

		private NumericUpDown nudDelayTo;

		private Label label3;

		private Label label4;

		private Button btnSave;

		private Label lblHeader;

		private Label label1;

		private Label label5;

		private Label label6;

		private NumericUpDown nudNhanTien;

		private Label label7;

		private BackgroundWorker backgroundWorker1;

		private GroupBox groupBox1;

		private Label label12;

		private Label label13;

		private NumericUpDown nudStop;

		public frmTDSControl(string label, string sourceData)
		{
			InitializeComponent();
			lblHeader.Text = label;
			this.sourceData = sourceData;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			TDSConfig tDSConfig = new TDSConfig();
			tDSConfig.DelayTo = Convert.ToInt32(nudDelayTo.Value);
			tDSConfig.DelayFrom = Convert.ToInt32(nudDelayFrom.Value);
			tDSConfig.Count = Convert.ToInt32(nudNumber.Value);
			tDSConfig.NhanTien = Convert.ToInt32(nudNhanTien.Value);
			tDSConfig.Stop = Convert.ToInt32(nudStop.Value);
			File.WriteAllText(sourceData, new JavaScriptSerializer().Serialize(tDSConfig));
			Close();
		}

		private void frmTDSControl_Load(object sender, EventArgs e)
		{
			if (sourceData != null && File.Exists(sourceData))
			{
				TDSConfig tDSConfig = new JavaScriptSerializer().Deserialize<TDSConfig>(Utils.ReadTextFile(sourceData));
				if (tDSConfig != null)
				{
					nudDelayTo.Value = tDSConfig.DelayTo;
					nudDelayFrom.Value = tDSConfig.DelayFrom;
					nudNumber.Value = tDSConfig.Count;
					nudNhanTien.Value = tDSConfig.NhanTien;
					nudStop.Value = tDSConfig.Stop;
				}
			}
		}

		private void lblHeader_SizeChanged(object sender, EventArgs e)
		{
			lblHeader.Left = (base.ClientSize.Width - lblHeader.Size.Width) / 2;
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
			label2 = new System.Windows.Forms.Label();
			nudDelayFrom = new System.Windows.Forms.NumericUpDown();
			nudNumber = new System.Windows.Forms.NumericUpDown();
			nudDelayTo = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			btnSave = new System.Windows.Forms.Button();
			lblHeader = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			nudNhanTien = new System.Windows.Forms.NumericUpDown();
			label7 = new System.Windows.Forms.Label();
			backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label12 = new System.Windows.Forms.Label();
			label13 = new System.Windows.Forms.Label();
			nudStop = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)nudDelayFrom).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudNumber).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudNhanTien).BeginInit();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudStop).BeginInit();
			SuspendLayout();
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(43, 60);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(86, 13);
			label2.TabIndex = 18;
			label2.Text = "Thời gian nghỉ từ";
			nudDelayFrom.Location = new System.Drawing.Point(146, 58);
			nudDelayFrom.Maximum = new decimal(new int[4] { 99999, 0, 0, 0 });
			nudDelayFrom.Name = "nudDelayFrom";
			nudDelayFrom.Size = new System.Drawing.Size(67, 20);
			nudDelayFrom.TabIndex = 16;
			nudNumber.Location = new System.Drawing.Point(146, 25);
			nudNumber.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudNumber.Name = "nudNumber";
			nudNumber.Size = new System.Drawing.Size(67, 20);
			nudNumber.TabIndex = 15;
			nudDelayTo.Location = new System.Drawing.Point(146, 92);
			nudDelayTo.Maximum = new decimal(new int[4] { 999999, 0, 0, 0 });
			nudDelayTo.Name = "nudDelayTo";
			nudDelayTo.Size = new System.Drawing.Size(67, 20);
			nudDelayTo.TabIndex = 16;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(80, 28);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(49, 13);
			label3.TabIndex = 17;
			label3.Text = "Số lượng";
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(33, 94);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(96, 13);
			label4.TabIndex = 18;
			label4.Text = "Thời gian nghỉ đến";
			btnSave.Location = new System.Drawing.Point(150, 296);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(89, 38);
			btnSave.TabIndex = 19;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			lblHeader.AutoSize = true;
			lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 254);
			lblHeader.Location = new System.Drawing.Point(57, 26);
			lblHeader.Name = "lblHeader";
			lblHeader.Size = new System.Drawing.Size(50, 25);
			lblHeader.TabIndex = 20;
			lblHeader.Text = "Msg";
			lblHeader.SizeChanged += new System.EventHandler(lblHeader_SizeChanged);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(219, 60);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(26, 13);
			label1.TabIndex = 21;
			label1.Text = "giây";
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(219, 94);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(26, 13);
			label5.TabIndex = 22;
			label5.Text = "giây";
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(56, 128);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(73, 13);
			label6.TabIndex = 24;
			label6.Text = "Nhận tiền sau";
			nudNhanTien.Location = new System.Drawing.Point(146, 126);
			nudNhanTien.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudNhanTien.Minimum = new decimal(new int[4] { 8, 0, 0, 0 });
			nudNhanTien.Name = "nudNhanTien";
			nudNhanTien.Size = new System.Drawing.Size(67, 20);
			nudNhanTien.TabIndex = 23;
			nudNhanTien.Value = new decimal(new int[4] { 8, 0, 0, 0 });
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(219, 128);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(21, 13);
			label7.TabIndex = 25;
			label7.Text = "job";
			groupBox1.Controls.Add(label12);
			groupBox1.Controls.Add(label13);
			groupBox1.Controls.Add(nudStop);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(label7);
			groupBox1.Controls.Add(nudDelayFrom);
			groupBox1.Controls.Add(label6);
			groupBox1.Controls.Add(nudNumber);
			groupBox1.Controls.Add(nudNhanTien);
			groupBox1.Controls.Add(nudDelayTo);
			groupBox1.Controls.Add(label5);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(label4);
			groupBox1.Location = new System.Drawing.Point(53, 75);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(278, 197);
			groupBox1.TabIndex = 26;
			groupBox1.TabStop = false;
			groupBox1.Text = "Cấu hình kiếm tiền";
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(219, 164);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(21, 13);
			label12.TabIndex = 28;
			label12.Text = "job";
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(33, 162);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(96, 13);
			label13.TabIndex = 27;
			label13.Text = "Chuyển nick khi lỗi";
			nudStop.Location = new System.Drawing.Point(146, 160);
			nudStop.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudStop.Name = "nudStop";
			nudStop.Size = new System.Drawing.Size(67, 20);
			nudStop.TabIndex = 26;
			nudStop.Value = new decimal(new int[4] { 5, 0, 0, 0 });
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(377, 413);
			base.Controls.Add(groupBox1);
			base.Controls.Add(lblHeader);
			base.Controls.Add(btnSave);
			base.Name = "frmTDSControl";
			Text = "Cấu hình TDS";
			base.Load += new System.EventHandler(frmTDSControl_Load);
			((System.ComponentModel.ISupportInitialize)nudDelayFrom).EndInit();
			((System.ComponentModel.ISupportInitialize)nudNumber).EndInit();
			((System.ComponentModel.ISupportInitialize)nudDelayTo).EndInit();
			((System.ComponentModel.ISupportInitialize)nudNhanTien).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudStop).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
