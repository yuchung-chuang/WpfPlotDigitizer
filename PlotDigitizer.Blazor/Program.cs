using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;

using PlotDigitizer.Blazor.Components;
using PlotDigitizer.Blazor.Models;
using PlotDigitizer.Blazor.Services;

using PlotDigitizer.Core;

var builder = WebApplication.CreateBuilder(args);

var sessionSection = builder.Configuration.GetSection(DigitizerSessionOptions.SectionName);
builder.Services.Configure<DigitizerSessionOptions>(sessionSection);

var sessionOptions = new DigitizerSessionOptions();
sessionSection.Bind(sessionOptions);

// Blazor Server keeps render state in the SignalR circuit, but digitization state must survive a
// dropped/reconnecting circuit (backgrounded tab, brief network blip). It stays exactly where the
// Razor Pages app keeps it: an in-memory cache keyed by the ASP.NET Core session cookie, resolved
// independently of any single circuit. See Services/CircuitSessionKey.cs for how a component gets
// at that cookie despite HttpContext not being reliably available once a circuit is interactive.
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = sessionOptions.IdleTimeout;
	options.Cookie.Name = ".PlotDigitizer.Blazor.Session";
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
});

builder.Services
	.AddDigitizerServices()
	.AddSessionScopedModel()
	.AddSingleton<DigitizerStateManager>()
	.AddScoped<CircuitSessionKey>()
	.AddScoped<IDigitizerStateAccessor, DigitizerStateAccessor>()
	.AddScoped<ImageSourceService>();

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAntiforgery();

// Images are served as real png responses over a plain HTTP GET, the same way the Razor Pages
// image endpoint works, rather than pushing byte arrays through the SignalR circuit. This request
// has an ordinary HttpContext (it never goes through Blazor's component lifecycle), so the session
// cookie can be read directly without the CircuitSessionKey indirection components need.
app.MapGet("/image/{kind}", (string kind, HttpContext context, DigitizerStateManager manager) =>
{
	var state = manager.GetOrCreate(context.Session.Id);
	var model = state.Model;

	Image<Rgba, byte>? image = kind switch
	{
		ImageKind.Input => model.InputImage,
		ImageKind.Cropped => model.CroppedImage,
		ImageKind.Filtered => model.FilteredImage,
		_ => null,
	};

	if (image is null) {
		return Results.NotFound();
	}

	context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
	{
		Private = true,
		NoStore = true,
	};
	return Results.File(image.ToPng(), "image/png");
});

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
