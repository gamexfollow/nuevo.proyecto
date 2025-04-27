using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CCKTiktok.Component
{
	public class frmRenderVideo : Form
	{
		private IContainer components = null;

		private Button btnPicture;

		public frmRenderVideo()
		{
			InitializeComponent();
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
			SuspendLayout();
			btnPicture.Location = new System.Drawing.Point(23, 23);
			btnPicture.Name = "btnPicture";
			btnPicture.Size = new System.Drawing.Size(123, 23);
			btnPicture.TabIndex = 0;
			btnPicture.Text = "Chonj th";
			btnPicture.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(800, 550);
			base.Controls.Add(btnPicture);
			base.Name = "frmRenderVideo";
			Text = "frmRenderVideo";
			ResumeLayout(false);
		}
	}
}
