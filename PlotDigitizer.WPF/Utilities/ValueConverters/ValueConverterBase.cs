using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlotDigitizer.WPF
{
	public abstract class ValueConverterBase<T> : MarkupExtension, IValueConverter
		where T : new()
	{
		public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

		public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

		private static readonly T instance = new T();

		public override object ProvideValue(IServiceProvider serviceProvider) => instance;
	}
}