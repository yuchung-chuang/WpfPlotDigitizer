using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class DataPointsNodeTests
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

        /// <summary>
        /// Kept alive for the duration of each test so that <see cref="EdittedImageNode"/>
        /// can call <c>filteredImage.Data?.Copy()</c> without accessing freed memory.
        /// Disposed in <see cref="OnTestCleanup"/>.
        /// </summary>
        private Image<Rgba, byte> seedImage;

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
        }

        [TestCleanup]
        public void OnTestCleanup() => seedImage?.Dispose();

        /// <summary>
        /// Seeds <see cref="filteredImage"/> with a live image so that
        /// <see cref="EdittedImageNode"/> can copy it.  The image is stored in
        /// <see cref="seedImage"/> and disposed by <see cref="OnTestCleanup"/>.
        /// </summary>
        private void SeedEdittedImage()
        {
            seedImage = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = seedImage;
        }

        // ──────────────────────────────────────────────────────────────────
        // Early-return when dependencies not updated
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDependenciesNotUpdated_ReturnsNull()
        {
            var result = dataPoints.GetUpdatedData();
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDependenciesNotUpdated_DoesNotCallAnyService()
        {
            dataPoints.GetUpdatedData();

            Assert.AreEqual(0, imageService.GetDiscretePointsCallCount);
            Assert.AreEqual(0, imageService.GetContinuousPointsCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Dispatch to Continuous
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDataTypeIsContinuous_CallsGetContinuousPoints()
        {
            var expected = new List<PointD> { new PointD(1, 2) };
            imageService.ContinuousPointsResult = expected;

            SeedEdittedImage();
            dataType.Data = DataType.Continuous;

            var result = dataPoints.GetUpdatedData();

            Assert.AreEqual(1, imageService.GetContinuousPointsCallCount);
            Assert.AreEqual(0, imageService.GetDiscretePointsCallCount);
            Assert.AreSame(expected, result);
        }

        // ──────────────────────────────────────────────────────────────────
        // Dispatch to Discrete
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDataTypeIsDiscrete_CallsGetDiscretePoints()
        {
            var expected = new List<PointD> { new PointD(3, 4) };
            imageService.DiscretePointsResult = expected;

            SeedEdittedImage();
            dataType.Data = DataType.Discrete;

            var result = dataPoints.GetUpdatedData();

            Assert.AreEqual(1, imageService.GetDiscretePointsCallCount);
            Assert.AreEqual(0, imageService.GetContinuousPointsCallCount);
            Assert.AreSame(expected, result);
        }

        // ──────────────────────────────────────────────────────────────────
        // Recomputes when DataType changes
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterDataTypeChanges_Recomputes()
        {
            SeedEdittedImage();
            dataType.Data = DataType.Continuous;
            dataPoints.GetUpdatedData();

            dataType.Data = DataType.Discrete;
            dataPoints.GetUpdatedData();

            Assert.AreEqual(1, imageService.GetContinuousPointsCallCount);
            Assert.AreEqual(1, imageService.GetDiscretePointsCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Recomputes when edittedImage changes
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterEdittedImageChanges_Recomputes()
        {
            SeedEdittedImage();
            dataType.Data = DataType.Continuous;
            dataPoints.GetUpdatedData();

            using var newFiltered = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = newFiltered;
            dataPoints.GetUpdatedData();

            Assert.AreEqual(2, imageService.GetContinuousPointsCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Caching
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_CalledTwiceWithNoChange_DoesNotRecompute()
        {
            SeedEdittedImage();
            dataType.Data = DataType.Continuous;

            dataPoints.GetUpdatedData();
            dataPoints.GetUpdatedData();

            Assert.AreEqual(1, imageService.GetContinuousPointsCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Outdated event
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Outdated_WhenDataTypeChanges_IsRaised()
        {
            SeedEdittedImage();
            dataType.Data = DataType.Continuous;
            dataPoints.GetUpdatedData();

            var outdatedRaised = false;
            dataPoints.Outdated += (s, e) => outdatedRaised = true;

            dataType.Data = DataType.Discrete;

            Assert.IsTrue(outdatedRaised);
        }
    }
}
