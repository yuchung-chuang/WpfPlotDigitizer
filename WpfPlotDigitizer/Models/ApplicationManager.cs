using CycWpfLibrary;
using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static CycWpfLibrary.NativeMethod;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class ApplicationManager
  {
    public ApplicationManager()
    {
      PageManager.TurnNextEvent += PageManager_TurnNextEvent;
      PageManager.TurnBackEvent += PageManager_TurnBackEvent;
    }

    public bool IsBusy { get; set; } = false;
    public async Task BackgroundTaskAsync(Action action)
    {
      IsBusy = true;
      try //in case there is any error in the action
      {
        await CursorWaitForAsync(action);
      }
      catch (Exception e)
      {
        throw e;
      }
      finally
      {
        IsBusy = false;
      }
    }

    public PageManagerBase PageManager { get; private set; } = pageManager;

    private void PageManager_TurnBackEvent(object sender, EventArgs e)
    {
      var pageManager = sender as PageManager;
      SetAnimationProperty();

      void SetAnimationProperty()
      {
        var currentPage = pageManager.CurrentPage as AnimatedPage;
        currentPage.SlideType = PageSlideType.Right;
        currentPage.TransitionType = PageTransitionType.Out;
        var previousPage = pageManager.PreviousPage as AnimatedPage;
        previousPage.SlideType = PageSlideType.Left;
        previousPage.TransitionType = PageTransitionType.In;
      }
    }
    private void PageManager_TurnNextEvent(object sender, EventArgs e)
    {
      var pageManager = sender as PageManager;
      TurnNextFrom();
      TurnNextTo();
      SetAnimationProperty();

      void SetAnimationProperty()
      {
        var currentPage = pageManager.CurrentPage as AnimatedPage;
        currentPage.SlideType = PageSlideType.Left;
        currentPage.TransitionType = PageTransitionType.Out;
        var nextPage = pageManager.NextPage as AnimatedPage;
        nextPage.SlideType = PageSlideType.Right;
        nextPage.TransitionType = PageTransitionType.In;
      }
      void TurnNextFrom()
      {
        switch ((ApplicationPages)pageManager.Index)
        {
          case ApplicationPages.Browse:
            appData.PBFilterW = new PixelBitmap(appData.PBInput.Size)
            {
              Pixel = ImageProcessing.FilterW(appData.PBInput)
            };
            break;
          case ApplicationPages.AxLim:
            appData.AxLim = axLimPageVM.AxLim;
            appData.AxLogBase = axLimPageVM.AxLogBase;
            break;
          case ApplicationPages.Axis:
            appData.PBAxis = appData.PBInput.Bitmap
                                          .Crop(appData.Axis)
                                          .ToPixelBitmap();
            appData.ImageAxis = appData.PBAxis.ToImage<Bgra, byte>();
            break;
          case ApplicationPages.Filter:
            break;
          case ApplicationPages.Erase:
            appData.ImageErase = erasePageVM.editManager.Object as Image<Bgra, byte>;
            break;
          case ApplicationPages.Data:
            break;
          default:
            break;
        }
      }
      void TurnNextTo()
      {
        // call before actually turned next
        switch ((ApplicationPages)pageManager.Index + 1)
        {
          case ApplicationPages.AxLim:
            axLimPageVM.OnPropertyChanged(nameof(axLimPageVM.imageSource));
            break;
          case ApplicationPages.Axis:
            axisPageVM.GetAxis();
            axisPageVM.OnPropertyChanged(nameof(axisPageVM.ImageSource));
            break;
          case ApplicationPages.Filter:
            appData.ImageFilterRGB = appData.ImageAxis.Clone();
            filterPageVM.InRange();
            break;
          case ApplicationPages.Erase:
            appData.ImageErase = appData.ImageFilterRGB.Clone();
            erasePageVM.editManager.Init(appData.ImageErase);
            break;
          case ApplicationPages.Data:
            dataPageVM.imageDisplay = appData.ImageErase.Clone();
            dataPageVM.ParamChanged();
            break;
          case ApplicationPages.Save:
            savePageVM.PlotData();
            break;
          default:
            break;
        }
      }
    }
  }
}
