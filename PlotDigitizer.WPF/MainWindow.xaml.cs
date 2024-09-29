using System.Windows;
using System.Windows.Input;

using Wpf.Ui.Controls;

namespace PlotDigitizer.WPF
{
    public partial class MainWindow : UiWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
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
	}
}