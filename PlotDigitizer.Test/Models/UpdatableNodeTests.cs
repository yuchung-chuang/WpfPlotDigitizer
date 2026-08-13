using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.Models
{
    /// <summary>
    /// Tests for the abstract <see cref="UpdatableNode"/> and <see cref="UpdatableNode{TData}"/>
    /// base classes, exercised through a minimal concrete probe node defined inline.
    /// </summary>
    [TestClass]
    public class UpdatableNodeTests
    {
        // ──────────────────────────────────────────────────────────────────
        // Probe nodes
        // ──────────────────────────────────────────────────────────────────

        /// <summary>A leaf node that counts how many times Update() is called.</summary>
        private sealed class CountingNode : UpdatableNode<int>
        {
            public int UpdateCallCount { get; private set; }

            protected override void Update()
            {
                UpdateCallCount++;
                Data = UpdateCallCount; // sets IsUpdated via the setter
            }
        }

        /// <summary>A downstream node that depends on one upstream node.</summary>
        private sealed class DownstreamNode : UpdatableNode<string>
        {
            private readonly CountingNode upstream;

            public DownstreamNode(CountingNode upstream)
            {
                this.upstream = upstream;
                DependsOn(upstream);
            }

            protected override void Update()
            {
                if (!IsAllDependenciesUpdated())
                    return;
                Data = upstream.Data.ToString();
            }
        }

        /// <summary>
        /// A generic chain node that depends on any <see cref="UpdatableNode"/> and simply
        /// records that it was updated. Used to build multi-level chains in tests.
        /// </summary>
        private sealed class ChainNode : UpdatableNode<int>
        {
            public int UpdateCallCount { get; private set; }

            public ChainNode(UpdatableNode upstream)
            {
                DependsOn(upstream);
            }

            protected override void Update()
            {
                if (!IsAllDependenciesUpdated())
                    return;
                UpdateCallCount++;
                Data = UpdateCallCount;
            }
        }

        /// <summary>A node whose update always fails (simulates missing upstream data).</summary>
        private sealed class NeverUpdatesNode : UpdatableNode<int>
        {
            protected override void Update()
            {
                // intentionally does not assign Data / call OnUpdated
            }
        }

        /// <summary>A node that depends on a NeverUpdatesNode.</summary>
        private sealed class DependsOnStaleDependency : UpdatableNode<int>
        {
            public DependsOnStaleDependency(NeverUpdatesNode never)
            {
                DependsOn(never);
            }

            protected override void Update()
            {
                if (!IsAllDependenciesUpdated())
                    return;
                Data = 42;
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // IsUpdated initial state
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsUpdated_AfterConstruction_IsFalse()
        {
            var node = new CountingNode();
            Assert.IsFalse(node.IsUpdated);
        }

        // ──────────────────────────────────────────────────────────────────
        // Data setter / OnUpdated
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void DataSetter_WhenAssigned_SetsIsUpdatedTrue()
        {
            var node = new CountingNode();
            node.Data = 99;
            Assert.IsTrue(node.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataSetter_WhenAssigned_RaisesUpdatedEvent()
        {
            var node = new CountingNode();
            var raised = false;
            node.Updated += (s, e) => raised = true;

            node.Data = 5;

            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataSetter_WhenAssigned_DoesNotRaiseOutdatedEvent()
        {
            var node = new CountingNode();
            var outdatedRaised = false;
            node.Outdated += (s, e) => outdatedRaised = true;

            node.Data = 5;

            Assert.IsFalse(outdatedRaised);
        }

        // ──────────────────────────────────────────────────────────────────
        // CheckUpdate / GetUpdatedData laziness
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenStale_CallsUpdateOnce()
        {
            var node = new CountingNode();
            node.GetUpdatedData();
            Assert.AreEqual(1, node.UpdateCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_CalledTwiceWithoutChange_CallsUpdateOnlyOnce()
        {
            var node = new CountingNode();
            node.GetUpdatedData();
            node.GetUpdatedData();
            Assert.AreEqual(1, node.UpdateCallCount);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterUpstreamChangeMakesNodeStale_RecomputesOnNextCall()
        {
            // Build a two-level chain: upstream (CountingNode) → downstream (DownstreamNode).
            // Seed both, then change upstream so downstream goes stale, then call
            // GetUpdatedData() on downstream and verify it recomputed with the new value.
            var upstream = new CountingNode();
            var downstream = new DownstreamNode(upstream);

            upstream.GetUpdatedData();       // upstream computes: Data = 1
            downstream.GetUpdatedData();     // downstream computes: Data = "1"
            Assert.AreEqual("1", downstream.Data);

            upstream.Data = 7;               // upstream raises Updated → downstream goes stale
            Assert.IsFalse(downstream.IsUpdated, "Downstream must be stale after upstream changes.");

            var result = downstream.GetUpdatedData(); // downstream must recompute

            // DownstreamNode.Update() assigns Data = upstream.Data.ToString() = "7"
            Assert.AreEqual("7", result, "Downstream must reflect the new upstream value after recompute.");
        }

        // ──────────────────────────────────────────────────────────────────
        // DependsOn — upstream Updated propagates Outdated downstream
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void DependsOn_WhenUpstreamRaisesUpdated_DownstreamBecomesOutdated()
        {
            var upstream = new CountingNode();
            var downstream = new DownstreamNode(upstream);

            // Get both into updated state
            downstream.GetUpdatedData();

            // Now mutate upstream
            var outdatedRaised = false;
            downstream.Outdated += (s, e) => outdatedRaised = true;

            upstream.Data = 10; // raises Updated on upstream → Outdated on downstream

            Assert.IsTrue(outdatedRaised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DependsOn_WhenUpstreamRaisesUpdated_DownstreamIsUpdatedBecomeFalse()
        {
            var upstream = new CountingNode();
            var downstream = new DownstreamNode(upstream);

            downstream.GetUpdatedData(); // both updated

            upstream.Data = 10; // invalidate

            Assert.IsFalse(downstream.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DependsOn_WhenUpstreamRaisesOutdated_OutdatedCascadesToThreeLevelChain()
        {
            // Chain: nodeA (CountingNode) → nodeB (ChainNode) → nodeC (ChainNode).
            // When nodeA raises Updated, nodeB receives it and raises its own Outdated;
            // that Outdated must cascade further and raise nodeC's Outdated.
            var nodeA = new CountingNode();
            var nodeB = new ChainNode(nodeA);
            var nodeC = new ChainNode(nodeB);

            // Seed the whole chain into the updated state.
            nodeA.GetUpdatedData();
            nodeB.GetUpdatedData();
            nodeC.GetUpdatedData();
            Assert.IsTrue(nodeC.IsUpdated, "Sanity: nodeC must be updated after seeding.");

            var cOutdated = false;
            nodeC.Outdated += (s, e) => cOutdated = true;

            // Changing A raises A.Updated → B receives it and raises B.Outdated
            // → C receives B.Outdated and raises C.Outdated.
            nodeA.Data = 100;

            Assert.IsTrue(cOutdated,
                "A three-level Outdated cascade must reach nodeC when nodeA changes.");
            Assert.IsFalse(nodeC.IsUpdated,
                "nodeC must be marked stale after the cascade.");
        }

        // ──────────────────────────────────────────────────────────────────
        // IsAllDependenciesUpdated
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsAllDependenciesUpdated_WhenDependencyNeverUpdates_ReturnsFalse()
        {
            var never = new NeverUpdatesNode();
            var node = new DependsOnStaleDependency(never);

            var result = node.GetUpdatedData();

            Assert.IsFalse(node.IsUpdated);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDependencyReturnsDefault_NodeDataRemainsDefault()
        {
            var never = new NeverUpdatesNode();
            var node = new DependsOnStaleDependency(never);

            var result = node.GetUpdatedData();

            Assert.AreEqual(default(int), result);
        }
    }
}
