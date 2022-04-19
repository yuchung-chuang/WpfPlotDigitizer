using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class RelayCommandBinding : Behavior<FrameworkElement>
	{
		private CommandBinding commandBinding;
		public static readonly DependencyProperty ApplicationCommandProperty =
			DependencyProperty.Register("ApplicationCommand", typeof(ICommand), typeof(RelayCommandBinding), new PropertyMetadata(default, OnApplicationCommandChanged));
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(RelayCommandBinding), new PropertyMetadata(default, OnCommandChanged));
		public ICommand ApplicationCommand
		{
			get { return (ICommand)GetValue(ApplicationCommandProperty); }
			set { SetValue(ApplicationCommandProperty, value); }
		}
		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		private static void OnApplicationCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is RelayCommandBinding relay)) {
				return;
			}
			if (e.NewValue != null) {
				relay.CreateCommandBinding();
			}
		}
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is RelayCommandBinding relay)) {
				return;
			}
			if (e.OldValue != null) {
				relay.RemoveCommandBinding();
			} else if (e.NewValue != null) {
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
			AssociatedObject.CommandBindings.Add(commandBinding);
			CommandManager.InvalidateRequerySuggested();
		}
		private void RemoveCommandBinding()
		{
			AssociatedObject.CommandBindings.Remove(commandBinding);
			CommandManager.InvalidateRequerySuggested();
		}
		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Command == null || Command.CanExecute(e);
		}
		private void Execute(object sender, ExecutedRoutedEventArgs e)
		{
			Command?.Execute(e);
		}
		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.CommandBindings.Remove(commandBinding);
		}
	}
}
