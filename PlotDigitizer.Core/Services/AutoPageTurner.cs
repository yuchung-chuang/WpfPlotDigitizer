using System;

namespace PlotDigitizer.Core
{
	public class AutoPageTurner
	{
		private readonly MainWindowViewModel mainWindowViewModel;

		public AutoPageTurner(MainWindowViewModel mainWindowViewModel, LoadPageViewModel viewModel)
		{
			this.mainWindowViewModel = mainWindowViewModel;
			viewModel.NextPage += LoadPage_NextPage;
		}

		private void LoadPage_NextPage(object sender, EventArgs e)
		{
			if (mainWindowViewModel.NextPageCommand.CanExecute(null)) {
				mainWindowViewModel.NextPageCommand.Execute(null);
			}
		}
	}
}