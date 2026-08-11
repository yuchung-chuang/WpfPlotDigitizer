using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class DataTypeTests
    {
        // The underlying value of Continuous is critical because Setting.Load uses
        // the default-value check: if the source property equals default(DataType),
        // (i.e. Continuous == 0), it is skipped rather than copied.

        [TestMethod]
        [TestCategory("Unit")]
        public void Continuous_UnderlyingValue_IsZero()
        {
            Assert.AreEqual(0, (int)DataType.Continuous);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Discrete_UnderlyingValue_IsOne()
        {
            Assert.AreEqual(1, (int)DataType.Discrete);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataType_DefaultValue_IsContinuous()
        {
            DataType defaultValue = default;
            Assert.AreEqual(DataType.Continuous, defaultValue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataType_HasExactlyTwoMembers()
        {
            var names = System.Enum.GetNames(typeof(DataType));
            Assert.AreEqual(2, names.Length);
        }
    }
}
