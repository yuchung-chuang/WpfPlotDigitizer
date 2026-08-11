using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class FilteredImageNodeTests
    {
        private FakeImageService imageService;
        private InputImageNode inputImage;
        private AxisLocationNode axisLocation;
        private CroppedImageNode croppedImage;
        private FilterMinNode filterMin;
        private FilterMaxNode filterMax;
        private FilteredImageNode filteredImage;

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
        }

        private void SeedValidGraph()
        {
            using var src = new Image<Rgba, byte>(40, 30);
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 40, 30);
        }

        // ──────────────────────────────────────────────────────────────────
        // Early-return: dependencies not updated
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenNoDependenciesUpdated_DoesNotCallFilterRgb()
        {
            var result = filteredImage.GetUpdatedData();

            Assert.AreEqual(0, imageService.FilterRgbCallCount);
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────────────────────────
        // Early-return: croppedImage.Data is null
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenCroppedImageDataIsNull_DoesNotCallFilterRgb()
        {
            // inputImage must be seeded FIRST so that filterMin/filterMax become updated.
            // croppedImage.Data is set to null AFTER so the null survives — if we set
            // inputImage.Data after, croppedImage would recompute via CropImage and become
            // non-null, which would allow FilterRGB to run.
            using var src = new Image<Rgba, byte>(4, 4);
            inputImage.Data = src;

            croppedImage.Data = null; // overwrite with null after inputImage is set

            var result = filteredImage.GetUpdatedData();

            Assert.AreEqual(0, imageService.FilterRgbCallCount);
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────────────────────────
        // Happy path
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenAllDependenciesReady_CallsFilterRgbOnce()
        {
            using var src = new Image<Rgba, byte>(4, 4);
            using var filtered = new Image<Rgba, byte>(4, 4);
            imageService.FilterRgbResult = filtered;
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 4, 4);

            var result = filteredImage.GetUpdatedData();

            Assert.AreEqual(1, imageService.FilterRgbCallCount);
            Assert.AreSame(filtered, result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_CallsFilterRgb_WithCurrentFilterMinAndMax()
        {
            using var src = new Image<Rgba, byte>(4, 4);
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 4, 4);

            var expectedMin = new Rgba(10, 20, 30, 255);
            var expectedMax = new Rgba(100, 150, 200, 255);
            filterMin.Data = expectedMin;
            filterMax.Data = expectedMax;

            filteredImage.GetUpdatedData();

            Assert.AreEqual(expectedMin, imageService.LastFilterMin);
            Assert.AreEqual(expectedMax, imageService.LastFilterMax);
        }

        // ──────────────────────────────────────────────────────────────────
        // Caching — does not recompute when already updated
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_CalledTwiceWithNoChange_CallsFilterRgbOnce()
        {
            using var src = new Image<Rgba, byte>(4, 4);
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 4, 4);

            filteredImage.GetUpdatedData();
            filteredImage.GetUpdatedData();

            Assert.AreEqual(1, imageService.FilterRgbCallCount);
        }

        // ──────────────────────────────────────────────────────────────────
        // Invalidation when upstream changes
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterFilterMinChanges_RecomputesFilter()
        {
            using var src = new Image<Rgba, byte>(4, 4);
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 4, 4);

            filteredImage.GetUpdatedData();
            filterMin.Data = new Rgba(5, 5, 5, 255);
            filteredImage.GetUpdatedData();

            Assert.AreEqual(2, imageService.FilterRgbCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterFilterMaxChanges_RecomputesFilter()
        {
            using var src = new Image<Rgba, byte>(4, 4);
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 4, 4);

            filteredImage.GetUpdatedData();
            filterMax.Data = new Rgba(200, 200, 200, 255);
            filteredImage.GetUpdatedData();

            Assert.AreEqual(2, imageService.FilterRgbCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterCroppedImageChanges_RecomputesFilter()
        {
            using var src = new Image<Rgba, byte>(4, 4);
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 4, 4);

            filteredImage.GetUpdatedData();

            using var newCropped = new Image<Rgba, byte>(4, 4);
            croppedImage.Data = newCropped;
            filteredImage.GetUpdatedData();

            Assert.AreEqual(2, imageService.FilterRgbCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Outdated_WhenCroppedImageChanges_IsRaised()
        {
            using var src = new Image<Rgba, byte>(4, 4);
            inputImage.Data = src;
            axisLocation.Data = new RectangleD(0, 0, 4, 4);
            filteredImage.GetUpdatedData();

            var outdatedRaised = false;
            filteredImage.Outdated += (s, e) => outdatedRaised = true;

            using var newCropped = new Image<Rgba, byte>(4, 4);
            croppedImage.Data = newCropped;

            Assert.IsTrue(outdatedRaised);
        }
    }
}
