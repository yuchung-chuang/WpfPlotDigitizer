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
using System.Media;
using System.IO;

namespace DataCapturer
{
  public partial class StartMovie : Form
  {

    public StartMovie()
    {
      InitializeComponent();   
    }
    private void StartMovie_Load(object sender, EventArgs e)
    {
      Task Start = Task.Run(() => StartAsync());
    }

    private float moviePercentage = 0;
    private PixelImage image = new PixelImage(Properties.Resources.icon81);
    private PixelImage display;
    private void FadeStartLogo()
    {
      while (moviePercentage < 1)
      {
        display = Drawing.Fade(image, moviePercentage);
        pictureBox1.InvokeIfRequired(() => pictureBox1.Image = display.Bitmap);
        moviePercentage += 0.01f;
        Thread.Sleep(10);
      }
    }
    private void StartSoundAsync()
    {
      SoundPlayer soundPlayer = new SoundPlayer(Properties.Resources.startSound);
      soundPlayer.PlaySync();
    }
    private void StartMainForm()
    {
      this.InvokeIfRequired(() =>
      {
        this.Hide();
        var dataCapturer = new DataCapturer();
        dataCapturer.BringToFront();
        dataCapturer.ShowDialog(this);
        this.Close();
      });

    }
    private void StartAsync()
    {
      Task StartSound = Task.Run(() => StartSoundAsync());
      FadeStartLogo();
      StartSound.Wait();
      StartMainForm();
    }

  }
}
