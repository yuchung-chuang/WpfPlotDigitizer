using System.Windows;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	public partial class AxisLimitPage : Page
	{
		public AxisLimitPage()
		{
			InitializeComponent();
		}

		public AxisLimitPage(AxisLimitPageViewModel viewModel) : this()
		{
			DataContext = viewModel;
		}
	}
}