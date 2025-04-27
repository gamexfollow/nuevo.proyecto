using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CCKTiktok.Bussiness
{
	public class CircleCopier
	{
		private Bitmap originalImage;

		public CircleCopier(string imagePath)
		{
			originalImage = new Bitmap(imagePath);
		}

		public void CopyTransparentCircleToNewImage(string outputPath)
		{
			int width = originalImage.Width;
			int num = 180;
			int num2 = width / 2;
			int num3 = width / 2;
			Bitmap bitmap = new Bitmap(width, width, PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				using (Brush brush = new SolidBrush(Color.FromArgb(0, 255, 255, 255)))
				{
					graphics.FillRectangle(brush, 0, 0, width, width);
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.FillEllipse(brush, num2 - num / 2, num3 - num / 2, num, num);
				}
				using ImageAttributes imageAttributes = new ImageAttributes();
				imageAttributes.SetColorKey(Color.FromArgb(0, 255, 255, 255), Color.FromArgb(0, 255, 255, 255));
				graphics.DrawImage(originalImage, new Rectangle(0, 0, width, width), num2 - num / 2, num3 - num / 2, num, num, GraphicsUnit.Pixel, imageAttributes);
			}
			bitmap.Save(outputPath, ImageFormat.Png);
			bitmap.Dispose();
		}

		public void CopyCircleToNewImage2(string outputPath)
		{
			int width = originalImage.Width;
			int num = 180;
			int num2 = width / 2;
			int num3 = width / 2;
			Bitmap bitmap = new Bitmap(width, width);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(originalImage, new Rectangle(0, 0, width, width), new Rectangle(num2 - num / 2, num3 - num / 2, num, num), GraphicsUnit.Pixel);
			}
			bitmap.Save(outputPath);
			bitmap.Dispose();
		}
	}
}
