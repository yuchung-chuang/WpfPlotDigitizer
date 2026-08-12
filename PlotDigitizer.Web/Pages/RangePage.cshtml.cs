using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Models;
using PlotDigitizer.Web.Services;

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Web counterpart of <c>RangePageViewModel</c>: calibrate the axes. The six selection boxes
	/// mark where the tick labels and axis titles are, and "Detect" runs the same Core OCR the
	/// desktop app uses to read them.
	/// </summary>
	public class RangePageModel : WorkflowPageModel
	{
		private readonly IImageService imageService;
		private readonly AxisOcrReader ocr;
		private readonly ILogger<RangePageModel> logger;

		public RangePageModel(IDigitizerStateAccessor stateAccessor,
			WorkflowService workflow,
			IImageService imageService,
			AxisOcrReader ocr,
			ILogger<RangePageModel> logger)
			: base(stateAccessor, workflow)
		{
			this.imageService = imageService;
			this.ocr = ocr;
			this.logger = logger;
		}

		public override WorkflowStep Step => WorkflowStep.Range;

		[BindProperty, Required, Display(Name = "X min")]
		public double? XMin { get; set; }

		[BindProperty, Required, Display(Name = "X max")]
		public double? XMax { get; set; }

		[BindProperty, Display(Name = "X log base")]
		public double? XLog { get; set; }

		[BindProperty, Required, Display(Name = "Y min")]
		public double? YMin { get; set; }

		[BindProperty, Required, Display(Name = "Y max")]
		public double? YMax { get; set; }

		[BindProperty, Display(Name = "Y log base")]
		public double? YLog { get; set; }

		[BindProperty, Display(Name = "X label")]
		public string XLabel { get; set; }

		[BindProperty, Display(Name = "Y label")]
		public string YLabel { get; set; }

		[BindProperty]
		public RectangleInput XMinBox { get; set; } = new();

		[BindProperty]
		public RectangleInput XMaxBox { get; set; } = new();

		[BindProperty]
		public RectangleInput YMinBox { get; set; } = new();

		[BindProperty]
		public RectangleInput YMaxBox { get; set; } = new();

		[BindProperty]
		public RectangleInput XLabelBox { get; set; } = new();

		[BindProperty]
		public RectangleInput YLabelBox { get; set; } = new();

		public string DetectionMessage { get; private set; }

		public void OnGet()
		{
			if (Setting.AxisTextBox == default) {
				DetectTextBoxes();
				RunOcr();
			}
			else {
				LoadFromSetting();
			}
		}

		/// <summary>Re-runs OCR over the boxes as the user has positioned them.</summary>
		public IActionResult OnPostDetect()
		{
			// The values read from the image replace whatever is in the form, so clear the
			// validation state that was gathered for the previous values.
			ModelState.Clear();

			if (AllBoxesEmpty()) {
				DetectTextBoxes();
			}

			RunOcr();
			return Page();
		}

		public IActionResult OnPostAutoDetect()
		{
			ModelState.Clear();
			DetectTextBoxes();
			RunOcr();
			return Page();
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid) {
				return Page();
			}

			if (XMin == XMax || YMin == YMax) {
				ModelState.AddModelError(string.Empty, "The minimum and maximum of an axis must differ.");
				return Page();
			}

			SaveToSetting();
			return RedirectToNextStep();
		}

		private bool AllBoxesEmpty() =>
			XMinBox.IsEmpty && XMaxBox.IsEmpty && YMinBox.IsEmpty && YMaxBox.IsEmpty
			&& XLabelBox.IsEmpty && YLabelBox.IsEmpty;

		private void LoadFromSetting()
		{
			var boxes = Setting.AxisTextBox;
			XMinBox = RectangleInput.From(boxes.XMin);
			XMaxBox = RectangleInput.From(boxes.XMax);
			YMinBox = RectangleInput.From(boxes.YMin);
			YMaxBox = RectangleInput.From(boxes.YMax);
			XLabelBox = RectangleInput.From(boxes.XLabel);
			YLabelBox = RectangleInput.From(boxes.YLabel);

			var limit = Setting.AxisLimit;
			XMin = Sanitize(limit.Left);
			XMax = Sanitize(limit.Right);
			YMin = Sanitize(limit.Top);
			YMax = Sanitize(limit.Bottom);

			XLog = Sanitize(Setting.AxisLogBase.X);
			YLog = Sanitize(Setting.AxisLogBase.Y);

			XLabel = Setting.AxisTitle.XLabel;
			YLabel = Setting.AxisTitle.YLabel;
		}

		private void SaveToSetting()
		{
			Setting.AxisTextBox = new AxisLimitTextBoxD
			{
				XMin = XMinBox.ToRectangleD(),
				XMax = XMaxBox.ToRectangleD(),
				YMin = YMinBox.ToRectangleD(),
				YMax = YMaxBox.ToRectangleD(),
				XLabel = XLabelBox.ToRectangleD(),
				YLabel = YLabelBox.ToRectangleD(),
			};

			Setting.AxisLimit = new RectangleD(
				XMin ?? 0,
				YMin ?? 0,
				(XMax ?? 0) - (XMin ?? 0),
				(YMax ?? 0) - (YMin ?? 0));

			Setting.AxisLogBase = new PointD(XLog ?? double.NaN, YLog ?? double.NaN);
			Setting.AxisTitle = new AxisTitle(XLabel, YLabel);
		}

		private void DetectTextBoxes()
		{
			var image = Model.InputImage;
			if (image is null) {
				return;
			}

			try {
				var detected = imageService.GetAxisTextBox(image, Setting.AxisLocation);
				XMinBox = RectangleInput.From(new RectangleD(detected.XMin));
				XMaxBox = RectangleInput.From(new RectangleD(detected.XMax));
				YMinBox = RectangleInput.From(new RectangleD(detected.YMin));
				YMaxBox = RectangleInput.From(new RectangleD(detected.YMax));
				XLabelBox = RectangleInput.From(new RectangleD(detected.XLabel));
				YLabelBox = RectangleInput.From(new RectangleD(detected.YLabel));
			}
			catch (Exception ex) {
				logger?.LogError(ex, "Failed to detect the axis text boxes.");
				DetectionMessage = "The tick labels could not be located automatically. Position the boxes by hand.";
			}
		}

		/// <summary>
		/// Mirrors <c>RangePageViewModel.Ocr</c>: read each box and keep the value only when it
		/// parses, so a failed read never wipes out something the user typed.
		/// </summary>
		private void RunOcr()
		{
			var image = Model.InputImage;
			if (image is null) {
				return;
			}

			if (!ocr.IsAvailable) {
				DetectionMessage = "Text recognition is not available on this server. Enter the axis values by hand.";
				return;
			}

			var read = 0;
			read += ReadNumber(image, XMinBox, value => XMin = value);
			read += ReadNumber(image, XMaxBox, value => XMax = value);
			read += ReadNumber(image, YMinBox, value => YMin = value);
			read += ReadNumber(image, YMaxBox, value => YMax = value);
			read += ReadText(image, XLabelBox, false, value => XLabel = value);
			read += ReadText(image, YLabelBox, true, value => YLabel = value);

			if (read == 0 && DetectionMessage is null) {
				DetectionMessage = "Nothing could be read from the boxes. Adjust them and try again, or type the values in.";
			}
		}

		private int ReadNumber(Image<Rgba, byte> image, RectangleInput box, Action<double> assign)
		{
			var text = ocr.ReadNumber(image, box.ToRectangleD());
			if (text is not null
				&& double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) {
				assign(value);
				return 1;
			}
			return 0;
		}

		private int ReadText(Image<Rgba, byte> image, RectangleInput box, bool rotate, Action<string> assign)
		{
			var text = ocr.ReadText(image, box.ToRectangleD(), rotate)?.Trim();
			if (!string.IsNullOrEmpty(text)) {
				assign(text);
				return 1;
			}
			return 0;
		}

		private static double? Sanitize(double value) =>
			double.IsNaN(value) || double.IsInfinity(value) ? null : value;
	}
}

