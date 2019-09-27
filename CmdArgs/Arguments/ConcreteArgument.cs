#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public abstract class ConcreteArgument<TArg> : ValuedArgument
    {
        #region ctors
        protected ConcreteArgument(char shortName) : base(shortName) { }
        protected ConcreteArgument(string longName) : base(longName) { }
        protected ConcreteArgument(char shortName, string longName) : base(shortName, longName) { }


        protected ConcreteArgument(char shortName, string longName, string description) : base(
            shortName,
            longName, description) { }
        #endregion


        public override void CheckFieldType(Type fieldType)
        {
            Type elemType;
            Type valueTypeMustBe = typeof(TArg);
            if (!(fieldType == valueTypeMustBe ||
                  (elemType = GetElemTypeIfCollection(fieldType)) != null && elemType == valueTypeMustBe))
                throw new ConfException(
                    $"Argument [{Name}]: field type must be {valueTypeMustBe.Name} or collection of it but {fieldType.Name} provided");
        }


        protected override bool CompareWithAllowedValue(object value, object allowedValue)
        {
            var v = (TArg) DeserializeOneOrPass(value);
            var a = (TArg) DeserializeOneOrPass(allowedValue);

            return Compare(v, a);
        }


        protected virtual bool Compare(TArg value, TArg allowedValue) =>
            object.Equals(value, allowedValue);
    }
}