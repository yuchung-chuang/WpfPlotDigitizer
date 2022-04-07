using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Win32;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using PropertyChanged;

namespace PlotDigitizer.App
{
	public partial class PreviewPage : Page
	{
		private readonly PreviewPageViewModel viewModel;

		private PreviewPage()
		{
			InitializeComponent();
			Loaded += PreviewPage_Loaded;
		}
		public PreviewPage(PreviewPageViewModel viewModel) : this()
		{
			this.viewModel = viewModel;
			DataContext = viewModel;
		}

		private void PreviewPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			viewModel.ExtractPoints();
		}

		
	}

	public enum ExportResults
	{
		None,
		Sucessful,
		Failed,
		Canceled
	}
}