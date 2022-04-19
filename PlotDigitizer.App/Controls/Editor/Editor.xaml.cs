using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using PropertyChanged;
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

		public static readonly DependencyProperty EditorStateProperty =
			DependencyProperty.Register("EditorState", typeof(EditorState), typeof(Editor), new PropertyMetadata(NoMode.Instance, OnEditorStateChanged));

		public static readonly DependencyProperty EditManagerProperty =
			DependencyProperty.Register("EditManager", typeof(EditManager<Image<Rgba, byte>>), typeof(Editor), new PropertyMetadata(default));

		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(Image<Rgba,byte>), typeof(Editor), new PropertyMetadata(default));

		public double ZoomScale { get; set; }
		public double EraserSize => ImageControl.ActualWidth * 0.05 / ZoomScale;
		public double PencilSize => ImageControl.ActualWidth * 0.01 / ZoomScale;
		public double EraserStrokeSize => 1.5 / ZoomScale;
		public double PencilStrokeSize => 1.5 / ZoomScale;
		public double SelectRectStrokeSize => 1.5 / ZoomScale;
		public double SelectPolyStrokeSize => 1.5 / ZoomScale;

		private CommandBinding undoCommandBinding;

		private CommandBinding redoCommandBinding;
		private EditManager<Image<Rgba, byte>> editManager;

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

		public Image<Rgba,byte> Image
		{
			get { return (Image<Rgba,byte>)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		public EditManager<Image<Rgba, byte>> EditManager
		{
			get { return (EditManager<Image<Rgba, byte>>)GetValue(EditManagerProperty); }
			set { SetValue(EditManagerProperty, value); }
		}

		public EditorState EditorState
		{
			get { return (EditorState)GetValue(EditorStateProperty); }
			set { SetValue(EditorStateProperty, value); }
		}

		[OnChangedMethod(nameof(OnEdittingStateChanged))]
		public EdittingState EdittingState { get; set; } = NotEditting.Instance;

		public Editor()
		{
			InitializeComponent();
			Loaded += Editor_Loaded;
			Unloaded += Editor_Unloaded;
			ImageControl.SizeChanged += ImageControl_SizeChanged;
		}

		[SuppressPropertyChangedWarnings]
		private static void OnEditorStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is Editor editor)) {
				return;
			}
			editor.EditorState.Enter(editor);
		}

		private void OnEdittingStateChanged()
		{
			EdittingState.Enter(this);
			UpdateVisibility();
		}

		/// <summary>
		/// This makes sure <see cref="ImageControl.ActualWidth"/> is evaludated.
		/// </summary>
		private void ImageControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			OnPropertyChanged(nameof(EraserSize));
			OnPropertyChanged(nameof(PencilSize));
		}


		public void Initialise(Image<Rgba, byte> image)
		{
			Image = image.Copy();
		}

		private void UpdateVisibility()
		{
			switch (EdittingState) {
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

			editManager = EditManager; // keep a reference for unsubscription when unloading
			editManager.PropertyChanged += EditManager_PropertyChanged;
		}

		private void Editor_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!(Application.Current.MainWindow is Window mainWindow)) {
				return;
			}
			mainWindow.PreviewKeyDown -= MainWindow_KeyDown;

			editManager.PropertyChanged -= EditManager_PropertyChanged;
		}

		private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
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

		private void MainGrid_MouseMove(object sender, MouseEventArgs e)
		{
			EditorState.MouseMove(this, e);
			EdittingState.MouseMove(this, e);
		}

		private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
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