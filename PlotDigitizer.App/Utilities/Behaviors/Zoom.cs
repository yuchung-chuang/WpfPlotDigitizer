using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	/// <summary>
	/// An attached property that provides zooming functionalities, usually it's attached to an <see cref="System.Windows.Controls.Image"/>.
	/// </summary>
	public static class Zoom
	{
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
		  "IsEnabled",
		  typeof(bool),
		  typeof(Zoom),
		  new PropertyMetadata(OnIsEnabledChanged));

		public static readonly DependencyProperty ModifierKeysProperty =
			DependencyProperty.RegisterAttached("ModifierKeys", typeof(ModifierKeys), typeof(Zoom), new PropertyMetadata(ModifierKeys.None));

		public static readonly DependencyProperty MouseWheelProperty =
			DependencyProperty.RegisterAttached("MouseWheel", typeof(EventHandler<double>), typeof(Zoom), new PropertyMetadata(null));

		public static readonly DependencyProperty ScaleProperty = DependencyProperty.RegisterAttached("Scale", typeof(double), typeof(Zoom), new PropertyMetadata(1d));

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static double GetScale(DependencyObject obj) 
			=> (double)obj.GetValue(ScaleProperty);

		public static void SetScale(DependencyObject obj, double value) 
			=> obj.SetValue(ScaleProperty, value);

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetIsEnabled(DependencyObject obj)
		  => (bool)obj.GetValue(IsEnabledProperty);

		public static void SetIsEnabled(DependencyObject obj, bool value)
		  => obj.SetValue(IsEnabledProperty, value);

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static ModifierKeys GetModifierKeys(DependencyObject obj)
		{
			return (ModifierKeys)obj.GetValue(ModifierKeysProperty);
		}

		public static void SetModifierKeys(DependencyObject obj, ModifierKeys value)
		{
			obj.SetValue(ModifierKeysProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static EventHandler<double> GetMouseWheel(DependencyObject obj) 
			=> (EventHandler<double>)obj.GetValue(MouseWheelProperty);

		public static void SetMouseWheel(DependencyObject obj, EventHandler<double> value) 
			=> obj.SetValue(MouseWheelProperty, value);

		private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is FrameworkElement element))
				throw new NotSupportedException($"Can only set the {IsEnabledProperty} attached behavior on a UIElement.");

			if ((bool)e.NewValue) {
				element.MouseWheel += Element_MouseWheel;
				element.EnsureTransforms();
				element.Parent?.SetValue(UIElement.ClipToBoundsProperty, true);
			}
			else {
				element.MouseWheel -= Element_MouseWheel;
			}
		}

		private static readonly double WheelTime = 0.1;
		private static void Element_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!(sender is FrameworkElement element)) {
				return;
			}
			var transforms = (element.RenderTransform as TransformGroup).Children;
			var translate = transforms.FirstOrDefault(tr => tr is TranslateTransform) as TranslateTransform;
			var scale = transforms.FirstOrDefault(tr => tr is ScaleTransform) as ScaleTransform;

			//ZoomSpeed
			var zoom = scale.ScaleX * (e.Delta > 0 ? .2 : -.2);

			var relative = e.GetPosition(element);
			var absolute = e.GetAbsolutePosition(element);
			//必須是scale先，translate後
			var ToScale = Math.Max(scale.ScaleX + zoom, 1);
			var ToX = Math.Max(Math.Min(absolute.X - relative.X * ToScale, 0), element.ActualWidth * (1 - ToScale));
			var ToY = Math.Max(Math.Min(absolute.Y - relative.Y * ToScale, 0), element.ActualHeight * (1 - ToScale));

			scale.BeginAnimation(ScaleTransform.ScaleXProperty, ToScale, WheelTime);
			scale.BeginAnimation(ScaleTransform.ScaleYProperty, ToScale, WheelTime);
			translate.BeginAnimation(TranslateTransform.XProperty, ToX, WheelTime);
			translate.BeginAnimation(TranslateTransform.YProperty, ToY, WheelTime);

			SetScale(element, ToScale);
			GetMouseWheel(element)?.Invoke(element, ToScale);
		}
	}
}