using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class SettingTests
    {
        // ---------------------------------------------------------------
        // Copy — produces independent instance
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_ReturnsNewInstance()
        {
            var src = new Setting();
            var copy = src.Copy();
            Assert.IsFalse(ReferenceEquals(src, copy));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_AxisLocation_IsCopied()
        {
            var src = new Setting { AxisLocation = new RectangleD(1, 2, 3, 4) };
            var copy = src.Copy();
            Assert.AreEqual(src.AxisLocation, copy.AxisLocation);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_AxisLimit_IsCopied()
        {
            var src = new Setting { AxisLimit = new RectangleD(10, 20, 30, 40) };
            var copy = src.Copy();
            Assert.AreEqual(src.AxisLimit, copy.AxisLimit);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_FilterMin_IsCopied()
        {
            var src = new Setting { FilterMin = new Rgba(1, 2, 3, 4) };
            var copy = src.Copy();
            Assert.AreEqual(src.FilterMin, copy.FilterMin);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_FilterMax_IsCopied()
        {
            var src = new Setting { FilterMax = new Rgba(200, 150, 100, 255) };
            var copy = src.Copy();
            Assert.AreEqual(src.FilterMax, copy.FilterMax);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_DataType_IsCopied()
        {
            var src = new Setting { DataType = DataType.Discrete };
            var copy = src.Copy();
            Assert.AreEqual(DataType.Discrete, copy.DataType);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_AxisLogBase_IsCopied()
        {
            var src = new Setting { AxisLogBase = new PointD(10, 10) };
            var copy = src.Copy();
            Assert.AreEqual(src.AxisLogBase, copy.AxisLogBase);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_AxisTitle_IsCopied()
        {
            var src = new Setting { AxisTitle = new AxisTitle("X", "Y") };
            var copy = src.Copy();
            Assert.AreEqual(src.AxisTitle, copy.AxisTitle);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_AxisTextBox_IsCopied()
        {
            var box = new AxisLimitTextBoxD
            {
                XMax = new RectangleD(1, 2, 3, 4),
                YMax = new RectangleD(5, 6, 7, 8)
            };
            var src = new Setting { AxisTextBox = box };
            var copy = src.Copy();
            Assert.AreEqual(src.AxisTextBox, copy.AxisTextBox);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Copy_IsIndependent_MutatingSourceDoesNotAffectCopy()
        {
            var src = new Setting { DataType = DataType.Discrete };
            var copy = src.Copy();
            src.DataType = DataType.Continuous;
            // copy must retain Discrete
            Assert.AreEqual(DataType.Discrete, copy.DataType);
        }

        // ---------------------------------------------------------------
        // Load — actual behaviour (BUG documented below)
        //
        // BUG: The implementation uses `if (value != default)` where `value` is
        // `object` (the return type of PropertyInfo.GetValue). For C# reference
        // semantics `default(object)` == null. All Setting properties are value
        // types (structs / enums) whose boxed representations are NEVER null.
        // Therefore Load ALWAYS overwrites the destination with the source value,
        // even when the source has a zeroed / default-valued struct.
        //
        // Intended behaviour per documentation: skip default-valued source
        // properties.  Actual behaviour: always copy.
        //
        // The tests below document CURRENT (buggy) behaviour so the orchestrator
        // can track a regression if the bug is fixed later.
        // ---------------------------------------------------------------

        // BUG: should NOT overwrite but currently does.
        [TestMethod]
        [TestCategory("Unit")]
        public void Load_DataTypeContinuous_CurrentlyOverwritesDestination()
        {
            var dest = new Setting { DataType = DataType.Discrete };
            var src = new Setting { DataType = DataType.Continuous }; // default enum (0)
            dest.Load(src);
            // CURRENT BEHAVIOUR: Load copies every non-null value; boxed enum 0 != null.
            Assert.AreEqual(DataType.Continuous, dest.DataType);
        }

        // BUG: should NOT overwrite but currently does.
        [TestMethod]
        [TestCategory("Unit")]
        public void Load_ZeroRectangleD_CurrentlyOverwritesDestination()
        {
            var dest = new Setting { AxisLocation = new RectangleD(1, 2, 3, 4) };
            var src = new Setting { AxisLocation = default }; // zeroed RectangleD
            dest.Load(src);
            // CURRENT BEHAVIOUR: boxed default(RectangleD) != null, so it is copied.
            Assert.AreEqual(default(RectangleD), dest.AxisLocation);
        }

        // BUG: should NOT overwrite but currently does.
        [TestMethod]
        [TestCategory("Unit")]
        public void Load_DefaultRgba_CurrentlyOverwritesDestination()
        {
            var dest = new Setting { FilterMin = new Rgba(10, 20, 30, 255) };
            var src = new Setting { FilterMin = default };
            dest.Load(src);
            // CURRENT BEHAVIOUR: boxed default(Rgba) != null, so it is copied.
            Assert.AreEqual(default(Rgba), dest.FilterMin);
        }

        // BUG: should NOT overwrite but currently does.
        [TestMethod]
        [TestCategory("Unit")]
        public void Load_DefaultPointD_CurrentlyOverwritesDestination()
        {
            var dest = new Setting { AxisLogBase = new PointD(10, 10) };
            var src = new Setting { AxisLogBase = default };
            dest.Load(src);
            // CURRENT BEHAVIOUR: boxed default(PointD) != null, so it is copied.
            Assert.AreEqual(default(PointD), dest.AxisLogBase);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Load_NonDefaultValue_OverwritesDestination()
        {
            var dest = new Setting { DataType = DataType.Continuous };
            var src = new Setting { DataType = DataType.Discrete };
            dest.Load(src);
            Assert.AreEqual(DataType.Discrete, dest.DataType);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Load_NonDefaultRectangleD_OverwritesDestination()
        {
            var dest = new Setting { AxisLocation = new RectangleD(1, 1, 1, 1) };
            var src = new Setting { AxisLocation = new RectangleD(5, 6, 7, 8) };
            dest.Load(src);
            Assert.AreEqual(new RectangleD(5, 6, 7, 8), dest.AxisLocation);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Load_AllPropertiesFromSource_AllCopied()
        {
            var src = new Setting
            {
                AxisLimit    = new RectangleD(900, 0, 70, 20),
                AxisLocation = new RectangleD(138, 100, 632, 399),
                FilterMin    = new Rgba(0, 0, 0, 255),
                FilterMax    = new Rgba(126, 254, 254, 255),
                DataType     = DataType.Discrete,
                AxisLogBase  = new PointD(10, 10),
                AxisTitle    = new AxisTitle("X", "Y"),
            };
            var dest = new Setting();
            dest.Load(src);
            Assert.AreEqual(DataType.Discrete,           dest.DataType);
            Assert.AreEqual(new RectangleD(900, 0, 70, 20), dest.AxisLimit);
            Assert.AreEqual(new PointD(10, 10),          dest.AxisLogBase);
            Assert.AreEqual(new AxisTitle("X", "Y"),     dest.AxisTitle);
        }

        // ---------------------------------------------------------------
        // PropertyChanged weaving
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Setting_SetDataType_RaisesPropertyChanged()
        {
            var setting = new Setting();
            string raisedProp = null;
            setting.PropertyChanged += (s, e) => raisedProp = e.PropertyName;
            setting.DataType = DataType.Discrete;
            Assert.AreEqual(nameof(Setting.DataType), raisedProp);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Setting_SetAxisLocation_RaisesPropertyChanged()
        {
            var setting = new Setting();
            string raisedProp = null;
            setting.PropertyChanged += (s, e) => raisedProp = e.PropertyName;
            setting.AxisLocation = new RectangleD(1, 2, 3, 4);
            Assert.AreEqual(nameof(Setting.AxisLocation), raisedProp);
        }
    }
}
