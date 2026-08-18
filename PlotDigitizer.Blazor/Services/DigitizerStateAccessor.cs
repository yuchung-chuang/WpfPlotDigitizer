using PlotDigitizer.Core;

using System;

using Model = PlotDigitizer.Blazor.Models.Model;

namespace PlotDigitizer.Blazor.Services
{
	/// <summary>
	/// Resolves the calling browser's digitization state. Components depend on this rather than on
	/// <see cref="Model"/> directly, because the model lives in the session scope rather than in
	/// the circuit's own DI scope.
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
		private readonly CircuitSessionKey sessionKey;
		private readonly DigitizerStateManager manager;
		private DigitizerState? state;

		public DigitizerStateAccessor(CircuitSessionKey sessionKey, DigitizerStateManager manager)
		{
			this.sessionKey = sessionKey;
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

		private string SessionId =>
			sessionKey.Value
			?? throw new InvalidOperationException(
				"No session id has been captured yet. Components must render under the App root " +
				"component before resolving digitization state.");
	}
}
