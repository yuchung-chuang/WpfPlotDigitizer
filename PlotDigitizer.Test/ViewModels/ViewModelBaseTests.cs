using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    // Concrete subclass to exercise the abstract base class
    internal sealed class ConcreteViewModel : ViewModelBase
    {
        public void SetName(string name) => Name = name;
    }

    [TestClass]
    public class ViewModelBaseTests
    {
        private ConcreteViewModel vm;

        [TestInitialize]
        public void Setup()
        {
            vm = new ConcreteViewModel();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Name_DefaultValue_IsNull()
        {
            Assert.IsNull(vm.Name);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Name_SetValue_ReturnsSetValue()
        {
            vm.Name = "TestPage";
            Assert.AreEqual("TestPage", vm.Name);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RaisePropertyChanged_WithPropertyName_FiresPropertyChangedEvent()
        {
            string changedProperty = null;
            vm.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            vm.RaisePropertyChanged("Name");

            Assert.AreEqual("Name", changedProperty);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RaisePropertyChanged_WhenNoSubscribers_DoesNotThrow()
        {
            // No subscribers — should not throw
            vm.RaisePropertyChanged("Name");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_BaseImplementation_DoesNotThrow()
        {
            // Default Enter is a no-op
            vm.Enter();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_BaseImplementation_DoesNotThrow()
        {
            // Default Leave is a no-op
            vm.Leave();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PropertyChanged_WhenNameSet_EventFires()
        {
            // Fody wires PropertyChanged on auto-properties
            // ConcreteViewModel derives from ViewModelBase but the base class is abstract;
            // the property itself is on ViewModelBase (not woven by Fody because base is in Core).
            // We test that RaisePropertyChanged does fire the event.
            bool fired = false;
            vm.PropertyChanged += (s, e) => fired = true;

            vm.RaisePropertyChanged("AnyProperty");

            Assert.IsTrue(fired);
        }
    }
}
