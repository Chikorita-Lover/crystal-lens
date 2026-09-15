namespace CrystalLens.ViewModels
{
    public abstract class DataTabViewModel : ObservableViewModel, IChangeTracking
    {
        protected readonly ChangeTracker tracker = new();
        
        public string Name
        {
            get; set { field = value; OnPropertyChanged(); }
        }

        ChangeTracker IChangeTracking.Tracker => tracker;

        protected DataTabViewModel()
        { }
    }
}
