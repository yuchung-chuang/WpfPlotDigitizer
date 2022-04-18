using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PlotDigitizer.Core
{
	public class EditPageViewModel : PageViewModelBase
	{
        public IEnumerable<string> UndoList => EditManager?.TagList?.GetRange(0, EditManager.Index + 1).Reverse<string>();

        public IEnumerable<string> RedoList => EditManager?.TagList?.GetRange(EditManager.Index, EditManager.TagList.Count - EditManager.Index);

        public EditManager<Image<Rgba, byte>> EditManager { 
            get; 
            set; 
        } = new EditManager<Image<Rgba, byte>>();

        public Model Model { get; private set; }
        public bool IsEnabled => Model != null && Model.FilteredImage != null;
        
		public EditPageViewModel()
		{
            Name = "EditPage";
            EditManager.PropertyChanged += EditManager_PropertyChanged;
		}
		public EditPageViewModel(Model model) : this()
		{
			Model = model;
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.FilteredImage)) {
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditManager.Index)) {
                OnPropertyChanged(nameof(UndoList));
                OnPropertyChanged(nameof(RedoList));
            }
        }
    }
}
