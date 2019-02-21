using Dna;
using DIConstruct = Dna.FrameworkConstruction;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using CycWpfLibrary;

namespace WpfPlotDigitizer
{
  public static class DI
  {
    public static PageManager pageManager => Framework.Service<PageManager>();
    public static MessageManager messageManager => Framework.Service<MessageManager>();
    public static TutorialManager tutorialManager => Framework.Service<TutorialManager>();
    public static ApplicationManager appManager => Framework.Service<ApplicationManager>();
    public static ApplicationData appData => Framework.Service<ApplicationData>();

    public static MainWindowVM mainWindowVM => Framework.Service<MainWindowVM>();
    public static SplashPageVM splashPageVM => Framework.Service<SplashPageVM>();
    public static BrowsePageVM browsePageVM => Framework.Service<BrowsePageVM>();
    public static AxisPageVM axisPageVM => Framework.Service<AxisPageVM>();
    public static AxLimPageVM axLimPageVM => Framework.Service<AxLimPageVM>();
    public static FilterPageVM filterPageVM => Framework.Service<FilterPageVM>();
    public static ErasePageVM erasePageVM => Framework.Service<ErasePageVM>();
    public static DataPageVM dataPageVM => Framework.Service<DataPageVM>();
    public static SavePageVM savePageVM => Framework.Service<SavePageVM>();

    public static MainWindow mainWindow => Framework.Service<MainWindow>();
    public static SplashPage splashPage => Framework.Service<SplashPage>();
    public static BrowsePage browsePage => Framework.Service<BrowsePage>();
    public static AxisPage axisPage => Framework.Service<AxisPage>();
    public static AxLimPage axLimPage => Framework.Service<AxLimPage>();
    public static FilterPage filterPage => Framework.Service<FilterPage>();
    public static ErasePage erasePage => Framework.Service<ErasePage>();
    public static DataPage dataPage => Framework.Service<DataPage>();
    public static SavePage savePage => Framework.Service<SavePage>();
    public static readonly AnimatedPage emptyPage = new AnimatedPage();

    public static DIConstruct AddWpfPlotDigitizerServices(this DIConstruct construction)
    {
      construction.Services.AddSingleton<PageManager>();
      construction.Services.AddSingleton<TutorialManager>();
      construction.Services.AddSingleton<MessageManager>();
      construction.Services.AddSingleton<ApplicationManager>();
      construction.Services.AddSingleton<ApplicationData>();

      construction.Services.AddSingleton<MainWindowVM>();
      construction.Services.AddSingleton<SplashPageVM>();
      construction.Services.AddSingleton<BrowsePageVM>();
      construction.Services.AddSingleton<AxisPageVM>();
      construction.Services.AddSingleton<AxLimPageVM>();
      construction.Services.AddSingleton<FilterPageVM>();
      construction.Services.AddSingleton<ErasePageVM>();
      construction.Services.AddSingleton<DataPageVM>();
      construction.Services.AddSingleton<SavePageVM>();

      construction.Services.AddSingleton<MainWindow>();
      construction.Services.AddSingleton<SplashPage>();
      construction.Services.AddSingleton<BrowsePage>();
      construction.Services.AddSingleton<AxisPage>();
      construction.Services.AddSingleton<AxLimPage>();
      construction.Services.AddSingleton<FilterPage>();
      construction.Services.AddSingleton<ErasePage>();
      construction.Services.AddSingleton<DataPage>();
      construction.Services.AddSingleton<SavePage>();

      return construction;
    }
  }
}
