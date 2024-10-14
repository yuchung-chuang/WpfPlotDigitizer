using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PlotDigitizer.Core
{
    /// <summary>
    /// Represents a node that can track its update status and manage dependencies with other nodes. 
    /// This node raises the <see cref="Updated"/> event when it has been successfully updated 
    /// and raises the <see cref="Outdated"/> event when it becomes outdated.
    /// 
    /// Nodes can depend on other nodes by using <see cref="DependsOn"/>. If a dependent node is updated or 
    /// outdated, the current node is automatically marked as outdated.
    /// 
    /// Subclasses should override <see cref="Update"/> to implement custom update logic and call 
    /// <see cref="OnUpdated"/> at the end of the process.
    /// </summary>
    public abstract class UpdatableNode
    {
        protected readonly ICollection<UpdatableNode> dependencies = [];

        public event EventHandler Outdated;

        public event EventHandler Updated;

        public virtual bool IsUpdated { get; set; } = false;

        protected bool CheckUpdate()
        {
            if (!IsUpdated) {
                Update();
            }
            return IsUpdated;
        }

        protected virtual void Update() { }

        protected void DependsOn(UpdatableNode node) 
		{
            dependencies.Add(node);
			node.Updated += (s, e) => OnOutdated();
			node.Outdated += (s, e) => OnOutdated();
		}

        protected bool IsAllDependenciesUpdated()
        {
            return dependencies.All(node => node.CheckUpdate());
        }

        protected void OnOutdated()
        {
            IsUpdated = false;
            Outdated?.Invoke(this, EventArgs.Empty); // broadcast the update to every dependent nodes
        }

        protected void OnUpdated()
        {
            IsUpdated = true;
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
    
    /// <summary>
    /// Represents an <see cref="UpdatableNode"/> that holds a specific data value of type 
    /// <typeparamref name="TData"/>.
    /// 
    /// The node triggers the <see cref="Updated"/> event when the data is changed via the 
    /// <see cref="Set(TData)"/> method. 
    /// 
    /// When the data is accessed through <see cref="GetUpdatedData()"/>, the node ensures it is updated by checking
    /// whether it is updated.
    /// </summary>
    /// <typeparam name="TData">The type of the data that this node holds.</typeparam>
    public abstract class UpdatableNode<TData> : UpdatableNode
	{
        private TData data;

        /// <summary>
        /// Setter calls <see cref="UpdatableNode.OnUpdated"/>. Getter does not call <see cref="UpdatableNode.CheckUpdate"/>. Use <see cref="GetUpdatedData"/> to guarantee it is updated.
        /// </summary>
        public TData Data
        {
            get => data; 
            set {
                data = value;
                OnUpdated();
            }
        }

		public TData GetUpdatedData()
		{
			CheckUpdate();
			return Data;
        }
    }
}