using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    [TestClass]
    public class EditPageViewModelTests
    {
        private Model model;
        private FakeImageService imageService;
        private FakeEditService<Image<Rgba, byte>> editService;
        private EditPageViewModel vm;

        [TestInitialize]
        public void Setup()
        {
            model = new Model();
            imageService = new FakeImageService();
            editService = new FakeEditService<Image<Rgba, byte>>();
            vm = new EditPageViewModel(model, imageService, editService, null);
        }

        // ── IsEnabled ──────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenModelIsNull_ReturnsFalse()
        {
            var noModel = new EditPageViewModel(null, imageService, editService, null);
            Assert.IsFalse(noModel.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenFilteredImageIsNull_ReturnsFalse()
        {
            model.FilteredImage = null;
            Assert.IsFalse(vm.IsEnabled);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_WhenModelHasFilteredImage_ReturnsTrue()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            Assert.IsTrue(vm.IsEnabled);
        }

        // ── UndoList / RedoList — uninitialised ────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void UndoList_WhenEditServiceNotInitialised_ReturnsEmpty()
        {
            editService.IsInitialised = false;
            Assert.IsFalse(vm.UndoList.Any());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RedoList_WhenEditServiceNotInitialised_ReturnsEmpty()
        {
            editService.IsInitialised = false;
            Assert.IsFalse(vm.RedoList.Any());
        }

        // ── UndoList / RedoList — after initialise ─────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void UndoList_AfterInitialise_StartsFromIndexGoesDown()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // Initialises with Index=0, TagList=["initialise"]

            var list = vm.UndoList.ToList();

            CollectionAssert.AreEqual(new[] { "initialise" }, list);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RedoList_AfterInitialise_StartsFromIndexGoesUp()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // Index=0, TagList=["initialise"]

            var list = vm.RedoList.ToList();

            // RedoList from Index=0 to Count-1=0: just ["initialise"]
            CollectionAssert.AreEqual(new[] { "initialise" }, list);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void UndoList_AfterTwoEdits_ReturnsTagsFromIndexDownToZero()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // Index=0, TagList=["initialise"]

            // Add two edits; FakeEditService.Edit increments Index
            editService.Edit((img, "edit1")); // Index=1, TagList=["initialise","edit1"]
            editService.Edit((img, "edit2")); // Index=2, TagList=["initialise","edit1","edit2"]

            var list = vm.UndoList.ToList();

            // From Index=2 down to 0
            CollectionAssert.AreEqual(new[] { "edit2", "edit1", "initialise" }, list);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RedoList_WhenAtMiddle_ReturnsTagsFromIndexToEnd()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // Index=0

            editService.Edit((img, "edit1")); // Index=1
            editService.Edit((img, "edit2")); // Index=2
            editService.GoTo(1);             // Index=1

            var list = vm.RedoList.ToList();

            // From Index=1 to end (3 items total: "initialise","edit1","edit2")
            CollectionAssert.AreEqual(new[] { "edit1", "edit2" }, list);
        }

        // ── Command CanExecute delegates ───────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void UndoCommand_CanExecute_DelegatesToEditService()
        {
            editService.CanUndoResult = false;
            Assert.IsFalse(vm.UndoCommand.CanExecute());

            editService.CanUndoResult = true;
            Assert.IsTrue(vm.UndoCommand.CanExecute());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RedoCommand_CanExecute_DelegatesToEditService()
        {
            editService.CanRedoResult = false;
            Assert.IsFalse(vm.RedoCommand.CanExecute());

            editService.CanRedoResult = true;
            Assert.IsTrue(vm.RedoCommand.CanExecute());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GoToCommand_CanExecute_DelegatesToEditService()
        {
            editService.CanGoToResult = false;
            Assert.IsFalse(vm.GoToCommand.CanExecute(0));

            editService.CanGoToResult = true;
            Assert.IsTrue(vm.GoToCommand.CanExecute(0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void EditCommand_CanExecute_DelegatesToEditService()
        {
            editService.CanEditResult = false;
            using var img = new Image<Rgba, byte>(4, 4);
            Assert.IsFalse(vm.EditCommand.CanExecute((img, "t")));

            editService.CanEditResult = true;
            Assert.IsTrue(vm.EditCommand.CanExecute((img, "t")));
        }

        // ── UndoTo / RedoTo ────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void UndoTo_WhenIndexIsZero_DoesNotCallGoTo()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // Index=0

            vm.UndoToCommand.Execute(0);

            Assert.AreEqual(0, editService.GoToTargets.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void UndoTo_WhenIndexIsPositive_CallsGoToWithIndexMinusArg()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // Index=0
            editService.Edit((img, "e1")); // Index=1
            editService.GoToTargets.Clear();

            vm.UndoToCommand.Execute(1); // GoTo(Index - 1) = GoTo(1 - 1) = GoTo(0)

            Assert.AreEqual(1, editService.GoToTargets.Count);
            Assert.AreEqual(0, editService.GoToTargets[0]);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RedoTo_WhenIndexIsZero_DoesNotCallGoTo()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter();
            editService.GoToTargets.Clear();

            vm.RedoToCommand.Execute(0);

            Assert.AreEqual(0, editService.GoToTargets.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void RedoTo_WhenIndexIsPositive_CallsGoToWithIndexPlusArg()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // Index=0
            editService.GoToTargets.Clear();

            vm.RedoToCommand.Execute(1); // GoTo(0 + 1) = GoTo(1)

            Assert.AreEqual(1, editService.GoToTargets.Count);
            Assert.AreEqual(1, editService.GoToTargets[0]);
        }

        // ── ClearBorder ────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void ClearBorder_CallsClearBorderOnCurrentObject()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter();
            imageService.ClearBorderResult = img;

            vm.ClearBorderCommand.Execute();

            Assert.AreEqual(1, imageService.ClearBorderCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ClearBorder_RecordsEditTaggedClearBorder()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter();
            imageService.ClearBorderResult = img;

            vm.ClearBorderCommand.Execute();

            Assert.IsTrue(editService.Edits.Count > 0);
            Assert.AreEqual("Clear Border", editService.Edits[editService.Edits.Count - 1].tag);
        }

        // ── Enter ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenDisabled_DoesNotInitialiseEditService()
        {
            model.FilteredImage = null;

            vm.Enter();

            Assert.AreEqual(0, editService.InitialisedWith.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenEnabled_InitialisesEditService()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;

            vm.Enter();

            Assert.AreEqual(1, editService.InitialisedWith.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Enter_WhenAlreadyInitialised_DoesNotReinitialise()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter(); // first initialise

            vm.Enter(); // second call — should skip

            Assert.AreEqual(1, editService.InitialisedWith.Count);
        }

        // ── Leave ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenEnabled_WritesVmImageToModelEdittedImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            using var edited = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Image = edited;

            vm.Leave();

            Assert.AreSame(edited, model.EdittedImage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Leave_WhenDisabled_DoesNotWriteToModel()
        {
            model.FilteredImage = null;
            using var edited = new Image<Rgba, byte>(4, 4);
            vm.Image = edited;

            vm.Leave();

            Assert.IsNull(model.EdittedImage);
        }

        // ── EditService.PropertyChanged → UndoList/RedoList notifications ──

        [TestMethod]
        [TestCategory("Unit")]
        public void EditServiceIndexChanged_RaisesPropertyChangedForUndoList()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter();

            var changed = new List<string>();
            vm.PropertyChanged += (s, e) => changed.Add(e.PropertyName);

            editService.Edit((img, "tag1")); // triggers Index change

            CollectionAssert.Contains(changed, nameof(EditPageViewModel.UndoList));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void EditServiceIndexChanged_RaisesPropertyChangedForRedoList()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter();

            var changed = new List<string>();
            vm.PropertyChanged += (s, e) => changed.Add(e.PropertyName);

            editService.Edit((img, "tag1"));

            CollectionAssert.Contains(changed, nameof(EditPageViewModel.RedoList));
        }

        // ── Dispose ────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Dispose_UnsubscribesFromEditServicePropertyChanged()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;
            vm.Enter();

            vm.Dispose();

            // After dispose, property changed should no longer reach the vm
            var changed = new List<string>();
            vm.PropertyChanged += (s, e) => changed.Add(e.PropertyName);

            editService.RaisePropertyChanged(nameof(editService.Index));

            CollectionAssert.DoesNotContain(changed, nameof(EditPageViewModel.UndoList));
        }
    }
}
