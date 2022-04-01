using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace PlotDigitizer.Core
{
	public class Model
	{
		public Image<Rgba, byte> InputImage { get; set; }
		public Image<Rgba, byte> CroppedImage { get; set; }
		public Image<Rgba, byte> FilteredImage { get; set; }
		public Image<Rgba, byte> EdittedImage { get; set; }
		public DataType DataType { get; set; }
		public RectangleD AxisLimit { get; set; }
		public PointD AxisLogBase { get; set; }

		public Rectangle AxisLocation { get; set; }

		public Rgba FilterMin { get; set; } = new Rgba(0, 0, 0, byte.MaxValue);
		public Rgba FilterMax { get; set; } = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);

		public void CropImage()
		{
			CroppedImage = Methods.CropImage(InputImage, AxisLocation);
		}

		public void FilterImage()
		{
			FilteredImage = Methods.FilterRGB(CroppedImage, FilterMin, FilterMax);
		}
	}

	public enum DataType
	{
		Continuous,
		Discrete,
	}
}