using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PlotDigitizer.Core
{
	public class EditPageViewModel : PageViewModelBase
	{
		#region Properties

		public EditManager<Image<Rgba, byte>> EditManager { get; set; } = new EditManager<Image<Rgba, byte>>();
		public Image<Rgba, byte> Image { get; set; }
		public bool IsEnabled => Model != null && Model.FilteredImage != null;
		public Model Model { get; private set; }
		public IEnumerable<string> RedoList => EditManager?.TagList?.GetRange(EditManager.Index, EditManager.TagList.Count - EditManager.Index);
		public RelayCommand<int> RedoToCommand { get; set; }
		public IEnumerable<string> UndoList => EditManager?.TagList?.GetRange(0, EditManager.Index + 1).Reverse<string>();
		public RelayCommand<int> UndoToCommand { get; set; }

		#endregion Properties

		#region Constructors

		public EditPageViewModel()
		{
			Name = "EditPage";
			UndoToCommand = new RelayCommand<int>(UndoTo);
			RedoToCommand = new RelayCommand<int>(RedoTo);
			EditManager.PropertyChanged += EditManager_PropertyChanged;
		}

		public EditPageViewModel(Model model) : this()
		{
			Model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}

		#endregion Constructors

		#region Methods

		public override void Enter() => base.Enter();

		public override void Leave()
		{
			base.Leave();
			Model.EdittedImage = Image;
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditManager.Index)) {
				OnPropertyChanged(nameof(UndoList));
				OnPropertyChanged(nameof(RedoList));
			}
		}

		/// <summary>
		/// Do NOT initialise it when loading, so long as the <see cref="Model.FilteredImage"/> is un changed, the previous editting is retained.
		/// </summary>
		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Core.Model.FilteredImage)) {
				OnPropertyChanged(nameof(IsEnabled));
				EditManager.Initialise(Model.FilteredImage);
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