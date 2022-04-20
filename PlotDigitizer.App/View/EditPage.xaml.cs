using PlotDigitizer.Core;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlotDigitizer.App
{
	public partial class EditPage : UserControl
	{
		private EditPageViewModel viewModel;

		public Model Model { get; private set; }

		public EditPage()
		{
			InitializeComponent();
			Loaded += EditPage_Loaded;
			Unloaded += EditPage_Unloaded;
		}

		private void EditPage_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel = DataContext as EditPageViewModel;
			Model = viewModel.Model;

			if (!viewModel.IsEnabled) {
				return;
			}
			if (!viewModel.EditManager.IsInitialised) {
				editor.Initialise(Model.FilteredImage);
			} else {
				editor.Initialise(editor.EditManager.CurrentObject.Copy());
			}
			Model.PropertyChanged += Model_PropertyChanged;
			viewModel.EditManager.PropertyChanged += EditManager_PropertyChanged;
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
			// make sure to unsubscribe the events to avoid memory leak!!!
			Model.PropertyChanged -= Model_PropertyChanged;
			viewModel.EditManager.PropertyChanged -= EditManager_PropertyChanged;
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(viewModel.EditManager.Index)) {
				UndoComboBox.SelectedIndex = 0;
				RedoComboBox.SelectedIndex = 0;
			}
		}
	}
}