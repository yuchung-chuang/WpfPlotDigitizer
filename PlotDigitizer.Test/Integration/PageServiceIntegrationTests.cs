using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.Integration
{
	/// <summary>
	/// Drives the real <see cref="PageService"/> through <see cref="AddViewModels"/> +
	/// <see cref="ServiceExtensions.AddModel"/>, with a real <see cref="EmguCvService"/> and
	/// hand-rolled fakes only at the true boundaries (dialogs, clipboard, download, OCR). Covers
	/// navigation order, the <c>CanExecute</c> boundaries, real <c>Leave()</c>/<c>Enter()</c> side
	/// effects, and - the highest-value scenario - that changing the filter discards the Edit
	/// page's undo history by disposing its DI scope.
	/// </summary>
	[TestClass]
	public class PageServiceIntegrationTests
	{
		private static readonly RectangleD ValidAxisLocation = new(10, 10, 100, 80);

		private ServiceProvider provider;
		private Model model;
		private Setting setting;
		private IPageService pageService;

		[TestInitialize]
		public void OnTestInitialize()
		{
			provider = new ServiceCollection()
				.AddLogging()
				.AddTransient<IImageService, EmguCvService>()
				.AddSingleton<IFileDialogService>(new FakeFileDialogService())
				.AddSingleton<IClipboardService>(new FakeClipboardService())
				.AddSingleton<IMessageBoxService>(new FakeMessageBoxService())
				.AddSingleton<IAwaitTaskService>(new FakeAwaitTaskService())
				.AddSingleton<IDownloadService>(new FakeDownloadService())
				.AddKeyedSingleton<IOcrService>("Numerical", new FakeOcrService())
				.AddKeyedSingleton<IOcrService>("Text", new FakeOcrService())
				.AddSingleton<IPageService, PageService>()
				.AddViewModels()
				.AddModel()
				.BuildServiceProvider();

			model = provider.GetRequiredService<Model>();
			setting = provider.GetRequiredService<Setting>();
			pageService = provider.GetRequiredService<IPageService>();
		}

		[TestCleanup]
		public void OnTestCleanup() => provider?.Dispose();

		/// <summary>
		/// Loads a real image and a known-good crop, then forces one read of every stage so
		/// CroppedImage/FilteredImage/EditedImage/DataPoints/Data are all already computed. This
		/// mirrors the real app: by the time a user reaches the Edit page, Axis/Range/Filter's own
		/// <c>Enter()</c> methods have already read Model.CroppedImage at least once. Skipping this
		/// warm-up matters because DependsOn reacts to a node's very first `Updated` the same way
		/// it reacts to a real invalidation, so reading a stage for the first time cascades a
		/// (harmless, one-off) Outdated through every downstream stage that has never been read
		/// either - see <see cref="NavigatingStraightToTheNeverEnteredEditPage_CanSelfDisposeItsOwnBrandNewScope"/>.
		/// </summary>
		private void LoadValidImage()
		{
			model.InputImage = new Image<Rgba, byte>("Assets/data.png");
			setting.AxisLocation = ValidAxisLocation;
			_ = model.Data;
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void Initialise_StartsOnLoadPageAndRaisesNavigatedWithIndexZero()
		{
			int? raisedIndex = null;
			pageService.Navigated += (s, index) => raisedIndex = index;

			pageService.Initialise();

			Assert.IsInstanceOfType(pageService.CurrentPage, typeof(LoadPageViewModel));
			Assert.AreEqual(0, pageService.CurrentPageIndex);
			Assert.AreEqual(0, raisedIndex);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void NextPage_WalksEveryPageInDeclaredOrder_AndCanExecuteBecomesFalseOnlyAtTheLastPage()
		{
			pageService.Initialise();

			// Right after Initialise, the page order starts at the first page: PrevPage must be blocked.
			Assert.IsFalse(pageService.PrevPageCommand.CanExecute(null), "PrevPage must be disabled on the first page.");

			for (var i = 0; i < PageService.Pages.Count - 1; i++) {
				Assert.IsTrue(pageService.NextPageCommand.CanExecute(null), $"NextPage should be enabled while leaving page {i}.");
				pageService.NextPageCommand.Execute(null);

				Assert.AreEqual(i + 1, pageService.CurrentPageIndex);
				Assert.IsInstanceOfType(pageService.CurrentPage, PageService.Pages[i + 1]);
				Assert.IsTrue(pageService.PrevPageCommand.CanExecute(null), "PrevPage must be enabled once we've left the first page.");
			}

			Assert.IsFalse(pageService.NextPageCommand.CanExecute(null), "NextPage must be disabled on the last page.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void NavigateTo_CallsLeaveOnOutgoingPage_AndEnterOnIncomingPage_ProvenByRealSideEffects()
		{
			LoadValidImage();
			pageService.Initialise();

			pageService.NavigateCommand.Execute(typeof(AxisPageViewModel));
			var axisPage = (AxisPageViewModel)pageService.CurrentPage;

			// AxisLocation was already non-default, so Enter() loaded it from Setting instead of
			// running OCR auto-detection - proving Enter() ran the real logic, not a no-op.
			Assert.AreEqual(ValidAxisLocation, axisPage.AxisLocation);

			var customLocation = new RectangleD(5, 5, 50, 40);
			axisPage.AxisLocation = customLocation;

			// Navigating onward calls Leave() on the Axis page, which must persist the edited value.
			pageService.NextPageCommand.Execute(null);

			Assert.IsInstanceOfType(pageService.CurrentPage, typeof(RangePageViewModel));
			Assert.AreEqual(customLocation, setting.AxisLocation, "AxisPageViewModel.Leave() must persist the current AxisLocation to Setting.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void FilteredImageOutdated_DisposesEditScopeAndDiscardsUndoHistory()
		{
			LoadValidImage();
			pageService.Initialise();

			pageService.NavigateCommand.Execute(typeof(EditPageViewModel));
			var editPage1 = (EditPageViewModel)pageService.CurrentPage;
			Assert.IsTrue(editPage1.IsEnabled, "The Edit page must be enabled once a valid FilteredImage exists.");
			Assert.IsTrue(editPage1.EditService.IsInitialised);

			using (var edited = editPage1.Model.FilteredImage.Copy()) {
				editPage1.EditService.Edit((edited, "test-edit"));
			}
			Assert.AreEqual(1, editPage1.EditService.Index, "The real edit must be recorded in undo history.");

			// Changing the filter outdates Model.FilteredImage, which PageService reacts to by
			// disposing the Edit page's scope - this is the behaviour users notice as "my edits
			// disappeared after I changed the filter".
			setting.FilterMin = new Rgba(30, 30, 30, byte.MaxValue);

			pageService.NavigateCommand.Execute(typeof(EditPageViewModel));
			var editPage2 = (EditPageViewModel)pageService.CurrentPage;

			Assert.AreNotSame(editPage1, editPage2, "A new scope must produce a new EditPageViewModel instance.");
			Assert.AreNotSame(editPage1.EditService, editPage2.EditService, "The new scope must produce a new IEditService<T> instance.");
			Assert.AreEqual(0, editPage2.EditService.Index, "Undo history must be discarded when the filter changes.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void LeavingAndReturningToTheEditPage_WithoutInvalidatingTheFilter_PreservesUndoHistory()
		{
			LoadValidImage();
			pageService.Initialise();

			pageService.NavigateCommand.Execute(typeof(EditPageViewModel));
			var editPage1 = (EditPageViewModel)pageService.CurrentPage;

			using (var edited = editPage1.Model.FilteredImage.Copy()) {
				editPage1.EditService.Edit((edited, "test-edit"));
			}
			Assert.AreEqual(1, editPage1.EditService.Index);

			// Navigate away to a different page and back, without touching the filter.
			pageService.NavigateCommand.Execute(typeof(DataPageViewModel));
			pageService.NavigateCommand.Execute(typeof(EditPageViewModel));
			var editPage2 = (EditPageViewModel)pageService.CurrentPage;

			Assert.AreSame(editPage1, editPage2, "Without a FilteredImage invalidation, the same Edit scope - and therefore the same view model - must be reused.");
			Assert.AreEqual(1, editPage2.EditService.Index, "Undo history must survive navigating away and back when the filter never changed.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void NavigatingStraightToTheNeverEnteredEditPage_CanSelfDisposeItsOwnBrandNewScope()
		{
			// This documents a real, subtle interaction rather than a defect this change should
			// fix: DependsOn subscribes to a node's Updated *and* Outdated events, so the very
			// first time an upstream node computes, every downstream node that has never been
			// read yet is cascaded through Outdated too - even though it was never valid to begin
			// with. PageService.Model_PropertyOutdated treats any "FilteredImage" outdated event
			// as "the filter changed, discard undo history", with no way to distinguish a first
			// computation from a real invalidation. In the shipped app this is invisible, because
			// Axis/Range/Filter's own Enter() methods read Model.CroppedImage well before the user
			// ever reaches the Edit page, so the cascade is already spent by then (see
			// LoadValidImage's warm-up). It only surfaces when the Edit page is entered before
			// anything upstream has ever been read - exactly what this test does deliberately.
			model.InputImage = new Image<Rgba, byte>("Assets/data.png");
			setting.AxisLocation = ValidAxisLocation;
			// Deliberately no warm-up read here - CroppedImage/FilteredImage have never computed.

			pageService.Initialise();
			pageService.NavigateCommand.Execute(typeof(EditPageViewModel));
			var firstEntry = (EditPageViewModel)pageService.CurrentPage;

			// The view model itself still works fine (Dispose() only unsubscribes an event), but
			// navigating to the Edit page a second time - with nothing about the filter having
			// changed in between - produces a different instance, because the first entry already
			// disposed and nulled out the scope it was resolved from.
			pageService.NavigateCommand.Execute(typeof(DataPageViewModel));
			pageService.NavigateCommand.Execute(typeof(EditPageViewModel));
			var secondEntry = (EditPageViewModel)pageService.CurrentPage;

			Assert.AreNotSame(firstEntry, secondEntry,
				"Entering the Edit page before Model.FilteredImage has ever been computed self-disposes its own scope.");
		}
	}
}
