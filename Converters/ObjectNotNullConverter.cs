using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrystalLens.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class ObjectNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return null;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
