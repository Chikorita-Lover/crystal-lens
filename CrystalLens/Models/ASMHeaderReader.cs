using System.IO;

namespace CrystalLens.Models
{
    internal class ASMHeaderReader : ASMReader
    {
        private bool _inMacroDef;

        internal ASMHeaderReader(ASMProject project, StreamReader reader) : base(project, reader)
        { }

        internal void LoadDefinitions()
        {
            string? line;
            while ((line = ReadLine()) != null)
            {
                ASMCommand command = ASMCommand.FromLine(line);
                ProcessCommand(command.WithComment(string.Empty));
            }
        }

        protected override void ProcessCustomCommand(ASMCommand command)
        {
            switch (command.Command)
            {
                case "DEF" or "def" when !_inMacroDef:
                    string[] keywords = command.ToString().Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
                    string symbol = keywords[1];
                    string? expression = keywords.Length > 3 ? keywords[3] : null;
                    if (expression != null)
                    {
                        if (expression.StartsWith('"') && expression.EndsWith('"'))
                        {
                            expression = expression[1..(expression.Length - 1)];
                        }
                        else
                        {
                            expression = Project.Evaluate(expression).ToString();
                        }
                    }

                    switch (keywords[2].ToUpper())
                    {
                        case "=":
                            Project.Definitions[symbol] = expression;
                            break;
                        case "+=":
                            Project.Definitions[symbol] = Project.Evaluate($"{symbol} + {expression}").ToString();
                            break;
                        case "-=":
                            Project.Definitions[symbol] = Project.Evaluate($"{symbol} - {expression}").ToString();
                            break;
                        case "EQU" or "EQUS":
                            Project.Definitions.Add(symbol, expression);
                            break;
                        case "RB":
                            Project.Definitions[symbol] = Project.Definitions["_RS"];
                            Project.Definitions["_RS"] = Project.Evaluate($"_RS + {expression ?? "1"}").ToString();
                            break;
                        case "RW":
                            Project.Definitions[symbol] = Project.Definitions["_RS"];
                            Project.Definitions["_RS"] = Project.Evaluate($"_RS + ({expression ?? "1"}) * 2").ToString();
                            break;
                        case "RL":
                            Project.Definitions[symbol] = Project.Definitions["_RS"];
                            Project.Definitions["_RS"] = Project.Evaluate($"_RS + ({expression ?? "1"}) * 4").ToString();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case "const_def" when !_inMacroDef:
                    if (command.Count >= 1)
                    {
                        Project.Definitions["const_value"] = Project.Evaluate(command.Get(0)).ToString();
                    }
                    else
                    {
                        Project.Definitions["const_value"] = "0";
                    }
                    if (command.Count >= 2)
                    {
                        Project.Definitions["const_inc"] = Project.Evaluate(command.Get(1)).ToString();
                    }
                    else
                    {
                        Project.Definitions["const_inc"] = "1";
                    }
                    break;
                case "const" when !_inMacroDef:
                    Project.Definitions.Add(command.Get(0), Project.Definitions["const_value"]);
                    Project.Definitions["const_value"] = (Project.Evaluate("const_value") + 1).ToString();
                    break;
                case "shift_const" when !_inMacroDef:
                    Project.Definitions.Add(command.Get(0), (1 << int.Parse(Project.Definitions["const_value"])).ToString());
                    ProcessCommand(new("const", [$"{command.Get(0)}_F"]));
                    break;
                case "const_skip" when !_inMacroDef:
                    int increase = Project.Evaluate("const_inc");
                    if (command.Count >= 1)
                    {
                        increase *= command.GetInt(0);
                    }
                    Project.Definitions["const_value"] = (Project.Evaluate("const_value") + increase).ToString();
                    break;
                case "const_next" when !_inMacroDef:
                    Project.Definitions["const_value"] = command.Get(0);
                    break;

                case "MACRO" or "MACRO?": // TEMP
                    _inMacroDef = true;
                    break;
                case "ENDM": // TEMP
                    _inMacroDef = false;
                    break;
                case "add_tm": // TEMP
                    // constantGroup.Add($"TM_{command.Get(0)}", constValue++);
                    break;
                case "add_hm": // TEMP
                    // constantGroup.Add($"HM_{command.Get(0)}", constValue++);
                    break;
            }
        }
    }
}
