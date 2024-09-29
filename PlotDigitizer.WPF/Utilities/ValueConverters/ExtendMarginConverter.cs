using System;
using System.Globalization;
using System.Windows;

namespace PlotDigitizer.WPF
{
    public class ExtendMarginConverter : MultiValueConverterBase<ExtendMarginConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) {
                return null;
            }
            if (values[0] is not Thickness boxMargin
                || values[1] is not double boxThickness) {
                return null;
            }
            return new Thickness(boxMargin.Left - boxThickness, boxMargin.Top - boxThickness, 0, 0);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
