using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.WPF
{
    /// <summary>
    /// Adjustable selection box. The selection box is typically placed with the target control at the same level with the same size (i.e. overlap with the control), so the location and size of the selection box represents the area selected in the control.
    /// </summary>
    public partial class SelectionBox : UserControl, INotifyPropertyChanged
    {
        #region Public Fields
        public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(nameof(IsMoveEnabled), typeof(bool), typeof(SelectionBox), new PropertyMetadata(true));

        public static readonly DependencyProperty BoxHeightProperty = DependencyProperty.Register(nameof(BoxHeight), typeof(double), typeof(SelectionBox), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty BoxLeftProperty = DependencyProperty.Register(nameof(BoxLeft), typeof(double), typeof(SelectionBox), new PropertyMetadata(default(double), OnBoxMarginUpdated));

        public static readonly DependencyProperty BoxTopProperty = DependencyProperty.Register(nameof(BoxTop), typeof(double), typeof(SelectionBox), new PropertyMetadata(default(double), OnBoxMarginUpdated));

        public static readonly DependencyProperty BoxWidthProperty = DependencyProperty.Register(nameof(BoxWidth), typeof(double), typeof(SelectionBox), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty BoxThicknessProperty = DependencyProperty.Register(nameof(BoxThickness), typeof(double), typeof(SelectionBox), new PropertyMetadata(2d));

        public static readonly DependencyProperty PanelProperty = DependencyProperty.Register(nameof(Panel), typeof(Panel), typeof(SelectionBox), new PropertyMetadata());

        public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(nameof(Gesture), typeof(MouseGesture), typeof(SelectionBox), new PropertyMetadata(new MouseGesture(MouseAction.LeftClick)));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(SelectionBox), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(SelectionBox), new PropertyMetadata(1d));

        #endregion Public Fields

        #region Public Constructors

        public SelectionBox()
        {
            InitializeComponent();
            Loaded += ImageSelectionBox_Loaded;
            Unloaded += ImageSelectionBox_Unloaded;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties
        public bool IsMoveEnabled
        {
            get { return (bool)GetValue(IsMoveEnabledProperty); }
            set { SetValue(IsMoveEnabledProperty, value); }
        }
        public double BoxLeft
        {
            get => (double)GetValue(BoxLeftProperty);
            set => SetValue(BoxLeftProperty, MathHelpers.Clamp(value, BoxRight - BoxThickness, 0));
        }
        public double BoxTop
        {
            get => (double)GetValue(BoxTopProperty);
            set => SetValue(BoxTopProperty, MathHelpers.Clamp(value, BoxBottom - BoxThickness, 0));
        }
        public double BoxWidth
        {
            get => (double)GetValue(BoxWidthProperty);
            set => SetValue(BoxWidthProperty, MathHelpers.Clamp(value, double.MaxValue, BoxThickness));
        }
        public double BoxHeight
        {
            get => (double)GetValue(BoxHeightProperty);
            set => SetValue(BoxHeightProperty, MathHelpers.Clamp(value, double.MaxValue, BoxThickness));
        }
        public double BoxThickness
        {
            get { return (double)GetValue(BoxThicknessProperty); }
            set { SetValue(BoxThicknessProperty, value); }
        }
        public MouseGesture Gesture
        {
            get => (MouseGesture)GetValue(GestureProperty);
            set => SetValue(GestureProperty, value);
        }
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// The panel that captures all the mouse events for adjusting the selection box. This is typically the parent control that contains both the selection box and the target control.
        /// </summary>
        public Panel Panel
        {
            get { return (Panel)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }
        public double BoxRight => BoxLeft + BoxWidth;
        public double BoxBottom => BoxTop + BoxHeight;
        public Point BoxLocation => new (BoxLeft, BoxTop);
        public Thickness BoxMargin => new(BoxLeft, BoxTop, 0, 0);
        public SelectionBoxState State { get; set; } = SelectionBoxStates.None;

        #endregion Public Properties

        #region Protected Methods

        protected void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion Protected Methods


        #region Public Methods

        public void UpdateCursor(SelectionBoxState state)
        {
            if (state is NullState) {
                Cursor = Cursors.Arrow;
                return;
            }
            if (state is MoveState) {
                Cursor = Cursors.SizeAll;
                return;
            }
            if (state is AdjustState adjustState) {
                if (adjustState.StateX is not NullState && adjustState.StateY is NullState) {
                    Cursor = Cursors.SizeWE;
                }
                else if (adjustState.StateX is NullState && adjustState.StateY is not NullState) {
                    Cursor = Cursors.SizeNS;
                }
                else if (adjustState.StateX is AdjustLeftState && adjustState.StateY is AdjustTopState
                    || adjustState.StateX is AdjustRightState && adjustState.StateY is AdjustBottomState) {
                    Cursor = Cursors.SizeNWSE;
                }
                else if (adjustState.StateX is AdjustRightState && adjustState.StateY is AdjustTopState
                    || adjustState.StateX is AdjustLeftState && adjustState.StateY is AdjustBottomState) {
                    Cursor = Cursors.SizeNESW;
                }
                return;
            }
        }

        public SelectionBoxState GetState(Point mousePos)
        {
            var adjustState = new AdjustState();
            if (MathHelpers.ApproxEqual(mousePos.X, BoxLeft, BoxThickness) &&
                MathHelpers.IsIn(mousePos.Y, BoxBottom + BoxThickness, BoxTop - BoxThickness)) {
                adjustState.StateX = SelectionBoxStates.AdjustLeft;
            }
            if (MathHelpers.ApproxEqual(mousePos.X, BoxRight, BoxThickness) &&
              MathHelpers.IsIn(mousePos.Y, BoxBottom + BoxThickness, BoxTop - BoxThickness)) {
                adjustState.StateX = SelectionBoxStates.AdjustRight;
            }
            if (MathHelpers.ApproxEqual(mousePos.Y, BoxTop, BoxThickness) &&
              MathHelpers.IsIn(mousePos.X, BoxRight + BoxThickness, BoxLeft - BoxThickness)) {
                adjustState.StateY = SelectionBoxStates.AdjustTop;
            }
            if (MathHelpers.ApproxEqual(mousePos.Y, BoxBottom, BoxThickness) &&
              MathHelpers.IsIn(mousePos.X, BoxRight + BoxThickness, BoxLeft - BoxThickness)) {
                adjustState.StateY = SelectionBoxStates.AdjustBottom;
            }
            if (adjustState.StateX is not NullState || adjustState.StateY is not NullState) {
                return adjustState;
            }

            if (IsMoveEnabled && 
                mousePos.X > BoxLeft + BoxThickness && mousePos.X < BoxRight - BoxThickness &&
                mousePos.Y > BoxTop + BoxThickness && mousePos.Y < BoxBottom - BoxThickness) {
                return new MoveState(BoxLocation, mousePos);
            }
            return SelectionBoxStates.None;
        }

        #endregion Public Methods


        #region Private Methods

        private static void OnBoxMarginUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as SelectionBox;
            box.RaisePropertyChanged(nameof(BoxMargin));
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            State.MouseDown(this, e);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            State.MouseMove(this, e);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            State.MouseUp(this, e);
        }

        private void ImageSelectionBox_Loaded(object sender, RoutedEventArgs e)
        {
            Panel.MouseDown += Grid_MouseDown;
            Panel.MouseMove += Grid_MouseMove;
            Panel.MouseUp += Grid_MouseUp;
        }
        private void ImageSelectionBox_Unloaded(object sender, RoutedEventArgs e)
        {
            Panel.MouseDown -= Grid_MouseDown;
            Panel.MouseMove -= Grid_MouseMove;
            Panel.MouseUp -= Grid_MouseUp;
        }
        #endregion Private Methods
    }
}