using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppTemplate1
{
    public class RoundingConverter : ValueConverterBase<RoundingConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double @double) {
                return value;
            }
            if (parameter is not int digits) {
                digits = 2;
            }
            return Math.Round(@double, digits);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
