using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IDownloadService"/>. Never touches the network.
	/// </summary>
	internal sealed class FakeDownloadService : IDownloadService
	{
		public Image<Rgba, byte> Result { get; set; }

		public List<Uri> RequestedUrls { get; } = [];

		public Task<Image<Rgba, byte>> DownloadImageAsync(Uri url, CancellationToken token)
		{
			RequestedUrls.Add(url);
			return Task.FromResult(Result);
		}
	}
}
