#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class ArgsFileArgumentAttribute : ValuedArgumentAttribute
    {
        #region ctors
        public ArgsFileArgumentAttribute(char shortName)
            : base(new ArgsFileArgument(shortName)) { }


        public ArgsFileArgumentAttribute(string longName)
            : base(new ArgsFileArgument(longName)) { }


        public ArgsFileArgumentAttribute(char shortName, string longName)
            :
            base(new ArgsFileArgument(shortName, longName)) { }


        public ArgsFileArgumentAttribute(char shortName, string longName, string description)
            : base(new ArgsFileArgument(shortName, longName, description)) { }
        #endregion
    }
}