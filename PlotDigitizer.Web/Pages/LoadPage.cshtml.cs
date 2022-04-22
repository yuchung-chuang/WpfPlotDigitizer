using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;

using PlotDigitizer.Web.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Pages
{
	public class LoadPageModel : PageModel
	{
		public Model ImageModel { get; }
		public LoadPageModel(Model model)
		{
			ImageModel = model;
		}

		[BindProperty, Display(Name = "File")]
		public IFormFile UploadedFile { get; set; }

		public IActionResult OnGetImage()
		{
			return Partial("_LoadedImage", ImageModel.InputImageSource);
		}

		public async Task<IActionResult> OnPostAsync()
		{
			//ImageModel.InputImage = await formFile.ToImageAsync();
			//ImageModel.InputImageSource = await formFile.ToImgSrcAsync();
			//return RedirectToPage("AxisLimitPage");
			ImageModel.InputImage = await UploadedFile.ToImageAsync();
			ImageModel.InputImageSource = await UploadedFile.ToImgSrcAsync();
			return Page();
		}
	}
}