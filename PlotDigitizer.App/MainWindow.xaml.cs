using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public PageManager PageManager { get; private set; }

		public IEnumerable<string> PageNameList { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		public MainWindow(PageManager pageManager) : this()
		{
			PageManager = pageManager;
			PageNameList = pageManager.PageList.Select(page => page.GetType().Name);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void mainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			// disable frame navigation to prevent keyboard input conflict
			if (e.NavigationMode == NavigationMode.Back ||
				e.NavigationMode == NavigationMode.Forward) {
				e.Cancel = true;
			}
		}
	}
}
