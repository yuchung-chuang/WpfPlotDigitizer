using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	public class SelectionChangedEventArgsConverter : ValueConverterBase<SelectionChangedEventArgsConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is SelectionChangedEventArgs e)) {
				return value;
			}
			if (!(e.Source is ComboBox comboBox)) {
				return value;
			}
			return comboBox.SelectedIndex;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
