using Emgu.CV;
using Emgu.CV.Structure;

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
	}

	public enum DataType
	{
		Continuous,
		Discrete,
	}
}