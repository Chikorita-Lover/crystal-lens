namespace CrystalLens.ViewModels
{
    public abstract class DataTabViewModel : ObservableViewModel, IChangeTracking
    {
        protected readonly ChangeTracker tracker = new();
        
        public virtual string Name
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public bool HasUnsavedChanges
        {
            get; set { field = value; OnPropertyChanged(); }
        }

        ChangeTracker IChangeTracking.Tracker => tracker;

        protected DataTabViewModel()
        { }

        public virtual void Save()
        {
            tracker.MarkAsSaved();
        }
    }
}
