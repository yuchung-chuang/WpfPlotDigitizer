using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlotDigitizer.Core.Tests.Wpf.Controls
{
	/// <summary>
	/// The modal progress window shown by <see cref="AwaitTaskService"/>. Its contract is the
	/// bindable progress state and the cancel button that both reports and closes.
	/// </summary>
	[TestClass]
	public class ProgressPopupTests
	{
		private static Button CancelButton(ProgressPopup popup)
			=> ((StackPanel)popup.Content).Children.OfType<Button>().Single();

		[TestMethod]
		[TestCategory("Unit")]
		public void Defaults_AreAnIndeterminateEmptyProgress()
		{
			StaTestContext.Run(() =>
			{
				var popup = new ProgressPopup();

				Assert.IsTrue(popup.IsIndeterminate, "indeterminate");
				Assert.AreEqual(0, popup.Value, "value");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Value_WhenSet_NotifiesTheProgressBar()
		{
			StaTestContext.Run(() =>
			{
				var popup = new ProgressPopup();
				var notified = 0;
				((INotifyPropertyChanged)popup).PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == nameof(ProgressPopup.Value)) {
						notified++;
					}
				};

				popup.Value = 42;

				Assert.AreEqual(42, popup.Value, "value");
				Assert.AreEqual(1, notified, "notification count");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CancelButton_WhenClicked_ReportsCancellationAndClosesTheWindow()
		{
			StaTestContext.Run(() =>
			{
				var popup = new ProgressPopup();
				var canceled = 0;
				popup.Canceled += (s, e) => canceled++;
				popup.Show();

				CancelButton(popup).RaiseEvent(new System.Windows.RoutedEventArgs(ButtonBase.ClickEvent));

				Assert.AreEqual(1, canceled, "cancellation count");
				Assert.IsFalse(popup.IsVisible, "the popup must close itself");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CancelButton_WithoutASubscriber_StillCloses()
		{
			StaTestContext.Run(() =>
			{
				var popup = new ProgressPopup();
				popup.Show();

				CancelButton(popup).RaiseEvent(new System.Windows.RoutedEventArgs(ButtonBase.ClickEvent));

				Assert.IsFalse(popup.IsVisible);
			});
		}
	}
}
