#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class CustomArgumentAttribute : ValuedArgumentAttribute
    {
        #region ctors
        public CustomArgumentAttribute(char shortName)
            : base(new CustomArgument(shortName)) { }


        public CustomArgumentAttribute(string longName)
            : base(new CustomArgument(longName)) { }


        public CustomArgumentAttribute(char shortName, string longName)
            :
            base(new CustomArgument(shortName, longName)) { }


        public CustomArgumentAttribute(char shortName, string longName, string description)
            : base(new CustomArgument(shortName, longName, description)) { }
        #endregion
    }
}