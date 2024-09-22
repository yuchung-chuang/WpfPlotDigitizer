using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace PlotDigitizer.Core
{
	public class EditPageViewModel : PageViewModelBase, IDisposable
	{
		private readonly IImageService imageService;

		#region Properties
		public IEditService<Image<Rgba, byte>> EditService { get; set; }
		public Image<Rgba, byte> Image { get; set; }
		public Model Model { get; private set; }
		public bool IsEnabled => Model != null && Model.FilteredImage != null;
		public IEnumerable<string> RedoList
		{
			get {
				if (!EditService.IsInitialised)
					yield break;
				for (int i = EditService.Index; i < EditService.TagList.Count; i++) {
					yield return EditService.TagList[i];
				}
			}
		}

		public IEnumerable<string> UndoList
		{
			get {
				if (!EditService.IsInitialised)
					yield break;
				for (var i = EditService.Index; i >= 0; i--) {
					yield return EditService.TagList[i];
				}
			}
		}

		public RelayCommand<int> RedoToCommand { get; private set; }
		public RelayCommand<int> UndoToCommand { get; private set; }
		public RelayCommand<(Image<Rgba, byte> obj, string tag)> EditCommand { get; private set; }
		public RelayCommand<int> GoToCommand { get; private set; }
		public RelayCommand RedoCommand { get; private set; }
		public RelayCommand UndoCommand { get; private set; }
        public RelayCommand ClearBorderCommand { get; set; }

        #endregion Properties

        #region Constructors

        public EditPageViewModel()
		{
			Name = "EditPage";
			UndoCommand = new RelayCommand(Undo, CanUndo);
			RedoCommand = new RelayCommand(Redo, CanRedo);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
			EditCommand = new RelayCommand<(Image<Rgba, byte>, string)>(Edit, CanEdit);
			UndoToCommand = new RelayCommand<int>(UndoTo);
			RedoToCommand = new RelayCommand<int>(RedoTo);
			ClearBorderCommand = new RelayCommand(ClearBorder);
		}


		public EditPageViewModel(Model model,
			IImageService imageService,
			IEditService<Image<Rgba, byte>> editService) : this()
		{
			Model = model;
			this.imageService = imageService;
			EditService = editService;
		}
		#endregion Constructors

		#region Methods
		public override void Enter()
		{
			if (!IsEnabled) 
				return;
			if (EditService.IsInitialised) 
				return;
			
			base.Enter();
			EditService.Initialise(Model.FilteredImage);
			EditService.PropertyChanged += EditService_PropertyChanged;
			EditService.ObjectList.CollectionChanged += ObjectList_CollectionChanged;
		}

		public override void Leave()
		{
			if (!IsEnabled) {
				return;
			}
			base.Leave();
			Model.EdittedImage = Image;
		}
		private bool CanEdit((Image<Rgba, byte> obj, string tag) arg) => EditService.CanEdit(arg);
		private bool CanGoTo(int targetIndex) => EditService.CanGoTo(targetIndex);
		private bool CanRedo() => EditService.CanRedo();
		private bool CanUndo() => EditService.CanUndo();
		private void Edit((Image<Rgba, byte> obj, string tag) edit) => EditService.Edit(edit);
		private void GoTo(int targetIndex) => EditService.GoTo(targetIndex);
		private void Redo() => EditService.Redo();
		private void Undo() => EditService.Undo();
		private void RedoTo(int index)
		{
			if (index <= 0)
				return;
			var targetIndex = EditService.Index + index;
			if (EditService.CanGoTo(targetIndex))
				EditService.GoTo(targetIndex);
		}
		private void UndoTo(int index)
		{
			if (index <= 0)
				return;
			var targetIndex = EditService.Index - index;
			if (EditService.CanGoTo(targetIndex))
				EditService.GoTo(targetIndex);
		}
		private void ClearBorder()
		{
			var image = imageService.ClearBorder(EditService.CurrentObject);
			Edit((image, "Clear Border"));
		}

		private void EditService_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditService.Index)) {
				OnPropertyChanged(nameof(UndoList));
				OnPropertyChanged(nameof(RedoList));
				UndoCommand.RaiseCanExecuteChanged();
				RedoCommand.RaiseCanExecuteChanged();
			}
		}
		private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		}

		public void Dispose()
		{
			EditService.PropertyChanged -= EditService_PropertyChanged;
			if (EditService.ObjectList is not null)
				EditService.ObjectList.CollectionChanged -= ObjectList_CollectionChanged;
			GC.SuppressFinalize(this);
		}

		#endregion Methods
	}
}