using CrystalLens.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrystalLens.Converters
{
    public class SpeciesToShinyPaletteConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ASMProject project = (ASMProject)values[0];
            string species = (string)values[1];
            GBCPalette gbcpal = project.GetShinyPalette(species);

            List<Color> colors = [Colors.White];
            colors.AddRange(gbcpal.Colors.Select(color =>
            {
                (byte r, byte g, byte b) = GBCPalette.ToRGB24(color);
                return Color.FromRgb(r, g, b);
            }));
            colors.Add(Colors.Black);

            return new BitmapPalette(colors);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return [DependencyProperty.UnsetValue, DependencyProperty.UnsetValue];
        }
    }
}
