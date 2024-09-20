using System;
using System.Drawing;
using System.Text.Json.Serialization;

namespace PlotDigitizer.Core
{
	/// <summary>
	/// This class is implemented because the core project cannot depend on <see cref="System.Windows.Rectangle"/>.
	/// </summary>
	[Serializable]
	public struct RectangleD(double left, double top, double width, double height)
	{
		public RectangleD(Rectangle maxRect) : this(maxRect.Left, maxRect.Top, maxRect.Width, maxRect.Height)
		{
		}

		public double Left { get; set; } = left;
		public double Top { get; set; } = top;
		public double Width { get; set; } = width;
		public double Height { get; set; } = height;

		[JsonIgnore]
		public double X => Left;

		[JsonIgnore]
		public double Y => Top;

		[JsonIgnore]
		public double Right => Left + Width;

		[JsonIgnore]
		public double Bottom => Top + Height;

		public override bool Equals(object obj)
		{
			return obj is RectangleD d &&
				   Left == d.Left &&
				   Top == d.Top &&
				   Width == d.Width &&
				   Height == d.Height;
		}

		public override int GetHashCode()
		{
			var hashCode = 1636396453;
			hashCode = hashCode * -1521134295 + Left.GetHashCode();
			hashCode = hashCode * -1521134295 + Top.GetHashCode();
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			return hashCode;
		}

		public override string ToString() => $"RectangleD {{{Left},{Top},{Width},{Bottom}}}";

		public static bool operator ==(RectangleD left, RectangleD right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RectangleD left, RectangleD right)
		{
			return !(left == right);
		}
	}
}