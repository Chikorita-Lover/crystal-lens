using CrystalLens.Models;
using CrystalLens.Views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
        private EncounterTableMap encounterTables;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "ASM Files|*.asm|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                ASMFile file = new(dialog.FileName);
                file.ReadFile();
                Queue<ASMCommand> commands = file.GetCommands(file.GetLabels().First());
                encounterTables = EncounterTableMap.ReadASM(commands);

                TabItem tab = new()
                {
                    Header = file.Name.Replace("_", "__"),
                    Content = new EncounterTableMapView(encounterTables)
                };
                tabs.Items.Add(tab);
                tabs.SelectedIndex = tabs.Items.Count - 1;
            }
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            tabs.Items.RemoveAt(tabs.SelectedIndex);
        }

        private void CanExecuteClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabs.HasItems;
        }
    }
}