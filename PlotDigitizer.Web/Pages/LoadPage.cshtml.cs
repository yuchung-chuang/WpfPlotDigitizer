using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.IO;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Pages
{
	public class LoadPageModel : PageModel
	{
		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync(IFormFile formFile)
		{
			var downloadPath = Path.GetTempFileName();
			using var stream = new FileStream(downloadPath, FileMode.OpenOrCreate);
			await formFile.CopyToAsync(stream);

			return RedirectToPage("AxisLimitPage");
		}
	}
}