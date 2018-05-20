using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoResizeControls
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.Tag = this.Height + "|" + this.Width;
			foreach (Control control in this.Controls)
			{
				control.Tag = control.Top + "|" + control.Left + "|" + control.Height + "|" + control.Width;
			}
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			foreach (Control control in this.Controls)
			{
				control.Width = (int)(double.Parse(control.Tag.ToString().Split('|')[3]) * (this.Width / double.Parse(this.Tag.ToString().Split('|')[1])));
				control.Height = (int)(double.Parse(control.Tag.ToString().Split('|')[2]) * (this.Height / double.Parse(this.Tag.ToString().Split('|')[0])));
				control.Left = (int)(double.Parse(control.Tag.ToString().Split('|')[1]) * (this.Width / double.Parse(this.Tag.ToString().Split('|')[1])));
				control.Top = (int)(double.Parse(control.Tag.ToString().Split('|')[0]) * (this.Height / double.Parse(this.Tag.ToString().Split('|')[0])));
			}
		}
	}
}
