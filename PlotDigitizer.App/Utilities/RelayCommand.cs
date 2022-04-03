using System;
using System.Windows.Input;

namespace PlotDigitizer.App
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

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public bool CanExecute(object parameter = null)
		{
			return canAction == null || canAction.Invoke();
		}

		public void Execute(object parameter = null)
		{
			action?.Invoke();
		}
	}

	public class RelayCommand<TParam> : ICommand
	{
		private readonly Action<TParam> action;
		private readonly Func<TParam, bool> canAction;

		public RelayCommand(Action<TParam> action, Func<TParam, bool> canAction)
		{
			this.action = action;
			this.canAction = canAction;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

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