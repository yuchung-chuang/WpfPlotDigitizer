using System;

namespace PlotDigitizer.Core
{
	/// <summary>
	/// This class is implemented because the core project cannot depend on <see cref="System.Windows.Point"/>.
	/// </summary>
	[Serializable]
	public struct PointD(double x, double y)
	{
		public double X { get; set; } = x;
		public double Y { get; set; } = y;

		public override bool Equals(object obj)
		{
			return obj is PointD d &&
				   X == d.X &&
				   Y == d.Y;
		}

		public override int GetHashCode()
		{
			var hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + X.GetHashCode();
			hashCode = hashCode * -1521134295 + Y.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(PointD left, PointD right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PointD left, PointD right)
		{
			return !(left == right);
		}

		public override string ToString() => $"PointD {{{X}, {Y}}}";
	}
}