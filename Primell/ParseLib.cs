using System.Numerics;

namespace dpenner1.Primell
{
    static class ParseLib
    {
        public static PLNumber ParseInteger(string value, int @base)
        {
            var retval = BigInteger.Zero;
            int sign = 1; 
            if (value.StartsWith("'"))
            {
                sign = -1;
                value = value.Substring(1);
            }

            var str = new string(value.Reverse().ToArray());

            if (@base == 1) return new PLNumber(str.Length);

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                int digitValue = 0;

                if (c >= '0' && c <= '9') digitValue = c - '0';
                else if (c >= 'A' && c <= 'Z') digitValue = c - 'A' + 10;
                else if (c >= 'a' && c <= 'z')
                {
                    if (@base <= 36)
                        digitValue = c - 'a' + 10;
                    else
                        digitValue = c - 'a' + 36;
                }
                else if (c == 'Þ') digitValue = 62;
                else if (c == 'þ') digitValue = 63;
                else throw new InvalidOperationException($"Unrecognized character ({c})");

                if (digitValue >= @base) throw new InvalidOperationException($"Character ({c}) not valid in base {@base}");
                retval += digitValue * BigInteger.Pow(@base, i);
            }

            return new PLNumber(retval * sign);
        }

        public static PLOperationOptions ParseOptions(string options)
        {
            if (string.IsNullOrWhiteSpace(options)) return new PLOperationOptions();

            var retval = new PLOperationOptions()
            {
                Power = options.Contains("^"),
                Cut = options.Contains("`")
            };
            return retval;
        }

        public static PLProgramSettings ParseCommandLineSettings(string[] args, bool defaultIsFile)
        {
            var settings = new PLProgramSettings();

            UpdateSettings(settings, args, defaultIsFile);    

            return settings;
        }

        public static void UpdateSettings(PLProgramSettings settings, string[] args, bool defaultIsFile)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-b":
                    case "--base":
                        settings.InputBase = settings.OutputBase = settings.SourceBase = GetBase(args[++i]);
                        break;
                    case "-ib":
                    case "--input-base":
                        settings.InputBase = GetBase(args[++i]);
                        break;
                    case "-ob":
                    case "--output-base":
                        settings.OutputBase = GetBase(args[++i]);
                        break;
                    case "-sb":
                    case "--source-base":
                        settings.SourceBase = GetBase(args[++i]);
                        break;
                    case "-ie":
                    case "--input-encoding":
                        i++;
                        throw new NotImplementedException();
                    case "-oe":
                    case "--output-encoding":
                        i++;
                        throw new NotImplementedException();
                    case "-se":
                    case "--source-encoding":
                        i++;
                        throw new NotImplementedException();
                    case "-fs":
                    case "--free-source":
                        if (i != args.Length - 1 && !args[i + 1].StartsWith("-"))
                            settings.FreeSource = GetYesNoTrueFalse(args[++i]);
                        else settings.FreeSource = !settings.FreeSource;
                        break;
                    case "-o":
                    case "--output":
                        if (i != args.Length - 1 && !args[i + 1].StartsWith("-"))
                            settings.OutputFilePath = args[++i];
                        else
                            settings.OutputFilePath = "";
                        break;
                    case "-r":
                    case "--rounding-mode":
                        i++;
                        throw new NotImplementedException();
                    case "-p":
                    case "--precision":
                        i++;
                        throw new NotImplementedException();
                    default:
                        if (defaultIsFile)
                        {
                            settings.SourceFilePath = args[i];
                            defaultIsFile = false; // if there's more than one, we won't recognize it.
                        }
                        throw new ArgumentException($"Unrecognized argument {args[i]}");
                }
            }
        }

        private static bool GetYesNoTrueFalse(string value)
        {
            char c = char.ToLowerInvariant(value[0]);
            if (c == 'y' || c == 't' ) return true;
            if (c == 'n' || c == 'f') return false;

            throw new ArgumentException("Unrecognized Yes/No value");
        }

        private static int GetBase(string baseNum)
        {
            if (baseNum.Length == 1)
            {
                var c = baseNum[0];
                if (c >= '1' && c <= '9') return c - '0';
                if (c >= 'A' && c <= 'Z') return c - 'A' + 10;
                if (c >= 'a' && c <= 'z') return c - 'a' + 36;
                if (c == 'Þ') return 62;
                if (c == 'þ') return 63;

                return 64; // since there is no way to specify base 64 otherwise, assume so.
            }
            else // assume base 10 argument
            {
                int retval;
                if (!int.TryParse(baseNum, out retval))
                    throw new ArgumentException($"{baseNum} is an invalid base");
                if (retval <= 0)
                    throw new ArgumentException($"{baseNum} is an invalid base");
                return retval;
            }
        }
    }
}
