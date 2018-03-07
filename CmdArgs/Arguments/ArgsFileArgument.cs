#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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


        internal void Apply<T>(FileInfo value, CmdArgsParser<T> p, Bindings<T> bsTarget)
            where T : new()
        {
            string[] fileCmdArgs = value.ReadFileAsArgs();

            Bindings<T> bsSource = p.ParseCommandLineEgoist(fileCmdArgs, new Res<T>());

            bsTarget.Merge(bsSource);
        }
    }
}