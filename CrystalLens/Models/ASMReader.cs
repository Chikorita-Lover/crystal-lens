using System.IO;
using System.Text.RegularExpressions;

namespace CrystalLens.Models
{
    public abstract class ASMReader
    {
        protected readonly ASMProject Project;
        private readonly Stack<StreamReader> _readers = [];
        private readonly Queue<string> _queuedLines = [];

        private StreamReader? CurrentReader => _readers.Count > 0 ? _readers.Peek() : null;

        public ASMReader(ASMProject project, StreamReader reader)
        {
            Project = project;
            _readers.Push(reader);
        }

        public ASMReader(ASMProject project, string path) : this(project, new StreamReader(path))
        { }

        public void Close()
        {
            while (_readers.Count > 0)
            {
                _readers.Pop().Close();
            }
        }

        protected string? ReadLine()
        {
            string? line = _queuedLines.Count > 0 ? _queuedLines.Dequeue() : CurrentReader?.ReadLine();
            if (line == null)
            {
                if (_readers.Count > 1)
                {
                    _readers.Pop().Close();
                    return ReadLine();
                }
                else
                {
                    return null;
                }
            }

            if (ASMCommand.HasLabel(line))
            {
                string label = Regex.Match(line, "[A-Za-z_][\\w#$@]+(\\.[A-Za-z_][\\w#$@]+)?").Value;
                
                string remainder = line.Trim()[(label.Length + 1)..];
                bool isGlobal;
                if (isGlobal = remainder.StartsWith(':'))
                {
                    remainder = remainder[1..];
                }
                _queuedLines.Enqueue(remainder);
                return isGlobal ? $"{label}::" : $"{label}:";
            }
            else
            {
                string[] instructions = line.Split("::");
                string? first = null;
                foreach (string instruction in instructions)
                {
                    if (first == null)
                    {
                        first = instruction;
                    }
                    else
                    {
                        _queuedLines.Enqueue(instruction);
                    }
                }
                return first;
            }
        }

        private void Include(string path)
        {
            _readers.Push(new(Path.Combine(Project.Path, path)));
        }

        protected void ProcessCommand(ASMCommand command)
        {
            switch (command.Command)
            {
                case "INCLUDE":
                    string path = command.Get(0);
                    Include(path[1 .. (path.Length - 1)]);
                    break;
                default:
                    ProcessCustomCommand(command);
                    break;
            }
        }

        protected abstract void ProcessCustomCommand(ASMCommand command);
    }
}
