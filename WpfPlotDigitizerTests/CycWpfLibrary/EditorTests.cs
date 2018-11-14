using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using CycWpfLibrary.Media;
using CycWpfLibrary;

namespace CycWpfLibrary.Tests
{
  [TestClass()]
  public class EditorTests
  {
    [TestMethod()]
    public void EditorTest()
    {
      var PB1 = new Bitmap(@"C:\Users\alex\Desktop\WPF\WpfPlotDigitizer\WpfPlotDigitizerTests\data.png").ToPixelBitmap();
      var PB2 = new Bitmap(@"C:\Users\alex\Desktop\WPF\WpfPlotDigitizer\WpfPlotDigitizer\images\ocr.png").ToPixelBitmap();
      var manager = new PixelBitmapEditor(PB1);
      manager.Edit(PB2);
      Assert.AreEqual(PB2, manager.Object);
      manager.Undo();
      Assert.AreEqual(PB1, manager.Object);
      manager.Redo();
      Assert.AreEqual(PB2, manager.Object);
    }
  }
}