using CrystalLens.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrystalLens.Converters
{
    [ValueConversion(typeof(ASMProject), typeof(IList<string>))]
    public class ProjectToConstantsConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ASMProject project = (ASMProject)value;
            ASMConstantGroup constantGroup = Enum.Parse<ASMConstantGroup>((string)parameter);
            return project.Constants[constantGroup].Keys;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
