using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    [TestClass]
    public class AxisPageViewModelTests
    {
        private Model model;
        private Setting setting;
        private FakeImageService imageService;
        private AxisPageViewModel vm;

        [TestInitialize]
        public void Setup()
        {
            model = new Model();
            setting = new Setting();
            imageService = new FakeImageService();
            vm = new AxisPageViewModel(model, setting, imageService, null);
        }

        // ── IsEnabled ──────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenModelIsNull_ReturnsFalse()
        {
            var noModel = new AxisPageViewModel(null, setting, imageService, null);
            Assert.IsFalse(noModel.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenInputImageIsNull_ReturnsFalse()
        {
            model.InputImage = null;
            Assert.IsFalse(vm.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenModelHasInputImage_ReturnsTrue()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.InputImage = img;
            Assert.IsTrue(vm.IsEnabled);
        }

        // ── Image property ─────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Image_WhenNotEnabled_ReturnsNull()
        {
            model.InputImage = null;
            Assert.IsNull(vm.Image);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Image_WhenEnabled_ReturnsModelInputImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.InputImage = img;
            Assert.AreSame(img, vm.Image);
        }

        // ── AxisRelative ───────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisRelative_WhenImageIsNull_ReturnsDefaultRectangleD()
        {
            model.InputImage = null;
            var rel = vm.AxisRelative;
            Assert.AreEqual(new RectangleD(), rel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisRelative_WhenImageSet_ReturnsNormalisedRectangle()
        {
            using var img = new Image<Rgba, byte>(100, 200);
            model.InputImage = img;
            vm.AxisLocation = new RectangleD(10, 20, 50, 100);

            var rel = vm.AxisRelative;

            Assert.AreEqual(0.1, rel.Left, 1e-9);
            Assert.AreEqual(0.1, rel.Top, 1e-9);
            Assert.AreEqual(0.5, rel.Width, 1e-9);
            Assert.AreEqual(0.5, rel.Height, 1e-9);
        }

        // ── GetAxisCommand.CanExecute ──────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetAxisCommand_CanExecute_WhenDisabled_ReturnsFalse()
        {
            model.InputImage = null;
            Assert.IsFalse(vm.GetAxisCommand.CanExecute());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetAxisCommand_CanExecute_WhenEnabled_ReturnsTrue()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.InputImage = img;
            Assert.IsTrue(vm.GetAxisCommand.CanExecute());
        }

        // ── Enter — auto-detect branch ─────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenSettingAxisLocationIsDefault_CallsGetAxisLocation()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisLocation = default;
            imageService.GetAxisLocationResult = new RectangleD(1, 1, 2, 2);

            vm.Enter();

            Assert.AreEqual(1, imageService.GetAxisLocationCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenSettingAxisLocationIsDefault_SetsAxisLocationFromService()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisLocation = default;
            var expected = new RectangleD(1, 2, 3, 4);
            imageService.GetAxisLocationResult = expected;

            vm.Enter();

            Assert.AreEqual(expected, vm.AxisLocation);
        }

        // ── Enter — load-stored branch ─────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenSettingAxisLocationIsNotDefault_LoadsStoredAxisLocation()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            var stored = new RectangleD(5, 6, 7, 8);
            setting.AxisLocation = stored;

            vm.Enter();

            Assert.AreEqual(stored, vm.AxisLocation);
            Assert.AreEqual(0, imageService.GetAxisLocationCallCount);
        }

        // ── Enter — disabled guard ─────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenDisabled_DoesNotCallGetAxisLocation()
        {
            model.InputImage = null;
            vm.Enter();
            Assert.AreEqual(0, imageService.GetAxisLocationCallCount);
        }

        // ── Leave ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenEnabled_WritesAxisLocationToSetting()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            var loc = new RectangleD(1, 2, 3, 4);
            vm.AxisLocation = loc;

            vm.Leave();

            Assert.AreEqual(loc, setting.AxisLocation);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenDisabled_DoesNotWriteToSetting()
        {
            model.InputImage = null;
            setting.AxisLocation = new RectangleD(9, 9, 9, 9);

            vm.Leave();

            // Setting should remain unchanged
            Assert.AreEqual(new RectangleD(9, 9, 9, 9), setting.AxisLocation);
        }

        // ── GetAxis fallback ───────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetAxis_WhenGetAxisLocationThrows_FallsBackToCentredQuarterRectangle()
        {
            using var img = new Image<Rgba, byte>(100, 80);
            model.InputImage = img;
            imageService.GetAxisLocationBehaviour = _ => throw new Exception("OCR failed");

            vm.GetAxisCommand.Execute();

            var expected = new RectangleD(25, 20, 50, 40);
            Assert.AreEqual(expected, vm.AxisLocation);
        }
    }
}
