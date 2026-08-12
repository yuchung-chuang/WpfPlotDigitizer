using Microsoft.AspNetCore.Http;

using PlotDigitizer.Core;

using System;

using Model = PlotDigitizer.Web.Models.Model;

namespace PlotDigitizer.Web.Services
{
	/// <summary>
	/// Resolves the calling browser's digitization state. Page models depend on this rather than on
	/// <see cref="Model"/> directly, because the model lives in the session scope rather than in
	/// the request scope.
	/// </summary>
	public interface IDigitizerStateAccessor
	{
		DigitizerState State { get; }

		Model Model { get; }

		Setting Setting { get; }

		void Reset();
	}

	public sealed class DigitizerStateAccessor : IDigitizerStateAccessor
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly DigitizerStateManager manager;
		private DigitizerState state;

		public DigitizerStateAccessor(IHttpContextAccessor httpContextAccessor, DigitizerStateManager manager)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.manager = manager;
		}

		public DigitizerState State => state ??= manager.GetOrCreate(SessionId);

		public Model Model => State.Model;

		public Setting Setting => State.Setting;

		/// <summary>
		/// Discards everything the caller has digitized so far and starts a fresh graph.
		/// </summary>
		public void Reset()
		{
			manager.Remove(SessionId);
			state = null;
		}

		private string SessionId
		{
			get {
				var context = httpContextAccessor.HttpContext
					?? throw new InvalidOperationException("Digitization state is only available inside a request.");
				return context.Session.Id;
			}
		}
	}
}
