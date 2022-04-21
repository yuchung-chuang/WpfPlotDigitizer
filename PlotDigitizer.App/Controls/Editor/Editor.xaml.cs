﻿using Emgu.CV;
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
		#region Fields

		public static readonly DependencyProperty BlockInteractionProperty =
			DependencyProperty.Register("BlockInteraction", typeof(bool), typeof(Editor), new PropertyMetadata(false));

		public static readonly DependencyProperty DeleteKeyProperty = DependencyProperty.Register("DeleteKey", typeof(KeyGesture), typeof(Editor), new PropertyMetadata(new KeyGesture(Key.Delete)));

		public static readonly DependencyProperty EditGestureProperty = DependencyProperty.Register(nameof(EditGesture), typeof(MouseGesture), typeof(Editor), new PropertyMetadata(new MouseGesture(MouseAction.LeftClick)));

		public static readonly DependencyProperty EditManagerProperty =
			DependencyProperty.Register("EditManager", typeof(EditManager<Image<Rgba, byte>>), typeof(Editor), new PropertyMetadata(default));

		public static readonly DependencyProperty EditorStateProperty =
			DependencyProperty.Register("EditorState", typeof(EditorState), typeof(Editor), new PropertyMetadata(EditorState.NoMode, OnEditorStateChanged));

		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(Image<Rgba, byte>), typeof(Editor), new PropertyMetadata(default));

		public static readonly DependencyProperty SelectedGestureProperty = DependencyProperty.Register("SelectedGesture", typeof(MouseGesture), typeof(Editor), new PropertyMetadata(new MouseGesture(MouseAction.LeftDoubleClick)));

		private EditManager<Image<Rgba, byte>> editManager;

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

		public KeyGesture DeleteKey
		{
			get => (KeyGesture)GetValue(DeleteKeyProperty);
			set => SetValue(DeleteKeyProperty, value);
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

		public EditorState EditorState
		{
			get => (EditorState)GetValue(EditorStateProperty);
			set => SetValue(EditorStateProperty, value);
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

		public void Initialise(Image<Rgba, byte> image) => Image = image.Copy();

		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		[SuppressPropertyChangedWarnings]
		private static void OnEditorStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is Editor editor)) {
				return;
			}
			editor.EditorState.Enter(editor);
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

			EdittingState.PolySelecting.EditGesture = EditGesture;
			EdittingState.PolySelecting.SelectedGesture = SelectedGesture;
		}

		private void Editor_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!(Application.Current.MainWindow is Window mainWindow)) {
				return;
			}
			mainWindow.PreviewKeyDown -= MainWindow_KeyDown;

			editManager.PropertyChanged -= EditManager_PropertyChanged;
		}

		/// <summary>
		/// This makes sure <see cref="ImageControl.ActualWidth"/> is evaludated.
		/// </summary>
		private void ImageControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			OnPropertyChanged(nameof(EraserSize));
			OnPropertyChanged(nameof(PencilSize));
		}

		private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!EditGesture.Matches(sender, e) && !SelectedGesture.Matches(sender, e)) {
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
			if (!DeleteKey.Matches(sender, e)) {
				return;
			}

			EditorState.KeyDown(this, e);
			EdittingState.KeyDown(this, e);

			if (BlockInteraction) {
				e.Handled = true;
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

		#endregion Methods
	}
}