using PropertyChanged;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;

namespace PlotDigitizer.Core
{
	public class EditService<TObject> : IEditService<TObject>
	{
		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Events

		#region Properties

		public int Index { get; private set; }

		public IList<TObject> ObjectList { get; private set; }
		
		/// <summary>
		/// The tag to indicate editting command
		/// </summary>
		public IList<string> TagList { get; private set; }
		
		public TObject CurrentObject => ObjectList[Index];
		public string CurrentTag => TagList[Index];
		public bool IsInitialised => ObjectList != null;

		#endregion Properties

		#region Constructors
		public EditService()
		{
			
		}

		public EditService(TObject _object) : this()
		{
			Initialise(_object);
		}


		#endregion Constructors

		#region Methods

		public void Initialise(TObject _object)
		{
			ObjectList = [ _object ];
			TagList = [ "initialise" ];
			Index = 0;
		}

		public virtual bool CanEdit((TObject obj, string tag) arg) => IsInitialised;

		public bool CanGoTo(int targetIndex) => IsInitialised && targetIndex >= 0 && targetIndex < TagList.Count;

		public bool CanRedo() => IsInitialised && Index < (ObjectList?.Count ?? 0) - 1;

		public bool CanUndo() => IsInitialised && Index > 0;

		public void Edit((TObject obj, string tag) edit)
		{
			var i = Index + 1;
			while (ObjectList.Count > i) {
				ObjectList.RemoveAt(i);
			}
			ObjectList.Add(edit.obj);

			while (TagList.Count > i) {
				TagList.RemoveAt(i);
			}
			TagList.Add(edit.tag);

			Index++;
		}

		public void GoTo(int targetIndex) => Index = targetIndex;
		public void Redo() => Index++;
		public void Undo() => Index--;

		protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		#endregion Methods
	}
}