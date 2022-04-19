using PlotDigitizer.Core;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlotDigitizer.App
{
	public partial class EditPage : Page
	{
		private EditPageViewModel viewModel;

		public Model Model { get; private set; }

		public EditPage()
		{
			InitializeComponent();
			Loaded += EditPage_Loaded;
			Unloaded += EditPage_Unloaded;

		}

		/// <summary>
		/// Need to update the binding expression, otherwise the binding may happens when the <see cref="EditManager"/> has not been really yet
		/// </summary>
		private void EditPage_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel = DataContext as EditPageViewModel;
			Model = viewModel.Model;
			Model.PropertyChanged += Model_PropertyChanged;
			viewModel.EditManager.PropertyChanged += EditManager_PropertyChanged;

			if (!viewModel.IsEnabled) {
				return;
			}
			if (!viewModel.EditManager.IsInitialised) {
				editor.Initialise(Model.FilteredImage);
			} else {
				editor.Initialise(editor.EditManager.CurrentObject.Copy());
			}
			UndoButton.GetBindingExpression(ButtonBase.CommandProperty).UpdateTarget();
			RedoButton.GetBindingExpression(ButtonBase.CommandProperty).UpdateTarget();
		}

		/// <summary>
		/// Do NOT initialise it when loading, so long as the <see cref="Model.FilteredImage"/> is un changed, the previous editting is retained. 
		/// </summary>
		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Core.Model.FilteredImage)) {
				editor.Initialise(Model.FilteredImage);
			}
		}

		private void EditPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			Model.PropertyChanged -= Model_PropertyChanged;
			viewModel.EditManager.PropertyChanged -= EditManager_PropertyChanged;
			Model.EdittedImage = editor.Image;
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(viewModel.EditManager.Index)) {
				UndoComboBox.SelectedIndex = 0;
				RedoComboBox.SelectedIndex = 0;
			}
		}

		//private void UndoComboBox_DropDownClosed(object sender, EventArgs e)
		//{
		//	var comboBox = sender as ComboBox;
		//	if (comboBox.SelectedIndex <= 0)
		//		return;
		//	var targetIndex = viewModel.EditManager.Index - comboBox.SelectedIndex;
		//	if (viewModel.EditManager.GoToCommand.CanExecute(targetIndex))
		//		viewModel.EditManager.GoToCommand.Execute(targetIndex);
		//}

		//private void RedoComboBox_DropDownClosed(object sender, EventArgs e)
		//{
		//	var comboBox = sender as ComboBox;
		//	if (comboBox.SelectedIndex <= 0)
		//		return;
		//	var targetIndex = viewModel.EditManager.Index + comboBox.SelectedIndex;
		//	if (viewModel.EditManager.GoToCommand.CanExecute(targetIndex))
		//		viewModel.EditManager.GoToCommand.Execute(targetIndex);
		//}

	}
}