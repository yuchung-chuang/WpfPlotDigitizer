namespace PlotDigitizer.Core.Tests
{
	/// <summary>
	/// Tracks Enter/Leave calls and exposes counters so page lifecycle tests can verify calls.
	/// </summary>
	internal sealed class StubPage : ViewModelBase
	{
		public int EnterCount { get; private set; }
		public int LeaveCount { get; private set; }

		public override void Enter() => EnterCount++;
		public override void Leave() => LeaveCount++;
	}
}
