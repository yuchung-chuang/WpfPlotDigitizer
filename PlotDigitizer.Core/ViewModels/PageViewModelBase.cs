using System;
using System.Collections.Generic;
using System.Text;

namespace PlotDigitizer.Core
{
	public class PageViewModelBase : ViewModelBase
	{
		public string Name { get; set; }

		public virtual void Enter()
		{

		}

		public virtual void Leave()
		{

		}
	}
}
