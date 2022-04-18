using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlotDigitizer.App
{
	public partial class EditPage : Page
	{
		private readonly EditPageViewModel viewModel;

		public Model Model { get; }

		public EditPage()
		{
			InitializeComponent();
			Loaded += EditPage_Loaded;
			Unloaded += EditPage_Unloaded;
		}


		public EditPage(Model model, EditPageViewModel viewModel) : this()
		{
			Model = model;
			this.viewModel = viewModel;
			DataContext = viewModel;
			model.PropertyChanged += Model_PropertyChanged;
			viewModel.EditManager.PropertyChanged += EditManager_PropertyChanged;
		}

		/// <summary>
		/// Need to update the binding expression, otherwise the binding may happens when the <see cref="EditManager"/> has not been really yet
		/// </summary>
		private void EditPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			if (!viewModel.EditManager.IsInitialised) {
				editor.Initialise(Model.FilteredImage);
				editor.EditManager.Initialise(Model.FilteredImage);
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
			if (e.PropertyName == nameof(Model.FilteredImage)) {
				editor.Initialise(Model.FilteredImage);
				editor.EditManager.Initialise(Model.FilteredImage);
			}
		}

		private void EditPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!viewModel.IsEnabled) {
				return;
			}
			Model.EdittedImage = editor.Image;
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(viewModel.EditManager.Index)) {
				UndoComboBox.SelectedIndex = 0;
				RedoComboBox.SelectedIndex = 0;
			}
		}

		private void UndoComboBox_DropDownClosed(object sender, EventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox.SelectedIndex <= 0)
				return;
			var targetIndex = viewModel.EditManager.Index - comboBox.SelectedIndex;
			if (viewModel.EditManager.GoToCommand.CanExecute(targetIndex))
				viewModel.EditManager.GoToCommand.Execute(targetIndex);
		}

		private void RedoComboBox_DropDownClosed(object sender, EventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox.SelectedIndex <= 0)
				return;
			var targetIndex = viewModel.EditManager.Index + comboBox.SelectedIndex;
			if (viewModel.EditManager.GoToCommand.CanExecute(targetIndex))
				viewModel.EditManager.GoToCommand.Execute(targetIndex);
		}

	}
}