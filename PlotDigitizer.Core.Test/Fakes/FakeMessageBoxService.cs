using System.Collections.Generic;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IMessageBoxService"/>. Records every prompt so a test
	/// can assert that the user was told what happened without touching a real dialog.
	/// </summary>
	internal sealed class FakeMessageBoxService : IMessageBoxService
	{
		public List<(string message, string caption)> OkMessages { get; } = [];

		public List<(string message, string caption)> OkCancelMessages { get; } = [];

		public List<(string message, string caption)> WarningOkCancelMessages { get; } = [];

		public bool OkCancelResult { get; set; }

		public bool WarningOkCancelResult { get; set; }

		public void Show_OK(string message, string caption) => OkMessages.Add((message, caption));

		public bool Show_OkCancel(string message, string caption)
		{
			OkCancelMessages.Add((message, caption));
			return OkCancelResult;
		}

		public bool Show_Warning_OkCancel(string message, string caption)
		{
			WarningOkCancelMessages.Add((message, caption));
			return WarningOkCancelResult;
		}
	}
}
