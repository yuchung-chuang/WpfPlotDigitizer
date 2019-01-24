using CycWpfLibrary;
using CycWpfLibrary.Controls;
using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
      PageManager.TurnToEvent += PageManager_TurnToEvent;
    }


    public PageManagerBase PageManager { get; private set; } = new PageManager();

    private bool PageManager_TurnBackEvent(object sender, EventArgs e)
    {
      var pageManager = sender as PageManager;
      SetAnimationProperty();
      return true;

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
    private bool PageManager_TurnNextEvent(object sender, EventArgs e)
    {
      var pageManager = sender as PageManager;
      var result = TurnNextFrom() && TurnNextTo();
      if (result)
        SetAnimationProperty();
      return result;

      void SetAnimationProperty()
      {
        var currentPage = pageManager.CurrentPage as AnimatedPage;
        currentPage.SlideType = PageSlideType.Left;
        currentPage.TransitionType = PageTransitionType.Out;
        var nextPage = pageManager.NextPage as AnimatedPage;
        nextPage.SlideType = PageSlideType.Right;
        nextPage.TransitionType = PageTransitionType.In;
      }
      bool TurnNextFrom()
      {
        var turnResult = true;
        switch ((ApplicationPages)pageManager.Index)
        {
          case ApplicationPages.Browse:
            if (PBInputCheck())
            {
              imageData.PBFilterW = new PixelBitmap(imageData.PBInput.Size)
              {
                Pixel = ImageProcessing.FilterW(imageData.PBInput)
              };
            }
            else
            {
              MessageBox.Show("Please select an image.", "Warning", MessageBoxButton.OK);
              turnResult = false;
            }
            break;
          case ApplicationPages.AxLim:
            if (AxLimCheck())
            {
              imageData.AxLim = axLimPageVM.AxLim;
              imageData.AxLogBase = axLimPageVM.AxLogBase;
            }
            else
            {
              MessageBox.Show("Please type in all valid axis limits.", "Warning", MessageBoxButton.OK);
              turnResult = false;
            }
            break;
          case ApplicationPages.Axis:
            imageData.PBAxis = imageData.PBInput.Bitmap
                                          .Crop(imageData.Axis)
                                          .ToPixelBitmap();
            imageData.ImageAxis = imageData.PBAxis.ToImage<Bgra, byte>();
            break;
          case ApplicationPages.Filter:
            break;
          case ApplicationPages.Erase:
            imageData.ImageErase = erasePageVM.editManager.Object as Image<Bgra, byte>;
            break;
          case ApplicationPages.Data:
            break;
          default:
            break;
        }
        return turnResult;
      }
      bool TurnNextTo()
      {
        var turnResult = true;
        // call before actually turned next
        switch ((ApplicationPages)pageManager.Index + 1)
        {
          case ApplicationPages.AxLim:
            //axLimPageVM.GetAxisLimit();
            break;
          case ApplicationPages.Axis:
            axisPageVM.GetAxis();
            break;
          case ApplicationPages.Filter:
            imageData.ImageFilterRGB = imageData.ImageAxis.Clone();
            filterPageVM.InRange();
            break;
          case ApplicationPages.Erase:
            imageData.ImageErase = imageData.ImageFilterRGB.Clone();
            erasePageVM.editManager.Init(imageData.ImageErase);
            break;
          case ApplicationPages.Data:
            dataPageVM.imageDisplay = imageData.ImageErase.Clone();
            dataPageVM.ParamChanged();
            break;
          case ApplicationPages.Save:
            savePageVM.imageSave = imageData.ImageErase.Clone();
            break;
          default:
            break;
        }
        return turnResult;
      }
      bool AxLimCheck() => axLimPageVM.AxLim != Rect.Empty;
      bool PBInputCheck() => imageData.PBInput != null;
    }

    private bool PageManager_TurnToEvent(object sender, int index)
    {
      var pageManager = sender as PageManager;
      if (index > pageManager.Index)
      {
        var turns = index - pageManager.Index;
        for (int i = 0; i < turns; i++)
        {
          if (!pageManager.TurnNext()) // turn is cancelled
            return false;
        }
      }
      return false;
    }
  }
}
