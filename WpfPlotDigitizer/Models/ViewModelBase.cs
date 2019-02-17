using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  public class MessageEventArgs : EventArgs
  {
    public MessageEventArgs(string message, MessageTypes type)
    {
      Message = message;
      Type = type;
    }

    public string Message { get; set; }
    public MessageTypes Type { get; set; }
  }

  public class ViewModelBase : CycLibrary.MVVM.ViewModelBase
  {
    public event EventHandler<MessageEventArgs> MessageRequest;

    /// <summary>
    /// Allow View Models to send message.show request to subscribed view.
    /// </summary>
    /// <remarks>
    /// Highly coupled with <see cref="MessageManager"/>.
    /// </remarks>
    public void OnMessageRequest(string message, MessageTypes type)
    {
      MessageRequest?.Invoke(this, new MessageEventArgs(message, type));
    }
  }
}