using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    [TestClass]
    public class RangePageViewModelTests
    {
        private Model model;
        private Setting setting;
        private FakeImageService imageService;
        private FakeOcrService numericalOcr;
        private FakeOcrService textOcr;
        private RangePageViewModel vm;

        [TestInitialize]
        public void Setup()
        {
            model = new Model();
            setting = new Setting();
            imageService = new FakeImageService();
            numericalOcr = new FakeOcrService();
            textOcr = new FakeOcrService();
            vm = new RangePageViewModel(model, setting, imageService, numericalOcr, textOcr, null);
        }

        // ── AxisLimit round-trip ───────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimit_Set_SetsXMinXMaxYMinYMax()
        {
            vm.AxisLimit = new RectangleD(1, 2, 8, 6); // left=1,top=2,w=8,h=6 → right=9,bottom=8

            Assert.AreEqual(1.0, vm.XMin);
            Assert.AreEqual(9.0, vm.XMax);
            Assert.AreEqual(2.0, vm.YMin);
            Assert.AreEqual(8.0, vm.YMax);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimit_Get_ReturnsDerivedFromXYMinMax()
        {
            vm.XMin = 2; vm.XMax = 10; vm.YMin = 3; vm.YMax = 7;

            var limit = vm.AxisLimit;

            Assert.AreEqual(2.0, limit.Left);
            Assert.AreEqual(3.0, limit.Top);
            Assert.AreEqual(8.0, limit.Width);
            Assert.AreEqual(4.0, limit.Height);
        }

        // ── AxisLogBase round-trip ─────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLogBase_Set_SetsXLogYLog()
        {
            vm.AxisLogBase = new PointD(10, 100);

            Assert.AreEqual(10.0, vm.XLog);
            Assert.AreEqual(100.0, vm.YLog);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLogBase_Get_ReturnsDerivedFromXLogYLog()
        {
            vm.XLog = 2; vm.YLog = 5;

            var logBase = vm.AxisLogBase;

            Assert.AreEqual(2.0, logBase.X);
            Assert.AreEqual(5.0, logBase.Y);
        }

        // ── AxisTitle round-trip ───────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTitle_Set_SetsXLabelYLabel()
        {
            vm.AxisTitle = new AxisTitle("Voltage", "Current");

            Assert.AreEqual("Voltage", vm.XLabel);
            Assert.AreEqual("Current", vm.YLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTitle_Get_ReturnsDerivedFromXLabelYLabel()
        {
            vm.XLabel = "Time"; vm.YLabel = "Distance";

            var title = vm.AxisTitle;

            Assert.AreEqual("Time", title.XLabel);
            Assert.AreEqual("Distance", title.YLabel);
        }

        // ── Enter — disabled guard ─────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenDisabled_DoesNotCallImageService()
        {
            model.InputImage = null;

            vm.Enter();

            Assert.AreEqual(0, imageService.GetAxisTextBoxCallCount);
        }

        // ── Enter — detect branch (AxisTextBox == default) ─────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenAxisTextBoxIsDefault_CallsGetAxisTextBox()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = default;
            imageService.GetAxisTextBoxResult = new AxisTextBox();

            vm.Enter();

            Assert.AreEqual(1, imageService.GetAxisTextBoxCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenAxisTextBoxIsDefault_CallsOcr()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = default;
            imageService.GetAxisTextBoxResult = new AxisTextBox();
            numericalOcr.EnqueueResults("1.0", "2.0", "3.0", "4.0");
            textOcr.EnqueueResults("X", "Y");

            vm.Enter();

            Assert.IsTrue(numericalOcr.OcrCallCount > 0);
        }

        // ── Enter — restore branch (AxisTextBox != default) ────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenAxisTextBoxIsNotDefault_RestoresFromSetting()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            var stored = new AxisLimitTextBoxD
            {
                XMax = new RectangleD(1, 2, 3, 4),
                XMin = new RectangleD(5, 6, 7, 8),
                YMax = new RectangleD(9, 10, 11, 12),
                YMin = new RectangleD(13, 14, 15, 16),
                XLabel = new RectangleD(17, 18, 19, 20),
                YLabel = new RectangleD(21, 22, 23, 24),
            };
            setting.AxisTextBox = stored;
            setting.AxisLimit = new RectangleD(0, 0, 100, 200);
            setting.AxisTitle = new AxisTitle("X", "Y");
            setting.AxisLogBase = new PointD(10, 10);

            vm.Enter();

            Assert.AreEqual(stored.XMax, vm.XMaxTextBox);
            Assert.AreEqual(stored.XMin, vm.XMinTextBox);
            Assert.AreEqual(stored.YMax, vm.YMaxTextBox);
            Assert.AreEqual(stored.YMin, vm.YMinTextBox);
            Assert.AreEqual(stored.XLabel, vm.XLabelTextBox);
            Assert.AreEqual(stored.YLabel, vm.YLabelTextBox);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenAxisTextBoxIsNotDefault_RestoresAxisLimitFromSetting()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = new AxisLimitTextBoxD { XMax = new RectangleD(1, 1, 1, 1) };
            var storedLimit = new RectangleD(0, 0, 100, 200);
            setting.AxisLimit = storedLimit;
            setting.AxisLogBase = new PointD(10, 10);

            vm.Enter();

            Assert.AreEqual(storedLimit, vm.AxisLimit);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_AlwaysLoadsAxisLogBaseFromSetting()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            // AxisTextBox != default path
            setting.AxisTextBox = new AxisLimitTextBoxD { XMax = new RectangleD(1, 1, 1, 1) };
            setting.AxisLogBase = new PointD(3, 7);

            vm.Enter();

            Assert.AreEqual(new PointD(3, 7), vm.AxisLogBase);
        }

        // ── Leave ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenEnabled_WritesAxisTextBoxToSetting()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            vm.XMaxTextBox = new RectangleD(1, 2, 3, 4);

            vm.Leave();

            Assert.AreEqual(new RectangleD(1, 2, 3, 4), setting.AxisTextBox.XMax);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenEnabled_WritesAxisTitleToSetting()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            vm.XLabel = "FreqX"; vm.YLabel = "AmpY";

            vm.Leave();

            Assert.AreEqual("FreqX", setting.AxisTitle.XLabel);
            Assert.AreEqual("AmpY", setting.AxisTitle.YLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenEnabled_WritesAxisLogBaseToSetting()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            vm.XLog = 10; vm.YLog = 2;

            vm.Leave();

            Assert.AreEqual(new PointD(10, 2), setting.AxisLogBase);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenDisabled_DoesNotWriteToSetting()
        {
            model.InputImage = null;
            setting.AxisLogBase = new PointD(5, 5);

            vm.Leave();

            Assert.AreEqual(new PointD(5, 5), setting.AxisLogBase);
        }

        // ── Ocr ───────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Ocr_ParsesNumericValuesIntoXMaxXMinYMaxYMin()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = default;
            imageService.GetAxisTextBoxResult = new AxisTextBox();
            numericalOcr.EnqueueResults("100.0", "0.0", "50.0", "10.0");
            textOcr.EnqueueResults("Time", "Force");

            vm.Enter(); // triggers Ocr

            Assert.AreEqual(100.0, vm.XMax);
            Assert.AreEqual(0.0, vm.XMin);
            Assert.AreEqual(50.0, vm.YMax);
            Assert.AreEqual(10.0, vm.YMin);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Ocr_TrimsLabelWhitespace()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = default;
            imageService.GetAxisTextBoxResult = new AxisTextBox();
            numericalOcr.EnqueueResults("1", "2", "3", "4");
            textOcr.EnqueueResults("  Time  ", "  Force  ");

            vm.Enter();

            Assert.AreEqual("Time", vm.XLabel);
            Assert.AreEqual("Force", vm.YLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Ocr_WhenNumericalParsingFails_LeavesValueUnchanged()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = default;
            imageService.GetAxisTextBoxResult = new AxisTextBox();
            // pre-set XMax
            vm.XMax = 99.0;
            numericalOcr.EnqueueResults("NOT_A_NUMBER", "2", "3", "4");
            textOcr.EnqueueResults("X", "Y");

            vm.Enter();

            Assert.AreEqual(99.0, vm.XMax);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Ocr_RotatesYLabelCropBy90Degrees()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = default;
            imageService.GetAxisTextBoxResult = new AxisTextBox();
            numericalOcr.EnqueueResults("1", "2", "3", "4");
            textOcr.EnqueueResults("X", "Y");

            vm.Enter();

            Assert.AreEqual(90.0, imageService.LastRotateAngle);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Ocr_WhenOcrServiceThrows_SwallowsException()
        {
            using var img = new Image<Rgba, byte>(8, 8);
            model.InputImage = img;
            setting.AxisTextBox = default;
            imageService.GetAxisTextBoxResult = new AxisTextBox();
            numericalOcr.ThrowOnOcr = new InvalidOperationException("OCR failure");

            // Should not throw
            vm.Enter();
        }
    }
}
