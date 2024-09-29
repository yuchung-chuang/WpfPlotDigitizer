namespace PlotDigitizer.Core
{
    public interface IWindowService
	{
		bool CloseDialog(ViewModelBase viewModel, bool dialogResult);
		bool CloseWindow(ViewModelBase viewModel);
		bool? ShowDialog(ViewModelBase viewModel);
		void ShowWindow(ViewModelBase viewModel);
	}
}
