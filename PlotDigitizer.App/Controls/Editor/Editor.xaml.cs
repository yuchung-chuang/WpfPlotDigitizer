using Emgu.CV;
using Emgu.CV.Structure;

using PlotDigitizer.Core;

using PropertyChanged;

using System.Collections.Generic;
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
		#region Fields

		public static readonly DependencyProperty BlockInteractionProperty =
			DependencyProperty.Register("BlockInteraction", typeof(bool), typeof(Editor), new PropertyMetadata(false));

		public static readonly DependencyProperty DeleteKeysProperty =
			DependencyProperty.Register("DeleteKeys", typeof(InputGestureCollection), typeof(Editor), new PropertyMetadata(new InputGestureCollection(new List<InputGesture> { new KeyGesture(Key.Back), new KeyGesture(Key.Delete) })));

		public static readonly DependencyProperty EditGestureProperty = DependencyProperty.Register(nameof(EditGesture), typeof(MouseGesture), typeof(Editor), new PropertyMetadata(new MouseGesture(MouseAction.LeftClick)));

		public static readonly DependencyProperty EditManagerProperty =
			DependencyProperty.Register("EditManager", typeof(EditManager<Image<Rgba, byte>>), typeof(Editor), new PropertyMetadata(default));

		public static readonly DependencyProperty EditorModeProperty =
			DependencyProperty.Register("EditorMode", typeof(EditorMode), typeof(Editor), new PropertyMetadata(EditorMode.NoMode, OnEditorModeChanged));

		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(Image<Rgba, byte>), typeof(Editor), new PropertyMetadata(default));

		public static readonly DependencyProperty SelectedGestureProperty = DependencyProperty.Register("SelectedGesture", typeof(MouseGesture), typeof(Editor), new PropertyMetadata(new MouseGesture(MouseAction.LeftDoubleClick)));

		private EditManager<Image<Rgba, byte>> editManager;
		private Window window;

		#endregion Fields

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Events

		#region Properties

		public bool BlockInteraction
		{
			get => (bool)GetValue(BlockInteractionProperty);
			set => SetValue(BlockInteractionProperty, value);
		}

		public InputGestureCollection DeleteKeys
		{
			get => (InputGestureCollection)GetValue(DeleteKeysProperty);
			set => SetValue(DeleteKeysProperty, value);
		}

		public MouseGesture EditGesture
		{
			get => (MouseGesture)GetValue(EditGestureProperty);
			set => SetValue(EditGestureProperty, value);
		}

		public EditManager<Image<Rgba, byte>> EditManager
		{
			get => (EditManager<Image<Rgba, byte>>)GetValue(EditManagerProperty);
			set => SetValue(EditManagerProperty, value);
		}

		public EditorMode EditorMode
		{
			get => (EditorMode)GetValue(EditorModeProperty);
			set => SetValue(EditorModeProperty, value);
		}

		[OnChangedMethod(nameof(OnEdittingStateChanged))]
		public EdittingState EdittingState { get; set; } = EdittingState.NotEditting;

		public double EraserSize => ImageControl.ActualWidth * 0.05 / ZoomScale;
		public double EraserStrokeSize => 1.5 / ZoomScale;

		public Image<Rgba, byte> Image
		{
			get => (Image<Rgba, byte>)GetValue(ImageProperty);
			set => SetValue(ImageProperty, value);
		}

		public ImageSource ImageSource => Image?.ToBitmapSource();
		public double PencilSize => ImageControl.ActualWidth * 0.01 / ZoomScale;
		public double PencilStrokeSize => 1.5 / ZoomScale;

		public MouseGesture SelectedGesture
		{
			get => (MouseGesture)GetValue(SelectedGestureProperty);
			set => SetValue(SelectedGestureProperty, value);
		}

		public double SelectPolyStrokeSize => 1.5 / ZoomScale;
		public double SelectRectStrokeSize => 1.5 / ZoomScale;
		public double ZoomScale { get; set; }

		#endregion Properties

		#region Constructors

		public Editor()
		{
			InitializeComponent();
			Loaded += Editor_Loaded;
			Unloaded += Editor_Unloaded;
			ImageControl.SizeChanged += ImageControl_SizeChanged;
		}

		#endregion Constructors

		#region Methods

		private void Editor_Loaded(object sender, RoutedEventArgs e)
		{
			if (!EditManager.IsInitialised) {
				return;
			}

			editManager = EditManager;
			Image = EditManager.CurrentObject.Copy();
			EditManager.PropertyChanged += EditManager_PropertyChanged;

			window = Window.GetWindow(this);
			window.PreviewKeyDown += MainWindow_KeyDown;

			EdittingState.PolySelecting.EditGesture = EditGesture;
			EdittingState.PolySelecting.SelectedGesture = SelectedGesture;
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditManager.CurrentObject)) {
				Image = EditManager.CurrentObject.Copy();
				Debug.WriteLine("CurrentObject changed.");
			}
			if (e.PropertyName == nameof(EditManager.Index)) {
				Debug.WriteLine("Index changed.");
			}
		}

		private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!EditGesture.Matches(sender, e) && !SelectedGesture.Matches(sender, e)) {
				return;
			}

			EditorMode.MouseDown(this, e);
			EdittingState.MouseDown(this, e);

			if (BlockInteraction) {
				e.Handled = true;
			}
		}

		private void MainGrid_MouseMove(object sender, MouseEventArgs e)
		{
			EditorMode.MouseMove(this, e);
			EdittingState.MouseMove(this, e);
		}

		private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
		{
			EditorMode.MouseUp(this, e);
			EdittingState.MouseUp(this, e);
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (!DeleteKeysMatchAny(sender, e)) {
				return;
			}

			EditorMode.KeyDown(this, e);
			EdittingState.KeyDown(this, e);

			if (BlockInteraction) {
				e.Handled = true;
			}

			bool DeleteKeysMatchAny(object sender, KeyEventArgs e)
			{
				foreach (KeyGesture key in DeleteKeys) {
					if (key.Matches(sender, e)) {
						return true;
					}
				}
				return false;
			}
		}

		private void OnEdittingStateChanged()
		{
			EdittingState.Enter(this);
			UpdateVisibility();
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

		/// <summary>
		/// This makes sure <see cref="ImageControl.ActualWidth"/> is evaludated.
		/// </summary>
		private void ImageControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			OnPropertyChanged(nameof(EraserSize));
			OnPropertyChanged(nameof(PencilSize));
		}
		private void Editor_Unloaded(object sender, RoutedEventArgs e)
		{
			if (window is not null)
				window.PreviewKeyDown -= MainWindow_KeyDown;
			if (editManager is not null)
				editManager.PropertyChanged -= EditManager_PropertyChanged;
		}

		[SuppressPropertyChangedWarnings]
		private static void OnEditorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is Editor editor)) {
				return;
			}
			editor.EditorMode.Enter(editor);
		}

		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		#endregion Methods
	}
}