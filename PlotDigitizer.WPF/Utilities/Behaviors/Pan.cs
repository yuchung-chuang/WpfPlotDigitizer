﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.WPF
{
	/// <summary>
	/// An attached property that provides panning functionalities, usually it's attached to an <see cref="System.Windows.Controls.Image"/>.
	/// </summary>
	public static class Pan
	{
		#region Fields

		public static readonly DependencyProperty GestureProperty = DependencyProperty.RegisterAttached("Gesture", typeof(MouseGesture), typeof(Pan), new PropertyMetadata(new MouseGesture(MouseAction.LeftClick)));

		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(Pan), new PropertyMetadata(default(bool), OnIsEnabledChanged));

		public static readonly DependencyProperty ModifierKeysProperty = DependencyProperty.RegisterAttached("ModifierKeys", typeof(ModifierKeys), typeof(Pan), new PropertyMetadata(ModifierKeys.None));

		public static readonly DependencyProperty MouseButtonProperty = DependencyProperty.RegisterAttached("MouseButton", typeof(MouseButton), typeof(Pan), new PropertyMetadata(MouseButton.Left));

		private static readonly Cursor panCursor = new Uri(@"/Assets/pan.cur", UriKind.Relative).ToCursor();
		private static Cursor cursorCache;
		private static bool IsPanning;
		private static Point mouseAnchor;
		private static TranslateTransform translate;
		private static Point translateAnchor;

		#endregion Fields

		#region Methods

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static MouseGesture GetGesture(DependencyObject obj) => (MouseGesture)obj.GetValue(GestureProperty);

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetIsEnabled(UIElement element) => (bool)element.GetValue(IsEnabledProperty);

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static ModifierKeys GetModifierKeys(DependencyObject obj) => (ModifierKeys)obj.GetValue(ModifierKeysProperty);

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static MouseButton GetMouseButton(DependencyObject obj) => (MouseButton)obj.GetValue(MouseButtonProperty);

		public static void SetGesture(DependencyObject obj, MouseGesture value) => obj.SetValue(GestureProperty, value);

		public static void SetIsEnabled(UIElement element, bool value) => element.SetValue(IsEnabledProperty, value);

		public static void SetModifierKeys(DependencyObject obj, ModifierKeys value) => obj.SetValue(ModifierKeysProperty, value);

		public static void SetMouseButton(DependencyObject obj, MouseButton value) => obj.SetValue(MouseButtonProperty, value);

		private static void Element_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is FrameworkElement element)) {
				return;
			}
			if (!GetGesture(element).Matches(element, e)) {
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
			} else {
				element.MouseDown -= Element_MouseDown;
				element.MouseUp -= Element_MouseUp;
				element.MouseMove -= Element_MouseMove;
			}
		}

		private static Cursor ToCursor(this Uri uri) => new Cursor(Application.GetResourceStream(uri).Stream);

		#endregion Methods
	}
}