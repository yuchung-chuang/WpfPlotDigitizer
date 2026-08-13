using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;

using PlotDigitizer.Core;

using System;
using System.Collections.Generic;

using Model = PlotDigitizer.Web.Models.Model;

namespace PlotDigitizer.Web.Services
{
	/// <summary>
	/// One user's digitization state: a private instance of the whole node graph, the
	/// <see cref="Core.Setting"/>, and the editor history.
	///
	/// The desktop app can afford a single global graph because it only ever serves one user. On a
	/// server every caller needs their own, so each state owns a dependency injection scope in
	/// which the nodes are registered as scoped services.
	/// </summary>
	public sealed class DigitizerState : IDisposable
	{
		private readonly IServiceScope scope;
		private bool editorStale = true;
		private int committedEditIndex = -1;
		private bool disposed;

		public DigitizerState(IServiceScopeFactory scopeFactory)
		{
			scope = scopeFactory.CreateScope();
			Model = scope.ServiceProvider.GetRequiredService<Model>();
			Setting = Model.Setting;
			EditService = scope.ServiceProvider.GetRequiredService<IEditService<Image<Rgba, byte>>>();

			// The desktop app throws away the editor scope when the filtered image changes; an undo
			// history built on a previous filter is meaningless afterwards.
			Model.PropertyOutdated += OnModelPropertyOutdated;
		}

		public Model Model { get; }

		public Setting Setting { get; }

		public IEditService<Image<Rgba, byte>> EditService { get; }

		/// <summary>
		/// Initialises the editor from the current filtered image, or re-initialises it when the
		/// filtered image has changed since the history was built.
		/// </summary>
		public void EnsureEditorInitialised()
		{
			var filtered = Model.FilteredImage;
			if (filtered is null) {
				return;
			}

			if (EditService.IsInitialised && !editorStale) {
				return;
			}

			DisposeEditHistory();
			EditService.Initialise(filtered.Copy());
			editorStale = false;
			committedEditIndex = -1;
		}

		/// <summary>
		/// Publishes the editor's current image to the model, the web equivalent of
		/// <c>EditPageViewModel.Leave</c>. Skipped when nothing changed so that downstream point
		/// extraction is not re-run needlessly.
		/// </summary>
		public void CommitEdits()
		{
			if (!EditService.IsInitialised || editorStale) {
				return;
			}

			if (committedEditIndex == EditService.Index) {
				return;
			}

			Model.EdittedImage = EditService.CurrentObject.Copy();
			committedEditIndex = EditService.Index;
		}

		private void OnModelPropertyOutdated(object sender, string propertyName)
		{
			if (propertyName == nameof(Model.FilteredImage)) {
				editorStale = true;
			}
		}

		private void DisposeEditHistory()
		{
			if (!EditService.IsInitialised) {
				return;
			}

			foreach (var image in EditService.ObjectList) {
				image?.Dispose();
			}
		}

		public void Dispose()
		{
			if (disposed) {
				return;
			}
			disposed = true;

			Model.PropertyOutdated -= OnModelPropertyOutdated;

			// Emgu images wrap unmanaged buffers. Release them eagerly rather than waiting for a
			// finalizer, otherwise an idle server keeps hundreds of megabytes alive.
			DisposeEditHistory();
			foreach (var image in GetCachedImages()) {
				image?.Dispose();
			}

			scope.Dispose();
		}

		/// <summary>
		/// Reads the images already cached in the graph. Deliberately uses <c>Data</c> rather than
		/// <c>GetUpdatedData</c> so that disposal never triggers a recomputation.
		/// </summary>
		private IEnumerable<Image<Rgba, byte>> GetCachedImages()
		{
			yield return scope.ServiceProvider.GetRequiredService<InputImageNode>().Data;
			yield return scope.ServiceProvider.GetRequiredService<CroppedImageNode>().Data;
			yield return scope.ServiceProvider.GetRequiredService<FilteredImageNode>().Data;
			yield return scope.ServiceProvider.GetRequiredService<EdittedImageNode>().Data;
		}
	}
}
