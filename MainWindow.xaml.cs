using CrystalLens.Models;
using CrystalLens.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
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
                EncounterTableMap encounterTables = EncounterTableMap.ReadASM(commands);

                ((MainViewModel)DataContext).OpenFiles.Add(new ASMFileViewModel(file, encounterTables));
                tabs.SelectedIndex = tabs.Items.Count - 1;
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ((MainViewModel)DataContext).OpenFiles.RemoveAt(tabs.SelectedIndex);
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabs.HasItems;
        }
    }
}