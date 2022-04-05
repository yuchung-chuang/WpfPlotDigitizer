using System;
using System.Windows;

namespace PlotDigitizer.App
{
	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs(string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageBoxResult result, MessageBoxOptions options)
		{
			Message = message;
			Caption = caption;
			Button = button;
			Image = image;
			Result = result;
			Options = options;
		}

		public string Message { get; set; }
		public string Caption { get; set; }
		public MessageBoxButton Button { get; set; }
		public MessageBoxImage Image { get; set; }
		public MessageBoxResult Result { get; set; }
		public MessageBoxOptions Options { get; set; }


	}
}
