using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CCKTiktok.Bussiness
{
	public class CropCircleFromSquare
	{
		private Bitmap originalImage;

		public CropCircleFromSquare(string imagePath)
		{
			originalImage = new Bitmap(imagePath);
		}

		public void CropCircleAndSave(string outputPath, int w)
		{
			int width = originalImage.Width;
			int num = width / 2;
			int num2 = width / 2;
			using Bitmap bitmap = new Bitmap(width, width, PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				using GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddEllipse(num - w / 2, num2 - w / 2, w, w);
				using Region region = new Region(graphicsPath);
				graphics.SetClip(region, CombineMode.Replace);
				graphics.DrawImage(originalImage, new Rectangle(0, 0, width, width));
			}
			originalImage = bitmap;
			int width2 = originalImage.Width;
			int num3 = width2 / 2;
			int num4 = width2 / 2;
			int srcX = num3 - w / 2;
			int srcY = num4 - w / 2;
			int srcWidth = w;
			int srcHeight = w;
			using (Bitmap bitmap2 = new Bitmap(w, w))
			{
				using (Graphics graphics2 = Graphics.FromImage(bitmap2))
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(0, 255, 255, 255)))
					{
						graphics2.FillRectangle(brush, 0, 0, w, w);
					}
					graphics2.SmoothingMode = SmoothingMode.AntiAlias;
					graphics2.DrawImage(originalImage, new Rectangle(0, 0, w, w), srcX, srcY, srcWidth, srcHeight, GraphicsUnit.Pixel);
				}
				bitmap2.Save(outputPath, ImageFormat.Png);
				bitmap2.Dispose();
			}
			bitmap.Dispose();
		}
	}
}
