#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class UnstrictlyConfArgumentAttribute : ArgumentAttribute
    {
        #region ctors
        public UnstrictlyConfArgumentAttribute(UnstrictlyConfArgument arg) 
            : base(arg) { }


        public UnstrictlyConfArgumentAttribute(char shortName)
            : base(new UnstrictlyConfArgument(shortName)) { }


        public UnstrictlyConfArgumentAttribute(string longName)
            : base(new UnstrictlyConfArgument(longName)) { }


        public UnstrictlyConfArgumentAttribute(char shortName, string longName)
            : base(new UnstrictlyConfArgument(shortName, longName)) { }


        public UnstrictlyConfArgumentAttribute(char shortName, string longName,
            string description)
            : base(new UnstrictlyConfArgument(shortName, longName, description)) { }
        #endregion
    }
}