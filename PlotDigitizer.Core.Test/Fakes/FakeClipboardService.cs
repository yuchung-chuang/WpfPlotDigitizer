using System.Collections.Specialized;

using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IClipboardService"/>. Every probe is configurable so a
	/// test can drive each branch of the paste logic independently.
	/// </summary>
	internal sealed class FakeClipboardService : IClipboardService
	{
		public bool HasImage { get; set; }

		public bool HasFileDropList { get; set; }

		public bool HasText { get; set; }

		public Image<Rgba, byte> ImageResult { get; set; }

		public StringCollection FileDropListResult { get; set; } = [];

		public string TextResult { get; set; }

		public bool ContainsFileDropList() => HasFileDropList;

		public bool ContainsImage() => HasImage;

		public bool ContainsText() => HasText;

		public StringCollection GetFileDropList() => FileDropListResult;

		public Image<Rgba, byte> GetImage() => ImageResult;

		public string GetText() => TextResult;
	}
}
