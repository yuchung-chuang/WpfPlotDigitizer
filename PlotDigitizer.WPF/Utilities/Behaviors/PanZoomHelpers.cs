using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PlotDigitizer.WPF
{
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
			var duration = TimeSpan.FromMilliseconds(durationMs);
			AnimationTimeline animation = to switch
			{
				int i => new Int32Animation(i, duration),
				double d => new DoubleAnimation(d, duration),
				Color color => new ColorAnimation(color, duration),
				Thickness thickness => new ThicknessAnimation(thickness, duration),
				Rect rect => new RectAnimation(rect, duration),
				_ => throw new NotSupportedException(),
			};
			animatable.BeginAnimation(dp, animation);
		}
	}
}