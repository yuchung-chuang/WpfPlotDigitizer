using Microsoft.VisualStudio.TestTools.UnitTesting;


using System;
using System.Globalization;
using System.Linq;

namespace PlotDigitizer.WPF.Tests.Utilities.ValueConverters
{
	/// <summary>
	/// The editor toolbar binds one toggle button per mode through this converter, so it has to
	/// map exactly one checked button onto the matching <see cref="EditorMode"/> singleton.
	/// </summary>
	[TestClass]
	public class EditorStateConverterTests
	{
		private readonly EditorStateConverter converter = new();

		private object Convert(params bool[] values)
			=> converter.Convert(values.Cast<object>().ToArray(), typeof(EditorMode), null, CultureInfo.InvariantCulture);

		[DataTestMethod]
		[DataRow(0)]
		[DataRow(1)]
		[DataRow(2)]
		[DataRow(3)]
		[TestCategory("Unit")]
		public void Convert_SingleCheckedButton_ReturnsMatchingModeSingleton(int index)
		{
			var values = new bool[4];
			values[index] = true;
			EditorMode[] expected = [EditorMode.PencilMode, EditorMode.EraserMode, EditorMode.RectMode, EditorMode.PolyMode];

			Assert.AreSame(expected[index], Convert(values));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_NothingChecked_ReturnsNoMode()
		{
			Assert.AreSame(EditorMode.NoMode, Convert(false, false, false, false));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_NoValues_ReturnsNoMode()
		{
			Assert.AreSame(EditorMode.NoMode, Convert());
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_MoreThanOneChecked_ReturnsNull()
		{
			Assert.IsNull(Convert(true, true, false, false));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_NonBooleanValue_ReturnsNull()
		{
			Assert.IsNull(converter.Convert([true, null, false, false], typeof(EditorMode), null, CultureInfo.InvariantCulture));
		}

		[DataTestMethod]
		[DataRow(0)]
		[DataRow(1)]
		[DataRow(2)]
		[DataRow(3)]
		[TestCategory("Unit")]
		public void ConvertBack_Mode_ChecksOnlyItsOwnButton(int index)
		{
			EditorMode[] modes = [EditorMode.PencilMode, EditorMode.EraserMode, EditorMode.RectMode, EditorMode.PolyMode];

			var result = converter.ConvertBack(modes[index], [typeof(bool)], null, CultureInfo.InvariantCulture);

			CollectionAssert.AreEqual(
				Enumerable.Range(0, 4).Select(i => (object)(i == index)).ToArray(),
				result);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_NoMode_ChecksNothing()
		{
			var result = converter.ConvertBack(EditorMode.NoMode, [typeof(bool)], null, CultureInfo.InvariantCulture);

			CollectionAssert.AreEqual(new object[] { false, false, false, false }, result);
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("PencilMode")]
		[TestCategory("Unit")]
		public void ConvertBack_NotAnEditorMode_Throws(object value)
		{
			Assert.ThrowsException<NotImplementedException>(
				() => converter.ConvertBack(value, [typeof(bool)], null, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// <see cref="EdittingState"/> shares a base class with <see cref="EditorMode"/> but is
		/// not a toolbar mode, so it is rejected as well.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_EdittingState_Throws()
		{
			Assert.ThrowsException<NotImplementedException>(
				() => converter.ConvertBack(EdittingState.NotEditting, [typeof(bool)], null, CultureInfo.InvariantCulture));
		}
	}
}
