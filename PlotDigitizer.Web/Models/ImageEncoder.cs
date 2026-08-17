using Emgu.CV;
using Emgu.CV.Structure;

using System;

namespace PlotDigitizer.Web.Models
{
	/// <summary>
	/// The images the client can request from the image endpoint.
	/// </summary>
	public static class ImageKind
	{
		public const string Input = "input";
		public const string Cropped = "cropped";
		public const string Filtered = "filtered";
		public const string Edited = "edited";
		public const string Editor = "editor";
		public const string Preview = "preview";
	}

	public static class ImageEncoder
	{
		/// <summary>
		/// Encodes to PNG bytes. Images are served from a dedicated endpoint rather than inlined as
		/// a base64 <c>data:</c> uri, which keeps the html small and lets the browser cache them.
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
