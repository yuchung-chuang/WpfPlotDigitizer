using System;
using System.Collections.Specialized;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    [TestClass]
    public class LoadPageViewModelTests
    {
        private const string AssetPath = "Assets/data.png";
        private const string MissingPath = "Assets/does-not-exist.png";

        private Model model;
        private FakeFileDialogService fileDialog;
        private FakeAwaitTaskService awaitTask;
        private FakeClipboardService clipboard;
        private FakeMessageBoxService messageBox;
        private FakePageService pageService;
        private FakeDownloadService downloadService;
        private LoadPageViewModel vm;

        [TestInitialize]
        public void Setup()
        {
            model = new Model();
            fileDialog = new FakeFileDialogService();
            awaitTask = new FakeAwaitTaskService();
            clipboard = new FakeClipboardService();
            messageBox = new FakeMessageBoxService();
            pageService = new FakePageService();
            downloadService = new FakeDownloadService();
            vm = new LoadPageViewModel(model, fileDialog, awaitTask, clipboard, messageBox, null, pageService, downloadService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            model.InputImage?.Dispose();
            downloadService.Result?.Dispose();
            clipboard.ImageResult?.Dispose();
        }

        // ── Construction ───────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Constructor_Parameterless_WiresNameAndCommands()
        {
            var bare = new LoadPageViewModel();

            Assert.AreEqual("Load Page", bare.Name);
            Assert.IsNotNull(bare.BrowseCommand);
            Assert.IsNotNull(bare.PasteCommand);
            Assert.IsNotNull(bare.DropCommand);
        }

        // ── Browse ─────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Browse_WhenDialogIsCancelled_LeavesModelUntouched()
        {
            fileDialog.OpenResult = new FileDialogResults(null, false);

            vm.BrowseCommand.Execute(null);

            Assert.AreEqual(1, fileDialog.OpenCallCount);
            Assert.IsNull(model.InputImage);
            Assert.AreEqual(0, pageService.NextPageExecutedCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Browse_WhenFileIsSelected_SetsImageAndNavigatesToNextPage()
        {
            fileDialog.OpenResult = new FileDialogResults(AssetPath, true);

            vm.BrowseCommand.Execute(null);

            Assert.IsNotNull(model.InputImage);
            Assert.AreEqual(1, pageService.NextPageExecutedCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Browse_Always_RequestsTheImageFileFilter()
        {
            vm.BrowseCommand.Execute(null);

            var filter = fileDialog.RequestedFilters[0];
            StringAssert.StartsWith(filter, "Images (*.jpg;*.jpeg;*.png;*.bmp;*.tif)");
            StringAssert.Contains(filter, "Any |*.*");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Browse_WhenNextPageCannotExecute_SetsImageWithoutNavigating()
        {
            pageService.CanNextPage = false;
            fileDialog.OpenResult = new FileDialogResults(AssetPath, true);

            vm.BrowseCommand.Execute(null);

            Assert.IsNotNull(model.InputImage);
            Assert.AreEqual(0, pageService.NextPageExecutedCount);
        }

        // ── Drop ───────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Drop_WithFile_SetsImageFromFileName()
        {
            var args = new DropEventArgs { Type = DropEventArgs.DropType.File, FileName = AssetPath };

            vm.DropCommand.Execute(args);

            Assert.IsNotNull(model.InputImage);
            Assert.AreEqual(1, pageService.NextPageExecutedCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Drop_WithUrl_DownloadsThroughAwaitTaskServiceAndSetsImage()
        {
            var url = new Uri("https://example.com/plot.png");
            downloadService.Result = new Image<Rgba, byte>(4, 4);
            var args = new DropEventArgs { Type = DropEventArgs.DropType.Url, Url = url };

            vm.DropCommand.Execute(args);

            Assert.AreEqual(1, awaitTask.RunAsyncCallCount);
            CollectionAssert.AreEqual(new[] { url }, downloadService.RequestedUrls);
            Assert.AreSame(downloadService.Result, model.InputImage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Drop_WithUrl_WhenDownloadReturnsNull_LeavesModelUntouched()
        {
            downloadService.Result = null;
            var args = new DropEventArgs { Type = DropEventArgs.DropType.Url, Url = new Uri("https://example.com/plot.png") };

            vm.DropCommand.Execute(args);

            Assert.IsNull(model.InputImage);
            Assert.AreEqual(0, pageService.NextPageExecutedCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Drop_WithImage_ConvertsBitmapAndSetsImage()
        {
            using var bitmap = new Bitmap(6, 3);
            var args = new DropEventArgs { Type = DropEventArgs.DropType.Image, Image = bitmap };

            vm.DropCommand.Execute(args);

            Assert.IsNotNull(model.InputImage);
            Assert.AreEqual(new Size(6, 3), model.InputImage.Size);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Drop_WithNonDropEventArgsParameter_DoesNothing()
        {
            vm.DropCommand.Execute("not a drop");

            Assert.IsNull(model.InputImage);
            Assert.AreEqual(0, pageService.NextPageExecutedCount);
        }

        // ── Paste ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Paste_WhenClipboardHasImage_SetsImage()
        {
            clipboard.HasImage = true;
            clipboard.ImageResult = new Image<Rgba, byte>(4, 4);

            vm.PasteCommand.Execute(null);

            Assert.AreSame(clipboard.ImageResult, model.InputImage);
            Assert.AreEqual(1, pageService.NextPageExecutedCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Paste_WhenClipboardImageIsNull_LeavesModelUntouchedWithoutWarning()
        {
            clipboard.HasImage = true;
            clipboard.ImageResult = null;

            vm.PasteCommand.Execute(null);

            Assert.IsNull(model.InputImage);
            Assert.AreEqual(0, messageBox.OkMessages.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Paste_WhenClipboardHasFileDropList_LoadsTheFirstEntry()
        {
            clipboard.HasFileDropList = true;
            clipboard.FileDropListResult = new StringCollection { AssetPath, MissingPath };

            vm.PasteCommand.Execute(null);

            Assert.IsNotNull(model.InputImage);
            Assert.AreEqual(1, pageService.NextPageExecutedCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Paste_WhenClipboardHasUrlText_DownloadsAndSetsImage()
        {
            clipboard.HasText = true;
            clipboard.TextResult = "https://example.com/plot.png";
            downloadService.Result = new Image<Rgba, byte>(4, 4);

            vm.PasteCommand.Execute(null);

            Assert.AreEqual(1, awaitTask.RunAsyncCallCount);
            CollectionAssert.AreEqual(new[] { new Uri("https://example.com/plot.png") }, downloadService.RequestedUrls);
            Assert.AreSame(downloadService.Result, model.InputImage);
            Assert.AreEqual(0, messageBox.OkMessages.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Paste_WhenClipboardTextIsNotAUrl_WarnsTheUser()
        {
            clipboard.HasText = true;
            clipboard.TextResult = "just some text";

            vm.PasteCommand.Execute(null);

            Assert.IsNull(model.InputImage);
            Assert.AreEqual(0, downloadService.RequestedUrls.Count);
            CollectionAssert.Contains(messageBox.OkMessages, ("Clipboard does not contain image.", "Warning"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Paste_WhenClipboardIsEmpty_WarnsTheUser()
        {
            vm.PasteCommand.Execute(null);

            Assert.IsNull(model.InputImage);
            CollectionAssert.Contains(messageBox.OkMessages, ("Clipboard does not contain image.", "Warning"));
        }

        // ── FilePath ───────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilePath_WhenFileDoesNotExist_LeavesModelUntouched()
        {
            vm.FilePath = MissingPath;

            Assert.IsNull(model.InputImage);
            Assert.AreEqual(0, pageService.NextPageExecutedCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilePath_WhenFileExists_SetsImageAndNavigatesToNextPage()
        {
            vm.FilePath = AssetPath;

            Assert.IsNotNull(model.InputImage);
            Assert.AreEqual(1, pageService.NextPageExecutedCount);
        }
    }
}
