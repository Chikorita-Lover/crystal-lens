using CrystalLens.Models;
using CrystalLens.ViewModels;
using CrystalLens.Views;
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

            ViewModel.OpenTabs.Add(new ASMFileViewModel(file));
            tabs.SelectedIndex = tabs.Items.Count - 1;

            return file;
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

        private async void OpenProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();

            if (dialog.ShowDialog() == true)
            {
                ViewModel.LoadingProject = true;

                Task<ASMProject> task = Task.Run(() =>
                {
                    return ASMProject.OpenProject(dialog.FolderName);
                });

                ASMProject project = await task;

                ViewModel.LoadingProject = false;
                ViewModel.OpenProject = new(project);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataTabViewModel file = (DataTabViewModel)tabs.SelectedItem;
            file.Save();
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabs.HasItems && tabs.SelectedItem is ASMFileViewModel;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataTabViewModel tab = (DataTabViewModel)tabs.SelectedItem;
            if (tab is ASMFileViewModel file)
            {
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
                    file.Save();
                }
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataTabViewModel tab = e.Parameter as DataTabViewModel ?? (DataTabViewModel)tabs.SelectedItem;
            MessageBoxResult? result = null;
            if (tab.HasUnsavedChanges)
            {
                string text = $"{tab.Name} has unsaved changes. Would you like to save?";
                result = MessageBox.Show(text, "Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);

                if (result == MessageBoxResult.Yes)
                {
                    tab.Save();
                }
            }
            if (result != MessageBoxResult.Cancel)
            {
                ViewModel.OpenTabs.Remove(tab);
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

        private void PokemonSearch_Click(object sender, RoutedEventArgs e)
        {
            PokemonSearchOptionsWindow optionsWindow = new()
            {
                Owner = this
            };
            if (optionsWindow.ShowDialog() == true)
            {
                ViewModel.OpenTabs.Add(new PokemonSearchViewModel(ViewModel.OpenProject.Model, optionsWindow.ViewModel));
                tabs.SelectedIndex = tabs.Items.Count - 1;
            }
        }
    }
}