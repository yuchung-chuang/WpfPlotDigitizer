using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Collections.Generic;

namespace PlotDigitizer.WPF.Tests.Themes
{
	/// <summary>
	/// The range slider behind the colour filter. These tests exercise the range arithmetic,
	/// which is independent of the control template, so no template is applied.
	/// </summary>
	[TestClass]
	public class RangeSliderTests
	{
		private static RangeSlider CreateSlider(long start = 0, long stop = 100, long selectedStart = 20, long selectedStop = 80)
		{
			var slider = new RangeSlider
			{
				RangeStart = start,
				RangeStop = stop,
			};
			slider.SetSelectedRange(selectedStart, selectedStop);
			return slider;
		}

		private static List<RangeSelectionChangedEventArgs> Record(RangeSlider slider)
		{
			var events = new List<RangeSelectionChangedEventArgs>();
			slider.RangeSelectionChanged += (s, e) => events.Add(e);
			return events;
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Defaults_AreAFullySelectedZeroToOneRange()
		{
			StaTestContext.Run(() =>
			{
				var slider = new RangeSlider();

				Assert.AreEqual(0, slider.RangeStart, "range start");
				Assert.AreEqual(1, slider.RangeStop, "range stop");
				Assert.AreEqual(0, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(1, slider.RangeStopSelected, "selected stop");
				Assert.AreEqual(0, slider.MinRange, "min range");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetSelectedRange_InsideTheRange_SelectsItAndRaisesTheEvent()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();
				var events = Record(slider);

				slider.SetSelectedRange(30, 60);

				Assert.AreEqual(30, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(60, slider.RangeStopSelected, "selected stop");
				Assert.AreEqual(1, events.Count, "event count");
				Assert.AreEqual(30, events[0].NewRangeStart, "reported start");
				Assert.AreEqual(60, events[0].NewRangeStop, "reported stop");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetSelectedRange_OutsideTheRange_IsClampedToIt()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();

				slider.SetSelectedRange(-50, 500);

				Assert.AreEqual(0, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(100, slider.RangeStopSelected, "selected stop");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetSelectedRange_InvertedSelection_IsRejected()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();
				var events = Record(slider);

				slider.SetSelectedRange(60, 30);

				Assert.AreEqual(20, slider.RangeStartSelected, "selected start is untouched");
				Assert.AreEqual(80, slider.RangeStopSelected, "selected stop is untouched");
				Assert.AreEqual(0, events.Count, "no event for a rejected selection");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetSelectedRange_NarrowerThanMinRange_IsRejected()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();
				slider.MinRange = 10;

				slider.SetSelectedRange(30, 35);

				Assert.AreEqual(20, slider.RangeStartSelected, "selected start is untouched");
				Assert.AreEqual(80, slider.RangeStopSelected, "selected stop is untouched");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MinRange_Negative_Throws()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();

				Assert.ThrowsException<ArgumentOutOfRangeException>(() => slider.MinRange = -1);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MinRange_LargerThanTheCurrentSelection_WidensTheSelectionAndTheRange()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider(0, 100, 20, 30);

				slider.MinRange = 150;

				Assert.AreEqual(170, slider.RangeStopSelected, "the selection grows to honour the minimum");
				Assert.AreEqual(170, slider.RangeStop, "the range grows with it");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MoveSelection_BySpan_SlidesBothEndsAndRaisesTheEvent()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();
				var events = Record(slider);

				slider.MoveSelection(10);

				Assert.AreEqual(30, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(90, slider.RangeStopSelected, "selected stop");
				Assert.AreEqual(1, events.Count, "event count");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MoveSelection_PastTheEnd_StopsAtTheEdgeAndKeepsTheWidth()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();

				slider.MoveSelection(1000);

				Assert.AreEqual(40, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(100, slider.RangeStopSelected, "selected stop");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MoveSelection_PastTheStart_StopsAtTheEdgeAndKeepsTheWidth()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();

				slider.MoveSelection(-1000);

				Assert.AreEqual(0, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(60, slider.RangeStopSelected, "selected stop");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MoveSelection_ByZero_ChangesNothing()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider();
				var events = Record(slider);

				slider.MoveSelection(0);

				Assert.AreEqual(20, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(80, slider.RangeStopSelected, "selected stop");
				Assert.AreEqual(0, events.Count, "no event");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ZoomToSpan_Wider_GrowsTheSelectionSymmetrically()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider(0, 100, 40, 60);

				slider.ZoomToSpan(40);

				Assert.AreEqual(30, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(70, slider.RangeStopSelected, "selected stop");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ZoomToSpan_Narrower_ShrinksTheSelectionSymmetrically()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider(0, 100, 20, 80);

				slider.ZoomToSpan(20);

				Assert.AreEqual(40, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(60, slider.RangeStopSelected, "selected stop");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ZoomToSpan_AtTheRightEdge_SpillsTheGrowthToTheLeft()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider(0, 100, 90, 100);

				slider.ZoomToSpan(30);

				Assert.AreEqual(100, slider.RangeStopSelected, "the selection cannot pass the end");
				Assert.AreEqual(70, slider.RangeStartSelected, "so the whole growth goes left");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ZoomToSpan_WiderThanTheWholeRange_SelectsEverything()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider(0, 100, 40, 60);

				slider.ZoomToSpan(1000);

				Assert.AreEqual(0, slider.RangeStartSelected, "selected start");
				Assert.AreEqual(100, slider.RangeStopSelected, "selected stop");
			});
		}

		/// <summary>
		/// <c>ZoomToSpan</c> returns early when the span already matches, but it has already set
		/// its internal-update flag and never clears it, so every later selection change is
		/// treated as internal and stops raising <c>RangeSelectionChanged</c>.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void ZoomToSpan_ToTheCurrentSpan_LeavesTheSliderUnableToReportChanges()
		{
			StaTestContext.Run(() =>
			{
				var slider = CreateSlider(0, 100, 40, 60);
				var events = Record(slider);

				slider.ZoomToSpan(20);
				slider.RangeStartSelected = 10;

				Assert.AreEqual(10, slider.RangeStartSelected, "the value still changes");
				Assert.AreEqual(0, events.Count, "but no change is reported any more");
			});
		}
	}
}
