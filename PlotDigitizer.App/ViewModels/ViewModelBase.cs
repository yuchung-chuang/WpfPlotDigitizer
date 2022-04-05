using System.ComponentModel;

namespace PlotDigitizer.App
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}
	}
}
