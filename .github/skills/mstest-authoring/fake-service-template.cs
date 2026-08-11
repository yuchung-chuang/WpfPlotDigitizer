// Template: hand-rolled fake for a PlotDigitizer.Core service interface.
// There is no mocking library in PlotDigitizer.Test, so fakes are written by hand.
//
// Guidelines:
//  - Put shared fakes in PlotDigitizer.Test/Fakes/ and mark them `internal sealed`.
//  - Expose settable result properties so each test controls the return value.
//  - Record call counts and last-received arguments so tests can assert on interactions.
//  - Leave members a test does not need as NotImplementedException; that turns an
//    unexpected call into a loud failure instead of a silently wrong result.

using System;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

using PlotDigitizer.Core;

namespace PlotDigitizer.Core.Tests.Fakes
{
    internal sealed class FakeImageService : IImageService
    {
        public int CropImageCallCount { get; private set; }
        public RectangleD LastCropRoi { get; private set; }
        public Image<Rgba, byte> CropImageResult { get; set; }

        public int FilterRgbCallCount { get; private set; }
        public Image<Rgba, byte> FilterRgbResult { get; set; }

        public RectangleD GetAxisLocationResult { get; set; }
        public Func<Image<Rgba, byte>, RectangleD> GetAxisLocationBehaviour { get; set; }

        public IEnumerable<PointD> DiscretePointsResult { get; set; } = Array.Empty<PointD>();
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

        // Use the behaviour hook when a test needs to throw, to prove the caller's fallback path.
        public RectangleD GetAxisLocation(Image<Rgba, byte> image)
            => GetAxisLocationBehaviour is null ? GetAxisLocationResult : GetAxisLocationBehaviour(image);

        public IEnumerable<PointD> GetDiscretePoints(Image<Rgba, byte> image) => DiscretePointsResult;

        public IEnumerable<PointD> GetContinuousPoints(Image<Rgba, byte> image) => DiscretePointsResult;

        public IEnumerable<PointD> TransformData(
            IEnumerable<PointD> points, Size imageSize, RectangleD axLim, PointD axLogBase)
            => TransformDataResult;

        // Not needed yet: fail loudly rather than returning a misleading default.
        public Image<Rgba, byte> ClearBorder(Image<Rgba, byte> image) => throw new NotImplementedException();

        public AxisTextBox GetAxisTextBox(Image<Rgba, byte> image, RectangleD axis) => throw new NotImplementedException();

        public Image<Rgba, byte> RotateImage(Image<Rgba, byte> image, double angle) => throw new NotImplementedException();
    }
}
