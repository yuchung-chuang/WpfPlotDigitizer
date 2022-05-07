using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Web.Models;

namespace PlotDigitizer.Web.Pages
{
    public class EditPageModel : PageModel
    {
		public EditPageModel(Model model)
		{
			Model = model;
		}

		public Model Model { get; }

		public void OnGet()
        {
        }

		public IActionResult OnGetView()
		{
			return Partial("_EditPageView", Model);
		}
    }
}
