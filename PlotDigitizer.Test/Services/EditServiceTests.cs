using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class EditServiceTests
    {
        // ---------------------------------------------------------------
        // Uninitialised state
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void IsInitialised_BeforeInitialise_ReturnsFalse()
        {
            var sut = new EditService<string>();
            Assert.IsFalse(sut.IsInitialised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CurrentObject_BeforeInitialise_Throws()
        {
            var sut = new EditService<string>();
            Assert.ThrowsException<NullReferenceException>(() => _ = sut.CurrentObject);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CurrentTag_BeforeInitialise_Throws()
        {
            var sut = new EditService<string>();
            Assert.ThrowsException<NullReferenceException>(() => _ = sut.CurrentTag);
        }

        // ---------------------------------------------------------------
        // Constructor overload
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Constructor_WithObject_InitialisesService()
        {
            var sut = new EditService<string>("hello");
            Assert.IsTrue(sut.IsInitialised);
            Assert.AreEqual("hello", sut.CurrentObject);
        }

        // ---------------------------------------------------------------
        // Initialise
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_SetsIsInitialisedTrue()
        {
            var sut = new EditService<string>();
            sut.Initialise("first");
            Assert.IsTrue(sut.IsInitialised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_SetsCurrentObjectToSuppliedValue()
        {
            var sut = new EditService<string>();
            sut.Initialise("initial");
            Assert.AreEqual("initial", sut.CurrentObject);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_SetsTagToInitialise()
        {
            var sut = new EditService<string>();
            sut.Initialise("x");
            Assert.AreEqual("initialise", sut.CurrentTag);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Initialise_SetsIndexToZero()
        {
            var sut = new EditService<string>();
            sut.Initialise("x");
            Assert.AreEqual(0, sut.Index);
        }

        // ---------------------------------------------------------------
        // CanEdit
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void CanEdit_BeforeInitialise_ReturnsFalse()
        {
            var sut = new EditService<string>();
            Assert.IsFalse(sut.CanEdit(("x", "tag")));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanEdit_AfterInitialise_ReturnsTrue()
        {
            var sut = new EditService<string>("x");
            Assert.IsTrue(sut.CanEdit(("y", "tag")));
        }

        // ---------------------------------------------------------------
        // CanUndo / CanRedo
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void CanUndo_AtIndexZero_ReturnsFalse()
        {
            var sut = new EditService<string>("x");
            Assert.IsFalse(sut.CanUndo());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanUndo_AfterOneEdit_ReturnsTrue()
        {
            var sut = new EditService<string>("x");
            sut.Edit(("y", "edit1"));
            Assert.IsTrue(sut.CanUndo());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanRedo_AtLatestEntry_ReturnsFalse()
        {
            var sut = new EditService<string>("x");
            sut.Edit(("y", "edit1"));
            Assert.IsFalse(sut.CanRedo());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanRedo_AfterUndo_ReturnsTrue()
        {
            var sut = new EditService<string>("x");
            sut.Edit(("y", "edit1"));
            sut.Undo();
            Assert.IsTrue(sut.CanRedo());
        }

        // ---------------------------------------------------------------
        // CanGoTo
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void CanGoTo_BeforeInitialise_ReturnsFalse()
        {
            var sut = new EditService<string>();
            Assert.IsFalse(sut.CanGoTo(0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanGoTo_ValidIndex_ReturnsTrue()
        {
            var sut = new EditService<string>("x");
            Assert.IsTrue(sut.CanGoTo(0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanGoTo_NegativeIndex_ReturnsFalse()
        {
            var sut = new EditService<string>("x");
            Assert.IsFalse(sut.CanGoTo(-1));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanGoTo_IndexEqualToCount_ReturnsFalse()
        {
            var sut = new EditService<string>("x");
            // Count is 1 after Initialise; index 1 is out of range
            Assert.IsFalse(sut.CanGoTo(1));
        }

        // ---------------------------------------------------------------
        // Edit — basic appending behaviour
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Edit_AppendsObjectAndAdvancesIndex()
        {
            var sut = new EditService<string>("x");
            sut.Edit(("y", "e1"));
            Assert.AreEqual(1, sut.Index);
            Assert.AreEqual("y", sut.CurrentObject);
            Assert.AreEqual("e1", sut.CurrentTag);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Edit_AfterTwoEdits_ObjectListHasThreeEntries()
        {
            var sut = new EditService<string>("x");
            sut.Edit(("y", "e1"));
            sut.Edit(("z", "e2"));
            Assert.AreEqual(3, sut.ObjectList.Count);
        }

        // ---------------------------------------------------------------
        // Edit — truncates redo branch
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Edit_AfterUndo_TruncatesRedoBranch()
        {
            var sut = new EditService<string>("a");
            sut.Edit(("b", "e1"));
            sut.Edit(("c", "e2"));
            sut.Undo();               // Index = 1, ObjectList has 3 items
            sut.Edit(("d", "branch")); // should drop "c" and append "d"

            Assert.AreEqual(3, sut.ObjectList.Count); // "a", "b", "d"
            Assert.AreEqual("d", sut.CurrentObject);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Edit_AfterUndo_TagListAlsoTruncated()
        {
            var sut = new EditService<string>("a");
            sut.Edit(("b", "e1"));
            sut.Edit(("c", "e2"));
            sut.Undo();
            sut.Edit(("d", "branch"));

            Assert.AreEqual(3, sut.TagList.Count);
        }

        // ---------------------------------------------------------------
        // Undo / Redo index movement
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void Undo_DecrementsIndex()
        {
            var sut = new EditService<string>("a");
            sut.Edit(("b", "e1"));
            sut.Undo();
            Assert.AreEqual(0, sut.Index);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Redo_IncrementsIndex()
        {
            var sut = new EditService<string>("a");
            sut.Edit(("b", "e1"));
            sut.Undo();
            sut.Redo();
            Assert.AreEqual(1, sut.Index);
        }

        // ---------------------------------------------------------------
        // GoTo
        // ---------------------------------------------------------------

        [TestMethod]
        [TestCategory("Unit")]
        public void GoTo_SetsIndexToTarget()
        {
            var sut = new EditService<string>("a");
            sut.Edit(("b", "e1"));
            sut.Edit(("c", "e2"));
            sut.GoTo(0);
            Assert.AreEqual(0, sut.Index);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GoTo_CurrentObjectReflectsNewIndex()
        {
            var sut = new EditService<string>("a");
            sut.Edit(("b", "e1"));
            sut.GoTo(0);
            Assert.AreEqual("a", sut.CurrentObject);
        }
    }
}
