using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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