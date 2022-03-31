using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlotDigitizer.Core
{
	public class VisibilityConverter : MarkupExtension, IValueConverter
	{
		private static readonly VisibilityConverter instance = new VisibilityConverter();

		/// <summary>
		/// Convert <see cref="bool"/> to <see cref="Visibility"/>
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible ? true : false;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return instance;
		}
	}
}