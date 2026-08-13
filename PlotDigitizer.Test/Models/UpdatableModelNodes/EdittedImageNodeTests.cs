using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class EdittedImageNodeTests
    {
        private FakeImageService imageService;
        private InputImageNode inputImage;
        private AxisLocationNode axisLocation;
        private CroppedImageNode croppedImage;
        private FilterMinNode filterMin;
        private FilterMaxNode filterMax;
        private FilteredImageNode filteredImage;
        private EdittedImageNode edittedImage;

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
        }

        // ──────────────────────────────────────────────────────────────────
        // Null filtered image yields null editted image
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenFilteredImageDataIsNull_ReturnsNull()
        {
            // filteredImage.Data is null and is updated (force it)
            filteredImage.Data = null;

            var result = edittedImage.GetUpdatedData();

            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────────────────────────
        // Happy path — returns a copy (distinct instance)
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenFilteredImageHasData_ReturnsDistinctCopy()
        {
            using var filterSource = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = filterSource;

            using var result = edittedImage.GetUpdatedData();

            Assert.IsNotNull(result);
            Assert.AreNotSame(filterSource, result,
                "EdittedImageNode must return a Copy(), not the same instance.");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenFilteredImageHasData_SetsIsUpdatedTrue()
        {
            using var filterSource = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = filterSource;

            edittedImage.GetUpdatedData();

            Assert.IsTrue(edittedImage.IsUpdated);
        }

        // ──────────────────────────────────────────────────────────────────
        // Dependencies not updated → early return
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenFilteredImageIsStale_ReturnsNull()
        {
            // filteredImage is stale (no inputImage assigned), so IsAllDependenciesUpdated
            // returns false and edittedImage should not update.
            var result = edittedImage.GetUpdatedData();

            Assert.IsNull(result);
            Assert.IsFalse(edittedImage.IsUpdated);
        }

        // ──────────────────────────────────────────────────────────────────
        // Caching
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_CalledTwiceWithNoChange_ReturnsSameInstance()
        {
            using var filterSource = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = filterSource;

            var first = edittedImage.GetUpdatedData();
            var second = edittedImage.GetUpdatedData();

            // Both calls should return the same cached copy (not make a new copy each call)
            Assert.AreSame(first, second,
                "Calling GetUpdatedData twice without a change must return the cached instance.");
            first?.Dispose();
        }

        // ──────────────────────────────────────────────────────────────────
        // Invalidation when filtered image changes
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Outdated_WhenFilteredImageChanges_IsRaised()
        {
            using var filterSource = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = filterSource;
            edittedImage.GetUpdatedData();

            var outdatedRaised = false;
            edittedImage.Outdated += (s, e) => outdatedRaised = true;

            using var newFilter = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = newFilter;

            Assert.IsTrue(outdatedRaised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterFilteredImageChanges_ReturnsNewCopy()
        {
            using var filterSource1 = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = filterSource1;
            var first = edittedImage.GetUpdatedData();

            using var filterSource2 = new Image<Rgba, byte>(4, 4);
            filteredImage.Data = filterSource2;
            var second = edittedImage.GetUpdatedData();

            Assert.AreNotSame(first, second,
                "After an upstream change, EdittedImageNode must produce a fresh copy.");
            first?.Dispose();
            second?.Dispose();
        }
    }
}
