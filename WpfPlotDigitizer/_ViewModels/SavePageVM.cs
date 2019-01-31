using CycWpfLibrary;
using CycWpfLibrary.Emgu;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;
using Excel = Microsoft.Office.Interop.Excel;
using Point = System.Windows.Point;

namespace WpfPlotDigitizer
{
  public class SavePageVM : ViewModelBase
  {
    public SavePageVM()
    {
      SaveCommand = new RelayCommand<object, Task>(SaveAsync);
    }

    public Image<Bgra, byte> imageSave
    {
      get => appData.ImageSave;
      set => appData.ImageSave = value;
    }

    public List<Point> data
    {
      get => appData.Data;
      set => appData.Data = value;
    }

    public BitmapSource imageSource => imageSave?.ToBitmapSource();

    public ICommand SaveCommand { get; set; }
    public async Task SaveAsync(object param = null)
    {
      if (appManager.IsBusy)
        return;
      var saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Excel (.xlsx) | *.xlsx |CSV (.csv) | *.csv |TXT (.txt) | *.txt";
      if (saveFileDialog.ShowDialog() == false)
        return;
      var IsSucessfulSave = false;
      switch (saveFileDialog.FilterIndex)
      {
        default:
        case 1:
          IsSucessfulSave = await SaveAsExcelAsync();
          break;
        case 2:
          IsSucessfulSave = await SaveAsCSVAsync();
          break;
        case 3:
          IsSucessfulSave = await SaveAsTXTAsync();
          break;
      }

      if (IsSucessfulSave)
        MessageBoxManager.Information("Sucessfully saved!");
      else
        MessageBoxManager.Warning("Sorry... there's something wrong while saving...");

      async Task<bool> SaveAsExcelAsync()
      {
        int dataCount = data.Count;
        object[,] dataArray = new object[data.Count + 1, 2];
        dataArray[0, 0] = "X";
        dataArray[0, 1] = "Y";
        for (int i = 0; i < dataCount; i++)
        {
          dataArray[i + 1, 0] = data[i].X;
          dataArray[i + 1, 1] = data[i].Y;
        }

        var excel = new Excel.Application()
        {
          Visible = false,
          DisplayAlerts = false,
        };
        var wBook = excel.Workbooks.Add(Type.Missing);
        var wSheet = (Excel._Worksheet)wBook.Worksheets[1];
        try
        {
          await appManager.BackgroundTaskAsync(() =>
          {
            wBook.Activate();
            wSheet.Activate();

            string finalColLetter = "B";
            string excelRange = string.Format("A1:{0}{1}",
                finalColLetter, dataCount + 1);

            wSheet.get_Range(excelRange, Type.Missing).Value2 = dataArray;
            wBook.SaveAs(saveFileDialog.FileName);
            wBook.Close(false);
            excel.Quit();
          });
          return true;
        }
        catch (Exception)
        {
          return false;
        }
        finally
        {
          Marshal.ReleaseComObject(wSheet);
          Marshal.ReleaseComObject(excel);
          wSheet = null;
          excel = null;
          GC.Collect();
          GC.WaitForPendingFinalizers();
        }
      }
      async Task<bool> SaveAsCSVAsync()
      {
        try
        {
          await Task.Run(() =>
          {
            string strPath = saveFileDialog.FileName;

            StringBuilder content = new StringBuilder();
            content.AppendLine("X,Y");
            int dataCount = data.Count;
            for (int i = 0; i < dataCount; i++)
            {
              content.AppendLine(data[i].X.ToString() + "," + data[i].Y.ToString());
            }

            using (var fs = File.OpenWrite(strPath))
            using (var sw = new StreamWriter(fs))
            {
              sw.Write(content.ToString());
            }

          });
          return true;
        }
        catch (Exception)
        {
          return false;
        }
      }
      async Task<bool> SaveAsTXTAsync()
      {
        try
        {
          await Task.Run(() =>
          {
            string strPath = saveFileDialog.FileName;

            StringBuilder content = new StringBuilder();
            content.AppendLine("X\tY");
            int dataCount = data.Count;
            for (int i = 0; i < dataCount; i++)
            {
              content.AppendLine(data[i].X.ToString() + "\t" + data[i].Y.ToString());
            }

            using (var fs = File.OpenWrite(strPath))
            using (var sw = new StreamWriter(fs))
            {
              sw.Write(content.ToString());
            }

          });
          return true;
        }
        catch (Exception)
        {
          return false;
        }
      }
    }

    
  }
}
