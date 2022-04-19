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
			Updated?.Invoke(this, EventArgs.Empty);
			IsUpdated = true;
		}

		public bool CheckUpdate()
		{
			if (!IsUpdated) {
				Update();
			}
			return IsUpdated;
		}

		public virtual void Update() { }

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

		protected virtual void DependencyUpdated(object sender, EventArgs e)
		{
			Updated?.Invoke(this, EventArgs.Empty); // broadcast the update to every dependent nodes
			IsUpdated = false; 
		}
	}
}
