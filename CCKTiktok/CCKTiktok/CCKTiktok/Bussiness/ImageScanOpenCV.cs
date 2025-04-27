using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CCKTiktok.Bussiness
{
	public class ImageScanOpenCV
	{
		public static Bitmap GetImage(string path)
		{
			return new Bitmap(path);
		}

		public static Bitmap Find(string main, string sub, double percent = 0.9)
		{
			GetImage(main);
			GetImage(sub);
			return Find(main, sub, percent);
		}

		public static Bitmap Find(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
		{
			Image<Bgr, byte> image = mainBitmap.ToImage<Bgr, byte>();
			Image<Bgr, byte> image2 = subBitmap.ToImage<Bgr, byte>();
			Image<Bgr, byte> image3 = image.Copy();
			using (Image<Gray, float> image4 = image.MatchTemplate(image2, TemplateMatchingType.CcoeffNormed))
			{
				image4.MinMax(out var _, out var maxValues, out var _, out var maxLocations);
				if (maxValues[0] > percent)
				{
					Rectangle rect = new Rectangle(maxLocations[0], image2.Size);
					image3.Draw(rect, new Bgr(System.Drawing.Color.Red), 2);
				}
				else
				{
					image3 = null;
				}
			}
			return image3?.ToBitmap();
		}

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		public static Point? FindOutPoint(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
		{
			Point? result2;
			if (subBitmap != null && mainBitmap != null)
			{
				if (subBitmap.Width <= mainBitmap.Width && subBitmap.Height <= mainBitmap.Height)
				{
					Image<Bgr, byte> image = mainBitmap.ToImage<Bgr, byte>();
					Image<Bgr, byte> template = subBitmap.ToImage<Bgr, byte>();
					Point? result = null;
					using (Image<Gray, float> image2 = image.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
					{
						image2.MinMax(out var _, out var maxValues, out var _, out var maxLocations);
						if (maxValues[0] > percent)
						{
							result = maxLocations[0];
						}
					}
					GC.Collect();
					GC.WaitForPendingFinalizers();
					return result;
				}
				result2 = null;
			}
			else
			{
				result2 = null;
			}
			return result2;
		}

		public static List<Point> FindOutPoints(string mainBitmap, string subBitmap, double percent = 0.9)
		{
			try
			{
				using Image<Bgr, byte> image = new Bitmap(mainBitmap).ToImage<Bgr, byte>();
				using Image<Bgr, byte> image2 = new Bitmap(subBitmap).ToImage<Bgr, byte>();
				List<Point> list = new List<Point>();
				while (true)
				{
					using Image<Gray, float> image3 = image.MatchTemplate(image2, TemplateMatchingType.CcoeffNormed);
					image3.MinMax(out var _, out var maxValues, out var _, out var maxLocations);
					if (!(maxValues[0] > percent))
					{
						break;
					}
					Rectangle rect = new Rectangle(maxLocations[0], image2.Size);
					image.Draw(rect, new Bgr(System.Drawing.Color.Blue), -1);
					list.Add(maxLocations[0]);
					if (list.Count > 3)
					{
						return list;
					}
					continue;
				}
				return list;
			}
			catch (Exception ex)
			{
				Utils.CCKLog("FindOutPoints", ex.Message);
			}
			return new List<Point>();
		}

		public static List<Point> FindColor(Bitmap mainBitmap, System.Drawing.Color color)
		{
			int num = color.ToArgb();
			List<Point> list = new List<Point>();
			try
			{
				for (int i = 0; i < mainBitmap.Width; i++)
				{
					for (int j = 0; j < mainBitmap.Height; j++)
					{
						if (num.Equals(mainBitmap.GetPixel(i, j).ToArgb()))
						{
							list.Add(new Point(i, j));
						}
					}
				}
			}
			finally
			{
				((IDisposable)mainBitmap)?.Dispose();
			}
			return list;
		}

		public static List<Point> FindColor(Bitmap mainBitmap, string color)
		{
			System.Drawing.Color color2 = (System.Drawing.Color)System.Windows.Media.ColorConverter.ConvertFromString(color);
			return FindColor(mainBitmap, color2);
		}

		public static Bitmap ThreshHoldBinary(Bitmap bmp, byte threshold = 190)
		{
			Image<Gray, byte> image = bmp.ToImage<Gray, byte>();
			Image<Gray, byte> image2 = image.ThresholdBinary(new Gray((int)threshold), new Gray(255.0));
			return image2.ToBitmap();
		}

		public static Bitmap NotWhiteToTransparentPixelReplacement(Bitmap bmp)
		{
			bmp = CreateNonIndexedImage(bmp);
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					System.Drawing.Color pixel = bmp.GetPixel(i, j);
					if (pixel.R > 200 && pixel.G > 200 && pixel.B > 200)
					{
						bmp.SetPixel(i, j, System.Drawing.Color.Transparent);
					}
				}
			}
			return bmp;
		}

		public static Bitmap WhiteToBlackPixelReplacement(Bitmap bmp)
		{
			bmp = CreateNonIndexedImage(bmp);
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					System.Drawing.Color pixel = bmp.GetPixel(i, j);
					if (pixel.R > 20 && pixel.G > 230 && pixel.B > 230)
					{
						bmp.SetPixel(i, j, System.Drawing.Color.Black);
					}
				}
			}
			return bmp;
		}

		public static Bitmap TransparentToWhitePixelReplacement(Bitmap bmp)
		{
			bmp = CreateNonIndexedImage(bmp);
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					if (bmp.GetPixel(i, j).A >= 1)
					{
						bmp.SetPixel(i, j, System.Drawing.Color.White);
					}
				}
			}
			return bmp;
		}

		public static Bitmap CreateNonIndexedImage(Image src)
		{
			Bitmap bitmap = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(src, 0, 0);
			}
			return bitmap;
		}
	}
}
