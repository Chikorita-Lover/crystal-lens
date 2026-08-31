using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CrystalLens.ViewModels
{
    public class SpriteViewModel : ObservableViewModel
    {
        public string Path
        {
            get; set { field = value; UpdateBitmap(); }
        }
        public BitmapSource Source
        {
            get; private set { field = value; OnPropertyChanged(); }
        }
        public int Width { get; private set; }

        public SpriteViewModel()
        { }

        private void UpdateBitmap()
        {
            try
            {
                BitmapImage image = new(new Uri(Path));
                Width = image.PixelWidth;
                Int32Rect rect = new(0, 0, Width, Width);
                Source = new CroppedBitmap(image, rect);
            }
            catch (UriFormatException)
            {
                Source = null;
            }
            catch (FileNotFoundException)
            {
                Source = null;
            }
        }
    }
}
