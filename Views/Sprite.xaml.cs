using CrystalLens.Models;
using CrystalLens.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            try
            {
                BitmapImage bitmap = new(new Uri(ViewModel.Path));
                int width = ViewModel.Width = bitmap.PixelWidth;
                Int32Rect rect = new(0, frame * width, width, width);
                image.Source = new CroppedBitmap(bitmap, rect);
            }
            catch (Exception ex) when (ex is ArgumentNullException or UriFormatException or FileNotFoundException or DirectoryNotFoundException)
            {
                image.Source = null;
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
                ((SpriteViewModel)e.OldValue).PropertyChanged -= ViewModel_PropertyChanged;
            }
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Frame = 0;
            UpdateBitmap(0);
            if (ViewModel.Animation != null)
            {
                animation = CreateAnimationTimeline(ViewModel.Animation);
                PlayAnimation();
            }
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
