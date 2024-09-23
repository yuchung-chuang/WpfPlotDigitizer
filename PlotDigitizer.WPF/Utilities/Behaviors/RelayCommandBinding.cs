using Microsoft.Xaml.Behaviors;

using System.Windows;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
	public class RelayCommandBinding : Behavior<FrameworkElement>
	{

		#region Fields

		public static readonly DependencyProperty ApplicationCommandProperty =
			DependencyProperty.Register("ApplicationCommand", typeof(ICommand), typeof(RelayCommandBinding), new PropertyMetadata(default, OnApplicationCommandChanged));

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(RelayCommandBinding), new PropertyMetadata(default));

		private CommandBinding commandBinding;

		#endregion Fields


		#region Properties

		public ICommand ApplicationCommand
		{
			get => (ICommand)GetValue(ApplicationCommandProperty);
			set => SetValue(ApplicationCommandProperty, value);
		}

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		#endregion Properties


		#region Methods

		protected override void OnAttached()
		{
			base.OnAttached();
			AddCommandBinding();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			RemoveCommandBinding();
		}

		private static void OnApplicationCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is RelayCommandBinding relay)) {
				return;
			}
			if (e.NewValue != null) {
				relay.CreateCommandBinding();
				relay.AddCommandBinding();
			}
		}
		private void CreateCommandBinding()
		{
			commandBinding = new CommandBinding(ApplicationCommand,
				new ExecutedRoutedEventHandler(Execute),
				new CanExecuteRoutedEventHandler(CanExecute));
		}

		private void AddCommandBinding()
		{
			if (AssociatedObject is null || commandBinding is null) {
				return;
			}
			AssociatedObject.CommandBindings.Add(commandBinding);
			CommandManager.InvalidateRequerySuggested();
		}

		private void RemoveCommandBinding()
		{
			AssociatedObject.CommandBindings.Remove(commandBinding);
			CommandManager.InvalidateRequerySuggested();
		}

		private void Execute(object sender, ExecutedRoutedEventArgs e) => Command?.Execute(e);
		private void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Command == null || Command.CanExecute(e);

		#endregion Methods
	}
}