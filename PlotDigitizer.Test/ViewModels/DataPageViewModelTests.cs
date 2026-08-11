using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    [TestClass]
    public class DataPageViewModelTests
    {
        private Model model;
        private Setting setting;
        private FakeMessageBoxService messageBox;
        private FakeFileDialogService fileDialog;
        private FakeAwaitTaskService awaitTask;
        private FakeImageService imageService;
        private DataPageViewModel vm;
        private string tempDir;

        [TestInitialize]
        public void Setup()
        {
            model = new Model();
            setting = new Setting();
            messageBox = new FakeMessageBoxService();
            fileDialog = new FakeFileDialogService();
            awaitTask = new FakeAwaitTaskService();
            imageService = new FakeImageService();
            vm = new DataPageViewModel(model, setting, messageBox, fileDialog, awaitTask, imageService, null);
            tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }

        // ── IsContinuous / IsDiscrete ──────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsContinuous_WhenDataTypeIsContinuous_ReturnsTrue()
        {
            setting.DataType = DataType.Continuous;
            Assert.IsTrue(vm.IsContinuous);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsContinuous_WhenDataTypeIsDiscrete_ReturnsFalse()
        {
            setting.DataType = DataType.Discrete;
            Assert.IsFalse(vm.IsContinuous);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsContinuous_SetTrue_SetsDataTypeToContinuous()
        {
            setting.DataType = DataType.Discrete;
            vm.IsContinuous = true;
            Assert.AreEqual(DataType.Continuous, setting.DataType);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsContinuous_SetFalse_SetsDataTypeToDiscrete()
        {
            setting.DataType = DataType.Continuous;
            vm.IsContinuous = false;
            Assert.AreEqual(DataType.Discrete, setting.DataType);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsDiscrete_WhenDataTypeIsDiscrete_ReturnsTrue()
        {
            setting.DataType = DataType.Discrete;
            Assert.IsTrue(vm.IsDiscrete);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsDiscrete_WhenDataTypeIsContinuous_ReturnsFalse()
        {
            setting.DataType = DataType.Continuous;
            Assert.IsFalse(vm.IsDiscrete);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsDiscrete_SetTrue_SetsDataTypeToDiscrete()
        {
            setting.DataType = DataType.Continuous;
            vm.IsDiscrete = true;
            Assert.AreEqual(DataType.Discrete, setting.DataType);
        }

        // ── IsEnabled ──────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenModelIsNull_ReturnsFalse()
        {
            var noModel = new DataPageViewModel(null, setting, messageBox, fileDialog, awaitTask, imageService, null);
            Assert.IsFalse(noModel.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenEdittedImageIsNull_ReturnsFalse()
        {
            model.EdittedImage = null;
            Assert.IsFalse(vm.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenEdittedImageIsSet_ReturnsTrue()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.EdittedImage = img;
            Assert.IsTrue(vm.IsEnabled);
        }

        // ── Enter ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenEnabled_SetsImageToACopyOfEdittedImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.EdittedImage = img;
            model.Data = Enumerable.Empty<PointD>();

            vm.Enter();

            Assert.IsNotNull(vm.Image);
            Assert.AreNotSame(img, vm.Image); // must be a copy
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenDisabled_DoesNotSetImage()
        {
            model.EdittedImage = null;

            vm.Enter();

            Assert.IsNull(vm.Image);
        }

        // ── CanExport ──────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void CanExport_WhenDataIsNull_ReturnsFalse()
        {
            model.Data = null;
            Assert.IsFalse(vm.ExportCommand.CanExecute());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanExport_WhenDataIsEmpty_ReturnsFalse()
        {
            model.Data = Enumerable.Empty<PointD>();
            Assert.IsFalse(vm.ExportCommand.CanExecute());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanExport_WhenDataHasItems_ReturnsTrue()
        {
            model.Data = new[] { new PointD(1, 2) };
            Assert.IsTrue(vm.ExportCommand.CanExecute());
        }

        // ── Export — dialog cancelled ──────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Export_WhenDialogCancelled_WritesNoFile()
        {
            model.Data = new[] { new PointD(1, 2) };
            fileDialog.SaveResult = new FileDialogResults(null, false);

            vm.ExportCommand.Execute();

            Assert.AreEqual(0, Directory.GetFiles(tempDir).Length);
            Assert.AreEqual(0, messageBox.OkMessages.Count);
        }

        // ── Export — CSV ───────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Export_ToCsvFile_WritesHeaderAndDataWithCommasSeparator()
        {
            var csvPath = Path.Combine(tempDir, "out.csv");
            fileDialog.SaveResult = new FileDialogResults(csvPath, true);
            setting.AxisTitle = new AxisTitle("Time", "Force");
            model.Data = new[] { new PointD(1.0, 2.0), new PointD(3.0, 4.0) };

            vm.ExportCommand.Execute();

            var lines = File.ReadAllLines(csvPath);
            Assert.AreEqual("Time,Force", lines[0].TrimEnd('\r'));
            Assert.AreEqual("1,2", lines[1].TrimEnd('\r'));
            Assert.AreEqual("3,4", lines[2].TrimEnd('\r'));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Export_ToCsvFile_WhenAxisTitleIsEmpty_UsesXYFallback()
        {
            var csvPath = Path.Combine(tempDir, "out.csv");
            fileDialog.SaveResult = new FileDialogResults(csvPath, true);
            setting.AxisTitle = new AxisTitle("", "");
            model.Data = new[] { new PointD(1.0, 2.0) };

            vm.ExportCommand.Execute();

            var firstLine = File.ReadAllLines(csvPath)[0].TrimEnd('\r');
            Assert.AreEqual("X,Y", firstLine);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Export_ToCsvFile_ShowsSuccessMessage()
        {
            var csvPath = Path.Combine(tempDir, "out.csv");
            fileDialog.SaveResult = new FileDialogResults(csvPath, true);
            setting.AxisTitle = new AxisTitle("X", "Y");
            model.Data = new[] { new PointD(1.0, 2.0) };

            vm.ExportCommand.Execute();

            Assert.AreEqual(1, messageBox.OkMessages.Count);
        }

        // ── Export — TXT ───────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Export_ToTxtFile_WritesHeaderWithTabSeparator()
        {
            var txtPath = Path.Combine(tempDir, "out.txt");
            fileDialog.SaveResult = new FileDialogResults(txtPath, true);
            setting.AxisTitle = new AxisTitle("X", "Y");
            model.Data = new[] { new PointD(5.0, 6.0) };

            vm.ExportCommand.Execute();

            var firstLine = File.ReadAllLines(txtPath)[0].TrimEnd('\r');
            Assert.AreEqual("X\tY", firstLine);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Export_ToTxtFile_WritesDataWithTabSeparator()
        {
            var txtPath = Path.Combine(tempDir, "out.txt");
            fileDialog.SaveResult = new FileDialogResults(txtPath, true);
            setting.AxisTitle = new AxisTitle("X", "Y");
            model.Data = new[] { new PointD(5.0, 6.0) };

            vm.ExportCommand.Execute();

            var lines = File.ReadAllLines(txtPath);
            Assert.AreEqual("5\t6", lines[1].TrimEnd('\r'));
        }

        // ── Export — unrecognised extension ────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Export_WithUnrecognisedExtension_ShowsWarningDialog()
        {
            var badPath = Path.Combine(tempDir, "out.xyz");
            fileDialog.SaveResult = new FileDialogResults(badPath, true);
            model.Data = new[] { new PointD(1, 2) };
            messageBox.WarningOkCancelResult = false; // don't retry

            vm.ExportCommand.Execute();

            Assert.AreEqual(1, messageBox.WarningOkCancelMessages.Count);
        }
    }
}
