using System;

namespace PlotDigitizer.Core
{
	public abstract class ModelNode<TData>
	{
		public virtual bool IsUpdated { get; set; } = false;
		public TData Value { get; set; }

		public event EventHandler Updated;

		/// <summary>
		/// should be called at the end of every <see cref="Update"/> method.
		/// </summary>
		protected virtual void OnUpdated()
		{
			IsUpdated = true;
			Updated?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler Outdated;

		/// <summary>
		/// should be hooked to dependencies' <see cref="Updated"/> and <see cref="Outdated"/> events in the constructor.
		/// </summary>
		protected virtual void OnOutdated()
		{
			IsUpdated = false;
			Outdated?.Invoke(this, EventArgs.Empty); // broadcast the update to every dependent nodes
		}

		public bool CheckUpdate()
		{
			if (!IsUpdated) {
				Update();
			}
			return IsUpdated;
		}

		/// <summary>
		/// The base method should be called at the end of derived method.
		/// </summary>
		public virtual void Update()
		{
			OnUpdated();
		}

		public void Set(TData value)
		{
			Value = value;
			OnUpdated();
		}

		public TData Get()
		{
			CheckUpdate();
			return Value;
		}
	}
}