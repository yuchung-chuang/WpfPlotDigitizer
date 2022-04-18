using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Specialized;

namespace PlotDigitizer.Core
{
	public interface IClipboard
	{
		bool ContainsFileDropList();
		bool ContainsImage();
		StringCollection GetFileDropList();
		Image<Rgba, byte> GetImage();
	}
}
