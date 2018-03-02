#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class SwitchArgumentAttribute : ArgumentAttribute
    {
        #region ctors
        public SwitchArgumentAttribute(char shortName)
            : base(new SwitchArgument(shortName)) { }


        public SwitchArgumentAttribute(string longName)
            : base(new SwitchArgument(longName)) { }


        public SwitchArgumentAttribute(char shortName, string longName)
            : base(new SwitchArgument(shortName, longName)) { }


        public SwitchArgumentAttribute(char shortName, string longName, string description)
            : base(new SwitchArgument(shortName, longName, description)) { }
        #endregion
    }
}