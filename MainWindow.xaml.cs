using CrystalLens.Models;
using CrystalLens.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedCommand OpenProjectCommand = new();

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private ASMFile OpenFile(string path, ASMProject? project)
        {
            ASMFile file = ASMFile.ReadFile(path, project);

            ViewModel.OpenFiles.Add(new ASMFileViewModel(file));
            tabs.SelectedIndex = tabs.Items.Count - 1;

            return file;
        }

        private void SaveFile(ASMFileViewModel viewModel)
        {
            ((IChangeTracking)viewModel).Tracker.MarkAsSaved();
            ASMFile file = viewModel.File;
            using StreamWriter output = new(file.Path);
            file.WriteFile(output);
            output.Close();
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "ASM Files|*.asm|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                OpenFile(dialog.FileName, null);
            }
        }

        private void OpenProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();

            if (dialog.ShowDialog() == true)
            {
                ViewModel.OpenProject = new(ASMProject.OpenProject(dialog.FolderName));
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ASMFileViewModel file = (ASMFileViewModel)tabs.SelectedItem;
            SaveFile(file);
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
                SaveFile(file);
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ASMFileViewModel file = e.Parameter as ASMFileViewModel ?? (ASMFileViewModel)tabs.SelectedItem;
            MessageBoxResult? result = null;
            if (file.HasUnsavedChanges)
            {
                string text = $"{file.Name} has unsaved changes. Would you like to save?";
                result = MessageBox.Show(text, "Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
                
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile(file);
                }
            }
            if (result != MessageBoxResult.Cancel)
            {
                ViewModel.OpenFiles.Remove(file);
            }
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabs.HasItems;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ASMProjectViewModel.FileEntry fileEntry = (ASMProjectViewModel.FileEntry)((ListViewItem)sender).DataContext;
            ASMFile file = OpenFile(fileEntry.Path, ViewModel.OpenProject.Model);
        }
    }
}