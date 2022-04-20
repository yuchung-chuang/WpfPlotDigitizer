using System;

namespace PlotDigitizer.App
{
	public static class AxisHelpers
	{
		public static dynamic Add(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return a | b;
		}

		public static bool Contain(this Enum enumA, Enum enumB)
		{
			var (a, b) = ConvertEnums(enumA, enumB);
			return (a & b) == b;
		}

		public static double Clamp(double value, double Max, double Min)
		{
			if (Min > Max)
				Swap(ref Max, ref Min);

			if (value > Max)
				return Max;
			else if (value < Min)
				return Min;
			else
				return value;
		}

		public static void Swap<T>(ref T x, ref T y) => (x, y) = (y, x);

		/// <summary>
		/// 判斷<paramref name="value"/>是否位於閉區間[<paramref name="Max"/>,<paramref name="Min"/>]中。<paramref name="excludeBoundary"/>為真時，改為判斷開區間(<paramref name="Max"/>,<paramref name="Min"/>)。
		/// </summary>
		public static bool IsIn(double value, double Max, double Min, bool excludeBoundary = false)
		{
			if (Min > Max)
				Swap(ref Max, ref Min);
			if (!excludeBoundary)
				return (value <= Max && value >= Min) ? true : false;
			else
				return (value < Max && value > Min) ? true : false;
		}

		/// <summary>
		/// 判斷<paramref name="A"/>是否約等於<paramref name="B"/>。
		/// </summary>
		/// <param name="tol">容許誤差。</param>
		public static bool ApproxEqual(double A, double B, double tol)
		{
			return IsIn(A, B + tol, B - tol);
		}

		private static (ulong a, ulong b) ConvertEnums(Enum enumA, Enum enumB) => (Convert.ToUInt64(enumA), Convert.ToUInt64(enumB));
	}
}