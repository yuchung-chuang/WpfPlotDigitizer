using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PlotDigitizer.Web.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
		}

		public string Message { get; set; }

		public void OnGet()
		{
			Message = "OnGet executed";
		}

		public void OnPost()
		{
			Message = "OnPost executed";
		}

		public void OnPostType1() => Message = "Type1 posted";
		public void OnPostType2() => Message = "Type2 posted";

		//public int OnPost(int number1, int number2) => number1 + number2;
	}
}