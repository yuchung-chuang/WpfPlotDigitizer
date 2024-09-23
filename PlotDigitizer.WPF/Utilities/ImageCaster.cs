using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Bitmap = System.Drawing.Bitmap;

namespace PlotDigitizer.WPF
{
	public static class ImageCaster
	{
		/// <summary>
		/// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
		/// </summary>
		/// <param name="image">The Emgu CV Image</param>
		/// <returns>The equivalent BitmapSource</returns>
		public static BitmapSource ToBitmapSource<TColor, TDepth>(this Image<TColor, TDepth> image)
			where TColor : struct, IColor
			where TDepth : new() =>
			// must convert to bgra to retain the transparency
			image.Convert<Bgra, byte>().ToBitmap().ToBitmapSource();

		public static BitmapSource ToBitmapSource(this Bitmap bitmap)
		{
			var hBitmap = bitmap.GetHbitmap();
			try {
				return Imaging.CreateBitmapSourceFromHBitmap(
							 hBitmap,
							 IntPtr.Zero,
							 Int32Rect.Empty,
							 BitmapSizeOptions.FromEmptyOptions());
			}
			finally {
				DeleteObject(hBitmap);
			}
		}

		/// <summary>
		/// Delete a GDI object
		/// </summary>
		/// <param name="o">The poniter to the GDI object to be deleted</param>
		/// <returns></returns>
		[DllImport("gdi32")]
		private static extern int DeleteObject(IntPtr o);

		public static Bitmap ToBitmap(this BitmapSource source)
		{
			var stream = new MemoryStream(); // Do NOT dispose the memory stream for an bitmap, it should stay open during the lifetime of the bitmap
			var enc = new PngBitmapEncoder(); // 使用PngEncoder才不會流失透明度
			enc.Frames.Add(BitmapFrame.Create(source));
			enc.Save(stream);
			return new Bitmap(stream);
		}
	}
}