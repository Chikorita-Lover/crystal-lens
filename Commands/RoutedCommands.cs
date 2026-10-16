using System.Windows.Input;

namespace CrystalLens.Commands
{
    internal static class RoutedCommands
    {
        public static readonly ICommand OpenInExplorer =
            new RoutedUICommand("Open in Explorer", "OpenInExplorer", typeof(RoutedCommands));
    }
}
