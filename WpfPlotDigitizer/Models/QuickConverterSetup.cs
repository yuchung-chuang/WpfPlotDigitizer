using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  public static class QuickConverterHelpers
  {
    public static void Setup()
    {
      QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
      QuickConverter.EquationTokenizer.AddNamespace(typeof(double));
    }
  }
}
