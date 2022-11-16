using PropertyChanged;

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
		#region Fields

		public static readonly DependencyProperty AxisHeightProperty = DependencyProperty.Register(
			nameof(AxisHeight),
			typeof(double),
			typeof(Axis),
			new PropertyMetadata(default(double), OnAxisHeightChanged));

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

		public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(nameof(Gesture), typeof(MouseGesture), typeof(Axis), new PropertyMetadata(new MouseGesture(MouseAction.LeftClick)));

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
			nameof(ImageSource),
			typeof(ImageSource),
			typeof(Axis),
			new PropertyMetadata(default(ImageSource), OnImageSourceChanged));

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
			nameof(Stroke),
			typeof(Brush),
			typeof(Axis),
			new PropertyMetadata(new SolidColorBrush(Colors.Red)));

		private readonly double tol = 10;

		private bool IsAdjust = false;

		private AdjustType State = AdjustType.None;

		#endregion Fields

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Events

		#region Properties

		public double AxisBottom => AxisTop + AxisHeight;

		public double AxisHeight
		{
			get => (double)GetValue(AxisHeightProperty);
			set => SetValue(AxisHeightProperty, AxisHelpers.Clamp(value, double.MaxValue, tol));
		}

		public double AxisLeft
		{
			get => (double)GetValue(AxisLeftProperty);
			set => SetValue(AxisLeftProperty, AxisHelpers.Clamp(value, AxisRight - tol, 0));
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

		public MouseGesture Gesture
		{
			get => (MouseGesture)GetValue(GestureProperty);
			set => SetValue(GestureProperty, value);
		}

		public BitmapSource Image => ImageSource as BitmapSource;

		public ImageSource ImageSource
		{
			get => (ImageSource)GetValue(ImageSourceProperty);
			set => SetValue(ImageSourceProperty, value);
		}

		public Brush Stroke
		{
			get => (Brush)GetValue(StrokeProperty);
			set => SetValue(StrokeProperty, value);
		}

		#endregion Properties

		#region Constructors

		public Axis()
		{
			InitializeComponent();
			gridMain.DataContext = this;
		}

		#endregion Constructors

		#region Methods

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (!Gesture.Matches(this, e))
				return;
			var mousePos = e.GetPosition(gridMain);
			State = GetState(mousePos);
			UpdateCursor(State);

			// Initialize Adjust
			if (State != AdjustType.None) {
				IsAdjust = true;
				CaptureMouse();

				// block other events
				e.Handled = true;
			}
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

			// adjust
			if (State.Contain(AdjustType.Left)) {
				var right = AxisRight;
				AxisLeft = Math.Min(Math.Max(mousePos.X, 0), right - 1);
				AxisWidth = right - AxisLeft;
			} else if (State.Contain(AdjustType.Right)) {
				AxisWidth = Math.Min(Math.Max(mousePos.X - AxisLeft, 1), Image.Width - AxisLeft);
			}

			if (State.Contain(AdjustType.Top)) {
				var bottom = AxisBottom;
				AxisTop = Math.Min(Math.Max(mousePos.Y, 0), bottom - 1);
				AxisHeight = bottom - AxisTop;
			} else if (State.Contain(AdjustType.Bottom)) {
				AxisHeight = Math.Min(Math.Max(mousePos.Y - AxisTop, 1), Image.Height - AxisTop);
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			IsAdjust = false;
			ReleaseMouseCapture();
		}

		protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		[SuppressPropertyChangedWarnings]
		private static void OnAxisHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		[SuppressPropertyChangedWarnings]
		private static void OnAxisLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisMargin));
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		[SuppressPropertyChangedWarnings]
		private static void OnAxisTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisMargin));
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		[SuppressPropertyChangedWarnings]
		private static void OnAxisWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var axis = d as Axis;
			axis.OnPropertyChanged(nameof(AxisRelative));
		}

		[SuppressPropertyChangedWarnings]
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

		#endregion Methods
	}
}