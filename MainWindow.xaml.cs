using CrystalLens.Models;
using CrystalLens.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

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
                ASMFile file = ASMFile.ReadFile(dialog.FileName);

                ViewModel.OpenFiles.Add(new ASMFileViewModel(file));
                tabs.SelectedIndex = tabs.Items.Count - 1;
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ASMFileViewModel fileVM = (ASMFileViewModel)tabs.SelectedItem;
            ((IChangeTracking)fileVM).Tracker.MarkAsSaved();
            ASMFile file = fileVM.File;
            using StreamWriter output = new(file.Path);
            file.WriteFile(output);
            output.Close();
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabs.HasItems;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ASMFileViewModel file = (ASMFileViewModel)tabs.SelectedItem;
            SaveFileDialog dialog = new()
            {
                InitialDirectory = Path.GetDirectoryName(file.Path),
                FileName = Path.GetFileNameWithoutExtension(file.Path),
                DefaultExt = "asm",
                Filter = "ASM Files|*.asm|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                file.Path = dialog.FileName;
                Save_Executed(sender, e);
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.OpenFiles.RemoveAt(tabs.SelectedIndex);
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabs.HasItems;
        }
    }
}