using System;

namespace PlotDigitizer.Core
{
	public class AutoPageTurner
	{
		private readonly PageManager pageManager;

		public AutoPageTurner(MainWindowViewModel mainWindowViewModel, LoadPageViewModel viewModel)
		{
			pageManager = mainWindowViewModel.PageManager;
			viewModel.NextPage += LoadPage_NextPage;
		}

		private void LoadPage_NextPage(object sender, EventArgs e)
		{
			if (pageManager.NextCommand.CanExecute()) {
				pageManager.NextCommand.Execute();
			}
		}

	}
}
