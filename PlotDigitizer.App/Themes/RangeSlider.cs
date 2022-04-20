using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Delegate for the RangeSelectionChanged event
	/// </summary>
	/// <param name="sender">The object raising the event</param>
	/// <param name="e">The event arguments</param>
	public delegate void RangeSelectionChangedEventHandler(object sender, RangeSelectionChangedEventArgs e);

	/// <summary>
	/// A slider that provides the a range
	/// </summary>
	[DefaultEvent("RangeSelectionChanged"),
	TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel)),
	TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb)),
	TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb)),
	TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
	public sealed class RangeSlider : Control
	{
		#region Data members

		/// <summary>
		/// The min value for the range of the range slider
		/// </summary>
		public static readonly DependencyProperty RangeStartProperty =
			DependencyProperty.Register("RangeStart", typeof(long), typeof(RangeSlider),
			new UIPropertyMetadata((long)0,
				delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e)
				{
					var slider = (RangeSlider)sender;
					if (!slider.internalUpdate)//check if the property is set internally
					{
						slider.ReCalculateRanges();
						slider.ReCalculateWidths();
					}
				}));

		/// <summary>
		/// The max value for the range of the range slider
		/// </summary>
		public static readonly DependencyProperty RangeStopProperty =
			DependencyProperty.Register("RangeStop", typeof(long), typeof(RangeSlider),
			new UIPropertyMetadata((long)1,
				delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e)
				{
					var slider = (RangeSlider)sender;
					if (!slider.internalUpdate)//check if the property is set internally
					{
						slider.ReCalculateRanges();
						slider.ReCalculateWidths();
					}
				}));

		/// <summary>
		/// The min value of the selected range of the range slider
		/// </summary>
		public static readonly DependencyProperty RangeStartSelectedProperty =
			DependencyProperty.Register("RangeStartSelected", typeof(long), typeof(RangeSlider),
			new UIPropertyMetadata((long)0,
				delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e)
				{
					var slider = (RangeSlider)sender;
					if (!slider.internalUpdate)//check if the property is set internally
					{
						slider.ReCalculateWidths();
						slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider));
					}
				}));

		/// <summary>
		/// The max value of the selected range of the range slider
		/// </summary>
		public static readonly DependencyProperty RangeStopSelectedProperty =
			DependencyProperty.Register("RangeStopSelected", typeof(long), typeof(RangeSlider),
			new UIPropertyMetadata((long)1,
				delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e)
				{
					var slider = (RangeSlider)sender;
					if (!slider.internalUpdate)//check if the property is set internally
					{
						slider.ReCalculateWidths();
						slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider));
					}
				}));

		/// <summary>
		/// The min range value that you can have for the range slider
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when MinRange is set less than 0</exception>
		public static readonly DependencyProperty MinRangeProperty =
			DependencyProperty.Register("MinRange", typeof(long), typeof(RangeSlider),
			new UIPropertyMetadata((long)0,
				delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e)
				{
					if ((long)e.NewValue < 0)
						throw new ArgumentOutOfRangeException("value", "value for MinRange cannot be less than 0");

					var slider = (RangeSlider)sender;
					if (!slider.internalUpdate)//check if the property is set internally
					{
						slider.internalUpdate = true;//set flag to signal that the properties are being set by the object itself
						slider.RangeStopSelected = Math.Max(slider.RangeStopSelected, slider.RangeStartSelected + (long)e.NewValue);
						slider.RangeStop = Math.Max(slider.RangeStop, slider.RangeStopSelected);
						slider.internalUpdate = false;//set flag to signal that the properties are being set by the object itself

						slider.ReCalculateRanges();
						slider.ReCalculateWidths();
					}
				}));

		/// <summary>
		/// Event raised whenever the selected range is changed
		/// </summary>
		public static readonly RoutedEvent RangeSelectionChangedEvent =
			EventManager.RegisterRoutedEvent("RangeSelectionChanged",
			RoutingStrategy.Bubble, typeof(RangeSelectionChangedEventHandler), typeof(RangeSlider));

		/// <summary>
		/// Command to move back the selection
		/// </summary>
		public static RoutedUICommand MoveBack =
			new RoutedUICommand("MoveBack", "MoveBack", typeof(RangeSlider),
				new InputGestureCollection(new InputGesture[] {
					new KeyGesture(Key.B, ModifierKeys.Control)
				}));

		/// <summary>
		/// Command to move forward the selection
		/// </summary>
		public static RoutedUICommand MoveForward =
			new RoutedUICommand("MoveForward", "MoveForward", typeof(RangeSlider),
				new InputGestureCollection(new InputGesture[] {
					new KeyGesture(Key.F, ModifierKeys.Control)
				}));

		/// <summary>
		/// Command to move all forward the selection
		/// </summary>
		public static RoutedUICommand MoveAllForward =
			new RoutedUICommand("MoveAllForward", "MoveAllForward", typeof(RangeSlider),
				new InputGestureCollection(new InputGesture[] {
					new KeyGesture(Key.F, ModifierKeys.Alt)
				}));

		/// <summary>
		/// Command to move all back the selection
		/// </summary>
		public static RoutedUICommand MoveAllBack =
			new RoutedUICommand("MoveAllBack", "MoveAllBack", typeof(RangeSlider),
				new InputGestureCollection(new InputGesture[] {
					new KeyGesture(Key.B, ModifierKeys.Alt)
				}));

		private const double RepeatButtonMoveRatio = 0.1;

		//used to move the selection by x ratio when click the repeat buttons
		private const double DefaultSplittersThumbWidth = 10;

		private bool internalUpdate = false;
		private Thumb centerThumb; //the center thumb to move the range around
		private Thumb leftThumb;//the left thumb that is used to expand the range selected
		private Thumb rightThumb;//the right thumb that is used to expand the range selected
		private RepeatButton leftButton;//the left side of the control (movable left part)
		private RepeatButton rightButton;//the right side of the control (movable right part)
		private StackPanel visualElementsContainer;//stackpanel to store the visual elements for this control

		#endregion Data members

		#region properties and events

		private long movableRange = 0;

		private double movableWidth = 0;
		private readonly EventHandler widthChangedHandler;

		/// <summary>
		/// Event raised whenever the selected range is changed
		/// </summary>
		public event RangeSelectionChangedEventHandler RangeSelectionChanged
		{
			add { AddHandler(RangeSelectionChangedEvent, value); }
			remove { RemoveHandler(RangeSelectionChangedEvent, value); }
		}

		/// <summary>
		/// The min value for the range of the range slider
		/// </summary>
		public long RangeStart
		{
			get { return (long)GetValue(RangeStartProperty); }
			set { SetValue(RangeStartProperty, value); }
		}

		/// <summary>
		/// The max value for the range of the range slider
		/// </summary>
		public long RangeStop
		{
			get { return (long)GetValue(RangeStopProperty); }
			set { SetValue(RangeStopProperty, value); }
		}

		/// <summary>
		/// The min value of the selected range of the range slider
		/// </summary>
		public long RangeStartSelected
		{
			get { return (long)GetValue(RangeStartSelectedProperty); }
			set { SetValue(RangeStartSelectedProperty, value); }
		}

		/// <summary>
		/// The max value of the selected range of the range slider
		/// </summary>
		public long RangeStopSelected
		{
			get { return (long)GetValue(RangeStopSelectedProperty); }
			set { SetValue(RangeStopSelectedProperty, value); }
		}

		/// <summary>
		/// The min range value that you can have for the range slider
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when MinRange is set less than 0</exception>
		public long MinRange
		{
			get { return (long)GetValue(MinRangeProperty); }
			set { SetValue(MinRangeProperty, value); }
		}

		#endregion properties and events

		/// <summary>
		/// Static constructor
		/// </summary>
		static RangeSlider()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider))
				);
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public RangeSlider()
		{
			CommandBindings.Add(new CommandBinding(MoveBack, MoveBackHandler));
			CommandBindings.Add(new CommandBinding(MoveForward, MoveForwardHandler));
			CommandBindings.Add(new CommandBinding(MoveAllForward, MoveAllForwardHandler));
			CommandBindings.Add(new CommandBinding(MoveAllBack, MoveAllBackHandler));

			//hook to the size change event of the range slider
			widthChangedHandler = new EventHandler((s, e) => ReCalculateWidths());
			DependencyPropertyDescriptor
				.FromProperty(ActualWidthProperty, typeof(RangeSlider))
				.AddValueChanged(this, widthChangedHandler);
			Unloaded += RangeSlider_Unloaded;
		}

		private void RangeSlider_Unloaded(object sender, RoutedEventArgs e)
		{
			// make sure you always unhook the extenal events to avoid event-induced memory leakages!!!
			DependencyPropertyDescriptor
				.FromProperty(ActualWidthProperty, typeof(RangeSlider))
				.RemoveValueChanged(this, widthChangedHandler);
		}

		#region Command handlers

		/// <summary>
		/// moves the current selection with x value
		/// </summary>
		/// <param name="isLeft">True if you want to move to the left</param>
		public void MoveSelection(bool isLeft)
		{
			var widthChange = RepeatButtonMoveRatio * (RangeStopSelected - RangeStartSelected)
				* movableWidth / movableRange;

			widthChange = isLeft ? -widthChange : widthChange;
			MoveThumb(leftButton, rightButton, widthChange);
			ReCalculateRangeSelected(true, true);
		}

		/// <summary>
		/// Reset the Slider to the Start/End
		/// </summary>
		/// <param name="isStart">Pass true to reset to start point</param>
		public void ResetSelection(bool isStart)
		{
			double widthChange = RangeStop - RangeStart;
			widthChange = isStart ? -widthChange : widthChange;

			MoveThumb(leftButton, rightButton, widthChange);
			ReCalculateRangeSelected(true, true);
		}

		///<summary>
		/// Change the range selected
		///</summary>
		///<param name="span">The steps</param>
		public void MoveSelection(long span)
		{
			if (span > 0) {
				if (RangeStopSelected + span > RangeStop)
					span = RangeStop - RangeStopSelected;
			}
			else {
				if (RangeStartSelected + span < RangeStart)
					span = RangeStart - RangeStartSelected;
			}

			if (span != 0) {
				internalUpdate = true;//set flag to signal that the properties are being set by the object itself
				RangeStartSelected += span;
				RangeStopSelected += span;
				ReCalculateWidths();
				internalUpdate = false;//set flag to signal that the properties are being set by the object itself

				OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
			}
		}

		/// <summary>
		/// Sets the selected range in one go. If the selection is invalid, nothing happens.
		/// </summary>
		/// <param name="selectionStart">New selection start value</param>
		/// <param name="selectionStop">New selection stop value</param>
		public void SetSelectedRange(long selectionStart, long selectionStop)
		{
			var start = Math.Max(RangeStart, selectionStart);
			var stop = Math.Min(selectionStop, RangeStop);
			start = Math.Min(start, RangeStop - MinRange);
			stop = Math.Max(RangeStart + MinRange, stop);
			if (stop >= start + MinRange) {
				internalUpdate = true;//set flag to signal that the properties are being set by the object itself
				RangeStartSelected = start;
				RangeStopSelected = stop;
				ReCalculateWidths();
				internalUpdate = false;//set flag to signal that the properties are being set by the object itself
				OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
			}
		}

		/// <summary>
		/// Changes the selected range to the supplied range
		/// </summary>
		/// <param name="span">The span to zoom</param>
		public void ZoomToSpan(long span)
		{
			internalUpdate = true;//set flag to signal that the properties are being set by the object itself
								  // Ensure new span is within the valid range
			span = Math.Min(span, RangeStop - RangeStart);
			span = Math.Max(span, MinRange);
			if (span == RangeStopSelected - RangeStartSelected)
				return; // No change

			// First zoom half of it to the right
			var rightChange = (span - (RangeStopSelected - RangeStartSelected)) / 2;
			var leftChange = rightChange;

			// If we will hit the right edge, spill over the leftover change to the other side
			if (rightChange > 0 && RangeStopSelected + rightChange > RangeStop)
				leftChange += rightChange - (RangeStop - RangeStopSelected);
			RangeStopSelected = Math.Min(RangeStopSelected + rightChange, RangeStop);
			rightChange = 0;

			// If we will hit the left edge and there is space on the right, add the leftover change to the other side
			if (leftChange > 0 && RangeStartSelected - leftChange < RangeStart)
				rightChange = RangeStart - (RangeStartSelected - leftChange);
			RangeStartSelected = Math.Max(RangeStartSelected - leftChange, RangeStart);
			if (rightChange > 0) // leftovers to the right
				RangeStopSelected = Math.Min(RangeStopSelected + rightChange, RangeStop);

			ReCalculateWidths();
			internalUpdate = false;//set flag to signal that the properties are being set by the object itself
			OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
		}

		/// <summary>
		/// Overide to get the visuals from the control template
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			visualElementsContainer = EnforceInstance<StackPanel>("PART_RangeSliderContainer");
			centerThumb = EnforceInstance<Thumb>("PART_MiddleThumb");
			leftButton = EnforceInstance<RepeatButton>("PART_LeftEdge");
			rightButton = EnforceInstance<RepeatButton>("PART_RightEdge");
			leftThumb = EnforceInstance<Thumb>("PART_LeftThumb");
			rightThumb = EnforceInstance<Thumb>("PART_RightThumb");
			InitializeVisualElementsContainer();
			ReCalculateWidths();
		}

		//resizes the left column and the right column
		private static void MoveThumb(FrameworkElement x, FrameworkElement y, double horizonalChange)
		{
			double change = 0;
			if (horizonalChange < 0) //slider went left
				change = GetChangeKeepPositive(x.Width, horizonalChange);
			else if (horizonalChange > 0) //slider went right if(horizontal change == 0 do nothing)
				change = -GetChangeKeepPositive(y.Width, -horizonalChange);

			x.Width += change;
			y.Width -= change;
		}

		//ensures that the new value (newValue param) is a valid value. returns false if not
		private static double GetChangeKeepPositive(double width, double increment)
		{
			return Math.Max(width + increment, 0) - width;
		}

		private void MoveAllBackHandler(object sender, ExecutedRoutedEventArgs e)
		{
			ResetSelection(true);
		}

		private void MoveAllForwardHandler(object sender, ExecutedRoutedEventArgs e)
		{
			ResetSelection(false);
		}

		private void MoveBackHandler(object sender, ExecutedRoutedEventArgs e)
		{
			MoveSelection(true);
		}

		private void MoveForwardHandler(object sender, ExecutedRoutedEventArgs e)
		{
			MoveSelection(false);
		}

		#endregion Command handlers

		#region event handlers for visual elements to drag the range

		//drag thumb from the right splitter
		private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
		{
			MoveThumb(centerThumb, rightButton, e.HorizontalChange);
			ReCalculateRangeSelected(false, true);
		}

		//drag thumb from the left splitter
		private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
		{
			MoveThumb(leftButton, centerThumb, e.HorizontalChange);
			ReCalculateRangeSelected(true, false);
		}

		//left repeat button clicked
		private void LeftButtonClick(object sender, RoutedEventArgs e)
		{
			MoveSelection(true);
		}

		//right repeat button clicked
		private void RightButtonClick(object sender, RoutedEventArgs e)
		{
			MoveSelection(false);
		}

		//drag thumb from the middle
		private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
		{
			MoveThumb(leftButton, rightButton, e.HorizontalChange);
			ReCalculateRangeSelected(true, true);
		}

		#endregion event handlers for visual elements to drag the range

		#region logic to calculate the range

		//recalculates the movableRange. called from the RangeStop setter, RangeStart setter and MinRange setter
		private void ReCalculateRanges()
		{
			movableRange = RangeStop - RangeStart - MinRange;
		}

		//recalculates the movableWidth. called whenever the width of the control changes
		private void ReCalculateWidths()
		{
			if (leftButton != null && rightButton != null && centerThumb != null) {
				movableWidth = Math.Max(ActualWidth - rightThumb.ActualWidth - leftThumb.ActualWidth - centerThumb.MinWidth, 1);
				leftButton.Width = Math.Max(movableWidth * (RangeStartSelected - RangeStart) / movableRange, 0);
				rightButton.Width = Math.Max(movableWidth * (RangeStop - RangeStopSelected) / movableRange, 0);
				centerThumb.Width = Math.Max(ActualWidth - leftButton.Width - rightButton.Width - rightThumb.ActualWidth - leftThumb.ActualWidth, 0);
			}
		}

		//recalculates the rangeStartSelected called when the left thumb is moved and when the middle thumb is moved
		//recalculates the rangeStopSelected called when the right thumb is moved and when the middle thumb is moved
		private void ReCalculateRangeSelected(bool reCalculateStart, bool reCalculateStop)
		{
			internalUpdate = true;//set flag to signal that the properties are being set by the object itself
			if (reCalculateStart) {
				// Make sure to get exactly rangestart if thumb is at the start
				if (leftButton.Width == 0.0)
					RangeStartSelected = RangeStart;
				else
					RangeStartSelected =
						Math.Max(RangeStart, (long)(RangeStart + movableRange * leftButton.Width / movableWidth));
			}

			if (reCalculateStop) {
				// Make sure to get exactly rangestop if thumb is at the end
				if (rightButton.Width == 0.0)
					RangeStopSelected = RangeStop;
				else
					RangeStopSelected =
						Math.Min(RangeStop, (long)(RangeStop - movableRange * rightButton.Width / movableWidth));
			}

			internalUpdate = false;//set flag to signal that the properties are being set by the object itself

			if (reCalculateStart || reCalculateStop)
				//raise the RangeSelectionChanged event
				OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
		}

		#endregion logic to calculate the range

		//Raises the RangeSelectionChanged event
		private void OnRangeSelectionChanged(RangeSelectionChangedEventArgs e)
		{
			e.RoutedEvent = RangeSelectionChangedEvent;
			RaiseEvent(e);
		}

		

		#region Helper

		private T EnforceInstance<T>(string partName)
			where T : FrameworkElement, new()
		{
			if (!(GetTemplateChild(partName) is T element))
				element = new T();
			return element;
		}

		//adds all visual element to the conatiner
		private void InitializeVisualElementsContainer()
		{
			visualElementsContainer.Orientation = Orientation.Horizontal;
			leftThumb.Width = DefaultSplittersThumbWidth;
			leftThumb.Tag = "left";
			rightThumb.Width = DefaultSplittersThumbWidth;
			rightThumb.Tag = "right";

			//handle the drag delta
			centerThumb.DragDelta += CenterThumbDragDelta;
			leftThumb.DragDelta += LeftThumbDragDelta;
			rightThumb.DragDelta += RightThumbDragDelta;
			leftButton.Click += LeftButtonClick;
			rightButton.Click += RightButtonClick;
		}

		#endregion Helper
	}

	/// <summary>
	/// Event arguments for the Range slider RangeSelectionChanged event
	/// </summary>
	public class RangeSelectionChangedEventArgs : RoutedEventArgs
	{

		/// <summary>
		/// The new range start selected in the range slider
		/// </summary>
		public long NewRangeStart { get; set; }

		/// <summary>
		/// The new range stop selected in the range slider
		/// </summary>
		public long NewRangeStop { get; set; }

		/// <summary>
		/// sets the range start and range stop for the event args
		/// </summary>
		/// <param name="newRangeStart">The new range start set</param>
		/// <param name="newRangeStop">The new range stop set</param>
		internal RangeSelectionChangedEventArgs(long newRangeStart, long newRangeStop)
		{
			this.NewRangeStart = newRangeStart;
			this.NewRangeStop = newRangeStop;
		}

		/// <summary>
		/// sets the range start and range stop for the event args by using the slider RangeStartSelected and RangeStopSelected properties
		/// </summary>
		/// <param name="slider">The slider to get the info from</param>
		internal RangeSelectionChangedEventArgs(RangeSlider slider)
			: this(slider.RangeStartSelected, slider.RangeStopSelected)
		{ }
	}
}