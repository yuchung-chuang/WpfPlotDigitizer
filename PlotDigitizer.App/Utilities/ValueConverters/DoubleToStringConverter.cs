using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace PlotDigitizer.App
{
	public class DoubleToStringConverter : ValueConverterBase<DoubleToStringConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is double d)) {
				throw new NotSupportedException();
			}
			return !double.IsNaN(d) ? d.ToString() : string.Empty;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.IsNullOrWhiteSpace(value.ToString())) {
				return double.NaN;
			}
			if (double.TryParse(value.ToString(), out var result)) {
				return result;
			}
			return null;
		}

	}
}
