using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    [TestClass]
    public class FilterPageViewModelTests
    {
        private Model model;
        private Setting setting;
        private FakeImageService imageService;
        private FilterPageViewModel vm;

        [TestInitialize]
        public void Setup()
        {
            model = new Model();
            setting = new Setting();
            imageService = new FakeImageService();
            vm = new FilterPageViewModel(model, setting, imageService, null);
        }

        // ── IsEnabled ──────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenModelIsNull_ReturnsFalse()
        {
            var noModel = new FilterPageViewModel(null, setting, imageService, null);
            Assert.IsFalse(noModel.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenCroppedImageIsNull_ReturnsFalse()
        {
            model.CroppedImage = null;
            Assert.IsFalse(vm.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenModelHasCroppedImage_ReturnsTrue()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            Assert.IsTrue(vm.IsEnabled);
        }

        // ── CroppedImage ───────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void CroppedImage_ReturnsSameReferenceAsModelCroppedImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            Assert.AreSame(img, vm.CroppedImage);
        }

        // ── FilterMax / FilterMin round-trip ───────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMax_Set_SetsMaxRMaxGMaxB()
        {
            model.CroppedImage = null; // disable to avoid FilterImage side effect
            vm.FilterMax = new Rgba(200, 150, 100, 255);

            Assert.AreEqual(200.0, vm.MaxR);
            Assert.AreEqual(150.0, vm.MaxG);
            Assert.AreEqual(100.0, vm.MaxB);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMax_Get_ReturnsDerivedFromMaxChannels()
        {
            // Set via individual channel properties; reading FilterMax should compose them
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img; // avoid null Image result

            vm.MaxR = 10; vm.MaxG = 20; vm.MaxB = 30;

            var max = vm.FilterMax;
            Assert.AreEqual(10.0, max.Red);
            Assert.AreEqual(20.0, max.Green);
            Assert.AreEqual(30.0, max.Blue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMin_Set_SetsMinRMinGMinB()
        {
            model.CroppedImage = null;
            vm.FilterMin = new Rgba(10, 20, 30, 255);

            Assert.AreEqual(10.0, vm.MinR);
            Assert.AreEqual(20.0, vm.MinG);
            Assert.AreEqual(30.0, vm.MinB);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMin_Get_ReturnsDerivedFromMinChannels()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;

            vm.MinR = 5; vm.MinG = 10; vm.MinB = 15;

            var min = vm.FilterMin;
            Assert.AreEqual(5.0, min.Red);
            Assert.AreEqual(10.0, min.Green);
            Assert.AreEqual(15.0, min.Blue);
        }

        // ── FilterImage ────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterImage_WhenDisabled_DoesNotCallFilterRgb()
        {
            model.CroppedImage = null;

            vm.FilterImage();

            Assert.AreEqual(0, imageService.FilterRgbCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterImage_WhenEnabled_CallsFilterRgbWithCurrentMinMax()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;
            vm.MaxR = 200; vm.MaxG = 150; vm.MaxB = 100;
            vm.MinR = 10; vm.MinG = 20; vm.MinB = 30;

            vm.FilterImage();

            Assert.AreEqual(200.0, imageService.LastFilterMax.Red);
            Assert.AreEqual(150.0, imageService.LastFilterMax.Green);
            Assert.AreEqual(100.0, imageService.LastFilterMax.Blue);
            Assert.AreEqual(10.0, imageService.LastFilterMin.Red);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterImage_WhenFilterRgbThrows_SwallowsException()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.ThrowOnFilterRgb = new Exception("filter failed");

            // Should not propagate
            vm.FilterImage();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterImage_WhenEnabled_SetsImageProperty()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            using var filtered = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = filtered;

            vm.FilterImage();

            Assert.AreSame(filtered, vm.Image);
        }

        // ── [OnChangedMethod] hooks ────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void MaxR_WhenSet_TriggersFilterImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;
            var callsBefore = imageService.FilterRgbCallCount;

            vm.MaxR = 100;

            Assert.IsTrue(imageService.FilterRgbCallCount > callsBefore);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MinB_WhenSet_TriggersFilterImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;
            var callsBefore = imageService.FilterRgbCallCount;

            vm.MinB = 5;

            Assert.IsTrue(imageService.FilterRgbCallCount > callsBefore);
        }

        // ── Enter ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenEnabled_LoadsFilterMinMaxFromSetting()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;
            setting.FilterMin = new Rgba(5, 10, 15, 255);
            setting.FilterMax = new Rgba(200, 210, 220, 255);

            vm.Enter();

            Assert.AreEqual(5.0, vm.MinR);
            Assert.AreEqual(200.0, vm.MaxR);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenDisabled_DoesNotLoadFromSetting()
        {
            model.CroppedImage = null;
            setting.FilterMin = new Rgba(5, 10, 15, 255);

            vm.Enter();

            // MinR should still be the default (0), not 5
            Assert.AreEqual(0.0, vm.MinR);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenEnabled_CallsFilterImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;

            vm.Enter();

            Assert.IsTrue(imageService.FilterRgbCallCount > 0);
        }

        // ── Leave ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenEnabled_WritesFilterMinToSetting()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;
            vm.MinR = 10; vm.MinG = 20; vm.MinB = 30;

            vm.Leave();

            Assert.AreEqual(10.0, setting.FilterMin.Red);
            Assert.AreEqual(20.0, setting.FilterMin.Green);
            Assert.AreEqual(30.0, setting.FilterMin.Blue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenEnabled_WritesFilterMaxToSetting()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            imageService.FilterRgbResult = img;
            vm.MaxR = 200; vm.MaxG = 210; vm.MaxB = 220;

            vm.Leave();

            Assert.AreEqual(200.0, setting.FilterMax.Red);
            Assert.AreEqual(210.0, setting.FilterMax.Green);
            Assert.AreEqual(220.0, setting.FilterMax.Blue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenDisabled_DoesNotWriteToSetting()
        {
            model.CroppedImage = null;
            setting.FilterMin = new Rgba(77, 77, 77, 255);

            vm.Leave();

            Assert.AreEqual(77.0, setting.FilterMin.Red);
        }
    }
}
