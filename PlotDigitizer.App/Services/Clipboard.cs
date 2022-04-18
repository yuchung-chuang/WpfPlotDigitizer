using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	public class Clipboard : IClipboard
	{
		public bool ContainsImage() => System.Windows.Clipboard.ContainsImage();

		public bool ContainsFileDropList() => System.Windows.Clipboard.ContainsFileDropList();

		public Image<Rgba,byte> GetImage() => System.Windows.Clipboard.GetImage().ToBitmap().ToImage<Rgba, byte>();

		public StringCollection GetFileDropList() => System.Windows.Clipboard.GetFileDropList();

	}
}
