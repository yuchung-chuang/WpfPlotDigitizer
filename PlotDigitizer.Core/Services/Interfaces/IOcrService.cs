using Emgu.CV.Structure;
using Emgu.CV;

namespace PlotDigitizer.Core
{
    public interface IOcrService
    {
        string Ocr(Image<Rgba, byte> image);
    }
}
