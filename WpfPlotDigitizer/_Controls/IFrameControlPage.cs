using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public interface IPageManager
  {
    int Index { get; set; }
    /// <summary>
    /// 根據<see cref="Index"/>來取得當前頁面。
    /// </summary>
    UserControl CurrentPage { get; }
    ICommand TurnNextCommand { get; set; }
    ICommand TurnBackCommand { get; set; }

    void TurnNext();
    void TurnBack();
    bool CanTurnNext();
    bool CanTurnBack();
  }
}
