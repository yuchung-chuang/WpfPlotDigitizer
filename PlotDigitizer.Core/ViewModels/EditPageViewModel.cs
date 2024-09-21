using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PlotDigitizer.Core
{
	// TODO: Clear Border checkbox, allowing user to decide.
	public class EditPageViewModel : PageViewModelBase
	{
		#region Properties

		public EditManager<Image<Rgba, byte>> EditManager { get; set; }
		public Image<Rgba, byte> Image { get; set; }
		public Model Model { get; private set; }
		public bool IsEnabled => Model != null && Model.FilteredImage != null;
		public IEnumerable<string> RedoList => EditManager?.TagList?.GetRange(EditManager.Index, EditManager.TagList.Count - EditManager.Index);
		public IEnumerable<string> UndoList => EditManager?.TagList?.GetRange(0, EditManager.Index + 1).Reverse<string>();
		public RelayCommand<int> RedoToCommand { get; set; }
		public RelayCommand<int> UndoToCommand { get; set; }

		#endregion Properties

		#region Constructors

		public EditPageViewModel()
		{
			Name = "EditPage";
			UndoToCommand = new RelayCommand<int>(UndoTo);
			RedoToCommand = new RelayCommand<int>(RedoTo);
			EditManager = new EditManager<Image<Rgba, byte>>();
			EditManager.PropertyChanged += EditManager_PropertyChanged;
		}

		public EditPageViewModel(Model model, 
			IImageService imageService) : this()
		{
			Model = model;
			var image = Model.FilteredImage; 
			image = imageService.ClearBorder(image);
			EditManager.Initialise(image);
		}

		#endregion Constructors

		#region Methods

		public override void Enter() => base.Enter();

		public override void Leave()
		{
			base.Leave();
			if (!IsEnabled) {
				return;
			}
			Model.EdittedImage = Image;
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditManager.Index)) {
				OnPropertyChanged(nameof(UndoList));
				OnPropertyChanged(nameof(RedoList));
			}
		}


		private void RedoTo(int index)
		{
			if (index <= 0)
				return;
			var targetIndex = EditManager.Index + index;
			if (EditManager.GoToCommand.CanExecute(targetIndex))
				EditManager.GoToCommand.Execute(targetIndex);
		}

		private void UndoTo(int index)
		{
			if (index <= 0)
				return;
			var targetIndex = EditManager.Index - index;
			if (EditManager.GoToCommand.CanExecute(targetIndex))
				EditManager.GoToCommand.Execute(targetIndex);
		}

		#endregion Methods
	}
}