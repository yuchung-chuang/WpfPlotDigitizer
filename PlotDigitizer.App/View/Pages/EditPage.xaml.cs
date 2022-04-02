﻿using Emgu.CV;
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

        public bool IsDisabled => model.FilteredImage is null;

        [DependsOn(null, new[] {nameof(IsPencil), nameof(IsEraser), nameof(IsRect), nameof(IsPoly)})]
        public bool IsPencil 
        { 
            get => editor?.EditorState is PencilMode; 
            set => editor.EditorState = PencilMode.Instance; 
        }

        [DependsOn(null, new[] {nameof(IsPencil), nameof(IsEraser), nameof(IsRect), nameof(IsPoly)})]
        public bool IsEraser {
            get => editor?.EditorState is EraserMode;
            set => editor.EditorState = EraserMode.Instance;
        }

        [DependsOn(null, new[] {nameof(IsPencil), nameof(IsEraser), nameof(IsRect), nameof(IsPoly)})]
        public bool IsRect
        {
            get => editor?.EditorState is RectMode;
            set => editor.EditorState = RectMode.Instance;
        }

        [DependsOn(null, new[] {nameof(IsPencil), nameof(IsEraser), nameof(IsRect), nameof(IsPoly)})]
        public bool IsPoly
        {
            get => editor?.EditorState is PolyMode;
            set => editor.EditorState = PolyMode.Instance;
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

        /// <summary>
        /// Need to update the binding expression, otherwise the binding may happens when the <see cref="EditManager"/> has not been really yet
        /// </summary>
        private void EditPage_Loaded(object sender, RoutedEventArgs e)
        {
            IsEnabled = !IsDisabled;
            if (IsDisabled) {
                return;
            }
            UndoButton.GetBindingExpression(ButtonBase.CommandProperty).UpdateTarget();
            RedoButton.GetBindingExpression(ButtonBase.CommandProperty).UpdateTarget();
        }

        /// <summary>
        /// Do NOT initialise it when loading, so long as the <see cref="Model.FilteredImage"/> is un changed, the previous editting is retained. 
        /// </summary>
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.FilteredImage)) {
                editor.Initialise(model.FilteredImage);
            }
        }

        private void EditPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (IsDisabled) {
                return;
            }
            model.EdittedImage = Image;
        }

        private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditManager.Index)) {
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

    }
}