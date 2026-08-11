using System;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IImageService"/>. The test project has no mocking
	/// library, so collaborators are implemented directly. Every member records what it was
	/// called with, and each result is configurable so a test can pin a node or a view model
	/// down without running real image processing.
	/// </summary>
	internal sealed class FakeImageService : IImageService
	{
		public int CropImageCallCount { get; private set; }

		public RectangleD LastCropRoi { get; private set; }

		public Rectangle LastCropRectangle { get; private set; }

		public List<Rectangle> CropRectangles { get; } = [];

		public Image<Rgba, byte> CropImageResult { get; set; }

		public int FilterRgbCallCount { get; private set; }

		public Rgba LastFilterMin { get; private set; }

		public Rgba LastFilterMax { get; private set; }

		public Image<Rgba, byte> FilterRgbResult { get; set; }

		public Exception ThrowOnFilterRgb { get; set; }

		public RectangleD GetAxisLocationResult { get; set; }

		public Func<Image<Rgba, byte>, RectangleD> GetAxisLocationBehaviour { get; set; }

		public int GetAxisLocationCallCount { get; private set; }

		public IEnumerable<PointD> PointsResult { get; set; } = Array.Empty<PointD>();

		public IEnumerable<PointD> DiscretePointsResult { get; set; }

		public IEnumerable<PointD> ContinuousPointsResult { get; set; }

		public int GetDiscretePointsCallCount { get; private set; }

		public int GetContinuousPointsCallCount { get; private set; }

		public IEnumerable<PointD> TransformDataResult { get; set; } = Array.Empty<PointD>();

		public int TransformDataCallCount { get; private set; }

		public IEnumerable<PointD> LastTransformPoints { get; private set; }

		public Size LastTransformImageSize { get; private set; }

		public RectangleD LastTransformAxisLimit { get; private set; }

		public PointD LastTransformAxisLogBase { get; private set; }

		public AxisTextBox GetAxisTextBoxResult { get; set; }

		public int GetAxisTextBoxCallCount { get; private set; }

		public RectangleD LastAxisTextBoxAxis { get; private set; }

		public Exception ThrowOnGetAxisTextBox { get; set; }

		public Image<Rgba, byte> ClearBorderResult { get; set; }

		public int ClearBorderCallCount { get; private set; }

		public Image<Rgba, byte> RotateImageResult { get; set; }

		public int RotateImageCallCount { get; private set; }

		public double LastRotateAngle { get; private set; }

		public Image<Rgba, byte> CropImage(Image<Rgba, byte> image, RectangleD roi)
		{
			CropImageCallCount++;
			LastCropRoi = roi;
			return CropImageResult ?? image;
		}

		public Image<Rgba, byte> CropImage(Image<Rgba, byte> image, Rectangle roi)
		{
			CropImageCallCount++;
			LastCropRectangle = roi;
			CropRectangles.Add(roi);
			return CropImageResult ?? image;
		}

		public Image<Rgba, byte> FilterRGB(Image<Rgba, byte> image, Rgba min, Rgba max)
		{
			FilterRgbCallCount++;
			LastFilterMin = min;
			LastFilterMax = max;
			if (ThrowOnFilterRgb != null) {
				throw ThrowOnFilterRgb;
			}
			return FilterRgbResult ?? image;
		}

		public RectangleD GetAxisLocation(Image<Rgba, byte> image)
		{
			GetAxisLocationCallCount++;
			return GetAxisLocationBehaviour is null ? GetAxisLocationResult : GetAxisLocationBehaviour(image);
		}

		public IEnumerable<PointD> GetDiscretePoints(Image<Rgba, byte> image)
		{
			GetDiscretePointsCallCount++;
			return DiscretePointsResult ?? PointsResult;
		}

		public IEnumerable<PointD> GetContinuousPoints(Image<Rgba, byte> image)
		{
			GetContinuousPointsCallCount++;
			return ContinuousPointsResult ?? PointsResult;
		}

		public IEnumerable<PointD> TransformData(IEnumerable<PointD> points, Size imageSize, RectangleD axLim, PointD axLogBase)
		{
			TransformDataCallCount++;
			LastTransformPoints = points;
			LastTransformImageSize = imageSize;
			LastTransformAxisLimit = axLim;
			LastTransformAxisLogBase = axLogBase;
			return TransformDataResult;
		}

		public Image<Rgba, byte> ClearBorder(Image<Rgba, byte> image)
		{
			ClearBorderCallCount++;
			return ClearBorderResult ?? image;
		}

		public AxisTextBox GetAxisTextBox(Image<Rgba, byte> image, RectangleD axis)
		{
			GetAxisTextBoxCallCount++;
			LastAxisTextBoxAxis = axis;
			if (ThrowOnGetAxisTextBox != null) {
				throw ThrowOnGetAxisTextBox;
			}
			return GetAxisTextBoxResult;
		}

		public Image<Rgba, byte> RotateImage(Image<Rgba, byte> image, double angle)
		{
			RotateImageCallCount++;
			LastRotateAngle = angle;
			return RotateImageResult ?? image;
		}
	}
}
