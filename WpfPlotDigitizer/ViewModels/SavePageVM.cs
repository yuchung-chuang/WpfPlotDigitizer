using CycLibrary;
using CycLibrary.Emgu;
using CycLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
using System.Windows.Data;
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

    public List<Point> data => appData.Data;
    public PlotModel plotModel { get; set; }

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
        OnMessageRequest(Application.Current.FindResource("InformationSaveMessage") as string, MessageTypes.Information);
      else
        OnMessageRequest(Application.Current.FindResource("WarningSaveMessage") as string, MessageTypes.Warning);

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

    private Point AxLogBase => appData.AxLogBase;
    private Rect AxLim => appData.AxLim;
    public void PlotData()
    {
      var model = new PlotModel();
      var series = new LineSeries
      {
        StrokeThickness = 0,
        MarkerSize = 3,
        MarkerFill = OxyColors.Red,
        MarkerStroke = OxyColors.Black,
        MarkerType = MarkerType.Circle
      };

      foreach (var d in data)
        series.Points.Add(new DataPoint(d.X, d.Y));

      Axis axisX = AxLogBase.X > 0 ? (Axis)new LogarithmicAxis() : new LinearAxis();
      Axis axisY = AxLogBase.Y > 0 ? (Axis)new LogarithmicAxis() : new LinearAxis();

      axisX.Position = AxisPosition.Bottom;
      axisX.AbsoluteMaximum = AxLim.Right;
      axisX.AbsoluteMinimum = AxLim.Left;
      axisX.MajorGridlineStyle = LineStyle.Solid;

      axisY.Position = AxisPosition.Left;
      axisY.AbsoluteMaximum = AxLim.Bottom;
      axisY.AbsoluteMinimum = AxLim.Top;
      axisY.MajorGridlineStyle = LineStyle.Solid;

      model.Axes.Add(axisX);
      model.Axes.Add(axisY);

      model.Series.Add(series);
      plotModel = model;
    }
  }
}
