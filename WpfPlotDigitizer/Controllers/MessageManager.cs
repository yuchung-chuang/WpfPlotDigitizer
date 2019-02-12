using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class MessageManager
  {
    public MessageManager()
    {
      browsePageVM.MessageRequest += OnMessageRequested;
      savePageVM.MessageRequest += OnMessageRequested;
    }

    private void OnMessageRequested(object sender, MessageEventArgs e)
    {
      switch (e.Type)
      {
        case MessageTypes.Warning:
          Warning(e.Message);
          break;
        case MessageTypes.Information:
          Information(e.Message);
          break;
        default:
          break;
      }
    }

    public static void Warning(string message)
    {
      mainWindow.ShowMessageAsync("Plot Digitizer Warning", message, MessageDialogStyle.Affirmative, new MetroDialogSettings
      {
        ColorScheme = MetroDialogColorScheme.Inverted,
      });
    }

    public static void Information(string message)
    {
      mainWindow.ShowMessageAsync("Plot Digitizer Message", message);
    }
  }
}
