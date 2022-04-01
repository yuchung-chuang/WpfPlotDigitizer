using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PlotDigitizer.App
{
	/// <summary>
	/// 提供平移相依屬性的靜態類別。
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

		private static bool InputCheck(FrameworkElement element, MouseButtonEventArgs e)
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

	/// <summary>
	/// 提供縮放相依屬性的靜態類別。
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

		private static readonly double WheelTime = 0.1;

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static bool GetIsEnabled(UIElement element)
		  => (bool)element.GetValue(IsEnabledProperty);

		public static void SetIsEnabled(UIElement element, bool value)
		  => element.SetValue(IsEnabledProperty, value);

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static ModifierKeys GetModifierKeys(DependencyObject obj)
		{
			return (ModifierKeys)obj.GetValue(ModifierKeysProperty);
		}

		public static void SetModifierKeys(DependencyObject obj, ModifierKeys value)
		{
			obj.SetValue(ModifierKeysProperty, value);
		}

		public static EventHandler<double> GetMouseWheel(DependencyObject obj) => (EventHandler<double>)obj.GetValue(MouseWheelProperty);

		public static void SetMouseWheel(DependencyObject obj, EventHandler<double> value) => obj.SetValue(MouseWheelProperty, value);

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

			GetMouseWheel(element)?.Invoke(element, ToScale);
		}
	}

	public static class PanZoomHelpers
	{
		/// <summary>
		/// 取得滑鼠相對於<paramref name="element"/>在螢幕上的座標，此結果不會受到<see cref="UIElement.RenderTransform"/>的影響。
		/// </summary>
		public static Point GetAbsolutePosition(this MouseEventArgs e, UIElement element)
		{
			var transformsTemplate = (element.RenderTransform as TransformGroup).Children;
			var transformsIdentity = new TransformCollection();
			// 重設UIElement的transforms
			(element.RenderTransform as TransformGroup).Children = transformsIdentity;
			// 取得座標
			var absolute = e.GetPosition(element);
			// 復原transforms
			(element.RenderTransform as TransformGroup).Children = transformsTemplate;
			return absolute;
		}

		public static void EnsureTransforms(this UIElement element)
		{
			var transform = element.RenderTransform;
			if (transform is TransformGroup group &&
			  group.Children.Count == 4 &&
			  group.Children[0] is ScaleTransform &&
			  group.Children[1] is TranslateTransform &&
			  group.Children[2] is RotateTransform &&
			  group.Children[3] is SkewTransform)
				return; //需要確認模式，以保證使用此方法檢驗過的element都具有相同的transform，方便使用者紀錄transformCache
			element.RenderTransform = new TransformGroup
			{
				Children = new TransformCollection
				{
				  // 必須是scale先，translate後
				  new ScaleTransform(),
				  new TranslateTransform(),
				  new RotateTransform(),
				  new SkewTransform(),
				},
			};
			element.RenderTransformOrigin = new Point(0, 0);
		}

		/// <summary>
		/// 針對<paramref name="animatable"/>執行泛型動畫。
		/// </summary>
		/// <typeparam name="PropertyType">執行動畫的屬性型別。</typeparam>
		/// <param name="animatable">要執行動畫的個體。</param>
		/// <param name="dp">要執行動畫的屬性。</param>
		/// <param name="to">屬性改變的目標值。</param>
		/// <param name="durationMs">動畫的時長。</param>
		public static void BeginAnimation<PropertyType>(this IAnimatable animatable, DependencyProperty dp, PropertyType to, double durationMs)
		{
			DependencyObject animation;
			var duration = TimeSpan.FromMilliseconds(durationMs);
			switch (to) {
				case int i:
					animation = new Int32Animation(i, duration);
					break;
				case double d:
					animation = new DoubleAnimation(d, duration);
					break;
				case Color color:
					animation = new ColorAnimation(color, duration);
					break;
				case Thickness thickness:
					animation = new ThicknessAnimation(thickness, duration);
					break;
				case Rect rect:
					animation = new RectAnimation(rect, duration);
					break;
				default:
					throw new NotSupportedException();
			}
			animatable.BeginAnimation(dp, animation as AnimationTimeline);
		}
	}
}