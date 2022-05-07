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
		public AxisPageModel(Model model)
		{
			Model = model;
		}

		public Model Model { get; }

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
			var image = Model.InputImage;
			if (image is null) {
				return Page();
			}
			var rect = Methods.GetAxisLocation(image) ?? new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
			Model.Setting.AxisLocation = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
			return Page();
		}

		public IActionResult OnPost(int x, int y, int width, int height)
		{
			Model.Setting.AxisLocation = new Rectangle(x, y, width, height);
			return RedirectToPage("FilterPage");
		}
	}
}
