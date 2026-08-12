using System;

namespace PlotDigitizer.Web.Services
{
	/// <summary>
	/// Bounds the memory used by per-user digitization state. Every live session holds several
	/// full size images plus an undo history, so both the idle timeout and the session cap matter.
	/// </summary>
	public class DigitizerSessionOptions
	{
		public const string SectionName = "DigitizerSession";

		/// <summary>
		/// How long a session may sit idle before its images are evicted and disposed.
		/// </summary>
		public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(30);

		/// <summary>
		/// Maximum number of concurrently retained sessions. The least recently used session is
		/// evicted once this is exceeded.
		/// </summary>
		public int MaxConcurrentSessions { get; set; } = 50;

		/// <summary>
		/// Maximum accepted upload size in bytes.
		/// </summary>
		public long MaxUploadBytes { get; set; } = 20 * 1024 * 1024;
	}
}
