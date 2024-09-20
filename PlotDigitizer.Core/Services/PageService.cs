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

		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand NavigateCommand { get; private set; }
		public RelayCommand NextPageCommand { get; private set; }
		public RelayCommand PrevPageCommand { get; private set; }
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
			NextPageCommand = new RelayCommand(NextPage, CanNextPage);
			PrevPageCommand = new RelayCommand(PrevPage, CanPrevPage);

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

			NextPageCommand.RaiseCanExecuteChanged();
			PrevPageCommand.RaiseCanExecuteChanged();
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