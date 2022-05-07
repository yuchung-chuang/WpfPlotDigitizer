using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Emgu.CV.Structure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Web.Models;

namespace PlotDigitizer.Web.Pages
{
	public class FilterPageModel : PageModel
	{
		public FilterPageModel(Model model)
		{
			Model = model;
		}

		public Model Model { get; }

		public void OnGet()
		{
			
		}

		public IActionResult OnGetView()
		{
			return Partial("_FilterPageView", Model);
		}

		public IActionResult OnPost(int minR, int maxR, int minG, int maxG, int minB, int maxB)
		{
			Model.Setting.FilterMax = new Rgba(maxR, maxG, maxB, byte.MaxValue);
			Model.Setting.FilterMin = new Rgba(minR, minG, minB, byte.MaxValue);
			return RedirectToPage("EditPage");
		}
    }
}
