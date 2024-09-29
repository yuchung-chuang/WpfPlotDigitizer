using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;

namespace PlotDigitizer.Core
{
	public interface IImageService
	{
		Image<Rgba, byte> ClearBorder(Image<Rgba, byte> image);
		Image<Rgba, byte> CropImage(Image<Rgba, byte> image, RectangleD roi);
		Image<Rgba, byte> CropImage(Image<Rgba, byte> image, Rectangle roi);
		Image<Rgba, byte> FilterRGB(Image<Rgba, byte> image, Rgba min, Rgba max);
        Rectangle? FixROI(Image<Rgba, byte> image, Rectangle roi);
        (Rectangle xMax, Rectangle xMin, Rectangle yMax, Rectangle yMin) GetAxisLimitTextBoxes(Image<Rgba, byte> image, RectangleD axis);
        RectangleD? GetAxisLocation(Image<Rgba, byte> image);
		IEnumerable<PointD> GetContinuousPoints(Image<Rgba, byte> image);
		IEnumerable<PointD> GetDiscretePoints(Image<Rgba, byte> image);
        IEnumerable<PointD> TransformData(IEnumerable<PointD> points, Size imageSize, RectangleD axLim, PointD axLogBase);
	}
}