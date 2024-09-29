using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WpfAppTemplate1
{
    public class WindowService : IWindowService
    {
        private readonly Dictionary<ViewModelBase, Window> openedWindows = [];

        public void ShowMainWindow(MainViewModel mainViewModel)
        {
            Window window = new MainWindow
            {
                DataContext = mainViewModel,
            };
            if (mainViewModel is not ViewModelBase viewModel) {
                return;
            }
            Show(window, viewModel);
        }

        public void ShowWindow(ViewModelBase viewModel)
        {
            var window = new Window
            {
                Content = viewModel,
            };
            Show(window, viewModel);
        }

        public bool CloseWindow(ViewModelBase viewModel)
        {
            var window = openedWindows[viewModel];
            if (window is null) {
                return false;
            }
            window.Close();
            return true;
        }
        private void Show(Window window, ViewModelBase viewModel)
        {
            openedWindows.Add(viewModel, window);
            window.Closed += (sender, e) => Window_Closed(sender, viewModel);

            viewModel.Enter();
            window.Show();
        }

        private void Window_Closed(object? sender, ViewModelBase viewModel)
        {
            viewModel.Leave();
        }

    }
}
