using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  public class PageConverter : ValueConverterBase<PageConverter>
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch ((Pages)value)
      {
        default:
        case Pages.Browse:
          return new BrowsePage();
        case Pages.Axis:
          return new AxisPage();
        case Pages.Filter:
          return new FilterPage();
        case Pages.Erase:
          return new ErasePage();
      }
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
