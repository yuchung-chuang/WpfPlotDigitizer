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
		private Model model;

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
			this.model = viewModel.Model;

			// must attach binding to the hosting window, otherwise the key events won't be captuered if this page is not in focus.
			new RelayCommandBinding
			{
				ApplicationCommand = ApplicationCommands.Undo,
				Command = viewModel.EditManager.UndoCommand,
			}.Attach(Window.GetWindow(this));
			new RelayCommandBinding
			{
				ApplicationCommand = ApplicationCommands.Redo,
				Command = viewModel.EditManager.RedoCommand,
			}.Attach(Window.GetWindow(this));

			if (!viewModel.EditManager.IsInitialised) {
				editor.Initialise(model.FilteredImage);
			} else {
				editor.Initialise(editor.EditManager.CurrentObject.Copy());
			}
			viewModel.EditManager.PropertyChanged += EditManager_PropertyChanged;
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
			if (!viewModel.IsEnabled) {
				return;
			}
			// make sure to unsubscribe the events to avoid memory leak!!!
			viewModel.EditManager.PropertyChanged -= EditManager_PropertyChanged;
		}

	}
}