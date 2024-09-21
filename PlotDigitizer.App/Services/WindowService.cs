using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PlotDigitizer.Core;
using Wpf.Ui.Controls;

namespace PlotDigitizer.App
{
	public class WindowService : IWindowService
    {
        private readonly Collection<Window> openedWindows = [];
		private SplashWindow splashWindow;

		public void ShowMainWindow(ViewModelBase viewModel)
        {
            var window = new MainWindow
            {
                DataContext = viewModel,
            };
			openedWindows.Add(window);
			window.Closed += (sender, e) => Window_Closed(sender, viewModel);

			viewModel.Enter();
			window.Show();
		}

        public void ShowWindow(ViewModelBase viewModel)
        {
            var window = new Window
            {
                Content = viewModel,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 800,
                Height = 600,
            };
            openedWindows.Add(window);
            window.Closed += (sender, e) => Window_Closed(sender, viewModel);
            
            viewModel.Enter();
            window.Show();
        }

        public bool? ShowDialog(ViewModelBase viewModel)
        {
            var dialog = new Window
            {
                Content = viewModel,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Width = 600,
                Height = 300,
                Owner = Application.Current.MainWindow,
            };
            openedWindows.Add(dialog);
            dialog.Closed += (sender, e) => Window_Closed(sender, viewModel);

            viewModel.Enter();
            return dialog.ShowDialog();
        }

        private void Window_Closed(object? sender, ViewModelBase viewModel)
        {
            viewModel.Leave();
        }

        public bool CloseWindow(ViewModelBase viewModel)
        {
            var window = openedWindows.FirstOrDefault(window => window.Content == viewModel);
            var result = window != null;
            window?.Close();
            return result;
        }

        public bool CloseDialog(ViewModelBase viewModel, bool dialogResult)
        {
            var window = openedWindows.FirstOrDefault(window => window.Content == viewModel);
            if (window == null) {
                return false;
            }
            window.DialogResult = dialogResult;
            window.Close();
            return true;
        }

		public void ShowSplashWindow()
		{
			splashWindow = new SplashWindow();
			splashWindow.Show();
		}

        public void CloseSplashWindow()
        {
            splashWindow?.Close();
        }
	}
}
