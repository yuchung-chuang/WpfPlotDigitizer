using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CycWpfLibrary.Math;

namespace WpfPlotDigitizer
{
  public enum AdjustType
  {
    None = 0x0,
    Left = 0x1,
    Top = 0x2,
    Right = 0x4,
    Bottom = 0x8,
    LeftTop = Left | Top,
    RightTop = Right | Top,
    LeftBottom = Left | Bottom,
    RightBottom = Right | Bottom,
  }
  public static class AdjustTypeExtension
  {
    public static bool Contain(this AdjustType typeA, AdjustType typeB) => (typeA & typeB) == typeB;
  }
  /// <summary>
  /// AdjustableRectangle.xaml 的互動邏輯
  /// </summary>
  public partial class Axis : UserControl
  {
    public Axis()
    {
      InitializeComponent();

      gridMain.DataContext = this;
    }

    public double AxisLeft { get; set; }
    public double AxisTop { get; set; }
    public double AxisWidth { get; set; }
    public double AxisHeight { get; set; }
    public double AxisRight => AxisLeft + AxisWidth;
    public double AxisBottom => AxisTop + AxisHeight;

    private double tol = 10;


    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      var mouseState = new AdjustType();
      var mousePos = e.GetPosition(gridMain);
      // judge mouse position
      if (ApproxEqual(mousePos.X, AxisLeft, tol))
      {
        mouseState |= AdjustType.Left;
      }
      if (ApproxEqual(mousePos.Y, AxisTop, tol))
      {
        mouseState |= AdjustType.Top;
      }
      if (ApproxEqual(mousePos.Y, AxisRight, tol))
      {
        mouseState |= AdjustType.Right;
      }
      if (ApproxEqual(mousePos.Y, AxisBottom, tol))
      {
        mouseState |= AdjustType.Bottom;
      }
      // Change Cursor
      switch (mouseState)
      {
        default:
        case AdjustType.None:
          break;
        case AdjustType.Right:
        case AdjustType.Left:
          Cursor = Cursors.SizeWE;
          break;
        case AdjustType.Top:
        case AdjustType.Bottom:
          Cursor = Cursors.SizeNS;
          break;
        case AdjustType.RightBottom:
        case AdjustType.LeftTop:
          Cursor = Cursors.SizeNESW;
          break;
        case AdjustType.RightTop:
        case AdjustType.LeftBottom:
          Cursor = Cursors.SizeNWSE;
          break;
      }
      // adjust 
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (mouseState.Contain(AdjustType.Left))
        {
          AxisLeft = mousePos.X;
        }
        if (mouseState.Contain(AdjustType.Top))
        {
          AxisLeft = mousePos.Y;
        }
        if (mouseState.Contain(AdjustType.Right))
        {
          AxisWidth = mousePos.X - AxisLeft;
        }
        if (mouseState.Contain(AdjustType.Bottom))
        {
          AxisHeight = mousePos.Y - AxisBottom;
        }
      }

    }
  }
}
