using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmAddAccountOrder : Form
	{
		private IContainer components = null;

		private Label label2;

		private Label label1;

		private TextBox txtCode;

		private Button btnBatch;

		private Button button2;

		private Button button1;

		private Button btnUp;

		private ListBox lstField;

		private CheckBox cbxPassEmail;

		private CheckBox cbxEmail;

		private CheckBox cbx2fa;

		private CheckBox cbxCookie;

		private CheckBox cbxToken;

		private CheckBox cbxPass;

		private CheckBox cbxUid;

		private CheckBox cbxBirthday;

		private CheckBox cbxBrand;

		private CheckBox cbxProxy;

		public DataTable RetTable { get; set; }

		public frmAddAccountOrder()
		{
			InitializeComponent();
			RetTable = new DataTable();
		}

		private void btnBatch_Click(object sender, EventArgs e)
		{
			RetTable = new DataTable();
			if (!RetTable.Columns.Contains("stt"))
			{
				RetTable.Columns.Add("stt");
			}
			if (!RetTable.Columns.Contains("uid"))
			{
				RetTable.Columns.Add("uid");
			}
			if (!RetTable.Columns.Contains("pass"))
			{
				RetTable.Columns.Add("pass");
			}
			if (!RetTable.Columns.Contains("token"))
			{
				RetTable.Columns.Add("token");
			}
			if (!RetTable.Columns.Contains("cookie"))
			{
				RetTable.Columns.Add("cookie");
			}
			if (!RetTable.Columns.Contains("privatekey"))
			{
				RetTable.Columns.Add("privatekey");
			}
			if (!RetTable.Columns.Contains("email"))
			{
				RetTable.Columns.Add("email");
			}
			if (!RetTable.Columns.Contains("passemail"))
			{
				RetTable.Columns.Add("passemail");
			}
			if (!RetTable.Columns.Contains("birthday"))
			{
				RetTable.Columns.Add("birthday");
			}
			if (!RetTable.Columns.Contains("proxy"))
			{
				RetTable.Columns.Add("proxy");
			}
			if (!RetTable.Columns.Contains("brand"))
			{
				RetTable.Columns.Add("brand");
			}
			string[] array = txtCode.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				label2.Text = "Dữ liệu không hợp lệ";
				return;
			}
			int num = 1;
			string[] array2 = array;
			foreach (string text in array2)
			{
				DataRow dataRow = RetTable.NewRow();
				dataRow["stt"] = num++;
				string[] array3 = text.Split("|,".ToCharArray());
				if (array3.Length < lstField.Items.Count)
				{
					label2.Text = "Dữ liệu không hợp lệ";
					Application.DoEvents();
					continue;
				}
				label1.Text = "";
				for (int j = 0; j < lstField.Items.Count; j++)
				{
					switch (lstField.Items[j].ToString())
					{
					case "Token":
						dataRow["token"] = array3[j];
						break;
					case "Email":
						dataRow["email"] = array3[j];
						break;
					case "Proxy":
						dataRow["Proxy"] = array3[j];
						break;
					case "2FA":
						dataRow["privatekey"] = array3[j];
						break;
					case "brand":
						dataRow["brand"] = array3[j];
						break;
					case "Birthday":
						dataRow["birthday"] = array3[j];
						break;
					case "Pass":
						dataRow["pass"] = array3[j];
						break;
					case "UID":
						dataRow["uid"] = array3[j];
						break;
					case "Cookie":
						dataRow["cookie"] = array3[j];
						break;
					case "PassEmail":
						dataRow["passemail"] = array3[j];
						break;
					}
				}
				RetTable.Rows.InsertAt(dataRow, RetTable.Rows.Count);
				RetTable.AcceptChanges();
			}
			Close();
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
			MoveItem(-1);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MoveItem(1);
		}

		public void MoveItem(int direction)
		{
			if (lstField.SelectedItem != null && lstField.SelectedIndex >= 0)
			{
				int num = lstField.SelectedIndex + direction;
				if (num >= 0 && num < lstField.Items.Count)
				{
					object selectedItem = lstField.SelectedItem;
					lstField.Items.Remove(selectedItem);
					lstField.Items.Insert(num, selectedItem);
					lstField.SetSelected(num, value: true);
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (lstField.SelectedIndex != -1)
			{
				lstField.Items.RemoveAt(lstField.SelectedIndex);
			}
		}

		private void cbxUid_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxUid.Text);
			if (cbxUid.Checked)
			{
				lstField.Items.Add(cbxUid.Text);
			}
		}

		private void cbxPass_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxPass.Text);
			if (cbxPass.Checked)
			{
				lstField.Items.Add(cbxPass.Text);
			}
		}

		private void cbxToken_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxToken.Text);
			if (cbxToken.Checked)
			{
				lstField.Items.Add(cbxToken.Text);
			}
		}

		private void cbxCookie_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxCookie.Text);
			if (cbxCookie.Checked)
			{
				lstField.Items.Add(cbxCookie.Text);
			}
		}

		private void cbx2fa_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbx2fa.Text);
			if (cbx2fa.Checked)
			{
				lstField.Items.Add(cbx2fa.Text);
			}
		}

		private void cbxEmail_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxEmail.Text);
			if (cbxEmail.Checked)
			{
				lstField.Items.Add(cbxEmail.Text);
			}
		}

		private void cbxPassEmail_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxPassEmail.Text);
			if (cbxPassEmail.Checked)
			{
				lstField.Items.Add(cbxPassEmail.Text);
			}
		}

		private void AddAccountOrder_Load(object sender, EventArgs e)
		{
		}

		private void frmAddAccountOrder_Load(object sender, EventArgs e)
		{
		}

		private void cbxBirthday_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxBirthday.Text);
			if (cbxBirthday.Checked)
			{
				lstField.Items.Add(cbxBirthday.Text);
			}
		}

		private void cbxBrand_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxBrand.Text);
			if (cbxBrand.Checked)
			{
				lstField.Items.Add(cbxBrand.Text);
			}
		}

		private void cbxProxy_CheckedChanged(object sender, EventArgs e)
		{
			lstField.Items.Remove(cbxProxy.Text);
			if (cbxProxy.Checked)
			{
				lstField.Items.Add(cbxProxy.Text);
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
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			txtCode = new System.Windows.Forms.TextBox();
			btnBatch = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			button1 = new System.Windows.Forms.Button();
			btnUp = new System.Windows.Forms.Button();
			lstField = new System.Windows.Forms.ListBox();
			cbxPassEmail = new System.Windows.Forms.CheckBox();
			cbxEmail = new System.Windows.Forms.CheckBox();
			cbx2fa = new System.Windows.Forms.CheckBox();
			cbxCookie = new System.Windows.Forms.CheckBox();
			cbxToken = new System.Windows.Forms.CheckBox();
			cbxPass = new System.Windows.Forms.CheckBox();
			cbxUid = new System.Windows.Forms.CheckBox();
			cbxBirthday = new System.Windows.Forms.CheckBox();
			cbxBrand = new System.Windows.Forms.CheckBox();
			cbxProxy = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			label2.AutoSize = true;
			label2.ForeColor = System.Drawing.Color.Red;
			label2.Location = new System.Drawing.Point(287, 603);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(0, 13);
			label2.TabIndex = 43;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(49, 31);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(248, 13);
			label1.TabIndex = 42;
			label1.Text = "Chọn các trường dữ liệu bạn có / Select data fields";
			txtCode.Location = new System.Drawing.Point(398, 59);
			txtCode.MaxLength = 327670000;
			txtCode.Multiline = true;
			txtCode.Name = "txtCode";
			txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtCode.Size = new System.Drawing.Size(361, 285);
			txtCode.TabIndex = 41;
			btnBatch.Location = new System.Drawing.Point(309, 382);
			btnBatch.Margin = new System.Windows.Forms.Padding(2);
			btnBatch.Name = "btnBatch";
			btnBatch.Size = new System.Drawing.Size(163, 37);
			btnBatch.TabIndex = 40;
			btnBatch.Text = "Save";
			btnBatch.UseVisualStyleBackColor = true;
			btnBatch.Click += new System.EventHandler(btnBatch_Click);
			button2.Location = new System.Drawing.Point(305, 186);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(63, 23);
			button2.TabIndex = 39;
			button2.Text = "Remove";
			button2.UseVisualStyleBackColor = true;
			button2.Click += new System.EventHandler(button2_Click);
			button1.Location = new System.Drawing.Point(305, 232);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(63, 23);
			button1.TabIndex = 38;
			button1.Text = "Down";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);
			btnUp.Location = new System.Drawing.Point(306, 145);
			btnUp.Name = "btnUp";
			btnUp.Size = new System.Drawing.Size(63, 23);
			btnUp.TabIndex = 37;
			btnUp.Text = "Up";
			btnUp.UseVisualStyleBackColor = true;
			btnUp.Click += new System.EventHandler(btnUp_Click);
			lstField.FormattingEnabled = true;
			lstField.Location = new System.Drawing.Point(155, 59);
			lstField.Name = "lstField";
			lstField.Size = new System.Drawing.Size(145, 277);
			lstField.TabIndex = 36;
			cbxPassEmail.AutoSize = true;
			cbxPassEmail.Location = new System.Drawing.Point(52, 198);
			cbxPassEmail.Name = "cbxPassEmail";
			cbxPassEmail.Size = new System.Drawing.Size(74, 17);
			cbxPassEmail.TabIndex = 35;
			cbxPassEmail.Text = "PassEmail";
			cbxPassEmail.UseVisualStyleBackColor = true;
			cbxPassEmail.CheckedChanged += new System.EventHandler(cbxPassEmail_CheckedChanged);
			cbxEmail.AutoSize = true;
			cbxEmail.Location = new System.Drawing.Point(52, 174);
			cbxEmail.Name = "cbxEmail";
			cbxEmail.Size = new System.Drawing.Size(51, 17);
			cbxEmail.TabIndex = 34;
			cbxEmail.Text = "Email";
			cbxEmail.UseVisualStyleBackColor = true;
			cbxEmail.CheckedChanged += new System.EventHandler(cbxEmail_CheckedChanged);
			cbx2fa.AutoSize = true;
			cbx2fa.Location = new System.Drawing.Point(52, 151);
			cbx2fa.Name = "cbx2fa";
			cbx2fa.Size = new System.Drawing.Size(45, 17);
			cbx2fa.TabIndex = 33;
			cbx2fa.Text = "2FA";
			cbx2fa.UseVisualStyleBackColor = true;
			cbx2fa.CheckedChanged += new System.EventHandler(cbx2fa_CheckedChanged);
			cbxCookie.AutoSize = true;
			cbxCookie.Location = new System.Drawing.Point(52, 128);
			cbxCookie.Name = "cbxCookie";
			cbxCookie.Size = new System.Drawing.Size(59, 17);
			cbxCookie.TabIndex = 32;
			cbxCookie.Text = "Cookie";
			cbxCookie.UseVisualStyleBackColor = true;
			cbxCookie.CheckedChanged += new System.EventHandler(cbxCookie_CheckedChanged);
			cbxToken.AutoSize = true;
			cbxToken.Location = new System.Drawing.Point(52, 105);
			cbxToken.Name = "cbxToken";
			cbxToken.Size = new System.Drawing.Size(57, 17);
			cbxToken.TabIndex = 31;
			cbxToken.Text = "Token";
			cbxToken.UseVisualStyleBackColor = true;
			cbxToken.CheckedChanged += new System.EventHandler(cbxToken_CheckedChanged);
			cbxPass.AutoSize = true;
			cbxPass.Location = new System.Drawing.Point(52, 83);
			cbxPass.Name = "cbxPass";
			cbxPass.Size = new System.Drawing.Size(49, 17);
			cbxPass.TabIndex = 30;
			cbxPass.Text = "Pass";
			cbxPass.UseVisualStyleBackColor = true;
			cbxPass.CheckedChanged += new System.EventHandler(cbxPass_CheckedChanged);
			cbxUid.AutoSize = true;
			cbxUid.Location = new System.Drawing.Point(52, 59);
			cbxUid.Name = "cbxUid";
			cbxUid.Size = new System.Drawing.Size(45, 17);
			cbxUid.TabIndex = 29;
			cbxUid.Text = "UID";
			cbxUid.UseVisualStyleBackColor = true;
			cbxUid.CheckedChanged += new System.EventHandler(cbxUid_CheckedChanged);
			cbxBirthday.AutoSize = true;
			cbxBirthday.Location = new System.Drawing.Point(52, 221);
			cbxBirthday.Name = "cbxBirthday";
			cbxBirthday.Size = new System.Drawing.Size(64, 17);
			cbxBirthday.TabIndex = 44;
			cbxBirthday.Text = "Birthday";
			cbxBirthday.UseVisualStyleBackColor = true;
			cbxBirthday.CheckedChanged += new System.EventHandler(cbxBirthday_CheckedChanged);
			cbxBrand.AutoSize = true;
			cbxBrand.Location = new System.Drawing.Point(52, 244);
			cbxBrand.Name = "cbxBrand";
			cbxBrand.Size = new System.Drawing.Size(54, 17);
			cbxBrand.TabIndex = 45;
			cbxBrand.Text = "Brand";
			cbxBrand.UseVisualStyleBackColor = true;
			cbxBrand.CheckedChanged += new System.EventHandler(cbxBrand_CheckedChanged);
			cbxProxy.AutoSize = true;
			cbxProxy.Location = new System.Drawing.Point(52, 267);
			cbxProxy.Name = "cbxProxy";
			cbxProxy.Size = new System.Drawing.Size(52, 17);
			cbxProxy.TabIndex = 46;
			cbxProxy.Text = "Proxy";
			cbxProxy.UseVisualStyleBackColor = true;
			cbxProxy.CheckedChanged += new System.EventHandler(cbxProxy_CheckedChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(777, 451);
			base.Controls.Add(cbxProxy);
			base.Controls.Add(cbxBrand);
			base.Controls.Add(cbxBirthday);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(txtCode);
			base.Controls.Add(btnBatch);
			base.Controls.Add(button2);
			base.Controls.Add(button1);
			base.Controls.Add(btnUp);
			base.Controls.Add(lstField);
			base.Controls.Add(cbxPassEmail);
			base.Controls.Add(cbxEmail);
			base.Controls.Add(cbx2fa);
			base.Controls.Add(cbxCookie);
			base.Controls.Add(cbxToken);
			base.Controls.Add(cbxPass);
			base.Controls.Add(cbxUid);
			base.Margin = new System.Windows.Forms.Padding(2);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(793, 365);
			base.Name = "frmAddAccountOrder";
			Text = "frmAddAccountOrder";
			base.Load += new System.EventHandler(frmAddAccountOrder_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
