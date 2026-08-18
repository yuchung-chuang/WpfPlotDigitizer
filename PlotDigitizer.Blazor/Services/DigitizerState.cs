using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;

using PlotDigitizer.Core;

using System;
using System.Collections.Generic;

using Model = PlotDigitizer.Blazor.Models.Model;

namespace PlotDigitizer.Blazor.Services
{
	/// <summary>
	/// One user's digitization state: a private instance of the whole node graph plus the
	/// <see cref="Core.Setting"/>.
	///
	/// Trimmed from <c>PlotDigitizer.Web.Services.DigitizerState</c>: this spike only exercises
	/// Load and Filter, so the editor lifecycle (<c>EnsureEditorInitialised</c>/<c>CommitEdits</c>)
	/// that the Edit page needs is left out entirely.
	///
	/// The desktop app can afford a single global graph because it only ever serves one user. On a
	/// server every caller needs their own, so each state owns a dependency injection scope in
	/// which the nodes are registered as scoped services.
	/// </summary>
	public sealed class DigitizerState : IDisposable
	{
		private readonly IServiceScope scope;
		private bool disposed;

		public DigitizerState(IServiceScopeFactory scopeFactory)
		{
			scope = scopeFactory.CreateScope();
			Model = scope.ServiceProvider.GetRequiredService<Model>();
			Setting = Model.Setting;
		}

		public Model Model { get; }

		public Setting Setting { get; }

		public void Dispose()
		{
			if (disposed) {
				return;
			}
			disposed = true;

			// Emgu images wrap unmanaged buffers. Release them eagerly rather than waiting for a
			// finalizer, otherwise an idle server keeps hundreds of megabytes alive.
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
			yield return scope.ServiceProvider.GetRequiredService<EditedImageNode>().Data;
		}
	}
}
