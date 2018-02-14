#region usings
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class ValuedArgument : Argument, IValuedArgument
    {
        public override Type ValueType { get; }


        public object DefaultValue { get; set; }
        public bool ValueIsCollection { get; set; } = false;
        public IFormatProvider Culture { get; set; }


        #region ctors
        public ValuedArgument(Type valueType, char shortName)
            : base(shortName)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }


        public ValuedArgument(Type valueType, string longName)
            : base(longName)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }


        public ValuedArgument(Type valueType, char shortName, string longName)
            : base(shortName,
                longName)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }


        public ValuedArgument(Type valueType, char shortName, string longName, string description) :
            base(shortName, longName, description)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }
        #endregion


        internal override bool CanHaveValue => true;


        public virtual object Deserialize(string val)
        {
            object rv = Convert.ChangeType(val, ValueType, Culture ?? CultureInfo.InvariantCulture);
            return rv;
        }
    }
}