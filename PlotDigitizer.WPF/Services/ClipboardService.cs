﻿using Emgu.CV;
using Emgu.CV.Structure;

using PlotDigitizer.Core;

using System.Collections.Specialized;
using System.Windows;

namespace PlotDigitizer.WPF
{
	public class ClipboardService : IClipboardService
	{
		public bool ContainsText() => Clipboard.ContainsText();
		public bool ContainsImage() => Clipboard.ContainsImage();
		public bool ContainsFileDropList() => Clipboard.ContainsFileDropList();

		public string GetText() => Clipboard.GetText();
		public Image<Rgba, byte> GetImage() => Clipboard.GetImage().ToBitmap().ToImage<Rgba, byte>();
		public StringCollection GetFileDropList() => Clipboard.GetFileDropList();
	}
}