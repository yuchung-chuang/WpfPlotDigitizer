using System;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IImageService"/>. The test project has no mocking
	/// library, so collaborators are implemented directly. Members that no test needs yet
	/// throw, so an unexpected call fails loudly instead of returning a misleading default.
	/// </summary>
	internal sealed class FakeImageService : IImageService
	{
		public int CropImageCallCount { get; private set; }

		public RectangleD LastCropRoi { get; private set; }

		public Image<Rgba, byte> CropImageResult { get; set; }

		public int FilterRgbCallCount { get; private set; }

		public Image<Rgba, byte> FilterRgbResult { get; set; }

		public RectangleD GetAxisLocationResult { get; set; }

		public Func<Image<Rgba, byte>, RectangleD> GetAxisLocationBehaviour { get; set; }

		public IEnumerable<PointD> PointsResult { get; set; } = Array.Empty<PointD>();

		public IEnumerable<PointD> TransformDataResult { get; set; } = Array.Empty<PointD>();

		public Image<Rgba, byte> CropImage(Image<Rgba, byte> image, RectangleD roi)
		{
			CropImageCallCount++;
			LastCropRoi = roi;
			return CropImageResult ?? image;
		}

		public Image<Rgba, byte> CropImage(Image<Rgba, byte> image, Rectangle roi)
		{
			CropImageCallCount++;
			return CropImageResult ?? image;
		}

		public Image<Rgba, byte> FilterRGB(Image<Rgba, byte> image, Rgba min, Rgba max)
		{
			FilterRgbCallCount++;
			return FilterRgbResult ?? image;
		}

		public RectangleD GetAxisLocation(Image<Rgba, byte> image)
			=> GetAxisLocationBehaviour is null ? GetAxisLocationResult : GetAxisLocationBehaviour(image);

		public IEnumerable<PointD> GetDiscretePoints(Image<Rgba, byte> image) => PointsResult;

		public IEnumerable<PointD> GetContinuousPoints(Image<Rgba, byte> image) => PointsResult;

		public IEnumerable<PointD> TransformData(IEnumerable<PointD> points, Size imageSize, RectangleD axLim, PointD axLogBase)
			=> TransformDataResult;

		public Image<Rgba, byte> ClearBorder(Image<Rgba, byte> image) => throw new NotImplementedException();

		public AxisTextBox GetAxisTextBox(Image<Rgba, byte> image, RectangleD axis) => throw new NotImplementedException();

		public Image<Rgba, byte> RotateImage(Image<Rgba, byte> image, double angle) => throw new NotImplementedException();
	}
}
