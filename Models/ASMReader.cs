using System.IO;

namespace CrystalLens.Models
{
    public class ASMReader
    {
        private readonly ASMProject _project;
        private readonly Stack<StreamReader> _readers = [];
        private readonly Queue<object> _queue = [];

        private StreamReader? CurrentReader => _readers.Count > 0 ? _readers.Peek() : null;

        public ASMReader(ASMProject project, StreamReader reader)
        {
            _project = project;
            _readers.Push(reader);
        }

        public ASMReader(ASMProject project, string path) : this(project, new StreamReader(path))
        { }

        public void CloseChildren()
        {
            while (_readers.Count > 1)
            {
                _readers.Pop().Close();
            }
        }

        public string? Read()
        {
            if (_queue.Count == 0)
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
                if (!ASMCommand.IsLabel(line))
                {
                    ASMCommand command = ASMCommand.FromLine(line);
                    RunCommand(command);
                }
                return Read();
            }
            return _queue.Dequeue().ToString();
        }

        public byte ReadByte()
        {
            string? value = Read();
            return (byte)(int.Parse(value ?? "0") % 256);
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
                    _queue.Enqueue(command.GetByte(0));
                    _queue.Enqueue(command.GetByte(1));
                    break;
                case "endanim":
                    _queue.Enqueue(SpriteAnimation.EndAnimCommand);
                    break;
                case "setrepeat":
                    _queue.Enqueue(SpriteAnimation.SetRepeatCommand);
                    _queue.Enqueue(command.GetByte(0));
                    break;
                case "dorepeat":
                    _queue.Enqueue(SpriteAnimation.DoRepeatCommand);
                    _queue.Enqueue(command.GetByte(0));
                    break;
                default:
                    for (int i = 0; i < command.Count; i++)
                    {
                        _queue.Enqueue(command.Get(i));
                    }
                    break;
            }
        }
    }
}
