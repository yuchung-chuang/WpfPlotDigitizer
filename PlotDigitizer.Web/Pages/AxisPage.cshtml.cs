using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Models;

using Model = PlotDigitizer.Web.Models.Model;

namespace PlotDigitizer.Web.Pages
{
	public class AxisPageModel : PageModel
	{
		public AxisPageModel(Model model, Setting setting)
		{
			Model = model;
			Setting = setting;
		}

		public Model Model { get; }

		public Setting Setting { get; }

		public void OnGet()
		{
			OnGetAxisLocation();
		}

		public IActionResult OnGetView()
		{
			return Partial("_AxisPageView", Model);
		}
		public IActionResult OnGetAxisLocation()
		{
			var rect = Methods.GetAxisLocation(Model.InputImage);
			if (!rect.HasValue) {
				return Page();
			}
			Setting.AxisLocation = new Rectangle(rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height);
			return Page();
		}

		public IActionResult OnPost(int x, int y, int width, int height)
		{
			Setting.AxisLocation = new Rectangle(x, y, width, height);
			return RedirectToPage("FilterPage");
		}
	}
}
