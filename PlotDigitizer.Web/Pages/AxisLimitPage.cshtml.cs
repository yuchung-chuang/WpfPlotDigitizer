using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Web.Models;

using System;
using System.IO;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Pages
{
	public class AxisLimitPageModel : PageModel
	{
		public Model Model { get; }
		public AxisLimitPageModel(Model model)
		{
			Model = model;
		}


		public void OnGet()
		{
		}

	}
}