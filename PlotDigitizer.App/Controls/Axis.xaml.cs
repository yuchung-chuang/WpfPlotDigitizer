using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Axis.xaml 的互動邏輯
	/// </summary>
	public partial class Axis : UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
			nameof(Stroke),
			typeof(Brush),
			typeof(Axis),
			new PropertyMetadata(new SolidColorBrush(Colors.Red)));

		public static readonly DependencyProperty AxisLeftProperty = DependencyProperty.Register(
			nameof(AxisLeft),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisLeftChanged));

		public static readonly DependencyProperty AxisTopProperty = DependencyProperty.Register(
			nameof(AxisTop),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisTopChanged));

		public static readonly DependencyProperty AxisWidthProperty = DependencyProperty.Register(
			nameof(AxisWidth),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisWidthChanged));

		public static readonly DependencyProperty AxisHeightProperty = DependencyProperty.Register(
			nameof(AxisHeight),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisHeightChanged));

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
			nameof(ImageSource),
			typeof(ImageSource),
			typeof(Axis),
			new PropertyMetadata(default(ImageSource), OnImageSourceChanged));

		public static readonly DependencyProperty MouseButtonProperty =
			DependencyProperty.Register(nameof(MouseButton), typeof(MouseButton), typeof(Axis), new PropertyMetadata(MouseButton.Left));

		private readonly double tol = 10;

		private bool IsAdjust = false;

		private AdjustType State = AdjustType.None;

		public event PropertyChangedEventHandler PropertyChanged;

		public Brush Stroke
		{
			get => (Brush)GetValue(StrokeProperty);
			set => SetValue(StrokeProperty, value);
		}

		public double AxisLeft
		{
			get => (double)GetValue(AxisLeftProperty);
			set => SetValue(AxisLeftProperty, AxisHelpers.Clamp(value, AxisRight - tol, 0));
		}

		public double AxisTop
		{
			get => (double)GetValue(AxisTopProperty);
			set => SetValue(AxisTopProperty, AxisHelpers.Clamp(value, AxisBottom - tol, 0));
		}

		public double AxisWidth
		{
			get => (double)GetValue(AxisWidthProperty);
			set => SetValue(AxisWidthProperty, AxisHelpers.Clamp(value, double.MaxValue, tol));
		}

		public double AxisHeight
		{
			get => (double)GetValue(AxisHeightProperty);
			set => SetValue(AxisHeightProperty, AxisHelpers.Clamp(value, double.MaxValue, tol));
		}

		public ImageSource ImageSource
		{
			get => (ImageSource)GetValue(ImageSourceProperty);
			set => SetValue(ImageSourceProperty, value);
		}

		public MouseButton MouseButton
		{
			get { return (MouseButton)GetValue(MouseButtonProperty); }
			set { SetValue(MouseButtonProperty, value); }
		}

		public Thickness AxisMargin => new Thickness(AxisLeft, AxisTop, 0, 0);

		public Rect AxisRelative =>
			Image == null ?
			new Rect() :
			new Rect(AxisLeft / Image.PixelWidth,
				AxisTop / Image.PixelHeight,
				AxisWidth / Image.PixelWidth,
				AxisHeight / Image.PixelHeight);

		public double AxisRight => AxisLeft + AxisWidth;
		public double AxisBottom => AxisTop + AxisHeight;

		public BitmapSource Image => ImageSource as BitmapSource;

		public Axis()
		{
			InitializeComponent();
			gridMain.DataContext = this;
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (!InputCheck(e))
				return;
			var mousePos = e.GetPosition(gridMain);
			State = GetState(mousePos);
			UpdateCursor(State);

			// Initialize Adjust
			if (State != (AdjustType.None)) {
				IsAdjust = true;
				CaptureMouse();

				// block other events
				e.Handled = true;
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			IsAdjust = false;
			ReleaseMouseCapture();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			var mousePos = e.GetPosition(gridMain);
			// is not adjusting, just update the cursor
			if (!IsAdjust) {
				UpdateCursor(GetState(mousePos));
				return;
			}
			// avoid mouse go outside the grid
			if (!(AxisHelpers.IsIn(mousePos.X, gridMain.ActualWidth, 0) &&
				  AxisHelpers.IsIn(mousePos.Y, gridMain.ActualHeight, 0)))
				return;
			// adjust
			if (State.Contain(AdjustType.Left)) {
				var delta = mousePos.X - AxisLeft;
				AxisLeft = mousePos.X; // must be checked earlier than width
				AxisWidth -= delta;
			}
			if (State.Contain(AdjustType.Top)) {
				var delta = mousePos.Y - AxisTop;
				AxisTop = mousePos.Y;
				AxisHeight -= delta;
			}
			if (State.Contain(AdjustType.Right))
				AxisWidth = mousePos.X - AxisLeft;
			if (State.Contain(AdjustType.Bottom))
				AxisHeight = mousePos.Y - AxisTop;
		}

		private static void OnAxisLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisMargin));
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		private static void OnAxisTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisMargin));
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		private static void OnAxisWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		private static void OnAxisHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisRelative));
			axis.OnPropertyChanged(nameof(Image));
		}

		private AdjustType GetState(Point mousePos)
		{
			var state = new AdjustType();
			if (AxisHelpers.ApproxEqual(mousePos.X, AxisLeft, tol) &&
			  AxisHelpers.IsIn(mousePos.Y, AxisBottom + tol, AxisTop - tol))
				state = (AdjustType)state.Add(AdjustType.Left);
			if (AxisHelpers.ApproxEqual(mousePos.Y, AxisTop, tol) &&
			  AxisHelpers.IsIn(mousePos.X, AxisRight + tol, AxisLeft - tol))
				state = (AdjustType)state.Add(AdjustType.Top);
			if (AxisHelpers.ApproxEqual(mousePos.X, AxisRight, tol) &&
			  AxisHelpers.IsIn(mousePos.Y, AxisBottom + tol, AxisTop - tol))
				state = (AdjustType)state.Add(AdjustType.Right);
			if (AxisHelpers.ApproxEqual(mousePos.Y, AxisBottom, tol) &&
			  AxisHelpers.IsIn(mousePos.X, AxisRight + tol, AxisLeft - tol))
				state = (AdjustType)state.Add(AdjustType.Bottom);
			return state;
		}

		private void UpdateCursor(AdjustType state)
		{
			switch (state) {
				default:
				case AdjustType.None:
					Cursor = Cursors.Arrow;
					break;
				case AdjustType.Left:
				case AdjustType.Right:
					Cursor = Cursors.SizeWE;
					break;
				case AdjustType.Top:
				case AdjustType.Bottom:
					Cursor = Cursors.SizeNS;
					break;
				case AdjustType.LeftTop:
				case AdjustType.RightBottom:
					Cursor = Cursors.SizeNWSE;
					break;
				case AdjustType.RightTop:
				case AdjustType.LeftBottom:
					Cursor = Cursors.SizeNESW;
					break;
			}
		}

		private bool InputCheck(MouseButtonEventArgs e)
		{
			return IsPressed(MouseButton);

			bool IsPressed(MouseButton mouseButton)
			{
				return mouseButton switch
				{
					MouseButton.Left => Mouse.LeftButton == MouseButtonState.Pressed,
					MouseButton.Right => Mouse.RightButton == MouseButtonState.Pressed,
					MouseButton.Middle => Mouse.MiddleButton == MouseButtonState.Pressed,
					MouseButton.XButton1 => Mouse.XButton1 == MouseButtonState.Pressed,
					MouseButton.XButton2 => Mouse.XButton2 == MouseButtonState.Pressed,
					_ => true,
				};
			}
		}
	}

	public enum AdjustType
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		LeftTop = Left | Top,
		RightTop = Right | Top,
		LeftBottom = Left | Bottom,
		RightBottom = Right | Bottom,
	}

	public static class AxisHelpers
	{
		public static dynamic Add(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return a | b;
		}

		public static bool Contain(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return (a & b) == b;
		}

		public static double Clamp(double value, double Max, double Min)
		{
			if (Min > Max)
				Swap(ref Max, ref Min);

			if (value > Max)
				return Max;
			else if (value < Min)
				return Min;
			else
				return value;
		}

		public static void Swap<T>(ref T x, ref T y) => (x, y) = (y, x);

		/// <summary>
		/// 判斷<paramref name="value"/>是否位於閉區間[<paramref name="Max"/>,<paramref name="Min"/>]中。<paramref name="excludeBoundary"/>為真時，改為判斷開區間(<paramref name="Max"/>,<paramref name="Min"/>)。
		/// </summary>
		public static bool IsIn(double value, double Max, double Min, bool excludeBoundary = false)
		{
			if (Min > Max)
				Swap(ref Max, ref Min);
			if (!excludeBoundary)
				return (value <= Max && value >= Min) ? true : false;
			else
				return (value < Max && value > Min) ? true : false;
		}

		/// <summary>
		/// 判斷<paramref name="A"/>是否約等於<paramref name="B"/>。
		/// </summary>
		/// <param name="tol">容許誤差。</param>
		public static bool ApproxEqual(double A, double B, double tol)
		{
			return IsIn(A, B + tol, B - tol);
		}

		private static (ulong a, ulong b) ConvertEnums(Enum enumA, Enum enumB) => (Convert.ToUInt64(enumA), Convert.ToUInt64(enumB));
	}
}