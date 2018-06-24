using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace example
{
  public partial class Form2 : Form
  {
    public Form2()
    {
      InitializeComponent();
      backgroundWorker1.RunWorkerAsync();
      backgroundWorker2.RunWorkerAsync();
      int x = 1;
    }

    private class MyClass
    {
      public int x = 1;
    }

    private MyClass myClass = new MyClass();

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      for (int i = 0; i < 1000; i++)
      {
        myClass.x = i;
        Console.WriteLine(myClass.x);
      }
    }
    

    private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
    {
      for (int i = 0; i < 1000; i++)
      {
        myClass.x = i;
        Console.WriteLine(myClass.x);
      }
    }

    private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Console.WriteLine("1 done");
    }

    private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Console.WriteLine("2 done");
    }
  }
}
