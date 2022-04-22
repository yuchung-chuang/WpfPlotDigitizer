using Microsoft.Extensions.DependencyInjection;

namespace PlotDigitizer.Core
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddModel(this IServiceCollection services) =>
			services.AddSingleton<Model, ModelFacade>()
					.AddSingleton<Setting, SettingFacade>()
					.AddSingleton<InputImageNode>()
					.AddSingleton<CroppedImageNode>()
					.AddSingleton<FilteredImageNode>()
					.AddSingleton<EdittedImageNode>()
					.AddSingleton<PreviewImageNode>()
					.AddSingleton<DataNode>()

					.AddSingleton<AxisLimitNode>()
					.AddSingleton<AxisLogBaseNode>()
					.AddSingleton<AxisLocationNode>()
					.AddSingleton<FilterMinNode>()
					.AddSingleton<FilterMaxNode>()
					.AddSingleton<DataTypeNode>();

		public static IServiceCollection AddViewModels(this IServiceCollection services) =>
			services.AddSingleton<MainWindowViewModel>()
					.AddSingleton<LoadPageViewModel>()
					.AddSingleton<AxisLimitPageViewModel>()
					.AddSingleton<AxisPageViewModel>()
					.AddSingleton<FilterPageViewModel>()
					.AddSingleton<EditPageViewModel>()
					.AddSingleton<PreviewPageViewModel>();

	}
}