using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfAppTemplate1
{
    public abstract class ValueConverterBase<T> : MarkupExtension, IValueConverter where T : class, new()
    {
        private static T instance;
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance ??= new T();
        }
    }
}
