using System.ComponentModel;

namespace PlotDigitizer.App
{
	public class AutoPageTurner
	{
		private readonly PageManager pageManager;
		private readonly LoadPage loadPage;

		public AutoPageTurner(PageManager pageManager, LoadPage loadPage)
		{
			this.pageManager = pageManager;
			this.loadPage = loadPage;
			loadPage.PropertyChanged += LoadPage_PropertyChanged;
		}

		private void LoadPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(LoadPage.ImageSource)) {
				if (pageManager.NextCommand.CanExecute(null)) {
					pageManager.NextCommand.Execute(null);
				}
			}
		}
	}
}
