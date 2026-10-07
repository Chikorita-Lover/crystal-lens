using System.IO;
using System.Text.RegularExpressions;

namespace CrystalLens.Models
{
    public class ASMReader
    {
        private readonly ASMProject _project;
        private readonly Stack<StreamReader> _readers = [];
        private readonly List<object> _stream = [];
        private readonly Dictionary<string, int> _labelsToPositions = [];
        private int _position;

        private StreamReader? CurrentReader => _readers.Count > 0 ? _readers.Peek() : null;

        public ASMReader(ASMProject project, StreamReader reader)
        {
            _project = project;
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

        public string? Read()
        {
            if (_position >= _stream.Count)
            {
                if (CurrentReader == null)
                {
                    return null;
                }
                string? line = CurrentReader.ReadLine();
                if (line == null)
                {
                    if (_readers.Count > 1)
                    {
                        _readers.Pop().Close();
                        return Read();
                    }
                    else
                    {
                        return null;
                    }
                }
                if (ASMCommand.IsLabel(line))
                {
                    string label = Regex.Match(line, "[A-Za-z_][\\w#$@]+(\\.[A-Za-z_][\\w#$@]+)?").Value;
                    _labelsToPositions.Add(label, _stream.Count);
                }
                else
                {
                    ASMCommand command = ASMCommand.FromLine(line);
                    RunCommand(command);
                }
                return Read();
            }
            return _stream[_position++].ToString();
        }

        public byte ReadByte()
        {
            string? value = Read();
            return (byte)(int.Parse(value ?? "0") % 256);
        }

        public void JumpTo(string label)
        {
            bool hasLabel;
            while (!(hasLabel = _labelsToPositions.ContainsKey(label)) && Read() != null)
            { }
            if (!hasLabel)
            {
                throw new KeyNotFoundException("The specified label is not defined in the provided ASM file.");
            }
            _position = _labelsToPositions[label];
        }

        private void Include(string path)
        {
            _readers.Push(new(Path.Combine(_project.Path, path)));
        }

        private void RunCommand(ASMCommand command)
        {
            switch (command.Command)
            {
                case "INCLUDE":
                    string path = command.Get(0);
                    Include(path[1 .. (path.Length - 1)]);
                    break;
                case "frame":
                    _stream.Add(command.GetByte(0));
                    _stream.Add(command.GetByte(1));
                    break;
                case "endanim":
                    _stream.Add(SpriteAnimation.EndAnimCommand);
                    break;
                case "setrepeat":
                    _stream.Add(SpriteAnimation.SetRepeatCommand);
                    _stream.Add(command.GetByte(0));
                    break;
                case "dorepeat":
                    _stream.Add(SpriteAnimation.DoRepeatCommand);
                    _stream.Add(command.GetByte(0));
                    break;
                default:
                    for (int i = 0; i < command.Count; i++)
                    {
                        _stream.Add(command.Get(i));
                    }
                    break;
            }
        }
    }
}
