using Dna;
using DIConstruct = Dna.FrameworkConstruction;
using Microsoft.Extensions.DependencyInjection;

namespace WpfPlotDigitizer
{
  public static class DIConstructExtensions
  {
    public static DIConstruct AddWpfPlotDigitizerViewModels(this DIConstruct construction)
    {
      construction.Services.AddSingleton<ApplicationManager>();
      construction.Services.AddSingleton<ImageProcessingManager>();

      return construction;
    }
  }
}
