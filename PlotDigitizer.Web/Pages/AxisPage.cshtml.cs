using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Services;

using System;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Web counterpart of <c>AxisPageViewModel</c>: choose the rectangle that bounds the plotting
	/// area, either by dragging it or by letting Core detect it.
	/// </summary>
	public class AxisPageModel : WorkflowPageModel
	{
		private readonly IImageService imageService;
		private readonly ILogger<AxisPageModel> logger;

		public AxisPageModel(IDigitizerStateAccessor stateAccessor,
			WorkflowService workflow,
			IImageService imageService,
			ILogger<AxisPageModel> logger)
			: base(stateAccessor, workflow)
		{
			this.imageService = imageService;
			this.logger = logger;
		}

		public override WorkflowStep Step => WorkflowStep.Axis;

		public void OnGet()
		{
			if (!WorkflowService.HasAxisLocation(Setting)) {
				Setting.AxisLocation = DetectAxisLocation();
			}
		}

		public IActionResult OnGetView() => Partial("_AxisPageView", Model);

		/// <summary>Re-runs automatic detection and returns the refreshed panel.</summary>
		public IActionResult OnGetDetect()
		{
			Setting.AxisLocation = DetectAxisLocation();
			return Partial("_AxisPageView", Model);
		}

		public IActionResult OnPost(double x, double y, double width, double height)
		{
			var image = Model.InputImage;
			var clamped = Clamp(new RectangleD(x, y, width, height), image.Width, image.Height);

			if (clamped.Width < 1 || clamped.Height < 1) {
				ModelState.AddModelError(string.Empty, "Draw a box around the plotting area before continuing.");
				return Page();
			}

			Setting.AxisLocation = clamped;
			return RedirectToNextStep();
		}

		private RectangleD DetectAxisLocation()
		{
			var image = Model.InputImage;
			if (image is null) {
				return default;
			}

			try {
				var detected = imageService.GetAxisLocation(image);
				var clamped = Clamp(detected, image.Width, image.Height);
				if (clamped.Width >= 1 && clamped.Height >= 1) {
					return clamped;
				}
				logger?.LogWarning("Axis detection returned an unusable rectangle {Rectangle}.", detected);
			}
			catch (Exception ex) {
				logger?.LogError(ex, "Failed to detect the axis location.");
			}

			// Same fallback as the desktop app: the middle half of the image.
			return new RectangleD(image.Width / 4d, image.Height / 4d, image.Width / 2d, image.Height / 2d);
		}

		private static RectangleD Clamp(RectangleD rect, double imageWidth, double imageHeight)
		{
			var left = Math.Clamp(rect.Left, 0, imageWidth);
			var top = Math.Clamp(rect.Top, 0, imageHeight);
			var width = Math.Clamp(rect.Width, 0, imageWidth - left);
			var height = Math.Clamp(rect.Height, 0, imageHeight - top);
			return new RectangleD(left, top, width, height);
		}
	}
}
