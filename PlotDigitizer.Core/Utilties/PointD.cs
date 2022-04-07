using System;

namespace PlotDigitizer.Core
{
    [Serializable]
	public struct PointD
    {
        public double X { get; set; }
        public double Y { get; set; }
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

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
        public override string ToString()
        {
            return $"PointD {{{X}, {Y}}}";
        }
    }
}

