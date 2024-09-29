using PlotDigitizer.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Wpf.Ui.Controls;

namespace PlotDigitizer.WPF
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
        private bool isDirectNavigated = false;

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			if (Content == null) {
				return;
			}
			if (allowDirectNavigation) {
				isDirectNavigated = true;
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
			if (!isDirectNavigated) {
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
			isDirectNavigated = false;
			allowDirectNavigation = false;
		}
	}
}
