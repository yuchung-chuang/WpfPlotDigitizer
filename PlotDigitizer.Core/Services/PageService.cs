using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace PlotDigitizer.Core
{
	public class PageService : IPageService, IDisposable, INotifyPropertyChanged
	{
		private readonly IServiceScope serviceScope;
		private readonly IServiceProvider serviceProvider;
		private readonly RelayCommand nextPageCommand;
		private readonly RelayCommand prevPageCommand;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<int> Navigated;
		public ICommand NavigateCommand { get; }
		public ICommand NextPageCommand => nextPageCommand; 
		public ICommand PrevPageCommand => prevPageCommand; 
		public static List<Type> Pages { get; private set; } = [
				typeof(LoadPageViewModel),
				typeof(RangePageViewModel),
				typeof(AxisPageViewModel),
				typeof(FilterPageViewModel),
				typeof(EditPageViewModel),
				typeof(DataPageViewModel),
			];
        public PageViewModelBase CurrentPage { get; set; } 
        public int CurrentPageIndex => CurrentPage is null ? -1 : Pages.FindIndex(t => t == CurrentPage.GetType());

        public PageService(IServiceScopeFactory serviceScopeFactory)
        {
			NavigateCommand = new RelayCommand<Type>(NavigateTo);
			nextPageCommand = new RelayCommand(NextPage, CanNextPage);
			prevPageCommand = new RelayCommand(PrevPage, CanPrevPage);

			serviceScope = serviceScopeFactory.CreateScope();
			serviceProvider = serviceScope.ServiceProvider;
		}

		public void Initialise()
		{
			CurrentPage = serviceProvider.GetRequiredService(Pages[0]) as PageViewModelBase;
		}
        private void NavigateTo(Type pageType)
		{
			var PrevPage = CurrentPage;
			PrevPage.Leave();
			var newPage = serviceProvider.GetRequiredService(pageType) as PageViewModelBase;
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

		public void Dispose()
		{
			serviceScope.Dispose();
			GC.SuppressFinalize(this);
		}

	}
}