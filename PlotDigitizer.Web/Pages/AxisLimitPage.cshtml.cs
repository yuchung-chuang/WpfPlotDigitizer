using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Models;
using Model = PlotDigitizer.Web.Models.Model;

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Pages
{
	public class AxisLimitPageModel : PageModel
	{
		private readonly Setting setting;

		public Model Model { get; }
		public AxisLimitPageModel(Model model, Setting setting)
		{
			Model = model;
			this.setting = setting;
		}

		[BindProperty]
		public double? YMax { get; set; }
		[BindProperty]
		public double? YLog { get; set; }
		[BindProperty]
		public double? YMin { get; set; }
		[BindProperty]
		public double? XMax { get; set; }
		[BindProperty]
		public double? XLog { get; set; }
		[BindProperty]
		public double? XMin { get; set; }
		public IActionResult OnPost()
		{
			if (!ModelState.IsValid) {
				return Page();
			}
			setting.AxisLimit = new RectangleD(XMin ?? 0, YMin ?? 0, XMax ?? 0 - XMin ?? 0, YMax ?? 0 - YMin ?? 0);
			setting.AxisLogBase = new PointD(XLog ?? 0, YLog ?? 0);
			return RedirectToPage("AxisPage");
		}
		public void OnGet()
		{
		}

	}
}