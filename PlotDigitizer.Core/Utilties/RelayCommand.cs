using System;
using System.Windows.Input;

namespace PlotDigitizer.Core
{
	public class RelayCommand : ICommand
	{
		private readonly Action action;
		private readonly Func<bool> canAction;

		public RelayCommand(Action action)
		{
			this.action = action;
		}

		public RelayCommand(Action action, Func<bool> canAction)
		{
			this.action = action;
			this.canAction = canAction;
		}

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

		public bool CanExecute(object parameter = null) => canAction == null || canAction.Invoke();

		public void Execute(object parameter = null) => action?.Invoke();
	}

	public class RelayCommand<TParam>(Action<TParam> action, Func<TParam, bool> canAction = null) : ICommand
	{
		private readonly Action<TParam> action = action;
		private readonly Func<TParam, bool> canAction = canAction;

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

		public bool CanExecute(object parameter)
		{
			return canAction == null ||
				parameter is TParam TParam && canAction.Invoke(TParam);
		}

		public void Execute(object parameter)
		{
			if (parameter is TParam TParam) {
				action?.Invoke(TParam);
			}
		}
	}
}