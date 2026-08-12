using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.Services
{
    [TestClass]
    public class EmguCvServiceTests
    {
        private const double Tol = 1e-9;
        private static readonly Rgba Opaque = new(255, 255, 255, 255);

        private EmguCvService service;

        [TestInitialize]
        public void Setup() => service = new EmguCvService();

        // ── TransformData ──────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void TransformData_WithLinearAxes_MapsImageCornersOntoAxisLimits()
        {
            var imageSize = new Size(100, 50);
            var axLim = new RectangleD(0, 0, 10, 5);
            var points = new[] { new PointD(0, 50), new PointD(100, 0), new PointD(50, 25) };

            var data = service.TransformData(points, imageSize, axLim, new PointD(0, 0)).ToList();

            AssertPoint(0, 0, data[0]);
            AssertPoint(10, 5, data[1]);
            AssertPoint(5, 2.5, data[2]);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void TransformData_WithLogBaseOnX_MapsTheImageMidpointToTheGeometricMean()
        {
            var imageSize = new Size(100, 100);
            var axLim = new RectangleD(1, 0, 99, 10);
            var points = new[] { new PointD(0, 100), new PointD(50, 100), new PointD(100, 100) };

            var data = service.TransformData(points, imageSize, axLim, new PointD(10, 0)).ToList();

            Assert.IsTrue(MathHelpers.ApproxEqual(1, data[0].X, 1e-6), $"expected 1 but was {data[0].X}");
            Assert.IsTrue(MathHelpers.ApproxEqual(10, data[1].X, 1e-6), $"expected 10 but was {data[1].X}");
            Assert.IsTrue(MathHelpers.ApproxEqual(100, data[2].X, 1e-6), $"expected 100 but was {data[2].X}");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void TransformData_WithLogBaseOnY_MapsTheImageMidpointToTheGeometricMean()
        {
            var imageSize = new Size(100, 100);
            var axLim = new RectangleD(0, 1, 10, 99);
            var points = new[] { new PointD(0, 100), new PointD(0, 50), new PointD(0, 0) };

            var data = service.TransformData(points, imageSize, axLim, new PointD(0, 10)).ToList();

            Assert.IsTrue(MathHelpers.ApproxEqual(1, data[0].Y, 1e-6), $"expected 1 but was {data[0].Y}");
            Assert.IsTrue(MathHelpers.ApproxEqual(10, data[1].Y, 1e-6), $"expected 10 but was {data[1].Y}");
            Assert.IsTrue(MathHelpers.ApproxEqual(100, data[2].Y, 1e-6), $"expected 100 but was {data[2].Y}");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void TransformData_WithNoPoints_ReturnsEmpty()
        {
            var data = service.TransformData([], new Size(10, 10), new RectangleD(0, 0, 1, 1), new PointD(0, 0));

            Assert.AreEqual(0, data.Count());
        }

        // ── GetContinuousPoints ────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetContinuousPoints_WithFullyTransparentImage_ReturnsEmpty()
        {
            using var image = new Image<Rgba, byte>(3, 3);

            Assert.AreEqual(0, service.GetContinuousPoints(image).Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetContinuousPoints_ReturnsEveryOpaquePixelInRowMajorOrder()
        {
            using var image = new Image<Rgba, byte>(3, 3);
            image[0, 2] = Opaque;
            image[1, 1] = Opaque;

            var points = service.GetContinuousPoints(image).ToList();

            CollectionAssert.AreEqual(new[] { new PointD(2, 0), new PointD(1, 1) }, points);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetContinuousPoints_IgnoresColouredPixelsWithZeroAlpha()
        {
            using var image = new Image<Rgba, byte>(3, 3);
            image[1, 1] = new Rgba(255, 0, 0, 0);

            Assert.AreEqual(0, service.GetContinuousPoints(image).Count());
        }

        // ── GetDiscretePoints ──────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetDiscretePoints_WithFullyTransparentImage_ReturnsEmpty()
        {
            using var image = new Image<Rgba, byte>(10, 10);

            Assert.AreEqual(0, service.GetDiscretePoints(image).Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetDiscretePoints_WithSinglePixelBlobs_ReturnsTheBlobPixels()
        {
            using var image = new Image<Rgba, byte>(10, 10);
            image[2, 3] = Opaque;
            image[7, 8] = Opaque;

            var points = service.GetDiscretePoints(image).ToList();

            Assert.AreEqual(2, points.Count);
            CollectionAssert.Contains(points, new PointD(3, 2));
            CollectionAssert.Contains(points, new PointD(8, 7));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetDiscretePoints_WithASquareBlob_ReturnsItsCentroid()
        {
            using var image = new Image<Rgba, byte>(10, 10);
            for (var y = 2; y <= 6; y++) {
                for (var x = 2; x <= 6; x++) {
                    image[y, x] = Opaque;
                }
            }

            var points = service.GetDiscretePoints(image).ToList();

            Assert.AreEqual(1, points.Count);
            Assert.AreEqual(new PointD(4, 4), points[0]);
        }

        // ── RotateImage ────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void RotateImage_WithZeroAngle_PreservesSizeAndPixels()
        {
            using var image = new Image<Rgba, byte>(6, 3);
            image[1, 2] = Opaque;

            using var rotated = service.RotateImage(image, 0);

            Assert.AreEqual(image.Size, rotated.Size);
            Assert.AreEqual(Opaque, rotated[1, 2]);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RotateImage_WithoutCropping_GrowsTheCanvasToFitTheRotatedImage()
        {
            using var image = new Image<Rgba, byte>(40, 20);

            using var rotated = service.RotateImage(image, 45);

            Assert.IsTrue(rotated.Width > image.Width, $"width {rotated.Width} did not grow past {image.Width}");
            Assert.IsTrue(rotated.Height > image.Height, $"height {rotated.Height} did not grow past {image.Height}");
        }

        // ── ClearBorder ────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void ClearBorder_RemovesBlobsTouchingTheBorderAndKeepsInteriorBlobs()
        {
            using var image = new Image<Rgba, byte>(10, 10);
            image[0, 0] = Opaque;
            image[5, 5] = Opaque;

            using var cleared = service.ClearBorder(image);

            Assert.AreEqual(0, cleared[0, 0].Alpha);
            Assert.AreEqual(255, cleared[5, 5].Alpha);
        }

        // ── CropImage ──────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void CropImage_WithRectangleD_RoundsTheRoiToTheNearestEvenPixel()
        {
            using var image = new Image<Rgba, byte>(10, 10);
            image[2, 2] = Opaque;

            // Math.Round is banker's rounding: 1.5 -> 2 and 4.5 -> 4.
            using var cropped = service.CropImage(image, new RectangleD(1.5, 2.4, 4.5, 3.6));

            Assert.AreEqual(new Size(4, 4), cropped.Size);
            Assert.AreEqual(Opaque, cropped[0, 0]);
        }

        // ── Argument guards ────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetAxisLocation_WithNullImage_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => service.GetAxisLocation(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CropImage_WithNullImage_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => service.CropImage(null, new Rectangle(0, 0, 1, 1)));
            Assert.ThrowsException<ArgumentNullException>(() => service.CropImage(null, new RectangleD(0, 0, 1, 1)));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterRGB_WithNullImage_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => service.FilterRGB(null, new Rgba(), Opaque));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ClearBorder_WithNullImage_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => service.ClearBorder(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetContinuousPoints_WithNullImage_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => service.GetContinuousPoints(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetDiscretePoints_WithNullImage_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => service.GetDiscretePoints(null));
        }

        // ── GetAxisTextBox ─────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetAxisTextBox_OnARealPlot_FindsEveryTextBoxAroundTheAxis()
        {
            using var image = new Image<Rgba, byte>("Assets/data.png");
            var axis = service.GetAxisLocation(image);

            var textBox = service.GetAxisTextBox(image, axis);

            foreach (var (name, box) in new (string, Rectangle)[] {
                (nameof(textBox.XMin), textBox.XMin),
                (nameof(textBox.XMax), textBox.XMax),
                (nameof(textBox.YMin), textBox.YMin),
                (nameof(textBox.YMax), textBox.YMax),
                (nameof(textBox.XLabel), textBox.XLabel),
                (nameof(textBox.YLabel), textBox.YLabel),
            }) {
                Assert.AreNotEqual(default(Rectangle), box, $"{name} was not detected");
            }

            Assert.IsTrue(textBox.XMin.Top > axis.Bottom, "XMin must sit below the axis");
            Assert.IsTrue(textBox.XLabel.Top > axis.Bottom, "XLabel must sit below the axis");
            Assert.IsTrue(textBox.YMin.Right < axis.Left, "YMin must sit left of the axis");
            Assert.IsTrue(textBox.YLabel.Right < axis.Left, "YLabel must sit left of the axis");
        }

        private static void AssertPoint(double expectedX, double expectedY, PointD actual)
        {
            Assert.IsTrue(MathHelpers.ApproxEqual(expectedX, actual.X, Tol)
                && MathHelpers.ApproxEqual(expectedY, actual.Y, Tol),
                $"expected ({expectedX}, {expectedY}) but was {actual}");
        }
    }
}
