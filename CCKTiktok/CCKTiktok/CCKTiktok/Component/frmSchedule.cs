using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmSchedule : Form
	{
		private bool running = true;

		private IContainer components = null;

		private Button btnSchedule;

		private Label lblWorking;

		private DateTimePicker dateTimePickerDay;

		private ComboBox cbxHour;

		private ComboBox cbxMin;

		private Label label1;

		private Label label2;

		private Label label3;

		public DateTime RunTime { get; set; }

		public frmSchedule()
		{
			InitializeComponent();
		}

		private void frmSchedule_Load(object sender, EventArgs e)
		{
			RunTime = DateTime.Now.Date;
			cbxHour.SelectedIndex = DateTime.Now.Hour;
			cbxMin.SelectedIndex = DateTime.Now.Minute - 1;
		}

		private void btnSchedule_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DateTime date = dateTimePickerDay.Value.Date;
				RunTime = Convert.ToDateTime(date.Day + "-" + date.Month + "-" + date.Year + " " + Utils.Convert2Int(cbxHour.SelectedItem.ToString()) + ":" + Utils.Convert2Int(cbxMin.SelectedItem.ToString()), new CultureInfo(1066));
				while (running)
				{
					if (RunTime < DateTime.Now)
					{
						Close();
						break;
					}
					TimeSpan timeSpan = RunTime.Subtract(DateTime.Now);
					lblWorking.Text = $"Phần mềm sẽ chạy sau {timeSpan.Hours} giờ {timeSpan.Minutes} phút {timeSpan.Seconds} giây";
					Application.DoEvents();
					Thread.Sleep(1000);
				}
			}).Start();
		}

		private void frmSchedule_FormClosing(object sender, FormClosingEventArgs e)
		{
			running = false;
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
			btnSchedule = new System.Windows.Forms.Button();
			lblWorking = new System.Windows.Forms.Label();
			dateTimePickerDay = new System.Windows.Forms.DateTimePicker();
			cbxHour = new System.Windows.Forms.ComboBox();
			cbxMin = new System.Windows.Forms.ComboBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			SuspendLayout();
			btnSchedule.Location = new System.Drawing.Point(267, 35);
			btnSchedule.Name = "btnSchedule";
			btnSchedule.Size = new System.Drawing.Size(75, 23);
			btnSchedule.TabIndex = 0;
			btnSchedule.Text = "Save";
			btnSchedule.UseVisualStyleBackColor = true;
			btnSchedule.Click += new System.EventHandler(btnSchedule_Click);
			lblWorking.AutoSize = true;
			lblWorking.Location = new System.Drawing.Point(56, 78);
			lblWorking.Name = "lblWorking";
			lblWorking.Size = new System.Drawing.Size(47, 13);
			lblWorking.TabIndex = 2;
			lblWorking.Text = "Working";
			dateTimePickerDay.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			dateTimePickerDay.Location = new System.Drawing.Point(59, 38);
			dateTimePickerDay.Name = "dateTimePickerDay";
			dateTimePickerDay.Size = new System.Drawing.Size(82, 20);
			dateTimePickerDay.TabIndex = 3;
			cbxHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxHour.FormattingEnabled = true;
			cbxHour.Items.AddRange(new object[24]
			{
				"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
				"10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
				"20", "21", "22", "23"
			});
			cbxHour.Location = new System.Drawing.Point(147, 37);
			cbxHour.Name = "cbxHour";
			cbxHour.Size = new System.Drawing.Size(54, 21);
			cbxHour.TabIndex = 4;
			cbxMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxMin.FormattingEnabled = true;
			cbxMin.Items.AddRange(new object[59]
			{
				"1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
				"11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
				"21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
				"31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
				"41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
				"51", "52", "53", "54", "55", "56", "57", "58", "59"
			});
			cbxMin.Location = new System.Drawing.Point(207, 37);
			cbxMin.Name = "cbxMin";
			cbxMin.Size = new System.Drawing.Size(54, 21);
			cbxMin.TabIndex = 5;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(83, 18);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(30, 13);
			label1.TabIndex = 6;
			label1.Text = "Date";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(157, 18);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(30, 13);
			label2.TabIndex = 7;
			label2.Text = "Hour";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(214, 18);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(39, 13);
			label3.TabIndex = 8;
			label3.Text = "Minute";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(373, 125);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(cbxMin);
			base.Controls.Add(cbxHour);
			base.Controls.Add(dateTimePickerDay);
			base.Controls.Add(lblWorking);
			base.Controls.Add(btnSchedule);
			base.Name = "frmSchedule";
			Text = "Schedule";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmSchedule_FormClosing);
			base.Load += new System.EventHandler(frmSchedule_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
