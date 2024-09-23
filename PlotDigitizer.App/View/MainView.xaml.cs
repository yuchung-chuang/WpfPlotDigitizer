using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();
			Loaded += MainView_Loaded;
		}

		private void MainView_Loaded(object sender, RoutedEventArgs e)
		{
			(DataContext as MainViewModel).PageService.Navigated += PageService_Navigated;
		}

		private void PageService_Navigated(object sender, int pageIndex)
		{
			foreach (var item in navigation.Items.OfType<NavigationItem>()) {
				item.IsActive = false;
			}
			// *2 because there are separators
			(navigation.Items[pageIndex * 2] as NavigationItem).IsActive = true;
		}

		private bool allowDirectNavigation = false;
		private NavigatingCancelEventArgs navArgs = null;
		private Duration duration = new(TimeSpan.FromSeconds(0.3));
		private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			if (Content == null || allowDirectNavigation) {
				return;
			}
			allowDirectNavigation = true;

			e.Cancel = true;

			navArgs = e;

			var animationFade = new DoubleAnimation
			{
				From = 1,
				To = 0,
				Duration = duration
			};
			animationFade.Completed += AnimationFadeOut_Completed;
			frame.BeginAnimation(OpacityProperty, animationFade);
		}

		private void AnimationFadeOut_Completed(object sender, EventArgs e)
		{
			switch (navArgs.NavigationMode) {
				case NavigationMode.New:
					if (navArgs.Uri == null)
						frame.Navigate(navArgs.Content);
					else
						frame.Navigate(navArgs.Uri);
					break;
				case NavigationMode.Back:
					frame.GoBack();
					break;
				case NavigationMode.Forward:
					frame.GoForward();
					break;
				case NavigationMode.Refresh:
					frame.Refresh();
					break;
			}

			var animationFade = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = duration
			};
			animationFade.Completed += AnimationFadeIn_Completed;
			frame.BeginAnimation(OpacityProperty, animationFade);
		}

		private void AnimationFadeIn_Completed(object sender, EventArgs e)
		{
			allowDirectNavigation = false;
		}
	}
}
