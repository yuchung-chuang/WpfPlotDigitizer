using PlotDigitizer.Core;

using System;
using System.Windows;
using System.Windows.Input;

using Wpf.Ui.Controls;

namespace PlotDigitizer.App
{
	public partial class MainWindow : UiWindow
	{
		private PageViewModelBase viewModelCache;

		public RelayCommand NextPageCommand { get; }
		public RelayCommand PrevPageCommand { get; }

		public MainWindow()
		{
			NextPageCommand = new RelayCommand(NextPage, CanNextPage);
			PrevPageCommand = new RelayCommand(PrevPage, CanPrevPage);
			InitializeComponent();
			Loaded += MainWindow_Loaded;
			(DI.Resolver.Invoke(typeof(LoadPageViewModel)) as LoadPageViewModel).NextPage += LoadPage_NextPage;
		}

		private void LoadPage_NextPage(object sender, EventArgs e)
		{
			if (NextPageCommand.CanExecute()) {
				NextPageCommand.Execute();
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (KeyGesture gesture in NavigationCommands.BrowseBack.InputGestures) {
				if (gesture.Key == Key.Back) {
					NavigationCommands.BrowseBack.InputGestures.Remove(gesture);
					break;
				}
			}
			foreach (KeyGesture gesture in NavigationCommands.BrowseForward.InputGestures) {
				if (gesture.Key == Key.Back) {
					NavigationCommands.BrowseForward.InputGestures.Remove(gesture);
					break;
				}
			}
		}

		private void PrevPage()
		{
			viewModelCache?.Leave();
			navigation?.Navigate(navigation.SelectedPageIndex - 1);
		}

		private bool CanPrevPage() => navigation?.SelectedPageIndex > 0;
		private void NextPage()
		{
			viewModelCache?.Leave();
			navigation?.Navigate(navigation.SelectedPageIndex + 1);
		}

		private bool CanNextPage() => navigation?.SelectedPageIndex < navigation?.Items.Count - 1;

		private void frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			NextPageCommand.RaiseCanExecuteChanged();
			PrevPageCommand.RaiseCanExecuteChanged();
			viewModelCache = (e.Content as FrameworkElement)?.DataContext as PageViewModelBase;
			viewModelCache?.Enter();
		}

		private void navigation_PreviewNavigate(object sender, EventArgs e)
		{
			viewModelCache?.Leave();
		}
	}
}