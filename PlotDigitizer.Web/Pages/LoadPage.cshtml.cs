using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PlotDigitizer.Web.Services;

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Web counterpart of <c>LoadPageViewModel</c>. The desktop version offers a file dialog,
	/// clipboard paste, drag and drop and url download; the browser equivalents of all four post
	/// here.
	/// </summary>
	public class LoadPageModel : WorkflowPageModel
	{
		private readonly ImageSourceService imageSource;

		public LoadPageModel(IDigitizerStateAccessor stateAccessor,
			WorkflowService workflow,
			ImageSourceService imageSource)
			: base(stateAccessor, workflow)
		{
			this.imageSource = imageSource;
		}

		public override WorkflowStep Step => WorkflowStep.Load;

		[BindProperty, Display(Name = "Image file")]
		public IFormFile UploadedFile { get; set; }

		[BindProperty, Display(Name = "Image address")]
		public string ImageAddress { get; set; }

		/// <summary>A <c>data:</c> uri produced by a browser paste or bitmap drop.</summary>
		[BindProperty]
		public string PastedImage { get; set; }

		public string ErrorMessage { get; private set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostUploadAsync()
		{
			var result = await imageSource.FromUploadAsync(UploadedFile, HttpContext.RequestAborted);
			return Apply(result);
		}

		public async Task<IActionResult> OnPostAddressAsync()
		{
			var result = await imageSource.FromUrlAsync(ImageAddress, HttpContext.RequestAborted);
			return Apply(result);
		}

		public IActionResult OnPostPaste()
		{
			var result = imageSource.FromDataUrl(PastedImage);
			return Apply(result);
		}

		public IActionResult OnPostReset()
		{
			StateAccessor.Reset();
			return RedirectToPage();
		}

		private IActionResult Apply(ImageLoadResult result)
		{
			if (!result.IsValid) {
				ErrorMessage = result.Error;
				ModelState.AddModelError(string.Empty, result.Error);
				return Page();
			}

			SetInputImage(result.Image);

			// The desktop app advances automatically once an image is loaded.
			return RedirectToNextStep();
		}

		private void SetInputImage(Image<Rgba, byte> image)
		{
			// Every downstream node holds its own copy, so the replaced image can be released.
			var previous = Model.InputImage;
			Model.InputImage = image;
			if (!ReferenceEquals(previous, image)) {
				previous?.Dispose();
			}
		}
	}
}
