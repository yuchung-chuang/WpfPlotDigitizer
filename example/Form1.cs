using System.Drawing;
using System.Windows.Forms;

namespace example
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      PixelImage image = new PixelImage(new Bitmap("C:\\Users\\alex\\Dropbox (Alex)\\SMCMLAB\\matlab\\DataCapturer\\images\\19451854599_fdc0d1a8d7_c.jpg"));
      GrayScale(image);
      pictureBox1.Image = image.Bitmap;
    }

    private void GrayScale(PixelImage image)
    {
      byte r, g, b, gray;
      byte[] pixel = image.Pixel;
      for (int i = 0; i < pixel.Length; i += image.Byte)
      {
        b = pixel[i];
        g = pixel[i + 1];
        r = pixel[i + 2];

        gray = (byte)(b * .11f + g * .59f + r * .3f);

        pixel[i] = gray;
        pixel[i + 1] = gray;
        pixel[i + 2] = gray;
      }
      image.Pixel = pixel;
    }

    private void button1_Click(object sender, System.EventArgs e)
    {
      Form2 form2 = new Form2();
      this.Hide();
      if (form2.ShowDialog() == DialogResult.Yes)
      {
        this.Show();
      }
      else
      {
        this.Close();
      }
    }
  }
}
