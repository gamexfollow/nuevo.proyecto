using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmInputControl : Form
	{
		private IContainer components = null;

		private TextBox input;

		private Label labelInput;

		private Button btnOk;

		private Label lblNote;

		public string Result { get; set; }

		public string LabelResult { get; set; }

		public frmInputControl(bool isSingleLine = true, string msg = "Nhập vào đây", string note = "")
		{
			InitializeComponent();
			Result = "";
			LabelResult = msg;
			labelInput.Text = msg;
			if (!isSingleLine)
			{
				input.Multiline = true;
				input.Height = 150;
				input.ScrollBars = ScrollBars.Both;
			}
			else
			{
				input.Height = 50;
			}
			lblNote.Text = note;
		}

		public frmInputControl(string labelResult, string Result = "")
		{
			InitializeComponent();
			this.Result = Result;
			LabelResult = labelResult;
		}

		private void InputBox_Load(object sender, EventArgs e)
		{
			labelInput.Text = LabelResult;
			input.Text = Result;
		}

		public void SetText(string txt)
		{
			input.Text = txt;
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			Result = input.Text.Trim();
			Close();
		}

		private void frmInputControl_Load(object sender, EventArgs e)
		{
		}

		private void frmInputControl_FormClosing(object sender, FormClosingEventArgs e)
		{
			Result = input.Text.Trim();
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
			input = new System.Windows.Forms.TextBox();
			labelInput = new System.Windows.Forms.Label();
			btnOk = new System.Windows.Forms.Button();
			lblNote = new System.Windows.Forms.Label();
			SuspendLayout();
			input.Location = new System.Drawing.Point(63, 49);
			input.MaxLength = 999999999;
			input.Name = "input";
			input.Size = new System.Drawing.Size(286, 20);
			input.TabIndex = 5;
			labelInput.AutoSize = true;
			labelInput.Location = new System.Drawing.Point(62, 27);
			labelInput.Name = "labelInput";
			labelInput.Size = new System.Drawing.Size(75, 13);
			labelInput.TabIndex = 4;
			labelInput.Text = "Nhập vào đây";
			btnOk.Location = new System.Drawing.Point(367, 49);
			btnOk.Name = "btnOk";
			btnOk.Size = new System.Drawing.Size(77, 23);
			btnOk.TabIndex = 3;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			btnOk.Click += new System.EventHandler(btnOk_Click);
			lblNote.AutoSize = true;
			lblNote.Location = new System.Drawing.Point(62, 208);
			lblNote.Name = "lblNote";
			lblNote.Size = new System.Drawing.Size(44, 13);
			lblNote.TabIndex = 6;
			lblNote.Text = "Ghi chú";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(524, 253);
			base.Controls.Add(lblNote);
			base.Controls.Add(input);
			base.Controls.Add(labelInput);
			base.Controls.Add(btnOk);
			base.Margin = new System.Windows.Forms.Padding(2);
			base.Name = "frmInputControl";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Input Control";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmInputControl_FormClosing);
			base.Load += new System.EventHandler(frmInputControl_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
