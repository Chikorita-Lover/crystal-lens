using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for Sprite.xaml
    /// </summary>
    public partial class Sprite : UserControl
    {
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register(
                nameof(ImagePath),
                typeof(string),
                typeof(Sprite),
                new PropertyMetadata(string.Empty, OnImagePathChanged)
            );

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public Sprite()
        {
            InitializeComponent();
        }

        private static void OnImagePathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is Sprite sprite)
            {
                try
                {
                    BitmapImage image = new(new Uri(sprite.ImagePath));
                    int size = image.PixelWidth;
                    Int32Rect rect = new(0, 0, size, size);
                    CroppedBitmap bitmap = new(image, rect);
                    sprite.image.Source = bitmap;
                    sprite.image.Width = size;
                    sprite.image.HorizontalAlignment = size == 48 ? HorizontalAlignment.Right : HorizontalAlignment.Center;
                }
                catch (UriFormatException)
                {
                    sprite.image.Source = null;
                }
                catch (FileNotFoundException)
                {
                    sprite.image.Source = null;
                }
            }
        }
    }
}
