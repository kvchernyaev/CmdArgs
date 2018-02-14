#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class ValuedArgumentAttribute : ArgumentAttribute, IValuedArgument
    {
        #region ctors
        public ValuedArgumentAttribute(char shortName)
            : base(new ValuedArgument(shortName)) { }


        public ValuedArgumentAttribute(string longName)
            : base(new ValuedArgument(longName)) { }


        public ValuedArgumentAttribute(char shortName, string longName)
            : base(new ValuedArgument(shortName, longName)) { }


        public ValuedArgumentAttribute(char shortName, string longName,
            string description)
            : base(new ValuedArgument(shortName, longName, description)) { }
        #endregion


        public object DefaultValue
        {
            get => ((ValuedArgument) Argument).DefaultValue;
            set => ((ValuedArgument) Argument).DefaultValue = value;
        }


        public IFormatProvider Culture
        {
            get => ((ValuedArgument) Argument).Culture;
            set => ((ValuedArgument) Argument).Culture = value;
        }
    }
}