using CrystalLens.Models;
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
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register(
                nameof(Path),
                typeof(string),
                typeof(Sprite),
                new PropertyMetadata(string.Empty, Path_Changed)
                );
        public static readonly DependencyProperty FrameProperty =
            DependencyProperty.Register(
                nameof(Frame),
                typeof(byte),
                typeof(Sprite),
                new PropertyMetadata((byte)0, Frame_Changed)
                );
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register(
                nameof(Palette),
                typeof(List<(byte, byte, byte)>),
                typeof(Sprite),
                new PropertyMetadata(null, Palette_Changed)
                );
        public static readonly DependencyProperty ApplyPaletteProperty =
            DependencyProperty.Register(
                nameof(ApplyPalette),
                typeof(bool),
                typeof(Sprite),
                new PropertyMetadata(false, Palette_Changed)
                );
        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register(
                nameof(Animation),
                typeof(SpriteAnimation),
                typeof(Sprite),
                new PropertyMetadata(null, Animation_Changed)
                );
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }
        public byte Frame
        {
            get => (byte)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }
        public List<(byte R, byte G, byte B)>? Palette
        {
            get => (List<(byte, byte, byte)>)GetValue(PaletteProperty);
            set => SetValue(PaletteProperty, value);
        }
        public bool ApplyPalette
        {
            get => (bool)GetValue(ApplyPaletteProperty);
            set => SetValue(ApplyPaletteProperty, value);
        }
        public SpriteAnimation? Animation
        {
            get => (SpriteAnimation)GetValue(AnimationProperty);
            set => SetValue(AnimationProperty, value);
        }
        private BitmapSource? _source;
        private AnimationTimeline? _animationTimeline;

        public Sprite()
        {
            InitializeComponent();
        }

        private void UpdateBitmapSource()
        {
            if (Path.Length > 0)
            {
                try
                {
                    _source = new BitmapImage(new Uri(Path));

                    int width = _source.PixelWidth;
                    Image.Width = width;
                    Image.HorizontalAlignment = width == 48 ? HorizontalAlignment.Right : HorizontalAlignment.Center;

                    if (ApplyPalette && Palette != null)
                    {
                        BitmapPalette palette = new([.. Palette.Select(rgb => Color.FromRgb(rgb.R, rgb.G, rgb.B))]);
                        _source = CreatePaletteSwap(_source, palette);
                    }
                }
                catch (Exception ex) when (ex is ArgumentNullException or UriFormatException or FileNotFoundException or DirectoryNotFoundException)
                {
                    _source = null;
                }
            }
            else
            {
                _source = null;
            }
        }

        private void UpdateFrame(byte frame)
        {
            if (_source != null)
            {
                try
                {
                    int width = _source.PixelWidth;
                    Int32Rect rect = new(0, frame * width, width, width);
                    Image.Source = new CroppedBitmap(_source, rect);
                }
                catch (ArgumentException)
                {
                    Image.Source = null;
                }
            }
            else
            {
                Image.Source = null;
            }
        }

        public void PlayAnimation()
        {
            BeginAnimation(FrameProperty, _animationTimeline);
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

        private static void Path_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sprite sprite = (Sprite)sender;
            sprite.UpdateBitmapSource();
            sprite.Frame = 0;
            sprite.UpdateFrame(sprite.Frame);
        }

        private static void Palette_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sprite sprite = (Sprite)sender;
            sprite.UpdateBitmapSource();
            sprite.UpdateFrame(sprite.Frame);
        }

        private static void Frame_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sprite sprite = (Sprite)sender;
            sprite.UpdateFrame(sprite.Frame);
        }

        private static void Animation_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sprite sprite = (Sprite)sender;
            sprite.Frame = 0;
            sprite._animationTimeline = sprite.Animation != null ? CreateAnimationTimeline(sprite.Animation) : null;
            sprite.PlayAnimation();
        }
    }
}
