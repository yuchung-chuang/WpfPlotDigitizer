using Emgu.CV.Structure;
using System.Drawing;

namespace PlotDigitizer.Core
{
	public interface ISetting
	{
		RectangleD AxisLimit { get; set; }
		Rectangle AxisLocation { get; set; }
		PointD AxisLogBase { get; set; }
		Rgba FilterMax { get; set; }
		Rgba FilterMin { get; set; }
		DataType DataType { get; set; }

		void Load(ISetting setting);
	}
}