using System;
using System.Diagnostics;
using System.Globalization;

namespace PlotDigitizer.App
{
	public class DebugConverter : ValueConverterBase<DebugConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debugger.Break();
			return value;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debugger.Break();
			return value;
		}
	}
}