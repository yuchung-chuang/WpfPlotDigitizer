using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Globalization;

namespace PlotDigitizer.App
{
	public class ImageSourceConverter : ValueConverterBase<ImageSourceConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Image<Rgba, byte> image)) {
				return null;
			}
			return image?.ToBitmapSource();
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
