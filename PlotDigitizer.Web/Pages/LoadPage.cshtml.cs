using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Core;

using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Pages
{
	public class LoadPageModel : PageModel
	{
		private readonly Model model;

		public LoadPageModel(Model model)
		{
			this.model = model;
		}
		public void OnGet()
		{
		}

		public string ImageSource { get; set; }
		public async Task<IActionResult> OnPostAsync(IFormFile formFile)
		{
			//using var stream = new MemoryStream();
			//await formFile.CopyToAsync(stream);
			//var image = (Image.FromStream(stream) as Bitmap).ToImage<Rgba, byte>();
			//model.InputImage = image;
			//return RedirectToPage("AxisLimitPage");

			var path = Path.GetTempFileName();
			using (var fileStream = new FileStream(path, FileMode.OpenOrCreate)) {
				await formFile.CopyToAsync(fileStream);
			}
			using (var fileStream = new FileStream(path, FileMode.Open)) {
				using var memoryStream = new MemoryStream();
				await fileStream.CopyToAsync(memoryStream);
				var bytes = memoryStream.ToArray();
				var base64 = Convert.ToBase64String(bytes);
				var mimeType = "image/png";
				ImageSource = $"data:{mimeType};base64,{base64}";
			}
			return Page();
		}
	}
}