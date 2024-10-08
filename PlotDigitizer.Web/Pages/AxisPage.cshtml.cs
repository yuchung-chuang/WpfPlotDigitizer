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
		private readonly IImageService imageService;

		public AxisPageModel(Model model, IImageService imageService)
		{
			Model = model;
			this.imageService = imageService;
		}

		public Model Model { get; }

		public void OnGet()
		{
			if (Model.Setting.AxisLocation == default) {
				OnGetAxisLocation();
			}
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
			Model.Setting.AxisLocation = imageService.GetAxisLocation(image);
			return Page();
		}

		public IActionResult OnPost(int x, int y, int width, int height)
		{
			Model.Setting.AxisLocation = new RectangleD(x, y, width, height);
			return RedirectToPage("FilterPage");
		}
	}
}
