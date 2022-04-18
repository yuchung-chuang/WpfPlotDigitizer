using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public interface IModel
	{
		Image<Rgba, byte> InputImage { get; set; }
		Image<Rgba, byte> CroppedImage { get; }
		Image<Rgba, byte> FilteredImage { get; }
		Image<Rgba, byte> EdittedImage { get; set; }
		Image<Rgba, byte> PreviewImage { get; }
		IEnumerable<PointD> Data { get; }
	}
}