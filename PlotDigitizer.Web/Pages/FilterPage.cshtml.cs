using Emgu.CV.Structure;

using Microsoft.AspNetCore.Mvc;

using PlotDigitizer.Web.Services;

using System;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Web counterpart of <c>FilterPageViewModel</c>: keep only the pixels whose colour falls in
	/// the chosen RGB range. The desktop range sliders become noUiSlider controls that post the
	/// bounds and refresh the preview panel.
	/// </summary>
	public class FilterPageModel : WorkflowPageModel
	{
		public FilterPageModel(IDigitizerStateAccessor stateAccessor, WorkflowService workflow)
			: base(stateAccessor, workflow)
		{
		}

		public override WorkflowStep Step => WorkflowStep.Filter;

		public void OnGet()
		{
		}

		/// <summary>Applies new bounds and returns just the preview panel.</summary>
		public IActionResult OnPostFilter(int minR, int maxR, int minG, int maxG, int minB, int maxB)
		{
			Setting.FilterMin = new Rgba(Clamp(minR), Clamp(minG), Clamp(minB), byte.MaxValue);
			Setting.FilterMax = new Rgba(Clamp(maxR), Clamp(maxG), Clamp(maxB), byte.MaxValue);
			return Partial("_FilterPageView", Model);
		}

		public IActionResult OnPost() => RedirectToNextStep();

		private static double Clamp(int value) => Math.Clamp(value, 0, byte.MaxValue);
	}
}
