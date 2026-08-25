using CrystalLens.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
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

                List<EncounterTable<DayTime>> encounterTables = [];
                while (commands.Count > 0)
                {
                    ASMCommand command = commands.Dequeue();
                    if (command.Command.EndsWith(':'))
                    {
                        continue;
                    }
                    if (command.Command == "def_grass_wildmons")
                    {
                        EncounterTable<DayTime> encounterTable = EncounterTable<DayTime>.ReadAssembly(commands);
                        encounterTables.Add(encounterTable);

                        command = commands.Dequeue();
                        command.VerifyOrThrow("end_grass_wildmons");
                    }
                    else
                    {
                        command.VerifyOrThrow("db");
                    }
                }

                EncounterTable<DayTime> selectedTable = encounterTables[0];
                EncounterSet encounters = selectedTable.Get(DayTime.Day);
                encounterTableDisplay.ItemsSource = encounters.Encounters;
            }
        }
    }
}