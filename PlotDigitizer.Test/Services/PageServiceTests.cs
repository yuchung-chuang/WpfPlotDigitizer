using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace PlotDigitizer.Core.Tests
{
    // -----------------------------------------------------------------------
    // Stubs used only by PageServiceTests
    // -----------------------------------------------------------------------

    /// <summary>
    /// Tracks Enter/Leave calls and exposes a counter so tests can verify calls.
    /// </summary>
    internal sealed class StubPage : ViewModelBase
    {
        public int EnterCount { get; private set; }
        public int LeaveCount { get; private set; }

        public override void Enter() => EnterCount++;
        public override void Leave() => LeaveCount++;
    }

    /// <summary>
    /// An IServiceProvider that returns instances out of a dictionary keyed by Type.
    /// </summary>
    internal sealed class PageServiceTests_StubServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _map = new();

        public void Register(Type type, object instance) => _map[type] = instance;

        public object GetService(Type serviceType)
            => _map.TryGetValue(serviceType, out var val) ? val : null;
    }

    /// <summary>
    /// A minimal IServiceScope / IServiceScopeFactory that wraps a separate IServiceProvider.
    /// </summary>
    internal sealed class PageServiceTests_StubScope : IServiceScope
    {
        public PageServiceTests_StubScope(IServiceProvider sp) => ServiceProvider = sp;
        public IServiceProvider ServiceProvider { get; }
        public bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
    }

    internal sealed class PageServiceTests_StubScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProvider _sp;
        public PageServiceTests_StubScopeFactory(IServiceProvider sp) => _sp = sp;
        public IServiceScope LastScope { get; private set; }
        public int CreateCount { get; private set; }

        public IServiceScope CreateScope()
        {
            CreateCount++;
            LastScope = new PageServiceTests_StubScope(_sp);
            return LastScope;
        }
    }

    /// <summary>
    /// Exposes OnPropertyOutdated so tests can fire it without needing UpdatableModel.
    /// </summary>
    internal sealed class TestableModel : Model
    {
        public void FirePropertyOutdated(string propertyName) => OnPropertyOutdated(propertyName);
    }

    // -----------------------------------------------------------------------
    // Test class
    // -----------------------------------------------------------------------

    [TestClass]
    public class PageServiceTests
    {
        private TestableModel _model;
        private PageServiceTests_StubServiceProvider _globalSp;
        private PageServiceTests_StubServiceProvider _scopedSp;
        private PageServiceTests_StubScopeFactory _scopeFactory;

        private StubPage _loadPage;
        private StubPage _axisPage;
        private StubPage _rangePage;
        private StubPage _filterPage;
        private StubPage _editPage;
        private StubPage _dataPage;

        [TestInitialize]
        public void Init()
        {
            _model = new TestableModel();

            _loadPage   = new StubPage();
            _axisPage   = new StubPage();
            _rangePage  = new StubPage();
            _filterPage = new StubPage();
            _editPage   = new StubPage();
            _dataPage   = new StubPage();

            _globalSp = new PageServiceTests_StubServiceProvider();
            _globalSp.Register(typeof(LoadPageViewModel),   _loadPage);
            _globalSp.Register(typeof(AxisPageViewModel),   _axisPage);
            _globalSp.Register(typeof(RangePageViewModel),  _rangePage);
            _globalSp.Register(typeof(FilterPageViewModel), _filterPage);
            _globalSp.Register(typeof(DataPageViewModel),   _dataPage);

            _scopedSp = new PageServiceTests_StubServiceProvider();
            _scopedSp.Register(typeof(EditPageViewModel), _editPage);

            _scopeFactory = new PageServiceTests_StubScopeFactory(_scopedSp);
        }

        private PageService CreateSut() =>
            new PageService(_model, _globalSp, _scopeFactory);

        /// <summary>
        /// Builds a PageService whose providers contain real view model instances so that
        /// <see cref="PageService.CurrentPageIndex"/> (which matches on the exact runtime type)
        /// returns the correct index.  Enter/Leave are no-ops when Model is null.
        /// </summary>
        private PageService CreateSutWithRealVms()
        {
            var realGlobalSp = new PageServiceTests_StubServiceProvider();
            realGlobalSp.Register(typeof(LoadPageViewModel),   new LoadPageViewModel());
            realGlobalSp.Register(typeof(AxisPageViewModel),   new AxisPageViewModel());
            realGlobalSp.Register(typeof(RangePageViewModel),  new RangePageViewModel());
            realGlobalSp.Register(typeof(FilterPageViewModel), new FilterPageViewModel());
            realGlobalSp.Register(typeof(DataPageViewModel),   new DataPageViewModel());

            var realScopedSp = new PageServiceTests_StubServiceProvider();
            realScopedSp.Register(typeof(EditPageViewModel), new EditPageViewModel());

            var realScopeFactory = new PageServiceTests_StubScopeFactory(realScopedSp);
            return new PageService(_model, realGlobalSp, realScopeFactory);
        }

        // ---------------------------------------------------------------
        // Pages order
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Pages_ContainsSixEntries()
        {
            Assert.AreEqual(6, PageService.Pages.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Pages_FirstPageIsLoadPageViewModel()
        {
            Assert.AreEqual(typeof(LoadPageViewModel), PageService.Pages[0]);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Pages_LastPageIsDataPageViewModel()
        {
            Assert.AreEqual(typeof(DataPageViewModel), PageService.Pages[PageService.Pages.Count - 1]);
        }

        // ---------------------------------------------------------------
        // CurrentPageIndex before Initialise
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void CurrentPageIndex_BeforeInitialise_IsMinusOne()
        {
            var sut = CreateSut();
            Assert.AreEqual(-1, sut.CurrentPageIndex);
        }

        // ---------------------------------------------------------------
        // Initialise
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_NavigatesToLoadPage()
        {
            var sut = CreateSut();
            sut.Initialise();
            Assert.AreSame(_loadPage, sut.CurrentPage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_CallsEnterOnLoadPage()
        {
            var sut = CreateSut();
            sut.Initialise();
            Assert.AreEqual(1, _loadPage.EnterCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_CurrentPageIndexIsZero()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise();
            Assert.AreEqual(0, sut.CurrentPageIndex);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_CreatesScopeViaFactory()
        {
            var sut = CreateSut();
            sut.Initialise();
            // One scope created during Initialise
            Assert.AreEqual(1, _scopeFactory.CreateCount);
        }

        // ---------------------------------------------------------------
        // NavigateCommand
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void NavigateCommand_ToAxisPage_ChangesCurrentPage()
        {
            var sut = CreateSut();
            sut.Initialise();
            sut.NavigateCommand.Execute(typeof(AxisPageViewModel));
            Assert.AreSame(_axisPage, sut.CurrentPage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void NavigateCommand_CallsLeaveOnPreviousPage()
        {
            var sut = CreateSut();
            sut.Initialise();
            sut.NavigateCommand.Execute(typeof(AxisPageViewModel));
            Assert.AreEqual(1, _loadPage.LeaveCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void NavigateCommand_CallsEnterOnNewPage()
        {
            var sut = CreateSut();
            sut.Initialise();
            sut.NavigateCommand.Execute(typeof(AxisPageViewModel));
            Assert.AreEqual(1, _axisPage.EnterCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void NavigateCommand_RaisesNavigatedEvent()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise();
            int navigatedIndex = -99;
            sut.Navigated += (s, idx) => navigatedIndex = idx;
            sut.NavigateCommand.Execute(typeof(AxisPageViewModel));
            Assert.AreEqual(1, navigatedIndex);
        }

        // ---------------------------------------------------------------
        // NextPageCommand
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void NextPageCommand_CanExecute_TrueWhenNotOnLastPage()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise(); // navigates to LoadPageViewModel, index 0 — genuinely not last
            Assert.IsTrue(sut.NextPageCommand.CanExecute(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void NextPageCommand_CanExecute_FalseWhenOnLastPage()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise();
            // Navigate to last page (DataPage, index 5)
            sut.NavigateCommand.Execute(typeof(DataPageViewModel));
            Assert.IsFalse(sut.NextPageCommand.CanExecute(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void NextPageCommand_Execute_MovesToNextPage()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise();
            sut.NextPageCommand.Execute(null);
            Assert.AreEqual(1, sut.CurrentPageIndex);
        }

        // ---------------------------------------------------------------
        // PrevPageCommand
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void PrevPageCommand_CanExecute_FalseWhenOnFirstPage()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise(); // navigates to LoadPageViewModel, CurrentPageIndex == 0
            Assert.AreEqual(0, sut.CurrentPageIndex); // confirm the fixture state
            Assert.IsFalse(sut.PrevPageCommand.CanExecute(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PrevPageCommand_CanExecute_TrueWhenNotOnFirstPage()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise();
            sut.NavigateCommand.Execute(typeof(AxisPageViewModel));
            Assert.IsTrue(sut.PrevPageCommand.CanExecute(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PrevPageCommand_Execute_MovesToPrevPage()
        {
            var sut = CreateSutWithRealVms();
            sut.Initialise();
            sut.NavigateCommand.Execute(typeof(AxisPageViewModel));
            sut.PrevPageCommand.Execute(null);
            Assert.AreEqual(0, sut.CurrentPageIndex);
        }

        // ---------------------------------------------------------------
        // EditPage scope behaviour
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void NavigateToEditPage_UsesEditPageScope()
        {
            var sut = CreateSut();
            sut.Initialise();
            sut.NavigateCommand.Execute(typeof(EditPageViewModel));
            Assert.AreSame(_editPage, sut.CurrentPage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ModelFilteredImageOutdated_DisposesEditScope()
        {
            var sut = CreateSut();
            sut.Initialise();
            var scopeCreatedDuringInit = _scopeFactory.LastScope as PageServiceTests_StubScope;

            _model.FirePropertyOutdated(nameof(Model.FilteredImage));

            Assert.IsTrue(scopeCreatedDuringInit.Disposed);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ModelOtherPropertyOutdated_DoesNotDisposeEditScope()
        {
            var sut = CreateSut();
            sut.Initialise();
            var scopeCreatedDuringInit = _scopeFactory.LastScope as PageServiceTests_StubScope;

            _model.FirePropertyOutdated(nameof(Model.InputImage));

            Assert.IsFalse(scopeCreatedDuringInit.Disposed);
        }

        // ---------------------------------------------------------------
        // Dispose
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Dispose_DisposesEditScope()
        {
            var sut = CreateSut();
            sut.Initialise();
            var scope = _scopeFactory.LastScope as PageServiceTests_StubScope;
            sut.Dispose();
            Assert.IsTrue(scope.Disposed);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Dispose_UnsubscribesFromModelPropertyOutdated()
        {
            var sut = CreateSut();
            sut.Initialise();
            sut.Dispose();
            // Create a fresh scope so we can see if the old handler fires
            var newSutForScopeCheck = _scopeFactory.CreateCount;
            // After Dispose, firing PropertyOutdated should NOT create a new scope
            _model.FirePropertyOutdated(nameof(Model.FilteredImage));
            Assert.AreEqual(newSutForScopeCheck, _scopeFactory.CreateCount);
        }
    }
}
