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


        internal void Apply<TArgs>(FileInfo value, CmdArgsParser<TArgs> p, Bindings<TArgs> bsTarget)
            where TArgs : new()
        {
            string[] fileCmdArgs = value.ReadFileAsArgs();

            Bindings<TArgs> bsSource = p.ParseCommandLineEgoist(fileCmdArgs, new Res<TArgs>());

            bsTarget.Merge(bsSource);
        }
    }
}