using PlotDigitizer.Core;

using Wpf.Ui.Controls;

namespace PlotDigitizer.App.Services
{
	public class WpfUiMessageBoxService : IMessageBoxService
	{
		public void Show_OK(string message, string caption)
		{
			var messageBox = new MessageBox
			{
				ButtonLeftName = "OK",
				ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Success,
			};
			messageBox.ButtonLeftClick += CloseMessageBox;
			messageBox.ButtonRightClick += CloseMessageBox;
			messageBox.Show(caption, message);
		}

		private void CloseMessageBox(object sender, System.Windows.RoutedEventArgs e) => (sender as MessageBox)?.Close();

		public bool Show_OkCancel(string message, string caption)
		{
			var messageBox = new MessageBox
			{
				ButtonLeftName = "OK",
				ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Info,
				ButtonRightName = "Cancel",
				ButtonRightAppearance = Wpf.Ui.Common.ControlAppearance.Light,
				Title = caption,
				Content = message,
			};
			bool? result = null;
			messageBox.ButtonLeftClick += (s, e) => result = true;
			messageBox.ButtonRightClick += (s, e) => result = false;
			messageBox.ButtonLeftClick += CloseMessageBox;
			messageBox.ButtonRightClick += CloseMessageBox;
			messageBox.ShowDialog();
			return result is null ? false : (bool)result;
		}

		public bool Show_Warning_OkCancel(string message, string caption)
		{
			var messageBox = new MessageBox
			{
				ButtonLeftName = "OK",
				ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Info,
				ButtonRightName = "Cancel",
				ButtonRightAppearance = Wpf.Ui.Common.ControlAppearance.Light,
				Title = caption,
				Content = message,
			};
			bool? result = null;
			messageBox.ButtonLeftClick += (s, e) => result = true;
			messageBox.ButtonRightClick += (s, e) => result = false;
			messageBox.ButtonLeftClick += CloseMessageBox;
			messageBox.ButtonRightClick += CloseMessageBox;
			messageBox.ShowDialog();
			return result is null ? false : (bool)result;
		}
	}
}
