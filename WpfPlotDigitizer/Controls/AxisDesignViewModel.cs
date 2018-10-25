using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  public class AxisDesignViewModel : Axis
  {
    public static AxisDesignViewModel instance { get; private set; } = new AxisDesignViewModel();

    public AxisDesignViewModel()
    {
      AxisLeft = 100;
      AxisTop = 100;
      AxisWidth = 150;
      AxisHeight = 100;
      AxisBrush = "Red";
      ShadowBrush = "#99999999";
    }
  }
}
