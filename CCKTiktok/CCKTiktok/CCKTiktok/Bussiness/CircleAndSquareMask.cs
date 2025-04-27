using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CCKTiktok.Bussiness
{
	public class CircleAndSquareMask
	{
		private Bitmap originalImage;

		public CircleAndSquareMask(string imagePath)
		{
			originalImage = new Bitmap(imagePath);
		}

		public void ApplyMaskAndSave(string outputPath, int size)
		{
			int width = originalImage.Width;
			int num = width / 2;
			int num2 = width / 2;
			Bitmap bitmap = new Bitmap(width, width, PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				using (Brush brush = new SolidBrush(Color.White))
				{
					graphics.FillRectangle(brush, 0, 0, width, width);
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.FillEllipse(brush, num - size / 2, num2 - size / 2, size, size);
				}
				using GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddEllipse(num - size / 2, num2 - size / 2, size, size);
				graphicsPath.AddRectangle(new Rectangle(0, 0, width, width));
				using Region region = new Region(graphicsPath);
				graphics.SetClip(region, CombineMode.Replace);
				graphics.DrawImage(originalImage, new Rectangle(0, 0, width, width));
			}
			bitmap.Save(outputPath, ImageFormat.Png);
			bitmap.Dispose();
		}
	}
}
