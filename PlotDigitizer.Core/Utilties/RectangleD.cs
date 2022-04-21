using System;
using System.Text.Json.Serialization;

namespace PlotDigitizer.Core
{
	[Serializable]
	public struct RectangleD
	{
		public RectangleD(double left, double top, double width, double height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		public double Left { get; set; }
		public double Top { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }

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