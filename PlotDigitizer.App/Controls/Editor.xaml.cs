using Emgu.CV;
using Emgu.CV.Structure;
using System.ComponentModel;
using System.Diagnostics;
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

		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Editor), new PropertyMetadata());

		public static readonly DependencyProperty BlockInteractionProperty =
			DependencyProperty.Register("BlockInteraction", typeof(bool), typeof(Editor), new PropertyMetadata(false));

		private readonly double eraserOriginalSize;
		private readonly double eraserOriginalStrokeThickness;
		private readonly double pencilOriginalSize;
		private readonly double pencilOriginalStrokeThickness;
		private Image<Rgba, byte> image;

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

		public ImageSource ImageSource
		{
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public bool BlockInteraction
		{
			get { return (bool)GetValue(BlockInteractionProperty); }
			set { SetValue(BlockInteractionProperty, value); }
		}

		public Image<Rgba, byte> Image
		{
			get => image;
			set
			{
				image = value;
				ImageSource = image?.ToBitmapSource();
			}
		}

		public EditManager<Image<Rgba, byte>> EditManager { get; set; }

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
				}
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
		}

		public Editor()
		{
			InitializeComponent();
			Loaded += Editor_Loaded;
			Unloaded += Editor_Unloaded;
			PropertyChanged += Editor_PropertyChanged;
			eraserOriginalSize = eraserRect.Width;
			eraserOriginalStrokeThickness = eraserRect.StrokeThickness;
			pencilOriginalSize = pencilPointer.Width;
			pencilOriginalStrokeThickness = pencilPointer.StrokeThickness;
			mainGrid.DataContext = this;
		}

		private void Editor_Loaded(object sender, RoutedEventArgs e)
		{
			var mainWindow = Application.Current.MainWindow;
			mainWindow.PreviewKeyDown += MainWindow_KeyDown;
			undoCommandBinding = new CommandBinding(ApplicationCommands.Undo,
				(s, e) => EditManager.UndoCommand.Execute(null),
				(s, e) => e.CanExecute = EditManager.UndoCommand.CanExecute(null));
			redoCommandBinding = new CommandBinding(ApplicationCommands.Redo,
				(s, e) => EditManager.RedoCommand.Execute(null),
				(s, e) => e.CanExecute = EditManager.RedoCommand.CanExecute(null));
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

		private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
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

		private void PanZoomGrid_MouseWheel(object sender, double scale)
		{
			eraserRect.Width = eraserOriginalSize / scale;
			eraserRect.StrokeThickness = eraserOriginalStrokeThickness / scale;
			pencilPointer.Width = pencilOriginalSize / scale;
			pencilPointer.StrokeThickness = pencilOriginalStrokeThickness / scale;
		}

		private void OnPropertyChanged(string propertyName)
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