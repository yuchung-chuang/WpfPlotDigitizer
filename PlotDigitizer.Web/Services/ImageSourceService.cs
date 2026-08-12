using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PlotDigitizer.Core;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Services
{
	public sealed class ImageLoadResult
	{
		private ImageLoadResult(Image<Rgba, byte> image, string error)
		{
			Image = image;
			Error = error;
		}

		public Image<Rgba, byte> Image { get; }

		public string Error { get; }

		public bool IsValid => Image is not null;

		public static ImageLoadResult Success(Image<Rgba, byte> image) => new(image, null);

		public static ImageLoadResult Failure(string error) => new(null, error);
	}

	/// <summary>
	/// The web replacement for the desktop <c>IFileDialogService</c>, clipboard and drag-and-drop
	/// image sources. Everything a browser can hand us is untrusted, so each entry point validates
	/// before decoding.
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

		private readonly IDownloadService downloadService;
		private readonly CollectingMessageBoxService messageBox;
		private readonly DigitizerSessionOptions options;
		private readonly ILogger<ImageSourceService> logger;

		public ImageSourceService(IDownloadService downloadService,
			IMessageBoxService messageBox,
			IOptions<DigitizerSessionOptions> options,
			ILogger<ImageSourceService> logger)
		{
			this.downloadService = downloadService;
			this.messageBox = messageBox as CollectingMessageBoxService;
			this.options = options.Value;
			this.logger = logger;
		}

		public async Task<ImageLoadResult> FromUploadAsync(IFormFile file, CancellationToken token = default)
		{
			if (file is null || file.Length == 0) {
				return ImageLoadResult.Failure("Choose an image file to upload.");
			}

			if (file.Length > options.MaxUploadBytes) {
				return ImageLoadResult.Failure($"The image is larger than the {options.MaxUploadBytes / (1024 * 1024)} MB limit.");
			}

			var extension = Path.GetExtension(file.FileName);
			if (!AllowedContentTypes.Contains(file.ContentType ?? string.Empty)
				&& !AllowedExtensions.Contains(extension ?? string.Empty)) {
				return ImageLoadResult.Failure("Only PNG, JPEG, BMP, GIF and TIFF images are supported.");
			}

			using var memoryStream = new MemoryStream();
			await file.CopyToAsync(memoryStream, token);
			memoryStream.Position = 0;
			return Decode(memoryStream);
		}

		/// <summary>
		/// Handles a <c>data:image/...;base64,...</c> payload, which is what the browser produces
		/// for a pasted or dropped bitmap.
		/// </summary>
		public ImageLoadResult FromDataUrl(string dataUrl)
		{
			if (string.IsNullOrWhiteSpace(dataUrl)) {
				return ImageLoadResult.Failure("No image data was received.");
			}

			var separator = dataUrl.IndexOf(",", StringComparison.Ordinal);
			if (!dataUrl.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) || separator < 0) {
				return ImageLoadResult.Failure("The pasted content is not an image.");
			}

			byte[] bytes;
			try {
				bytes = Convert.FromBase64String(dataUrl[(separator + 1)..]);
			}
			catch (FormatException) {
				return ImageLoadResult.Failure("The pasted image could not be decoded.");
			}

			if (bytes.LongLength > options.MaxUploadBytes) {
				return ImageLoadResult.Failure($"The image is larger than the {options.MaxUploadBytes / (1024 * 1024)} MB limit.");
			}

			using var memoryStream = new MemoryStream(bytes);
			return Decode(memoryStream);
		}

		public async Task<ImageLoadResult> FromUrlAsync(string url, CancellationToken token = default)
		{
			var uri = (url ?? string.Empty).Trim().ToUri();
			if (uri is null || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)) {
				return ImageLoadResult.Failure("Enter an http or https address that points directly at an image.");
			}

			if (!await IsPubliclyRoutableAsync(uri, token)) {
				return ImageLoadResult.Failure("That address is not allowed.");
			}

			messageBox?.Clear();
			var image = await downloadService.DownloadImageAsync(uri, token);
			if (image is null) {
				var reason = messageBox?.Messages.FirstOrDefault() ?? "The image could not be downloaded.";
				return ImageLoadResult.Failure(reason);
			}

			return ImageLoadResult.Success(image);
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

		/// <summary>
		/// Blocks server side request forgery: the supplied host must not resolve to a loopback,
		/// link local or private address, which would let a caller reach internal services.
		/// </summary>
		private async Task<bool> IsPubliclyRoutableAsync(Uri uri, CancellationToken token)
		{
			try {
				var addresses = IPAddress.TryParse(uri.DnsSafeHost, out var literal)
					? new[] { literal }
					: await Dns.GetHostAddressesAsync(uri.DnsSafeHost, token);

				return addresses.Length > 0 && addresses.All(IsPublic);
			}
			catch (Exception ex) {
				logger?.LogWarning(ex, "Could not resolve {Host} while validating a download url.", uri.DnsSafeHost);
				return false;
			}
		}

		private static bool IsPublic(IPAddress address)
		{
			if (IPAddress.IsLoopback(address)
				|| address.Equals(IPAddress.Any)
				|| address.Equals(IPAddress.IPv6Any)) {
				return false;
			}

			if (address.AddressFamily == AddressFamily.InterNetworkV6) {
				if (address.IsIPv6LinkLocal || address.IsIPv6SiteLocal || address.IsIPv6Multicast) {
					return false;
				}
				// Unique local addresses, fc00::/7.
				var v6 = address.GetAddressBytes();
				return (v6[0] & 0xFE) != 0xFC;
			}

			var bytes = address.GetAddressBytes();
			return bytes[0] switch
			{
				0 => false,
				10 => false,
				127 => false,
				169 when bytes[1] == 254 => false,                        // link local
				172 when bytes[1] >= 16 && bytes[1] <= 31 => false,       // private
				192 when bytes[1] == 168 => false,                        // private
				100 when bytes[1] >= 64 && bytes[1] <= 127 => false,      // carrier grade NAT
				>= 224 => false,                                          // multicast and reserved
				_ => true,
			};
		}
	}
}
