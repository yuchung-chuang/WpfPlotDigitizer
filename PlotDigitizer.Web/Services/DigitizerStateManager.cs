using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;

namespace PlotDigitizer.Web.Services
{
	/// <summary>
	/// Keeps one <see cref="DigitizerState"/> per browser session. Entries expire after an idle
	/// period and are capped in number, and eviction disposes the images the state holds.
	/// </summary>
	public sealed class DigitizerStateManager : IDisposable
	{
		private readonly IServiceScopeFactory scopeFactory;
		private readonly DigitizerSessionOptions options;
		private readonly ILogger<DigitizerStateManager> logger;
		private readonly MemoryCache cache;
		private readonly object gate = new();
		private bool disposed;

		public DigitizerStateManager(IServiceScopeFactory scopeFactory,
			IOptions<DigitizerSessionOptions> options,
			ILogger<DigitizerStateManager> logger)
		{
			this.scopeFactory = scopeFactory;
			this.options = options.Value;
			this.logger = logger;

			cache = new MemoryCache(new MemoryCacheOptions
			{
				SizeLimit = Math.Max(1, this.options.MaxConcurrentSessions),
			});
		}

		public DigitizerState GetOrCreate(string sessionId)
		{
			if (string.IsNullOrEmpty(sessionId)) {
				throw new ArgumentException("A session id is required.", nameof(sessionId));
			}

			if (cache.TryGetValue<DigitizerState>(sessionId, out var existing)) {
				return existing;
			}

			// Creating a state builds a whole node graph, so serialise creation to avoid two
			// concurrent requests from the same browser each building one.
			lock (gate) {
				if (cache.TryGetValue(sessionId, out existing)) {
					return existing;
				}

				var state = new DigitizerState(scopeFactory);
				var entryOptions = new MemoryCacheEntryOptions
				{
					Size = 1,
					SlidingExpiration = options.IdleTimeout,
				};
				entryOptions.RegisterPostEvictionCallback(OnEvicted);

				cache.Set(sessionId, state, entryOptions);
				logger?.LogInformation("Created digitization state for session {SessionId}.", sessionId);
				return state;
			}
		}

		public void Remove(string sessionId)
		{
			if (!string.IsNullOrEmpty(sessionId)) {
				cache.Remove(sessionId);
			}
		}

		private void OnEvicted(object key, object value, EvictionReason reason, object state)
		{
			(value as DigitizerState)?.Dispose();
			logger?.LogInformation("Evicted digitization state for session {SessionId} ({Reason}).", key, reason);
		}

		public void Dispose()
		{
			if (disposed) {
				return;
			}
			disposed = true;

			cache.Compact(1.0);
			cache.Dispose();
		}
	}
}
