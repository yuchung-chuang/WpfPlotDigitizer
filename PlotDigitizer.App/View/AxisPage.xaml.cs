using System.Windows;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	public partial class AxisPage : Page
	{
		private readonly AxisPageViewModel viewModel;

		public AxisPage()
		{
			InitializeComponent();
			Loaded += AxisPage_Loaded;
			Unloaded += AxisPage_Unloaded;
		}


		public AxisPage(AxisPageViewModel viewModel) : this()
		{
			this.viewModel = viewModel;
			DataContext = viewModel;
		}

		private void AxisPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			if (viewModel.Model.Setting.AxisLocation == default) {
				if (viewModel.GetAxisCommand.CanExecute()) {
					viewModel.GetAxisCommand.Execute();
				}
			}
		}
		private void AxisPage_Unloaded(object sender, RoutedEventArgs e)
		{
			viewModel.Model.CropImage();
		}
	}
}