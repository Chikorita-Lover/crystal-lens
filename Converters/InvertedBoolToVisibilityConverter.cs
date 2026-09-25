using System.Globalization;

namespace CrystalLens.Converters
{
    public class InvertedBoolToVisibilityConverter : BoolToVisibilityConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return base.Convert(!(bool)value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
