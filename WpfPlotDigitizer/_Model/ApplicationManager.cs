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
      PageManager.TurnToEvent += PageManager_TurnToEvent;
    }

    public PageManagerBase PageManager { get; private set; } = new PageManager();

    private bool PageManager_TurnNextEvent(object sender, EventArgs e)
    {
      var isCanceled = false;
      var pageManager = sender as PageManager;
      TurnNextFrom();
      if (isCanceled)
        return isCanceled;
      TurnNextTo();
      return isCanceled;

      void TurnNextFrom()
      {
        switch ((ApplicationPages)pageManager.Index)
        {
          case ApplicationPages.Browse:
            imageData.PBFilterW = new PixelBitmap(imageData.PBInput.Size)
            {
              Pixel = ImageProcessing.FilterW(imageData.PBInput)
            };
            break;
          case ApplicationPages.AxLim:
            if (!AxLimCheck())
            {
              imageData.AxLim = axLimPageVM.AxLim;
              imageData.AxLogBase = axLimPageVM.AxLogBase;
            }
            else
            {
              MessageBox.Show("Please type in all axis limits.", "Warning", MessageBoxButton.OK);
              isCanceled = true;
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
      }
      void TurnNextTo()
      {
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
      }
      bool AxLimCheck() => axLimPageVM.AxLim == Rect.Empty;
    }

    private bool PageManager_TurnToEvent(object sender, int index)
    {
      var pageManager = sender as PageManager;
      if (index > pageManager.Index)
      {
        var turns = index - pageManager.Index;
        for (int i = 0; i < turns; i++)
        {
          var isCanceled = pageManager.TurnNext();
          if (isCanceled)
            return true;
        }
      }
      return false;
    }
  }
}
