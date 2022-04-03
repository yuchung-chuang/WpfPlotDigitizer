using Emgu.CV;
using Emgu.CV.Structure;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for Editor.xaml
	/// </summary>
	public partial class Editor : UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty MouseButtonProperty =
			DependencyProperty.Register("MouseButton", typeof(MouseButton), typeof(Editor), new PropertyMetadata(MouseButton.Left));

		public static readonly DependencyProperty ModifierKeysProperty =
			DependencyProperty.Register("ModifierKeys", typeof(ModifierKeys), typeof(Editor), new PropertyMetadata(ModifierKeys.None));

		public static readonly DependencyProperty DeleteKeysProperty =
			DependencyProperty.Register("DeleteKeys", typeof(Key), typeof(Editor), new PropertyMetadata(Key.Delete | Key.Back));

		public static readonly DependencyProperty BlockInteractionProperty =
			DependencyProperty.Register("BlockInteraction", typeof(bool), typeof(Editor), new PropertyMetadata(false));

		public double ZoomScale { get; set; }
		public double EraserSize => ImageControl.ActualWidth * 0.05 / ZoomScale;
		public double PencilSize => ImageControl.ActualWidth * 0.01 / ZoomScale;
		public double EraserStrokeSize => 1.5 / ZoomScale;
		public double PencilStrokeSize => 1.5 / ZoomScale;
		public double SelectRectStrokeSize => 1.5 / ZoomScale;
		public double SelectPolyStrokeSize => 1.5 / ZoomScale;

		private EditorState editorState = NoMode.Instance;

		private EdittingState edittingState = NotEditting.Instance;

		private CommandBinding undoCommandBinding;

		private CommandBinding redoCommandBinding;

		public event PropertyChangedEventHandler PropertyChanged;

		public MouseButton MouseButton
		{
			get { return (MouseButton)GetValue(MouseButtonProperty); }
			set { SetValue(MouseButtonProperty, value); }
		}

		public ModifierKeys ModifierKeys
		{
			get { return (ModifierKeys)GetValue(ModifierKeysProperty); }
			set { SetValue(ModifierKeysProperty, value); }
		}

		public Key DeleteKeys
		{
			get { return (Key)GetValue(DeleteKeysProperty); }
			set { SetValue(DeleteKeysProperty, value); }
		}

		public bool BlockInteraction
		{
			get { return (bool)GetValue(BlockInteractionProperty); }
			set { SetValue(BlockInteractionProperty, value); }
		}

		public ImageSource ImageSource => Image?.ToBitmapSource();

		public Image<Rgba, byte> Image { get; private set; }


		public EditManager<Image<Rgba, byte>> EditManager { get; } = new EditManager<Image<Rgba, byte>>();

		public EditorState EditorState
		{
			get => editorState;
			set
			{
				if (editorState != value) {
					editorState = value;
					editorState.Enter(this);
				}
			}
		}

		public EdittingState EdittingState
		{
			get => edittingState;
			set
			{
				if (edittingState != value) {
					edittingState = value;
					edittingState.Enter(this);
					UpdateVisibility(value);
				}
			}
		}

		public Editor()
		{
			InitializeComponent();
			Loaded += Editor_Loaded;
			Unloaded += Editor_Unloaded;
			EditManager.PropertyChanged += EditManager_PropertyChanged;
			ImageControl.Loaded += ImageControl_Loaded;
		}

		/// <summary>
		/// This makes sure <see cref="ImageControl.ActualWidth"/> is evaludated.
		/// </summary>
		private void ImageControl_Loaded(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged(nameof(EraserSize));
			OnPropertyChanged(nameof(PencilSize));
		}

		public void Initialise(Image<Rgba, byte> image)
		{
			Image = image.Copy();
			EditManager.Initialise(image);
		}

		private void UpdateVisibility(EdittingState value)
		{
			switch (value) {
				case NotEditting _:
					selectRect.Visibility = Visibility.Hidden;
					selectPoly.Visibility = Visibility.Hidden;
					eraserRect.Visibility = Visibility.Hidden;
					pencilPointer.Visibility = Visibility.Hidden;
					break;
				case RectSelected _:
				case RectSelecting _:
					selectRect.Visibility = Visibility.Visible;
					break;
				case PolySelected _:
				case PolySelecting _:
					selectPoly.Visibility = Visibility.Visible;
					break;
				case Erasing _:
					eraserRect.Visibility = Visibility.Visible;
					break;
				case Drawing _:
					pencilPointer.Visibility = Visibility.Visible;
					break;
				default:
					break;
			}
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditManager.Index)) {
				Image = EditManager.CurrentObject.Copy();
			}
		}

		private void Editor_Loaded(object sender, RoutedEventArgs e)
		{
			var mainWindow = Application.Current.MainWindow;
			mainWindow.PreviewKeyDown += MainWindow_KeyDown;
			undoCommandBinding = new CommandBinding(ApplicationCommands.Undo,
				(s, e) => EditManager.UndoCommand.Execute(),
				(s, e) => e.CanExecute = EditManager.UndoCommand.CanExecute());
			redoCommandBinding = new CommandBinding(ApplicationCommands.Redo,
				(s, e) => EditManager.RedoCommand.Execute(),
				(s, e) => e.CanExecute = EditManager.RedoCommand.CanExecute());
			mainWindow.CommandBindings.Add(undoCommandBinding);
			mainWindow.CommandBindings.Add(redoCommandBinding);
		}

		private void Editor_Unloaded(object sender, RoutedEventArgs e)
		{
			var mainWindow = Application.Current.MainWindow;
			mainWindow.PreviewKeyDown -= MainWindow_KeyDown;
			mainWindow.CommandBindings.Remove(undoCommandBinding);
			mainWindow.CommandBindings.Remove(redoCommandBinding);
		}

		private void mainGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!MouseDownInputCheck()) {
				return;
			}

			EditorState.MouseDown(this, e);
			EdittingState.MouseDown(this, e);

			if (BlockInteraction) {
				e.Handled = true;
			}
		}

		private void mainGrid_MouseMove(object sender, MouseEventArgs e)
		{
			EditorState.MouseMove(this, e);
			EdittingState.MouseMove(this, e);
		}

		private void mainGrid_MouseUp(object sender, MouseButtonEventArgs e)
		{
			EditorState.MouseUp(this, e);
			EdittingState.MouseUp(this, e);
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (!DeleteInputCheck(e)) {
				return;
			}

			EditorState.KeyDown(this, e);
			EdittingState.KeyDown(this, e);

			if (BlockInteraction) {
				e.Handled = true;
			}
		}

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private bool MouseDownInputCheck()
		{
			return IsMouseButtonPressed(MouseButton) && IsKeyPressed(ModifierKeys);

			static bool IsKeyPressed(ModifierKeys key)
			{
				return key == ModifierKeys.None || Contains(Keyboard.Modifiers, key);

				static bool Contains(ModifierKeys a, ModifierKeys b)
				{
					return (a & b) == b;
				}
			}

			static bool IsMouseButtonPressed(MouseButton mouseButton)
			{
				return mouseButton switch
				{
					MouseButton.Left => Mouse.LeftButton == MouseButtonState.Pressed,
					MouseButton.Right => Mouse.RightButton == MouseButtonState.Pressed,
					MouseButton.Middle => Mouse.MiddleButton == MouseButtonState.Pressed,
					MouseButton.XButton1 => Mouse.XButton1 == MouseButtonState.Pressed,
					MouseButton.XButton2 => Mouse.XButton2 == MouseButtonState.Pressed,
					_ => true,
				};
			}
		}

		private bool DeleteInputCheck(KeyEventArgs e)
		{
			return Contain(DeleteKeys, e.Key);

			static bool Contain(Key a, Key b)
			{
				return (a & b) == b;
			}
		}
	}
}