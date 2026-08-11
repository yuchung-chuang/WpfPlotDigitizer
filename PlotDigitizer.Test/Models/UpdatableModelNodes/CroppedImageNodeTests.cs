using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests
{
    /// <summary>
    /// Covers <see cref="CroppedImageNode"/>, the first derived node in the digitization
    /// graph. The behaviour under test is the lazy update contract shared by every node:
    /// compute only when stale, stay silent while dependencies are missing, and recompute
    /// after an upstream change.
    /// </summary>
    [TestClass()]
    public class CroppedImageNodeTests
    {
        private FakeImageService imageService;
        private InputImageNode inputImage;
        private AxisLocationNode axisLocation;
        private CroppedImageNode croppedImage;

        [TestInitialize]
        public void OnTestInitialize()
        {
            imageService = new FakeImageService();
            inputImage = new InputImageNode();
            axisLocation = new AxisLocationNode(inputImage);
            croppedImage = new CroppedImageNode(inputImage, axisLocation, imageService);
        }

        [TestMethod()]
        [TestCategory("Unit")]
        public void GetUpdatedData_WithoutInputImage_DoesNotCropAndReturnsNull()
        {
            var result = croppedImage.GetUpdatedData();

            Assert.IsNull(result);
            Assert.AreEqual(0, imageService.CropImageCallCount,
                "A node must not invoke its service while an upstream dependency has no data.");
        }

        [TestMethod()]
        [TestCategory("Unit")]
        public void GetUpdatedData_WithInputImage_CropsUsingCurrentAxisLocation()
        {
            using var source = new Image<Rgba, byte>(40, 30);
            using var cropped = new Image<Rgba, byte>(10, 10);
            imageService.CropImageResult = cropped;
            inputImage.Data = source;
            axisLocation.Data = new RectangleD(1, 2, 3, 4);

            var result = croppedImage.GetUpdatedData();

            Assert.AreSame(cropped, result);
            Assert.AreEqual(1, imageService.CropImageCallCount);
            Assert.AreEqual(new RectangleD(1, 2, 3, 4), imageService.LastCropRoi);
        }

        [TestMethod()]
        [TestCategory("Unit")]
        public void GetUpdatedData_CalledTwiceWithoutChange_ComputesOnlyOnce()
        {
            using var source = new Image<Rgba, byte>(40, 30);
            inputImage.Data = source;
            axisLocation.Data = new RectangleD(0, 0, 10, 10);

            croppedImage.GetUpdatedData();
            croppedImage.GetUpdatedData();

            Assert.AreEqual(1, imageService.CropImageCallCount,
                "An up-to-date node must serve cached data instead of recomputing.");
        }

        [TestMethod()]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterAxisLocationChanges_RecomputesCrop()
        {
            using var source = new Image<Rgba, byte>(40, 30);
            inputImage.Data = source;
            axisLocation.Data = new RectangleD(0, 0, 10, 10);
            croppedImage.GetUpdatedData();

            axisLocation.Data = new RectangleD(5, 5, 20, 20);
            croppedImage.GetUpdatedData();

            Assert.AreEqual(2, imageService.CropImageCallCount,
                "Changing AxisLocation must invalidate the downstream cropped image.");
            Assert.AreEqual(new RectangleD(5, 5, 20, 20), imageService.LastCropRoi);
        }

        [TestMethod()]
        [TestCategory("Unit")]
        public void OutdatedEvent_WhenUpstreamChanges_IsRaised()
        {
            using var source = new Image<Rgba, byte>(40, 30);
            inputImage.Data = source;
            axisLocation.Data = new RectangleD(0, 0, 10, 10);
            croppedImage.GetUpdatedData();

            var outdatedRaised = false;
            croppedImage.Outdated += (s, e) => outdatedRaised = true;

            axisLocation.Data = new RectangleD(1, 1, 5, 5);

            Assert.IsTrue(outdatedRaised,
                "Downstream nodes must broadcast Outdated so the facades can raise PropertyOutdated.");
        }
    }
}