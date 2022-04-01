using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for EditPage.xaml
	/// </summary>
	public partial class EditPage : Page, INotifyPropertyChanged
    {
        private readonly Model model;
        private readonly List<ToggleButton> stateButtons;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<string> UndoList => EditManager?.TagList.GetRange(0, EditManager.Index + 1).Reverse<string>();

        public IEnumerable<string> RedoList => EditManager?.TagList.GetRange(EditManager.Index, EditManager.TagList.Count - EditManager.Index);

        public EditManager<Image<Rgba, byte>> EditManager => editor?.EditManager;

        public Image<Rgba, byte> Image => editor?.Image;

        public EditorState EditorState
        {
            get => editor.EditorState;
            set => editor.EditorState = value;
        }

        public EditPage()
        {
            InitializeComponent();
            Loaded += EditPage_Loaded;
            Unloaded += EditPage_Unloaded;

            stateButtons = new List<ToggleButton>
            {
                PencilButton, EraserButton, RectButton, PolyButton,
            };
        }


        public EditPage(Model model) : this()
        {
            this.model = model;
            model.PropertyChanged += Model_PropertyChanged;
            EditManager.PropertyChanged += EditManager_PropertyChanged;
        }

        private void EditPage_Loaded(object sender, RoutedEventArgs e)
        {
            UndoButton.GetBindingExpression(ButtonBase.CommandProperty).UpdateTarget();
            RedoButton.GetBindingExpression(ButtonBase.CommandProperty).UpdateTarget();
        }
        
        private void EditPage_Unloaded(object sender, RoutedEventArgs e)
        {
            model.EdittedImage = Image;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.FilteredImage))
            {
                editor.Initialise(model.FilteredImage);                
            }
        }

        private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditManager.Index))
            {
                OnPropertyChanged(nameof(UndoList));
                OnPropertyChanged(nameof(RedoList));
                UndoComboBox.SelectedIndex = 0;
                RedoComboBox.SelectedIndex = 0;
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EraserButton_Checked(object sender, RoutedEventArgs e)
        {
            ResetStateButtons(sender);
            EditorState = ErasorMode.Instance;
        }

        private void RectButton_Checked(object sender, RoutedEventArgs e)
        {
            ResetStateButtons(sender);
            EditorState = RectMode.Instance;
        }

        private void PolyButton_Checked(object sender, RoutedEventArgs e)
        {
            ResetStateButtons(sender);
            EditorState = PolyMode.Instance;
        }
        private void PencilButton_Checked(object sender, RoutedEventArgs e)
        {
            ResetStateButtons(sender);
            EditorState = PencilMode.Instance;
        }

        private void StateButton_Unchecked(object sender, RoutedEventArgs e)
        {
            EditorState = NoMode.Instance;
        }

        private void UndoComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex <= 0)
                return;
            var targetIndex = EditManager.Index - comboBox.SelectedIndex;
            if (EditManager.GoToCommand.CanExecute(targetIndex))
                EditManager.GoToCommand.Execute(targetIndex);
        }

        private void RedoComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex <= 0)
                return;
            var targetIndex = EditManager.Index + comboBox.SelectedIndex;
            if (EditManager.GoToCommand.CanExecute(targetIndex))
                EditManager.GoToCommand.Execute(targetIndex);
        }

        private void ResetStateButtons(object sender)
        {
            foreach (var button in stateButtons.Where(b => b != sender))
            {
                button.IsChecked = false;
            }
        }

    }
}