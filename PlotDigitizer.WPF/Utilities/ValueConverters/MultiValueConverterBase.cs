using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlotDigitizer.WPF
{
	public abstract class MultiValueConverterBase<T> : MarkupExtension, IMultiValueConverter
		where T : new()
	{
		private static readonly object instance = new T();

		public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

		public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);

		public override object ProvideValue(IServiceProvider serviceProvider) => instance;
	}
}