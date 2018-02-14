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
        public Type ValueCollectionType { get; set; }
        public override Type ValueType { get; set; }
        public Type ValueNullableType { get; set; }


        public void SetType(Type src)
        {
            if (src.IsArray)
            {
                ValueCollectionType = src;
                SetValueType(src.GetElementType());
            }
            //if (typeof(IEnumerable<>).IsAssignableFrom(src)) // open vs closed generic =(
            else if (src.IsGenericType && src.GetInterfaces()
                         .Any(t => t.IsGenericType &&
                                   t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                ValueCollectionType = src;
                SetValueType(src.GenericTypeArguments[0]);
            }
            else
            {
                ValueCollectionType = null;
                SetValueType(src);
            }

            void SetValueType(Type t)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    ValueNullableType = t;
                    ValueType = t.GenericTypeArguments[0];
                }
                else
                {
                    ValueNullableType = null;
                    ValueType = t;
                }
            }
        }


        public object DefaultValue { get; set; }
        public IFormatProvider Culture { get; set; }


        public bool ValueIsCollection => ValueCollectionType != null;


        #region ctors
        public ValuedArgument(char shortName)
            : base(shortName) { }


        public ValuedArgument(string longName)
            : base(longName) { }


        public ValuedArgument(char shortName, string longName)
            : base(shortName,
                longName) { }


        public ValuedArgument(char shortName, string longName, string description) :
            base(shortName, longName, description) { }
        #endregion


        internal override bool CanHaveValue => true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Instance of ValueType</returns>
        public virtual object DeserializeOne(string val)
        {
            object rv = Convert.ChangeType(val, ValueType, Culture ?? CultureInfo.InvariantCulture);
            return rv;
        }
    }
}