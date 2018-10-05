using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class ImageProcessing
  {
    public static BitmapImage GetImage(Uri uri)
    {
      return new BitmapImage(uri);
    }
  }
}
