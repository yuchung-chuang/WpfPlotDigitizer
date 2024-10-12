using PlotDigitizer.Core;

using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.WPF
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

			} else if (e.Data.GetDataPresent(DataFormats.Text)
				&& e.Data.GetData(DataFormats.Text).ToString().ToUri() is Uri uri) {

				args.Type = DropEventArgs.DropType.Url;
				args.Url = uri;

			} else if (e.Data.GetDataPresent(DataFormats.MetafilePicture)) {

				args.Type = DropEventArgs.DropType.Image;
				// TODO: lose resolution, why?
				args.Image = (e.Data.GetData(DataFormats.Bitmap) as BitmapSource).ToBitmap();

			} else {
				return null;
			}

			return args;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}