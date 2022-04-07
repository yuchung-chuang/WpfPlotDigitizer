using PlotDigitizer.Core;
using System.Windows;

namespace PlotDigitizer.App
{
	public class MessageBoxService : IMessageBoxService
	{
		public void Show_OK(string message, string caption)
		{
			MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public bool Show_OkCancel(string message, string caption)
		{
			var result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Information);
			return result switch
			{
				MessageBoxResult.OK => true,
				_ => false,
			};
		}

		public bool Show_Warning_OkCancel(string message, string caption)
		{
			var result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
			return result switch
			{
				MessageBoxResult.OK => true,
				_ => false,
			};
		}
	}
}
