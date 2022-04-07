using System.Windows;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	public partial class AxisLimitPage : Page
	{
		private readonly AxisLimitPageViewModel viewModel;

		public AxisLimitPage()
		{
			InitializeComponent();
			Unloaded += AxisLimitPage_Unloaded;
		}

		public AxisLimitPage(AxisLimitPageViewModel viewModel) : this()
		{
			this.viewModel = viewModel;
			DataContext = viewModel;
		}

		private void AxisLimitPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			viewModel.Model.Setting.AxisLimit = viewModel.AxisLimit;
			viewModel.Model.Setting.AxisLogBase = viewModel.AxisLogBase;
		}
	}
}