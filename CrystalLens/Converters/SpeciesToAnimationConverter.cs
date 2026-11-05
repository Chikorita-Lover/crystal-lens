using CrystalLens.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace CrystalLens.Converters
{
    public class SpeciesToAnimationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ASMProject project = (ASMProject)values[0];
            string species = (string)values[1];
            SpriteAnimation anim = project.GetAnimation(species);

            return CreateAnimationTimeline(anim);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return [DependencyProperty.UnsetValue, DependencyProperty.UnsetValue];
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
    }
}
