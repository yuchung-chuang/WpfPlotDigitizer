using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MyLibrary.Classes;
using System.Diagnostics;
using MyLibrary.Methods;
using System.Threading;

namespace DataCapturer
{
  public partial class StartMovie : Form
  {

    public StartMovie()
    {
      InitializeComponent();

      backgroundWorker1.RunWorkerAsync();
    }

    private float moviePercentage = 0;
    private PixelImage image = new PixelImage(Properties.Resources.icon5);
    private PixelImage display;
    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      while (moviePercentage < 1)
      {
        display = Drawing.Fade(image, moviePercentage);
        pictureBox1.InvokeIfRequired(new Action(() => { pictureBox1.Image = display.Bitmap; }));
        moviePercentage += 0.01f;
      }
      Thread.Sleep(500);
    }
    private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      this.Hide();
      var dataCapturer = new DataCapturer();
      dataCapturer.BringToFront();
      dataCapturer.ShowDialog(this);
      this.Close();
    }
  }
}
