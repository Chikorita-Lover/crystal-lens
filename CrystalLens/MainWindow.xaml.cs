using CrystalLens.Models;
using CrystalLens.Properties;
using CrystalLens.ViewModels;
using CrystalLens.Views;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedCommand OpenProjectCommand = new();

        internal MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenProject(string path)
        {
            ViewModel.LoadingProject = true;

            Task<ASMProject> task = Task.Run(() =>
            {
                return ASMProject.OpenProject(path);
            });

            ASMProject project = await task;

            ViewModel.LoadingProject = false;
            ViewModel.OpenProject = new(project);

            AddRecentProject(project.Path);
        }

        private ASMFile OpenFile(ASMFile.Header header, string label)
        {
            ASMFile file = ViewModel.OpenProject.Model.LoadFile(header);
            ViewModel.OpenTab(new ASMFileViewModel(file, label));
            return file;
        }

        private ASMFile OpenFile(ASMFile.Header header)
        {
            return OpenFile(header, header.Labels.First());
        }

        private async void OpenProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();

            if (dialog.ShowDialog() == true)
            {
                OpenProject(dialog.FolderName);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataTabViewModel file = e.Parameter as DataTabViewModel ?? (DataTabViewModel)tabs.SelectedItem;
            file.Save();
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataTabViewModel? item = e.Parameter as DataTabViewModel ?? (tabs.HasItems ? (DataTabViewModel)tabs.SelectedItem : null);
            e.CanExecute = item is ASMFileViewModel;
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
                result = ShowUnsavedChangesBox(text);

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

        private void CopyPath_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText((string)e.Parameter);
        }

        private void CopyRelativePath_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string? relativePath = ViewModel.OpenProject?.Model.GetRelativePath((string)e.Parameter);
            Clipboard.SetText(relativePath ?? string.Empty);
        }

        private void CopyRelativePath_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.OpenProject != null;
        }

        private void OpenInExternalProgram_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = (string)e.Parameter,
                UseShellExecute = true
            });
        }

        private void OpenInExternalProgram_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is string path && File.Exists(path);
        }

        private void ShowInExplorer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string path = (string)e.Parameter;
            if (Directory.Exists(path))
            {
                OpenInExternalProgram_Executed(sender, e);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{path}\"",
                UseShellExecute = true
            });
        }

        private void ShowInExplorer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is string path && (Directory.Exists(path) || File.Exists(path));
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            object data = item.DataContext;
            if (ProjectExplorer.SelectedItem == data)
            {
                if (data is ASMProjectViewModel.FileEntry fileEntry)
                {
                    OpenFile(fileEntry.Header);
                    // TreeViewItem expands on double-click after the callback, inverting the value set below
                    if (fileEntry.Header.Labels.Count == 1 && fileEntry.Header.Labels[0] == string.Empty)
                    {
                        item.IsExpanded ^= true;
                    }
                    else
                    {
                        item.IsExpanded = false;
                    }
                }
                else if (data is ASMProjectViewModel.LabelEntry labelEntry)
                {
                    OpenFile(labelEntry.Header, labelEntry.Label);
                }
            }
        }

        private void GridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContentGrid.ColumnDefinitions[0].Width = new GridLength(300);
        }

        private void RecentProject_Click(object sender, RoutedEventArgs e)
        {
            string? path;
            if (sender is MenuItem item && (path = item.DataContext.ToString()) != "(none)" && path != null)
            {
                OpenProject(path);
            }
        }

        private void PokemonSearch_Click(object sender, RoutedEventArgs e)
        {
            PokemonSearchOptionsWindow optionsWindow = new()
            {
                Owner = this,
                DataContext = new PokemonSearchOptions(ViewModel.DefaultSearchOptions)
            };
            if (optionsWindow.ShowDialog() == true)
            {
                ViewModel.DefaultSearchOptions = optionsWindow.ViewModel;
                ViewModel.OpenTab(new PokemonSearchViewModel(ViewModel.OpenProject.Model, optionsWindow.ViewModel));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IEnumerable<DataTabViewModel> unsavedTabs = ViewModel.OpenTabs.Where(tab => tab.HasUnsavedChanges);
            if (unsavedTabs.Any())
            {
                string text = $"One or more files have unsaved changes. Would you like to save?";
                MessageBoxResult result = ShowUnsavedChangesBox(text);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        foreach (DataTabViewModel tab in unsavedTabs)
                        {
                            tab.Save();
                        }
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private static MessageBoxResult ShowUnsavedChangesBox(string text)
        {
            return MessageBox.Show(text, "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
        }

        private static void AddRecentProject(string path)
        {
            StringCollection recentProjects = Settings.Default.RecentProjects;
            int index = recentProjects.IndexOf(path);
            if (index > 0)
            {
                recentProjects.RemoveAt(index);
            }
            if (index != 0)
            {
                recentProjects.Insert(0, path);
            }

            if (recentProjects[^1] == "(none)" || recentProjects.Count > 10)
            {
                recentProjects.RemoveAt(recentProjects.Count - 1);
            }

            App.RefreshSettings();
        }
    }
}