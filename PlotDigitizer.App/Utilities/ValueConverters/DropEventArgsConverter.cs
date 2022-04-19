using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace PlotDigitizer.App
{
	public class DropEventArgsConverter : ValueConverterBase<DropEventArgsConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var e = value as DragEventArgs;
			var args = new DropEventArgs();

			if (e.Data.GetDataPresent(DataFormats.FileDrop)
				&& File.Exists((e.Data.GetData(DataFormats.FileDrop) as string[])[0])) {
				args.Type = DropEventArgs.DropType.File;
				args.FileName = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
			} 
			else if (e.Data.GetDataPresent(DataFormats.Text)
				// check if it's valid Uri
				&& Uri.TryCreate(e.Data.GetData(DataFormats.Text).ToString(), UriKind.Absolute, out var uri)
				// check if it's a web uri
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) {
				args.Type = DropEventArgs.DropType.Url;
				args.Url = uri;
			} else {
				throw new Exception("Unknown drop type.");
			}

			return args;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
