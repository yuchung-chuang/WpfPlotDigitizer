using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	/// <summary>
	/// An attached property that provides panning functionalities, usually it's attached to an <see cref="System.Windows.Controls.Image"/>.
	/// </summary>
	public static class Pan
	{
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
		  "IsEnabled",
		  typeof(bool),
		  typeof(Pan),
		  new PropertyMetadata(default(bool), OnIsEnabledChanged));

		public static readonly DependencyProperty MouseButtonProperty =
			DependencyProperty.RegisterAttached("MouseButton", typeof(MouseButton), typeof(Pan), new PropertyMetadata(MouseButton.Left));

		public static readonly DependencyProperty ModifierKeysProperty =
			DependencyProperty.RegisterAttached("ModifierKeys", typeof(ModifierKeys), typeof(Pan), new PropertyMetadata(ModifierKeys.None));

		private static readonly Cursor panCursor = new Uri(@"/Assets/pan.cur", UriKind.Relative).ToCursor();

		private static Cursor cursorCache;

		private static Point mouseAnchor;

		private static TranslateTransform translate;

		private static Point translateAnchor;

		private static bool IsPanning;

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetIsEnabled(UIElement element)
		  => (bool)element.GetValue(IsEnabledProperty);

		public static void SetIsEnabled(UIElement element, bool value)
		  => element.SetValue(IsEnabledProperty, value);

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static MouseButton GetMouseButton(DependencyObject obj)
		{
			return (MouseButton)obj.GetValue(MouseButtonProperty);
		}

		public static void SetMouseButton(DependencyObject obj, MouseButton value)
		{
			obj.SetValue(MouseButtonProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static ModifierKeys GetModifierKeys(DependencyObject obj)
		{
			return (ModifierKeys)obj.GetValue(ModifierKeysProperty);
		}

		public static void SetModifierKeys(DependencyObject obj, ModifierKeys value)
		{
			obj.SetValue(ModifierKeysProperty, value);
		}

		private static Cursor ToCursor(this Uri uri) => new Cursor(Application.GetResourceStream(uri).Stream);

		private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is FrameworkElement element))
				throw new NotSupportedException($"Can only set the {IsEnabledProperty} attached behavior on a UIElement.");

			if ((bool)e.NewValue) {
				element.MouseDown += Element_MouseDown;
				element.MouseUp += Element_MouseUp;
				element.MouseMove += Element_MouseMove;
				element.EnsureTransforms();
				element.Parent?.SetValue(UIElement.ClipToBoundsProperty, true);
			}
			else {
				element.MouseDown -= Element_MouseDown;
				element.MouseUp -= Element_MouseUp;
				element.MouseMove -= Element_MouseMove;
			}
		}

		private static void Element_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is FrameworkElement element)) {
				return;
			}
			if (!InputCheck(element, e)) {
				return;
			}
			var transforms = (element.RenderTransform as TransformGroup).Children;
			translate = transforms.FirstOrDefault(tr => tr is TranslateTransform) as TranslateTransform;
			mouseAnchor = e.GetAbsolutePosition(element);
			translateAnchor = new Point(translate.X, translate.Y);
			cursorCache = element.Cursor;
			element.Cursor = panCursor;
			element.CaptureMouse();
			IsPanning = true;
		}

		private static void Element_MouseMove(object sender, MouseEventArgs e)
		{
			if (!(sender is FrameworkElement element)) {
				return;
			}
			if (!IsPanning || !element.IsMouseCaptured) {
				return;
			}
			var delta = e.GetAbsolutePosition(element) - mouseAnchor;
			var transforms = (element.RenderTransform as TransformGroup).Children;
			var scale = transforms.FirstOrDefault(t => t is ScaleTransform) as ScaleTransform;
			var toX = Math.Max(Math.Min(translateAnchor.X + delta.X, 0), element.ActualWidth * (1 - scale.ScaleX));
			var toY = Math.Max(Math.Min(translateAnchor.Y + delta.Y, 0), element.ActualHeight * (1 - scale.ScaleY));
			translate.BeginAnimation(TranslateTransform.XProperty, toX, 0);
			translate.BeginAnimation(TranslateTransform.YProperty, toY, 0);
		}

		private static void Element_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is FrameworkElement element)) {
				return;
			}
			if (!element.IsMouseCaptured) {
				return;
			}
			element.ReleaseMouseCapture();
			element.Cursor = cursorCache;
			IsPanning = false;
		}

		private static bool InputCheck(FrameworkElement element, MouseButtonEventArgs _)
		{
			var mouseButton = GetMouseButton(element);
			var key = GetModifierKeys(element);
			return IsMouseButtonPressed(mouseButton) && IsKeyPressed(key);

			static bool IsKeyPressed(ModifierKeys key)
			{
				return key == ModifierKeys.None || Contains(Keyboard.Modifiers, key);

				static bool Contains(ModifierKeys a, ModifierKeys b)
				{
					return (a & b) == b;
				}
			}

			static bool IsMouseButtonPressed(MouseButton mouseButton)
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
}