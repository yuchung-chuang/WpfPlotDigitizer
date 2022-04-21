using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace PlotDigitizer.App
{
	public class VisibilityConverter : ValueConverterBase<VisibilityConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool b)) {
				return null;
			}
			return b switch
			{
				true => Visibility.Visible,
				false => Visibility.Collapsed
			};
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
