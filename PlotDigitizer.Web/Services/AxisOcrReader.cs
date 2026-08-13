using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using PlotDigitizer.Core;

using System;

namespace PlotDigitizer.Web.Services
{
	/// <summary>
	/// Reads the axis tick labels and titles with the shared Core OCR services.
	///
	/// Tesseract is constructed lazily and every failure is contained: a server that cannot load
	/// its training data must still serve the range page so the user can type the values in, rather
	/// than returning an error for the whole request.
	/// </summary>
	public sealed class AxisOcrReader
	{
		private readonly IServiceProvider services;
		private readonly IImageService imageService;
		private readonly ILogger<AxisOcrReader> logger;

		private IOcrService numerical;
		private IOcrService text;
		private bool? available;

		public AxisOcrReader(IServiceProvider services, IImageService imageService, ILogger<AxisOcrReader> logger)
		{
			this.services = services;
			this.imageService = imageService;
			this.logger = logger;
		}

		public bool IsAvailable => available ??= TryResolve();

		public string ReadNumber(Image<Rgba, byte> image, RectangleD roi) => Read(image, roi, false, numerical);

		public string ReadText(Image<Rgba, byte> image, RectangleD roi, bool rotate) => Read(image, roi, rotate, text);

		private bool TryResolve()
		{
			try {
				numerical = services.GetRequiredKeyedService<IOcrService>("Numerical");
				text = services.GetRequiredKeyedService<IOcrService>("Text");
				return true;
			}
			catch (Exception ex) {
				logger?.LogError(ex, "OCR is unavailable; axis values will have to be entered by hand.");
				return false;
			}
		}

		private string Read(Image<Rgba, byte> image, RectangleD roi, bool rotate, IOcrService ocr)
		{
			if (!IsAvailable || image is null || roi.Width < 1 || roi.Height < 1) {
				return null;
			}

			Image<Rgba, byte> region = null;
			try {
				region = imageService.CropImage(image, roi);
				if (rotate) {
					var rotated = imageService.RotateImage(region, 90);
					region.Dispose();
					region = rotated;
				}
				return ocr.Ocr(region);
			}
			catch (Exception ex) {
				logger?.LogWarning(ex, "OCR failed for region {Region}.", roi);
				return null;
			}
			finally {
				region?.Dispose();
			}
		}
	}
}
