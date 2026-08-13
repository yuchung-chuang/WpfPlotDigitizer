using PlotDigitizer.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using Model = PlotDigitizer.Web.Models.Model;

namespace PlotDigitizer.Web.Services
{
	public enum WorkflowStep
	{
		Load = 0,
		Axis = 1,
		Range = 2,
		Filter = 3,
		Edit = 4,
		Data = 5,
	}

	public sealed class WorkflowStepInfo
	{
		public WorkflowStepInfo(WorkflowStep step, string pageName, string title, string blockedReason)
		{
			Step = step;
			PageName = pageName;
			Title = title;
			BlockedReason = blockedReason;
		}

		public WorkflowStep Step { get; }

		/// <summary>Razor page name, as used by <c>asp-page</c> and <c>RedirectToPage</c>.</summary>
		public string PageName { get; }

		public string Title { get; }

		/// <summary>Message shown when the user reaches the step too early.</summary>
		public string BlockedReason { get; }
	}

	/// <summary>
	/// The web counterpart of <see cref="PageService"/>: it fixes the same step order as the
	/// desktop app and decides which steps the current state allows.
	/// </summary>
	public sealed class WorkflowService
	{
		public static IReadOnlyList<WorkflowStepInfo> Steps { get; } = new[]
		{
			new WorkflowStepInfo(WorkflowStep.Load, "/LoadPage", "Load", "Load an image first."),
			new WorkflowStepInfo(WorkflowStep.Axis, "/AxisPage", "Axis", "Load an image first."),
			new WorkflowStepInfo(WorkflowStep.Range, "/RangePage", "Range", "Set the axis location first."),
			new WorkflowStepInfo(WorkflowStep.Filter, "/FilterPage", "Filter", "Set the axis location first."),
			new WorkflowStepInfo(WorkflowStep.Edit, "/EditPage", "Edit", "Set the axis location first."),
			new WorkflowStepInfo(WorkflowStep.Data, "/DataPage", "Data", "Set the axis location first."),
		};

		public static WorkflowStepInfo Get(WorkflowStep step) => Steps[(int)step];

		public static WorkflowStepInfo Previous(WorkflowStep step) =>
			step == WorkflowStep.Load ? null : Steps[(int)step - 1];

		public static WorkflowStepInfo Next(WorkflowStep step) =>
			step == WorkflowStep.Data ? null : Steps[(int)step + 1];

		/// <summary>
		/// An axis location of zero size means the user has not chosen one yet, and cropping with it
		/// throws inside <see cref="IImageService.CropImage"/>.
		/// </summary>
		public static bool HasAxisLocation(Setting setting) =>
			setting is not null
			&& setting.AxisLocation.Width >= 1
			&& setting.AxisLocation.Height >= 1;

		public static bool HasAxisLimit(Setting setting) =>
			setting is not null
			&& !double.IsNaN(setting.AxisLimit.Width)
			&& !double.IsNaN(setting.AxisLimit.Height)
			&& setting.AxisLimit.Width != 0
			&& setting.AxisLimit.Height != 0;

		public bool CanEnter(WorkflowStep step, Model model)
		{
			if (model is null) {
				return step == WorkflowStep.Load;
			}

			return step switch
			{
				WorkflowStep.Load => true,
				WorkflowStep.Axis => model.InputImage is not null,
				_ => model.InputImage is not null && HasAxisLocation(model.Setting),
			};
		}

		/// <summary>
		/// The furthest step the current state satisfies, used to bounce a user who deep links into
		/// a step they cannot enter yet.
		/// </summary>
		public WorkflowStep FurthestAvailable(Model model) =>
			Steps.Where(s => CanEnter(s.Step, model))
				.Select(s => s.Step)
				.DefaultIfEmpty(WorkflowStep.Load)
				.Max();
	}
}
