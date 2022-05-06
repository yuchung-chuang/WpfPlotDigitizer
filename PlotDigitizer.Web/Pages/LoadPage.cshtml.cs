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
		public Model Model { get; }
		public LoadPageModel(Model model)
		{
			Model = model;
		}

		[BindProperty, Display(Name = "File")]
		public IFormFile UploadedFile { get; set; }

		public IActionResult OnGetImage()
		{
			return Partial("_LoadPageView", Model.InputImageSource);
		}

		public async Task<IActionResult> OnPostAsync()
		{
			Model.InputImage = await UploadedFile.ToImageAsync();
			return RedirectToPage("AxisLimitPage");
		}
	}
}