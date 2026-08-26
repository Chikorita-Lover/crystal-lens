using CrystalLens.Models;
using CrystalLens.Views;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, TimedEncounterTable> encounterTables = [];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "ASM Files|*.asm|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                StreamReader sr = new(path);
                string? line;
                Queue<ASMCommand> commands = [];
                while ((line = sr.ReadLine()) != null)
                {
                    ASMCommand command = ASMCommand.FromLine(line);
                    if (!command.Command.IsWhiteSpace())
                    {
                        commands.Enqueue(command);
                    }
                }

                encounterTables.Clear();
                while (commands.Count > 0)
                {
                    ASMCommand command = commands.Dequeue();
                    if (command.Command.EndsWith(':'))
                    {
                        continue;
                    }
                    if (command.Command == "def_grass_wildmons")
                    {
                        TimedEncounterTable encounterTable = TimedEncounterTable.ReadAssembly(commands);
                        encounterTables.Add(command.Get(0), encounterTable);

                        command = commands.Dequeue();
                        command.VerifyOrThrow("end_grass_wildmons");
                    }
                    else
                    {
                        command.VerifyOrThrow("db");
                    }
                }

                mapBox.ItemsSource = encounterTables.Keys;
                mapBox.SelectedIndex = 0;
                mapBox.IsEnabled = true;
            }
        }

        private void MapSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TimedEncounterTable encounters = encounterTables[(string)mapBox.SelectedItem];
            if (canvas.Children.Count == 1)
            {
                EncounterTableView view = new(encounters);
                canvas.Children.Add(view);
            }
            else
            {
                EncounterTableView view = (EncounterTableView)canvas.Children[1];
                view.SetEncounters(encounters);
            }
        }
    }
}