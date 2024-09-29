using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlotDigitizer.WPF
{
    public class RectConverter : ValueConverterBase<RectConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not RectangleD rectangleD) {
                return null;
            }
            return new Rect(rectangleD.X, rectangleD.Y, rectangleD.Width, rectangleD.Height);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Rect rect) {
                return null;
            }
            return new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height);
        }
    }
}
