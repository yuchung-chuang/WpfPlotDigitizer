using Emgu.CV;
using Emgu.CV.Structure;

using System.Collections.Specialized;

namespace PlotDigitizer.Core
{
	public interface IClipboardService
	{
		bool ContainsFileDropList();

		bool ContainsImage();
        bool ContainsText();
        StringCollection GetFileDropList();

		Image<Rgba, byte> GetImage();
        string GetText();
    }
}