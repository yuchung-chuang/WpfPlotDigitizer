using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using PropertyChanged;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	public partial class FilterPage : Page
	{
		private readonly FilterPageViewModel viewModel;

		public FilterPage()
		{
			InitializeComponent();
			Loaded += FilterPage_Loaded;
			Unloaded += FilterPage_Unloaded;
		}

		public FilterPage(FilterPageViewModel viewModel) : this()
		{
			this.viewModel = viewModel;
			DataContext = viewModel;
		}


		private void FilterPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			viewModel.FilterImage();
		}

		private void FilterPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			viewModel.Model.Setting.FilterMin = viewModel.FilterMin;
			viewModel.Model.Setting.FilterMax = viewModel.FilterMax;
		}
	}
}