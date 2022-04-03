﻿using System;

namespace PlotDigitizer.App
{
	public class AutoPageTurner
	{
		private readonly PageManager pageManager;

		public AutoPageTurner(PageManager pageManager, LoadPage loadPage)
		{
			this.pageManager = pageManager;
			loadPage.NextPage += LoadPage_NextPage;
		}

		private void LoadPage_NextPage(object sender, EventArgs e)
		{
			if (pageManager.NextCommand.CanExecute()) {
				pageManager.NextCommand.Execute();
			}
		}

	}
}
