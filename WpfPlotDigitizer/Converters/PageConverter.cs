using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfPlotDigitizer
{
  public class PageConverter : ValueConverterBase<PageConverter>
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch ((ApplicationPages)value)
      {
        case ApplicationPages.Browse:
          return new BrowsePage();
        case ApplicationPages.Axis:
          return new AxisPage();
        case ApplicationPages.Filter:
          return new FilterPage();
        case ApplicationPages.Erase:
          return new ErasePage();
        default:
          return new UserControl();
      }
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
