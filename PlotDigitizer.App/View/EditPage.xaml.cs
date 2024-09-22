using PlotDigitizer.Core;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public partial class EditPage : UserControl
	{
		private EditPageViewModel viewModel;
		private RelayCommandBinding undoCommandBinding;
		private RelayCommandBinding redoCommandBinding;

		public EditPage()
		{
			InitializeComponent();
			Loaded += EditPage_Loaded;
			Unloaded += EditPage_Unloaded;
		}

		private void EditPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is not EditPageViewModel viewModel)
				return;
			if (!viewModel.IsEnabled) 
				return;
			this.viewModel = viewModel;
			viewModel.EditManager.PropertyChanged += EditManager_PropertyChanged;

			// must attach binding to the hosting window, otherwise the key events won't be captuered if this page is not in focus.
			undoCommandBinding = new RelayCommandBinding
			{
				ApplicationCommand = ApplicationCommands.Undo,
				Command = viewModel.UndoCommand,
			};
			undoCommandBinding.Attach(Window.GetWindow(this));

			redoCommandBinding = new RelayCommandBinding
			{
				ApplicationCommand = ApplicationCommands.Redo,
				Command = viewModel.RedoCommand,
			};
			redoCommandBinding.Attach(Window.GetWindow(this));
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(viewModel.EditManager.Index)) {
				UndoComboBox.SelectedIndex = 0;
				RedoComboBox.SelectedIndex = 0;
			}
		}

		private void EditPage_Unloaded(object sender, RoutedEventArgs e)
		{
			// make sure to unsubscribe the events to avoid memory leak!!!
			undoCommandBinding?.Detach();
			redoCommandBinding?.Detach();
			
			if (viewModel is null || !viewModel.IsEnabled) {
				return;
			}
			viewModel.EditManager.PropertyChanged -= EditManager_PropertyChanged;
		}

	}
}