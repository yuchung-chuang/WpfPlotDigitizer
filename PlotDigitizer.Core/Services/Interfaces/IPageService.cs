using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PlotDigitizer.Core
{
	public interface IPageService
	{
		ViewModelBase CurrentPage { get; set; }
		int CurrentPageIndex { get; }
		ICommand NavigateCommand { get; }
		ICommand NextPageCommand { get; }
		ICommand PrevPageCommand { get; }

		event EventHandler<int> Navigated;

		void Initialise();
	}
}