#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class ArgsFileArgument : FileArgument
    {
        #region ctors
        public ArgsFileArgument(char shortName)
            : base(shortName)
        {
            AllowMultiple = true;
        }


        public ArgsFileArgument(string longName)
            : base(longName)
        {
            AllowMultiple = true;
        }


        public ArgsFileArgument(char shortName, string longName)
            : base(shortName, longName)
        {
            AllowMultiple = true;
        }


        public ArgsFileArgument(char shortName, string longName, string description)
            : base(shortName, longName, description)
        {
            AllowMultiple = true;
        }
        #endregion


        public void Apply<T>(FileInfo value, CmdArgsParser<T> p, Res<T> target) 
            where T : new()
        {
            string contents = File.ReadAllText(value.FullName);
            string[] lines = contents.Split(new[] {Environment.NewLine},
                StringSplitOptions.RemoveEmptyEntries);

            string[] fileCmdArgs = lines.SelectMany(ReadArgs).ToArray();

            Res<T> rInner = p.ParseCommandLine(fileCmdArgs);

            target.Merge(rInner);
        }


        static string[] ReadArgs(string argsString)
        {
            var args = new List<string>();
            var currentArg = new StringBuilder();
            var escape = false;
            var inQuote = false;
            var hadQuote = false;
            var prevCh = '\0';
            // Iterate all characters from the input string
            foreach (char ch in argsString)
            {
                if (ch == '\\' && !escape)
                    escape = true;
                else if (ch == '\\' && escape)
                {
                    // Double backslash, keep one
                    currentArg.Append(ch);
                    escape = false;
                }
                else if (ch == '"' && !escape)
                {
                    // Toggle quoted range
                    inQuote = !inQuote;
                    hadQuote = true;
                    if (inQuote && prevCh == '"') currentArg.Append(ch);
                }
                else if (ch == '"' && escape)
                {
                    // Backslash-escaped quote, keep it
                    currentArg.Append(ch);
                    escape = false;
                }
                else if (char.IsWhiteSpace(ch) && !inQuote)
                {
                    if (escape)
                    {
                        // Add pending escape char
                        currentArg.Append('\\');
                        escape = false;
                    }
                    // Accept empty arguments only if they are quoted
                    if (currentArg.Length > 0 || hadQuote) args.Add(currentArg.ToString());
                    // Reset for next argument
                    currentArg.Clear();
                    hadQuote = false;
                }
                else
                {
                    if (escape)
                    {
                        // Add pending escape char
                        currentArg.Append('\\');
                        escape = false;
                    }
                    // Copy character from input, no special meaning
                    currentArg.Append(ch);
                }
                prevCh = ch;
            }
            // Save last argument
            if (currentArg.Length > 0 || hadQuote) args.Add(currentArg.ToString());
            return args.ToArray();
        }
    }
}