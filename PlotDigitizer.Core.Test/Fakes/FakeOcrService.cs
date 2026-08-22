using System;
using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IOcrService"/>. Returns the queued results in order
	/// so a test can pin down which region is read first, then repeats the last result.
	/// </summary>
	internal sealed class FakeOcrService : IOcrService
	{
		private readonly Queue<string> results = new();

		public string DefaultResult { get; set; } = string.Empty;

		public int OcrCallCount { get; private set; }

		public List<Image<Rgba, byte>> ReceivedImages { get; } = [];

		public Exception ThrowOnOcr { get; set; }

		public FakeOcrService EnqueueResults(params string[] values)
		{
			foreach (var value in values) {
				results.Enqueue(value);
			}
			return this;
		}

		public string Ocr(Image<Rgba, byte> image)
		{
			OcrCallCount++;
			ReceivedImages.Add(image);
			if (ThrowOnOcr != null) {
				throw ThrowOnOcr;
			}
			return results.Count > 0 ? results.Dequeue() : DefaultResult;
		}
	}
}
