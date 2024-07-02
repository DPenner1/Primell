using System.Text;

namespace dpenner1.Primell
{
    class PrimeProgramControl
    {
        public PLProgramSettings Settings { get; }

        public PLObject EmptyVariable { get; }

        public PLObject InfEmptyVariable { get; }

        public PLObject InfNumberVariable { get; }

        public bool LastOperationWasAssignment { get; set; }

        private List<PrimellParser.LineContext> LineContexts { get; }

        public int CurrentLine { get; private set; }


        public bool ExecuteCurrentLine()
        {
            if (CurrentLine >= LineContexts.Count) return false;

            var visitor = new PrimeTermSequenceVisitor(this);
            var result = visitor.Visit(LineContexts[CurrentLine]);

            // first character always "
            switch (LineContexts[CurrentLine].outMethod()?.GetText()?.Substring(1))
            {
                case null:
                case "\"":
                    if (!LastOperationWasAssignment) WriteListResult(result);
                    break;
                case "~":
                    if (LastOperationWasAssignment) WriteListResult(result);
                    break;
                case "":
                    WriteStringResult(result);
                    break;               
                default: throw new NotImplementedException("Out method not implemented");
            }

            LastOperationWasAssignment = false;
            CurrentLine++;
            return true;
        }

        // Allows out-of-order line execution for branching.
        public PLObject ExecuteRelativeLine(int offset)
        {
            var lineNumber = CurrentLine + offset;
            while (lineNumber < 0) lineNumber += LineContexts.Count;
            lineNumber %= LineContexts.Count;

            var visitor = new PrimeTermSequenceVisitor(this);
            return visitor.Visit(LineContexts[lineNumber]);
        }

        public PLObject GetListInput()
        {
            throw new NotImplementedException();
        }

        public PLObject GetStringInput()
        {
            // TODO - this works only for single byte encodings
            // Also I don't know the behaviour for when the byte sequences are illegal

            var input = Console.ReadLine();
            var bytes = Encoding.Convert(Encoding.Unicode, Settings.InputEncoding, Encoding.Unicode.GetBytes(input));

            var values = new List<PLObject>();
            foreach (var b in bytes)
            {
                values.Add((PLNumber)b);
            }
            return new PLObject(values);
        }


        private void WriteListResult(PLObject plobj)
        {
            var str = plobj.ToString(Settings.OutputBase);

            if (!plobj.IsEmpty && !plobj.IsAtomic) str = str.Substring(1, str.Length - 2); // remove outer parentheses

            Output(str);
        }

        private void WriteStringResult(PLObject plobj)
        {
            // TODO - this works only for single byte encodings
            // Also I don't know the behaviour for when the byte sequences are illegal

            var codes = new List<byte>();
            foreach (var num in plobj.DeepCopy().Flatten(true))
            {
                codes.Add((byte)PLNumber.RoundToInteger(num.Atom.Value));
            }

            Output(Settings.SourceEncoding.GetString(codes.ToArray()));
        }

        private void Output(string output)
        {
            if (string.IsNullOrWhiteSpace(Settings.OutputFilePath))
                Console.WriteLine(output);
            else
            {
                using (var writer = new StreamWriter(Settings.OutputFilePath, true, Settings.OutputEncoding))
                {
                    writer.WriteLine(output);
                }
            }
        }

        public PrimeProgramControl(List<PrimellParser.LineContext> lineContexts, PLProgramSettings settings)
        {
            EmptyVariable = new PLObject();
            InfEmptyVariable = new PLObject(new ConstantPLGenerator(PLObject.Empty, PLNumber.PositiveInfinity));
            InfNumberVariable = new PLObject(new PrimePLGenerator(0, PLNumber.PositiveInfinity));

            LineContexts = lineContexts;
            Settings = settings;
        }
    }

    class PLProgramSettings
    {
        public bool FreeSource { get; set; }

        public BooleanDefinition TruthDefinition { get; set; }

        public int InputBase { get; set; }

        public int OutputBase { get; set; }

        public int SourceBase { get; set; }

        public Encoding InputEncoding { get; set; }

        public Encoding OutputEncoding { get; set; }

        public Encoding SourceEncoding { get; set; }

        public string SourceFilePath { get; set; } // file being run

        public string OutputFilePath { get; set; }

        public PLProgramSettings()
        {
            FreeSource = false;

            InputBase = OutputBase = SourceBase = 10;

            InputEncoding = OutputEncoding = SourceEncoding = Encoding.UTF8;

            TruthDefinition = BooleanDefinition.AllPrimes;

            SourceFilePath = OutputFilePath = "";
        }
    }

    public struct BooleanDefinition
    {
        public bool UsePrimeBoolean { get; private set; }
        public bool EmptyIsTrue { get; private set; }
        public bool RequireAllTrue { get; private set; }
        public int Id { get; private set; }


        public static readonly BooleanDefinition AllPrimes = new BooleanDefinition
        {
            UsePrimeBoolean = true,
            EmptyIsTrue = false,
            RequireAllTrue = true,
            Id = 0
        };

        public static readonly BooleanDefinition AllNonZero = new BooleanDefinition
        {
            UsePrimeBoolean = false,
            EmptyIsTrue = false,
            RequireAllTrue = true,
            Id = 1
        };

        public static readonly BooleanDefinition AllPrimesOrEmpty = new BooleanDefinition
        {
            UsePrimeBoolean = true,
            EmptyIsTrue = true,
            RequireAllTrue = true,
            Id = 2
        };

        public static readonly BooleanDefinition AllNonZeroOrEmpty = new BooleanDefinition
        {
            UsePrimeBoolean = false,
            EmptyIsTrue = true,
            RequireAllTrue = true,
            Id = 3
        };


        public static readonly BooleanDefinition AtLeastOnePrime = new BooleanDefinition
        {
            UsePrimeBoolean = true,
            EmptyIsTrue = false,
            RequireAllTrue = false,
            Id = 4
        };

        public static readonly BooleanDefinition AtLeastOneNonZero = new BooleanDefinition
        {
            UsePrimeBoolean = false,
            EmptyIsTrue = false,
            RequireAllTrue = false,
            Id = 5
        };

        public static readonly BooleanDefinition AtLeastOnePrimeOrEmpty = new BooleanDefinition
        {
            UsePrimeBoolean = true,
            EmptyIsTrue = true,
            RequireAllTrue = false,
            Id = 6
        };

        public static readonly BooleanDefinition AtLeastOneNonZeroOrEmpty = new BooleanDefinition
        {
            UsePrimeBoolean = false,
            EmptyIsTrue = true,
            RequireAllTrue = false,
            Id = 7
        };
    }
}

