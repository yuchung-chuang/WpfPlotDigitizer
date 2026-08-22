using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;
using System;
using System.Windows.Input;

namespace PlotDigitizer.Core.Tests.Utilities
{
    [TestClass]
    public class RelayCommandTests
    {
        // ── RelayCommand (non-generic) ────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void CanExecute_NullPredicate_ReturnsTrue()
        {
            var cmd = new RelayCommand(() => { });
            Assert.IsTrue(cmd.CanExecute(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanExecute_PredicateReturnsTrue_ReturnsTrue()
        {
            var cmd = new RelayCommand(() => { }, () => true);
            Assert.IsTrue(cmd.CanExecute(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanExecute_PredicateReturnsFalse_ReturnsFalse()
        {
            var cmd = new RelayCommand(() => { }, () => false);
            Assert.IsFalse(cmd.CanExecute(null));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Execute_CallsAction()
        {
            int count = 0;
            var cmd = new RelayCommand(() => count++);
            cmd.Execute(null);
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RaiseCanExecuteChanged_FiresEvent()
        {
            var cmd = new RelayCommand(() => { });
            bool fired = false;
            cmd.CanExecuteChanged += (s, e) => fired = true;
            cmd.RaiseCanExecuteChanged();
            Assert.IsTrue(fired);
        }

        // ── RelayCommand<TParam> ─────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericCanExecute_NullPredicate_ReturnsTrue()
        {
            var cmd = new RelayCommand<int>(p => { });
            Assert.IsTrue(cmd.CanExecute(42));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericCanExecute_WithMatchingParam_InvokesPredicate()
        {
            var cmd = new RelayCommand<int>(p => { }, p => p > 0);
            Assert.IsTrue(cmd.CanExecute(5));
            Assert.IsFalse(cmd.CanExecute(-1));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericCanExecute_MismatchedParamType_ReturnsFalse()
        {
            // canAction is provided; parameter is wrong type → short-circuits to false
            var cmd = new RelayCommand<int>(p => { }, p => true);
            Assert.IsFalse(cmd.CanExecute("not an int"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericExecute_MatchingParamType_CallsAction()
        {
            int received = -1;
            var cmd = new RelayCommand<int>(p => received = p);
            cmd.Execute(42);
            Assert.AreEqual(42, received);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericExecute_MismatchedParamType_DoesNotCallAction()
        {
            int received = -1;
            var cmd = new RelayCommand<int>(p => received = p);
            cmd.Execute("not an int"); // silently does nothing
            Assert.AreEqual(-1, received);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericRaiseCanExecuteChanged_NoParam_FiresEventWithEmptyArgs()
        {
            var cmd = new RelayCommand<int>(p => { });
            EventArgs captured = null;
            cmd.CanExecuteChanged += (s, e) => captured = e;
            cmd.RaiseCanExecuteChanged();
            Assert.IsNotNull(captured);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericRaiseCanExecuteChanged_WithParam_FiresEvent()
        {
            var cmd = new RelayCommand<int>(p => { }, p => p > 0);
            bool fired = false;
            cmd.CanExecuteChanged += (s, e) => fired = true;
            cmd.RaiseCanExecuteChanged(99);
            Assert.IsTrue(fired);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GenericCanExecute_AfterRaiseWithParam_PredicateReceivesParam()
        {
            // RaiseCanExecuteChanged(param) sends a RelayCommandEventArgs;
            // CanExecute must unwrap it and invoke the predicate with its Parameter.
            int lastChecked = -1;
            var cmd = new RelayCommand<int>(p => { }, p => { lastChecked = p; return p > 0; });
            // Simulate the CanExecuteChanged handler calling CanExecute with the args object
            // by manually invoking through the ICommand interface via a subscriber.
            cmd.CanExecuteChanged += (s, e) => ((ICommand)cmd).CanExecute(e);
            cmd.RaiseCanExecuteChanged(7);
            Assert.AreEqual(7, lastChecked);
        }
    }
}
