using System;
using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.Models
{
    /// <summary>
    /// Tests for <see cref="DataNode"/>.
    ///
    /// <see cref="DataNode"/> does not need an explicit <c>DependsOn(editedImage)</c> — it
    /// already depends on <c>editedImage</c> transitively through <c>dataPoints</c>, so
    /// invalidation cascades correctly. See
    /// <see cref="Outdated_WhenEditedImageChanges_CascadesThroughDataPoints"/>.
    ///
    /// <c>Update()</c> does guard against <c>editedImage.Data</c> being null (e.g. when the
    /// image has been cleared) before reading <c>editedImage.Data.Size</c>, returning null
    /// instead of throwing. See
    /// <see cref="GetUpdatedData_WhenDataPointsNonNullAndEditedImageDataNull_ReturnsNull"/>.
    /// </summary>
    [TestClass]
    public class DataNodeTests
    {
        private FakeImageService imageService;
        private InputImageNode inputImage;
        private AxisLocationNode axisLocation;
        private CroppedImageNode croppedImage;
        private FilterMinNode filterMin;
        private FilterMaxNode filterMax;
        private FilteredImageNode filteredImage;
        private EditedImageNode editedImage;
        private DataTypeNode dataType;
        private DataPointsNode dataPoints;
        private AxisTextBoxNode axisTextBox;
        private AxisLimitNode axisLimit;
        private AxisLogBaseNode axisLogBase;
        private DataNode dataNode;

        [TestInitialize]
        public void OnTestInitialize()
        {
            imageService = new FakeImageService();
            inputImage = new InputImageNode();
            axisLocation = new AxisLocationNode(inputImage);
            croppedImage = new CroppedImageNode(inputImage, axisLocation, imageService);
            filterMin = new FilterMinNode(inputImage);
            filterMax = new FilterMaxNode(inputImage);
            filteredImage = new FilteredImageNode(croppedImage, filterMin, filterMax, imageService);
            editedImage = new EditedImageNode(filteredImage);
            dataType = new DataTypeNode(inputImage);
            dataPoints = new DataPointsNode(editedImage, dataType, imageService);
            axisTextBox = new AxisTextBoxNode(axisLocation);
            axisLimit = new AxisLimitNode(axisTextBox);
            axisLogBase = new AxisLogBaseNode(inputImage);
            dataNode = new DataNode(editedImage, axisLimit, axisLogBase, dataPoints, imageService);
        }

        // ──────────────────────────────────────────────────────────────────
        // Null dataPoints.Data yields null result
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDataPointsDataIsNull_ReturnsNull()
        {
            // Make dataPoints updated but with null Data
            dataPoints.Data = null;
            // axisLimit and axisLogBase need to be updated too
            axisLimit.Data = new RectangleD(0, 1, 10, 20);
            axisLogBase.Data = new PointD(10, 10);

            var result = dataNode.GetUpdatedData();

            Assert.IsNull(result);
            Assert.AreEqual(0, imageService.TransformDataCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Happy path: dataPoints has data and editedImage has data
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenAllDependenciesReady_CallsTransformData()
        {
            var points = new List<PointD> { new PointD(1, 2) };
            var transformResult = new List<PointD> { new PointD(3, 4) };
            imageService.TransformDataResult = transformResult;
            var axLim = new RectangleD(0, 1, 10, 20);
            var logBase = new PointD(10, 10);

            // editedImage must be seeded BEFORE dataPoints so that the editedImage.Updated
            // event (which invalidates dataPoints via DependsOn) fires before we mark
            // dataPoints updated by assigning dataPoints.Data.
            axisLimit.Data = axLim;
            axisLogBase.Data = logBase;
            using var img = new Image<Rgba, byte>(4, 4);
            editedImage.Data = img;
            dataPoints.Data = points; // last: marks dataPoints Updated; nothing invalidates it after this

            var result = dataNode.GetUpdatedData();

            Assert.AreEqual(1, imageService.TransformDataCallCount);
            Assert.AreSame(transformResult, result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenAllDependenciesReady_PassesCorrectArgsToTransformData()
        {
            var points = new List<PointD> { new PointD(1, 2) };
            imageService.TransformDataResult = new List<PointD>();
            var axLim = new RectangleD(0, 1, 10, 20);
            var logBase = new PointD(10, 10);

            axisLimit.Data = axLim;
            axisLogBase.Data = logBase;
            using var img = new Image<Rgba, byte>(6, 8);
            editedImage.Data = img;
            dataPoints.Data = points; // last

            dataNode.GetUpdatedData();

            Assert.AreEqual(axLim, imageService.LastTransformAxisLimit);
            Assert.AreEqual(logBase, imageService.LastTransformAxisLogBase);
            Assert.AreEqual(img.Size, imageService.LastTransformImageSize);
        }

        // ──────────────────────────────────────────────────────────────────
        // editedImage.Data null (e.g. cleared) yields null result, no throw
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDataPointsNonNullAndEditedImageDataNull_ReturnsNull()
        {
            // editedImage.Data is null (default after construction — never assigned)
            editedImage.Data = null; // explicitly null and mark updated

            var points = new List<PointD> { new PointD(1, 2) };
            dataPoints.Data = points;
            axisLimit.Data = new RectangleD(0, 1, 10, 20);
            axisLogBase.Data = new PointD(10, 10);

            var result = dataNode.GetUpdatedData();

            Assert.IsNull(result);
            Assert.AreEqual(0, imageService.TransformDataCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Invalidation cascades through dataPoints when editedImage changes
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Outdated_WhenEditedImageChanges_CascadesThroughDataPoints()
        {
            // Seed: editedImage first, then dataPoints last so both are Updated.
            imageService.TransformDataResult = new List<PointD>();
            axisLimit.Data = new RectangleD(0, 1, 10, 20);
            axisLogBase.Data = new PointD(10, 10);
            using var img1 = new Image<Rgba, byte>(4, 4);
            editedImage.Data = img1;
            dataPoints.Data = new List<PointD> { new PointD(1, 2) };

            dataNode.GetUpdatedData(); // compute once: all three dependencies satisfied
            Assert.IsTrue(dataNode.IsUpdated, "Sanity: dataNode must be updated after seeding.");

            var outdatedRaised = false;
            dataNode.Outdated += (s, e) => outdatedRaised = true;

            // Changing editedImage fires editedImage.Updated
            // → dataPoints.OnOutdated() (DataPointsNode depends on editedImage)
            // → dataPoints.Outdated fires
            // → dataNode.OnOutdated() (DataNode depends on dataPoints)
            // → dataNode.Outdated fires
            using var img2 = new Image<Rgba, byte>(8, 8);
            editedImage.Data = img2;

            Assert.IsTrue(outdatedRaised,
                "dataNode must become outdated when editedImage changes, via the dataPoints dependency.");
            Assert.IsFalse(dataNode.IsUpdated,
                "dataNode.IsUpdated must be false after the cascade.");
        }

        // ──────────────────────────────────────────────────────────────────
        // Dependencies not updated → early return
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDependenciesNotUpdated_ReturnsNull()
        {
            // Nothing seeded; dataPoints, axisLimit, axisLogBase are all stale
            var result = dataNode.GetUpdatedData();
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDependenciesNotUpdated_DoesNotCallTransformData()
        {
            dataNode.GetUpdatedData();
            Assert.AreEqual(0, imageService.TransformDataCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Caching
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_CalledTwiceWithNoChange_CallsTransformDataOnce()
        {
            var points = new List<PointD> { new PointD(1, 2) };
            imageService.TransformDataResult = new List<PointD>();

            axisLimit.Data = new RectangleD(0, 1, 10, 20);
            axisLogBase.Data = new PointD(10, 10);
            using var img = new Image<Rgba, byte>(4, 4);
            editedImage.Data = img;
            dataPoints.Data = points; // last: editedImage already Updated, so this survives

            dataNode.GetUpdatedData();
            dataNode.GetUpdatedData();

            Assert.AreEqual(1, imageService.TransformDataCallCount);
        }
    }
}
