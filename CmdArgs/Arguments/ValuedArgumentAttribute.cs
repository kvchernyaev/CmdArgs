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
        public ValuedArgumentAttribute(Type type, char shortName)
            : base(new ValuedArgument(type, shortName)) { }


        public ValuedArgumentAttribute(Type type, string longName)
            : base(new ValuedArgument(type, longName)) { }


        public ValuedArgumentAttribute(Type type, char shortName, string longName)
            : base(new ValuedArgument(type, shortName, longName)) { }


        public ValuedArgumentAttribute(Type type, char shortName, string longName,
            string description)
            : base(new ValuedArgument(type, shortName, longName, description)) { }
        #endregion


        public object DefaultValue
        {
            get => ((ValuedArgument) Argument).DefaultValue;
            set => ((ValuedArgument) Argument).DefaultValue = value;
        }


        public bool ValueIsCollection
        {
            get => ((ValuedArgument) Argument).ValueIsCollection;
            set => ((ValuedArgument) Argument).ValueIsCollection = value;
        }


        public IFormatProvider Culture
        {
            get => ((ValuedArgument) Argument).Culture;
            set => ((ValuedArgument) Argument).Culture = value;
        }
    }
}