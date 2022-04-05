using System;
using System.ComponentModel;
using System.Windows;

namespace PlotDigitizer.App
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<MessageEventArgs> MessageBoxRequested;

		protected virtual void OnMessageBoxRequested(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, MessageBoxResult result = MessageBoxResult.None, MessageBoxOptions options = MessageBoxOptions.None)
		{
			MessageBoxRequested?.Invoke(this, new MessageEventArgs(message, caption, button, image, result, options));
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}
	}
}
