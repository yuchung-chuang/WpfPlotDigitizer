using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace PlotDigitizer.Core
{
	public class PageService : IPageService, IDisposable, INotifyPropertyChanged
	{
		private readonly IServiceProvider serviceProvider;
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly RelayCommand nextPageCommand;
		private readonly RelayCommand prevPageCommand;
		private readonly Model model;
		private IServiceScope editPageScope;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<int> Navigated;
		public ICommand NavigateCommand { get; }
		public ICommand NextPageCommand => nextPageCommand;
		public ICommand PrevPageCommand => prevPageCommand;
		public static List<Type> Pages { get; private set; } = [
				typeof(LoadPageViewModel),
				typeof(AxisPageViewModel),
				typeof(RangePageViewModel),
				typeof(FilterPageViewModel),
				typeof(EditPageViewModel),
				typeof(DataPageViewModel),
			];
		public PageViewModelBase CurrentPage { get; set; }
		public int CurrentPageIndex => CurrentPage is null ? -1 : Pages.FindIndex(t => t == CurrentPage.GetType());

		public PageService(Model model,
			IServiceProvider serviceProvider,
			IServiceScopeFactory serviceScopeFactory)
		{
			this.model = model;
			this.serviceProvider = serviceProvider;
			this.serviceScopeFactory = serviceScopeFactory;

			NavigateCommand = new RelayCommand<Type>(NavigateTo);
			nextPageCommand = new RelayCommand(NextPage, CanNextPage);
			prevPageCommand = new RelayCommand(PrevPage, CanPrevPage);
		}

		public void Initialise()
		{
			CurrentPage = serviceProvider.GetRequiredService(Pages[0]) as PageViewModelBase;

			editPageScope = serviceScopeFactory.CreateScope();

			model.PropertyOutdated += Model_PropertyOutdated;
		}

		private void NavigateTo(Type pageType)
		{
			var PrevPage = CurrentPage;
			PrevPage.Leave();
			PageViewModelBase newPage;
			if (pageType == typeof(EditPageViewModel)) {
				editPageScope ??= serviceScopeFactory.CreateScope();
				newPage = editPageScope.ServiceProvider.GetRequiredService(pageType) as PageViewModelBase;
			}
			else {
				newPage = serviceProvider.GetRequiredService(pageType) as PageViewModelBase;
			}
			newPage.Enter();
			CurrentPage = newPage;

			nextPageCommand.RaiseCanExecuteChanged();
			prevPageCommand.RaiseCanExecuteChanged();
			Navigated?.Invoke(this, CurrentPageIndex);
		}

		private void NavigateTo(int index) => NavigateTo(Pages[index]);
		private void PrevPage() => NavigateTo(CurrentPageIndex - 1);
		private bool CanPrevPage() => CurrentPageIndex > 0;
		private void NextPage() => NavigateTo(CurrentPageIndex + 1);
		private bool CanNextPage() => CurrentPageIndex < Pages.Count - 1;
		private void Model_PropertyOutdated(object sender, string e)
		{
			if (e == nameof(Model.FilteredImage)) {
				editPageScope?.Dispose();
				editPageScope = null;
			}
		}
		public void Dispose()
		{
			editPageScope?.Dispose();
			model.PropertyOutdated -= Model_PropertyOutdated;
			GC.SuppressFinalize(this);
		}

	}
}