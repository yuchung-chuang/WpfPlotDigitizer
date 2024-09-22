namespace PlotDigitizer.Core
{
	public interface IMessageBoxService
	{
		void Show_OK(string message, string caption);

		bool Show_OkCancel(string message, string caption);

		bool Show_Warning_OkCancel(string message, string caption);
	}
}