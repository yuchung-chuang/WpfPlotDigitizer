using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Models;
using PlotDigitizer.Web.Services;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Serves the digitized images as real png responses instead of inlining them into the html as
	/// base64 <c>data:</c> uris. The url carries a version token that changes whenever the
	/// underlying node is updated or invalidated, so responses can be cached aggressively while
	/// never going stale.
	/// </summary>
	public class ImageModel : PageModel
	{
		private readonly IDigitizerStateAccessor stateAccessor;

		public ImageModel(IDigitizerStateAccessor stateAccessor)
		{
			this.stateAccessor = stateAccessor;
		}

		public IActionResult OnGet(string kind)
		{
			var state = stateAccessor.State;
			var model = state.Model;

			var image = kind switch
			{
				ImageKind.Input => model.InputImage,
				ImageKind.Cropped => WorkflowService.HasAxisLocation(model.Setting) ? model.CroppedImage : null,
				ImageKind.Filtered => WorkflowService.HasAxisLocation(model.Setting) ? model.FilteredImage : null,
				ImageKind.Edited => WorkflowService.HasAxisLocation(model.Setting) ? model.EditedImage : null,
				ImageKind.Editor => state.EditService.IsInitialised ? state.EditService.CurrentObject : null,
				ImageKind.Preview => BuildPreview(state),
				_ => null,
			};

			if (image is null) {
				return NotFound();
			}

			var bytes = image.ToPng();

			// Private: the image belongs to one session and must never be held by a shared cache.
			Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
			{
				Private = true,
				NoStore = kind is ImageKind.Editor or ImageKind.Preview,
				MaxAge = kind is ImageKind.Editor or ImageKind.Preview ? null : System.TimeSpan.FromHours(1),
			};

			return File(bytes, "image/png");
		}

		/// <summary>
		/// The data page preview: the edited image with the extracted points marked, exactly as
		/// <c>DataPageViewModel.UpdatePreviewImage</c> builds it.
		/// </summary>
		private static Emgu.CV.Image<Emgu.CV.Structure.Rgba, byte> BuildPreview(DigitizerState state)
		{
			var model = state.Model;
			if (!WorkflowService.HasAxisLocation(model.Setting) || model.EditedImage is null) {
				return null;
			}

			var preview = model.EditedImage.Copy();
			var points = model.DataPoints;
			if (points is null) {
				return preview;
			}

			if (model.Setting.DataType == DataType.Discrete) {
				preview.DrawDiscreteMarkers(points);
			}
			else {
				preview.DrawContinuousMarkers(points);
			}
			return preview;
		}
	}
}
