using CrystalLens.Models;
using System.Collections.ObjectModel;

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
        public ObservableCollection<(byte R, byte G, byte B)> Colors { get; } = [];
        public SpriteAnimation? Animation
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public bool PlayAnimationOnLoad { get; set; }

        public SpriteViewModel()
        { }
    }
}
