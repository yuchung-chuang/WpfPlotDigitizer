using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PlotDigitizer.App
{
	public class EditPageViewModel : ViewModelBase
	{        
        public bool Enabled => Model != null && Model.FilteredImage != null;

        
        public Model Model { get; }
		public EditPageViewModel()
		{

		}
		public EditPageViewModel(Model model) : this()
		{
			Model = model;
            model.PropertyChanged += Model_PropertyChanged;
        }
        /// <summary>
        /// Do NOT initialise it when loading, so long as the <see cref="Model.FilteredImage"/> is un changed, the previous editting is retained. 
        /// </summary>
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.FilteredImage)) {
                OnPropertyChanged(nameof(Enabled));
            }
        }

    }
}
