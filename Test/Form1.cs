using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LayerControl lc1 = new LayerControl(100, 100);
			lc1.Location = new Point(20, 20);
			lc1.DrawRectangle();
			LayerControl lc2 = new LayerControl(100, 100);
			lc2.Location = new Point(10, 10);
			lc2.DrawCircles();
			this.Controls.Add(lc1);
			this.Controls.Add(lc2);
			this.Controls.Remove(lc2);
		}
	}
}
