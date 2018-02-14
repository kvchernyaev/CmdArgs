#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class SwitchArgument : Argument
    {
        #region ctors
        public SwitchArgument(char shortName)
            : base(shortName) { }


        public SwitchArgument(string longName)
            : base(longName) { }


        public SwitchArgument(char shortName, string longName)
            : base(shortName, longName) { }


        public SwitchArgument(char shortName, string longName, string description)
            : base(shortName, longName, description) { }
        #endregion


        public override Type ValueType => typeof(bool);
        internal override bool CanHaveValue => false;
    }
}