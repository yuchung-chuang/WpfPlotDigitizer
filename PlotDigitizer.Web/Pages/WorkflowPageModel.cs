using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Services;

using Model = PlotDigitizer.Web.Models.Model;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Base for the six workflow pages. It resolves the caller's own digitization state and
	/// enforces the same step order as the desktop <see cref="PageService"/>, so a user who deep
	/// links into a step they cannot enter yet is bounced to the furthest step they can.
	/// </summary>
	public abstract class WorkflowPageModel : PageModel
	{
		protected WorkflowPageModel(IDigitizerStateAccessor stateAccessor, WorkflowService workflow)
		{
			StateAccessor = stateAccessor;
			Workflow = workflow;
		}

		protected IDigitizerStateAccessor StateAccessor { get; }

		protected WorkflowService Workflow { get; }

		protected DigitizerState State => StateAccessor.State;

		public Model Model => StateAccessor.Model;

		public Setting Setting => StateAccessor.Setting;

		public abstract WorkflowStep Step { get; }

		public WorkflowStepInfo StepInfo => WorkflowService.Get(Step);

		public WorkflowStepInfo PreviousStep => WorkflowService.Previous(Step);

		public WorkflowStepInfo NextStep => WorkflowService.Next(Step);

		public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
		{
			ViewData["CurrentStep"] = Step;

			if (!Workflow.CanEnter(Step, Model)) {
				var target = WorkflowService.Get(Workflow.FurthestAvailable(Model));
				TempData["StepBlocked"] = StepInfo.BlockedReason;
				context.Result = RedirectToPage(target.PageName);
			}

			base.OnPageHandlerExecuting(context);
		}

		protected IActionResult RedirectToNextStep() =>
			NextStep is null ? RedirectToPage(StepInfo.PageName) : RedirectToPage(NextStep.PageName);
	}
}
