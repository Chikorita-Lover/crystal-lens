using System.Windows.Input;

namespace CrystalLens.Commands
{
    internal static class RoutedCommands
    {
        public static readonly ICommand CopyPath =
            new RoutedUICommand("Copy Path", "CopyPath", typeof(RoutedCommands));
        public static readonly ICommand CopyRelativePath =
            new RoutedUICommand("Copy Relative Path", "CopyRelativePath", typeof(RoutedCommands));
        public static readonly ICommand OpenInExternalProgram =
            new RoutedUICommand("Open in External Program", "OpenInExternalProgram", typeof(RoutedCommands));
        public static readonly ICommand ShowInExplorer =
            new RoutedUICommand("Show in Explorer", "ShowInExplorer", typeof(RoutedCommands));
    }
}
