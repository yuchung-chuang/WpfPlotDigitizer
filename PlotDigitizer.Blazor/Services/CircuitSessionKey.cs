namespace PlotDigitizer.Blazor.Services
{
	/// <summary>
	/// Carries the ASP.NET Core session cookie id from the request that first rendered the page
	/// into the rest of the Blazor circuit's lifetime.
	///
	/// This exists because of a Blazor Server-specific gotcha: <c>Routes</c> (the component
	/// actually marked with an interactive render mode, not the static <c>App</c> host document)
	/// runs twice, in two <em>different</em> dependency injection scopes. The first pass is a
	/// static prerender that happens as part of an ordinary HTTP request, where
	/// <c>IHttpContextAccessor.HttpContext</c> is real. The second pass starts once the browser
	/// opens the SignalR circuit; nothing in that pass has an HTTP request behind it, so
	/// <c>HttpContext</c> is never available there, and a scoped service set during the first pass
	/// is a different instance from the one the second pass resolves.
	///
	/// <see cref="Components.Routes"/> bridges the two passes with <c>PersistentComponentState</c>
	/// (the framework's supported mechanism for exactly this handoff): it captures the session id
	/// during the static pass and republishes it into this scoped service once the interactive
	/// pass starts. Every other component in the circuit then reads <see cref="Value"/> instead of
	/// touching <c>HttpContext</c> directly.
	/// </summary>
	public sealed class CircuitSessionKey
	{
		public string? Value { get; set; }
	}
}
