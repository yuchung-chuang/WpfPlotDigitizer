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

  public partial class Axis : UserControl
  {
    public Axis()
    {
      InitializeComponent();

      gridMain.DataContext = this;
    }

    public static readonly DependencyProperty AxisLeftProperty = DependencyProperty.Register(nameof(AxisLeft), typeof(double), typeof(Axis));
    public double AxisLeft
    {
      get => (double)GetValue(AxisLeftProperty);
      set => SetValue(AxisLeftProperty, value);
    }
    public static readonly DependencyProperty AxisTopProperty = DependencyProperty.Register(nameof(AxisTop), typeof(double), typeof(Axis));
    public double AxisTop
    {
      get => (double)GetValue(AxisTopProperty);
      set => SetValue(AxisTopProperty, value);
    }
    public static readonly DependencyProperty AxisWidthProperty = DependencyProperty.Register(nameof(AxisWidth), typeof(double), typeof(Axis));
    public double AxisWidth
    {
      get => (double)GetValue(AxisWidthProperty);
      set => SetValue(AxisWidthProperty, value);
    }
    public static readonly DependencyProperty AxisHeightProperty = DependencyProperty.Register(nameof(AxisHeight), typeof(double), typeof(Axis));
    public double AxisHeight
    {
      get => (double)GetValue(AxisHeightProperty);
      set => SetValue(AxisHeightProperty, value);
    }
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
      if (ApproxEqual(mousePos.X, AxisRight, tol))
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
          Cursor = Cursors.Arrow;
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
          Cursor = Cursors.SizeNWSE;
          break;
        case AdjustType.RightTop:
        case AdjustType.LeftBottom:
          Cursor = Cursors.SizeNESW;
          break;
      }
      // adjust 
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (mouseState.Contain(AdjustType.Left))
        {
          var delta = mousePos.X - AxisLeft;
          AxisLeft = mousePos.X;
          AxisWidth -= delta;
        }
        if (mouseState.Contain(AdjustType.Top))
        {
          var delta = mousePos.Y - AxisTop;
          AxisTop = mousePos.Y;
          AxisHeight -= delta;
        }
        if (mouseState.Contain(AdjustType.Right))
        {
          AxisWidth = mousePos.X - AxisLeft;
        }
        if (mouseState.Contain(AdjustType.Bottom))
        {
          AxisHeight = mousePos.Y - AxisTop;
        }
      }

    }
  }
}
