using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace PlotDigitizer.WPF.Tests.Utilities.Behaviors
{
	/// <summary>
	/// The two behaviours attached to the axis-limit text boxes: Enter commits the edit, and
	/// focusing the box selects its content so the next keystroke replaces the old limit.
	/// </summary>
	[TestClass]
	public class TextBoxBehaviorTests
	{
		private sealed class Source : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private string text = "original";

			public string Text
			{
				get => text;
				set
				{
					text = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
				}
			}
		}

		private static TextBox CreateBoundTextBox(Source source)
		{
			var textBox = new TextBox();
			textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(Source.Text))
			{
				Source = source,
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
			});
			return textBox;
		}

		private static void PressKey(TextBox textBox, Key key)
		{
			using var presentationSource = new HwndSource(new HwndSourceParameters("PlotDigitizer tests"));
			textBox.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, presentationSource, 0, key)
			{
				RoutedEvent = UIElement.KeyDownEvent,
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void UpdateTextOnKeyDown_Enter_PushesTheEditToTheSource()
		{
			StaTestContext.Run(() =>
			{
				var source = new Source();
				var textBox = CreateBoundTextBox(source);
				new UpdateTextOnKeyDown().Attach(textBox);
				textBox.Text = "edited";
				Assert.AreEqual("original", source.Text, "the binding only commits on lost focus");

				PressKey(textBox, Key.Enter);

				Assert.AreEqual("edited", source.Text);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void UpdateTextOnKeyDown_AnotherKey_LeavesTheSourceAlone()
		{
			StaTestContext.Run(() =>
			{
				var source = new Source();
				var textBox = CreateBoundTextBox(source);
				new UpdateTextOnKeyDown().Attach(textBox);
				textBox.Text = "edited";

				PressKey(textBox, Key.A);

				Assert.AreEqual("original", source.Text);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void UpdateTextOnKeyDown_CustomKey_IsHonoured()
		{
			StaTestContext.Run(() =>
			{
				var source = new Source();
				var textBox = CreateBoundTextBox(source);
				new UpdateTextOnKeyDown { Key = Key.Tab }.Attach(textBox);
				textBox.Text = "edited";

				PressKey(textBox, Key.Enter);
				Assert.AreEqual("original", source.Text, "Enter is no longer the commit key");

				PressKey(textBox, Key.Tab);
				Assert.AreEqual("edited", source.Text);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void UpdateTextOnKeyDown_AfterDetaching_NoLongerCommits()
		{
			StaTestContext.Run(() =>
			{
				var source = new Source();
				var textBox = CreateBoundTextBox(source);
				var behavior = new UpdateTextOnKeyDown();
				behavior.Attach(textBox);
				behavior.Detach();
				textBox.Text = "edited";

				PressKey(textBox, Key.Enter);

				Assert.AreEqual("original", source.Text);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SelectAllTextOnFocus_KeyboardFocus_SelectsEverything()
		{
			StaTestContext.Run(() =>
			{
				var textBox = new TextBox { Text = "123" };
				new SelectAllTextOnFocusBehavior().Attach(textBox);

				textBox.RaiseEvent(new KeyboardFocusChangedEventArgs(Keyboard.PrimaryDevice, 0, null, textBox)
				{
					RoutedEvent = UIElement.GotKeyboardFocusEvent,
				});

				Assert.AreEqual("123", textBox.SelectedText);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SelectAllTextOnFocus_MouseCapture_SelectsEverything()
		{
			StaTestContext.Run(() =>
			{
				var textBox = new TextBox { Text = "123" };
				new SelectAllTextOnFocusBehavior().Attach(textBox);

				textBox.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0)
				{
					RoutedEvent = UIElement.GotMouseCaptureEvent,
				});

				Assert.AreEqual("123", textBox.SelectedText);
			});
		}

		/// <summary>
		/// The first click only focuses the box; swallowing it is what stops the caret from
		/// landing mid-text and clearing the selection.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void SelectAllTextOnFocus_FirstClick_IsSwallowed()
		{
			StaTestContext.Run(() =>
			{
				var textBox = new TextBox { Text = "123" };
				new SelectAllTextOnFocusBehavior().Attach(textBox);
				var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
				{
					RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
				};

				textBox.RaiseEvent(args);

				Assert.IsTrue(args.Handled);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SelectAllTextOnFocus_AfterDetaching_NoLongerSelects()
		{
			StaTestContext.Run(() =>
			{
				var textBox = new TextBox { Text = "123" };
				var behavior = new SelectAllTextOnFocusBehavior();
				behavior.Attach(textBox);
				behavior.Detach();

				textBox.RaiseEvent(new KeyboardFocusChangedEventArgs(Keyboard.PrimaryDevice, 0, null, textBox)
				{
					RoutedEvent = UIElement.GotKeyboardFocusEvent,
				});

				Assert.AreEqual(string.Empty, textBox.SelectedText);
			});
		}
	}
}
