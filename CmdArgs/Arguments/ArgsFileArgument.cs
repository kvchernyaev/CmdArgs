#region usings
using System;
using System.Collections.Generic;
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
    }
}