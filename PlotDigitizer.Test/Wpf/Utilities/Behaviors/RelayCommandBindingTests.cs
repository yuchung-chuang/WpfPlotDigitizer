using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core;

using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.WPF.Tests.Utilities.Behaviors
{
	/// <summary>
	/// Routes an application command (Undo/Redo) on a control to a plain view-model
	/// <see cref="RelayCommand"/>, which is how the edit page hooks Ctrl+Z to its view model.
	/// </summary>
	[TestClass]
	public class RelayCommandBindingTests
	{
		[TestMethod]
		[TestCategory("Unit")]
		public void Attach_WithApplicationCommand_AddsExactlyOneCommandBinding()
		{
			StaTestContext.Run(() =>
			{
				var element = new Button();
				var behavior = new RelayCommandBinding { ApplicationCommand = ApplicationCommands.Undo };

				behavior.Attach(element);

				Assert.AreEqual(1, element.CommandBindings.Count);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ApplicationCommand_SetAfterAttaching_StillBinds()
		{
			StaTestContext.Run(() =>
			{
				var element = new Button();
				var behavior = new RelayCommandBinding();
				behavior.Attach(element);

				behavior.ApplicationCommand = ApplicationCommands.Redo;

				Assert.AreEqual(1, element.CommandBindings.Count);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Execute_ApplicationCommand_RunsTheBoundRelayCommand()
		{
			StaTestContext.Run(() =>
			{
				var executions = 0;
				var element = new Button();
				var behavior = new RelayCommandBinding
				{
					ApplicationCommand = ApplicationCommands.Undo,
					Command = new RelayCommand(() => executions++),
				};
				behavior.Attach(element);

				ApplicationCommands.Undo.Execute(null, element);

				Assert.AreEqual(1, executions);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CanExecute_FollowsTheBoundRelayCommand()
		{
			StaTestContext.Run(() =>
			{
				var canExecute = false;
				var element = new Button();
				var behavior = new RelayCommandBinding
				{
					ApplicationCommand = ApplicationCommands.Undo,
					Command = new RelayCommand(() => { }, () => canExecute),
				};
				behavior.Attach(element);

				Assert.IsFalse(ApplicationCommands.Undo.CanExecute(null, element), "disabled command");

				canExecute = true;

				Assert.IsTrue(ApplicationCommands.Undo.CanExecute(null, element), "enabled command");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CanExecute_WithoutABoundCommand_IsTrue()
		{
			StaTestContext.Run(() =>
			{
				var element = new Button();
				var behavior = new RelayCommandBinding { ApplicationCommand = ApplicationCommands.Undo };
				behavior.Attach(element);

				Assert.IsTrue(ApplicationCommands.Undo.CanExecute(null, element));
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Detach_Always_RemovesTheBindingSoTheCommandNoLongerRuns()
		{
			StaTestContext.Run(() =>
			{
				var executions = 0;
				var element = new Button();
				var behavior = new RelayCommandBinding
				{
					ApplicationCommand = ApplicationCommands.Undo,
					Command = new RelayCommand(() => executions++),
				};
				behavior.Attach(element);

				behavior.Detach();
				ApplicationCommands.Undo.Execute(null, element);

				Assert.AreEqual(0, element.CommandBindings.Count, "binding count");
				Assert.AreEqual(0, executions, "execution count");
			});
		}
	}
}
