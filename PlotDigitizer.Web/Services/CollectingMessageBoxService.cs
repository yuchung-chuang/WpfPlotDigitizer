using PlotDigitizer.Core;

using System.Collections.Generic;

namespace PlotDigitizer.Web.Services
{
	/// <summary>
	/// Web implementation of the desktop <see cref="IMessageBoxService"/>. A server cannot show a
	/// modal dialog, so messages raised by shared Core services are collected here and rendered as
	/// inline notifications by the page that triggered them.
	/// </summary>
	public sealed class CollectingMessageBoxService : IMessageBoxService
	{
		private readonly List<string> messages = new();

		public IReadOnlyList<string> Messages => messages;

		public void Show_OK(string message, string caption)
		{
			messages.Add(message);
		}

		public bool Show_OkCancel(string message, string caption)
		{
			messages.Add(message);
			// There is nobody to ask, so never confirm.
			return false;
		}

		public bool Show_Warning_OkCancel(string message, string caption)
		{
			messages.Add(message);
			return false;
		}

		public void Clear() => messages.Clear();
	}
}
