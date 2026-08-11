using System;
using System.Windows.Input;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IPageService"/>. The commands are real
	/// <see cref="RelayCommand"/> instances so a caller that guards with
	/// <see cref="ICommand.CanExecute"/> is exercised the same way it is in production.
	/// </summary>
	internal sealed class FakePageService : IPageService
	{
		private readonly RelayCommand nextPageCommand;
		private readonly RelayCommand prevPageCommand;

		public FakePageService()
		{
			nextPageCommand = new RelayCommand(() => NextPageExecutedCount++, () => CanNextPage);
			prevPageCommand = new RelayCommand(() => PrevPageExecutedCount++, () => CanPrevPage);
			NavigateCommand = new RelayCommand<Type>(type => NavigatedTo = type);
		}

		public bool CanNextPage { get; set; } = true;

		public bool CanPrevPage { get; set; } = true;

		public int NextPageExecutedCount { get; private set; }

		public int PrevPageExecutedCount { get; private set; }

		public Type NavigatedTo { get; private set; }

		public int InitialiseCallCount { get; private set; }

		public ViewModelBase CurrentPage { get; set; }

		public int CurrentPageIndex { get; set; }

		public ICommand NavigateCommand { get; }

		public ICommand NextPageCommand => nextPageCommand;

		public ICommand PrevPageCommand => prevPageCommand;

		public event EventHandler<int> Navigated;

		public void Initialise()
		{
			InitialiseCallCount++;
			Navigated?.Invoke(this, CurrentPageIndex);
		}
	}
}
