using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PlotDigitizer.Web.Services;

using System;

namespace PlotDigitizer.Web
{
	public class Startup
	{
		/// <summary>
		/// Written on every request so that the session cookie, and therefore the session id used
		/// to key digitization state, is stable for the caller.
		/// </summary>
		private const string SessionMarkerKey = "plotdigitizer";

		private readonly IWebHostEnvironment environment;

		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			this.environment = environment;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var sessionSection = Configuration.GetSection(DigitizerSessionOptions.SectionName);
			services.Configure<DigitizerSessionOptions>(sessionSection);

			var sessionOptions = new DigitizerSessionOptions();
			sessionSection.Bind(sessionOptions);

			var razorPages = services.AddRazorPages();
			if (environment.IsDevelopment()) {
				// Development convenience only; it must not ship in the production path.
				razorPages.AddRazorRuntimeCompilation();
			}

			services.Configure<FormOptions>(options =>
			{
				options.MultipartBodyLengthLimit = sessionOptions.MaxUploadBytes;
			});

			services.AddHttpContextAccessor();
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.IdleTimeout = sessionOptions.IdleTimeout;
				options.Cookie.Name = ".PlotDigitizer.Session";
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.SameSite = SameSiteMode.Lax;
			});

			services
				.AddDigitizerServices(Configuration)
				.AddSessionScopedModel()
				.AddSingleton<DigitizerStateManager>()
				.AddScoped<IDigitizerStateAccessor, DigitizerStateAccessor>()
				.AddSingleton<WorkflowService>()
				.AddScoped<ImageSourceService>()
				.AddScoped<AxisOcrReader>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}
			else {
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection()
				.UseStaticFiles()
				.UseRouting()
				.UseSession()
				.Use(async (context, next) =>
				{
					// Touch the session so a cookie is issued. Without a write ASP.NET Core never
					// sends one, and every request would look like a brand new user.
					if (string.IsNullOrEmpty(context.Session.GetString(SessionMarkerKey))) {
						context.Session.SetString(SessionMarkerKey, DateTimeOffset.UtcNow.ToString("O"));
					}
					await next();
				})
				.UseAuthorization()
				.UseEndpoints(endpoints =>
				{
					endpoints.MapRazorPages();
				});
		}
	}
}
