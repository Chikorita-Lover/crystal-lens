using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class SpriteViewModel : ObservableViewModel
    {
        public string Path
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public int Width
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public SpriteAnimation? Animation
        {
            get; set { field = value; OnPropertyChanged(); }
        }

        public SpriteViewModel()
        { }
    }
}
