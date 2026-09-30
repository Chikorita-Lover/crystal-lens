using CrystalLens.Models;
using CrystalLens.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for Sprite.xaml
    /// </summary>
    public partial class Sprite : UserControl
    {
        public static readonly DependencyProperty FrameProperty =
            DependencyProperty.Register(
                nameof(Frame),
                typeof(byte),
                typeof(Sprite),
                new PropertyMetadata((byte)0, Frame_Changed)
                );
        public SpriteViewModel ViewModel => (SpriteViewModel)DataContext;
        public byte Frame
        {
            get => (byte)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }
        private AnimationTimeline? animation;

        public Sprite()
        {
            InitializeComponent();

            DataContextChanged += Sprite_DataContextChanged;
        }

        private void UpdateBitmap(byte frame)
        {
            if (ViewModel != null)
            {
                try
                {
                    BitmapSource source = new BitmapImage(new Uri(ViewModel.Path));

                    int width = ViewModel.Width = source.PixelWidth;
                    Int32Rect rect = new(0, frame * width, width, width);

                    if (ViewModel.Colors.Count > 0)
                    {
                        BitmapPalette palette = new([.. ViewModel.Colors.Select(rgb => Color.FromRgb(rgb.R, rgb.G, rgb.B))]);
                        source = CreatePaletteSwap(source, palette);
                    }

                    image.Source = new CroppedBitmap(source, rect);
                }
                catch (Exception ex) when (ex is ArgumentNullException or UriFormatException or FileNotFoundException or DirectoryNotFoundException)
                {
                    image.Source = null;
                }
            }
        }

        public void PlayAnimation()
        {
            BeginAnimation(FrameProperty, animation);
        }

        private void Sprite_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                SpriteViewModel oldModel = (SpriteViewModel)e.OldValue;
                oldModel.PropertyChanged -= ViewModel_PropertyChanged;
                oldModel.Colors.CollectionChanged -= Colors_CollectionChanged;
            }
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                Frame = 0;
                UpdateBitmap(Frame);
                ViewModel.Colors.CollectionChanged += Colors_CollectionChanged;
                if (ViewModel.Animation != null)
                {
                    animation = CreateAnimationTimeline(ViewModel.Animation);
                    if (ViewModel.PlayAnimationOnLoad)
                    {
                        PlayAnimation();
                    }
                }
            }
        }

        private void Colors_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateBitmap(Frame);
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SpriteViewModel.Animation) && ViewModel.Animation != null)
            {
                animation = CreateAnimationTimeline(ViewModel.Animation);
            }
            else if (e.PropertyName == nameof(SpriteViewModel.Path))
            {
                UpdateBitmap(Frame);
            }
        }

        private static BitmapSource CreatePaletteSwap(BitmapSource source, BitmapPalette palette)
        {
            int width = source.PixelWidth;
            int height = source.PixelHeight;

            int stride = (width * source.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[stride * height];
            source.CopyPixels(pixels, stride, 0);

            return BitmapSource.Create(
                width, height, source.DpiX, source.DpiY,
                source.Format, palette, pixels, stride
                );
        }

        private static ByteAnimationUsingKeyFrames CreateAnimationTimeline(SpriteAnimation animation)
        {
            ByteAnimationUsingKeyFrames timeline = new()
            {
                SpeedRatio = 60.0
            };
            ByteKeyFrameCollection keyFrames = timeline.KeyFrames;
            int i = 0;
            int currentFrame = 0;
            int repeats = 0;
            while (i < animation.CommandCount)
            {
                SpriteAnimation.Command command = animation.Get(i);
                if (command is SpriteAnimation.Frame frameCommand)
                {
                    DiscreteByteKeyFrame keyFrame = new(frameCommand.Index, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(currentFrame)));
                    keyFrames.Add(keyFrame);
                    currentFrame += frameCommand.Duration + 1;
                }
                else if (command is SpriteAnimation.SetRepeat setRepeatCommand)
                {
                    repeats = setRepeatCommand.Count;
                }
                else if (command is SpriteAnimation.DoRepeat doRepeatCommand)
                {
                    if (--repeats > 0)
                    {
                        i = doRepeatCommand.Index - 1;
                    }
                }

                i++;
            }
            keyFrames.Add(new DiscreteByteKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(currentFrame))));

            return timeline;
        }

        private static void Frame_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sprite sprite = (Sprite)sender;
            sprite.UpdateBitmap((byte)e.NewValue);
        }
    }
}
