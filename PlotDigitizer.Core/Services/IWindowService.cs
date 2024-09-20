using PlotDigitizer.Core;

namespace PlotDigitizer.Core
{
	public interface IWindowService
	{
		bool CloseDialog(ViewModelBase viewModel, bool dialogResult);
		void CloseSplashWindow();
		bool CloseWindow(ViewModelBase viewModel);
		bool? ShowDialog(ViewModelBase viewModel);
		void ShowMainWindow(ViewModelBase viewModel);
		void ShowSplashWindow();
		void ShowWindow(ViewModelBase viewModel);
	}
}
