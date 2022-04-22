using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;

using PlotDigitizer.Web.Models;

using System;
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


		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync(IFormFile formFile)
		{
			ImageModel.InputImage = await formFile.ToImageAsync();
			ImageModel.InputImageSource = await formFile.ToImgSrcAsync();
			return RedirectToPage("AxisLimitPage");
			//return Page();
		}
	}
}