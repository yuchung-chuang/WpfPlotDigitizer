using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataCapturer
{
  public partial class StartMovie : Form
  {
    public StartMovie()
    {
      InitializeComponent();

      timer1.Enabled = true;
      timer1.Interval = 450;//0.45秒觸發一次
    }
    
    private void timer1_Tick(object sender, EventArgs e)
    {
      progressBar1.Increment(10);
      this.axShockwaveFlash1.Forward();
      if (progressBar1.Value == 100)
      {
        timer1.Stop();
        this.axShockwaveFlash1.Stop();
        this.Close();
      }
    }

    private void StartMovie_Load(object sender, EventArgs e)
    {
      axShockwaveFlash1.LoadMovie(0, Application.StartupPath + "\\cloud.swf");

    }
  }
}
