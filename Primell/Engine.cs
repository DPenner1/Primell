using Antlr4.Runtime;

using static System.Console;

namespace dpenner1.Primell
{
    class Engine
    {
        static void Main(string[] args)
        {
            var settings = ParseLib.ParseCommandLineSettings(args, true);
            bool echo = false;

            if (!string.IsNullOrWhiteSpace(settings.SourceFilePath)) {
                new Engine().RunFromFile(settings);
            }
            else {
                WriteLine("Welcome to Prime. Enter ? for help.");

                while (true){
                    var input = ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(input)) continue; // as many blank lines as they want

                    if (input.StartsWith("?"))
                    {
                        var commandKey = ConsoleCommands.Select(x => x.Key).Where(key => key == input.Split()[0].Trim().Substring(1)).FirstOrDefault();
                        if (commandKey == null)
                        {
                            WriteLine("Unrecognized command");
                        }
                        else
                        {
                            var argument = input.Substring(input.IndexOf(commandKey) + commandKey.Length).Trim();
                            switch (commandKey)
                            {
                                case "":
                                    HelpSpiel();
                                    break;
                                case "q":
                                    return; // quit
                                case "run":
                                    settings.SourceFilePath = argument;
                                    new Engine().RunFromFile(settings);
                                    if (echo) WriteLine("Program has completed.");
                                    break;
                                case "set":
                                    var newSettings = argument.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (newSettings.Length == 0)
                                    {
                                        var output = "";
                                        int padding = 30;

                                        output += $"--input-base {settings.InputBase}".PadRight(padding);
                                        output += $"\n--output-base {settings.OutputBase}".PadRight(padding);
                                        output += $"\n--source-base {settings.SourceBase}".PadRight(padding);
                                        output += $"\n--free-source {settings.FreeSource}".PadRight(padding);
                                        output += $"\n--truth {settings.TruthDefinition.Id}".PadRight(padding);
                                        //output += $"--output {string.IsNullOrWhiteSpace(settings.OutputFilePath) ? "To Console" : settings.OutputFilePath}\n";

                                        WriteLine(output);
                                    }
                                    else
                                    {
                                        ParseLib.UpdateSettings(settings, newSettings, false);
                                        if (echo) WriteLine("Settings updated.");
                                    } 
                                    break;
                                case "echo":
                                    echo = !echo;
                                    if (echo) WriteLine("Echo turned on. on. on.");
                                    break;
                                default:
                                    throw new ArgumentException();
                            }
                        }
                    }
                    else
                    {
                        WriteLine("Unrecognized input");
                    }
                }
            }
        }

        static void HelpSpiel()
        {
            WriteLine();

            WriteLine("Commands");
            WriteLine("------------------------------");
            foreach (var command in ConsoleCommands)
            {
                int padding = ConsoleCommands.Max(x => x.Key.Length + x.ArgumentDescription.Length) + 4;
                WriteLine($"?{command.Key} {command.ArgumentDescription}".PadRight(padding) + "- " + command.Description);
            }

            WriteLine();
            WriteLine();
            WriteLine("Settings");
            WriteLine("------------------------------");
            foreach (var setting in ConsoleSettings)
            {
                WriteLine(setting.Item1);
                WriteLine("  " + setting.Item2);
                WriteLine();
            }
        }

        static List<ConsoleCommand> ConsoleCommands = new List<ConsoleCommand>
        {
            { new ConsoleCommand("", "", "Help. Which I assume you've already figured out.") },
            { new ConsoleCommand("echo", "", "Toggles echo. This provides feedback for some commands to soothe your worries.") },
            { new ConsoleCommand("q", "", "Quit Prime. Prime is sad.") },
            { new ConsoleCommand("run", "<file-path>", "Runs the given file. Note that REPL mode is not yet implemented.") },
            { new ConsoleCommand("set", "<settings-list>?", "Sets Prime to given settings list. Echoes current settings if none provided.") },
        };

        static List<Tuple<string, string>> ConsoleSettings = new List<Tuple<string, string>>
        {
            { Tuple.Create("-o <file-path>?  OR  --output <file-path>?", "Directs Prime to output to given file. If no file is specified, output is to console.") },
            { Tuple.Create("-b <base>  OR  --base <base>", "Sets the base globally to given value.") },
            { Tuple.Create("-ib <base>  OR  --input-base <base>", "Sets the base that Prime expects the user to write in for the list read (:_) operator.") },
            { Tuple.Create("-ob <base>  OR  --output-base <base>", "Sets the base that Prime outputs in.") },
            { Tuple.Create("-sb <base>  OR  --source-base <base>", "Sets the base that Prime expects the source code to be in.") },
            { Tuple.Create("-fs <Y or N>?  OR  --free-source <Y or N>?", "Sets whether non-prime values are allowed. If value not provided, toggles the current value.")},
            { Tuple.Create("-t <0 to 7>  OR  --truth <0 to 7>", "Sets what truth means for the boolean (?) operators") },
        };

        public void RunFromFile(PLProgramSettings settings)
        {
            string program;
            using (var reader = new StreamReader(settings.SourceFilePath, settings.SourceEncoding))
            {
                program = reader.ReadToEnd();
            }

            Run(program, settings);
        }

        public void Run(string program, PLProgramSettings settings)
        {
            AntlrInputStream stream = new AntlrInputStream(program);
            ITokenSource lexer = new PrimellLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            PrimellParser parser = new PrimellParser(tokens);
            parser.BuildParseTree = true;

            var allLineContexts = new List<PrimellParser.LineContext>();
            foreach (var lineContext in parser.program().line())
            {
                allLineContexts.Add(lineContext);
            }

            var programContext = new PrimeProgramControl(allLineContexts, settings);

            while (programContext.ExecuteCurrentLine()) ;
        }


        public static void RunPLNumberTests()
        {
            var nan = PLNumber.NaN;
            var pinf = PLNumber.PositiveInfinity;
            var ninf = PLNumber.NegativeInfinity;
            var pz = PLNumber.Zero;
            var nz = new PLNumber(0, -1, false);
            var num = 5;

            Console.WriteLine("--+--");
            Console.WriteLine();

            Console.WriteLine($"nan + num: {nan + num}");
            Console.WriteLine($"pinf + num: {pinf + num}");
            Console.WriteLine($"ninf + num: {ninf + num}");
            Console.WriteLine($"pz + num: {pz + num}");
            Console.WriteLine($"nz + num: {nz + num}");

            Console.WriteLine();

            Console.WriteLine($"nan + nan: {pinf + nan}");
            Console.WriteLine($"pinf + nan: {pinf + nan}");
            Console.WriteLine($"ninf + nan: {ninf + nan}");
            Console.WriteLine($"pz + nan: {pz + nan}");
            Console.WriteLine($"nz + nan: {nz + nan}");

            Console.WriteLine();

            Console.WriteLine($"pinf + pinf: {pinf + pinf}");
            Console.WriteLine($"ninf + pinf: {ninf + pinf}");
            Console.WriteLine($"pz + pinf: {pz + pinf}");
            Console.WriteLine($"nz + pinf: {nz + pinf}");

            Console.WriteLine();

            Console.WriteLine($"ninf + ninf: {ninf + ninf}");
            Console.WriteLine($"pz + ninf: {pz + ninf}");
            Console.WriteLine($"nz + ninf: {nz + ninf}");

            Console.WriteLine();

            Console.WriteLine($"pz + pz: {pz + pz}");
            Console.WriteLine($"nz + pz: {nz + pz}");
            Console.WriteLine($"nz + pz: {nz + pz}");


            Console.WriteLine("--*--");
            Console.WriteLine();

            Console.WriteLine($"nan * num: {nan * num}");
            Console.WriteLine($"pinf * num: {pinf * num}");
            Console.WriteLine($"ninf * num: {ninf * num}");
            Console.WriteLine($"pz * num: {pz * num}");
            Console.WriteLine($"nz * num: {nz * num}");

            Console.WriteLine();

            Console.WriteLine($"nan * nan: {pinf * nan}");
            Console.WriteLine($"pinf * nan: {pinf * nan}");
            Console.WriteLine($"ninf * nan: {ninf * nan}");
            Console.WriteLine($"pz * nan: {pz * nan}");
            Console.WriteLine($"nz * nan: {nz * nan}");

            Console.WriteLine();

            Console.WriteLine($"pinf * pinf: {pinf * pinf}");
            Console.WriteLine($"ninf * pinf: {ninf * pinf}");
            Console.WriteLine($"pz * pinf: {pz * pinf}");
            Console.WriteLine($"nz * pinf: {nz * pinf}");

            Console.WriteLine();

            Console.WriteLine($"ninf * ninf: {ninf * ninf}");
            Console.WriteLine($"pz * ninf: {pz * ninf}");
            Console.WriteLine($"nz * ninf: {nz * ninf}");

            Console.WriteLine();

            Console.WriteLine($"pz * pz: {pz * pz}");
            Console.WriteLine($"nz * pz: {nz * pz}");
            Console.WriteLine($"nz * pz: {nz * pz}");
        }
    }

    class ConsoleCommand
    {
        public ConsoleCommand(string key, string argumentDescription, string description)
        {
            Key = key;
            ArgumentDescription = argumentDescription;
            Description = description;
        }

        public string Key;
        public string ArgumentDescription;
        public string Description;
    }
}
