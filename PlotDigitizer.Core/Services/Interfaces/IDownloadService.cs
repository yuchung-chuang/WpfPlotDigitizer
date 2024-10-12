using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Core
{
    public interface IDownloadService
    {
        Task<Image<Rgba, byte>> DownloadImageAsync(Uri url, CancellationToken token);
    }
}
