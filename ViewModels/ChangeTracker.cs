namespace CrystalLens.ViewModels
{
    public class ChangeTracker
    {
        private ChangeTracker? parent;
        private readonly List<ChangeTracker> children = [];
        public bool HasUnsavedChanges { get; private set; }

        public event EventHandler? StatusChanged;
        
        internal ChangeTracker()
        { }

        internal void AddChild(ChangeTracker child)
        {
            children.Add(child);
            child.parent = this;
        }

        internal void ClearChildren()
        {
            foreach (ChangeTracker child in children)
            {
                child.parent = null;
            }
            children.Clear();
        }

        internal void TryAddChildOf(object child)
        {
            if (child is IChangeTracking tracking)
            {
                AddChild(tracking.Tracker);
            }
        }

        internal void MarkAsUnsaved()
        {
            HasUnsavedChanges = true;
            StatusChanged?.Invoke(this, EventArgs.Empty);
            parent?.MarkAsUnsaved();
        }

        internal void MarkAsSaved()
        {
            HasUnsavedChanges = false;
            StatusChanged?.Invoke(this, EventArgs.Empty);
            foreach (ChangeTracker child in children)
            {
                child.MarkAsSaved();
            }
        }
    }
}
