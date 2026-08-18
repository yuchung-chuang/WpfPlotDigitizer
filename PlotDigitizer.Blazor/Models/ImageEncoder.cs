using Emgu.CV;
using Emgu.CV.Structure;

using System;

namespace PlotDigitizer.Blazor.Models
{
	/// <summary>
	/// The images the <c>/image/{kind}</c> endpoint in <c>Program.cs</c> can serve.
	/// </summary>
	public static class ImageKind
	{
		public const string Input = "input";
		public const string Cropped = "cropped";
		public const string Filtered = "filtered";
	}

	public static class ImageEncoder
	{
		/// <summary>
		/// Encodes to PNG bytes. Images are served from a dedicated endpoint rather than inlined as
		/// a base64 <c>data:</c> uri, which keeps the rendered markup small and lets the browser
		/// cache them.
		/// </summary>
		public static byte[] ToPng(this Image<Rgba, byte> image)
		{
			if (image is null) {
				return Array.Empty<byte>();
			}
			return CvInvoke.Imencode(".png", image);
		}
	}
}
