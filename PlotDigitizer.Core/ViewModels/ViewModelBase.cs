using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string Name { get; set; }

		protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void RaisePropertyChanged(string propertyName) => OnPropertyChanged(propertyName);

		/// <summary>
		/// The actions to take when entering the view associated with this view model.
		/// </summary>
		public virtual void Enter() { }

		/// <summary>
		/// The actions to take when leaving the view associated with this view model.
		/// </summary>
		public virtual void Leave() { }
	}
}