using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public partial class ImageSelectionBox : UserControl, INotifyPropertyChanged
    {
        private readonly double tol = 10;

        private bool IsAdjust = false;

        private AdjustType State = AdjustType.None;

        public static readonly DependencyProperty ImageControlProperty = DependencyProperty.Register(nameof(ImageControl), typeof(Image), typeof(ImageSelectionBox), new PropertyMetadata());

        public static readonly DependencyProperty BoxHeightProperty = DependencyProperty.Register(nameof(BoxHeight), typeof(double), typeof(ImageSelectionBox), new PropertyMetadata(default(double), OnBoxHeightChanged));

        public static readonly DependencyProperty BoxLeftProperty = DependencyProperty.Register(
            nameof(BoxLeft), typeof(double), typeof(ImageSelectionBox), new PropertyMetadata(default(double), OnBoxLeftChanged));

        public static readonly DependencyProperty BoxTopProperty = DependencyProperty.Register(
            nameof(BoxTop), typeof(double), typeof(ImageSelectionBox), new PropertyMetadata(default(double), OnBoxTopChanged));

        public static readonly DependencyProperty BoxWidthProperty = DependencyProperty.Register(
            nameof(BoxWidth), typeof(double), typeof(ImageSelectionBox), new PropertyMetadata(default(double), OnBoxWidthChanged));

        public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(nameof(Gesture), typeof(MouseGesture), typeof(ImageSelectionBox), new PropertyMetadata(new MouseGesture(MouseAction.LeftClick)));

        public event PropertyChangedEventHandler PropertyChanged;

        public Image ImageControl
        {
            get { return (Image)GetValue(ImageControlProperty); }
            set { SetValue(ImageControlProperty, value); }
        }
        public double BoxLeft
        {
            get => (double)GetValue(BoxLeftProperty);
            set => SetValue(BoxLeftProperty, AxisHelpers.Clamp(value, BoxRight - tol, 0));
        }
        public double BoxTop
        {
            get => (double)GetValue(BoxTopProperty);
            set => SetValue(BoxTopProperty, AxisHelpers.Clamp(value, BoxBottom - tol, 0));
        }
        public double BoxWidth
        {
            get => (double)GetValue(BoxWidthProperty);
            set => SetValue(BoxWidthProperty, AxisHelpers.Clamp(value, double.MaxValue, tol));
        }
        public double BoxHeight
        {
            get => (double)GetValue(BoxHeightProperty);
            set => SetValue(BoxHeightProperty, AxisHelpers.Clamp(value, double.MaxValue, tol));
        }
        public Thickness BoxMargin => new(BoxLeft, BoxTop, 0, 0);

        public double BoxRight => BoxLeft + BoxWidth;
        public double BoxBottom => BoxTop + BoxHeight;

        public MouseGesture Gesture
        {
            get => (MouseGesture)GetValue(GestureProperty);
            set => SetValue(GestureProperty, value);
        }

        public ImageSelectionBox()
        {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!Gesture.Matches(this, e))
                return;
            var mousePos = e.GetPosition(this);
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
            var mousePos = e.GetPosition(this);
            // is not adjusting, just update the cursor
            if (!IsAdjust) {
                UpdateCursor(GetState(mousePos));
                return;
            }

            // adjust
            if (State.Contain(AdjustType.Left)) {
                var right = BoxRight;
                BoxLeft = AxisHelpers.Clamp(mousePos.X, right - 1, 0); 
                BoxWidth = right - BoxLeft;
            }
            else if (State.Contain(AdjustType.Right)) {
                BoxWidth = AxisHelpers.Clamp(mousePos.X - BoxLeft, ImageControl.ActualWidth - BoxLeft, 1);
            }

            if (State.Contain(AdjustType.Top)) {
                var bottom = BoxBottom;
                BoxTop = AxisHelpers.Clamp(mousePos.Y, bottom - 1, 0);
                BoxHeight = bottom - BoxTop;
            }
            else if (State.Contain(AdjustType.Bottom)) {
                BoxHeight = AxisHelpers.Clamp(mousePos.Y - BoxTop, ImageControl.ActualHeight - BoxTop, 1);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            IsAdjust = false;
            ReleaseMouseCapture();
        }

        private AdjustType GetState(Point mousePos)
        {
            var state = new AdjustType();
            if (AxisHelpers.ApproxEqual(mousePos.X, BoxLeft, tol) &&
              AxisHelpers.IsIn(mousePos.Y, BoxBottom + tol, BoxTop - tol))
                state = (AdjustType)state.Add(AdjustType.Left);
            if (AxisHelpers.ApproxEqual(mousePos.Y, BoxTop, tol) &&
              AxisHelpers.IsIn(mousePos.X, BoxRight + tol, BoxLeft - tol))
                state = (AdjustType)state.Add(AdjustType.Top);
            if (AxisHelpers.ApproxEqual(mousePos.X, BoxRight, tol) &&
              AxisHelpers.IsIn(mousePos.Y, BoxBottom + tol, BoxTop - tol))
                state = (AdjustType)state.Add(AdjustType.Right);
            if (AxisHelpers.ApproxEqual(mousePos.Y, BoxBottom, tol) &&
              AxisHelpers.IsIn(mousePos.X, BoxRight + tol, BoxLeft - tol))
                state = (AdjustType)state.Add(AdjustType.Bottom);
            return state;
        }

        private void UpdateCursor(AdjustType state)
        {
            Cursor = state switch
            {
                AdjustType.Left or AdjustType.Right => Cursors.SizeWE,
                AdjustType.Top or AdjustType.Bottom => Cursors.SizeNS,
                AdjustType.LeftTop or AdjustType.RightBottom => Cursors.SizeNWSE,
                AdjustType.RightTop or AdjustType.LeftBottom => Cursors.SizeNESW,
                _ => Cursors.Arrow,
            };
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [SuppressPropertyChangedWarnings]
        private static void OnBoxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as ImageSelectionBox;
            //box.OnPropertyChanged(nameof(BoxRelative));
        }

        [SuppressPropertyChangedWarnings]
        private static void OnBoxLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as ImageSelectionBox;
            box.OnPropertyChanged(nameof(BoxMargin));
            //Box.OnPropertyChanged(nameof(BoxRelative));
        }

        [SuppressPropertyChangedWarnings]
        private static void OnBoxTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as ImageSelectionBox;
            box.OnPropertyChanged(nameof(BoxMargin));
            //Box.OnPropertyChanged(nameof(BoxRelative));
        }

        [SuppressPropertyChangedWarnings]
        private static void OnBoxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as ImageSelectionBox;
            //box.OnPropertyChanged(nameof(BoxRelative));
        }
    }
}
