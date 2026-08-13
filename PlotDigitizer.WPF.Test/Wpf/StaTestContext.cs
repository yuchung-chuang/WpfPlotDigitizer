using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

using WpfApp = PlotDigitizer.WPF.App;

namespace PlotDigitizer.WPF.Tests
{
	/// <summary>
	/// Starts the WPF test thread before any test runs. Only one
	/// <see cref="Application"/> may exist per process, and other test classes create one lazily
	/// when the pack scheme is still unregistered, so the host has to win that race.
	/// </summary>
	[TestClass]
	public static class WpfTestHost
	{
		[AssemblyInitialize]
		public static void OnAssemblyInitialize(TestContext context) => _ = StaTestContext.Dispatcher;
	}

	/// <summary>
	/// Hosts a single STA thread with a running <see cref="Dispatcher"/> for the whole test
	/// assembly. WPF visuals, input devices and <see cref="Application"/> all require STA, but
	/// MSTest runs test methods on an MTA thread, so every test that touches a WPF object has to
	/// marshal onto this thread.
	/// </summary>
	/// <remarks>
	/// The thread also loads the real <c>App.xaml</c> resources (without running
	/// <c>OnStartup</c>), because several controls resolve <c>{StaticResource Math}</c> from the
	/// application resources, and it points <see cref="Application.ResourceAssembly"/> at the WPF
	/// assembly so relative resource URIs such as <c>/Assets/pan.cur</c> resolve.
	/// </remarks>
	internal static class StaTestContext
	{
		private static readonly Lazy<Dispatcher> dispatcher = new(Start, LazyThreadSafetyMode.ExecutionAndPublication);

		public static Dispatcher Dispatcher => dispatcher.Value;

		public static void Run(Action action) => dispatcher.Value.Invoke(action);

		public static T Run<T>(Func<T> func) => dispatcher.Value.Invoke(func);

		private static Dispatcher Start()
		{
			Dispatcher result = null;
			Exception failure = null;
			using var started = new ManualResetEventSlim();
			var thread = new Thread(() =>
			{
				try {
					SetResourceAssembly(typeof(WpfApp).Assembly);
					var app = new WpfApp();
					app.InitializeComponent();
					result = Dispatcher.CurrentDispatcher;
				}
				catch (Exception e) {
					failure = e;
				}
				finally {
					started.Set();
				}
				if (failure is null) {
					Dispatcher.Run();
				}
			})
			{
				IsBackground = true,
				Name = "PlotDigitizer WPF test thread",
			};
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			started.Wait();
			if (failure != null) {
				throw new InvalidOperationException("Could not start the WPF test thread.", failure);
			}
			return result;
		}

		/// <summary>
		/// Points WPF's resource resolution at the WPF assembly so relative resource URIs such as
		/// <c>/Assets/pan.cur</c> (used by the <c>Pan</c> behaviour) resolve.
		/// </summary>
		/// <remarks>
		/// The public <see cref="Application.ResourceAssembly"/> setter only works while the value
		/// is still unset, and the test host resolves it to itself before any test runs, so the
		/// backing fields are set directly. This is test-host-only surgery; production code always
		/// goes through the public setter.
		/// </remarks>
		private static void SetResourceAssembly(Assembly assembly)
		{
			if (Application.ResourceAssembly == assembly) {
				return;
			}
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
			typeof(Application).GetField("_resourceAssembly", flags)?.SetValue(null, assembly);
			typeof(BaseUriHelper).GetProperty("ResourceAssembly", flags)?.SetValue(null, assembly);
		}
	}
}
