using PropertyChanged;

using System.Collections.Generic;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class EditManager<TObject> : INotifyPropertyChanged
	{
		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Events

		#region Properties

		public TObject CurrentObject => ObjectList[Index];
		public string CurrentTag => TagList[Index];

		[OnChangedMethod(nameof(OnIndexChanged))]
		public int Index { get; private set; }

		public bool IsInitialised => ObjectList != null;

		[OnChangedMethod(nameof(OnObjectListChanged))]
		public List<TObject> ObjectList { get; private set; }


		[OnChangedMethod(nameof(OnTagListChanged))]
		public List<string> TagList { get; private set; }

		public RelayCommand<(TObject obj, string tag)> EditCommand { get; set; }
		public RelayCommand<int> GoToCommand { get; private set; }
		public RelayCommand RedoCommand { get; private set; }
		public RelayCommand UndoCommand { get; private set; }

		#endregion Properties

		#region Constructors

		public EditManager(TObject _object) : this()
		{
			Initialise(_object);
		}

		public EditManager()
		{
			UndoCommand = new RelayCommand(Undo, CanUndo);
			RedoCommand = new RelayCommand(Redo, CanRedo);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
			EditCommand = new RelayCommand<(TObject, string)>(Edit, CanEdit);
		}

		#endregion Constructors

		#region Methods

		public void Initialise(TObject _object)
		{
			ObjectList = new List<TObject>
			{
				_object,
			};
			TagList = new List<string>
			{
				"initialise",
			};
			Index = 0;
		}

		protected virtual bool CanEdit((TObject obj, string tag) arg) => true;

		protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private bool CanGoTo(int targetIndex) => targetIndex >= 0 && targetIndex < TagList.Count;

		private bool CanRedo() => Index < (ObjectList?.Count ?? 0) - 1;

		private bool CanUndo() => Index > 0;

		private void Edit((TObject obj, string tag) edit)
		{
			ObjectList.RemoveRange(Index + 1, ObjectList.Count - Index - 1);
			ObjectList.Add(edit.obj);
			OnPropertyChanged(nameof(ObjectList));

			TagList.RemoveRange(Index + 1, TagList.Count - Index - 1);
			TagList.Add(edit.tag);
			OnPropertyChanged(nameof(TagList));

			Index++;
		}

		private void GoTo(int targetIndex) => Index = targetIndex;

		private void OnIndexChanged()
		{
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		}

		private void OnObjectListChanged() => RedoCommand.RaiseCanExecuteChanged();

		private void OnTagListChanged() => GoToCommand.RaiseCanExecuteChanged();

		private void Redo() => Index++;

		private void Undo() => Index--;

		#endregion Methods
	}
}