using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Services;

using System.Collections.Generic;
using System.Linq;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Web counterpart of <c>EditPageViewModel</c> and the WPF <c>Editor</c> control. The drawing
	/// itself happens on a canvas in the browser, but the undo history stays on the server in the
	/// shared <see cref="IEditService{TObject}"/>, so both frontends behave identically.
	/// </summary>
	public class EditPageModel : WorkflowPageModel
	{
		private readonly IImageService imageService;
		private readonly ImageSourceService imageSource;
		private readonly ILogger<EditPageModel> logger;

		public EditPageModel(IDigitizerStateAccessor stateAccessor,
			WorkflowService workflow,
			IImageService imageService,
			ImageSourceService imageSource,
			ILogger<EditPageModel> logger)
			: base(stateAccessor, workflow)
		{
			this.imageService = imageService;
			this.imageSource = imageSource;
			this.logger = logger;
		}

		public override WorkflowStep Step => WorkflowStep.Edit;

		public IEditService<Image<Rgba, byte>> EditService => State.EditService;

		/// <summary>Tags from the current position backwards, newest first.</summary>
		public IReadOnlyList<string> UndoList => EditService.IsInitialised
			? Enumerable.Range(0, EditService.Index + 1)
				.Select(offset => EditService.TagList[EditService.Index - offset])
				.ToList()
			: [];

		/// <summary>Tags from the current position forwards.</summary>
		public IReadOnlyList<string> RedoList => EditService.IsInitialised
			? Enumerable.Range(EditService.Index, EditService.TagList.Count - EditService.Index)
				.Select(index => EditService.TagList[index])
				.ToList()
			: [];

		public string ErrorMessage { get; private set; }

		public void OnGet() => State.EnsureEditorInitialised();

		/// <summary>
		/// Receives the canvas contents after a completed stroke or region delete. The browser
		/// sends the whole edited image so the server never has to reimplement the drawing.
		/// </summary>
		public IActionResult OnPostCommit(string image, string tag)
		{
			State.EnsureEditorInitialised();
			if (!EditService.IsInitialised) {
				return Panel();
			}

			var result = imageSource.FromDataUrl(image);
			if (!result.IsValid) {
				ErrorMessage = result.Error;
				return Panel();
			}

			var current = EditService.CurrentObject;
			if (result.Image.Width != current.Width || result.Image.Height != current.Height) {
				result.Image.Dispose();
				ErrorMessage = "The edit did not match the image being edited. The editor has been refreshed.";
				logger?.LogWarning("Rejected an edit whose size did not match the edited image.");
				return Panel();
			}

			var edit = (result.Image, string.IsNullOrWhiteSpace(tag) ? "edit image" : tag);
			if (EditService.CanEdit(edit)) {
				EditService.Edit(edit);
			}
			else {
				result.Image.Dispose();
			}

			return Panel();
		}

		public IActionResult OnPostClearBorder()
		{
			State.EnsureEditorInitialised();
			if (!EditService.IsInitialised) {
				return Panel();
			}

			var cleared = imageService.ClearBorder(EditService.CurrentObject);
			var edit = (cleared, "Clear Border");
			if (EditService.CanEdit(edit)) {
				EditService.Edit(edit);
			}
			return Panel();
		}

		public IActionResult OnPostUndo()
		{
			State.EnsureEditorInitialised();
			if (EditService.CanUndo()) {
				EditService.Undo();
			}
			return Panel();
		}

		public IActionResult OnPostRedo()
		{
			State.EnsureEditorInitialised();
			if (EditService.CanRedo()) {
				EditService.Redo();
			}
			return Panel();
		}

		/// <summary>Steps back the given number of entries, as the WPF undo history dropdown does.</summary>
		public IActionResult OnPostUndoTo(int steps)
		{
			State.EnsureEditorInitialised();
			GoTo(EditService.Index - steps, steps);
			return Panel();
		}

		public IActionResult OnPostRedoTo(int steps)
		{
			State.EnsureEditorInitialised();
			GoTo(EditService.Index + steps, steps);
			return Panel();
		}

		public IActionResult OnPost()
		{
			State.EnsureEditorInitialised();
			State.CommitEdits();
			return RedirectToNextStep();
		}

		private void GoTo(int targetIndex, int steps)
		{
			if (steps > 0 && EditService.CanGoTo(targetIndex)) {
				EditService.GoTo(targetIndex);
			}
		}

		private IActionResult Panel() => Partial("_EditPageView", this);
	}
}
