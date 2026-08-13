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
    /// BUG DOCUMENTED: <see cref="DataNode"/> reads <c>edittedImage.Data.Size</c> inside
    /// <c>Update()</c> but does NOT call <c>DependsOn(edittedImage)</c> in its constructor.
    /// When <c>dataPoints.Data</c> is non-null and <c>edittedImage.Data</c> is null,
    /// <c>Update()</c> throws <see cref="NullReferenceException"/> (see
    /// <see cref="GetUpdatedData_WhenDataPointsNonNullAndEdittedImageDataNull_ThrowsNullReferenceException"/>).
    /// Location: <c>PlotDigitizer.Core\Models\UpdatableModelNodes\DataNode.cs</c>, line 33 (missing null guard).
    ///
    /// NOTE — retraction of earlier bug claim: it was previously claimed that a change to
    /// <c>edittedImage</c> does NOT invalidate <see cref="DataNode"/>. This was incorrect.
    /// <see cref="DataNode"/> depends on <c>dataPoints</c>, and <c>dataPoints</c> depends on
    /// <c>edittedImage</c>, so the invalidation does cascade transitively. See
    /// <see cref="Outdated_WhenEdittedImageChanges_CascadesThroughDataPoints"/>.
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
        private EdittedImageNode edittedImage;
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
            edittedImage = new EdittedImageNode(filteredImage);
            dataType = new DataTypeNode(inputImage);
            dataPoints = new DataPointsNode(edittedImage, dataType, imageService);
            axisTextBox = new AxisTextBoxNode(axisLocation);
            axisLimit = new AxisLimitNode(axisTextBox);
            axisLogBase = new AxisLogBaseNode(inputImage);
            dataNode = new DataNode(edittedImage, axisLimit, axisLogBase, dataPoints, imageService);
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
        // Happy path: dataPoints has data and edittedImage has data
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

            // edittedImage must be seeded BEFORE dataPoints so that the edittedImage.Updated
            // event (which invalidates dataPoints via DependsOn) fires before we mark
            // dataPoints updated by assigning dataPoints.Data.
            axisLimit.Data = axLim;
            axisLogBase.Data = logBase;
            using var img = new Image<Rgba, byte>(4, 4);
            edittedImage.Data = img;
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
            edittedImage.Data = img;
            dataPoints.Data = points; // last

            dataNode.GetUpdatedData();

            Assert.AreEqual(axLim, imageService.LastTransformAxisLimit);
            Assert.AreEqual(logBase, imageService.LastTransformAxisLogBase);
            Assert.AreEqual(img.Size, imageService.LastTransformImageSize);
        }

        // ──────────────────────────────────────────────────────────────────
        // BUG: NullReferenceException when dataPoints.Data is non-null but
        //      edittedImage.Data is null (no null guard in DataNode.Update).
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDataPointsNonNullAndEdittedImageDataNull_ThrowsNullReferenceException()
        {
            // edittedImage.Data is null (default after construction — never assigned)
            edittedImage.Data = null; // explicitly null and mark updated

            var points = new List<PointD> { new PointD(1, 2) };
            dataPoints.Data = points;
            axisLimit.Data = new RectangleD(0, 1, 10, 20);
            axisLogBase.Data = new PointD(10, 10);

            // Current behaviour: NullReferenceException because Update() does
            // edittedImage.Data.Size without a null guard.
            Assert.ThrowsException<NullReferenceException>(() => dataNode.GetUpdatedData());
        }

        // ──────────────────────────────────────────────────────────────────
        // Invalidation cascades through dataPoints when edittedImage changes
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Outdated_WhenEdittedImageChanges_CascadesThroughDataPoints()
        {
            // Seed: edittedImage first, then dataPoints last so both are Updated.
            imageService.TransformDataResult = new List<PointD>();
            axisLimit.Data = new RectangleD(0, 1, 10, 20);
            axisLogBase.Data = new PointD(10, 10);
            using var img1 = new Image<Rgba, byte>(4, 4);
            edittedImage.Data = img1;
            dataPoints.Data = new List<PointD> { new PointD(1, 2) };

            dataNode.GetUpdatedData(); // compute once: all three dependencies satisfied
            Assert.IsTrue(dataNode.IsUpdated, "Sanity: dataNode must be updated after seeding.");

            var outdatedRaised = false;
            dataNode.Outdated += (s, e) => outdatedRaised = true;

            // Changing edittedImage fires edittedImage.Updated
            // → dataPoints.OnOutdated() (DataPointsNode depends on edittedImage)
            // → dataPoints.Outdated fires
            // → dataNode.OnOutdated() (DataNode depends on dataPoints)
            // → dataNode.Outdated fires
            using var img2 = new Image<Rgba, byte>(8, 8);
            edittedImage.Data = img2;

            Assert.IsTrue(outdatedRaised,
                "dataNode must become outdated when edittedImage changes, via the dataPoints dependency.");
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
            edittedImage.Data = img;
            dataPoints.Data = points; // last: edittedImage already Updated, so this survives

            dataNode.GetUpdatedData();
            dataNode.GetUpdatedData();

            Assert.AreEqual(1, imageService.TransformDataCallCount);
        }
    }
}
