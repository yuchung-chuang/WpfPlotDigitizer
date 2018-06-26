using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataCapturer
{
  static class Program
  {
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDPIAwareness();

    /// <summary>
    /// 應用程式的主要進入點。
    /// </summary>
    [STAThread]
    static void Main()
    {

      if (System.Environment.OSVersion.Version.Major >= 6)
      {
        SetProcessDPIAware();
        //SetProcessDPIAwareness();
      }
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
      Application.Run(new DataCapturer());
#else
      Application.Run(new StartMovie());
#endif
    }
  }
}
