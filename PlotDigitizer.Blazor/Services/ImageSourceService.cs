using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Blazor.Services
{
	public sealed class ImageLoadResult
	{
		private ImageLoadResult(Image<Rgba, byte>? image, string? error)
		{
			Image = image;
			Error = error;
		}

		public Image<Rgba, byte>? Image { get; }

		public string? Error { get; }

		public bool IsValid => Image is not null;

		public static ImageLoadResult Success(Image<Rgba, byte> image) => new(image, null);

		public static ImageLoadResult Failure(string error) => new(null, error);
	}

	/// <summary>
	/// The web replacement for the desktop <c>IFileDialogService</c>. Everything a browser hands us
	/// is untrusted, so uploads are validated before decoding.
	///
	/// Trimmed from <c>PlotDigitizer.Web.Services.ImageSourceService</c>: this spike's minimal Load
	/// page only needs a file upload, so the clipboard/drag-and-drop data url handling and the
	/// SSRF-guarded url download path are left out.
	/// </summary>
	public sealed class ImageSourceService
	{
		private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
		{
			"image/png",
			"image/jpeg",
			"image/pjpeg",
			"image/bmp",
			"image/x-ms-bmp",
			"image/gif",
			"image/tiff",
		};

		private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
		{
			".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff",
		};

		private readonly DigitizerSessionOptions options;
		private readonly ILogger<ImageSourceService> logger;

		public ImageSourceService(IOptions<DigitizerSessionOptions> options, ILogger<ImageSourceService> logger)
		{
			this.options = options.Value;
			this.logger = logger;
		}

		/// <summary>
		/// Reads a file selected through Blazor's <c>InputFile</c>. Unlike the Razor Pages upload
		/// handler, this never sees an <c>IFormFile</c>: Blazor Server streams the bytes over the
		/// SignalR circuit instead of a multipart form post, so the size cap is enforced by the
		/// <c>maxAllowedSize</c> argument to <c>OpenReadStream</c> rather than by
		/// <c>FormOptions.MultipartBodyLengthLimit</c>.
		/// </summary>
		public async Task<ImageLoadResult> FromUploadAsync(IBrowserFile file, CancellationToken token = default)
		{
			if (file is null || file.Size == 0) {
				return ImageLoadResult.Failure("Choose an image file to upload.");
			}

			if (file.Size > options.MaxUploadBytes) {
				return ImageLoadResult.Failure($"The image is larger than the {options.MaxUploadBytes / (1024 * 1024)} MB limit.");
			}

			var extension = Path.GetExtension(file.Name);
			if (!AllowedContentTypes.Contains(file.ContentType ?? string.Empty)
				&& !AllowedExtensions.Contains(extension ?? string.Empty)) {
				return ImageLoadResult.Failure("Only PNG, JPEG, BMP, GIF and TIFF images are supported.");
			}

			using var memoryStream = new MemoryStream();
			await using (var stream = file.OpenReadStream(options.MaxUploadBytes, token)) {
				await stream.CopyToAsync(memoryStream, token);
			}
			memoryStream.Position = 0;
			return Decode(memoryStream);
		}

		private ImageLoadResult Decode(Stream stream)
		{
			try {
				using var bitmap = System.Drawing.Image.FromStream(stream) as Bitmap;
				if (bitmap is null) {
					return ImageLoadResult.Failure("The file is not a supported image.");
				}
				return ImageLoadResult.Success(bitmap.ToImage<Rgba, byte>());
			}
			catch (Exception ex) {
				logger?.LogWarning(ex, "Rejected an upload that could not be decoded as an image.");
				return ImageLoadResult.Failure("The file is not a supported image.");
			}
		}
	}
}
