using PlotDigitizer.Core;

using System.Globalization;

namespace PlotDigitizer.Web.Models
{
	/// <summary>
	/// Form shape of a <see cref="RectangleD"/>, so a selection box can round trip through a post
	/// without hand written parsing.
	/// </summary>
	public class RectangleInput
	{
		public double X { get; set; }

		public double Y { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public RectangleD ToRectangleD() => new(X, Y, Width, Height);

		public bool IsEmpty => Width < 1 || Height < 1;

		public static RectangleInput From(RectangleD rectangle) => new()
		{
			X = rectangle.Left,
			Y = rectangle.Top,
			Width = rectangle.Width,
			Height = rectangle.Height,
		};

		public string Format(double value) => value.ToString(CultureInfo.InvariantCulture);
	}
}
