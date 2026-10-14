using CrystalLens.Properties;
using System.Windows;

namespace CrystalLens
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Settings.Default.Reload();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
        }

        internal static void RefreshSettings()
        {
            Settings.Default.Save();
            Settings.Default.Reload();
        }
    }
}
