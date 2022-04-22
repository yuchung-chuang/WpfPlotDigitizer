using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Core;

using System;
using System.IO;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Pages
{
	public class AxisLimitPageModel : PageModel
	{
		private readonly Model model;

		
		public string ImageSource
		{
			get
			{
				var mimeType = "image/png";
				using var stream = new MemoryStream(model.InputImage.Bytes);
				var base64 = Convert.ToBase64String(stream.ToArray());
				return $"data:image/png;base64,{base64}";
			}
		}

		public AxisLimitPageModel(Model model)
		{
			this.model = model;
		}

		public void OnGet()
		{
		}

		public IActionResult GetImage() => File(model.InputImage.Bytes, "image/png");
	}
}